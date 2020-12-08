namespace DungeonGame
{
	public class ChangeAmount
	{
		public int _Amount { get; set; }
		public int _ChangeCurRound { get; set; }
		public int _ChangeMaxRound { get; set; }

		// Default constructor for JSON serialization
		public ChangeAmount() { }
		public ChangeAmount(int amount, int changeCurRound, int changeMaxRound)
		{
			_Amount = amount;
			_ChangeCurRound = changeCurRound;
			_ChangeMaxRound = changeMaxRound;
		}
	}
}