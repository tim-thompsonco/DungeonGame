namespace DungeonGame {
	public class Healing {
		public int HealAmount { get; set; }
		public int HealOverTime { get; set; }
		public int HealCurRounds { get; set; }
		public int HealMaxRounds { get; set; }

		// Default constructor for JSON serialization
		public Healing() { }
		public Healing(int healAmount) {
			HealAmount = healAmount;
		}
		public Healing(int healAmount, int healOverTime, int healCurRounds, int healMaxRounds) {
			HealAmount = healAmount;
			HealOverTime = healOverTime;
			HealCurRounds = healCurRounds;
			HealMaxRounds = healMaxRounds;
		}
	}
}