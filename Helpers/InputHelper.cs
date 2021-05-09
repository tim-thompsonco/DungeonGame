using DungeonGame.Coordinates;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Equipment;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame.Helpers {
	public static class InputHelper {
		public static string[] GetFormattedInput(string userInput) {
			string inputFormatted = userInput.ToLower().Trim();
			string[] inputParse = inputFormatted.Split(' ');
			return inputParse;
		}
		public static string ParseInput(string[] userInput) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < userInput.Length; i++) {
				if (i == userInput.Length - 1) {
					// If last loop iteration, check to see if it's a number, and if so do not add to input string
					bool isNumber = int.TryParse(userInput.Last(), out _);
					if (isNumber) {
						continue;
					}
				}
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			string parsedInput = inputString.ToString().Trim();
			return parsedInput;
		}
		public static void ProcessUserInput(Player player, string[] input, Timer globalTimer) {
			IRoom playerRoom = RoomHelper.Rooms[player.PlayerLocation];
			TownRoom isTownRoom = playerRoom as TownRoom;
			int playerX = player.PlayerLocation.X;
			int playerY = player.PlayerLocation.Y;
			int playerZ = player.PlayerLocation.Z;
			switch (input[0]) {
				case "a":
				case "attack":
				case "kill":
					playerRoom.AttackOpponent(player, input, globalTimer);
					break;
				case "buy":
					try {
						if (input[1] != null) {
							try {
								bool quantityProvided = int.TryParse(input.Last(), out int quantity);
								if (!quantityProvided) {
									quantity = 1;
								} else {
									input = input.Take(input.Count() - 1).ToArray();
								}
								isTownRoom?.Vendor.BuyItem(player, input, quantity);
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to buy an item from.");
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Buy what?");
					}
					break;
				case "cast":
					try {
						if (input[1] != null && input[1].Contains("town")) {
							player.CastSpell(ParseInput(input));
						} else if (input[1] != null) {
							player.CastSpell(ParseInput(input));
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
					} catch (NullReferenceException) {
						if (player.PlayerClass != PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					} catch (InvalidOperationException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							player.PlayerClass != PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
					}
					break;
				case "drop":
					GearHelper.DropItem(player, input);
					break;
				case "pickup":
					GearHelper.PickupItem(player, input);
					break;
				case "use":
					try {
						if (input.Contains("distance")) {
							player.UseAbility(input);
						} else if (input.Contains("ambush")) {
							player.UseAbility(playerRoom.Monster, input);
							playerRoom.AttackOpponent(player, input, globalTimer);
						} else if (input[1] != null) {
							player.UseAbility(input);
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						Console.WriteLine();
					} catch (ArgumentOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
					} catch (NullReferenceException) {
						if (player.PlayerClass == PlayerClassType.Mage) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					} catch (InvalidOperationException) {
						switch (player.PlayerClass) {
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
							case PlayerClassType.Mage:
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't use abilities. You're not a warrior or archer!");
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					break;
				case "equip":
				case "unequip":
					GearHelper.EquipItem(player, input);
					break;
				case "enhance":
					int itemIndex = player.Inventory.FindIndex(f => f.Name.Contains(input[1]));
					switch (player.Inventory[itemIndex]) {
						case Weapon _:
							GearHelper.UseWeaponKit(player, input);
							break;
						case Armor _:
							GearHelper.UseArmorKit(player, input);
							break;
						default:
							Messages.InvalidCommand();
							break;
					}
					break;
				case "reload":
					player.ReloadQuiver();
					break;
				case "i":
				case "inventory":
					PlayerHelper.ShowInventory(player);
					break;
				case "q":
				case "quit":
					bool quitConfirm = GameHelper.QuitGame(player);
					if (quitConfirm) {
						globalTimer.Dispose();
					}
					break;
				case "list":
					switch (input[1]) {
						case "abilities":
							try {
								PlayerHelper.ListAbilities(player);
							} catch (IndexOutOfRangeException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
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
							}
							break;
					}
					break;
				case "ability":
					try {
						PlayerHelper.AbilityInfo(player, input);
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}
					break;
				case "spell":
					try {
						PlayerHelper.SpellInfo(player, input);
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}
					break;
				case "quest":
					try {
						PlayerHelper.QuestInfo(player, input);
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What quest did you want to know about?");
					}
					break;
				case "complete":
					try {
						if (input[1] != null) {
							try {
								if (isTownRoom?.Trainer != null) {
									isTownRoom?.Trainer.CompleteQuest(player, input);
								} else {
									isTownRoom?.Vendor.CompleteQuest(player, input);
								}
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no one to turn in quests to here.");
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Complete what quest?");
					}
					break;
				case "l":
				case "look":
					try {
						if (input[1] != null) {
							PlayerHelper.LookAtObject(player, input);
						}
					} catch (IndexOutOfRangeException) {
						playerRoom.LookRoom();
					}
					break;
				case "loot":
					try {
						if (input[1] != null) {
							try {
								playerRoom.LootCorpse(player, input);
							} catch (Exception) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looting.");
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Loot what?");
					}
					break;
				case "drink":
					if (input.Last() == "potion") {
						player.AttemptDrinkPotion(ParseInput(input));
					} else {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
					}
					break;
				case "save":
					GameHelper.SaveGame(player);
					break;
				case "restore":
					isTownRoom?.Vendor.RestorePlayer(player);
					break;
				case "help":
					Messages.ShowCommandHelp();
					break;
				case "sell":
					try {
						if (input[1] != null) {
							try {
								isTownRoom?.Vendor.SellItem(player, input);
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"The vendor doesn't want that.");
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Sell what?");
					}
					break;
				case "repair":
					try {
						if (input[1] != null) {
							if (isTownRoom != null) {
								if (input[1] == "all") {
									foreach (IItem item in player.Inventory) {
										if (!(item is IEquipment itemToRepair && itemToRepair.Equipped)) {
											continue;
										}

										string[] itemNameArray = new[] { input[0], item.Name };
										isTownRoom.Vendor.RepairItem(player, itemNameArray, true);
									}
									break;
								}
								isTownRoom.Vendor.RepairItem(player, input, false);
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Repair what?");
					} catch (NullReferenceException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no vendor here!");
					}
					break;
				case "upgrade":
					try {
						if (input[1] != null) {
							if (isTownRoom != null) {
								if (player.PlayerClass == PlayerClassType.Mage) {
									isTownRoom.Trainer.UpgradeSpell(player, ParseInput(input));
								} else {
									isTownRoom.Trainer.UpgradeAbility(player, ParseInput(input));
								}
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Upgrade what?");
					} catch (NullReferenceException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "train":
					try {
						if (input[1] != null) {
							if (isTownRoom != null) {
								if (player.PlayerClass == PlayerClassType.Mage) {
									isTownRoom.Trainer.TrainSpell(player, ParseInput(input));
								} else {
									isTownRoom.Trainer.TrainAbility(player, ParseInput(input));
								}
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Train what?");
					} catch (NullReferenceException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "consider":
					try {
						if (isTownRoom?.Trainer != null) {
							isTownRoom.Trainer.OfferQuest(player, input);
						} else {
							isTownRoom?.Vendor.OfferQuest(player, input);
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Consider what quest?");
					} catch (NullReferenceException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no quest giver here!");
					}
					break;
				case "show":
					try {
						if (input[1].Contains("forsale")) {
							try {
								isTownRoom?.Vendor.DisplayGearForSale();
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to show inventory available for sale.");
							}
						}
						if (input[1].Contains("upgrade")) {
							try {
								isTownRoom?.Trainer.DisplayAvailableUpgrades(player);
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no trainer in the room to show available upgrades.");
							}
						}
						if (input[1].Contains("quests")) {
							try {
								if (isTownRoom?.Trainer != null) {
									isTownRoom.Trainer.ShowQuestList(player);
								} else {
									isTownRoom?.Vendor.ShowQuestList(player);
								}
							} catch (NullReferenceException) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no one in the room to show quests.");
							}
						}
					} catch (IndexOutOfRangeException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Show what?");
					}
					break;
				case "n":
				case "north":
					if (playerRoom.North != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX, playerY + 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "s":
				case "south":
					if (playerRoom.South != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX, playerY - 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "e":
				case "east":
					if (playerRoom.East != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX + 1, playerY, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "w":
				case "west":
					if (playerRoom.West != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX - 1, playerY, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "ne":
				case "northeast":
					if (playerRoom.NorthEast != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "nw":
				case "northwest":
					if (playerRoom.NorthWest != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "se":
				case "southeast":
					if (playerRoom.SouthEast != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "sw":
				case "southwest":
					if (playerRoom.SouthWest != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "u":
				case "up":
					if (playerRoom.Up != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX, playerY, playerZ + 1);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				case "d":
				case "down":
					if (playerRoom.Down != null) {
						try {
							Coordinate newCoord = new Coordinate(playerX, playerY, playerZ - 1);
							RoomHelper.ChangeRoom(player, newCoord);
						} catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					} else {
						Messages.InvalidDirection();
					}
					break;
				default:
					Messages.InvalidCommand();
					break;
			}
		}
	}
}