using System;
using System.Linq;

namespace DungeonGame {
	public class CombatHandler {
		public bool SingleCombat(Monster opponent, Player player) {
			player.InCombat = true;
			opponent.InCombat = true;
			var fightStartString = player.Name + ", you have encountered a " + opponent.Name + ". Time to fight!";
			RoomHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			RoomHandler.ShowUserOutput(player);
			while (true) {
				var input = InputHandler.GetFormattedInput(Console.ReadLine());
				Console.Clear();
				RoomHandler.ShowUserOutput(player, opponent);
				if (player.Effects.Any()) {
					GameHandler.RemovedExpiredEffects(player);
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
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								armorAbsorbString);
						}
						else if (attackDamage == 0) {
							var attackFailString = "You missed " + opponent.Name + "!";
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								attackFailString);
						}
						else {
							var attackAmount = attackDamage - opponent.ArmorRating(player);
							var attackSucceedString = "You hit the " + opponent.Name + " for " + attackAmount + " physical damage.";
							RoomHandler.Display.StoreUserOutput(
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
								var spellName = InputHandler.ParseInput(input);
								player.CastSpell(opponent, spellName);
								if (opponent.IsMonsterDead(player)) return true;
							}
							break;
						}
						catch (IndexOutOfRangeException) {
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that spell.");
							continue;
						}
						catch (NullReferenceException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								RoomHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
							}
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass != Player.PlayerClassType.Mage) {
								RoomHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't cast spells. You're not a mage!");
								continue;
							}
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You do not have enough mana to cast that spell!");
							continue;
						}
					case "use":
						try {
							if (input[1] != null && input[1] != "bandage") {
								var abilityName = InputHandler.ParseInput(input);
								player.UseAbility(opponent, abilityName);
								if (opponent.IsMonsterDead(player)) return true;
							}

							if (input[1] != null && input[1] == "bandage") {
								var abilityName = InputHandler.ParseInput(input);
								player.UseAbility(abilityName);
							}

							break;
						}
						catch (IndexOutOfRangeException) {
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (ArgumentOutOfRangeException) {
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You don't have that ability.");
							continue;
						}
						catch (NullReferenceException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								RoomHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
							}
							continue;
						}
						catch (InvalidOperationException) {
							if (player.PlayerClass == Player.PlayerClassType.Mage) {
								RoomHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								continue;
							}
							switch (player.PlayerClass) {
								case Player.PlayerClassType.Mage:
									continue;
								case Player.PlayerClassType.Warrior:
									RoomHandler.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"You do not have enough rage to use that ability!");
									continue;
								case Player.PlayerClassType.Archer:
									if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
										RoomHandler.Display.StoreUserOutput(
											Settings.FormatFailureOutputText(),
											Settings.FormatDefaultBackground(),
											"You do not have a bow equipped!");
										continue;
									}
									RoomHandler.Display.StoreUserOutput(
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
						GearHandler.EquipItem(player, input);
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
							RoomHandler.Display.StoreUserOutput(
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
						PlayerHandler.ShowInventory(player);
						continue;
					case "list":
						switch (input[1]) {
							case "abilities":
								try {
									PlayerHandler.ListAbilities(player);
								}
								catch (IndexOutOfRangeException) {
									RoomHandler.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"List what?");
								}
								continue;
							case "spells":
								try {
									PlayerHandler.ListSpells(player);
								}
								catch (IndexOutOfRangeException) {
									RoomHandler.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"List what?");
								}
								continue;
						}
						break;
					case "ability":
						try {
							PlayerHandler.AbilityInfo(player, input);
						}
						catch (IndexOutOfRangeException) {
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"What ability did you want to know about?");
						}
						continue;
					case "spell":
						try {
							PlayerHandler.SpellInfo(player, input[1]);
						}
						catch (IndexOutOfRangeException) {
							RoomHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"What spell did you want to know about?");
						}
						continue;
					default:
						Messages.InvalidCommand();
						continue;
				}
				var isOpponentStunned = false;
				if (opponent.Effects.Any()) {
					GameHandler.RemovedExpiredEffects(opponent);
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
					RoomHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					attackDamageM -= player.AbsorbDamageAmount;
					player.AbsorbDamageAmount = 0;
				}
				else if (attackDamageM < player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					RoomHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					player.AbsorbDamageAmount -= attackDamageM;
					attackDamageM = 0;
				}
				if (attackDamageM == 0) {
					var missString = "The " + opponent.Name + " missed you!"; 
					RoomHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						missString);
				}
				else if (attackDamageM - player.ArmorRating(opponent) < 0) {
					var armorAbsorbString = "Your armor absorbed all of " + opponent.Name + "'s attack!"; 
					RoomHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						armorAbsorbString);
					GearHandler.DecreaseArmorDurability(player);
				}
				else {
					var hitAmount = attackDamageM - player.ArmorRating(opponent);
					var hitString = "The " + opponent.Name + " hits you for " + hitAmount + " physical damage.";
					RoomHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						hitString);
					player.TakeDamage(hitAmount);
					GearHandler.DecreaseArmorDurability(player);
					if (player.HitPoints <= 0) {
						return false;
					}
				}
			}
		}
		private bool CanFleeCombat(Player player, Monster opponent) {
			var randomNum = GameHandler.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				RoomHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				opponent.InCombat = false;
				return true;
			}
			RoomHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
			return false;
		}
	}
}
