namespace DungeonGame
{
	public static class Settings
	{
		public static int GetGameWidth()
		{
			return 100 - GetBufferGap();
		}
		public static int GetMiniMapHeight()
		{
			return 5;
		}
		public static int GetMiniMapWidth()
		{
			return 10;
		}
		public static int GetBufferGap()
		{
			return 5;
		}
		public static int GetMiniMapBorderWidth()
		{
			return (GetMiniMapWidth() * 4) + 4;
		}
		public static string GetTileColor(Player player)
		{
			// Tile color depends on how deep in the dungeon the player is
			// The Z coordinate for player location is up/down, so it shows the dungeon level
			// Levels are depicted as negative numbers, so -8 is 8 levels deep into the dungeon, -9 is 9 levels deep, etc.

			if (player._PlayerLocation._Z <= -8)
			{
				return "darkred";
			}

			if (player._PlayerLocation._Z < -3)
			{
				return "darkgray";
			}

			return "gray";
		}
		public static string GetEmptyMapTileSizeTwo()
		{
			return "  ";
		}
		public static string GetUndiscoveredMapTileSizeTwo()
		{
			return "\"\"";
		}
		public static string GetUpDownMapTile()
		{
			return "OO";
		}
		public static string GetLeftMapBorderSizeTwo()
		{
			return "| ";
		}
		public static string GetRightMapBorderSizeTwo()
		{
			return " |";
		}
		public static string GetLeftMapBorderSizeFour()
		{
			return "|   ";
		}
		public static string GetRightMapBorderSizeFour()
		{
			return "   |";
		}
		public static string FormatDefaultBackground()
		{
			return "black";
		}
		public static string FormatHealthBackground()
		{
			return "darkred";
		}
		public static string FormatManaBackground()
		{
			return "darkblue";
		}
		public static string FormatRageBackground()
		{
			return "darkyellow";
		}
		public static string FormatComboBackground()
		{
			return "darkyellow";
		}
		public static string FormatExpBackground()
		{
			return "darkcyan";
		}
		public static string FormatSuccessOutputText()
		{
			return "green";
		}
		public static string FormatHiddenOutputText()
		{
			return "black";
		}
		public static string FormatFailureOutputText()
		{
			return "darkcyan";
		}
		public static string FormatRoomOutputText()
		{
			return "darkcyan";
		}
		public static string FormatOnFireText()
		{
			return "yellow";
		}
		public static string FormatAttackSuccessText()
		{
			return "red";
		}
		public static string FormatAttackFailText()
		{
			return "darkred";
		}
		public static string FormatInfoText()
		{
			return "white";
		}
		public static string FormatLevelUpText()
		{
			return "cyan";
		}
		public static string FormatGeneralInfoText()
		{
			return "darkgreen";
		}
		public static string FormatUpDownIndicator()
		{
			return "black";
		}
		public static string FormatAnnounceText()
		{
			return "gray";
		}
		public static string FormatPlayerTile()
		{
			return "green";
		}
		public static string FormatDiscoveredTile()
		{
			return "darkgray";
		}
		public static string FormatTextBorder()
		{
			return "========================================================";
		}
		public static int GetBaseExperienceToLevel()
		{
			return 500;
		}
	}
}