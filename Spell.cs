using System;

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
		public SpellType SpellCategory { get; set; }
		public FireOffense FireOffense { get; set; }
		public int ManaCost { get; set; }
		public int Rank { get; set; }

		public Spell(string name, int manaCost, int rank, SpellType spellType) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
			if (this.SpellCategory == SpellType.FireOffense) {
				this.FireOffense = new FireOffense(
					30, // Blast damage
					5, // Burn damage
					1, // Burn current rounds
					3 // Burn max rounds
					);
			}
		}

		public string GetName() {
			return this.Name.ToString();
		}
	}
}