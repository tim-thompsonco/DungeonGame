using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DungeonGame {
	class MainClass {
		public static void Main(string[] args) {
			try {
				Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			}
			catch (Exception) {}
			while (true) {
				// Game loading commands
				var output = new UserOutput();
				var mapOutput = new UserOutput();
				Helper.GameIntro(output);
				Player player;
				List<IRoom> spawnedRooms;
				try {
					player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(File.ReadAllText(
						"playersave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					spawnedRooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRoom>>(File.ReadAllText(
						"gamesave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					// Insert blank space before game reload info for formatting
					output.StoreUserOutput(
						Helper.FormatGeneralInfoText(),
						Helper.FormatDefaultBackground(),
						"");
					output.StoreUserOutput(
						Helper.FormatGeneralInfoText(), 
						Helper.FormatDefaultBackground(), 
						"Reloading your saved game.");
					// Insert blank space after game reload info for formatting
					output.StoreUserOutput(
						Helper.FormatGeneralInfoText(),
						Helper.FormatDefaultBackground(),
						"");
				}
				catch (FileNotFoundException) {
					// Create dungeon
					spawnedRooms = new RoomBuilder(
						100, 5, 1, 3, 
						0, 4, 0, RoomBuilder.StartDirection.Down).RetrieveSpawnRooms();
					player = Helper.BuildNewPlayer(output);
					GearHelper.EquipInitialGear(player, output);
					// Begin game by putting player at coords 0, 7, 0, town entrance
					var checkRoomIndex = Helper.SetPlayerLocation(spawnedRooms, player, 0, 7, 0);
					if (checkRoomIndex == -1) throw new InvalidOperationException();
				}
				/* Set initial room condition for player
					On loading game, display room that player starts in */
				var roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 0, output);
				// While loop to continue obtaining input from player
				var isGameOver = false;
				mapOutput = Helper.ShowMap(spawnedRooms, player, Helper.GetMiniMapHeight(), Helper.GetMiniMapWidth());
				PlayerHelper.DisplayPlayerStats(player, output);
				spawnedRooms[roomIndex].ShowCommands(output);
				output.RetrieveUserOutput(mapOutput);
				while (!isGameOver) {
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
										foreach (var effect in player.Effects) {
											effect.EnterCombat(player, output);
										}
										player.InCombat = true;
										var outcome = spawnedRooms[roomIndex].AttackOpponent(
											player, input, output, mapOutput, spawnedRooms);
										if (!outcome && player.HitPoints <= 0) {
											isGameOver = true;
										}
										else if (!outcome) {
											roomIndex = Helper.FleeRoom(spawnedRooms, player, output);
										}
										player.InCombat = false;
										foreach (var effect in player.Effects) {
											effect.ExitCombat(player, output);
										}
									}
									catch (Exception) {
										output.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"An error has occurred while attacking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(), 
									Helper.FormatDefaultBackground(), 
									"You can't attack that.");
							}
							break;
						case "buy":
							try {
								if (input[1] != null) {
									try {
										isTownRoom?.Vendor.BuyItemCheck(player, input, output);
									}
									catch (NullReferenceException) {
										output.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"There is no vendor in the room to buy an item from.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(), 
									Helper.FormatDefaultBackground(), 
									"Buy what?");
							}
							break;
						case "cast":
							try {
								if (input[1] != null) {
									var spellName = Helper.ParseInput(input);
									player.CastSpell(spellName, output);
								}
								break;
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(), 
									Helper.FormatDefaultBackground(), 
									"You don't have that spell.");
								continue;
							}
							catch (InvalidOperationException) {
								if (player.PlayerClass != Player.PlayerClassType.Mage) {
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't cast spells. You're not a mage!");
									continue;
								}
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You do not have enough mana to cast that spell!");
								continue;
							}
						case "drop":
							GearHelper.DropItem(player, input, output, spawnedRooms);
							break;		
						case "pickup":
							GearHelper.PickupItem(player, input, output, spawnedRooms);
							break;	
						case "use":
							try {
								if (input.Contains("distance")) {
									player.UseAbility(spawnedRooms, input, output);
								}
								if (input[1] != null) {
									var abilityName = Helper.ParseInput(input);
									player.UseAbility(abilityName, output);
								}
								break;
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You don't have that ability.");
								Console.WriteLine();
								continue;
							}
							catch (ArgumentOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You don't have that ability.");
								continue;
							}
							catch (InvalidOperationException) {
								if (player.PlayerClass == Player.PlayerClassType.Mage) {
									output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't use abilities. You're not a warrior or archer!");
									continue;
								}
								switch (player.PlayerClass) {
									case Player.PlayerClassType.Mage:
										continue;
									case Player.PlayerClassType.Warrior:
										output.StoreUserOutput(
											Helper.FormatFailureOutputText(), 
											Helper.FormatDefaultBackground(), 
											"You do not have enough rage to use that ability!");
										continue;
									case Player.PlayerClassType.Archer:
										if (player.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
											output.StoreUserOutput(
												Helper.FormatFailureOutputText(), 
												Helper.FormatDefaultBackground(), 
												"You do not have a bow equipped!");
											continue;
										}
										output.StoreUserOutput(
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
							GearHelper.EquipItem(player, input, output);
							break;
						case "reload":
							player.ReloadQuiver(output);
							break;
						case "i":
						case "inventory":
							PlayerHelper.ShowInventory(player, output);
							break;
						case "q":
						case "quit":
							var quitConfirm = Helper.QuitGame(player, output, spawnedRooms);
							if (quitConfirm) {
								return;
							}
							break;
						case "list":
							switch (input[1]) {
								case "abilities":
									try {
										PlayerHelper.ListAbilities(player, output);
									}
									catch (IndexOutOfRangeException) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"List what?");
									}
									break;
								case "spells":
									try {
										PlayerHelper.ListSpells(player, output);
									}
									catch (IndexOutOfRangeException) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"List what?");
									}
									break;
							}
							break;
						case "ability":
							try {
								PlayerHelper.AbilityInfo(player, input, output);
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"What ability did you want to know about?");
							}
							break;
						case "spell":
							try {
								PlayerHelper.SpellInfo(player, input[1], output);
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
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
										spawnedRooms[roomIndex].LookNpc(input, output);
									}
									catch (Exception) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"An error has occurred while looking.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								spawnedRooms[roomIndex].LookRoom(output);
							}
							break;
						case "loot":
							try {
								if (input[1] != null) {
									try {
										spawnedRooms[roomIndex].LootCorpse(player, input, output);
									}
									catch (Exception) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"An error has occurred while looting.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"Loot what?");
							}
							break;
						case "drink":
							if (input.Last() == "potion") {
								player.DrinkPotion(input, output);
							}
							else {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"You can't drink that!");
							}
							break;
						case "save":
							Helper.SaveGame(player, output, spawnedRooms);
							break;
						case "restore":
							isTownRoom?.Vendor.RestorePlayer(player, output);
							break;
						case "help":
							Helper.ShowCommandHelp(output);
							break;
						case "sell":
							try {
								if (input[1] != null) {
									try {
										isTownRoom?.Vendor.SellItemCheck(player, input, output);
									}
									catch (NullReferenceException) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"The vendor doesn't want that.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
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
												isTownRoom.Vendor.RepairItem(player, itemNameArray, output);
											}
											break;
										}
										isTownRoom.Vendor.RepairItem(player, input, output);
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"Repair what?");
							}
							catch (NullReferenceException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"There is no vendor here!");
							}
							break;
						case "show":
							try {
								if (input[1] == "forsale") {
									try {
										isTownRoom?.Vendor.DisplayGearForSale(player, output);
									}
									catch (NullReferenceException) {
										output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"There is no vendor in the room to show inventory available for sale.");
									}
								}
							}
							catch (IndexOutOfRangeException) {
								output.StoreUserOutput(
										Helper.FormatFailureOutputText(), 
										Helper.FormatDefaultBackground(), 
										"Show what?");
							}
							break;
						case "n":
						case "north":
							if (spawnedRooms[roomIndex].GoNorth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 1, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "s":
						case "south":
							if (spawnedRooms[roomIndex].GoSouth) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, -1, 0, output);
									
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "e":
						case "east":
							if (spawnedRooms[roomIndex].GoEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 0, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "w":
						case "west":
							if (spawnedRooms[roomIndex].GoWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 0, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "ne":
						case "northeast":
							if (spawnedRooms[roomIndex].GoNorthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 1, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "nw":
						case "northwest":
							if (spawnedRooms[roomIndex].GoNorthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 1, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "se":
						case "southeast":
							if (spawnedRooms[roomIndex].GoSouthEast) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, -1, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "sw":
						case "southwest":
							if (spawnedRooms[roomIndex].GoSouthWest) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, -1, 0, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "u":
						case "up":
							if (spawnedRooms[roomIndex].GoUp) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 1, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						case "d":
						case "down":
							if (spawnedRooms[roomIndex].GoDown) {
								try {
									roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, -1, output);
								}
								catch (ArgumentOutOfRangeException) {
									Helper.InvalidDirection(output);
								}
							}
							else {
								Helper.InvalidDirection(output);
							}
							break;
						default:
							Helper.InvalidCommand(output);
							break;
					}
				PlayerHelper.DisplayPlayerStats(player, output);
				spawnedRooms[roomIndex].ShowCommands(output);
				mapOutput = Helper.ShowMap(spawnedRooms, player, Helper.GetMiniMapHeight(), Helper.GetMiniMapWidth());
				output.RetrieveUserOutput(mapOutput);
				output.ClearUserOutput();
				}
			}
		}
	}
}