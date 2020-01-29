namespace DungeonGame {
	public static class OutputHandler {
		public static UserOutput Display = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();
		
		public static void ShowUserOutput(Player player, Monster opponent) {
			PlayerHandler.DisplayPlayerStats(player);
			opponent.DisplayStats();
			RoomHandler.Rooms[RoomHandler.RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
		public static void ShowUserOutput(Player player) {
			PlayerHandler.DisplayPlayerStats(player);
			RoomHandler.Rooms[RoomHandler.RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
	}
}