using System;
using System.Threading;

namespace DungeonGame {
	class MainClass {
		public static void Main(string[] args) {
			try {
				Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			}
			catch (Exception ex) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"The game window could not be resized on your computer. Error: " + ex);
			}
			// Game loading commands
			Messages.GameIntro();
			/* Load game if save game exists, and if not, build new game
			this method must run first so rooms exists to place player in */
			GameHandler.LoadGame();
			// Load player if save game exists, and if not, create a new player
			var player = GameHandler.LoadPlayer();
			// On loading game, display room that player starts in
			OutputHandler.ShowUserOutput(player);
			OutputHandler.Display.ClearUserOutput();
			// Check every second to see if any effects expired or events need to execute
			var globalTimer = new Timer(
				e => GameHandler.CheckStatus(player), 
				null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			while (!GameHandler.IsGameOver) {
				var input = InputHandler.GetFormattedInput(Console.ReadLine());
				InputHandler.ProcessUserInput(player, input, globalTimer);
				Console.Clear();
				OutputHandler.ShowUserOutput(player);
				OutputHandler.Display.ClearUserOutput();
				if (player.HitPoints > 0) continue;
				/* If player dies, provide option to continue playing. If there is a saved game, player can reload
				 from it. Otherwise, player can start over and create a new game. */
				if (GameHandler.ContinuePlaying()) {
					GameHandler.LoadGame();
					player = GameHandler.LoadPlayer();
					RoomHandler.SetPlayerLocation(player, player.X, player.Y, player.Z);
					RoomHandler.Rooms[RoomHandler.RoomIndex].LookRoom();
					OutputHandler.ShowUserOutput(player);
					OutputHandler.Display.ClearUserOutput();
				}
				else {
					GameHandler.IsGameOver = true;
					Messages.GameOver();
					OutputHandler.Display.RetrieveUserOutput();
					OutputHandler.Display.ClearUserOutput();
				}
			}
		}
	}
}