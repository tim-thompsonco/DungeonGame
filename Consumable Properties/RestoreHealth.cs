namespace DungeonGame
{
	public class RestoreHealth
	{
		public int RestoreHealthAmt { get; set; }

		// Default constructor for JSON serialization
		public RestoreHealth() { }
		public RestoreHealth(int amount)
		{
			this.RestoreHealthAmt = amount;
		}

		public void RestoreHealthPlayer(Player player)
		{
			player._HitPoints += this.RestoreHealthAmt;
			if (player._HitPoints > player._MaxHitPoints)
			{
				player._HitPoints = player._MaxHitPoints;
			}
		}
	}
}
