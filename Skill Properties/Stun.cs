namespace DungeonGame {
	public class Stun {
		public int DamageAmount { get; set; }
		public int StunCurRounds { get; set; }
		public int StunMaxRounds { get; set; }

		// Default constructor for JSON serialization
		public Stun() { }
		public Stun(int damageAmount, int stunCurRounds, int stunMaxRounds) {
			DamageAmount = damageAmount;
			StunCurRounds = stunCurRounds;
			StunMaxRounds = stunMaxRounds;
		}
	}
}