using System;

namespace DungeonGame {
  public class CombatHelper {
		public String[] Commands { get; set; } = new String[4] {
		"[A]ttack", "[C]ast [F]ireball", "[D]rink [H]ealth [P]otion", "[D]rink [M]ana [P]otion" };

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
							}
						else {
							Console.WriteLine("You do not have enough mana to cast that spell!");
						}
            break;
					case "dhp":
						if(player.HealthPotion.Quantity >= 1) {
							player.HealthPotion.RestoreHealth.RestoreHealthPlayer(player);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("You drank a potion and replenished {0} health.", player.HealthPotion.RestoreHealth.RestoreHealthAmt);
							player.HealthPotion.Quantity -= 1;
							player.Inventory.Remove((DungeonGame.IEquipment)player.HealthPotion);
						}
						else {
							Console.WriteLine("You don't have any health potions!");
						}
						break;
					case "dmp":
						if (player.ManaPotion.Quantity >= 1) {
							player.ManaPotion.RestoreMana.RestoreManaPlayer(player);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("You drank a potion and replenished {0} mana.", player.ManaPotion.RestoreMana.RestoreManaAmt);
							player.ManaPotion.Quantity -= 1;
							player.Inventory.Remove((DungeonGame.IEquipment)player.ManaPotion);
							}
						else {
							Console.WriteLine("You don't have any mana potions!");
							}
						break;
					default:
            Helper.InvalidCommand();
            break;
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
					attackDamageM = 0;
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
			player.GainExperience(opponent.ExperienceProvided);
			player.LevelUpCheck();
		}
  }
}
