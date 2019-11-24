using System;

namespace DungeonGame {
  public static class Helper {
		public static String[] Commands { get; set; } = new String[3] { "Check [I]nventory", "[F]ight", "[Q]uit" };

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
			Console.WriteLine(
				"Welcome to Dungeon Game v0.1! This is a text-based combat simulator where you fight one zombie in a room. " +
				"Over time, the game will be updated to allow you to move around, fight monsters, get loot and do all of " +
				"the typical stuff you can do in an RPG game.\n");
			Console.WriteLine("For now, please enter a player name.\n");
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
    public static void InvalidCommand() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine("Not a valid command.");
    } 
  }
}