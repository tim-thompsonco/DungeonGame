namespace DungeonGame {
	public class FireOffense {
		public int BlastDamage { get; set; }
		public int BurnDamage { get; set; }
		public int BurnCurRounds { get; set; }
		public int BurnMaxRounds { get; set; }

		public FireOffense(int blastDmg, int burnDmg, int burnCurRounds, int burnMaxRounds) {
			this.BlastDamage = blastDmg;
			this.BurnDamage = burnDmg;
			this.BurnCurRounds = burnCurRounds;
			this.BurnMaxRounds = burnMaxRounds;
		}
	}
}