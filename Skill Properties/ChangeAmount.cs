namespace DungeonGame
{
	public class ChangeAmount
	{
		public int Amount { get; set; }
		public int ChangeCurRound { get; set; }
		public int ChangeMaxRound { get; set; }

		// Default constructor for JSON serialization
		public ChangeAmount() { }
		public ChangeAmount(int amount, int changeCurRound, int changeMaxRound)
		{
			this.Amount = amount;
			this.ChangeCurRound = changeCurRound;
			this.ChangeMaxRound = changeMaxRound;
		}
	}
}