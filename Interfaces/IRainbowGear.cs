namespace DungeonGame {
	public interface IRainbowGear {
		bool IsRainbowGear { get; set; }

		void UpdateRainbowStats(Player player);
	}
}