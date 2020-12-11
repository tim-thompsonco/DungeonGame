namespace DungeonGame
{
	public class RestoreHealth
	{
		public int _RestoreHealthAmt { get; set; }

		// Default constructor for JSON serialization
		public RestoreHealth() { }
		public RestoreHealth(int amount)
		{
			_RestoreHealthAmt = amount;
		}

		public void RestoreHealthPlayer(Player player)
		{
			if (player._HitPoints + _RestoreHealthAmt > player._MaxHitPoints)
			{
				player._HitPoints = player._MaxHitPoints;
			}
			else
			{
				player._HitPoints += _RestoreHealthAmt;
			}
		}
	}
}
