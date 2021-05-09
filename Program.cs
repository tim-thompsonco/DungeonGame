using DungeonGame.Helpers;
using DungeonGame.Players;
using System;
using System.Threading;

namespace DungeonGame {
	internal class MainClass {
		public static void Main() {
			try {
				Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			} catch (Exception ex) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					$"The game window could not be resized on your computer. Error: {ex}");
			}
			// Game loading commands
			Messages.ShowGameIntro();
			OutputHelper.Display.RetrieveUserOutput();
			OutputHelper.Display.ClearUserOutput();
			/* Load game if save game exists, and if not, build new game
			this method must run first so rooms exists to place player in */
			GameHelper.LoadGame();
			// Load player if save game exists, and if not, create a new player
			Player player = GameHelper.LoadPlayer();
			// On loading game, display room that player starts in
			OutputHelper.ShowUserOutput(player);
			OutputHelper.Display.ClearUserOutput();
			// Check every second to see if any effects expired or events need to execute
			Timer globalTimer = new Timer(
				e => GameHelper.CheckStatus(player),
				null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			while (!GameHelper.IsGameOver) {
				GameHelper.RemovedExpiredEffectsAsync(player);
				string[] input = InputHelper.GetFormattedInput(Console.ReadLine());
				InputHelper.ProcessUserInput(player, input, globalTimer);
				Console.Clear();
				OutputHelper.ShowUserOutput(player);
				OutputHelper.Display.ClearUserOutput();
				if (player.HitPoints > 0) {
					continue;
				}
				/* If player dies, provide option to continue playing. If there is a saved game, player can reload
				from it. Otherwise, player can start over and create a new game. */
				if (GameHelper.ContinuePlaying()) {
					GameHelper.LoadGame();
					player = GameHelper.LoadPlayer();
					RoomHelper._Rooms[player.PlayerLocation].LookRoom();
					OutputHelper.ShowUserOutput(player);
					OutputHelper.Display.ClearUserOutput();
				} else {
					GameHelper.IsGameOver = true;
					Messages.GameOver();
					OutputHelper.Display.RetrieveUserOutput();
					OutputHelper.Display.ClearUserOutput();
				}
			}
		}
	}
}