﻿using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DungeonGame {
	public static class Helper {
		private static readonly Random _rndGenerate = new Random();
		
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
		public static string FormatHealthBackground() {
			return "darkred";
		}
		public static string FormatManaBackground() {
			return "darkblue";
		}
		public static string FormatRageBackground() {
			return "darkyellow";
		}
		public static string FormatComboBackground() {
			return "darkyellow";
		}
		public static string FormatExpBackground() {
			return "darkcyan";
		}
		public static string FormatSuccessOutputText() {
			return "green";
		}
		public static string FormatHiddenOutputText() {
			return "black";
		}
		public static string FormatFailureOutputText() {
			return "darkcyan";
		}
		public static string FormatRoomOutputText() {
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
		public static string FormatUpDownIndicator() {
			return "black";
		}
		public static string FormatAnnounceText() {
			return "gray";
		}
		public static string FormatPlayerTile() {
			return "green";
		}
		public static string FormatDiscoveredTile() {
			return "darkgray";
		}
		public static string FormatTextBorder() {
			return "========================================================";
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
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			output.StoreUserOutput(
				FormatAnnounceText(), FormatDefaultBackground(), "Please enter a player name.");
			string playerName;
			while (true) {
				var sameLineOutput = new List<string>();
				sameLineOutput.Add(FormatAnnounceText());
				sameLineOutput.Add(FormatDefaultBackground());
				sameLineOutput.Add("Player name: ");
				output.StoreUserOutput(sameLineOutput);
				output.RetrieveUserOutput();
				playerName = textInfo.ToTitleCase(Console.ReadLine());
				output.ClearUserOutput();
				var playerNameString = "Your player name is " + playerName + ", is that correct? [Y] or [N].";
				output.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), playerNameString);
				output.RetrieveUserOutput();
				RequestCommand(output);
				var input = GetFormattedInput();
				output.ClearUserOutput();
				if (input[0] == "y") {
					break;
				}
			}
			while (true) {
				output.StoreUserOutput(
					FormatAnnounceText(),
					FormatDefaultBackground(),
					"Please enter your class. You can select Mage, Warrior, or Archer.");
				var sameLineOutputClass = new List<string>();
				sameLineOutputClass.Add(FormatAnnounceText());
				sameLineOutputClass.Add(FormatDefaultBackground());
				sameLineOutputClass.Add("Player class: ");
				output.StoreUserOutput(sameLineOutputClass);
				output.RetrieveUserOutput();
				var userInput = GetFormattedInput();
				output.ClearUserOutput();
				var playerClassInput = textInfo.ToTitleCase(userInput[0].ToString());
				if (playerClassInput != "Mage" && playerClassInput != "Warrior" && playerClassInput != "Archer") {
					output.StoreUserOutput(
						FormatAnnounceText(), 
						FormatDefaultBackground(), 
						"Invalid selection. Please enter Mage, Warrior, or Archer for your class.");
					output.RetrieveUserOutput();
					continue;
				}
				var playerClass = playerClassInput;
				var playerClassString = "Your player class is " + playerClass + ", is that correct? [Y] or [N].";
				output.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), playerClassString);
				RequestCommand(output);
				output.RetrieveUserOutput();
				var input = GetFormattedInput();
				if (input[0] == "y") {
					output.ClearUserOutput();
					switch(playerClass) {
						case "Archer":
							var playerArcher = new Player(playerName, Player.PlayerClassType.Archer);
							var archerString = "You have selected Archer. You can 'use' an ability, for example " +
							                  "'use gut', if you have an ability named gut shot in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability distance'. " +
							                  "To use a bow, you must have a quiver equipped, and it must not be empty. To reload " +
							                  "your quiver, you can 'reload'.";
							for (var i = 0; i < archerString.Length; i += GetGameWidth()) {
								if (archerString.Length - i < Helper.GetGameWidth()) {
									output.StoreUserOutput(
										FormatAnnounceText(), 
										FormatDefaultBackground(), 
										archerString.Substring(i, archerString.Length - i));
									continue;
								}
								output.StoreUserOutput(
									FormatAnnounceText(), 
									FormatDefaultBackground(), 
									archerString.Substring(i, GetGameWidth()));
							}
							return playerArcher;
						case "Mage":
							var playerMage = new Player(playerName, Player.PlayerClassType.Mage);
							var mageString = "You have selected Mage. You can 'cast' a spell, for example " +
							                  "'cast fireball', if you have a spell named fireball in your spellbook. To see " +
							                  "the list of spells in your spellbook, you can 'list spells'. To view info " +
							                  "about a spell, you can 'spell' the spell name. For example, 'spell fireball'.";
							for (var i = 0; i < mageString.Length; i += GetGameWidth()) {
								if (mageString.Length - i < Helper.GetGameWidth()) {
									output.StoreUserOutput(
										FormatAnnounceText(), 
										FormatDefaultBackground(), 
										mageString.Substring(i, mageString.Length - i));
									continue;
								}
								output.StoreUserOutput(
									FormatAnnounceText(), 
									FormatDefaultBackground(), 
									mageString.Substring(i, GetGameWidth()));
							}
							return playerMage;
							case "Warrior":
							var playerWarrior = new Player(playerName, Player.PlayerClassType.Warrior);
							var warriorString = "You have selected Warrior. You can 'use' an ability, for example " +
							                  "'use charge', if you have an ability named charge in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability charge'.";
							for (var i = 0; i < warriorString.Length; i += GetGameWidth()) {
								if (warriorString.Length - i < Helper.GetGameWidth()) {
									output.StoreUserOutput(
										FormatAnnounceText(), 
										FormatDefaultBackground(), 
										warriorString.Substring(i, warriorString.Length - i));
									continue;
								}
								output.StoreUserOutput(
									FormatAnnounceText(), 
									FormatDefaultBackground(), 
									warriorString.Substring(i, GetGameWidth()));
							}
							return playerWarrior;
					}
				}
			}
		}
		public static void InvalidCommand(UserOutput output) {
			output.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), "Not a valid command.");
		}
		public static int ChangeRoom(List<IRoom> roomList, Player player, int x, int y, int z, UserOutput output) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(room);
			roomList[roomIndex].LookRoom(output);
			if (!roomList[roomIndex].IsDiscovered) roomList[roomIndex].IsDiscovered = true;
			var roomType = roomList[roomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			return roomIndex;
		}
		public static int SetPlayerLocation(List<IRoom> roomList, Player player, int x, int y, int z) {
			player.X = x;
			player.Y = y;
			player.Z = z;
			// Room at new coordinates is found and room description displayed for user
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(room);
			if (roomIndex == -1) return -1;
			if (!roomList[roomIndex].IsDiscovered) roomList[roomIndex].IsDiscovered = true;
			var roomType = roomList[roomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			return roomIndex;
		}
		public static void InvalidDirection(UserOutput output) {
			const string outputString = "You can't go that way!";
			output.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), outputString);
		}
		public static void InvalidVendorSell(UserOutput output) {
			const string outputString = "The vendor doesn't want that.";
			output.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), outputString);
		}
		public static bool QuitGame(Player player, UserOutput output, List<IRoom> spawnedRooms) {
			output.StoreUserOutput(
				FormatAnnounceText(),
				FormatDefaultBackground(),
				"Are you sure you want to quit?");
			output.RetrieveUserOutput();
			output.ClearUserOutput();
			var input = GetFormattedInput();
			if (input[0] == "yes" || input[0] == "y") {
				output.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), "Quitting the game.");
				player.CanSave = true;
				SaveGame(player, output, spawnedRooms);
				return true;
			}
			return false;
		}
		public static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void SaveGame(Player player, UserOutput output, List<IRoom> spawnedRooms) {
			string outputString;
			if (player.CanSave == true) {
				var serializerPlayer = new JsonSerializer();
				serializerPlayer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializerPlayer.NullValueHandling = NullValueHandling.Ignore;
				serializerPlayer.TypeNameHandling = TypeNameHandling.Auto;
				serializerPlayer.Formatting = Formatting.Indented;
				serializerPlayer.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (var sw = new StreamWriter("playersave.json"))
				using (var writer = new JsonTextWriter(sw)) {
					serializerPlayer.Serialize(writer, player, typeof(Player));
				}
				var serializerRooms = new JsonSerializer();
				serializerRooms.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializerRooms.NullValueHandling = NullValueHandling.Ignore;
				serializerRooms.TypeNameHandling = TypeNameHandling.Auto;
				serializerRooms.Formatting = Formatting.Indented;
				serializerRooms.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (var sw = new StreamWriter("gamesave.json"))
				using (var writer = new JsonTextWriter(sw)) {
					serializerPlayer.Serialize(writer, spawnedRooms, typeof(List<IRoom>));
				}
				outputString = "Your game has been saved.";
				output.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), outputString);
				output.RetrieveUserOutput();
				output.ClearUserOutput();
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			output.StoreUserOutput(
				FormatAnnounceText(), FormatDefaultBackground(), outputString);
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
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatPlayerTile()); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(FormatPlayerTile()); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == endRightPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatPlayerTile()); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(FormatPlayerTile()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == startLeftPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatPlayerTile()); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(FormatPlayerTile()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered && j == endRightPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 && roomList[roomIndex].IsDiscovered && j == startLeftPos) {
						if (roomList[roomIndex].GoUp || roomList[roomIndex].GoDown) {
							sameLineOutput.Add(FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (j == endRightPos) {
						sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(FormatDefaultBackground()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
					}
					if (j == startLeftPos) {
						sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(FormatDefaultBackground()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
					}
					sameLineOutput.Add(FormatGeneralInfoText()); // Foreground color
					sameLineOutput.Add(FormatDefaultBackground()); // Background color
					sameLineOutput.Add("  "); // What prints to display
				}
				output.StoreUserOutput(sameLineOutput);
			}
			output.StoreUserOutput(
				FormatGeneralInfoText(), 
				FormatDefaultBackground(), 
				mapBorder.ToString());
			return output;
		}
		public static int GetRandomNumber(int lowNum, int highNum) {
			return _rndGenerate.Next(lowNum, highNum + 1);
		}
		public static int RoundNumber(int number) {
			var lastDigit = number % 10;
			number /= 10;
			var newLastDigit = lastDigit >= 5 ? 1 : 0;
			number += newLastDigit;
			number *= 10;
			return number;
		}
	}
}