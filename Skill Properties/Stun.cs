namespace DungeonGame {
	public class Stun {
		public int DamageAmount { get; set; }
		public int StunCurRounds { get; set; }
		public int StunMaxRounds { get; set; }

		// Default constructor for JSON serialization
		public Stun() { }
		public Stun(int damageAmount, int stunCurRounds, int stunMaxRounds) {
			this.DamageAmount = damageAmount;
			this.StunCurRounds = stunCurRounds;
			this.StunMaxRounds = stunMaxRounds;
		}
	}
}