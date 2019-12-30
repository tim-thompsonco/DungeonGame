using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DungeonGame {
	class MainClass {
		public static void Main(string[] args) {
			while (true) {
				// Game loading commands
				Console.BufferHeight = 50;
				Console.BufferWidth = 150;
				Console.WindowHeight = 50;
				Console.WindowWidth = 150;
				Helper.GameIntro();
				Player player;
				try {
					player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(File.ReadAllText(
						"savegame.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					Helper.FormatGeneralInfoText();
					Console.WriteLine("Reloading your saved game.");
				}
				catch (FileNotFoundException) {
					player = Helper.BuildNewPlayer();
					GearHelper.EquipInitialGear(player);
				}
				var spawnedRooms = new SpawnRooms().RetrieveSpawnRooms();
				// Set initial room condition
				// On loading game, display room that player starts in
				// Begin game by putting player in room 100
				var roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 0);
				// While loop to continue obtaining input from player
				var isGameOver = false;
				// Player stats will replenish every 3 seconds
				var timer = new Timer(
					e => player.ReplenishStatsOverTime(),
					null,
					TimeSpan.Zero,
					TimeSpan.FromSeconds(3));
				while (!isGameOver) {
					PlayerHelper.DisplayPlayerStats(player);
					spawnedRooms[roomIndex].ShowCommands();
					var input = Helper.GetFormattedInput();
					var isTownRoom = spawnedRooms[roomIndex] as TownRoom;
					// Obtain player command and process command
					switch (input[0]) {
						case "a":
						case "attack":
						case "kill":
							try {
								if (input[1] != null) {
									try {
										timer.Dispose();
										var outcome = spawnedRooms[roomIndex].AttackOpponent(player, input);
										if (!outcome && player.HitPoints <= 0) {
											isGameOver = true;
										}
										else if (!outcome) {
											roomIndex = Helper.FleeRoom(spawnedRooms, player);
										}
										timer = new Timer(
											e => player.ReplenishStatsOverTime(),
											null,
											TimeSpan.Zero,
											TimeSpan.FromSeconds(3));
									}
									catch (Exception) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("An error has occurred while attacking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You can't attack that.");
							}
							break;
						case "buy":
							try {
								if (input[1] != null) {
									try {
										isTownRoom?.Vendor.BuyItemCheck(player, input);
									}
									catch (NullReferenceException) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("There is no vendor in the room to buy an item from.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("Buy what?");
							}
							break;
						case "cast":
							try {
								if (input[1] != null) {
									var spellName = Helper.ParseInput(input);
									player.CastSpell(spellName);
								}
								break;
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You don't have that spell.");
								continue;
							}
							catch (InvalidOperationException) {
								if (player.PlayerClass != Player.PlayerClassType.Mage) {
									Helper.FormatFailureOutputText();
									Console.WriteLine("You can't cast spells. You're not a mage!");
									continue;
								}
								Helper.FormatFailureOutputText();
								Console.WriteLine("You do not have enough mana to cast that spell!");
								continue;
							}
						case "map":
							Helper.ShowMap(spawnedRooms, player, 10, 20);
							break;
						case "use":
							try {
								if (input.Contains("distance")) {
									player.UseAbility(spawnedRooms, input);
								}
								break;
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You don't have that ability.");
								continue;
							}
							catch (ArgumentOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You don't have that ability.");
								continue;
							}
							catch (InvalidOperationException) {
								Helper.FormatFailureOutputText();
								if (player.PlayerClass == Player.PlayerClassType.Mage) {
									Console.WriteLine("You can't use abilities. You're not a warrior or archer!");
									continue;
								}
								switch (player.PlayerClass) {
									case Player.PlayerClassType.Mage:
										continue;
									case Player.PlayerClassType.Warrior:
										Console.WriteLine("You do not have enough rage to use that ability!");
										continue;
									case Player.PlayerClassType.Archer:
										if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
											Console.WriteLine("You do not have a bow equipped!");
											continue;
										}
										Console.WriteLine("You do not have enough combo points to use that ability!");
										continue;
									default:
										throw new ArgumentOutOfRangeException();
								}
							}
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
										Helper.FormatFailureOutputText();
										Console.WriteLine("List what?");
									}
									break;
								case "spells":
									try {
										PlayerHelper.ListSpells(player);
									}
									catch (IndexOutOfRangeException) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("List what?");
									}
									break;
							}
							break;
						case "ability":
							try {
								PlayerHelper.AbilityInfo(player, input);
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("What ability did you want to know about?");
							}
							break;
						case "spell":
							try {
								PlayerHelper.SpellInfo(player, input[1]);
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("What spell did you want to know about?");
							}
							break;
						case "l":
						case "look":
							try {
								if (input[1] != null) {
									try {
										spawnedRooms[roomIndex].LookNpc(input);
									}
									catch (Exception) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("An error has occurred while looking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								spawnedRooms[roomIndex].LookRoom();
							}
							break;
						case "loot":
							try {
								if (input[1] != null) {
									try {
										spawnedRooms[roomIndex].LootCorpse(player, input);
									}
									catch (Exception) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("An error has occurred while looting.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("Loot what?");
							}
							break;
						case "drink":
							if (input.Last() == "potion") {
								player.DrinkPotion(input);
							}
							else {
								Helper.FormatFailureOutputText();
								Console.WriteLine("You can't drink that!");
							}
							break;
						case "save":
							Helper.SaveGame(player);
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
										Helper.FormatFailureOutputText();
										Console.WriteLine("The vendor doesn't want that.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("Sell what?");
							}
							break;
						case "repair":
							try {
								if (input[1] != null) {
									if (isTownRoom != null) {
										if (input[1] == "all") {
											foreach (var item in player.Inventory) {
												if (!item.IsEquipped()) continue;
												var itemNameArray = new string[2] { input[0], item.Name };
												isTownRoom.Vendor.RepairItem(player, itemNameArray);
											}
											break;
										}
										isTownRoom.Vendor.RepairItem(player, input);
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("Repair what?");
							}
							catch (NullReferenceException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("There is no vendor here!");
							}
							break;
						case "show":
							try {
								if (input[1] == "forsale") {
									try {
										isTownRoom?.Vendor.DisplayGearForSale(player);
									}
									catch (NullReferenceException) {
										Helper.FormatFailureOutputText();
										Console.WriteLine("There is no vendor in the room to show inventory available for sale.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								Helper.FormatFailureOutputText();
								Console.WriteLine("Show what?");
							}
							break;
						case "n":
						case "north":
							if (spawnedRooms[roomIndex].GoNorth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 1, 0);
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
							if (spawnedRooms[roomIndex].GoSouth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, -1, 0);
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
							if (spawnedRooms[roomIndex].GoEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 0, 0);
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
							if (spawnedRooms[roomIndex].GoWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 0, 0);
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
							if (spawnedRooms[roomIndex].GoNorthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 1, 0);
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
							if (spawnedRooms[roomIndex].GoNorthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 1, 0);
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
							if (spawnedRooms[roomIndex].GoSouthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, -1, 0);
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
							if (spawnedRooms[roomIndex].GoSouthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, -1, 0);
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
							if (spawnedRooms[roomIndex].GoUp) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 1);
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
							if (spawnedRooms[roomIndex].GoDown) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, -1);
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
	}
}