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
		public static List<IRoom> Rooms { get; set; }
		public static bool IsGameOver { get; set; }
		public static int RoomIndex { get; set;}
		public static int GameTicks { get; set; }

		public static void CheckStatus(Player player) {
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
			foreach (var room in Rooms) {
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
			Display.StoreUserOutput(Settings.FormatAnnounceText(), 
				Settings.FormatDefaultBackground(), "Your command: ");
		}
		public static void PlayerDeath() {
			Display.StoreUserOutput(Settings.FormatAnnounceText(), 
				Settings.FormatDefaultBackground(), "You have died. Game over.");
		}
		public static void GameIntro() {
			var gameIntroString =
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot" +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.";
			for (var i = 0; i < gameIntroString.Length; i += Settings.GetGameWidth()) {
				if (gameIntroString.Length - i < Settings.GetGameWidth()) {
					Display.StoreUserOutput(
						Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
						gameIntroString.Substring(i, gameIntroString.Length - i));
					continue;
				}
				Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
					gameIntroString.Substring(i, Settings.GetGameWidth()));
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
			for (var i = 0; i < commandHelpString.Length; i += Settings.GetGameWidth()) {
				if (commandHelpString.Length - i < Settings.GetGameWidth()) {
					Display.StoreUserOutput(
						Settings.FormatAnnounceText(),Settings.FormatDefaultBackground(), 
						commandHelpString.Substring(i, commandHelpString.Length - i));
					continue;
				}
				Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
					commandHelpString.Substring(i, Settings.GetGameWidth()));
			}
		}
		
		public static void InvalidCommand() {
			Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), 
				Settings.FormatDefaultBackground(),"Not a valid command.");
		}
		public static void ChangeRoom(Player player, int x, int y, int z) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var room = Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var changeRoomIndex = Rooms.IndexOf(room);
			Rooms[changeRoomIndex].LookRoom();
			if (!Rooms[changeRoomIndex].IsDiscovered) Rooms[changeRoomIndex].IsDiscovered = true;
			var roomType = Rooms[changeRoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom"; 
			RoomIndex = changeRoomIndex;
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z) {
			player.X = x;
			player.Y = y;
			player.Z = z;
			// Room at new coordinates is found and room description displayed for user
			var room = Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var setRoomIndex = Rooms.IndexOf(room);
			if (setRoomIndex == -1) return;
			if (!Rooms[setRoomIndex].IsDiscovered) Rooms[setRoomIndex].IsDiscovered = true;
			var roomType = Rooms[setRoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			RoomIndex = setRoomIndex;
		}
		public static void InvalidDirection() {
			const string outputString = "You can't go that way!";
			Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static void InvalidVendorSell() {
			const string outputString = "The vendor doesn't want that.";
			Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static bool QuitGame(Player player) {
			Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Are you sure you want to quit?");
			Display.BuildUserOutput();
			Display.ClearUserOutput();
			var input = GetFormattedInput(Console.ReadLine());
			if (input[0] == "yes" || input[0] == "y") {
				Display.StoreUserOutput(
					Settings.FormatAnnounceText(), 
					Settings.FormatDefaultBackground(), "Quitting the game.");
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
					serializerPlayer.Serialize(writer, typeof(List<IRoom>));
				}
				outputString = "Your game has been saved.";
				Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
				Display.BuildUserOutput();
				Display.ClearUserOutput();
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static void FleeRoom(Player player) {
			if (Rooms[RoomIndex].GoDown) {
				ChangeRoom(player, 0, 0, -1);
			}
			else if (Rooms[RoomIndex].GoUp) {
				ChangeRoom(player, 0, 0, 1);
			}
			else if (Rooms[RoomIndex].GoNorth) {
				ChangeRoom(player, 0, 1, 0);
			}
			else if (Rooms[RoomIndex].GoSouth) {
				ChangeRoom(player, 0, -1, 0);
			}
			else if (Rooms[RoomIndex].GoEast) {
				ChangeRoom(player, 1, 0, 0);
			}
			else if (Rooms[RoomIndex].GoWest) {
				ChangeRoom(player, -1, 0, 0);
			}
			else if (Rooms[RoomIndex].GoNorthEast) {
				ChangeRoom(player, 1, 1, 0);
			}
			else if (Rooms[RoomIndex].GoNorthWest) {
				ChangeRoom(player, -1, 1, 0);
			}
			else if (Rooms[RoomIndex].GoSouthEast) {
				ChangeRoom(player, 1, -1, 0);
			}
			else if (Rooms[RoomIndex].GoSouthWest) {
				ChangeRoom(player, -1, -1, 0);
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
		public static void ShowUserOutput(Player player) {
			PlayerHelper.DisplayPlayerStats(player);
			Rooms[RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
	}
}