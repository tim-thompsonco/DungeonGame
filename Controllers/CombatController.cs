using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Linq;

namespace DungeonGame.Controllers {
	public class CombatController {
		private string[] _Input { get; set; }
		private Monster _Opponent { get; set; }
		private Player _Player { get; set; }
		private bool _FleeSuccess { get; set; }

		public CombatController(Monster opponent, Player player) {
			_Player = player;
			_Opponent = opponent;
			_Player._InCombat = true;
			_Opponent._InCombat = true;
		}

		public void StartCombat() {
			Console.Clear();
			string fightStartString = $"{_Player._Name}, you have encountered a {_Opponent._Name}. Time to fight!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);
			while (_Opponent._HitPoints > 0 && _Player._HitPoints > 0 &&
				   _Player._InCombat && _Opponent._InCombat) {
				GameController.RemovedExpiredEffectsAsync(_Player);
				GameController.RemovedExpiredEffectsAsync(_Opponent);
				bool isInputValid = false;
				// Get input and check to see if input is valid, and if not, keep trying to get input from user
				while (!isInputValid) {
					// Show initial output that announces start of fight
					OutputController.ShowUserOutput(_Player, _Opponent);
					OutputController.Display.ClearUserOutput();
					// Player will attack, use ability, cast spells, etc. to cause damage
					_Input = InputController.GetFormattedInput(Console.ReadLine());
					Console.Clear();
					isInputValid = ProcessPlayerInput();
				}
				if (_Player._Effects.Any()) {
					ProcessPlayerEffects();
				}
				if (_FleeSuccess) {
					return;
				}
				// Check to see if player attack killed monster
				if (_Opponent._HitPoints <= 0) {
					_Opponent.MonsterDeath(_Player);
					return;
				}
				if (_Opponent._Effects.Any()) {
					ProcessOpponentEffects();
				}
				// Check to see if damage over time effects killed monster
				if (_Opponent._HitPoints <= 0) {
					_Opponent.MonsterDeath(_Player);
					return;
				}
				if (_Opponent._IsStunned) {
					continue;
				}

				_Opponent.Attack(_Player);
				// Check at end of round to see if monster was killed by combat round
				if (_Opponent._HitPoints > 0) {
					continue;
				}

				_Opponent.MonsterDeath(_Player);
				return;
			}
		}
		private void FleeCombat() {
			int randomNum = GameController.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				_Player._InCombat = false;
				_Opponent._InCombat = false;
				_FleeSuccess = true;
				IRoom playerRoom = RoomController._Rooms[_Player._PlayerLocation];
				int playerX = _Player._PlayerLocation._X;
				int playerY = _Player._PlayerLocation._Y;
				int playerZ = _Player._PlayerLocation._Z;
				if (playerRoom._Up != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ + 1);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._East != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._West != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._North != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY + 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._South != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY - 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._NorthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._NorthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._SouthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._SouthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
				if (playerRoom._Down != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ - 1);
					RoomController.ChangeRoom(_Player, newCoord);
					return;
				}
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
		}

		private void ProcessPlayerEffects() {
			foreach (IEffect effect in _Player._Effects) {
				if (effect is HealingEffect healingEffect) {
					healingEffect.ProcessHealingRound(_Player);
				}

				if (effect is ChangePlayerDamageEffect changePlayerDmgEffect) {
					changePlayerDmgEffect.ProcessChangePlayerDamageRound(_Player);
				}

				if (effect is ChangeArmorEffect changeArmorEffect) {
					changeArmorEffect.ProcessChangeArmorRound();
				}

				if (effect is BurningEffect burningEffect) {
					burningEffect.ProcessBurningRound(_Player);
				}

				if (effect is BleedingEffect bleedingEffect) {
					bleedingEffect.ProcessBleedingRound(_Player);
				}

				if (effect is FrozenEffect frozenEffect) {
					frozenEffect.ProcessFrozenRound();
				}
			}
		}

		private void ProcessOpponentEffects() {
			GameController.RemovedExpiredEffectsAsync(_Opponent);

			foreach (IEffect effect in _Opponent._Effects) {
				if (effect is BurningEffect burningEffect) {
					burningEffect.ProcessBurningRound(_Opponent);
				}

				if (effect is BleedingEffect bleedingEffect) {
					bleedingEffect.ProcessBleedingRound(_Opponent);
				}

				if (effect is StunnedEffect stunnedEffect) {
					stunnedEffect.ProcessStunnedRound(_Opponent);
				}
			}
		}

		private bool ProcessPlayerInput() {
			switch (_Input[0]) {
				case "f":
				case "fight":
					int attackDamage = _Player.PhysicalAttack(_Opponent);
					if (attackDamage - _Opponent.ArmorRating(_Player) <= 0) {
						string armorAbsorbString = $"The {_Opponent._Name}'s armor absorbed all of your attack!";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					} else if (attackDamage == 0) {
						string attackFailString = $"You missed {_Opponent._Name}!";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					} else {
						int attackAmount = attackDamage - _Opponent.ArmorRating(_Player);
						string attackSucceedString = $"You hit the {_Opponent._Name} for {attackAmount} physical damage.";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						_Opponent._HitPoints -= attackAmount;
					}
					break;
				case "cast":
					try {
						if (_Input[1] != null) {
							string spellName = InputController.ParseInput(_Input);
							_Player.CastSpell(_Opponent, spellName);
						}
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
						return false;
					} catch (NullReferenceException) {
						if (_Player._PlayerClass != Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
							return false;
						}
					} catch (InvalidOperationException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							_Player._PlayerClass != Player.PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
						return false;
					}
					break;
				case "use":
					try {
						if (_Input[1] != null && _Input[1] != "bandage") {
							_Player.UseAbility(_Opponent, _Input);
						}
						if (_Input[1] != null && _Input[1] == "bandage") {
							_Player.UseAbility(_Input);
						}
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					} catch (ArgumentOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					} catch (NullReferenceException) {
						if (_Player._PlayerClass == Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
							return false;
						}
					} catch (InvalidOperationException) {
						switch (_Player._PlayerClass) {
							case Player.PlayerClassType.Mage:
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								break;
							case Player.PlayerClassType.Warrior:
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									_Player._PlayerWeapon._WeaponGroup != Weapon.WeaponType.Bow
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
					GearController.EquipItem(_Player, _Input);
					break;
				case "flee":
					FleeCombat();
					break;
				case "drink":
					if (_Input.Last() == "potion") {
						_Player.AttemptDrinkPotion(InputController.ParseInput(_Input));
					} else {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
						return false;
					}
					break;
				case "reload":
					_Player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerController.ShowInventory(_Player);
					return false;
				case "list":
					switch (_Input[1]) {
						case "abilities":
							try {
								PlayerController.ListAbilities(_Player);
							} catch (IndexOutOfRangeException) {
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
								return false;
							}
							break;
						case "spells":
							try {
								PlayerController.ListSpells(_Player);
							} catch (IndexOutOfRangeException) {
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
								return false;
							}
							break;
					}
					return false;
				case "ability":
					try {
						PlayerController.AbilityInfo(_Player, _Input);
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					} catch (NullReferenceException) {
						if (_Player._PlayerClass == Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					return false;
				case "spell":
					try {
						PlayerController.SpellInfo(_Player, _Input);
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					} catch (NullReferenceException) {
						if (_Player._PlayerClass != Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
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
