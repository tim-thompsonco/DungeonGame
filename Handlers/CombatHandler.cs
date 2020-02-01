using System;
using System.Linq;

namespace DungeonGame {
	public class CombatHandler {
		private string[] Input { get; set; }
		private Monster Opponent { get; set; }
		private Player Player { get; set; }
		private bool IsOpponentStunned { get; set; }
		private bool FleeSuccess { get; set; }
		
		public bool SingleCombat(Monster opponent, Player player) {
			Console.Clear();
			this.Player = player;
			this.Opponent = opponent;
			this.Player.InCombat = true;
			this.Opponent.InCombat = true;
			var fightStartString = this.Player.Name + ", you have encountered a " + this.Opponent.Name + ". Time to fight!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			OutputHandler.ShowUserOutput(this.Player);
			OutputHandler.Display.ClearUserOutput();
			while (this.Opponent.HitPoints > 0 && this.Player.HitPoints > 0 && 
			       this.Player.InCombat && this.Opponent.InCombat) {
				if (this.Player.Effects.Any()) {
					this.ProcessPlayerEffects();
				}
				this.Input = InputHandler.GetFormattedInput(Console.ReadLine());
				this.ProcessPlayerInput(); // Player will attack, use ability, cast spells, etc. to cause damage
				if (this.FleeSuccess) return false;
				if (this.Opponent.IsMonsterDead(this.Player)) return true;
				if (this.Opponent.Effects.Any()) {
					this.ProcessOpponentEffects();
				}
				if (this.Opponent.IsMonsterDead(this.Player)) return true;
				if (this.IsOpponentStunned) continue;
				this.ProcessMonsterAttack();
				Console.Clear();
				OutputHandler.ShowUserOutput(this.Player, this.Opponent);
				OutputHandler.Display.ClearUserOutput();
			}
			return player.HitPoints > 0;
		}
		private void FleeCombat() {
			var randomNum = GameHandler.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				this.Player.InCombat = false;
				this.Opponent.InCombat = false;
				this.FleeSuccess = true;
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoUp) {
					RoomHandler.ChangeRoom(this.Player, 0, 0, 1);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoEast) {
					RoomHandler.ChangeRoom(this.Player, 1, 0, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoWest) {
					RoomHandler.ChangeRoom(this.Player, -1, 0, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorth) {
					RoomHandler.ChangeRoom(this.Player, 0, 1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouth) {
					RoomHandler.ChangeRoom(this.Player, 0, -1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorthEast) {
					RoomHandler.ChangeRoom(this.Player, 1, 1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorthWest) {
					RoomHandler.ChangeRoom(this.Player, -1, 1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouthEast) {
					RoomHandler.ChangeRoom(this.Player, 1, -1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouthWest) {
					RoomHandler.ChangeRoom(this.Player, -1, -1, 0);
					return;
				}
				if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoDown) {
					RoomHandler.ChangeRoom(this.Player, 0, 0, -1);
					return;
				}
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
		}
		private void ProcessPlayerEffects() {
			GameHandler.RemovedExpiredEffects(this.Player);
			foreach (var effect in this.Player.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						effect.HealingRound(this.Player);
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
		private void ProcessOpponentEffects() {
			GameHandler.RemovedExpiredEffects(this.Opponent);
			foreach (var effect in this.Opponent.Effects) {
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
						effect.OnFireRound(this.Opponent);
						break;
					case Effect.EffectType.Bleeding:
						effect.BleedingRound(this.Opponent);
						break;
					case Effect.EffectType.Stunned:
						effect.StunnedRound(this.Opponent);
						this.IsOpponentStunned = true;
						break;
					case Effect.EffectType.Frozen:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		private void ProcessMonsterAttack() {
			var attackDamageM = this.Opponent.Attack(this.Player);
			var defenseMoveString = "Your defensive move blocked " + this.Player.AbsorbDamageAmount + " damage!";
			if (attackDamageM > this.Player.AbsorbDamageAmount && this.Player.AbsorbDamageAmount > 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					defenseMoveString);
				attackDamageM -= this.Player.AbsorbDamageAmount;
				this.Player.AbsorbDamageAmount = 0;
			}
			else if (attackDamageM < this.Player.AbsorbDamageAmount && this.Player.AbsorbDamageAmount > 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					defenseMoveString);
				this.Player.AbsorbDamageAmount -= attackDamageM;
				attackDamageM = 0;
			}
			if (attackDamageM == 0) {
				var missString = "The " + this.Opponent.Name + " missed you!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					missString);
			}
			else if (attackDamageM - this.Player.ArmorRating(this.Opponent) < 0) {
				var armorAbsorbString = "Your armor absorbed all of " + this.Opponent.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					armorAbsorbString);
				GearHandler.DecreaseArmorDurability(this.Player);
			}
			else {
				var hitAmount = attackDamageM - this.Player.ArmorRating(this.Opponent);
				var hitString = "The " + this.Opponent.Name + " hits you for " + hitAmount + " physical damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					hitString);
				this.Player.TakeDamage(hitAmount);
				GearHandler.DecreaseArmorDurability(this.Player);
			}
		}
		private void ProcessPlayerInput() {
			switch (this.Input[0]) {
				case "f":
				case "fight":
					var attackDamage = this.Player.Attack(this.Opponent);
					if (attackDamage - this.Opponent.ArmorRating(this.Player) < 0) {
						var armorAbsorbString = "The " + this.Opponent.Name + "'s armor absorbed all of your attack!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					}
					else if (attackDamage == 0) {
						var attackFailString = "You missed " + this.Opponent.Name + "!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					}
					else {
						var attackAmount = attackDamage - this.Opponent.ArmorRating(this.Player);
						var attackSucceedString = "You hit the " + this.Opponent.Name + " for " + attackAmount + " physical damage.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						this.Opponent.TakeDamage(attackAmount);
					}
					break;
				case "cast":
					try {
						if (this.Input[1] != null) {
							var spellName = InputHandler.ParseInput(this.Input);
							this.Player.CastSpell(this.Opponent, spellName);
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
					}
					catch (NullReferenceException) {
						if (this.Player.PlayerClass != Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					}
					catch (InvalidOperationException) {
						if (this.Player.PlayerClass != Player.PlayerClassType.Mage) {
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
							this.Player.UseAbility(this.Opponent, abilityName);
						}
						if (this.Input[1] != null && this.Input[1] == "bandage") {
							var abilityName = InputHandler.ParseInput(this.Input);
							this.Player.UseAbility(abilityName);
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
						if (this.Player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					catch (InvalidOperationException) {
						if (this.Player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
						switch (this.Player.PlayerClass) {
							case Player.PlayerClassType.Mage:
								break;
							case Player.PlayerClassType.Warrior:
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								if (this.Player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
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
					GearHandler.EquipItem(this.Player, this.Input);
					break;
				case "flee":
					this.FleeCombat();
					break;
				case "drink":
					if (this.Input.Last() == "potion") {
						this.Player.DrinkPotion(this.Input);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
					}
					break;
				case "reload":
					this.Player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerHandler.ShowInventory(this.Player);
					break;
				case "list":
					switch (this.Input[1]) {
						case "abilities":
							try {
								PlayerHandler.ListAbilities(this.Player);
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
								PlayerHandler.ListSpells(this.Player);
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
						PlayerHandler.AbilityInfo(this.Player, this.Input);
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
						PlayerHandler.SpellInfo(this.Player, this.Input[1]);
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
