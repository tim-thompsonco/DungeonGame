namespace DungeonGame {
	public class Item : IEquipment {
		public string Name { get; }
		public bool Equipped { get; set; }
		public int ItemValue { get; }

		public Item(string name, int itemValue) {
			this.Name = name;
			this.ItemValue = itemValue;
		}

		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}