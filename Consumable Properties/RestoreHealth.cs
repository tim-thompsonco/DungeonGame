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
			player.HitPoints += this.RestoreHealthAmt;
			if (player.HitPoints > player.MaxHitPoints)
			{
				player.HitPoints = player.MaxHitPoints;
			}
		}
	}
}
