using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Linq;

namespace DungeonGame.Helpers {
	public static class CombatHelper {
		private static string[] _input;
		private static bool _fleeSuccess;

		public static void StartCombat(Player player, Monster monster) {
			Console.Clear();

			string fightStartString = $"{player.Name}, you have encountered a {monster.Name}. Time to fight!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);

			while (monster.HitPoints > 0 && player.HitPoints > 0 &&
				   player.InCombat && monster.InCombat) {
				GameHelper.RemovedExpiredEffectsAsync(player);
				GameHelper.RemovedExpiredEffectsAsync(monster);

				bool isInputValid = false;
				// Get input and check to see if input is valid, and if not, keep trying to get input from user
				while (!isInputValid) {
					// Show initial output that announces start of fight
					OutputHelper.ShowUserOutput(player, monster);
					OutputHelper.Display.ClearUserOutput();
					// Player will attack, use ability, cast spells, etc. to cause damage
					_input = InputHelper.GetFormattedInput(Console.ReadLine());
					Console.Clear();
					isInputValid = ProcessPlayerInput(player, monster);
				}

				if (player.Effects.Any()) {
					ProcessPlayerEffects(player);
				}

				if (_fleeSuccess) {
					return;
				}

				// Check to see if player attack killed monster
				if (monster.HitPoints <= 0) {
					monster.MonsterDeath(player);
					return;
				}

				if (monster.Effects.Any()) {
					ProcessOpponentEffects(monster);
				}

				// Check to see if damage over time effects killed monster
				if (monster.HitPoints <= 0) {
					monster.MonsterDeath(player);
					return;
				}

				if (monster.IsStunned) {
					continue;
				}

				monster.Attack(player);

				// Check at end of round to see if monster was killed by combat round
				if (monster.HitPoints > 0) {
					continue;
				}

				monster.MonsterDeath(player);

				return;
			}
		}

