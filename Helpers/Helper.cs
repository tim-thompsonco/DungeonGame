using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace DungeonGame {
	public static class Helper {
		private static readonly Random RndGenerate = new Random();
		public static UserOutput Display = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();
		public static bool IsGameOver { get; set; }
		public static int RoomIndex { get; set;}
		public static int GameTicks { get; set; }

		public static void CheckStatus(Player player, List<IRoom> spawnedRooms) {
			GameTicks++;
			RemovedExpiredEffects(player);
			if (GameTicks % player.StatReplenishInterval == 0) ReplenishStatsOverTime(player);
			if (player.Effects.Any()) {
				foreach (var effect in player.Effects.Where(effect => GameTicks % effect.TickDuration == 0)) {
					switch (effect.EffectGroup) {
						case Effect.EffectType.Healing:
							effect.HealingRound(player);
							break;
						case Effect.EffectType.ChangeDamage:
							if (player.InCombat == false && effect.Name.Contains("berserk")) {
								effect.IsEffectExpired = true;
							}
							break;
						case Effect.EffectType.ChangeArmor:
							if (player.InCombat == false && effect.Name.Contains("berserk")) {
								effect.IsEffectExpired = true;
							}
							if (player.InCombat == false) effect.ChangeArmorRound(player);
							break;
						case Effect.EffectType.AbsorbDamage:
							break;
						case Effect.EffectType.OnFire:
							break;
						case Effect.EffectType.Bleeding:
							break;
						case Effect.EffectType.Stunned:
							continue;
						case Effect.EffectType.Frozen:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			foreach (var room in spawnedRooms) {
				foreach (var roomObject in room.RoomObjects.Where(
					roomObject => roomObject.GetType() == typeof(Monster))) {
					var monster = (Monster) roomObject;
					RemovedExpiredEffects(monster);
					if (GameTicks % monster.StatReplenishInterval == 0 && monster.HitPoints > 0) ReplenishStatsOverTime(monster);
					if (!monster.Effects.Any()) continue;
						foreach (var effect in monster.Effects.Where(effect => GameTicks % effect.TickDuration == 0)) {
							switch (effect.EffectGroup) {
								case Effect.EffectType.Healing:
									break;
								case Effect.EffectType.ChangeDamage:
									break;
								case Effect.EffectType.ChangeArmor:
									break;
								case Effect.EffectType.AbsorbDamage:
									break;
								case Effect.EffectType.OnFire:
									effect.OnFireRound(monster);
									break;
								case Effect.EffectType.Bleeding:
									effect.BleedingRound(monster);
									break;
								case Effect.EffectType.Stunned:
									effect.StunnedRound(monster);
									break;
								case Effect.EffectType.Frozen:
									effect.FrozenRound(monster);
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
				}
			}
		}
		public static string[] GetFormattedInput(string userInput) {
			var inputFormatted = userInput.ToLower().Trim();
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
		public static void RequestCommand() {
			Display.StoreUserOutput("gray", "black", "Your command: ");
		}
		public static void PlayerDeath() {
			Display.StoreUserOutput("gray", "black", "You have died. Game over.");
		}
		public static void GameIntro() {
			var gameIntroString =
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot" +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.";
			for (var i = 0; i < gameIntroString.Length; i += GetGameWidth()) {
				if (gameIntroString.Length - i < Helper.GetGameWidth()) {
					Display.StoreUserOutput(
						FormatAnnounceText(), 
						FormatDefaultBackground(), 
						gameIntroString.Substring(i, gameIntroString.Length - i));
					continue;
				}
				Display.StoreUserOutput(
					FormatAnnounceText(), 
					FormatDefaultBackground(), 
					gameIntroString.Substring(i, GetGameWidth()));
			}
		}
		public static void ShowCommandHelp() {
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
					Display.StoreUserOutput(
						FormatAnnounceText(), 
						FormatDefaultBackground(), 
						commandHelpString.Substring(i, commandHelpString.Length - i));
					continue;
				}
				Display.StoreUserOutput(
					FormatAnnounceText(), 
					FormatDefaultBackground(), 
					commandHelpString.Substring(i, GetGameWidth()));
			}
		}
		
		public static void InvalidCommand() {
			Display.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), "Not a valid command.");
		}
		public static void ChangeRoom(List<IRoom> roomList, Player player, int x, int y, int z) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var changeRoomIndex = roomList.IndexOf(room);
			roomList[changeRoomIndex].LookRoom();
			if (!roomList[changeRoomIndex].IsDiscovered) roomList[changeRoomIndex].IsDiscovered = true;
			var roomType = roomList[changeRoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom"; 
			RoomIndex = changeRoomIndex;
		}
		public static void SetPlayerLocation(List<IRoom> roomList, Player player, int x, int y, int z) {
			player.X = x;
			player.Y = y;
			player.Z = z;
			// Room at new coordinates is found and room description displayed for user
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var setRoomIndex = roomList.IndexOf(room);
			if (setRoomIndex == -1) return;
			if (!roomList[setRoomIndex].IsDiscovered) roomList[setRoomIndex].IsDiscovered = true;
			var roomType = roomList[setRoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			RoomIndex = setRoomIndex;
		}
		public static void InvalidDirection() {
			const string outputString = "You can't go that way!";
			Display.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), outputString);
		}
		public static void InvalidVendorSell() {
			const string outputString = "The vendor doesn't want that.";
			Display.StoreUserOutput(
				FormatFailureOutputText(), FormatDefaultBackground(), outputString);
		}
		public static bool QuitGame(Player player, List<IRoom> spawnedRooms) {
			Display.StoreUserOutput(
				FormatAnnounceText(),
				FormatDefaultBackground(),
				"Are you sure you want to quit?");
			Display.BuildUserOutput();
			Display.ClearUserOutput();
			var input = GetFormattedInput(Console.ReadLine());
			if (input[0] == "yes" || input[0] == "y") {
				Display.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), "Quitting the game.");
				player.CanSave = true;
				SaveGame(player, spawnedRooms);
				return true;
			}
			return false;
		}
		public static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void SaveGame(Player player, List<IRoom> spawnedRooms) {
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
				Display.StoreUserOutput(
					FormatAnnounceText(), FormatDefaultBackground(), outputString);
				Display.BuildUserOutput();
				Display.ClearUserOutput();
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			Display.StoreUserOutput(
				FormatAnnounceText(), FormatDefaultBackground(), outputString);
		}
		public static void FleeRoom(List<IRoom> roomList, Player player) {
			if (roomList[RoomIndex].GoDown) {
				ChangeRoom(roomList, player, 0, 0, -1);
			}
			else if (roomList[RoomIndex].GoUp) {
				ChangeRoom(roomList, player, 0, 0, 1);
			}
			else if (roomList[RoomIndex].GoNorth) {
				ChangeRoom(roomList, player, 0, 1, 0);
			}
			else if (roomList[RoomIndex].GoSouth) {
				ChangeRoom(roomList, player, 0, -1, 0);
			}
			else if (roomList[RoomIndex].GoEast) {
				ChangeRoom(roomList, player, 1, 0, 0);
			}
			else if (roomList[RoomIndex].GoWest) {
				ChangeRoom(roomList, player, -1, 0, 0);
			}
			else if (roomList[RoomIndex].GoNorthEast) {
				ChangeRoom(roomList, player, 1, 1, 0);
			}
			else if (roomList[RoomIndex].GoNorthWest) {
				ChangeRoom(roomList, player, -1, 1, 0);
			}
			else if (roomList[RoomIndex].GoSouthEast) {
				ChangeRoom(roomList, player, 1, -1, 0);
			}
			else if (roomList[RoomIndex].GoSouthWest) {
				ChangeRoom(roomList, player, -1, -1, 0);
			}
		}
		public static int GetRandomNumber(int lowNum, int highNum) {
			return RndGenerate.Next(lowNum, highNum + 1);
		}
		public static int RoundNumber(int number) {
			var lastDigit = number % 10;
			number /= 10;
			var newLastDigit = lastDigit >= 5 ? 1 : 0;
			number += newLastDigit;
			number *= 10;
			return number;
		}
		public static void RemovedExpiredEffects(Player player) {
			for (var i = 0; i < player.Effects.Count; i++) {
				if (player.Effects[i].IsEffectExpired) player.Effects.RemoveAt(i);				
			}
		}
		public static void RemovedExpiredEffects(IMonster monster) {
			for (var i = 0; i < monster.Effects.Count; i++) {
				if (monster.Effects[i].IsEffectExpired) monster.Effects.RemoveAt(i);				
			}
		}
		public static void ReplenishStatsOverTime(Player player) {
			if (player.InCombat) return;
			if (player.HitPoints == player.MaxHitPoints) return;
			player.HitPoints += 1;
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					if (player.ManaPoints == player.MaxManaPoints) return;
					player.ManaPoints += 1;
					break;
				case Player.PlayerClassType.Warrior:
					if (player.RagePoints == player.MaxRagePoints) return;
					player.RagePoints += 1;
					break;
				case Player.PlayerClassType.Archer:
					if (player.ComboPoints == player.MaxComboPoints) return;
					player.ComboPoints += 1;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void ReplenishStatsOverTime(IMonster monster) {
			if (monster.InCombat) return;
			if (monster.HitPoints == monster.MaxHitPoints) return;
			monster.HitPoints += 1;
		}
		public static bool IsWholeNumber(string value) {
			return value.All(char.IsNumber);
		}
		public static void ShowUserOutput(List<IRoom> spawnedRooms, Player player) {
			PlayerHelper.DisplayPlayerStats(player);
			spawnedRooms[RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(spawnedRooms, player, GetMiniMapHeight(), GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
	}
}