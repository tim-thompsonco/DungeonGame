using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
  class MainClass {
    public static void Main(string[] args) {
      Console.ForegroundColor = ConsoleColor.Gray;
      Helper.GameIntro();
      var player = new NewPlayer(Helper.FetchPlayerName());
      var monster = new Monster();
      while (true) {
        player.DisplayPlayerStats();
				if (monster.IsAlive()) {
					Console.WriteLine("There is a zombie in the room. What do you want to do?");
				}
        else {
					Console.WriteLine("There's a dead zombie in the room (yeah that's kind of repetitive)" +
						". What do you want to do?");
				}
        Console.Write("Available Commands: ");
				Console.WriteLine(String.Join(", ", Helper.Commands));
        var input = Helper.GetFormattedInput();
        switch(input) {
          case "f":
						if (monster.IsAlive()) {
							var fightEvent = new CombatHelper();
							var outcome = fightEvent.SingleCombat(monster, player);
							if (outcome == true) {
								break;
							}
							else {
								Helper.PlayerDeath();
								return;
							}
						}
						else {
							Console.WriteLine("The monster is already dead.");
							break;
						}
          case "i":
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Your inventory contains:\n");
            foreach (var element in player.Inventory) {
              Console.WriteLine(element);
            }
						Console.WriteLine(); // Add extra blank line after inventory display for formatting
            break;
					case "q":
						Console.WriteLine("Game over. You win!");
						return;
          default:
            break;
        }
      }
    }
  }
}