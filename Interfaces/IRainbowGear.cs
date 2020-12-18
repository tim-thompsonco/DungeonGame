using DungeonGame.Players;

namespace DungeonGame
{
	public interface IRainbowGear
	{
		bool _IsRainbowGear { get; set; }

		void UpdateRainbowStats(Player player);
	}
}