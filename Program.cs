using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame
{
  class MainClass {
    public static void Main(string[] args) {
      var player = new Player();
      MonsterEncounter(player);
    }
		static void MonsterEncounter(Player player) {
			var zombie = new Monster();
			Console.WriteLine("You encountered a monster. What do you do? '[F]ight' would be a good idea.");
			Console.Write("Your command: ");
      var input = Helper.GetFormattedInput();
      if (input == "f") {
        player.Combat(zombie);
      }
      if (player.CheckHealth() <= 0) {
        Console.WriteLine("You have died.");
				return;
      }
		}
  }
	public class Helper {
		public static string GetFormattedInput() {
			var input = Console.ReadLine();
      var inputFormatted = input.ToLower();
			return inputFormatted;
		}
	}
}