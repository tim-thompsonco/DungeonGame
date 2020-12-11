namespace DungeonGame
{
	public class RestoreMana
	{
		public int _RestoreManaAmt { get; set; }

		// Default constructor for JSON serialization
		public RestoreMana() { }
		public RestoreMana(int amount)
		{
			_RestoreManaAmt = amount;
		}

		public void RestoreManaPlayer(Player player)
		{
			if (player._ManaPoints + _RestoreManaAmt > player._MaxManaPoints)
			{
				player._ManaPoints = player._MaxManaPoints;
			}
			else
			{
				player._ManaPoints += _RestoreManaAmt;
			}
		}
	}
}