using System;

namespace DungeonGame {
  public class CombatHelper {
		public String[] Commands { get; set; } = new String[2] { "[A]ttack", "[C]ast [F]ireball" };

		public bool SingleCombat(IMonster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("{0}, you have encountered a {1}. Time to fight!",
        player.name, opponent.name);
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
              Console.WriteLine("You hit the {0} for {1} physical damage.", opponent.name, attackDamage);
              opponent.TakeDamage(attackDamage);
            }
            if (opponent.hitPoints <= 0) {
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
              Console.WriteLine("You hit the {0} for {1} fire damage.", opponent.name, attackDamage);
              opponent.TakeDamage(attackDamage);
							Console.ForegroundColor = ConsoleColor.Yellow;
							Console.WriteLine("The {0} bursts into flame!", opponent.name);
							opponent.onFire = true;
            }
            if (opponent.hitPoints <= 0) {
              this.SingleCombatWin(opponent, player);
              return true;
            }
            break;
          default:
            Helper.InvalidCommand();
            break;
        }
				if (opponent.onFire) {
					var burnDamage = player.FireballBurnDamage();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("The {0} burns for {1} fire damage.", opponent.name, burnDamage);
					opponent.TakeDamage(burnDamage);
					player._player_Spell.burnCurRounds += 1;
				}
				if (player._player_Spell.burnCurRounds > player._player_Spell.burnMaxRounds) {
					opponent.onFire = false;
					player._player_Spell.burnCurRounds = 1;
				}
				var attackDamageM = opponent.Attack();
				if (attackDamageM - player.ArmorRating() < 0) {
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("Your armor absorbed all of {0}'s attack!", opponent.name);
					attackDamageM = 0;
				}
        else if (attackDamageM == 0) {
          Console.ForegroundColor = ConsoleColor.DarkRed;
          Console.WriteLine("The {0} missed you!", opponent.name);
        }
        else {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("The {0} hits you for {1} physical damage.",
            opponent.name, attackDamageM - player.ArmorRating());
          player.TakeDamage(attackDamageM - player.ArmorRating());
          if (player.hitPoints <= 0) {
            return false;
          }
        }
      }
    }
    public void SingleCombatWin(IMonster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("You have defeated the {0}!", opponent.name);
			player.GainExperience(opponent.experienceProvided);
      // opponent.Name = "dead " + opponent.Name;
      player.LevelUpCheck();
		}
  }
}
