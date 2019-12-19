namespace DungeonGame {
	public interface IEquipment : IRoomInteraction {
		bool Equipped { get; set; }
		int ItemValue { get; set; }
		string Name { get; set; }
		bool IsEquipped();
	}
}