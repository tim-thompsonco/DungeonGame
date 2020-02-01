using System;
using System.Linq;

namespace DungeonGame {
	public class CombatHandler {
		private string[] Input { get; set; }
		
		public bool SingleCombat(Monster opponent, Player player) {
			player.InCombat = true;
			opponent.InCombat = true;
			var fightStartString = player.Name + ", you have encountered a " + opponent.Name + ". Time to fight!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			OutputHandler.ShowUserOutput(player);
			while (opponent.HitPoints > 0 && player.InCombat && opponent.InCombat) {
				this.Input = InputHandler.GetFormattedInput(Console.ReadLine());
				Console.Clear();
				OutputHandler.ShowUserOutput(player, opponent);
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
				this.ProcessPlayerInput(player, opponent);
				if (opponent.IsMonsterDead(player)) return true;
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
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					attackDamageM -= player.AbsorbDamageAmount;
					player.AbsorbDamageAmount = 0;
				}
				else if (attackDamageM < player.AbsorbDamageAmount && player.AbsorbDamageAmount > 0) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						defenseMoveString);
					player.AbsorbDamageAmount -= attackDamageM;
					attackDamageM = 0;
				}
				if (attackDamageM == 0) {
					var missString = "The " + opponent.Name + " missed you!"; 
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						missString);
				}
				else if (attackDamageM - player.ArmorRating(opponent) < 0) {
					var armorAbsorbString = "Your armor absorbed all of " + opponent.Name + "'s attack!"; 
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						armorAbsorbString);
					GearHandler.DecreaseArmorDurability(player);
				}
				else {
					var hitAmount = attackDamageM - player.ArmorRating(opponent);
					var hitString = "The " + opponent.Name + " hits you for " + hitAmount + " physical damage.";
					OutputHandler.Display.StoreUserOutput(
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
			return true;
		}
		private void FleeCombat(Player player, Monster opponent) {
			var randomNum = GameHandler.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				opponent.InCombat = false;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
		}
		private void ProcessPlayerInput(Player player, Monster opponent) {
			switch (this.Input[0]) {
				case "f":
				case "fight":
					var attackDamage = player.Attack(opponent);
					if (attackDamage - opponent.ArmorRating(player) < 0) {
						var armorAbsorbString = "The " + opponent.Name + "'s armor absorbed all of your attack!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					}
					else if (attackDamage == 0) {
						var attackFailString = "You missed " + opponent.Name + "!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					}
					else {
						var attackAmount = attackDamage - opponent.ArmorRating(player);
						var attackSucceedString = "You hit the " + opponent.Name + " for " + attackAmount + " physical damage.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						opponent.TakeDamage(attackAmount);
					}
					break;
				case "cast":
					try {
						if (this.Input[1] != null) {
							var spellName = InputHandler.ParseInput(this.Input);
							player.CastSpell(opponent, spellName);
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You do not have enough mana to cast that spell!");
					}
					break;
				case "use":
					try {
						if (this.Input[1] != null && this.Input[1] != "bandage") {
							var abilityName = InputHandler.ParseInput(this.Input);
							player.UseAbility(opponent, abilityName);
						}
						if (this.Input[1] != null && this.Input[1] == "bandage") {
							var abilityName = InputHandler.ParseInput(this.Input);
							player.UseAbility(abilityName);
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
					}
					catch (ArgumentOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
						switch (player.PlayerClass) {
							case Player.PlayerClassType.Mage:
								break;
							case Player.PlayerClassType.Warrior:
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
									OutputHandler.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"You do not have a bow equipped!");
								}
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough combo points to use that ability!");
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					break;
				case "equip":
				case "unequip":
					GearHandler.EquipItem(player, this.Input);
					break;
				case "flee":
					this.FleeCombat(player, opponent);
					break;
				case "drink":
					if (this.Input.Last() == "potion") {
						player.DrinkPotion(this.Input);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
					}
					break;
				case "reload":
					player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerHandler.ShowInventory(player);
					break;
				case "list":
					switch (this.Input[1]) {
						case "abilities":
							try {
								PlayerHandler.ListAbilities(player);
							}
							catch (IndexOutOfRangeException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
							}
							break;
						case "spells":
							try {
								PlayerHandler.ListSpells(player);
							}
							catch (IndexOutOfRangeException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
							}
							break;
					}
					break;
				case "ability":
					try {
						PlayerHandler.AbilityInfo(player, this.Input);
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}
					break;
				case "spell":
					try {
						PlayerHandler.SpellInfo(player, this.Input[1]);
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}
					break;
				default:
					Messages.InvalidCommand();
					break;
			}
		}
	}
}
