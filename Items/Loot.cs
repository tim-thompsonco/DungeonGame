namespace DungeonGame {
	public class Loot : IEquipment {
		public enum GemType {
			Ruby,
			Emerald,
			Diamond,
			Sapphire,
			Amethyst,
			Topaz
		}
		public enum GemLevel {
			Chipped,
			Dull,
			Normal
		}
		public string Name { get; set; }
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public GemType GemCategory { get; set; }
		public GemLevel GemStrength { get; set; }

		public Loot(string name, int level, int weight) {
			this.Name = name;
			this.Weight = weight;
			this.ItemValue = 15 + (level - 1) * 5;
		}
		public Loot(int level, GemType gemType) {
			this.GemCategory = gemType;
			this.Weight = 1;
			this.ItemValue = level * 20;
			string name;
			if (level <= 3) {
				this.GemStrength = GemLevel.Chipped;
				name = GemLevel.Chipped + " " + gemType;
			}
			else if (level <= 6) {
				this.GemStrength = GemLevel.Dull;
				name = GemLevel.Dull + " " + gemType;
			}
			else {
				this.GemStrength = GemLevel.Normal;
				name = gemType.ToString();
			}
			this.Name = name.ToLower();
		}
	}
}