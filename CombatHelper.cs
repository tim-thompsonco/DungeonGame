using System;
using System.Linq;

namespace DungeonGame {
  public class CombatHelper {
		public String[] Commands { get; set; } = new String[2] {
		"[F]ight", "[C]ast [F]ireball" };

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
				var inputParse = input.Split(' ');
				Console.WriteLine(); // To add a blank space between the command and fight sequence
        switch (inputParse[0]) {
          case "f":
					case "fight":
            var attackDamage = player.Attack();
						if (attackDamage - opponent.ArmorRating(player) < 0) {
							Console.ForegroundColor = ConsoleColor.DarkRed;
							Console.WriteLine("The {0}'s armor absorbed all of your attack!", opponent.Name);
						}
            else if (attackDamage == 0) {
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine("You missed {0}!", opponent.Name);
            }
            else {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("You hit the {0} for {1} physical damage.", opponent.Name, attackDamage - opponent.ArmorRating(player));
              opponent.TakeDamage(attackDamage - opponent.ArmorRating(player));
            }
            if (opponent.HitPoints <= 0) {
              this.SingleCombatWin(opponent, player);
              return true;
            }
						break;
					case "cf":
					case "cast fireball":
						if(player.ManaPoints >= player.Player_Spell.ManaCost) {
							player.ManaPoints -= player.Player_Spell.ManaCost;
							attackDamage = player.Player_Spell.FireOffense.BlastDamage;
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
						}
						else {
							Console.WriteLine("You do not have enough mana to cast that spell!");
							continue;
						}
					case "drink":
						if (inputParse.Last() == "potion") {
							player.DrinkPotion(inputParse);
						}
						else {
							Console.WriteLine("You can't drink that!");
						}
						continue;
					default:
            Helper.InvalidCommand();
						continue;
        }
				if (opponent.OnFire) {
					var burnDamage = player.Player_Spell.FireOffense.BurnDamage;
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("The {0} burns for {1} fire damage.", opponent.Name, burnDamage);
					opponent.TakeDamage(burnDamage);
					player.Player_Spell.FireOffense.BurnCurRounds += 1;
				}
				if (player.Player_Spell.FireOffense.BurnCurRounds > player.Player_Spell.FireOffense.BurnMaxRounds) {
					opponent.OnFire = false;
					player.Player_Spell.FireOffense.BurnCurRounds = 1;
				}
				var attackDamageM = opponent.Attack();
				if (attackDamageM - player.ArmorRating(opponent) < 0) {
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("Your armor absorbed all of {0}'s attack!", opponent.Name);
				}
        else if (attackDamageM == 0) {
          Console.ForegroundColor = ConsoleColor.DarkRed;
          Console.WriteLine("The {0} missed you!", opponent.Name);
        }
        else {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("The {0} hits you for {1} physical damage.",
            opponent.Name, attackDamageM - player.ArmorRating(opponent));
          player.TakeDamage(attackDamageM - player.ArmorRating(opponent));
          if (player.HitPoints <= 0) {
            return false;
          }
        }
      }
    }
    public void SingleCombatWin(IMonster opponent, NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("You have defeated the {0}!", opponent.Name);
			foreach(var loot in opponent.MonsterItems) {
				loot.Equipped = false;
			}
			opponent.Name = "Dead " + opponent.GetName();
			opponent.Desc = "A corpse of a monster you killed.";
			player.GainExperience(opponent.ExperienceProvided);
			player.LevelUpCheck();
		}
  }
}
