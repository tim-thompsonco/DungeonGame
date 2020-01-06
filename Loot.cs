namespace DungeonGame {
	public class Loot : IEquipment {
		public string Name { get; set; }
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }

		public Loot(string name, int level) {
			this.Name = name;
			this.ItemValue = 15 + ((level - 1) * 5);
		}

		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}