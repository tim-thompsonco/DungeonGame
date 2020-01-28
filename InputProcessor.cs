using System;
using System.Linq;
using System.Threading;

namespace DungeonGame {
	public static class InputProcessor {
		public static void ProcessUserInput(Player player, string[] input, Timer globalTimer) {
			var isTownRoom = Helper.Rooms[Helper.RoomIndex] as TownRoom;
			switch (input[0]) {
				case "a":
				case "attack":
				case "kill":
					try {
						if (input[1] != null) {
							try {
								globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
								var outcome = Helper.Rooms[Helper.RoomIndex].AttackOpponent(player, input);
								if (!outcome && player.HitPoints <= 0) {
									Helper.IsGameOver = true;
								}
								globalTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while attacking.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't attack that.");
					}

					break;
				case "buy":
					try {
						if (input[1] != null) {
							try {
								isTownRoom?.Vendor.BuyItemCheck(player, input);
							}
							catch (NullReferenceException) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no vendor in the room to buy an item from.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Buy what?");
					}

					break;
				case "cast":
					try {
						if (input[1] != null && input[1].Contains("town")) {
							player.CastSpell(Helper.ParseInput(input));
						}
						else if (input[1] != null) {
							player.CastSpell(Helper.ParseInput(input));
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that spell.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}

						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You do not have enough mana to cast that spell!");
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
						}

						if (input[1] != null) {
							player.UseAbility(Helper.ParseInput(input));
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
						Console.WriteLine();
					}
					catch (ArgumentOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have that ability.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}

						switch (player.PlayerClass) {
							case Player.PlayerClassType.Warrior:
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
									Helper.Display.StoreUserOutput(
										Settings.FormatFailureOutputText(),
										Settings.FormatDefaultBackground(),
										"You do not have a bow equipped!");
								}

								Helper.Display.StoreUserOutput(
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
					GearHelper.EquipItem(player, input);
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
					var quitConfirm = Helper.QuitGame(player);
					if (quitConfirm) {
						globalTimer.Dispose();
						return;
					}

					break;
				case "list":
					switch (input[1]) {
						case "abilities":
							try {
								PlayerHelper.ListAbilities(player);
							}
							catch (IndexOutOfRangeException) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"List what?");
							}

							break;
						case "spells":
							try {
								PlayerHelper.ListSpells(player);
							}
							catch (IndexOutOfRangeException) {
								Helper.Display.StoreUserOutput(
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
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}

					break;
				case "spell":
					try {
						PlayerHelper.SpellInfo(player, input[1]);
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
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
								Helper.Rooms[Helper.RoomIndex].LookNpc(input);
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looking.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Rooms[Helper.RoomIndex].LookRoom();
					}

					break;
				case "loot":
					try {
						if (input[1] != null) {
							try {
								Helper.Rooms[Helper.RoomIndex].LootCorpse(player, input);
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"An error has occurred while looting.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
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
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You can't drink that!");
					}

					break;
				case "save":
					Helper.SaveGame(player);
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
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"The vendor doesn't want that.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
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
										if (!item.IsEquipped()) continue;
										var itemNameArray = new string[2] {input[0], item.Name};
										isTownRoom.Vendor.RepairItem(player, itemNameArray);
									}

									break;
								}

								isTownRoom.Vendor.RepairItem(player, input);
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Repair what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
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
									isTownRoom.Trainer.UpgradeSpell(player, Helper.ParseInput(input));
								}
								else {
									isTownRoom.Trainer.UpgradeAbility(player, Helper.ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Upgrade what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
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
									isTownRoom.Trainer.TrainSpell(player, Helper.ParseInput(input));
								}
								else {
									isTownRoom.Trainer.TrainAbility(player, Helper.ParseInput(input));
								}
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Train what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"There is no trainer here!");
					}

					break;
				case "show":
					try {
						if (input[1] == "forsale") {
							try {
								isTownRoom?.Vendor.DisplayGearForSale(player);
							}
							catch (NullReferenceException) {
								Helper.Display.StoreUserOutput(
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
								Helper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"There is no trainer in the room to show available upgrades.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"Show what?");
					}

					break;
				case "n":
				case "north":
					if (Helper.Rooms[Helper.RoomIndex].GoNorth) {
						try {
							Helper.ChangeRoom(player, 0, 1, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoSouth) {
						try {
							Helper.ChangeRoom(player, 0, -1, 0);

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
					if (Helper.Rooms[Helper.RoomIndex].GoEast) {
						try {
							Helper.ChangeRoom(player, 1, 0, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoWest) {
						try {
							Helper.ChangeRoom(player, -1, 0, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoNorthEast) {
						try {
							Helper.ChangeRoom(player, 1, 1, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoNorthWest) {
						try {
							Helper.ChangeRoom(player, -1, 1, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoSouthEast) {
						try {
							Helper.ChangeRoom(player, 1, -1, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoSouthWest) {
						try {
							Helper.ChangeRoom(player, -1, -1, 0);
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
					if (Helper.Rooms[Helper.RoomIndex].GoUp) {
						try {
							Helper.ChangeRoom(player, 0, 0, 1);
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
					if (Helper.Rooms[Helper.RoomIndex].GoDown) {
						try {
							Helper.ChangeRoom(player, 0, 0, -1);
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