using Newtonsoft.Json;

namespace DungeonGame {
	public class Healing {
		public int HealAmount { get; set; }
		public int HealOverTime { get; set; }
		public int HealCurRounds { get; set; }
		public int HealMaxRounds { get; set; }

		public Healing(int healAmount) {
			this.HealAmount = healAmount;
		}
		[JsonConstructor]
		public Healing(int healAmount, int healOverTime, int healCurRounds, int healMaxRounds) {
			this.HealAmount = healAmount;
			this.HealOverTime = healOverTime;
			this.HealCurRounds = healCurRounds;
			this.HealMaxRounds = healMaxRounds;
		}
	}
}