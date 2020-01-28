using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DungeonGame {
	public static class InputProcessor {
		public static void ProcessUserInput(List<IRoom> spawnedRooms, Player player, string[] input, Timer globalTimer) {
			var isTownRoom = spawnedRooms[Helper.RoomIndex] as TownRoom;
			switch (input[0]) {
				case "a":
				case "attack":
				case "kill":
					try {
						if (input[1] != null) {
							try {
								globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
								var outcome = spawnedRooms[Helper.RoomIndex].AttackOpponent(player, input, spawnedRooms);
								if (!outcome && player.HitPoints <= 0) {
									Helper.IsGameOver = true;
								}
								globalTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"An error has occurred while attacking.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"There is no vendor in the room to buy an item from.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Buy what?");
					}

					break;
				case "cast":
					try {
						if (input[1] != null && input[1].Contains("town")) {
							player.CastSpell(spawnedRooms, Helper.ParseInput(input));
						}
						else if (input[1] != null) {
							player.CastSpell(Helper.ParseInput(input));
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You don't have that spell.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass != Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You can't cast spells. You're not a mage!");
						}

						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You do not have enough mana to cast that spell!");
					}

					break;
				case "drop":
					GearHelper.DropItem(player, input, spawnedRooms);
					break;
				case "pickup":
					GearHelper.PickupItem(player, input, spawnedRooms);
					break;
				case "use":
					try {
						if (input.Contains("distance")) {
							player.UseAbility(spawnedRooms, input);
						}

						if (input[1] != null) {
							player.UseAbility(Helper.ParseInput(input));
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You don't have that ability.");
						Console.WriteLine();
					}
					catch (ArgumentOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You don't have that ability.");
					}
					catch (NullReferenceException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}
					}
					catch (InvalidOperationException) {
						if (player.PlayerClass == Player.PlayerClassType.Mage) {
							Helper.Display.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You can't use abilities. You're not a warrior or archer!");
						}

						switch (player.PlayerClass) {
							case Player.PlayerClassType.Warrior:
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You do not have enough rage to use that ability!");
								break;
							case Player.PlayerClassType.Archer:
								if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
									Helper.Display.StoreUserOutput(
										Helper.FormatFailureOutputText(),
										Helper.FormatDefaultBackground(),
										"You do not have a bow equipped!");
								}

								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
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
					var quitConfirm = Helper.QuitGame(player, spawnedRooms);
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
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"List what?");
							}

							break;
						case "spells":
							try {
								PlayerHelper.ListSpells(player);
							}
							catch (IndexOutOfRangeException) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
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
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"What ability did you want to know about?");
					}

					break;
				case "spell":
					try {
						PlayerHelper.SpellInfo(player, input[1]);
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"What spell did you want to know about?");
					}

					break;
				case "l":
				case "look":
					try {
						if (input[1] != null) {
							try {
								spawnedRooms[Helper.RoomIndex].LookNpc(input);
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"An error has occurred while looking.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						spawnedRooms[Helper.RoomIndex].LookRoom();
					}

					break;
				case "loot":
					try {
						if (input[1] != null) {
							try {
								spawnedRooms[Helper.RoomIndex].LootCorpse(player, input);
							}
							catch (Exception) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"An error has occurred while looting.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Loot what?");
					}

					break;
				case "drink":
					if (input.Last() == "potion") {
						player.DrinkPotion(input);
					}
					else {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You can't drink that!");
					}

					break;
				case "save":
					Helper.SaveGame(player, spawnedRooms);
					break;
				case "restore":
					isTownRoom?.Vendor.RestorePlayer(player);
					break;
				case "help":
					Helper.ShowCommandHelp();
					break;
				case "sell":
					try {
						if (input[1] != null) {
							try {
								isTownRoom?.Vendor.SellItemCheck(player, input);
							}
							catch (NullReferenceException) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"The vendor doesn't want that.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Repair what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Upgrade what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Train what?");
					}
					catch (NullReferenceException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"There is no vendor in the room to show inventory available for sale.");
							}
						}

						if (input[1] == "upgrades") {
							try {
								isTownRoom?.Trainer.DisplayAvailableUpgrades(player);
							}
							catch (NullReferenceException) {
								Helper.Display.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"There is no trainer in the room to show available upgrades.");
							}
						}
					}
					catch (IndexOutOfRangeException) {
						Helper.Display.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"Show what?");
					}

					break;
				case "n":
				case "north":
					if (spawnedRooms[Helper.RoomIndex].GoNorth) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 0, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "s":
				case "south":
					if (spawnedRooms[Helper.RoomIndex].GoSouth) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 0, -1, 0);

						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "e":
				case "east":
					if (spawnedRooms[Helper.RoomIndex].GoEast) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 1, 0, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "w":
				case "west":
					if (spawnedRooms[Helper.RoomIndex].GoWest) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, -1, 0, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "ne":
				case "northeast":
					if (spawnedRooms[Helper.RoomIndex].GoNorthEast) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 1, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "nw":
				case "northwest":
					if (spawnedRooms[Helper.RoomIndex].GoNorthWest) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, -1, 1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "se":
				case "southeast":
					if (spawnedRooms[Helper.RoomIndex].GoSouthEast) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 1, -1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "sw":
				case "southwest":
					if (spawnedRooms[Helper.RoomIndex].GoSouthWest) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, -1, -1, 0);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "u":
				case "up":
					if (spawnedRooms[Helper.RoomIndex].GoUp) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 0, 0, 1);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				case "d":
				case "down":
					if (spawnedRooms[Helper.RoomIndex].GoDown) {
						try {
							Helper.ChangeRoom(spawnedRooms, player, 0, 0, -1);
						}
						catch (ArgumentOutOfRangeException) {
							Helper.InvalidDirection();
						}
					}
					else {
						Helper.InvalidDirection();
					}

					break;
				default:
					Helper.InvalidCommand();
					break;
			}
		}
	}
}