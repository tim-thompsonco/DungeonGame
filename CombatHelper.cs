using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DungeonGame {
	public class CombatHelper {
		private string[] Commands { get; set; } = new string[3] {
		"[F]ight", "[I]nventory", "Flee" };

		public bool SingleCombat(
			IMonster opponent, 
			Player player, 
			UserOutput output, 
			UserOutput mapOutput,
			List<IRoom> roomList) {
			var fightStartString = player.Name + ", you have encountered a " + opponent.Name + ". Time to fight!";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				fightStartString);
			output.RetrieveUserOutput();
			output.ClearUserOutput();
			while (true) {
				PlayerHelper.DisplayPlayerStats(player, output);
				opponent.DisplayStats(output);
				this.ShowCommands(output);
				mapOutput = Helper.ShowMap(roomList, player, Helper.GetMiniMapHeight(), Helper.GetMiniMapWidth());
				output.RetrieveUserOutput(mapOutput);
				output.ClearUserOutput();
				Helper.RequestCommand(output);
				var input = Helper.GetFormattedInput();
				Console.Clear();
				switch (input[0]) {
					case "f":
					case "fight":
						var attackDamage = player.Attack(output);
						if (player.IsDamageChanged) attackDamage += player.ChangeDamageAmount;
						if (attackDamage - opponent.ArmorRating(player) < 0) {
							var armorAbsorbString = "The " + opponent.Name + "'s armor absorbed all of your attack!";
							output.StoreUserOutput(
								Helper.FormatAttackFailText(),
								Helper.FormatDefaultBackground(),
								armorAbsorbString);
						}
						else if (attackDamage == 0) {
							var attackFailString = "You missed " + opponent.Name + "!";
							output.StoreUserOutput(
								Helper.FormatAttackFailText(),
								Helper.FormatDefaultBackground(),
								attackFailString);
						}
						else {
							var attackAmount = attackDamage - opponent.ArmorRating(player);
							var attackSucceedString = "You hit the " + opponent.Name + " for " + attackAmount + " physical damage.";
							output.StoreUserOutput(
								Helper.FormatAttackSuccessText(),
								Helper.FormatDefaultBackground(),
								attackSucceedString);
							opponent.TakeDamage(attackAmount);
						}
						if (opponent.IsMonsterDead(player, output)) return true;
						break;
					case "cast":
						try {
							if (input[1] != null) {
								var spellName = Helper.ParseInput(input);
								player.CastSpell(opponent, spellName, output);
								if (opponent.IsMonsterDead(player, output)) return true;
							}
							break;
						}
						catch (IndexOutOfRangeException) {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You don't have that spell.");
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
								continue;
							}
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You do not have enough mana to cast that spell!");
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
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (ArgumentOutOfRangeException) {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								continue;
							}
							switch (player.PlayerClass) {
								case Player.PlayerClassType.Mage:
									continue;
								case Player.PlayerClassType.Warrior:
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(),
										Helper.FormatDefaultBackground(),
										"You do not have enough rage to use that ability!");
									continue;
								case Player.PlayerClassType.Archer:
									if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
										output.StoreUserOutput(
											Helper.FormatFailureOutputText(),
											Helper.FormatDefaultBackground(),
											"You do not have a bow equipped!");
										continue;
									}
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(),
										Helper.FormatDefaultBackground(),
										"You do not have enough combo points to use that ability!");
									continue;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
					case "equip":
					case "unequip":
						GearHelper.EquipItem(player, input, output);
						break;
					case "flee":
						var canFlee = this.CanFleeCombat(output);
						if (canFlee == true) {
							return false;
						}
						break;
					case "drink":
						if (input.Last() == "potion") {
							player.DrinkPotion(input, output);
						}
						else {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You can't drink that!");
						}
						continue;
					case "reload":
						player.ReloadQuiver(output);
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
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(),
										Helper.FormatDefaultBackground(),
										"List what?");
								}
								continue;
							case "spells":
								try {
									PlayerHelper.ListSpells(player, output);
								}
								catch (IndexOutOfRangeException) {
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(),
										Helper.FormatDefaultBackground(),
										"List what?");
								}
								continue;
						}
						break;
					case "ability":
						try {
							PlayerHelper.AbilityInfo(player, input, output);
						}
						catch (IndexOutOfRangeException) {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"What ability did you want to know about?");
						}
						continue;
					case "spell":
						try {
							PlayerHelper.SpellInfo(player, input[1], output);
						}
						catch (IndexOutOfRangeException) {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"What spell did you want to know about?");
						}
						continue;
					default:
						Helper.InvalidCommand(output);
						continue;
				}
				if (player.IsHealing) this.HealingRound(player, output);
				if (opponent.OnFire) {
					this.BurnOnFire(opponent, output);
					if (opponent.IsMonsterDead(player, output)) return true;
				}
				if (opponent.IsBleeding) {
					this.Bleeding(opponent, output);
					if (opponent.IsMonsterDead(player, output)) return true;
				}
				if (player.IsArmorChanged) this.ChangeArmorRound(player, output);
				if (player.IsDamageChanged) this.ChangeDamageRound(player, output);
				if (opponent.IsStunned) {
					opponent.Stunned(output);
					continue;
				}
				var attackDamageM = opponent.Attack(player);
				var defenseMoveString = "Your defensive move blocked " + player.AbsorbDamageAmount + " damage!";
				if (attackDamageM > player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					output.StoreUserOutput(
						Helper.FormatAttackFailText(),
						Helper.FormatDefaultBackground(),
						defenseMoveString);
					attackDamageM -= player.AbsorbDamageAmount;
					player.AbsorbDamageAmount = 0;
				}
				else if (attackDamageM < player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					output.StoreUserOutput(
						Helper.FormatAttackFailText(),
						Helper.FormatDefaultBackground(),
						defenseMoveString);
					player.AbsorbDamageAmount -= attackDamageM;
					attackDamageM = 0;
				}
				if (attackDamageM == 0) {
					var missString = "The " + opponent.Name + " missed you!"; 
					output.StoreUserOutput(
						Helper.FormatAttackFailText(),
						Helper.FormatDefaultBackground(),
						missString);
				}
				else if (attackDamageM - player.ArmorRating(opponent) < 0) {
					var armorAbsorbString = "Your armor absorbed all of " + opponent.Name + "'s attack!"; 
					output.StoreUserOutput(
						Helper.FormatAttackFailText(),
						Helper.FormatDefaultBackground(),
						armorAbsorbString);
					GearHelper.DecreaseArmorDurability(player);
				}
				else {
					var hitAmount = attackDamageM - player.ArmorRating(opponent);
					var hitString = "The " + opponent.Name + " hits you for " + hitAmount + " physical damage.";
					output.StoreUserOutput(
						Helper.FormatAttackSuccessText(),
						Helper.FormatDefaultBackground(),
						hitString);
					player.TakeDamage(hitAmount);
					GearHelper.DecreaseArmorDurability(player);
					if (player.HitPoints <= 0) {
						return false;
					}
				}
			}
		}
		private bool CanFleeCombat(UserOutput output) {
			Console.ForegroundColor = ConsoleColor.Green;
			var randomNum = Helper.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					"You have fled combat successfully!");
				return true;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
			return false;
		}
		public void ChangeArmorRound(Player player, UserOutput output) {
			player.ChangeArmorCurRound += 1;
			var augmentString = "Your armor is augmented by " + player.ChangeArmorAmount + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				augmentString);
			if (player.ChangeArmorCurRound <= player.ChangeArmorMaxRound) return;
			player.IsArmorChanged = false;
			player.ChangeArmorCurRound = 1;
		}
		public void ChangeDamageRound(Player player, UserOutput output) {
			player.ChangeDamageCurRound += 1;
			var changeDmgString = player.ChangeDamageAmount > 0 ?
				"Your damage is increased by " + player.ChangeDamageAmount + "."
				: "Your damage is decreased by " + player.ChangeDamageAmount + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				changeDmgString);
			if (player.ChangeDamageCurRound <= player.ChangeDamageMaxRound) return;
			player.IsDamageChanged = false;
			player.ChangeDamageCurRound = 1;
		}
		public void HealingRound(Player player, UserOutput output) {
			player.HealCurRound += 1;
			player.HitPoints += player.HealAmount;
			if (player.HitPoints > player.MaxHitPoints) player.HitPoints = player.MaxHitPoints;
			var healAmtString = "You have been healed for " + player.HealAmount + " health."; 
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				healAmtString);
			if (player.HealCurRound <= player.HealMaxRound) return;
			player.IsHealing = false;
			player.HealCurRound = 1;
		}
		public void BurnOnFire(IMonster opponent, UserOutput output) {
			opponent.HitPoints -= opponent.OnFireDamage;
			var burnString = "The " + opponent.Name + " burns for " + opponent.OnFireDamage + " fire damage.";
			output.StoreUserOutput(
				Helper.FormatOnFireText(),
				Helper.FormatDefaultBackground(),
				burnString);
			opponent.OnFireCurRound += 1;
			if (opponent.OnFireCurRound <= opponent.OnFireMaxRound) return;
			opponent.OnFire = false;
			opponent.OnFireCurRound = 1;
		}
		public void Bleeding(IMonster opponent, UserOutput output) {
			opponent.HitPoints -= opponent.BleedDamage;
			var bleedString = "The " + opponent.Name + " bleeds for " + opponent.BleedDamage + " physical damage.";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				bleedString);
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
				if (this.Commands[objCount - 1] == command) sb.Append(".");
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add(sb.ToString());
			}
			output.StoreUserOutput(sameLineOutput);
		}
	}
}
