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

namespace DungeonGame.Controllers {
	public static class GameController {
		private static readonly Random RndGenerate = new Random();
		public static bool _IsGameOver { get; set; }
		private static int _GameTicks { get; set; }

		public static void CheckStatus(Player player) {
			_GameTicks++;

			if (_GameTicks % player._StatReplenishInterval == 0) {
				ReplenishStatsOverTime(player);
			}

			if (player._Effects.Any()) {
				foreach (IEffect effect in player._Effects.Where(effect => _GameTicks % effect.TickDuration == 0).ToList()) {
					if (effect is HealingEffect healingEffect) {
						healingEffect.ProcessHealingRound(player);
					}

					if (effect is ChangePlayerDamageEffect changePlayerDmgEffect && !player._InCombat && effect.Name.Contains("berserk")) {
						changePlayerDmgEffect.ProcessChangePlayerDamageRound(player);
					}

					if (effect is ChangeArmorEffect changeArmorEffect) {
						if (!player._InCombat && effect.Name.Contains("berserk")) {
							changeArmorEffect.IsEffectExpired = true;
						}

						if (!player._InCombat) {
							changeArmorEffect.ProcessChangeArmorRound();
						}
					}

					if (effect is BurningEffect burningEffect) {
						burningEffect.ProcessBurningRound(player);
					}

					if (effect is BleedingEffect bleedingEffect) {
						bleedingEffect.ProcessBleedingRound(player);
					}

					if (effect is FrozenEffect frozenEffect) {
						frozenEffect.ProcessFrozenRound();
					}

					if (effect is ReflectDamageEffect reflectDmgEffect) {
						reflectDmgEffect.ProcessReflectDamageRound();
					}

					if (effect is ChangeStatEffect changeStatEffect) {
						changeStatEffect.ProcessChangeStatRound(player);
					}

					if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
						if (!player._InCombat) {
							effect.IsEffectExpired = true;
						}

						changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
					}

					if (effect is BlockDamageEffect blockDamageEffect && !player._InCombat) {
						blockDamageEffect.ProcessBlockDamageRound();
					}
				}
			}

			foreach (IRoom room in RoomController._Rooms.Values) {
				foreach (IName roomObject in room._RoomObjects.Where(
					roomObject => roomObject?.GetType() == typeof(Monster))) {
					Monster monster = (Monster)roomObject;
					RemovedExpiredEffectsAsync(monster);
					if (_GameTicks % monster._StatReplenishInterval == 0 && monster._HitPoints > 0) {
						ReplenishStatsOverTime(monster);
					}

					if (!monster._Effects.Any()) {
						continue;
					}

					foreach (IEffect effect in monster._Effects.Where(effect => _GameTicks % effect.TickDuration == 0).ToList()) {
						if (effect is BurningEffect burningEffect) {
							burningEffect.ProcessBurningRound(monster);
						}

						if (effect is BleedingEffect bleedingEffect) {
							bleedingEffect.ProcessBleedingRound(monster);
						}

						if (effect is StunnedEffect stunnedEffect) {
							stunnedEffect.ProcessStunnedRound(monster);
						}

						if (effect is FrozenEffect frozenEffect) {
							frozenEffect.ProcessFrozenRound(monster);
						}
					}
				}
			}
		}

		public static int GetRandomNumber(int lowNum, int highNum) {
			return RndGenerate.Next(lowNum, highNum + 1);
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
				foreach (IEffect effect in player._Effects.ToList()) {
					if (effect.IsEffectExpired) {
						player._Effects.Remove(effect);
					}
				}
			});
		}

		public static async void RemovedExpiredEffectsAsync(Monster monster) {
			await Task.Run(() => {
				foreach (IEffect effect in monster._Effects.ToList()) {
					if (effect is StunnedEffect && effect.IsEffectExpired) {
						monster._IsStunned = false;
					}

					if (effect.IsEffectExpired) {
						monster._Effects.Remove(effect);
					}
				}
			});
		}

		private static void ReplenishStatsOverTime(Player player) {
			if (player._InCombat) {
				return;
			}

			if (player._HitPoints < player._MaxHitPoints) {
				player._HitPoints++;
			}

			switch (player._PlayerClass) {
				case Player.PlayerClassType.Mage:
					if (player._ManaPoints < player._MaxManaPoints) {
						player._ManaPoints++;
					}

					break;
				case Player.PlayerClassType.Warrior:
					if (player._RagePoints < player._MaxRagePoints) {
						player._RagePoints++;
					}

					break;
				case Player.PlayerClassType.Archer:
					if (player._ComboPoints < player._MaxComboPoints) {
						player._ComboPoints++;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void ReplenishStatsOverTime(Monster monster) {
			if (monster._InCombat) {
				return;
			}

			if (monster._HitPoints < monster._MaxHitPoints) {
				monster._HitPoints++;
			}

			if (monster._EnergyPoints < monster._MaxEnergyPoints) {
				monster._EnergyPoints++;
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
				GearController.EquipInitialGear(player);
				/* Set initial room condition for player
				Begin game by putting player at coords 0, 7, 0, town entrance */
				RoomController.SetPlayerLocation(player, 0, 4, 0);
			}
			return player;
		}

		public static void LoadGame() {
			try {
				JsonSerializerSettings serializerSettings = new JsonSerializerSettings() {
					TypeNameHandling = TypeNameHandling.Auto,
					NullValueHandling = NullValueHandling.Ignore
				};
				RoomController._Rooms = JsonConvert.DeserializeObject<Dictionary<Coordinate, IRoom>>(File.ReadAllText(
					"gamesave.json"), serializerSettings);
				// Insert blank space before game reload info for formatting
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"Reloading your saved game.");
				// Insert blank space after game reload info for formatting
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"");
			} catch (FileNotFoundException) {
				// Create new dungeon
				RoomController._Rooms = new RoomBuilder(200, 10, 0, 4, 0).RetrieveSpawnRooms();
			}
		}

		public static bool QuitGame(Player player) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Are you sure you want to quit?");
			OutputController.ShowUserOutput(player);
			OutputController.Display.ClearUserOutput();
			string[] input = InputController.GetFormattedInput(Console.ReadLine());
			Console.Clear();
			if (input[0] != "yes" && input[0] != "y") {
				return false;
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Quitting the game.");
			player._CanSave = true;
			_IsGameOver = true;
			SaveGame(player);
			return true;
		}

		public static void SaveGame(Player player) {
			string outputString;
			if (player._CanSave) {
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
					serializerRooms.Serialize(writer, RoomController._Rooms, typeof(Dictionary<Coordinate, IRoom>));
				}
				outputString = "Your game has been saved.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
		}

		public static bool ContinuePlaying() {
			while (true) {
				const string continuePlayingMessage = "Do you want to keep playing? [Y/N]";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					continuePlayingMessage);
				OutputController.Display.RetrieveUserOutput();
				OutputController.Display.ClearUserOutput();
				string[] input = InputController.GetFormattedInput(Console.ReadLine());
				OutputController.Display.RetrieveUserOutput();
				OutputController.Display.ClearUserOutput();
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