namespace DungeonGame {
	public class Offensive {
		public enum OffensiveType {
			Normal,
			Bleed,
			Fire
		}
		public OffensiveType _OffensiveGroup { get; set; }
		public int _Amount { get; set; }
		public int? _ChanceToSucceed { get; set; }
		public int _AmountOverTime { get; set; }
		public int _AmountCurRounds { get; set; }
		public int _AmountMaxRounds { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Offensive() { }
		public Offensive(int amount) {
			_Amount = amount;
		}
		public Offensive(int amount, int chanceToSucceed) {
			_Amount = amount;
			_ChanceToSucceed = chanceToSucceed;
		}
		public Offensive(int amount, int amountCurRounds, int amountMaxRounds) {
			_Amount = amount;
			_AmountCurRounds = amountCurRounds;
			_AmountMaxRounds = amountMaxRounds;
		}
		public Offensive(int amount, int amountOverTime, int amountCurRounds, int amountMaxRounds,
			OffensiveType offenseType) {
			_Amount = amount;
			_AmountOverTime = amountOverTime;
			_AmountCurRounds = amountCurRounds;
			_AmountMaxRounds = amountMaxRounds;
			_OffensiveGroup = offenseType;
		}
	}
}