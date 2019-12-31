using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class CombatHelper {
		private static readonly Random RndGenerate = new Random();
		private string[] Commands { get; set; } = new string[3] {
		"[F]ight", "[I]nventory", "Flee" };

		public bool SingleCombat(IMonster opponent, Player player, UserOutput output) {
			var fightStartString = player.Name + ", you have encountered a " + opponent.Name + ". Time to fight!";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				fightStartString);
			this.ShowCommands(output);
			output.RetrieveUserOutput();
			while (true) {
				Helper.RequestCommand(output);
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
							Console.WriteLine("You hit the {0} for {1} physical damage.", 
								opponent.Name, 
								attackDamage - opponent.ArmorRating(player));
							opponent.TakeDamage(attackDamage - opponent.ArmorRating(player));
						}
						if (opponent.IsMonsterDead(player, output)) return true;
						break;
					case "cast":
						try {
							if (input[1] != null) {
								var spellName = Helper.ParseInput(input);
								player.CastSpell(opponent, spellName);
								if (opponent.IsMonsterDead(player, output)) return true;
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
								player.UseAbility(opponent, abilityName, output);
								if (opponent.IsMonsterDead(player, output)) return true;
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
									if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
										Console.WriteLine("You do not have a bow equipped!");
										continue;
									}
									Console.WriteLine("You do not have enough combo points to use that ability!");
									continue;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
					case "equip":
					case "unequip":
						GearHelper.EquipItem(player, input);
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
					case "reload":
						player.ReloadQuiver();
						break;
					case "i":
					case "inventory":
						PlayerHelper.ShowInventory(player, output);
						continue;
					case "list":
						switch (input[1]) {
							case "abilities":
								try {
									PlayerHelper.ListAbilities(player, output);
								}
								catch (IndexOutOfRangeException) {
									Helper.FormatFailureOutputText();
									Console.WriteLine("List what?");
								}
								continue;
							case "spells":
								try {
									PlayerHelper.ListSpells(player, output);
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
							PlayerHelper.AbilityInfo(player, input, output);
						}
						catch (IndexOutOfRangeException) {
							Helper.FormatFailureOutputText();
							Console.WriteLine("What ability did you want to know about?");
						}
						continue;
					case "spell":
						try {
							PlayerHelper.SpellInfo(player, input[1], output);
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
				if (opponent.OnFire) {
					this.BurnOnFire(opponent);
					if (opponent.IsMonsterDead(player, output)) return true;
				}
				if (opponent.IsBleeding) {
					this.Bleeding(opponent);
					if (opponent.IsMonsterDead(player, output)) return true;
				}
				if (player.IsArmorChanged) this.ChangeArmorRound(player);
				if (player.IsDamageChanged) this.ChangeDamageRound(player);
				if (opponent.IsStunned) {
					opponent.Stunned();
					continue;
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
				PlayerHelper.DisplayPlayerStats(player, output);
				opponent.DisplayStats();
				this.ShowCommands(output);
				output.RetrieveUserOutput();
			}
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
		public void BurnOnFire(IMonster opponent) {
			opponent.HitPoints -= opponent.OnFireDamage;
			Helper.FormatOnFireText();
			Console.WriteLine("The {0} burns for {1} fire damage.", opponent.Name, opponent.OnFireDamage);
			opponent.OnFireCurRound += 1;
			if (opponent.OnFireCurRound <= opponent.OnFireMaxRound) return;
			opponent.OnFire = false;
			opponent.OnFireCurRound = 1;
		}
		public void Bleeding(IMonster opponent) {
			Helper.FormatAttackSuccessText();
			opponent.HitPoints -= opponent.BleedDamage;
			Console.WriteLine("The {0} bleeds for {1} physical damage.", opponent.Name, opponent.BleedDamage);
			opponent.BleedCurRound += 1;
			if (opponent.BleedCurRound <= opponent.BleedMaxRound) return;
			opponent.IsBleeding = false;
			opponent.BleedCurRound = 1;
		}
		public void ShowCommands(UserOutput output) {
			var sameLineOutput = new List<string> {
				Helper.FormatGeneralInfoText(), Helper.FormatDefaultBackground(), "Available Commands: "};
			var objCount = this.Commands.Length;
			foreach (var command in this.Commands) {
				var sb = new StringBuilder();
				sb.Append(command);
				if (this.Commands[objCount - 1] != command) {
					sb.Append(", ");
				}
				sb.Append(".");
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add(sb.ToString());
			}
			output.StoreUserOutput(sameLineOutput);
		}
	}
}
