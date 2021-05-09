namespace DungeonGame {
	public class Offensive {
		public OffensiveType OffensiveGroup { get; set; }
		public int Amount { get; set; }
		public int? ChanceToSucceed { get; set; }
		public int AmountOverTime { get; set; }
		public int AmountCurRounds { get; set; }
		public int AmountMaxRounds { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Offensive() { }
		public Offensive(int amount) {
			Amount = amount;
		}
		public Offensive(int amount, int chanceToSucceed) {
			Amount = amount;
			ChanceToSucceed = chanceToSucceed;
		}
		public Offensive(int amount, int amountCurRounds, int amountMaxRounds) {
			Amount = amount;
			AmountCurRounds = amountCurRounds;
			AmountMaxRounds = amountMaxRounds;
		}
		public Offensive(int amount, int amountOverTime, int amountCurRounds, int amountMaxRounds,
			OffensiveType offenseType) {
			Amount = amount;
			AmountOverTime = amountOverTime;
			AmountCurRounds = amountCurRounds;
			AmountMaxRounds = amountMaxRounds;
			OffensiveGroup = offenseType;
		}
	}
}