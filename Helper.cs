using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace DungeonGame {
	public static class Helper {
		public static string[] GetFormattedInput() {
			var input = Console.ReadLine();
			var inputFormatted = input.ToLower().Trim();
			var inputParse = inputFormatted.Split(' ');
			return inputParse;
		}
		public static void FormatSuccessOutputText() {
			Console.ForegroundColor = ConsoleColor.Green;
		}
		public static void FormatFailureOutputText() {
			Console.ForegroundColor = ConsoleColor.DarkCyan;
		}
		public static void FormatOnFireText() {
			Console.ForegroundColor = ConsoleColor.Yellow;
		}
		public static void FormatAttackSuccessText() {
			Console.ForegroundColor = ConsoleColor.Red;
		}
		public static void FormatAttackFailText() {
			Console.ForegroundColor = ConsoleColor.DarkRed;
		}
		public static void FormatInfoText() {
			Console.ForegroundColor = ConsoleColor.White;
		}
		public static void FormatLevelUpText() {
			Console.ForegroundColor = ConsoleColor.Cyan;
		}
		public static void FormatGeneralInfoText() {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
		}
		public static void FormatRoomInfoText() {
			Console.ForegroundColor = ConsoleColor.White;
		}
		public static void FormatAnnounceText() {
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		public static void ShowImpassibleDungeonTile() {
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("  ");
		}
		public static void ShowPlayerDungeonTile() {
			Console.BackgroundColor = ConsoleColor.Green;
			Console.Write("  ");
		}
		public static void ShowPlayerDungeonUpDownTile() {
			Console.BackgroundColor = ConsoleColor.Green;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("OO");
		}
		public static void ShowDiscoveredDungeonTile() {
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.Write("  ");
		}
		public static void ShowDiscoveredDungeonUpDownTile() {
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.Write("OO");
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
		public static void RequestCommand() {
			FormatAnnounceText();
			Console.Write("Your command: ");
		}
		public static void PlayerDeath() {
			FormatAnnounceText();
			Console.WriteLine("You have died. Game over.");
		}
		public static void GameIntro() {
			FormatAnnounceText();
			Console.WriteLine(
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot" +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.\n");
		}
		public static void ShowCommandHelp() {
			FormatAnnounceText();
			Console.WriteLine(
				"Commands: Players may move in any direction of the game using a shortkey or the full direction name. " +
				"For example, if you wish to go north, you may type either 'N' or 'North'. If a player wishes to look " +
				"at something, they can use 'l' or 'look' and then the name of what they want to look at. For example " +
				"'l zombie' or 'look zombie' would allow you to look at a zombie in the room. The same commands will  " +
				"work to loot a monster that you have killed. Look or 'L' by itself will look at the room. Other common " +
				"commands will be shown to the player. Any object that is consumable, such as a potion, can be drank " +
				"by typing 'drink' and then the name of the potion or object. To use armor or weapons, you must 'equip' " +
				"them. You can 'unequip' them as well.");
		}
		public static Player BuildNewPlayer() {
			FormatAnnounceText();
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			Console.WriteLine("Please enter a player name.\n");
			string playerName;
			while (true) {
				Console.Write("Player name: ");
				playerName = textInfo.ToTitleCase(Console.ReadLine().ToString());
				Console.WriteLine("Your player name is {0}, is that correct? [Y] or [N].", playerName);
				RequestCommand();
				var input = GetFormattedInput();
				if (input[0] == "y") {
					break;
				}
			}
			Console.WriteLine("Please enter your class. You can select Mage, Warrior, or Archer.\n");
			while (true) {
				Console.Write("Player class: ");
				var userInput = GetFormattedInput();
				var playerClassInput = textInfo.ToTitleCase(userInput[0].ToString());
				if (playerClassInput != "Mage" && playerClassInput != "Warrior" && playerClassInput != "Archer") {
					Console.WriteLine("Invalid selection. Please enter Mage, Warrior, or Archer for your class.");
					continue;
				}
				var playerClass = playerClassInput;
				Console.WriteLine("Your player class is {0}, is that correct? [Y] or [N].", playerClass);
				RequestCommand();
				var input = GetFormattedInput();
				if (input[0] == "y") {
					switch(playerClass) {
						case "Archer":
							var playerArcher = new Player(playerName, Player.PlayerClassType.Archer);
							Console.WriteLine("\n\nYou have selected Archer. You can 'use' an ability, for example " +
							                  "'use gut', if you have an ability named gut shot in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability distance'. " +
							                  "To use a bow, you must have a quiver equipped, and it must not be empty. To reload " +
							                  "your quiver, you can 'reload'.");
							return playerArcher;
						case "Mage":
							var playerMage = new Player(playerName, Player.PlayerClassType.Mage);
							Console.WriteLine("\n\nYou have selected Mage. You can 'cast' a spell, for example " +
							                  "'cast fireball', if you have a spell named fireball in your spellbook. To see " +
							                  "the list of spells in your spellbook, you can 'list spells'. To view info " +
							                  "about a spell, you can 'spell' the spell name. For example, 'spell fireball'.");
							return playerMage;
							case "Warrior":
							var playerWarrior = new Player(playerName, Player.PlayerClassType.Warrior);
							Console.WriteLine("\n\nYou have selected Warrior. You can 'use' an ability, for example " +
							                  "'use charge', if you have an ability named charge in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability charge'.");
							return playerWarrior;
					}
				}
			}
		}
		public static void InvalidCommand() {
			FormatFailureOutputText();
			Console.WriteLine("Not a valid command.");
		}
		public static int ChangeRoom(List<IRoom> roomList, Player player, int x, int y, int z) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var roomName = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(roomName);
			roomList[roomIndex].LookRoom();
			if (!roomList[roomIndex].IsDiscovered) roomList[roomIndex].IsDiscovered = true;
			var roomType = roomList[roomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			ShowMap(roomList, player, 5, 10);
			return roomIndex;
		}
		public static void InvalidDirection() {
			FormatFailureOutputText();
			Console.WriteLine("You can't go that way!");
		}
		public static void InvalidVendorSell() {
			FormatFailureOutputText();
			Console.WriteLine("The vendor doesn't want that.");
		}
		public static bool QuitGame(Player player) {
			FormatAnnounceText();
			Console.WriteLine("Are you sure you want to quit?");
			var input = GetFormattedInput();
			if (input[0] == "yes" || input[0] == "y") {
				Console.WriteLine("Quitting the game.");
				player.CanSave = true;
				SaveGame(player);
				return true;
			}
			return false;
		}
		public static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void SaveGame(Player player) {
			FormatAnnounceText();
			if (player.CanSave == true) {
				var serializer = new JsonSerializer();
				serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializer.NullValueHandling = NullValueHandling.Ignore;
				serializer.TypeNameHandling = TypeNameHandling.Auto;
				serializer.Formatting = Formatting.Indented;
				serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (var sw = new StreamWriter("savegame.json"))
				using (var writer = new JsonTextWriter(sw)) {
					serializer.Serialize(writer, player, typeof(Player));
				}
				Console.WriteLine("Your game has been saved.");
				return;
			}
			FormatFailureOutputText();
			Console.WriteLine("You can't save inside a dungeon! Go outside first.");
		}
		public static int FleeRoom(List<IRoom> roomList, Player player) {
			var roomName = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(roomName);
			if (roomList[roomIndex].GoDown) {
				roomIndex = ChangeRoom(roomList, player, 0, 0, -1);
			}
			else if (roomList[roomIndex].GoUp) {
				roomIndex = ChangeRoom(roomList, player, 0, 0, 1);
			}
			else if (roomList[roomIndex].GoNorth) {
				roomIndex = ChangeRoom(roomList, player, 0, 1, 0);
			}
			else if (roomList[roomIndex].GoSouth) {
				roomIndex = ChangeRoom(roomList, player, 0, -1, 0);
			}
			else if (roomList[roomIndex].GoEast) {
				roomIndex = ChangeRoom(roomList, player, 1, 0, 0);
			}
			else if (roomList[roomIndex].GoWest) {
				roomIndex = ChangeRoom(roomList, player, -1, 0, 0);
			}
			else if (roomList[roomIndex].GoNorthEast) {
				roomIndex = ChangeRoom(roomList, player, 1, 1, 0);
			}
			else if (roomList[roomIndex].GoNorthWest) {
				roomIndex = ChangeRoom(roomList, player, -1, 1, 0);
			}
			else if (roomList[roomIndex].GoSouthEast) {
				roomIndex = ChangeRoom(roomList, player, 1, -1, 0);
			}
			else if (roomList[roomIndex].GoSouthWest) {
				roomIndex = ChangeRoom(roomList, player, -1, -1, 0);
			}
			return roomIndex;
		}
		public static void ShowMap(List<IRoom> roomList, Player player, int height, int width) {
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */ 
			for (var i = player.Y + height; i > player.Y - height; i--) {
				for (var j = player.X - width; j < player.X + width; j ++) {
					var mapX = j;
					var mapY = i;
					var mapZ = player.Z;
					var roomName = roomList.Find(f => f.X == mapX && f.Y == mapY && f.Z == mapZ);
					var roomIndex = roomList.IndexOf(roomName);
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							ShowPlayerDungeonUpDownTile();
							continue;
						}
						ShowPlayerDungeonTile();
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							ShowDiscoveredDungeonUpDownTile();
							continue;
						}
						ShowDiscoveredDungeonTile();
						continue;
					}
					ShowImpassibleDungeonTile();
				}
				// End of each j loop should create new line on console
				Console.WriteLine();
			}
			// Restore console background color to default
			Console.BackgroundColor = ConsoleColor.Black;
		}
	}
}