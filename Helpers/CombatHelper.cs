using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DungeonGame {
	public class CombatHelper {
		private string[] Commands { get; set; } = new string[3] {
		"[F]ight", "[I]nventory", "Flee" };

		public bool SingleCombat(IMonster opponent, Player player) {
			player.InCombat = true;
			opponent.InCombat = true;
			var fightStartString = player.Name + ", you have encountered a " + opponent.Name + ". Time to fight!";
			Helper.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			Helper.Display.BuildUserOutput();
			Helper.Display.ClearUserOutput();
			while (true) {
				PlayerHelper.DisplayPlayerStats(player);
				opponent.DisplayStats();
				this.ShowCommands();
				Helper.MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
				Helper.EffectDisplay = EffectOutput.ShowEffects(player);
				Helper.Display.BuildUserOutput();
				Helper.Display.ClearUserOutput();
				Helper.RequestCommand();
				var input = Helper.GetFormattedInput(Console.ReadLine());
				Console.Clear();
				if (player.Effects.Any()) {
					Helper.RemovedExpiredEffects(player);
					foreach (var effect in player.Effects) {
						switch (effect.EffectGroup) {
							case Effect.EffectType.Healing:
								effect.HealingRound(player);
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
						var attackDamage = player.Attack(opponent);
						if (attackDamage - opponent.ArmorRating(player) < 0) {
							var armorAbsorbString = "The " + opponent.Name + "'s armor absorbed all of your attack!";
							Helper.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								armorAbsorbString);
						}
						else if (attackDamage == 0) {
							var attackFailString = "You missed " + opponent.Name + "!";
							Helper.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								attackFailString);
						}
						else {
							var attackAmount = attackDamage - opponent.ArmorRating(player);
							var attackSucceedString = "You hit the " + opponent.Name + " for " + attackAmount + " physical damage.";
							Helper.Display.StoreUserOutput(
								Settings.FormatAttackSuccessText(),
								Settings.FormatDefaultBackground(),
								attackSucceedString);
							opponent.TakeDamage(attackAmount);
						}
						if (opponent.IsMonsterDead(player)) return true;
						break;
					case "cast":
						try {
							if (input[1] != null) {
								var spellName = Helper.ParseInput(input);
								player.CastSpell(opponent, spellName);
								if (opponent.IsMonsterDead(player)) return true;
							}
							break;
						}
						catch (IndexOutOfRangeException) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that spell.");
							continue;
						}
						catch (NullReferenceException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
							}
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
								continue;
							}
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You do not have enough mana to cast that spell!");
							continue;
						}
					case "use":
						try {
							if (input[1] != null && input[1] != "bandage") {
								var abilityName = Helper.ParseInput(input);
								player.UseAbility(opponent, abilityName);
								if (opponent.IsMonsterDead(player)) return true;
							}

							if (input[1] != null && input[1] == "bandage") {
								var abilityName = Helper.ParseInput(input);
								player.UseAbility(abilityName);
							}

							break;
						}
						catch (IndexOutOfRangeException) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (ArgumentOutOfRangeException) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (NullReferenceException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
							}
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								continue;
							}
							switch (player.PlayerClass) {
								case Player.PlayerClassType.Mage:
									continue;
								case Player.PlayerClassType.Warrior:
									Helper.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"You do not have enough rage to use that ability!");
									continue;
								case Player.PlayerClassType.Archer:
									if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
										Helper.Display.StoreUserOutput(
											Settings.FormatFailureOutputText(),
											Settings.FormatDefaultBackground(),
											"You do not have a bow equipped!");
										continue;
									}
									Helper.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"You do not have enough combo points to use that ability!");
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
						var canFlee = this.CanFleeCombat(player, opponent);
						if (canFlee) {
							return false;
						}
						break;
					case "drink":
						if (input.Last() == "potion") {
							player.DrinkPotion(input);
						}
						else {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't drink that!");
						}
						continue;
					case "reload":
						player.ReloadQuiver();
						break;
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
									Helper.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"List what?");
								}
								continue;
							case "spells":
								try {
									PlayerHelper.ListSpells(player);
								}
								catch (IndexOutOfRangeException) {
									Helper.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"List what?");
								}
								continue;
						}
						break;
					case "ability":
						try {
							PlayerHelper.AbilityInfo(player, input);
						}
						catch (IndexOutOfRangeException) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"What ability did you want to know about?");
						}
						continue;
					case "spell":
						try {
							PlayerHelper.SpellInfo(player, input[1]);
						}
						catch (IndexOutOfRangeException) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"What spell did you want to know about?");
						}
						continue;
					default:
						Helper.InvalidCommand();
						continue;
				}
				var isOpponentStunned = false;
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
								effect.OnFireRound(opponent);
								break;
							case Effect.EffectType.Bleeding:
								effect.BleedingRound(opponent);
								break;
							case Effect.EffectType.Stunned:
								effect.StunnedRound(opponent);
								isOpponentStunned = true;
								break;
							case Effect.EffectType.Frozen:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						if (opponent.IsMonsterDead(player)) return true;
					}
				}
				if (isOpponentStunned) continue;
				var attackDamageM = opponent.Attack(player);
				var defenseMoveString = "Your defensive move blocked " + player.AbsorbDamageAmount + " damage!";
				if (attackDamageM > player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					Helper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					attackDamageM -= player.AbsorbDamageAmount;
					player.AbsorbDamageAmount = 0;
				}
				else if (attackDamageM < player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					Helper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					player.AbsorbDamageAmount -= attackDamageM;
					attackDamageM = 0;
				}
				if (attackDamageM == 0) {
					var missString = "The " + opponent.Name + " missed you!"; 
					Helper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						missString);
				}
				else if (attackDamageM - player.ArmorRating(opponent) < 0) {
					var armorAbsorbString = "Your armor absorbed all of " + opponent.Name + "'s attack!"; 
					Helper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						armorAbsorbString);
					GearHelper.DecreaseArmorDurability(player);
				}
				else {
					var hitAmount = attackDamageM - player.ArmorRating(opponent);
					var hitString = "The " + opponent.Name + " hits you for " + hitAmount + " physical damage.";
					Helper.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						hitString);
					player.TakeDamage(hitAmount);
					GearHelper.DecreaseArmorDurability(player);
					if (player.HitPoints <= 0) {
						return false;
					}
				}
			}
		}
		private bool CanFleeCombat(Player player, IMonster opponent) {
			var randomNum = Helper.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				Helper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				opponent.InCombat = false;
				return true;
			}
			Helper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
			return false;
		}
		public void ShowCommands() {
			var sameLineOutput = new List<string> {
				Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available Commands: "};
			var objCount = this.Commands.Length;
			foreach (var command in this.Commands) {
				var sb = new StringBuilder();
				sb.Append(command);
				if (this.Commands[objCount - 1] != command) {
					sb.Append(", ");
				}
				if (this.Commands[objCount - 1] == command) sb.Append(".");
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(sb.ToString());
			}
			Helper.Display.StoreUserOutput(sameLineOutput);
		}
	}
}
