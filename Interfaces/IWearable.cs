namespace DungeonGame {
	public interface IEquipment : IRoomInteraction {
		bool Equipped { get; set; }
		bool IsEquipped();
	}
}