namespace DungeonGame
{
	public class Healing
	{
		public int _HealAmount { get; set; }
		public int _HealOverTime { get; set; }
		public int _HealCurRounds { get; set; }
		public int _HealMaxRounds { get; set; }

		// Default constructor for JSON serialization
		public Healing() { }
		public Healing(int healAmount)
		{
			_HealAmount = healAmount;
		}
		public Healing(int healAmount, int healOverTime, int healCurRounds, int healMaxRounds)
		{
			_HealAmount = healAmount;
			_HealOverTime = healOverTime;
			_HealCurRounds = healCurRounds;
			_HealMaxRounds = healMaxRounds;
		}
	}
}