using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DungeonGame
{
	public static class GameHandler
	{
		private static readonly Random RndGenerate = new Random();
		public static bool IsGameOver { get; set; }
		private static int GameTicks { get; set; }

		public static void CheckStatus(Player player)
		{
			GameTicks++;
			if (GameTicks % player._StatReplenishInterval == 0) ReplenishStatsOverTime(player);
			if (player._Effects.Any())
			{
				foreach (var effect in player._Effects.Where(effect => GameTicks % effect._TickDuration == 0).ToList())
				{
					switch (effect._EffectGroup)
					{
						case Effect.EffectType.Healing:
							effect.HealingRound(player);
							break;
						case Effect.EffectType.ChangePlayerDamage:
							if (!player._InCombat && effect._Name.Contains("berserk"))
							{
								effect._IsEffectExpired = true;
							}
							break;
						case Effect.EffectType.ChangeArmor:
							if (!player._InCombat && effect._Name.Contains("berserk"))
							{
								effect._IsEffectExpired = true;
							}
							if (!player._InCombat) effect.ChangeArmorRound();
							break;
						case Effect.EffectType.OnFire:
							effect.OnFireRound(player);
							break;
						case Effect.EffectType.Bleeding:
							effect.BleedingRound(player);
							break;
						case Effect.EffectType.Stunned:
							continue;
						case Effect.EffectType.Frozen:
							effect.FrozenRound(player);
							break;
						case Effect.EffectType.ReflectDamage:
							effect.ReflectDamageRound();
							break;
						case Effect.EffectType.ChangeStat:
							effect.ChangeStatRound();
							break;
						case Effect.EffectType.ChangeOpponentDamage:
							if (!player._InCombat) effect._IsEffectExpired = true;
							effect.ChangeOpponentDamageRound(player);
							break;
						case Effect.EffectType.BlockDamage:
							effect.BlockDamageRound();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			foreach (var room in RoomHandler.Rooms.Values)
			{
				foreach (var roomObject in room._RoomObjects.Where(
					roomObject => roomObject?.GetType() == typeof(Monster)))
				{
					var monster = (Monster)roomObject;
					RemovedExpiredEffectsAsync(monster);
					if (GameTicks % monster._StatReplenishInterval == 0 && monster._HitPoints > 0) ReplenishStatsOverTime(monster);
					if (!monster._Effects.Any()) continue;
					foreach (var effect in monster._Effects.Where(effect => GameTicks % effect._TickDuration == 0).ToList())
					{
						switch (effect._EffectGroup)
						{
							case Effect.EffectType.Healing:
								break;
							case Effect.EffectType.ChangePlayerDamage:
								break;
							case Effect.EffectType.ChangeArmor:
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
							case Effect.EffectType.ReflectDamage:
								break;
							case Effect.EffectType.ChangeStat:
								break;
							case Effect.EffectType.ChangeOpponentDamage:
								break;
							case Effect.EffectType.BlockDamage:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
			}
		}
		public static int GetRandomNumber(int lowNum, int highNum)
		{
			return RndGenerate.Next(lowNum, highNum + 1);
		}
		public static int RoundNumber(int number)
		{
			var lastDigit = number % 10;
			number /= 10;
			var newLastDigit = lastDigit >= 5 ? 1 : 0;
			number += newLastDigit;
			number *= 10;
			return number;
		}
		public static async void RemovedExpiredEffectsAsync(Player player)
		{
			await Task.Run(() =>
			{
				for (var i = 0; i < player._Effects.Count; i++)
				{
					if (!player._Effects[i]._IsEffectExpired) continue;
					switch (player._Effects[i]._EffectGroup)
					{
						case Effect.EffectType.Healing:
							break;
						case Effect.EffectType.ChangePlayerDamage:
							break;
						case Effect.EffectType.ChangeArmor:
							break;
						case Effect.EffectType.OnFire:
							break;
						case Effect.EffectType.Bleeding:
							break;
						case Effect.EffectType.Stunned:
							break;
						case Effect.EffectType.ReflectDamage:
							break;
						case Effect.EffectType.Frozen:
							break;
						case Effect.EffectType.ChangeStat:
							switch (player._Effects[i]._StatGroup)
							{
								case ChangeStat.StatType.Intelligence:
									player._Intelligence -= player._Effects[i]._EffectAmountOverTime;
									break;
								case ChangeStat.StatType.Strength:
									player._Strength -= player._Effects[i]._EffectAmountOverTime;
									break;
								case ChangeStat.StatType.Dexterity:
									player._Dexterity -= player._Effects[i]._EffectAmountOverTime;
									break;
								case ChangeStat.StatType.Constitution:
									player._Constitution -= player._Effects[i]._EffectAmountOverTime;
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
							PlayerHandler.CalculatePlayerStats(player);
							break;
						case Effect.EffectType.ChangeOpponentDamage:
							break;
						case Effect.EffectType.BlockDamage:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					player._Effects.RemoveAt(i);
					i--; // Keep i at same amount, since effects.count will decrease, to keep checking effect list properly
				}
			});
		}
		public static async void RemovedExpiredEffectsAsync(Monster monster)
		{
			await Task.Run(() =>
			{
				for (var i = 0; i < monster._Effects.Count; i++)
				{
					if (!monster._Effects[i]._IsEffectExpired) continue;
					switch (monster._Effects[i]._EffectGroup)
					{
						case Effect.EffectType.Healing:
							break;
						case Effect.EffectType.ChangePlayerDamage:
							break;
						case Effect.EffectType.ChangeArmor:
							break;
						case Effect.EffectType.OnFire:
							break;
						case Effect.EffectType.Bleeding:
							break;
						case Effect.EffectType.Stunned:
							monster._IsStunned = false;
							break;
						case Effect.EffectType.ReflectDamage:
							break;
						case Effect.EffectType.Frozen:
							break;
						case Effect.EffectType.ChangeStat:
							break;
						case Effect.EffectType.ChangeOpponentDamage:
							break;
						case Effect.EffectType.BlockDamage:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					monster._Effects.RemoveAt(i);
					i--; // Keep i at same amount, since effects.count will decrease, to keep checking effect list properly
				}
			});
		}
		private static void ReplenishStatsOverTime(Player player)
		{
			if (player._InCombat) return;
			if (player._HitPoints < player._MaxHitPoints) player._HitPoints++;
			switch (player._PlayerClass)
			{
				case Player.PlayerClassType.Mage:
					if (player._ManaPoints < player._MaxManaPoints) player._ManaPoints++;
					break;
				case Player.PlayerClassType.Warrior:
					if (player._RagePoints < player._MaxRagePoints) player._RagePoints++;
					break;
				case Player.PlayerClassType.Archer:
					if (player._ComboPoints < player._MaxComboPoints) player._ComboPoints++;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void ReplenishStatsOverTime(Monster monster)
		{
			if (monster._InCombat) return;
			if (monster._HitPoints < monster._MaxHitPoints) monster._HitPoints++;
			if (monster._EnergyPoints < monster._MaxEnergyPoints) monster._EnergyPoints++;
		}
		public static bool IsWholeNumber(string value)
		{
			return value.All(char.IsNumber);
		}
		public static Player LoadPlayer()
		{
			Player player;
			try
			{
				player = JsonConvert.DeserializeObject<Player>(File.ReadAllText(
					"playersave.json"), new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.Auto,
						NullValueHandling = NullValueHandling.Ignore
					});
			}
			catch (FileNotFoundException)
			{
				player = new PlayerBuilder().BuildNewPlayer();
				GearHandler.EquipInitialGear(player);
				/* Set initial room condition for player
				Begin game by putting player at coords 0, 7, 0, town entrance */
				RoomHandler.SetPlayerLocation(player, 0, 4, 0);
			}
			return player;
		}
		public static void LoadGame()
		{
			try
			{
				var serializerSettings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.Auto,
					NullValueHandling = NullValueHandling.Ignore
				};
				RoomHandler.Rooms = JsonConvert.DeserializeObject<Dictionary<Coordinate, IRoom>>(File.ReadAllText(
					"gamesave.json"), serializerSettings);
				// Insert blank space before game reload info for formatting
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"Reloading your saved game.");
				// Insert blank space after game reload info for formatting
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
			}
			catch (FileNotFoundException)
			{
				// Create new dungeon
				RoomHandler.Rooms = new RoomBuilder(200, 10, 0, 4, 0).RetrieveSpawnRooms();
			}
		}
		public static bool QuitGame(Player player)
		{
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Are you sure you want to quit?");
			OutputHandler.ShowUserOutput(player);
			OutputHandler.Display.ClearUserOutput();
			var input = InputHandler.GetFormattedInput(Console.ReadLine());
			Console.Clear();
			if (input[0] != "yes" && input[0] != "y") return false;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Quitting the game.");
			player._CanSave = true;
			IsGameOver = true;
			SaveGame(player);
			return true;
		}
		public static void SaveGame(Player player)
		{
			string outputString;
			if (player._CanSave)
			{
				var serializerPlayer = new JsonSerializer()
				{
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,
					Formatting = Formatting.Indented,
					PreserveReferencesHandling = PreserveReferencesHandling.All
				};
				serializerPlayer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				using (var sw = new StreamWriter("playersave.json"))
				using (var writer = new JsonTextWriter(sw))
				{
					serializerPlayer.Serialize(writer, player, typeof(Player));
				}
				var serializerRooms = new JsonSerializer()
				{
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,
					Formatting = Formatting.Indented,
					PreserveReferencesHandling = PreserveReferencesHandling.All
				};
				serializerRooms.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				using (var sw = new StreamWriter("gamesave.json"))
				using (var writer = new JsonTextWriter(sw))
				{
					serializerRooms.Serialize(writer, RoomHandler.Rooms, typeof(Dictionary<Coordinate, IRoom>));
				}
				outputString = "Your game has been saved.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static bool ContinuePlaying()
		{
			while (true)
			{
				const string continuePlayingMessage = "Do you want to keep playing? [Y/N]";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					continuePlayingMessage);
				OutputHandler.Display.RetrieveUserOutput();
				OutputHandler.Display.ClearUserOutput();
				var input = InputHandler.GetFormattedInput(Console.ReadLine());
				OutputHandler.Display.RetrieveUserOutput();
				OutputHandler.Display.ClearUserOutput();
				switch (input[0])
				{
					case "y":
						return true;
					case "n":
						return false;
				}
			}
		}
	}
}