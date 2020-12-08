namespace DungeonGame
{
	public class Stun
	{
		public int _DamageAmount { get; set; }
		public int _StunCurRounds { get; set; }
		public int _StunMaxRounds { get; set; }

		// Default constructor for JSON serialization
		public Stun() { }
		public Stun(int damageAmount, int stunCurRounds, int stunMaxRounds)
		{
			_DamageAmount = damageAmount;
			_StunCurRounds = stunCurRounds;
			_StunMaxRounds = stunMaxRounds;
		}
	}
}