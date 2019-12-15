namespace DungeonGame {
	public class RestoreHealth {
		public int RestoreHealthAmt { get; set; }

		public RestoreHealth(int amount) {
			this.RestoreHealthAmt = amount;
		}
		public void RestoreHealthPlayer(Player player) {
			player.HitPoints += RestoreHealthAmt;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
		}
	}
}
