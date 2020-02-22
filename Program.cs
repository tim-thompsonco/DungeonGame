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
			}
		}
	}
}