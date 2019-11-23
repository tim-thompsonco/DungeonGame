using System;

namespace DungeonGame {
  public static class Helper {
    public static string GetFormattedInput() {
      var input = Console.ReadLine();
      var inputFormatted = input.ToLower();
      return inputFormatted;
    }
    public static void RequestCommand() {
      Console.Write("Your command: ");
    }
    public static void PlayerDeath() {
      Console.WriteLine("You have died. Game over.");
    }
		public static void GameIntro() {
			Console.WriteLine("Welcome to Dungeon Game! Please enter player details: ");
		}
		public static string FetchPlayerName() {
			while(true) {
				Console.Write("Player name: ");
				var playerName = Console.ReadLine();
				Console.WriteLine("Your player name is {0}, is that correct? [Y] or [N].", playerName);
				RequestCommand();
				var input = GetFormattedInput();
				if(input == "y") {
					return playerName;
				}
			}
		}
  }
}