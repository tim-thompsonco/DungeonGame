namespace DungeonGame {
	public class Spell {
		public string name { get; }
		public int blastDamage { get; set; }
		public int burnDamage { get; set; }
		public int burnCurRounds { get; set; }
		public int burnMaxRounds { get; set; }
		public int level { get; set; }

		public Spell(
			string name,
			int blastDamage,
			int burnDamage,
			int burnCurRounds,
			int burnMaxRounds,
			int level) {
			this.name = name;
			this.blastDamage = blastDamage;
			this.burnDamage = burnDamage;
			this.burnCurRounds = burnCurRounds;
			this.burnMaxRounds = burnMaxRounds;
			this.level = level;
		}
	}
}