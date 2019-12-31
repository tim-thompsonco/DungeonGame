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
				var initialOutput = new UserOutput();
				Helper.GameIntro(initialOutput);
				Player player;
				try {
					player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(File.ReadAllText(
						"savegame.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					initialOutput.StoreUserOutput(
						Helper.FormatGeneralInfoText(), 
						Helper.FormatDefaultBackground(), 
						"Reloading your saved game.");
				}
				catch (FileNotFoundException) {
					player = Helper.BuildNewPlayer(initialOutput);
					GearHelper.EquipInitialGear(player);
				}
				var spawnedRooms = new SpawnRooms().RetrieveSpawnRooms();
				// Set initial room condition
				// On loading game, display room that player starts in
				// Begin game by putting player in room 100
				var roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 0, initialOutput);
				// While loop to continue obtaining input from player
				var isGameOver = false;
				// Player stats will replenish every 3 seconds
				var timer = new Timer(
					e => player.ReplenishStatsOverTime(),
					null,
					TimeSpan.Zero,
					TimeSpan.FromSeconds(3));
				initialOutput.RetrieveUserOutput();
				while (!isGameOver) {
					var userOutput = new UserOutput();
					var input = Helper.GetFormattedInput();
					var isTownRoom = spawnedRooms[roomIndex] as TownRoom;
					Console.Clear();
					// Obtain player command and process command
					switch (input[0]) {
						case "a":
						case "attack":
						case "kill":
							try {
								if (input[1] != null) {
									try {
										timer.Dispose();
										var outcome = spawnedRooms[roomIndex].AttackOpponent(player, input, userOutput);
										if (!outcome && player.HitPoints <= 0) {
											isGameOver = true;
										}
										else if (!outcome) {
											roomIndex = Helper.FleeRoom(spawnedRooms, player, userOutput);
										}
										timer = new Timer(
											e => player.ReplenishStatsOverTime(),
											null,
											TimeSpan.Zero,
											TimeSpan.FromSeconds(3));
									}
									catch (Exception) {
										userOutput.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"An error has occurred while attacking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
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
										userOutput.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"There is no vendor in the room to buy an item from.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
									Helper.FormatFailureOutputText(), 
									Helper.FormatDefaultBackground(), 
									"Buy what?");
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
								userOutput.StoreUserOutput(
									Helper.FormatFailureOutputText(), 
									Helper.FormatDefaultBackground(), 
									"You don't have that spell.");
								continue;
							}
							catch (InvalidOperationException) {
								if (player.PlayerClass != Player.PlayerClassType.Mage) {
									userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't cast spells. You're not a mage!");
									continue;
								}
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You do not have enough mana to cast that spell!");
								continue;
							}
						case "map":
							Helper.ShowMap(spawnedRooms, player, 10, 20);
							break;
						case "use":
							try {
								if (input.Contains("distance")) {
									player.UseAbility(spawnedRooms, input, userOutput);
								}
								break;
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You don't have that ability.");
								Console.WriteLine();
								continue;
							}
							catch (ArgumentOutOfRangeException) {
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You don't have that ability.");
								continue;
							}
							catch (InvalidOperationException) {
								if (player.PlayerClass == Player.PlayerClassType.Mage) {
									userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't use abilities. You're not a warrior or archer!");
									continue;
								}
								switch (player.PlayerClass) {
									case Player.PlayerClassType.Mage:
										continue;
									case Player.PlayerClassType.Warrior:
										userOutput.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"You do not have enough rage to use that ability!");
										continue;
									case Player.PlayerClassType.Archer:
										if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
											userOutput.StoreUserOutput(
												Helper.FormatFailureOutputText(), 
												Helper.FormatDefaultBackground(), 
												"You do not have a bow equipped!");
											continue;
										}
										userOutput.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"You do not have enough combo points to use that ability!");
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
							PlayerHelper.ShowInventory(player, userOutput);
							break;
						case "q":
						case "quit":
							var quitConfirm = Helper.QuitGame(player, userOutput);
							if (quitConfirm) {
								return;
							}
							break;
						case "list":
							switch (input[1]) {
								case "abilities":
									try {
										PlayerHelper.ListAbilities(player, userOutput);
									}
									catch (IndexOutOfRangeException) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"List what?");
									}
									break;
								case "spells":
									try {
										PlayerHelper.ListSpells(player, userOutput);
									}
									catch (IndexOutOfRangeException) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"List what?");
									}
									break;
							}
							break;
						case "ability":
							try {
								PlayerHelper.AbilityInfo(player, input, userOutput);
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"What ability did you want to know about?");
							}
							break;
						case "spell":
							try {
								PlayerHelper.SpellInfo(player, input[1], userOutput);
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
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
										spawnedRooms[roomIndex].LookNpc(input);
									}
									catch (Exception) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"An error has occurred while looking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								spawnedRooms[roomIndex].LookRoom(userOutput);
							}
							break;
						case "loot":
							try {
								if (input[1] != null) {
									try {
										spawnedRooms[roomIndex].LootCorpse(player, input);
									}
									catch (Exception) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"An error has occurred while looting.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
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
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't drink that!");
							}
							break;
						case "save":
							Helper.SaveGame(player, userOutput);
							break;
						case "restore":
							isTownRoom?.Vendor.RestorePlayer(player);
							break;
						case "help":
							Helper.ShowCommandHelp(userOutput);
							break;
						case "sell":
							try {
								if (input[1] != null) {
									try {
										isTownRoom?.Vendor.SellItemCheck(player, input, userOutput);
									}
									catch (NullReferenceException) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"The vendor doesn't want that.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
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
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"Repair what?");
							}
							catch (NullReferenceException) {
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"There is no vendor here!");
							}
							break;
						case "show":
							try {
								if (input[1] == "forsale") {
									try {
										isTownRoom?.Vendor.DisplayGearForSale(player);
									}
									catch (NullReferenceException) {
										userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"There is no vendor in the room to show inventory available for sale.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								userOutput.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"Show what?");
							}
							break;
						case "n":
						case "north":
							if (spawnedRooms[roomIndex].GoNorth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "s":
						case "south":
							if (spawnedRooms[roomIndex].GoSouth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, -1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "e":
						case "east":
							if (spawnedRooms[roomIndex].GoEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 0, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "w":
						case "west":
							if (spawnedRooms[roomIndex].GoWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 0, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "ne":
						case "northeast":
							if (spawnedRooms[roomIndex].GoNorthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "nw":
						case "northwest":
							if (spawnedRooms[roomIndex].GoNorthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "se":
						case "southeast":
							if (spawnedRooms[roomIndex].GoSouthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, -1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "sw":
						case "southwest":
							if (spawnedRooms[roomIndex].GoSouthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, -1, 0, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "u":
						case "up":
							if (spawnedRooms[roomIndex].GoUp) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 1, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						case "d":
						case "down":
							if (spawnedRooms[roomIndex].GoDown) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, -1, userOutput);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(userOutput);
								}
							}
							else {
								Helper.InvalidDirection(userOutput);
							}
							break;
						default:
							Helper.InvalidCommand();
							break;
					}
				PlayerHelper.DisplayPlayerStats(player, userOutput);
				spawnedRooms[roomIndex].ShowCommands(userOutput);
				userOutput.RetrieveUserOutput();
				}
			}
		}
	}
}