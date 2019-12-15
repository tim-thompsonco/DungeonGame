namespace DungeonGame {
	public class Spell {
		public enum SpellType {
			FireOffense,
			FrostOffense,
			ArcaneOffense,
			Healing,
			Defense
		}
		public string Name { get; set; }
		public int ManaCost { get; set; }
		public int Level { get; set; }
		public SpellType SpellCategory { get; set; }
		public FireOffense FireOffense { get; set; }

		public Spell(
			string name,
			int manaCost,
			SpellType spellType,
			int level) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.SpellCategory = spellType;
			this.Level = level;
			if (this.SpellCategory == SpellType.FireOffense) {
				this.FireOffense = new FireOffense(30, 5, 1, 3);
			}
		}
	}
}