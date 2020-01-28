namespace DungeonGame {
	public class Loot : IEquipment {
		public string Name { get; set; }
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }

		public Loot(string name, int level, int weight) {
			this.Name = name;
			this.Weight = weight;
			this.ItemValue = 15 + ((level - 1) * 5);
		}
	}
}