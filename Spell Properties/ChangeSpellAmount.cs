namespace DungeonGame {
	public class ChangeSpellAmount {
		public int Amount { get; set; }
		public int ChangeCurRounds { get; set; }
		public int ChangeMaxRounds { get; set; }

		public ChangeSpellAmount(int amount, int changeCurRounds, int changeMaxRounds) {
			this.Amount = amount;
			this.ChangeCurRounds = changeCurRounds;
			this.ChangeMaxRounds = changeMaxRounds;
		}
	}
}