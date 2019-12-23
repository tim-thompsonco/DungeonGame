using System;
using System.Linq;

namespace DungeonGame {
	public class CombatHelper {
		private static readonly Random RndGenerate = new Random();
		public String[] Commands { get; set; } = new String[4] {
		"[F]ight", "[I]nventory", "Cast Fireball", "Flee" };

		public bool SingleCombat(IMonster opponent, Player player) {
			Helper.FormatSuccessOutputText();
			Console.WriteLine("{0}, you have encountered a {1}. Time to fight!",
				player.Name, opponent.Name);
			while (true) {
				player.DisplayPlayerStats();
				opponent.DisplayStats();
				Console.Write("Available Commands: ");
				Console.WriteLine(String.Join(", ", this.Commands));
				Helper.RequestCommand();
				string[] input = Helper.GetFormattedInput();
				Console.WriteLine(); // To add a blank space between the command and fight sequence
				int attackDamage;
				switch (input[0]) {
					case "f":
					case "fight":
						attackDamage = player.Attack();
						if (attackDamage - opponent.ArmorRating(player) < 0) {
							Helper.FormatAttackFailText();
							Console.WriteLine("The {0}'s armor absorbed all of your attack!", opponent.Name);
						}
						else if (attackDamage == 0) {
							Helper.FormatAttackFailText();
							Console.WriteLine("You missed {0}!", opponent.Name);
						}
						else {
							Helper.FormatAttackSuccessText();
							Console.WriteLine("You hit the {0} for {1} physical damage.", opponent.Name, attackDamage - opponent.ArmorRating(player));
							opponent.TakeDamage(attackDamage - opponent.ArmorRating(player));
						}
						if (opponent.HitPoints <= 0) {
							this.SingleCombatWin(opponent, player);
							return true;
						}
						break;
					case "cast":
						if (input[1] != null) {
							var spellName = Helper.ParseInput(input);
							player.CastSpell(opponent, spellName);
						}
						break;
					case "flee":
						var canFlee = this.CanFleeCombat();
						if (canFlee == true) {
							return false;
						}
						break;
					case "drink":
						if (input.Last() == "potion") {
							player.DrinkPotion(input);
						}
						else {
							Helper.FormatFailureOutputText();
							Console.WriteLine("You can't drink that!");
						}
						continue;
					case "i":
					case "inventory":
						player.ShowInventory(player);
						continue;
					default:
						Helper.InvalidCommand();
						continue;
				}
				if (opponent.HitPoints <= 0) {
					this.SingleCombatWin(opponent, player);
					return true;
				}
				if (opponent.OnFire) {
					opponent.BurnOnFire();
				}
				int attackDamageM = opponent.Attack();
				if (attackDamageM - player.ArmorRating(opponent) < 0) {
					Helper.FormatAttackFailText();
					Console.WriteLine("Your armor absorbed all of {0}'s attack!", opponent.Name);
					player.DecreaseArmorDurability();
				}
				else if (attackDamageM == 0) {
					Helper.FormatAttackFailText();
					Console.WriteLine("The {0} missed you!", opponent.Name);
				}
				else {
					Helper.FormatAttackSuccessText();
					Console.WriteLine("The {0} hits you for {1} physical damage.",
						opponent.Name, attackDamageM - player.ArmorRating(opponent));
					player.TakeDamage(attackDamageM - player.ArmorRating(opponent));
					player.DecreaseArmorDurability();
					if (player.HitPoints <= 0) {
						return false;
					}
				}
			}
		}
		public void SingleCombatWin(IMonster opponent, Player player) {
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have defeated the {0}!", opponent.Name);
			foreach (IEquipment loot in opponent.MonsterItems) {
				loot.Equipped = false;
			}
			opponent.Name = "Dead " + opponent.GetName();
			opponent.Desc = "A corpse of a monster you killed.";
			player.GainExperience(opponent.ExperienceProvided);
			player.LevelUpCheck();
		}
		public bool CanFleeCombat() {
			Console.ForegroundColor = ConsoleColor.Green;
			var randomNum = RndGenerate.Next(1, 10);
			if (randomNum > 5) {
				Helper.FormatSuccessOutputText();
				Console.WriteLine("You have fled combat successfully!");
				return true;
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("You tried to flee combat but failed!");
			return false;
		}
	}
}
