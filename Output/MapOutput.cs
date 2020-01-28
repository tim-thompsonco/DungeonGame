using System.Collections.Generic;
using System.Text;

namespace DungeonGame {
	public static class MapOutput {
		public static UserOutput BuildMap(Player player, int height, int width) {
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
			/* Map starts drawing from top left, so it needs to decrement since
			each new console writeline pushes screen down instead of up */
			for (var i = player.Y + height; i > player.Y - height; i--) {
				var sameLineOutput = new List<string>();
				var startLeftPos = player.X - width;
				var endRightPos = player.X + width - 1;
				for (var j = startLeftPos; j <= endRightPos; j ++) {
					var mapX = j;
					var mapY = i;
					var mapZ = player.Z;
					var roomName = Helper.Rooms.Find(f => f.X == mapX && f.Y == mapY && f.Z == mapZ);
					var roomIndex = Helper.Rooms.IndexOf(roomName);
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == endRightPos) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 &&
					    mapX == player.X &&
					    mapY == player.Y &&
					    mapZ == player.Z &&
					    j == startLeftPos) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatHiddenOutputText()); // Foreground color
						sameLineOutput.Add(Settings.FormatPlayerTile()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && Helper.Rooms[roomIndex].IsDiscovered) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("  "); // What prints to display
						continue;
					}
					if (roomIndex != -1 && Helper.Rooms[roomIndex].IsDiscovered && j == endRightPos) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("OO |"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
						continue;
					}
					if (roomIndex != -1 && Helper.Rooms[roomIndex].IsDiscovered && j == startLeftPos) {
						if (Helper.Rooms[roomIndex].GoUp || Helper.Rooms[roomIndex].GoDown) {
							sameLineOutput.Add(Settings.FormatUpDownIndicator()); // Foreground color
							sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
							sameLineOutput.Add("| OO"); // What prints to display
							continue;
						}
						sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(Settings.FormatDiscoveredTile()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
						continue;
					}
					if (j == endRightPos) {
						sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
						sameLineOutput.Add("   |"); // What prints to display
					}
					if (j == startLeftPos) {
						sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
						sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
						sameLineOutput.Add("|   "); // What prints to display
					}
					sameLineOutput.Add(Settings.FormatGeneralInfoText()); // Foreground color
					sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
					sameLineOutput.Add("  "); // What prints to display
				}
				output.StoreUserOutput(sameLineOutput);
			}
			output.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				mapBorder.ToString());
			return output;
		}
	}
}