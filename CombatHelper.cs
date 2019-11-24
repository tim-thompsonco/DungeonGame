using System;

namespace DungeonGame {
  public class CombatHelper {
    public bool SingleCombat(Monster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("{0}, you have encountered a monster. Time to fight!",
        player.GetName());
      while (true) {
        player.DisplayPlayerStats();
        opponent.DisplayStats();
        Console.WriteLine("Commands: '[A]ttack'");
        Helper.RequestCommand();
        var input = Helper.GetFormattedInput();
        Console.WriteLine(); // To add a blank space between the command and fight sequence
        switch (input) {
          case "a":
            var attackDamage = player.Attack();
            if (attackDamage == 0) {
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine("You missed!");
            }
            else {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("You hit the monster for {0} damage.", attackDamage);
              opponent.TakeDamage(attackDamage);
            }
            if (opponent.CheckHealth() <= 0) {
              this.SingleCombatWin(opponent, player);
              return true;
            }
            var attackDamageM = opponent.Attack();
            if (attackDamageM == 0) {
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine("They missed!");
            }
            else {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("The monster hits you for {0} damage.", attackDamageM - player.ArmorRating());
              player.TakeDamage(attackDamageM - player.ArmorRating());
              if (player.CheckHealth() <= 0) {
                return false;
              }
            }
            Console.WriteLine(); // To add a blank space between the command and fight sequence
            break;
          default:
            Helper.InvalidCommand();
            break;
        }
      }
    }
    public void SingleCombatWin(Monster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("You have defeated the {0}!", opponent.GetName());
      // setting opponent to null creates NullReferenceException error
      // for program.cs, add a try/catch for the fight option and have
      // it tell user that monster is already dead if exception triggers
      // since it would only trigger when monster is dead eg object destroyed
      // opponent = null;
      player.GainExperience(opponent.GiveExperience());
      player.LevelUpCheck();
    }
  }
}
