using DungeonGame.Controllers;
using DungeonGame.Players;
using System;
using System.Threading;

namespace DungeonGame {
	internal class MainClass {
		public static void Main() {
			try {
				Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			} catch (Exception ex) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					$"The game window could not be resized on your computer. Error: {ex}");
			}
			// Game loading commands
			Messages.ShowGameIntro();
			OutputController.Display.RetrieveUserOutput();
			OutputController.Display.ClearUserOutput();
			/* Load game if save game exists, and if not, build new game
			this method must run first so rooms exists to place player in */
			GameController.LoadGame();
			// Load player if save game exists, and if not, create a new player
			Player player = GameController.LoadPlayer();
			// On loading game, display room that player starts in
			OutputController.ShowUserOutput(player);
			OutputController.Display.ClearUserOutput();
			// Check every second to see if any effects expired or events need to execute
			Timer globalTimer = new Timer(
				e => GameController.CheckStatus(player),
				null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			while (!GameController.IsGameOver) {
				GameController.RemovedExpiredEffectsAsync(player);
				string[] input = InputController.GetFormattedInput(Console.ReadLine());
				InputController.ProcessUserInput(player, input, globalTimer);
				Console.Clear();
				OutputController.ShowUserOutput(player);
				OutputController.Display.ClearUserOutput();
				if (player.HitPoints > 0) {
					continue;
				}
				/* If player dies, provide option to continue playing. If there is a saved game, player can reload
				from it. Otherwise, player can start over and create a new game. */
				if (GameController.ContinuePlaying()) {
					GameController.LoadGame();
					player = GameController.LoadPlayer();
					RoomController._Rooms[player.PlayerLocation].LookRoom();
					OutputController.ShowUserOutput(player);
					OutputController.Display.ClearUserOutput();
				} else {
					GameController.IsGameOver = true;
					Messages.GameOver();
					OutputController.Display.RetrieveUserOutput();
					OutputController.Display.ClearUserOutput();
				}
			}
		}
	}
}