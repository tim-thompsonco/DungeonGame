using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame {
	public static class InputHandler {
		public static string[] GetFormattedInput(string userInput) {
			var inputFormatted = userInput.ToLower().Trim();
			var inputParse = inputFormatted.Split(' ');
			return inputParse;
		}
		public static string ParseInput(string[] userInput) {
			var inputString = new StringBuilder();
			for (var i = 1; i < userInput.Length; i++) {
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var parsedInput = inputString.ToString().Trim();
			return parsedInput;
		}
		public static void ProcessUserInput(Player player, string[] input, Timer globalTimer) {
			var isTownRoom = RoomHandler.Rooms[RoomHandler.RoomIndex] as TownRoom;
			switch (input[0]) {
				case "a":
				case "attack":
				case "kill":
					RoomHandler.Rooms[RoomHandler.RoomIndex].AttackOpponent(player, input, globalTimer);
					break;
				case "buy":
					try {
						if (input[1] != null) {
							try {
								isTownRoom?.Vendor.BuyItemCheck(player, input);
							}
							catch (NullReferenceException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to buy an item from.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Buy what?");
					}
					break;
				case "cast":
					try {
						if (input[1] != null && input[1].Contains("town")) {
							player.CastSpell(ParseInput(input));
						}
						else if (input[1] != null) {
							player.CastSpell(ParseInput(input));
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
				case "drop":
					GearHandler.DropItem(player, input);
					break;
				case "pickup":
					GearHandler.PickupItem(player, input);
					break;
				case "use":
					try {
						if (input.Contains("distance")) {
							player.UseAbility(input);
						}
						else if (input.Contains("ambush")) {
							player.UseAbility(RoomHandler.Rooms[RoomHandler.RoomIndex].Monster, input);
							RoomHandler.Rooms[RoomHandler.RoomIndex].AttackOpponent(player, input, globalTimer);
						}
						else if (input[1] != null) {
							player.UseAbility(input);
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						Console.WriteLine();
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
					GearHandler.EquipItem(player, input);
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
					if (quitConfirm) {
						globalTimer.Dispose();
					}
					break;
				case "list":
					switch (input[1]) {
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
						PlayerHandler.AbilityInfo(player, input);
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
						PlayerHandler.SpellInfo(player, input[1]);
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}
					break;
				case "l":
				case "look":
					try {
						if (input[1] != null) {
							try {
								RoomHandler.Rooms[RoomHandler.RoomIndex].LookNpc(input, player);
							}
							catch (Exception) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looking.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						RoomHandler.Rooms[RoomHandler.RoomIndex].LookRoom();
					}
					break;
				case "loot":
					try {
						if (input[1] != null) {
							try {
								RoomHandler.Rooms[RoomHandler.RoomIndex].LootCorpse(player, input);
							}
							catch (Exception) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looting.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Loot what?");
					}
					break;
				case "drink":
					if (input.Last() == "potion") {
						player.DrinkPotion(input);
					}
					else {
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
					isTownRoom?.Vendor.RestorePlayer(player);
					break;
				case "help":
					Messages.ShowCommandHelp();
					break;
				case "sell":
					try {
						if (input[1] != null) {
							try {
								isTownRoom?.Vendor.SellItemCheck(player, input);
							}
							catch (NullReferenceException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"The vendor doesn't want that.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
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
									foreach (var item in player.Inventory) {
										if (!item.Equipped) continue;
										var itemNameArray = new [] {input[0], item.Name};
										isTownRoom.Vendor.RepairItem(player, itemNameArray);
									}

									break;
								}

								isTownRoom.Vendor.RepairItem(player, input);
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Repair what?");
					}
					catch (NullReferenceException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no vendor here!");
					}
					break;
				case "upgrade":
					try {
						if (input[1] != null) {
							if (isTownRoom != null) {
								if (player.PlayerClass == Player.PlayerClassType.Mage) {
									isTownRoom.Trainer.UpgradeSpell(player, ParseInput(input));
								}
								else {
									isTownRoom.Trainer.UpgradeAbility(player, ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Upgrade what?");
					}
					catch (NullReferenceException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "train":
					try {
						if (input[1] != null) {
							if (isTownRoom != null) {
								if (player.PlayerClass == Player.PlayerClassType.Mage) {
									isTownRoom.Trainer.TrainSpell(player, ParseInput(input));
								}
								else {
									isTownRoom.Trainer.TrainAbility(player, ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Train what?");
					}
					catch (NullReferenceException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}
					break;
				case "show":
					try {
						if (input[1] == "forsale") {
							try {
								isTownRoom?.Vendor.DisplayGearForSale();
							}
							catch (NullReferenceException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to show inventory available for sale.");
							}
						}
						if (input[1] == "upgrades") {
							try {
								isTownRoom?.Trainer.DisplayAvailableUpgrades(player);
							}
							catch (NullReferenceException) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no trainer in the room to show available upgrades.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Show what?");
					}
					break;
				case "n":
				case "north":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorth) {
						try {
							RoomHandler.ChangeRoom(player, 0, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "s":
				case "south":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouth) {
						try {
							RoomHandler.ChangeRoom(player, 0, -1, 0);

						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "e":
				case "east":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoEast) {
						try {
							RoomHandler.ChangeRoom(player, 1, 0, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "w":
				case "west":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoWest) {
						try {
							RoomHandler.ChangeRoom(player, -1, 0, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "ne":
				case "northeast":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorthEast) {
						try {
							RoomHandler.ChangeRoom(player, 1, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "nw":
				case "northwest":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoNorthWest) {
						try {
							RoomHandler.ChangeRoom(player, -1, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "se":
				case "southeast":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouthEast) {
						try {
							RoomHandler.ChangeRoom(player, 1, -1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "sw":
				case "southwest":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoSouthWest) {
						try {
							RoomHandler.ChangeRoom(player, -1, -1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "u":
				case "up":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoUp) {
						try {
							RoomHandler.ChangeRoom(player, 0, 0, 1);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
						Messages.InvalidDirection();
					}
					break;
				case "d":
				case "down":
					if (RoomHandler.Rooms[RoomHandler.RoomIndex].GoDown) {
						try {
							RoomHandler.ChangeRoom(player, 0, 0, -1);
						}
						catch (ArgumentOutOfRangeException) {
							Messages.InvalidDirection();
						}
					}
					else {
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