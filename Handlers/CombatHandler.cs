using System;
using System.Linq;

namespace DungeonGame
{
	public class CombatHandler
	{
		private string[] Input { get; set; }
		private Monster Opponent { get; set; }
		private Player Player { get; set; }
		private bool FleeSuccess { get; set; }

		public CombatHandler(Monster opponent, Player player)
		{
			this.Player = player;
			this.Opponent = opponent;
			this.Player._InCombat = true;
			this.Opponent._InCombat = true;
		}

		public void StartCombat()
		{
			Console.Clear();
			var fightStartString = this.Player._Name + ", you have encountered a " + this.Opponent._Name + ". Time to fight!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			while (this.Opponent._HitPoints > 0 && this.Player._HitPoints > 0 &&
				   this.Player._InCombat && this.Opponent._InCombat)
			{
				GameHandler.RemovedExpiredEffectsAsync(this.Player);
				GameHandler.RemovedExpiredEffectsAsync(this.Opponent);
				var isInputValid = false;
				// Get input and check to see if input is valid, and if not, keep trying to get input from user
				while (!isInputValid)
				{
					// Show initial output that announces start of fight
					OutputHandler.ShowUserOutput(this.Player, this.Opponent);
					OutputHandler.Display.ClearUserOutput();
					// Player will attack, use ability, cast spells, etc. to cause damage
					this.Input = InputHandler.GetFormattedInput(Console.ReadLine());
					Console.Clear();
					isInputValid = this.ProcessPlayerInput();
				}
				if (this.Player._Effects.Any())
				{
					this.ProcessPlayerEffects();
				}
				if (this.FleeSuccess) return;
				// Check to see if player attack killed monster
				if (this.Opponent._HitPoints <= 0)
				{
					this.Opponent.MonsterDeath(this.Player);
					return;
				}
				if (this.Opponent._Effects.Any())
				{
					this.ProcessOpponentEffects();
				}
				// Check to see if damage over time effects killed monster
				if (this.Opponent._HitPoints <= 0)
				{
					this.Opponent.MonsterDeath(this.Player);
					return;
				}
				if (this.Opponent._IsStunned) continue;
				this.Opponent.Attack(this.Player);
				// Check at end of round to see if monster was killed by combat round
				if (this.Opponent._HitPoints > 0) continue;
				this.Opponent.MonsterDeath(this.Player);
				return;
			}
		}
		private void FleeCombat()
		{
			var randomNum = GameHandler.GetRandomNumber(1, 10);
			if (randomNum > 5)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				this.Player._InCombat = false;
				this.Opponent._InCombat = false;
				this.FleeSuccess = true;
				var playerRoom = RoomHandler.Rooms[this.Player._PlayerLocation];
				var playerX = this.Player._PlayerLocation._X;
				var playerY = this.Player._PlayerLocation._Y;
				var playerZ = this.Player._PlayerLocation._Z;
				if (playerRoom._Up != null)
				{
					var newCoord = new Coordinate(playerX, playerY, playerZ + 1);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._East != null)
				{
					var newCoord = new Coordinate(playerX + 1, playerY, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._West != null)
				{
					var newCoord = new Coordinate(playerX - 1, playerY, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._North != null)
				{
					var newCoord = new Coordinate(playerX, playerY + 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._South != null)
				{
					var newCoord = new Coordinate(playerX, playerY - 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._NorthEast != null)
				{
					var newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._NorthWest != null)
				{
					var newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._SouthEast != null)
				{
					var newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._SouthWest != null)
				{
					var newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
				if (playerRoom._Down != null)
				{
					var newCoord = new Coordinate(playerX, playerY, playerZ - 1);
					RoomHandler.ChangeRoom(this.Player, newCoord);
					return;
				}
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
		}
		private void ProcessPlayerEffects()
		{
			foreach (var effect in this.Player._Effects)
			{
				switch (effect._EffectGroup)
				{
					case Effect.EffectType.Healing:
						effect.HealingRound(this.Player);
						break;
					case Effect.EffectType.ChangePlayerDamage:
						effect.ChangePlayerDamageRound(this.Player);
						break;
					case Effect.EffectType.ChangeArmor:
						effect.ChangeArmorRound();
						break;
					case Effect.EffectType.OnFire:
						effect.OnFireRound(this.Player);
						break;
					case Effect.EffectType.Bleeding:
						effect.BleedingRound(this.Player);
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						effect.FrozenRound(this.Player);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		private void ProcessOpponentEffects()
		{
			GameHandler.RemovedExpiredEffectsAsync(this.Opponent);
			foreach (var effect in this.Opponent._Effects)
			{
				switch (effect._EffectGroup)
				{
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						effect.OnFireRound(this.Opponent);
						break;
					case Effect.EffectType.Bleeding:
						effect.BleedingRound(this.Opponent);
						break;
					case Effect.EffectType.Stunned:
						effect.StunnedRound(this.Opponent);
						break;
					case Effect.EffectType.Frozen:
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		private bool ProcessPlayerInput()
		{
			switch (this.Input[0])
			{
				case "f":
				case "fight":
					var attackDamage = this.Player.PhysicalAttack(this.Opponent);
					if (attackDamage - this.Opponent.ArmorRating(this.Player) <= 0)
					{
						var armorAbsorbString = "The " + this.Opponent._Name + "'s armor absorbed all of your attack!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					}
					else if (attackDamage == 0)
					{
						var attackFailString = "You missed " + this.Opponent._Name + "!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					}
					else
					{
						var attackAmount = attackDamage - this.Opponent.ArmorRating(this.Player);
						var attackSucceedString = "You hit the " + this.Opponent._Name + " for " + attackAmount + " physical damage.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						this.Opponent._HitPoints -= attackAmount;
					}
					break;
				case "cast":
					try
					{
						if (this.Input[1] != null)
						{
							var spellName = InputHandler.ParseInput(this.Input);
							this.Player.CastSpell(this.Opponent, spellName);
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
						return false;
					}
					catch (NullReferenceException)
					{
						if (this.Player._PlayerClass != Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
							return false;
						}
					}
					catch (InvalidOperationException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							this.Player._PlayerClass != Player.PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
						return false;
					}
					break;
				case "use":
					try
					{
						if (this.Input[1] != null && this.Input[1] != "bandage")
						{
							this.Player.UseAbility(this.Opponent, this.Input);
						}
						if (this.Input[1] != null && this.Input[1] == "bandage")
						{
							this.Player.UseAbility(this.Input);
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					}
					catch (ArgumentOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					}
					catch (NullReferenceException)
					{
						if (this.Player._PlayerClass == Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
							return false;
						}
					}
					catch (InvalidOperationException)
					{
						switch (this.Player._PlayerClass)
						{
							case Player.PlayerClassType.Mage:
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								break;
							case Player.PlayerClassType.Warrior:
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									this.Player._PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow
										? "You do not have a bow equipped!"
										: "You do not have enough combo points to use that ability!");
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						return false;
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
					if (this.Input.Last() == "potion")
					{
						this.Player.DrinkPotion(InputHandler.ParseInput(this.Input));
					}
					else
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
						return false;
					}
					break;
				case "reload":
					this.Player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerHandler.ShowInventory(this.Player);
					return false;
				case "list":
					switch (this.Input[1])
					{
						case "abilities":
							try
							{
								PlayerHandler.ListAbilities(this.Player);
							}
							catch (IndexOutOfRangeException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
								return false;
							}
							break;
						case "spells":
							try
							{
								PlayerHandler.ListSpells(this.Player);
							}
							catch (IndexOutOfRangeException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
								return false;
							}
							break;
					}
					return false;
				case "ability":
					try
					{
						PlayerHandler.AbilityInfo(this.Player, this.Input);
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}
					catch (NullReferenceException)
					{
						if (this.Player._PlayerClass == Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					return false;
				case "spell":
					try
					{
						PlayerHandler.SpellInfo(this.Player, this.Input);
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}
					catch (NullReferenceException)
					{
						if (this.Player._PlayerClass != Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use spells. You're not a mage!");
						}
					}
					return false;
				default:
					Messages.InvalidCommand();
					return false;
			}
			return true;
		}
	}
}
