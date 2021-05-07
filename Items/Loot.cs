namespace DungeonGame.Items {
	public class Loot : IItem {
		public string Name { get; set; }
		public string Desc { get; set; }
		public bool _Equipped { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }

		public Loot(string name, int level, int weight) {
			Name = name;
			Weight = weight;
			// Item value starts at 15 for level 1 then scales up an additional 5 per level
			ItemValue = 15 + ((level - 1) * 5);
			Desc = $"A {Name} that is worth some money to the right vendor.";
		}
	}
}