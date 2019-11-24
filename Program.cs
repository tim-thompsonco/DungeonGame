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
        Console.WriteLine("There is a zombie in the room. What do you want to do?");
        Console.WriteLine("Available Commands: '[F]ight' or '[C]heck Inventory'.");
        var input = Helper.GetFormattedInput();
        switch(input) {
          case "f":
            var fightEvent = new CombatHelper();
            var outcome = fightEvent.SingleCombat(monster, player);
            if (outcome == true) {

              break;
            }
            else {
              Helper.PlayerDeath();
              return;
            }
          case "c":
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Your inventory contains:\n");
            foreach (var element in player.ShowInventory()) {
              Console.WriteLine(element);
            }
            break;
          default:
            break;
        }
      }
    }
  }
}