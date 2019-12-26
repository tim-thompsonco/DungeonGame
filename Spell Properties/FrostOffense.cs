namespace DungeonGame {
	public class FrostOffense {
		public int FrostDamage { get; set; }
		public int FrozenCurRounds { get; set; }
		public int FrozenMaxRounds { get; set; }

		public FrostOffense(int frostDamage, int frozenCurRounds, int frozenMaxRounds) {
			this.FrostDamage = frostDamage;
			this.FrozenCurRounds = frozenCurRounds;
			this.FrozenMaxRounds = frozenMaxRounds;
		}
	}
}