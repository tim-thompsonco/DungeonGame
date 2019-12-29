using System;
using System.Linq;
using System.Linq.Expressions;

namespace DungeonGame {
	public class CombatHelper {
		private static readonly Random RndGenerate = new Random();
		private string[] Commands { get; set; } = new string[3] {
		"[F]ight", "[I]nventory", "Flee" };

		public bool SingleCombat(IMonster opponent, Player player) {
			Helper.FormatSuccessOutputText();
			Console.WriteLine("{0}, you have encountered a {1}. Time to fight!",
				player.Name, opponent.Name);
			while (true) {
				PlayerHelper.DisplayPlayerStats(player);
				opponent.DisplayStats();
				Console.Write("Available Commands: ");
				Console.WriteLine(string.Join(", ", this.Commands));
				Helper.RequestCommand();
				var input = Helper.GetFormattedInput();
				Console.WriteLine(); // To add a blank space between the command and fight sequence
				switch (input[0]) {
					case "f":
					case "fight":
						var attackDamage = player.Attack();
						if (player.IsDamageChanged) attackDamage += player.ChangeDamageAmount;
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
						try {
							if (input[1] != null) {
								var spellName = Helper.ParseInput(input);
								player.CastSpell(opponent, spellName);
							}
							break;
						}
						catch (IndexOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("You don't have that spell.");
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You can't cast spells. You're not a mage!");
								continue;
							}
							Helper.FormatFailureOutputText();
							Console.WriteLine("You do not have enough mana to cast that spell!");
							continue;
						}
					case "use":
						try {
							if (input[1] != null) {
								var abilityName = Helper.ParseInput(input);
								player.UseAbility(opponent, abilityName);
							}
							break;
						}
						catch (IndexOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("You don't have that ability.");
							continue;
						}
						catch (ArgumentOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("You don't have that ability.");
							continue;
						}
						catch (InvalidOperationException) {
							Helper.FormatFailureOutputText();
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								Console.WriteLine("You can't use abilities. You're not a warrior or archer!");
								continue;
							}
							switch (player.PlayerClass) {
								case Player.PlayerClassType.Mage:
									continue;
								case Player.PlayerClassType.Warrior:
									Console.WriteLine("You do not have enough rage to use that ability!");
									continue;
								case Player.PlayerClassType.Archer:
									Console.WriteLine("You do not have enough combo points to use that ability!");
									continue;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
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
						PlayerHelper.ShowInventory(player);
						continue;
					case "list":
						switch (input[1]) {
							case "abilities":
								try {
									PlayerHelper.ListAbilities(player);
								}
								catch (IndexOutOfRangeException) {
									Helper.FormatFailureOutputText();
									Console.WriteLine("List what?");
								}
								continue;
							case "spells":
								try {
									PlayerHelper.ListSpells(player);
								}
								catch (IndexOutOfRangeException) {
									Helper.FormatFailureOutputText();
									Console.WriteLine("List what?");
								}
								continue;
						}
						break;
					case "ability":
						try {
							PlayerHelper.AbilityInfo(player, input);
						}
						catch (IndexOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("What ability did you want to know about?");
						}
						continue;
					case "spell":
						try {
							PlayerHelper.SpellInfo(player, input[1]);
						}
						catch (IndexOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("What spell did you want to know about?");
						}
						continue;
					default:
						Helper.InvalidCommand();
						continue;
				}
				if (player.IsHealing) this.HealingRound(player);
				// Check opponent health to determine dead or not before special abilities
				if (opponent.HitPoints <= 0) {
					this.SingleCombatWin(opponent, player);
					return true;
				}
				if (opponent.OnFire) opponent.BurnOnFire();
				if (opponent.IsBleeding) opponent.Bleeding();
				if (player.IsArmorChanged) this.ChangeArmorRound(player);
				if (player.IsDamageChanged) this.ChangeDamageRound(player);
				if (opponent.IsStunned) {
					opponent.Stunned();
					continue;
				}
				// Check opponent health to determine dead or not after special abilities
				if (opponent.HitPoints <= 0) {
					this.SingleCombatWin(opponent, player);
					return true;
				}
				var attackDamageM = opponent.Attack();
				if (attackDamageM > player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					Console.WriteLine("Your defensive move blocked {0} damage!", player.AbsorbDamageAmount);
					attackDamageM -= player.AbsorbDamageAmount;
					player.AbsorbDamageAmount = 0;
				}
				else if (attackDamageM < player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					Console.WriteLine("Your defensive move blocked {0} damage!", player.AbsorbDamageAmount);
					player.AbsorbDamageAmount -= attackDamageM;
					attackDamageM = 0;
				}
				if (attackDamageM - player.ArmorRating(opponent) < 0) {
					Helper.FormatAttackFailText();
					Console.WriteLine("Your armor absorbed all of {0}'s attack!", opponent.Name);
					GearHelper.DecreaseArmorDurability(player);
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
					GearHelper.DecreaseArmorDurability(player);
					if (player.HitPoints <= 0) {
						return false;
					}
				}
			}
		}
		public void SingleCombatWin(IMonster opponent, Player player) {
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have defeated the {0}!", opponent.Name);
			foreach (var loot in opponent.MonsterItems) {
				loot.Equipped = false;
			}
			opponent.Name = "Dead " + opponent.GetName();
			opponent.Desc = "A corpse of a monster you killed.";
			player.GainExperience(opponent.ExperienceProvided);
			PlayerHelper.LevelUpCheck(player);
		}
		private bool CanFleeCombat() {
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
		public void ChangeArmorRound(Player player) {
			player.ChangeArmorCurRound += 1;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("Your armor is augmented by {0}.", player.ChangeArmorAmount);
			if (player.ChangeArmorCurRound <= player.ChangeArmorMaxRound) return;
			player.IsArmorChanged = false;
			player.ChangeArmorCurRound = 1;
		}
		public void ChangeDamageRound(Player player) {
			player.ChangeDamageCurRound += 1;
			Console.WriteLine(
				player.ChangeDamageAmount > 0 ? "Your damage is increased by {0}." : "Your damage is decreased by {0}",
				player.ChangeDamageAmount);
			if (player.ChangeDamageCurRound <= player.ChangeDamageMaxRound) return;
			player.IsDamageChanged = false;
			player.ChangeDamageCurRound = 1;
		}
		public void HealingRound(Player player) {
			player.HealCurRound += 1;
			Helper.FormatSuccessOutputText();
			player.HitPoints += player.HealAmount;
			if (player.HitPoints > player.MaxHitPoints) player.HitPoints = player.MaxHitPoints;
			Console.WriteLine("You have been healed for {0} health.", player.HealAmount);
			if (player.HealCurRound <= player.HealMaxRound) return;
			player.IsHealing = false;
			player.HealCurRound = 1;
		}
	}
}
