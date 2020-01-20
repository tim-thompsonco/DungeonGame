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
			player.InCombat = true;
			opponent.InCombat = true;
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
				if (player.Effects.Any()) {
					Helper.RemovedExpiredEffects(player);
					foreach (var effect in player.Effects) {
						switch (effect.EffectGroup) {
							case Effect.EffectType.Healing:
								effect.HealingRound(player, output);
								break;
							case Effect.EffectType.ChangeDamage:
								break;
							case Effect.EffectType.ChangeArmor:
								break;
							case Effect.EffectType.AbsorbDamage:
								break;
							case Effect.EffectType.OnFire:
								break;
							case Effect.EffectType.Bleeding:
								break;
							case Effect.EffectType.Stunned:
								break;
							case Effect.EffectType.Frozen:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
				switch (input[0]) {
					case "f":
					case "fight":
						var attackDamage = player.Attack(opponent, output);
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
						catch (NullReferenceException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
							}
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
							if (input[1] != null && input[1] != "bandage") {
								var abilityName = Helper.ParseInput(input);
								player.UseAbility(opponent, abilityName, output);
								if (opponent.IsMonsterDead(player, output)) return true;
							}

							if (input[1] != null && input[1] == "bandage") {
								var abilityName = Helper.ParseInput(input);
								player.UseAbility(abilityName, output);
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
						catch (NullReferenceException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
							}
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
						var canFlee = this.CanFleeCombat(player, output, opponent);
						if (canFlee) {
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
				if (opponent.Effects.Any()) {
					Helper.RemovedExpiredEffects(opponent);
					foreach (var effect in opponent.Effects) {
						switch (effect.EffectGroup) {
							case Effect.EffectType.Healing:
								break;
							case Effect.EffectType.ChangeDamage:
								break;
							case Effect.EffectType.ChangeArmor:
								break;
							case Effect.EffectType.AbsorbDamage:
								break;
							case Effect.EffectType.OnFire:
								effect.OnFireRound(opponent, output);
								break;
							case Effect.EffectType.Bleeding:
								effect.BleedingRound(opponent, output);
								break;
							case Effect.EffectType.Stunned:
								effect.StunnedRound(opponent, output);
								continue;
							case Effect.EffectType.Frozen:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						if (opponent.IsMonsterDead(player, output)) return true;
					}
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
				else if (attackDamageM - player.ArmorRating(opponent, output) < 0) {
					var armorAbsorbString = "Your armor absorbed all of " + opponent.Name + "'s attack!"; 
					output.StoreUserOutput(
						Helper.FormatAttackFailText(),
						Helper.FormatDefaultBackground(),
						armorAbsorbString);
					GearHelper.DecreaseArmorDurability(player);
				}
				else {
					var hitAmount = attackDamageM - player.ArmorRating(opponent, output);
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
		private bool CanFleeCombat(Player player, UserOutput output, IMonster opponent) {
			var randomNum = Helper.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				opponent.InCombat = false;
				return true;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
			return false;
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
