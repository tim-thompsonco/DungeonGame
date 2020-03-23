using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DungeonGame {
	public static class OutputHandler {
		public static UserOutput Display = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();

		private static UserOutput BuildMap(Player player, int height, int width) {
			var output = new UserOutput();
			// Draw top border of map
			var mapBorder = new StringBuilder();
			// Minimap border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Settings.GetMiniMapBorderWidth(); b++) {
				mapBorder.Append("=");
			}
			output.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				mapBorder.ToString());
			// Get player coordinates and room player is located in
			var playerX = player.PlayerLocation.X;
			var playerY = player.PlayerLocation.Y;
			var playerZ = player.PlayerLocation.Z;
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */
			for (var i = playerY + height; i > playerY - height; i--) {
				var sameLineOutput = new List<string>();
				var startLeftPos = playerX - width;
				var endRightPos = playerX + width - 1;
				for (var j = startLeftPos; j <= endRightPos; j ++) {
					var mapX = j;
					var mapY = i;
					var mapZ = playerZ;
					var findCoord = new Coordinate(mapX, mapY, mapZ);
					if (RoomHandler.Rooms.ContainsKey(findCoord)) {
						var room = RoomHandler.Rooms[findCoord];
						if (room.IsDiscovered) {
							if (j == startLeftPos) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else {
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
							else if (j == endRightPos) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								}
								else {
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
									sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
									sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
									sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
								}
							}
							else if (mapX == playerX && mapY == playerY && mapZ == playerZ) {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else {
									sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
									sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
							else {
								if (room.Up != null || room.Down != null) {
									sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetUpDownMapTile()); // What prints to display
								}
								else {
									sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
									sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
									sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								}
							}
						}
						else {
							if (j == startLeftPos) {
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetLeftMapBorderSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
							}
							else if (j == endRightPos) {
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetEmptyMapTileSizeTwo()); // What prints to display
								sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetRightMapBorderSizeTwo()); // What prints to display
							}
							else {
								sameLineOutput.Add(Settings.GetTileColor(player)); // Foreground color
								sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
								sameLineOutput.Add(Settings.GetUndiscoveredMapTileSizeTwo()); // What prints to display
							}
						}
					}
					else {
						if (j == startLeftPos) {
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetLeftMapBorderSizeFour()); // What prints to display
						}
						else if (j == endRightPos) {
							sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
							sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
							sameLineOutput.Add(Settings.GetRightMapBorderSizeFour()); // What prints to display
						}
						else {
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
			var effectUserOutput = new UserOutput();
			var badEffectUserOutput = new UserOutput();
			var goodEffectUserOutput = new UserOutput();
			// Draw title to show for player effects
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				"Player Effects:");
			var activeEffects = 0;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var effect in player.Effects) {
				string effectOutput;
				if (player.InCombat) {
					if (effect.EffectMaxRound + 1 - effect.EffectCurRound > 1) {
						effectOutput = "(" + (effect.EffectMaxRound + 1 - effect.EffectCurRound) + " rounds) " + 
						               textInfo.ToTitleCase(effect.Name);
					}
					else {
						effectOutput = "(" + (effect.EffectMaxRound + 1 - effect.EffectCurRound) + " round) " + 
						               textInfo.ToTitleCase(effect.Name);
					}
				}
				else {
					if (effect.EffectMaxRound + 1 - effect.EffectCurRound > 1) {
						effectOutput = "(" + (effect.EffectMaxRound + 1 - effect.EffectCurRound) * effect.TickDuration + 
						               " seconds) " + textInfo.ToTitleCase(effect.Name);
					}
					else {
						effectOutput = "(" + (effect.EffectMaxRound + 1 - effect.EffectCurRound) * effect.TickDuration + 
						               " second) " + textInfo.ToTitleCase(effect.Name);
					}
				}
				activeEffects++;
				if (effect.IsHarmful) {
					badEffectUserOutput.StoreUserOutput(
						Settings.FormatAttackFailText(), 
						Settings.FormatDefaultBackground(), 
						effectOutput);
				}
				else {
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
			foreach (var output in goodEffectUserOutput.Output) {
				effectUserOutput.Output.Add(output);
			}
			badEffectUserOutput.Output.OrderBy(output => output[2]);
			foreach (var output in badEffectUserOutput.Output) {
				effectUserOutput.Output.Add(output);
			}
			// Create bottom border for effects area
			var effectsBorder = new StringBuilder();
			// Effects border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Settings.GetMiniMapBorderWidth(); b++) {
				effectsBorder.Append("=");
			}
			effectUserOutput.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				effectsBorder.ToString());
			return effectUserOutput;
		}
		public static void ShowUserOutput(Player player, Monster opponent) {
			PlayerHandler.DisplayPlayerStats(player);
			MonsterHandler.DisplayStats(opponent);
			RoomHandler.Rooms[player.PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}
		public static void ShowUserOutput(Player player) {
			PlayerHandler.DisplayPlayerStats(player);
			RoomHandler.Rooms[player.PlayerLocation].ShowCommands();
			MapDisplay = BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
		}
	}
}