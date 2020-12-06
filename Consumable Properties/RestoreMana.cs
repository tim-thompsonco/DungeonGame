namespace DungeonGame
{
	public class RestoreMana
	{
		public int RestoreManaAmt { get; set; }

		// Default constructor for JSON serialization
		public RestoreMana() { }
		public RestoreMana(int amount)
		{
			this.RestoreManaAmt = amount;
		}

		public void RestoreManaPlayer(Player player)
		{
			player._ManaPoints += this.RestoreManaAmt;
			if (player._ManaPoints > player._MaxManaPoints)
			{
				player._ManaPoints = player._MaxManaPoints;
			}
		}
	}
}