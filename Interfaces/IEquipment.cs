namespace DungeonGame {
	public interface IEquipment : IRoomInteraction {
		bool Equipped { get; set; }
		int ItemValue { get; set; }
		int Weight { get; set; }
	}
}