		private static void FleeCombat(Player player, Monster monster) {
			int randomNum = GameHelper.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				monster.InCombat = false;
				_fleeSuccess = true;
				IRoom playerRoom = RoomHelper.Rooms[player.PlayerLocation];
				int playerX = player.PlayerLocation.X;
				int playerY = player.PlayerLocation.Y;
				int playerZ = player.PlayerLocation.Z;
				if (playerRoom.Up != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ + 1);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.East != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.West != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.North != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY + 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.South != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY - 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.NorthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.NorthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.SouthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.SouthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom.Down != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ - 1);
					RoomHelper.ChangeRoom(player, newCoord);
					return;
				}
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You tried to flee combat but failed!");
		}

		public static int GetMonsterAttackDamageUpdatedFromPlayerEffects(Player player, Monster monster, int attackDamage) {
			foreach (IEffect effect in player.Effects.ToList().Where(effect => effect.IsEffectExpired is false)) {
				if (effect is FrozenEffect frozenEffect) {
					attackDamage = frozenEffect.GetIncreasedDamageFromFrozen(attackDamage);

					frozenEffect.ProcessFrozenRound();
				}

				if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
					attackDamage = changeMonsterDmgEffect.GetUpdatedDamageFromChange(attackDamage);

					changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
				}

				if (effect is IChangeDamageEffect changeDamageEffect) {
					int baseSpellDamage = attackDamage;

					attackDamage = changeDamageEffect.GetChangedDamageFromEffect(attackDamage);

					changeDamageEffect.ProcessChangeDamageRound(baseSpellDamage);
				}

				if (attackDamage <= 0) {
					string effectAbsorbString = $"Your {effect.Name} absorbed all of {monster.Name}'s attack!";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);

					return 0;
				}
			}

			return attackDamage;
		}

		private static void ProcessPlayerEffects(Player player) {
			GameHelper.RemovedExpiredEffectsAsync(player);

			foreach (IEffect effect in player.Effects) {
				if (effect is HealingEffect healingEffect) {
					healingEffect.ProcessHealingRound(player);
				}

				if (effect is ChangePlayerDamageEffect changePlayerDmgEffect) {
					changePlayerDmgEffect.ProcessChangePlayerDamageRound(player);
				}

				if (effect is ChangeArmorEffect changeArmorEffect) {
					changeArmorEffect.ProcessChangeArmorRound();
				}

				if (effect is BurningEffect burningEffect) {
					burningEffect.ProcessBurningRound(player);
				}

				if (effect is BleedingEffect bleedingEffect) {
					bleedingEffect.ProcessRound();
				}

				if (effect is FrozenEffect frozenEffect) {
					frozenEffect.ProcessFrozenRound();
				}
			}
		}

		private static void ProcessOpponentEffects(Monster monster) {
			GameHelper.RemovedExpiredEffectsAsync(monster);

			foreach (IEffect effect in monster.Effects) {
				if (effect is BurningEffect burningEffect) {
					burningEffect.ProcessBurningRound(monster);
				} else {
					effect.ProcessRound();
				}
			}
		}

		private static bool ProcessPlayerInput(Player player, Monster monster) {
			switch (_input[0]) {
				case "f":
				case "fight":
					int attackDamage = player.PhysicalAttack(monster);
					if (attackDamage - monster.ArmorRating(player) <= 0) {
						string armorAbsorbString = $"The {monster.Name}'s armor absorbed all of your attack!";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					} else if (attackDamage == 0) {
						string attackFailString = $"You missed {monster.Name}!";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					} else {
						int attackAmount = attackDamage - monster.ArmorRating(player);
						string attackSucceedString = $"You hit the {monster.Name} for {attackAmount} physical damage.";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						monster.HitPoints -= attackAmount;
					}
					break;
				case "cast":
					try {
						if (_input[1] != null) {
							string spellName = InputHelper.ParseInput(_input);
							player.CastSpell(monster, spellName);
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
						return false;
					} catch (NullReferenceException) {
						if (player.PlayerClass != PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
							return false;
						}
					} catch (InvalidOperationException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							player.PlayerClass != PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
						return false;
					}
					break;
				case "use":
					try {
						if (_input[1] != null && _input[1] != "bandage") {
							player.UseAbility(monster, _input);
						}
						if (_input[1] != null && _input[1] == "bandage") {
							player.UseAbility(_input);
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					} catch (ArgumentOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						return false;
					} catch (NullReferenceException) {
						if (player.PlayerClass == PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
							return false;
						}
					} catch (InvalidOperationException) {
						switch (player.PlayerClass) {
							case PlayerClassType.Mage:
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								break;
							case PlayerClassType.Warrior:
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case PlayerClassType.Archer:
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									player.PlayerWeapon.WeaponGroup != WeaponType.Bow
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
					GearHelper.EquipItem(player, _input);
					break;
				case "flee":
					FleeCombat(player, monster);
					break;
				case "drink":
					if (_input.Last() == "potion") {
						player.AttemptDrinkPotion(InputHelper.ParseInput(_input));
					} else {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
						return false;
					}
					break;
				case "reload":
					player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerHelper.ShowInventory(player);
					return false;
				case "list":
					switch (_input[1]) {
						case "abilities":
							try {
								PlayerHelper.ListAbilities(player);
							} catch (IndexOutOfRangeException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
								return false;
							}
							break;
						case "spells":
							try {
								PlayerHelper.ListSpells(player);
							} catch (IndexOutOfRangeException) {
								OutputHelper.Display.StoreUserOutput(
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
						PlayerHelper.AbilityInfo(player, _input);
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					} catch (NullReferenceException) {
						if (player.PlayerClass == PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					return false;
				case "spell":
					try {
						PlayerHelper.SpellInfo(player, _input);
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					} catch (NullReferenceException) {
						if (player.PlayerClass != PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
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
