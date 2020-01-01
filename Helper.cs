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
		public static int GetGameWidth() {
			return 100 - GetBufferGap();
		}
		public static int GetMiniMapHeight() {
			return 5;
		}
		public static int GetMiniMapWidth() {
			return 10;
		}
		public static int GetBufferGap() {
			return 5;
		}
		public static int GetMiniMapBorderWidth() {
			return (GetMiniMapWidth() * 4) + 6;
		}
		public static string FormatDefaultBackground() {
			return "black";
		}
		public static string FormatSuccessOutputText() {
			return "green";
		}
		public static string FormatFailureOutputText() {
			return "darkcyan";
		}
		public static string FormatOnFireText() {
			return "yellow";
		}
		public static string FormatAttackSuccessText() {
			return "red";
		}
		public static string FormatAttackFailText() {
			return "darkred";
		}
		public static string FormatInfoText() {
			return "white";
		}
		public static string FormatLevelUpText() {
			return "cyan";
		}
		public static string FormatGeneralInfoText() {
			return "darkgreen";
		}
		public static string FormatAnnounceText() {
			return "gray";
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
		public static void RequestCommand(UserOutput output) {
			output.StoreUserOutput("gray", "black", "Your command: ");
		}
		public static void PlayerDeath(UserOutput output) {
			output.StoreUserOutput("gray", "black", "You have died. Game over.");
		}
		public static void GameIntro(UserOutput output) {
			var gameIntroString =
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot" +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.";
			for (var i = 0; i < gameIntroString.Length; i += GetGameWidth()) {
				if (gameIntroString.Length - i < Helper.GetGameWidth()) {
					output.StoreUserOutput(
						FormatAnnounceText(), 
						FormatDefaultBackground(), 
						gameIntroString.Substring(i, gameIntroString.Length - i));
					continue;
				}
				output.StoreUserOutput(
					FormatAnnounceText(), 
					FormatDefaultBackground(), 
					gameIntroString.Substring(i, GetGameWidth()));
			}
		}
		public static void ShowCommandHelp(UserOutput output) {
			var commandHelpString = 
				"Commands: Players may move in any direction of the game using a shortkey or the full direction name. " +
				"For example, if you wish to go north, you may type either 'N' or 'North'. If a player wishes to look " +
				"at something, they can use 'l' or 'look' and then the name of what they want to look at. For example " +
				"'l zombie' or 'look zombie' would allow you to look at a zombie in the room. The same commands will  " +
				"work to loot a monster that you have killed. Look or 'L' by itself will look at the room. Other common " +
				"commands will be shown to the player. Any object that is consumable, such as a potion, can be drank " +
				"by typing 'drink' and then the name of the potion or object. To use armor or weapons, you must 'equip' " +
				"them. You can 'unequip' them as well.";
			for (var i = 0; i < commandHelpString.Length; i += GetGameWidth()) {
				if (commandHelpString.Length - i < Helper.GetGameWidth()) {
					output.StoreUserOutput(
						FormatAnnounceText(), 
						FormatDefaultBackground(), 
						commandHelpString.Substring(i, commandHelpString.Length - i));
					continue;
				}
				output.StoreUserOutput(
					FormatAnnounceText(), 
					FormatDefaultBackground(), 
					commandHelpString.Substring(i, GetGameWidth()));
			}
		}
		public static Player BuildNewPlayer(UserOutput output) {
			FormatAnnounceText();
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			Console.WriteLine("Please enter a player name.\n");
			string playerName;
			while (true) {
				Console.Write("Player name: ");
				playerName = textInfo.ToTitleCase(Console.ReadLine().ToString());
				Console.WriteLine("Your player name is {0}, is that correct? [Y] or [N].", playerName);
				RequestCommand(output);
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
				RequestCommand(output);
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
		public static int ChangeRoom(List<IRoom> roomList, Player player, int x, int y, int z, UserOutput output) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var roomName = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(roomName);
			roomList[roomIndex].LookRoom(output);
			if (!roomList[roomIndex].IsDiscovered) roomList[roomIndex].IsDiscovered = true;
			var roomType = roomList[roomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			return roomIndex;
		}
		public static void InvalidDirection(UserOutput output) {
			const string outputString = "You can't go that way!";
			output.StoreUserOutput("darkcyan", "black", outputString);
		}
		public static void InvalidVendorSell(UserOutput output) {
			const string outputString = "The vendor doesn't want that.";
			output.StoreUserOutput("darkcyan", "black", outputString);
		}
		public static bool QuitGame(Player player, UserOutput output) {
			FormatAnnounceText();
			Console.WriteLine("Are you sure you want to quit?");
			var input = GetFormattedInput();
			if (input[0] == "yes" || input[0] == "y") {
				Console.WriteLine("Quitting the game.");
				player.CanSave = true;
				SaveGame(player, output);
				return true;
			}
			return false;
		}
		public static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void SaveGame(Player player, UserOutput output) {
			string outputString;
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
				outputString = "Your game has been saved.";
				output.StoreUserOutput("gray", "black", outputString);
				return;
			}

			outputString = "You can't save inside a dungeon! Go outside first.";
			output.StoreUserOutput("gray", "black", outputString);
		}
		public static int FleeRoom(List<IRoom> roomList, Player player, UserOutput output) {
			var roomName = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(roomName);
			if (roomList[roomIndex].GoDown) {
				roomIndex = ChangeRoom(roomList, player, 0, 0, -1, output);
			}
			else if (roomList[roomIndex].GoUp) {
				roomIndex = ChangeRoom(roomList, player, 0, 0, 1, output);
			}
			else if (roomList[roomIndex].GoNorth) {
				roomIndex = ChangeRoom(roomList, player, 0, 1, 0, output);
			}
			else if (roomList[roomIndex].GoSouth) {
				roomIndex = ChangeRoom(roomList, player, 0, -1, 0, output);
			}
			else if (roomList[roomIndex].GoEast) {
				roomIndex = ChangeRoom(roomList, player, 1, 0, 0, output);
			}
			else if (roomList[roomIndex].GoWest) {
				roomIndex = ChangeRoom(roomList, player, -1, 0, 0, output);
			}
			else if (roomList[roomIndex].GoNorthEast) {
				roomIndex = ChangeRoom(roomList, player, 1, 1, 0, output);
			}
			else if (roomList[roomIndex].GoNorthWest) {
				roomIndex = ChangeRoom(roomList, player, -1, 1, 0, output);
			}
			else if (roomList[roomIndex].GoSouthEast) {
				roomIndex = ChangeRoom(roomList, player, 1, -1, 0, output);
			}
			else if (roomList[roomIndex].GoSouthWest) {
				roomIndex = ChangeRoom(roomList, player, -1, -1, 0, output);
			}
			return roomIndex;
		}
		public static UserOutput ShowMap(List<IRoom> roomList, Player player, int height, int width) {
			var output = new UserOutput();
			// Draw top border of map
			var mapBorder = new StringBuilder();
			// Minimap border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < GetMiniMapBorderWidth(); b++) {
				mapBorder.Append("=");
			}
			output.StoreUserOutput(
				"darkgreen", 
				FormatDefaultBackground(), 
				mapBorder.ToString());
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */
			for (var i = player.Y + height; i > player.Y - height; i--) {
				var sameLineOutput = new List<string>();
				var startLeftPos = player.X - width;
				var endRightPos = player.X + width - 1;
				for (var j = startLeftPos; j <= endRightPos; j ++) {
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
							sameLineOutput.Add("black"); // Foreground color
							sameLineOutput.Add("green"); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add("black"); // Foreground color
						sameLineOutput.Add("green"); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == endRightPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add("black"); // Foreground color
							sameLineOutput.Add("green"); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add("black"); // Foreground color
						sameLineOutput.Add("green"); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == startLeftPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add("black"); // Foreground color
							sameLineOutput.Add("green"); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add("black"); // Foreground color
						sameLineOutput.Add("green"); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add("darkgreen"); // Foreground color
							sameLineOutput.Add("darkgray"); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add("darkgray"); // Foreground color
						sameLineOutput.Add("darkgray"); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered && j == endRightPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add("darkgreen"); // Foreground color
							sameLineOutput.Add("darkgray"); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add("darkgray"); // Foreground color
						sameLineOutput.Add("darkgray"); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered && j == startLeftPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add("darkgreen"); // Foreground color
							sameLineOutput.Add("darkgray"); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add("darkgray"); // Foreground color
						sameLineOutput.Add("darkgray"); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (j == endRightPos) {
						sameLineOutput.Add("darkgreen"); // Foreground color
						sameLineOutput.Add("black"); // Background color
						sameLineOutput.Add("   |"); // What prints to display
					}
					if (j == startLeftPos) {
						sameLineOutput.Add("darkgreen"); // Foreground color
						sameLineOutput.Add("black"); // Background color
						sameLineOutput.Add("|   "); // What prints to display
					}
					sameLineOutput.Add("darkgreen"); // Foreground color
					sameLineOutput.Add("black"); // Background color
					sameLineOutput.Add("  "); // What prints to display
				}
				output.StoreUserOutput(sameLineOutput);
			}
			output.StoreUserOutput(
				"darkgreen", 
				FormatDefaultBackground(), 
				mapBorder.ToString());
			return output;
		}
	}
}