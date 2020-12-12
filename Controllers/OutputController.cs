using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Controllers
{
	public static class OutputController
	{
		public static UserOutput Display = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();
		public static UserOutput QuestDisplay = new UserOutput();

		private static UserOutput BuildMap(Player player, int height, int width)
		{
			var output = new UserOutput();
			// Draw top border of map
			var mapBorder = new StringBuilder();
			// Minimap border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Settings.GetMiniMapBorderWidth(); b++)
			{
				mapBorder.Append("=");
			}
			output.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				mapBorder.ToString());
			// Get player coordinates and room player is located in
			var playerX = player._PlayerLocation._X;
			var playerY = player._PlayerLocation._Y;
			var playerZ = player._PlayerLocation._Z;
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */
			for (var i = playerY + height; i > playerY - height; i--)
			{
				var sameLineOutput = new List<string>();
				var startLeftPos = playerX - width;
				var endRightPos = playerX + width - 1;
				for (var j = startLeftPos; j <= endRightPos; j++)
				{
					var mapX = j;
					var mapY = i;
					var mapZ = playerZ;
					var findCoord = new Coordinate(mapX, mapY, mapZ);
					if (RoomController.Rooms.ContainsKey(findCoord))
					{
						var room = RoomController.Rooms[findCoord];
						if (room._IsDiscovered)
						{
							if (j == startLeftPos)
							{
								if (room._Up != null || room._Down != null)
								{
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else
								{
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
							else if (j == endRightPos)
							{
								if (room._Up != null || room._Down != null)
								{
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								}
								else
								{
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								}
							}
							else if (mapX == playerX && mapY == playerY && mapZ == playerZ)
							{
								if (room._Up != null || room._Down != null)
								{
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else
								{
									sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
							else
							{
								if (room._Up != null || room._Down != null)
								{
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else
								{
									sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
						}
						else
						{
							if (j == startLeftPos)
							{
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
							}
							else if (j == endRightPos)
							{
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
							}
							else
							{
								sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetUndiscoveredMapTileSizeTwo()); // What prints to display
							}
						}
					}
					else
					{
						if (j == startLeftPos)
						{
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetLeftMapBorderSizeFour()); // What prints to display
						}
						else if (j == endRightPos)
						{
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetRightMapBorderSizeFour()); // What prints to display
						}
						else
						{
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
		public static UserOutput ShowEffects(Player player)
		{
			var effectUserOutput = new UserOutput();
			var badEffectUserOutput = new UserOutput();
			var goodEffectUserOutput = new UserOutput();
			// Draw title to show for player effects
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Player _Effects:");
			var activeEffects = 0;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var effect in player._Effects)
			{
				string effectOutput;
				if (player._InCombat)
				{
					if (effect._EffectMaxRound + 1 - effect._EffectCurRound > 1)
					{
						effectOutput = "(" + (effect._EffectMaxRound + 1 - effect._EffectCurRound) + " rounds) " +
									   textInfo.ToTitleCase(effect._Name);
					}
					else
					{
						effectOutput = "(" + (effect._EffectMaxRound + 1 - effect._EffectCurRound) + " round) " +
									   textInfo.ToTitleCase(effect._Name);
					}
				}
				else
				{
					if (effect._EffectMaxRound + 1 - effect._EffectCurRound > 1)
					{
						effectOutput = "(" + (effect._EffectMaxRound + 1 - effect._EffectCurRound) * effect._TickDuration +
									   " seconds) " + textInfo.ToTitleCase(effect._Name);
					}
					else
					{
						effectOutput = "(" + (effect._EffectMaxRound + 1 - effect._EffectCurRound) * effect._TickDuration +
									   " second) " + textInfo.ToTitleCase(effect._Name);
					}
				}
				activeEffects++;
				if (effect._IsHarmful)
				{
					badEffectUserOutput.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectOutput);
				}
				else
				{
					goodEffectUserOutput.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						effectOutput);
				}
			}
			if (activeEffects == 0)
			{
				effectUserOutput.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"None.");
			}
			goodEffectUserOutput.Output.OrderBy(output => output[2]);
			foreach (var output in goodEffectUserOutput.Output)
			{
				effectUserOutput.Output.Add(output);
			}
			badEffectUserOutput.Output.OrderBy(output => output[2]);
			foreach (var output in badEffectUserOutput.Output)
			{
				effectUserOutput.Output.Add(output);
			}
			// Create bottom border for effects area
			var effectsBorder = new StringBuilder();
			// _Effects border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Settings.GetMiniMapBorderWidth(); b++)
			{
				effectsBorder.Append("=");
			}
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				effectsBorder.ToString());
			return effectUserOutput;
		}
		public static UserOutput ShowQuests(Player player)
		{
			var questUserOutput = new UserOutput();
			// Draw title to show for player quest log
			questUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Player Quests:");
			var quests = 0;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var quest in player._QuestLog)
			{
				var questOutput = quest._QuestCategory switch
				{
					Quest.QuestType.KillCount => (textInfo.ToTitleCase(quest._Name) + " (" + quest._CurrentKills + "/" +
												  quest._RequiredKills + " monsters)"),
					Quest.QuestType.KillMonster => (textInfo.ToTitleCase(quest._Name) + " (" + quest._CurrentKills + "/" +
													quest._RequiredKills + " " + quest._MonsterKillType + "s)"),
					Quest.QuestType.ClearLevel => (textInfo.ToTitleCase(quest._Name) + " (Lvl: " + quest._TargetLevel + " | " +
												   quest._MonstersRemaining + " monsters left)"),
					_ => throw new ArgumentOutOfRangeException()
				};
				var questStringBuilder = new StringBuilder(questOutput);
				if (quest._QuestCompleted) questStringBuilder.Append(" (Complete)");
				quests++;
				questUserOutput.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					questStringBuilder.ToString());
			}
			if (quests == 0)
			{
				questUserOutput.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"None.");
			}
			// Create bottom border for player quest log
			var questsBorder = new StringBuilder();
			// Quest log border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Settings.GetMiniMapBorderWidth(); b++)
			{
				questsBorder.Append("=");
			}
			questUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questsBorder.ToString());
			return questUserOutput;
		}
		public static void ShowUserOutput(Player player, Monster opponent)
		{
			PlayerController.DisplayPlayerStats(player);
			MonsterController.DisplayStats(opponent);
			RoomController.Rooms[player._PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			QuestDisplay = ShowQuests(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}
		public static void ShowUserOutput(Player player)
		{
			PlayerController.DisplayPlayerStats(player);
			RoomController.Rooms[player._PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			QuestDisplay = ShowQuests(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}
	}
}