using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DungeonGame.Helpers {
	public static class GameHelper {
		public static bool IsGameOver { get; set; }
		private static readonly Random _rndGenerate = new Random();
		private static int _gameTicks;

		public static void CheckStatus(Player player) {
			_gameTicks++;

			if (_gameTicks % player.StatReplenishInterval == 0) {
				ReplenishStatsOverTime(player);
			}

			if (player.Effects.Any()) {
				foreach (IEffect effect in player.Effects.Where(effect => _gameTicks % effect.TickDuration == 0).ToList()) {
					if (effect is ChangePlayerDamageEffect changePlayerDmgEffect && !player.InCombat && effect.Name.Contains("berserk")) {
						changePlayerDmgEffect.ProcessChangePlayerDamageRound(player);
					} else if (effect is ChangeArmorEffect changeArmorEffect) {
						if (!player.InCombat && effect.Name.Contains("berserk")) {
							changeArmorEffect.IsEffectExpired = true;
						}

						if (!player.InCombat) {
							changeArmorEffect.ProcessChangeArmorRound();
						}
					} else if (effect is FrozenEffect frozenEffect) {
						frozenEffect.ProcessFrozenRound();
					} else if (effect is ChangeStatEffect changeStatEffect) {
						changeStatEffect.ProcessChangeStatRound(player);
					} else if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
						if (!player.InCombat) {
							effect.IsEffectExpired = true;
						}

						changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
					} else {
						effect.ProcessRound();
					}
				}
			}

			foreach (IRoom room in RoomHelper.Rooms.Values) {
				foreach (IName roomObject in room.RoomObjects.Where(
					roomObject => roomObject?.GetType() == typeof(Monster))) {
					Monster monster = (Monster)roomObject;
					RemovedExpiredEffectsAsync(monster);
					if (_gameTicks % monster.StatReplenishInterval == 0 && monster.HitPoints > 0) {
						ReplenishStatsOverTime(monster);
					}

					if (!monster.Effects.Any()) {
						continue;
					}

					foreach (IEffect effect in monster.Effects.Where(effect => _gameTicks % effect.TickDuration == 0).ToList()) {
						if (effect is FrozenEffect frozenEffect) {
							frozenEffect.ProcessFrozenRound(monster);
						} else {
							effect.ProcessRound();
						}
					}
				}
			}
		}

		public static int GetRandomNumber(int lowNum, int highNum) {
			return _rndGenerate.Next(lowNum, highNum + 1);
		}

		public static int RoundNumber(int number) {
			int lastDigit = number % 10;
			number /= 10;
			int newLastDigit = lastDigit >= 5 ? 1 : 0;
			number += newLastDigit;
			number *= 10;
			return number;
		}

		public static async void RemovedExpiredEffectsAsync(Player player) {
			await Task.Run(() => {
				foreach (IEffect effect in player.Effects.ToList()) {
					if (effect.IsEffectExpired) {
						player.Effects.Remove(effect);
					}
				}
			});
		}

		public static async void RemovedExpiredEffectsAsync(Monster monster) {
			await Task.Run(() => {
				foreach (IEffect effect in monster.Effects.ToList()) {
					if (effect is StunnedEffect && effect.IsEffectExpired) {
						monster.IsStunned = false;
					}

					if (effect.IsEffectExpired) {
						monster.Effects.Remove(effect);
					}
				}
			});
		}

		private static void ReplenishStatsOverTime(Player player) {
			if (player.InCombat) {
				return;
			}

			if (player.HitPoints < player.MaxHitPoints) {
				player.HitPoints++;
			}

			switch (player.PlayerClass) {
				case PlayerClassType.Mage:
					if (player.ManaPoints < player.MaxManaPoints) {
						player.ManaPoints++;
					}

					break;
				case PlayerClassType.Warrior:
					if (player.RagePoints < player.MaxRagePoints) {
						player.RagePoints++;
					}

					break;
				case PlayerClassType.Archer:
					if (player.ComboPoints < player.MaxComboPoints) {
						player.ComboPoints++;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void ReplenishStatsOverTime(Monster monster) {
			if (monster.InCombat) {
				return;
			}

			if (monster.HitPoints < monster.MaxHitPoints) {
				monster.HitPoints++;
			}

			if (monster.EnergyPoints < monster.MaxEnergyPoints) {
				monster.EnergyPoints++;
			}
		}

		public static bool IsWholeNumber(string value) {
			return value.All(char.IsNumber);
		}

		public static Player LoadPlayer() {
			Player player;
			try {
				player = JsonConvert.DeserializeObject<Player>(File.ReadAllText(
					"playersave.json"), new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Auto,
						NullValueHandling = NullValueHandling.Ignore
					});
			} catch (FileNotFoundException) {
				player = new PlayerBuilder().BuildNewPlayer();
				GearHelper.EquipInitialGear(player);
				/* Set initial room condition for player
				Begin game by putting player at coords 0, 7, 0, town entrance */
				RoomHelper.SetPlayerLocation(player, 0, 4, 0);
			}
			return player;
		}

		public static void LoadGame() {
			try {
				JsonSerializerSettings serializerSettings = new JsonSerializerSettings() {
					TypeNameHandling = TypeNameHandling.Auto,
					NullValueHandling = NullValueHandling.Ignore
				};
				RoomHelper.Rooms = JsonConvert.DeserializeObject<Dictionary<Coordinate, IRoom>>(File.ReadAllText(
					"gamesave.json"), serializerSettings);
				// Insert blank space before game reload info for formatting
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"Reloading your saved game.");
				// Insert blank space after game reload info for formatting
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
			} catch (FileNotFoundException) {
				// Create new dungeon
				RoomHelper.Rooms = new RoomBuilder(200, 10, 0, 4, 0).RetrieveSpawnRooms();
			}
		}

		public static bool QuitGame(Player player) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Are you sure you want to quit?");
			OutputHelper.ShowUserOutput(player);
			OutputHelper.Display.ClearUserOutput();
			string[] input = InputHelper.GetFormattedInput(Console.ReadLine());
			Console.Clear();
			if (input[0] != "yes" && input[0] != "y") {
				return false;
			}

			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Quitting the game.");
			player.CanSave = true;
			IsGameOver = true;
			SaveGame(player);
			return true;
		}

		public static void SaveGame(Player player) {
			string outputString;
			if (player.CanSave) {
				JsonSerializer serializerPlayer = new JsonSerializer() {
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,
					Formatting = Formatting.Indented,
					PreserveReferencesHandling = PreserveReferencesHandling.All
				};
				serializerPlayer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				using (StreamWriter sw = new StreamWriter("playersave.json"))
				using (JsonTextWriter writer = new JsonTextWriter(sw)) {
					serializerPlayer.Serialize(writer, player, typeof(Player));
				}
				JsonSerializer serializerRooms = new JsonSerializer() {
					NullValueHandling = NullValueHandling.Ignore,
					TypeNameHandling = TypeNameHandling.Auto,
					Formatting = Formatting.Indented,
					PreserveReferencesHandling = PreserveReferencesHandling.All
				};
				serializerRooms.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				using (StreamWriter sw = new StreamWriter("gamesave.json"))
				using (JsonTextWriter writer = new JsonTextWriter(sw)) {
					serializerRooms.Serialize(writer, RoomHelper.Rooms, typeof(Dictionary<Coordinate, IRoom>));
				}
				outputString = "Your game has been saved.";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
		}

		public static bool ContinuePlaying() {
			while (true) {
				const string continuePlayingMessage = "Do you want to keep playing? [Y/N]";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					continuePlayingMessage);
				OutputHelper.Display.RetrieveUserOutput();
				OutputHelper.Display.ClearUserOutput();
				string[] input = InputHelper.GetFormattedInput(Console.ReadLine());
				OutputHelper.Display.RetrieveUserOutput();
				OutputHelper.Display.ClearUserOutput();
				switch (input[0]) {
					case "y":
						return true;
					case "n":
						return false;
				}
			}
		}
	}
}