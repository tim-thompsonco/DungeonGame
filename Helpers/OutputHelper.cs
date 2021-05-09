using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Quests;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Helpers {
	public static class OutputHelper {
		public static UserOutput Display = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput QuestDisplay = new UserOutput();
		private static readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;

		private static UserOutput BuildMap(Player player, int height, int width) {
			UserOutput output = new UserOutput();
			// Draw top border of map
			StringBuilder mapBorder = new StringBuilder();
			// Minimap border needs to extend same width as minimap itself in either direction
			for (int b = 0; b < Settings.GetMiniMapBorderWidth(); b++) {
				mapBorder.Append("=");
			}
			output.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				mapBorder.ToString());
			// Get player coordinates and room player is located in
			int playerX = player.PlayerLocation.X;
			int playerY = player.PlayerLocation.Y;
			int playerZ = player.PlayerLocation.Z;
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */
			for (int i = playerY + height; i > playerY - height; i--) {
				List<string> sameLineOutput = new List<string>();
				int startLeftPos = playerX - width;
				int endRightPos = playerX + width - 1;
				for (int j = startLeftPos; j <= endRightPos; j++) {
					int mapX = j;
					int mapY = i;
					int mapZ = playerZ;
					Coordinate findCoord = new Coordinate(mapX, mapY, mapZ);
					if (RoomHelper.Rooms.ContainsKey(findCoord)) {
						IRoom room = RoomHelper.Rooms[findCoord];
						if (room.IsDiscovered) {
							if (j == startLeftPos) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								} else {
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							} else if (j == endRightPos) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								} else {
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								}
							} else if (mapX == playerX && mapY == playerY && mapZ == playerZ) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								} else {
									sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							} else {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								} else {
									sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
						} else {
							if (j == startLeftPos) {
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
							} else if (j == endRightPos) {
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
							} else {
								sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetUndiscoveredMapTileSizeTwo()); // What prints to display
							}
						}
					} else {
						if (j == startLeftPos) {
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetLeftMapBorderSizeFour()); // What prints to display
						} else if (j == endRightPos) {
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetRightMapBorderSizeFour()); // What prints to display
						} else {
							sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetUndiscoveredMapTileSizeTwo()); // What prints to display
						}
					}
				}
				output.StoreUserOutput(sameLineOutput);
			}
			output.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				mapBorder.ToString());
			return output;
		}
		public static UserOutput ShowEffects(Player player) {
			UserOutput effectUserOutput = new UserOutput();
			UserOutput badEffectUserOutput = new UserOutput();
			UserOutput goodEffectUserOutput = new UserOutput();
			// Draw title to show for player effects
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Player _Effects:");
			int activeEffects = 0;
			foreach (IEffect effect in player.Effects) {
				string effectOutput = player.InCombat
					? effect.MaxRound + 1 - effect.CurrentRound > 1
						? $"({effect.MaxRound + 1 - effect.CurrentRound} rounds) {_textInfo.ToTitleCase(effect.Name)}"
						: $"({effect.MaxRound + 1 - effect.CurrentRound} round) {_textInfo.ToTitleCase(effect.Name)}"
					: effect.MaxRound + 1 - effect.CurrentRound > 1
						? $"({(effect.MaxRound + 1 - effect.CurrentRound) * effect.TickDuration} seconds) {_textInfo.ToTitleCase(effect.Name)}"
						: $"({(effect.MaxRound + 1 - effect.CurrentRound) * effect.TickDuration} second) {_textInfo.ToTitleCase(effect.Name)}";
				activeEffects++;
				if (effect.IsHarmful) {
					badEffectUserOutput.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectOutput);
				} else {
					goodEffectUserOutput.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						effectOutput);
				}
			}
			if (activeEffects == 0) {
				effectUserOutput.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"None.");
			}
			goodEffectUserOutput.Output.OrderBy(output => output[2]);
			foreach (List<string> output in goodEffectUserOutput.Output) {
				effectUserOutput.Output.Add(output);
			}
			badEffectUserOutput.Output.OrderBy(output => output[2]);
			foreach (List<string> output in badEffectUserOutput.Output) {
				effectUserOutput.Output.Add(output);
			}
			// Create bottom border for effects area
			StringBuilder effectsBorder = new StringBuilder();
			// _Effects border needs to extend same width as minimap itself in either direction
			for (int b = 0; b < Settings.GetMiniMapBorderWidth(); b++) {
				effectsBorder.Append("=");
			}
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				effectsBorder.ToString());
			return effectUserOutput;
		}
		public static UserOutput ShowQuests(Player player) {
			UserOutput questUserOutput = new UserOutput();
			// Draw title to show for player quest log
			questUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Player Quests:");
			int quests = 0;
			foreach (Quest quest in player.QuestLog) {
				string questOutput = quest.QuestCategory switch {
					QuestType.KillCount => $"{_textInfo.ToTitleCase(quest.Name)} ({quest.CurrentKills}/{quest.RequiredKills} monsters)",
					QuestType.KillMonster => $"{_textInfo.ToTitleCase(quest.Name)} ({quest.CurrentKills}/{quest.RequiredKills} {quest.MonsterKillType}s)",
					QuestType.ClearLevel => $"{_textInfo.ToTitleCase(quest.Name)} (Lvl: {quest.TargetLevel} | {quest.MonstersRemaining} monsters left)",
					_ => throw new ArgumentOutOfRangeException()
				};
				StringBuilder questStringBuilder = new StringBuilder(questOutput);
				if (quest.QuestCompleted) {
					questStringBuilder.Append(" (Complete)");
				}

				quests++;
				questUserOutput.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					questStringBuilder.ToString());
			}
			if (quests == 0) {
				questUserOutput.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"None.");
			}
			// Create bottom border for player quest log
			StringBuilder questsBorder = new StringBuilder();
			// Quest log border needs to extend same width as minimap itself in either direction
			for (int b = 0; b < Settings.GetMiniMapBorderWidth(); b++) {
				questsBorder.Append("=");
			}
			questUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questsBorder.ToString());
			return questUserOutput;
		}

		public static void ShowUserOutput(Player player, Monster opponent) {
			PlayerHelper.DisplayPlayerStats(player);
			MonsterHelper.DisplayStats(opponent);
			RoomHelper.Rooms[player.PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			QuestDisplay = ShowQuests(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}

		public static void ShowUserOutput(Player player) {
			PlayerHelper.DisplayPlayerStats(player);
			RoomHelper.Rooms[player.PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			QuestDisplay = ShowQuests(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}

		public static void StoreSuccessMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				message);
		}

		public static void StoreFailureMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				message);
		}

		public static void StoreOnFireMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				message);
		}

		public static void StoreAttackSuccessMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				message);
		}

		public static void StoreAttackFailMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatAttackFailText(),
				Settings.FormatDefaultBackground(),
				message);
		}

		public static void StoreAnnounceMessage(string message) {
			Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(),
				message);
		}
	}
}