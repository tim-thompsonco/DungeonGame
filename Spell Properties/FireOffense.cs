namespace DungeonGame {
	public class FireOffense {
		public int BlastDamage { get; }
		public int BurnDamage { get; }
		public int BurnCurRounds { get; set; }
		public int BurnMaxRounds { get; }

		public FireOffense(int blastDmg, int burnDmg, int burnCurRounds, int burnMaxRounds) {
			this.BlastDamage = blastDmg;
			this.BurnDamage = burnDmg;
			this.BurnCurRounds = burnCurRounds;
			this.BurnMaxRounds = burnMaxRounds;
		}
	}
}