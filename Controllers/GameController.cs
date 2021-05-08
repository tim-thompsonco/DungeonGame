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

namespace DungeonGame.Controllers {
	public static class GameController {
		private static readonly Random _rndGenerate = new Random();
		public static bool IsGameOver { get; set; }
		private static int GameTicks { get; set; }

		public static void CheckStatus(Player player) {
			GameTicks++;

			if (GameTicks % player.StatReplenishInterval == 0) {
				ReplenishStatsOverTime(player);
			}

			if (player.Effects.Any()) {
				foreach (IEffect effect in player.Effects.Where(effect => GameTicks % effect.TickDuration == 0).ToList()) {
					if (effect is HealingEffect healingEffect) {
						healingEffect.ProcessHealingRound(player);
					}

					if (effect is ChangePlayerDamageEffect changePlayerDmgEffect && !player.InCombat && effect.Name.Contains("berserk")) {
						changePlayerDmgEffect.ProcessChangePlayerDamageRound(player);
					}

					if (effect is ChangeArmorEffect changeArmorEffect) {
						if (!player.InCombat && effect.Name.Contains("berserk")) {
							changeArmorEffect.IsEffectExpired = true;
						}

						if (!player.InCombat) {
							changeArmorEffect.ProcessChangeArmorRound();
						}
					}

					if (effect is BurningEffect burningEffect) {
						burningEffect.ProcessBurningRound(player);
					}

					if (effect is BleedingEffect bleedingEffect) {
						bleedingEffect.ProcessRound();
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
						if (!player.InCombat) {
							effect.IsEffectExpired = true;
						}

						changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
					}

					if (effect is BlockDamageEffect blockDamageEffect && !player.InCombat) {
						blockDamageEffect.ProcessBlockDamageRound();
					}
				}
			}

			foreach (IRoom room in RoomController._Rooms.Values) {
				foreach (IName roomObject in room._RoomObjects.Where(
					roomObject => roomObject?.GetType() == typeof(Monster))) {
					Monster monster = (Monster)roomObject;
					RemovedExpiredEffectsAsync(monster);
					if (GameTicks % monster.StatReplenishInterval == 0 && monster.HitPoints > 0) {
						ReplenishStatsOverTime(monster);
					}

					if (!monster.Effects.Any()) {
						continue;
					}

					foreach (IEffect effect in monster.Effects.Where(effect => GameTicks % effect.TickDuration == 0).ToList()) {
						if (effect is BurningEffect burningEffect) {
							burningEffect.ProcessBurningRound(monster);
						} else if (effect is FrozenEffect frozenEffect) {
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
				case Player.PlayerClassType.Mage:
					if (player.ManaPoints < player.MaxManaPoints) {
						player.ManaPoints++;
					}

					break;
				case Player.PlayerClassType.Warrior:
					if (player.RagePoints < player.MaxRagePoints) {
						player.RagePoints++;
					}

					break;
				case Player.PlayerClassType.Archer:
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