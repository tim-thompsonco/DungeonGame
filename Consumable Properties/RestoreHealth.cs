namespace DungeonGame {
	public class RestoreHealth {
		public int RestoreHealthAmt { get; }

		public RestoreHealth(int amount) {
			this.RestoreHealthAmt = amount;
		}
		public void RestoreHealthPlayer(NewPlayer player) {
			player.HitPoints += RestoreHealthAmt;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
		}
	}
}
