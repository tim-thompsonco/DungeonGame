using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame
{
	public static class InputHandler
	{
		public static string[] GetFormattedInput(string userInput)
		{
			var inputFormatted = userInput.ToLower().Trim();
			var inputParse = inputFormatted.Split(' ');
			return inputParse;
		}
		public static string ParseInput(string[] userInput)
		{
			var inputString = new StringBuilder();
			for (var i = 1; i < userInput.Length; i++)
			{
				if (i == userInput.Length - 1)
				{
					// If last loop iteration, check to see if it's a number, and if so do not add to input string
					var isNumber = int.TryParse(userInput.Last(), out var number);
					if (isNumber)
					{
						continue;
					}
				}
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var parsedInput = inputString.ToString().Trim();
			return parsedInput;
		}
		public static void ProcessUserInput(Player player, string[] input, Timer globalTimer)
		{
			var playerRoom = RoomHandler.Rooms[player._PlayerLocation];
			var isTownRoom = playerRoom as TownRoom;
			var playerX = player._PlayerLocation._X;
			var playerY = player._PlayerLocation._Y;
			var playerZ = player._PlayerLocation._Z;
			switch (input[0])
			{
				case "a":
				case "attack":
				case "kill":
					playerRoom.AttackOpponent(player, input, globalTimer);
					break;
				case "buy":
					try
					{
						if (input[1] != null)
						{
							try
							{
								var quantityProvided = int.TryParse(input.Last(), out var quantity);
								if (!quantityProvided)
								{
									quantity = 1;
								}
								else
								{
									input = input.Take(input.Count() - 1).ToArray();
								}
								isTownRoom?._Vendor.BuyItem(player, input, quantity);
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to buy an item from.");
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Buy what?");
					}
					break;
				case "cast":
					try
					{
						if (input[1] != null && input[1].Contains("town"))
						{
							player.CastSpell(ParseInput(input));
						}
						else if (input[1] != null)
						{
							player.CastSpell(ParseInput(input));
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
					}
					catch (NullReferenceException)
					{
						if (player._PlayerClass != Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					}
					catch (InvalidOperationException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							player._PlayerClass != Player.PlayerClassType.Mage
								? "You can't cast spells. You're not a mage!"
								: "You do not have enough mana to cast that spell!");
					}
					break;
				case "drop":
					GearHandler.DropItem(player, input);
					break;
				case "pickup":
					GearHandler.PickupItem(player, input);
					break;
				case "use":
					try
					{
						if (input.Contains("distance"))
						{
							player.UseAbility(input);
						}
						else if (input.Contains("ambush"))
						{
							player.UseAbility(playerRoom._Monster, input);
							playerRoom.AttackOpponent(player, input, globalTimer);
						}
						else if (input[1] != null)
						{
							player.UseAbility(input);
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						Console.WriteLine();
					}
					catch (ArgumentOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
					}
					catch (NullReferenceException)
					{
						if (player._PlayerClass == Player.PlayerClassType.Mage)
						{
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					catch (InvalidOperationException)
					{
						switch (player._PlayerClass)
						{
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
									player._PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow
										? "You do not have a bow equipped!"
										: "You do not have enough combo points to use that ability!");
								break;
							case Player.PlayerClassType.Mage:
								OutputHandler.Display.StoreUserOutput(
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
					GearHandler.EquipItem(player, input);
					break;
				case "enhance":
					var itemIndex = player._Inventory.FindIndex(f => f._Name.Contains(input[1]));
					switch (player._Inventory[itemIndex])
					{
						case Weapon _:
							GearHandler.UseWeaponKit(player, input);
							break;
						case Armor _:
							GearHandler.UseArmorKit(player, input);
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
					PlayerHandler.ShowInventory(player);
					break;
				case "q":
				case "quit":
					var quitConfirm = GameHandler.QuitGame(player);
					if (quitConfirm)
					{
						globalTimer.Dispose();
					}
					break;
				case "list":
					switch (input[1])
					{
						case "abilities":
							try
							{
								PlayerHandler.ListAbilities(player);
							}
							catch (IndexOutOfRangeException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
							}
							break;
						case "spells":
							try
							{
								PlayerHandler.ListSpells(player);
							}
							catch (IndexOutOfRangeException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
							}
							break;
					}
					break;
				case "ability":
					try
					{
						PlayerHandler.AbilityInfo(player, input);
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}
					break;
				case "spell":
					try
					{
						PlayerHandler.SpellInfo(player, input);
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}
					break;
				case "quest":
					try
					{
						PlayerHandler.QuestInfo(player, input);
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What quest did you want to know about?");
					}
					break;
				case "complete":
					try
					{
						if (input[1] != null)
						{
							try
							{
								if (isTownRoom?._Trainer != null)
								{
									isTownRoom?._Trainer.CompleteQuest(player, input);
								}
								else
								{
									isTownRoom?._Vendor.CompleteQuest(player, input);
								}
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no one to turn in quests to here.");
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Complete what quest?");
					}
					break;
				case "l":
				case "look":
					try
					{
						if (input[1] != null)
						{
							PlayerHandler.LookAtObject(player, input);
						}
					}
					catch (IndexOutOfRangeException)
					{
						playerRoom.LookRoom();
					}
					break;
				case "loot":
					try
					{
						if (input[1] != null)
						{
							try
							{
								playerRoom.LootCorpse(player, input);
							}
							catch (Exception)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looting.");
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Loot what?");
					}
					break;
				case "drink":
					if (input.Last() == "potion")
					{
						player.DrinkPotion(ParseInput(input));
					}
					else
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
					}
					break;
				case "save":
					GameHandler.SaveGame(player);
					break;
				case "restore":
					isTownRoom?._Vendor.RestorePlayer(player);
					break;
				case "help":
					Messages.ShowCommandHelp();
					break;
				case "sell":
					try
					{
						if (input[1] != null)
						{
							try
							{
								isTownRoom?._Vendor.SellItem(player, input);
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"The vendor doesn't want that.");
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Sell what?");
					}
					break;
				case "repair":
					try
					{
						if (input[1] != null)
						{
							if (isTownRoom != null)
							{
								if (input[1] == "all")
								{
									foreach (var item in player._Inventory)
									{
										if (!item.Equipped) continue;
										var itemNameArray = new[] { input[0], item._Name };
										isTownRoom._Vendor.RepairItem(player, itemNameArray, true);
									}
									break;
								}
								isTownRoom._Vendor.RepairItem(player, input, false);
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Repair what?");
					}
					catch (NullReferenceException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no vendor here!");
					}
					break;
				case "upgrade":
					try
					{
						if (input[1] != null)
						{
							if (isTownRoom != null)
							{
								if (player._PlayerClass == Player.PlayerClassType.Mage)
								{
									isTownRoom._Trainer.UpgradeSpell(player, ParseInput(input));
								}
								else
								{
									isTownRoom._Trainer.UpgradeAbility(player, ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Upgrade what?");
					}
					catch (NullReferenceException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "train":
					try
					{
						if (input[1] != null)
						{
							if (isTownRoom != null)
							{
								if (player._PlayerClass == Player.PlayerClassType.Mage)
								{
									isTownRoom._Trainer.TrainSpell(player, ParseInput(input));
								}
								else
								{
									isTownRoom._Trainer.TrainAbility(player, ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Train what?");
					}
					catch (NullReferenceException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "consider":
					try
					{
						if (isTownRoom?._Trainer != null)
						{
							isTownRoom._Trainer.OfferQuest(player, input);
						}
						else
						{
							isTownRoom?._Vendor.OfferQuest(player, input);
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Consider what quest?");
					}
					catch (NullReferenceException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no quest giver here!");
					}
					break;
				case "show":
					try
					{
						if (input[1].Contains("forsale"))
						{
							try
							{
								isTownRoom?._Vendor.DisplayGearForSale();
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to show inventory available for sale.");
							}
						}
						if (input[1].Contains("upgrade"))
						{
							try
							{
								isTownRoom?._Trainer.DisplayAvailableUpgrades(player);
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no trainer in the room to show available upgrades.");
							}
						}
						if (input[1].Contains("quests"))
						{
							try
							{
								if (isTownRoom?._Trainer != null)
								{
									isTownRoom._Trainer.ShowQuestList(player);
								}
								else
								{
									isTownRoom?._Vendor.ShowQuestList(player);
								}
							}
							catch (NullReferenceException)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no one in the room to show quests.");
							}
						}
					}
					catch (IndexOutOfRangeException)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Show what?");
					}
					break;
				case "n":
				case "north":
					if (playerRoom._North != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX, playerY + 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "s":
				case "south":
					if (playerRoom._South != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX, playerY - 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "e":
				case "east":
					if (playerRoom._East != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX + 1, playerY, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "w":
				case "west":
					if (playerRoom._West != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX - 1, playerY, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "ne":
				case "northeast":
					if (playerRoom._NorthEast != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX + 1, playerY + 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "nw":
				case "northwest":
					if (playerRoom._NorthWest != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX - 1, playerY + 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "se":
				case "southeast":
					if (playerRoom._SouthEast != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX + 1, playerY - 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "sw":
				case "southwest":
					if (playerRoom._SouthWest != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX - 1, playerY - 1, playerZ);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "u":
				case "up":
					if (playerRoom._Up != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX, playerY, playerZ + 1);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
						Messages.InvalidDirection();
					}
					break;
				case "d":
				case "down":
					if (playerRoom._Down != null)
					{
						try
						{
							var newCoord = new Coordinate(playerX, playerY, playerZ - 1);
							RoomHandler.ChangeRoom(player, newCoord);
						}
						catch (ArgumentOutOfRangeException)
						{
							Messages.InvalidDirection();
						}
					}
					else
					{
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