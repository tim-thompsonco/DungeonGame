namespace DungeonGame {
	public class ChangeAbilityAmount {
		public int Amount { get; set; }
		public int ChangeCurRound { get; set; }
		public int ChangeMaxRound { get; set; }

		public ChangeAbilityAmount(int amount, int changeCurRound, int changeMaxRound) {
			this.Amount = amount;
			this.ChangeCurRound = changeCurRound;
			this.ChangeMaxRound = changeMaxRound;
		}
	}
}