using Newtonsoft.Json;

namespace DungeonGame {
	public class Offensive {
		public int Amount { get; set; }
		public int AmountOverTime { get; set; }
		public int AmountCurRounds { get; set; }
		public int AmountMaxRounds { get; set; }

		public Offensive(int amount) {
			this.Amount = amount;
		}
		[JsonConstructor]
		public Offensive(int amount, int amountOverTime, int amountCurRounds, int amountMaxRounds) {
			this.Amount = amount;
			this.AmountOverTime = amountOverTime;
			this.AmountCurRounds = amountCurRounds;
			this.AmountMaxRounds = amountMaxRounds;
		}
	}
}