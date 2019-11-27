using System;

namespace DungeonGame {
  public class CombatHelper {
		public String[] Commands { get; set; } = new String[2] { "[A]ttack", "[C]ast [F]ireball" };

		public bool SingleCombat(IMonster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("{0}, you have encountered a {1}. Time to fight!",
        player.Name, opponent.Name);
      while (true) {
        player.DisplayPlayerStats();
        opponent.DisplayStats();
				Console.Write("Available Commands: ");
				Console.WriteLine(String.Join(", ", this.Commands));
				Helper.RequestCommand();
        var input = Helper.GetFormattedInput();
        Console.WriteLine(); // To add a blank space between the command and fight sequence
        switch (input) {
          case "a":
            var attackDamage = 0;
            attackDamage = player.Attack();
            if (attackDamage == 0) {
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine("You missed!");
            }
            else {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("You hit the {0} for {1} physical damage.", opponent.Name, attackDamage);
              opponent.TakeDamage(attackDamage);
            }
            if (opponent.HitPoints <= 0) {
              this.SingleCombatWin(opponent, player);
              return true;
            }
            break;
					case "cf":
            attackDamage = player.CastFireball();
            if (attackDamage == 0) {
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine("You missed!");
            }
            else {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("You hit the {0} for {1} fire damage.", opponent.Name, attackDamage);
              opponent.TakeDamage(attackDamage);
							Console.ForegroundColor = ConsoleColor.Yellow;
							Console.WriteLine("The {0} bursts into flame!", opponent.Name);
							opponent.OnFire = true;
            }
            if (opponent.HitPoints <= 0) {
              this.SingleCombatWin(opponent, player);
              return true;
            }
            break;
          default:
            Helper.InvalidCommand();
            break;
        }
				if (opponent.OnFire) {
					var burnDamage = player.FireballBurnDamage();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("The {0} burns for {1} fire damage.", opponent.Name, burnDamage);
					opponent.TakeDamage(burnDamage);
				}
				var attackDamageM = opponent.Attack();
        if (attackDamageM == 0) {
          Console.ForegroundColor = ConsoleColor.DarkRed;
          Console.WriteLine("The {0} missed you!", opponent.Name);
        }
        else {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("The {0} hits you for {1} physical damage.",
            opponent.Name, attackDamageM - player.ArmorRating());
          player.TakeDamage(attackDamageM - player.ArmorRating());
          if (player.HitPoints <= 0) {
            return false;
          }
        }
      }
    }
    public void SingleCombatWin(IMonster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("You have defeated the {0}!", opponent.Name);
			player.GainExperience(opponent.ExperienceProvided);
      // opponent.Name = "dead " + opponent.Name;
      player.LevelUpCheck();
		}
  }
}
