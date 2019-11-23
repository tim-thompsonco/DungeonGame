using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
  class MainClass {
    public static void Main(string[] args) {
      Helper.GameIntro();
      var player = new NewPlayer(Helper.FetchPlayerName());
      while (true) {
        player.DisplayPlayerStats();
        Console.WriteLine("There is a zombie in the room. What do you want to do?");
        Console.WriteLine("Available Commands: '[F]ight' or '[C]heck Inventory'.");
        var input = Helper.GetFormattedInput();
        switch(input) {
          case "f":
            var outcome = MonsterEncounter(player);
            if (outcome == true) {
              break;
            }
            else {
              Helper.PlayerDeath();
              return;
            }
          case "c":
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
    static bool MonsterEncounter(NewPlayer player) {
      var zombie = new Monster();
      Console.WriteLine("{0}, you have encountered a monster. Time to fight!",
        player.GetName());
      var outcome = player.Combat(zombie);
      if (outcome == true) {
        Console.WriteLine("You have defeated the monster!");
        player.GainExperience(zombie.GiveExperience());
        player.LevelUpCheck();
        return true;
      }
      else {
        return false;
      }
    }
  }
}