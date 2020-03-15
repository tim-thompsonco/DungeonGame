﻿using System;

namespace DungeonGame {
	public static class Settings {
		public static int GetGameWidth() {
			return 100 - GetBufferGap();
		}
		public static int GetMiniMapHeight() {
			return 5;
		}
		public static int GetMiniMapWidth() {
			return 10;
		}
		public static int GetBufferGap() {
			return 5;
		}
		public static int GetMiniMapBorderWidth() {
			return GetMiniMapWidth() * 4 + 4;
		}
		public static string GetTileColor() {
			var room = RoomHandler.Rooms[RoomHandler.RoomIndex];
			if (room is TownRoom) return "darkgreen";
			var dungeonRoom = room as DungeonRoom;
			if (dungeonRoom.DungeonLevel <= 3) return "gray";
			return dungeonRoom.DungeonLevel <= 7 ? "darkgray" : "darkred";
		}
		public static string GetEmptyMapTileSizeTwo() {
			return "  ";
		}
		public static string GetUndiscoveredMapTileSizeTwo() {
			return "\"\"";
		}
		public static string GetUpDownMapTile() {
			return "OO";
		}
		public static string GetLeftMapBorderSizeTwo() {
			return "| ";
		}
		public static string GetRightMapBorderSizeTwo() {
			return " |";
		}
		public static string GetLeftMapBorderSizeFour() {
			return "|   ";
		}
		public static string GetRightMapBorderSizeFour() {
			return "   |";
		}
		public static string FormatDefaultBackground() {
			return "black";
		}
		public static string FormatHealthBackground() {
			return "darkred";
		}
		public static string FormatManaBackground() {
			return "darkblue";
		}
		public static string FormatRageBackground() {
			return "darkyellow";
		}
		public static string FormatComboBackground() {
			return "darkyellow";
		}
		public static string FormatExpBackground() {
			return "darkcyan";
		}
		public static string FormatSuccessOutputText() {
			return "green";
		}
		public static string FormatHiddenOutputText() {
			return "black";
		}
		public static string FormatFailureOutputText() {
			return "darkcyan";
		}
		public static string FormatRoomOutputText() {
			return "darkcyan";
		}
		public static string FormatOnFireText() {
			return "yellow";
		}
		public static string FormatAttackSuccessText() {
			return "red";
		}
		public static string FormatAttackFailText() {
			return "darkred";
		}
		public static string FormatInfoText() {
			return "white";
		}
		public static string FormatLevelUpText() {
			return "cyan";
		}
		public static string FormatGeneralInfoText() {
			return "darkgreen";
		}
		public static string FormatUpDownIndicator() {
			return "black";
		}
		public static string FormatAnnounceText() {
			return "gray";
		}
		public static string FormatPlayerTile() {
			return "green";
		}
		public static string FormatDiscoveredTile() {
			return "darkgray";
		}
		public static string FormatTextBorder() {
			return "========================================================";
		}
		public static int GetBaseExperienceToLevel() {
			return 500;
		}
	}
}