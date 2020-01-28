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
							if (player.InCombat == false) effect.ChangeArmorRound();
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
		public static void ChangeRoom(Player player, int x, int y, int z) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var room = Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			if (Rooms.IndexOf(room) != -1) RoomIndex = Rooms.IndexOf(room);
			Rooms[RoomIndex].LookRoom();
			if (!Rooms[RoomIndex].IsDiscovered) Rooms[RoomIndex].IsDiscovered = true;
			var roomType = Rooms[RoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom"; 
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
		public static void ShowUserOutput(Player player, IMonster opponent) {
			PlayerHelper.DisplayPlayerStats(player);
			opponent.DisplayStats();
			Rooms[RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
	}
}