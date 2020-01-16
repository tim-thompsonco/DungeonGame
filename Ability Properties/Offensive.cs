using Newtonsoft.Json;

namespace DungeonGame {
	public class Offensive {
		public int Amount { get; set;  }
		public int ChanceToSucceed { get; set; }
		public int AmountOverTime { get; set;  }
		public int AmountCurRounds { get; set;  }
		public int AmountMaxRounds { get; set;  }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Offensive() {}
		public Offensive(int amount) {
			this.Amount = amount;
		}
		public Offensive(int amount, int chanceToSucceed) {
			this.Amount = amount;
			this.ChanceToSucceed = chanceToSucceed;
		}
		public Offensive(int amount, int amountOverTime, int amountCurRounds, int amountMaxRounds) {
			this.Amount = amount;
			this.AmountOverTime = amountOverTime;
			this.AmountCurRounds = amountCurRounds;
			this.AmountMaxRounds = amountMaxRounds;
		}
	}
}