using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Linq;

namespace DungeonGame.Controllers {
	public static class CombatController {
		private static string[] Input { get; set; }
		private static bool FleeSuccess { get; set; }

		public static void StartCombat(Player player, Monster monster) {
			Console.Clear();

			string fightStartString = $"{player.Name}, you have encountered a {monster.Name}. Time to fight!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				fightStartString);

			while (monster.HitPoints > 0 && player.HitPoints > 0 &&
				   player.InCombat && monster.InCombat) {
				GameController.RemovedExpiredEffectsAsync(player);
				GameController.RemovedExpiredEffectsAsync(monster);

				bool isInputValid = false;
				// Get input and check to see if input is valid, and if not, keep trying to get input from user
				while (!isInputValid) {
					// Show initial output that announces start of fight
					OutputController.ShowUserOutput(player, monster);
					OutputController.Display.ClearUserOutput();
					// Player will attack, use ability, cast spells, etc. to cause damage
					Input = InputController.GetFormattedInput(Console.ReadLine());
					Console.Clear();
					isInputValid = ProcessPlayerInput(player, monster);
				}

				if (player.Effects.Any()) {
					ProcessPlayerEffects(player);
				}

				if (FleeSuccess) {
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
			int randomNum = GameController.GetRandomNumber(1, 10);
			if (randomNum > 5) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You have fled combat successfully!");
				player.InCombat = false;
				monster.InCombat = false;
				FleeSuccess = true;
				IRoom playerRoom = RoomController._Rooms[player.PlayerLocation];
				int playerX = player.PlayerLocation.X;
				int playerY = player.PlayerLocation.Y;
				int playerZ = player.PlayerLocation.Z;
				if (playerRoom._Up != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ + 1);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._East != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._West != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._North != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY + 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._South != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY - 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._NorthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._NorthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._SouthEast != null) {
					Coordinate newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._SouthWest != null) {
					Coordinate newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
				if (playerRoom._Down != null) {
					Coordinate newCoord = new Coordinate(playerX, playerY, playerZ - 1);
					RoomController.ChangeRoom(player, newCoord);
					return;
				}
			}
			OutputController.Display.StoreUserOutput(
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

				if (effect is ReflectDamageEffect reflectDmgEffect) {
					int baseSpellDamage = attackDamage;

					attackDamage = reflectDmgEffect.GetReflectedDamageAmount(attackDamage);

					reflectDmgEffect.ProcessReflectDamageRound(baseSpellDamage);
				}

				if (attackDamage <= 0) {
					string effectAbsorbString = $"Your {effect.Name} absorbed all of {monster.Name}'s attack!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);

					return 0;
				}
			}

			return attackDamage;
		}

		private static void ProcessPlayerEffects(Player player) {
			GameController.RemovedExpiredEffectsAsync(player);

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
			GameController.RemovedExpiredEffectsAsync(monster);

			foreach (IEffect effect in monster.Effects) {
				if (effect is BurningEffect burningEffect) {
					burningEffect.ProcessBurningRound(monster);
				} else {
					effect.ProcessRound();
				}
			}
		}

		private static bool ProcessPlayerInput(Player player, Monster monster) {
			switch (Input[0]) {
				case "f":
				case "fight":
					int attackDamage = player.PhysicalAttack(monster);
					if (attackDamage - monster.ArmorRating(player) <= 0) {
						string armorAbsorbString = $"The {monster.Name}'s armor absorbed all of your attack!";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							armorAbsorbString);
					} else if (attackDamage == 0) {
						string attackFailString = $"You missed {monster.Name}!";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							attackFailString);
					} else {
						int attackAmount = attackDamage - monster.ArmorRating(player);
						string attackSucceedString = $"You hit the {monster.Name} for {attackAmount} physical damage.";
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							attackSucceedString);
						monster.HitPoints -= attackAmount;
					}
					break;
				case "cast":
					try {
						if (Input[1] != null) {
							string spellName = InputController.ParseInput(Input);
							player.CastSpell(monster, spellName);
						}
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
						return false;
					} catch (NullReferenceException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
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
							player.PlayerClass != Player.PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
						return false;
					}
					break;
				case "use":
					try {
						if (Input[1] != null && Input[1] != "bandage") {
							player.UseAbility(monster, Input);
						}
						if (Input[1] != null && Input[1] == "bandage") {
							player.UseAbility(Input);
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
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
							return false;
						}
					} catch (InvalidOperationException) {
						switch (player.PlayerClass) {
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
									player.PlayerWeapon._WeaponGroup != Weapon.WeaponType.Bow
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
					GearController.EquipItem(player, Input);
					break;
				case "flee":
					FleeCombat(player, monster);
					break;
				case "drink":
					if (Input.Last() == "potion") {
						player.AttemptDrinkPotion(InputController.ParseInput(Input));
					} else {
						OutputController.Display.StoreUserOutput(
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
					PlayerController.ShowInventory(player);
					return false;
				case "list":
					switch (Input[1]) {
						case "abilities":
							try {
								PlayerController.ListAbilities(player);
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
								PlayerController.ListSpells(player);
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
						PlayerController.AbilityInfo(player, Input);
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					} catch (NullReferenceException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					return false;
				case "spell":
					try {
						PlayerController.SpellInfo(player, Input);
					} catch (IndexOutOfRangeException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					} catch (NullReferenceException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
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
