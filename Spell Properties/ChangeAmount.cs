namespace DungeonGame {
	public class ChangeAmount {
		public int Amount { get; set; }
		public int ChangeCurRounds { get; set; }
		public int ChangeMaxRounds { get; set; }

		public ChangeAmount(int amount, int changeCurRounds, int changeMaxRounds) {
			this.Amount = amount;
			this.ChangeCurRounds = changeCurRounds;
			this.ChangeMaxRounds = changeMaxRounds;
		}
	}
}