namespace DungeonGame {
	public class RestoreMana {
		public int RestoreManaAmt { get; set; }

		public RestoreMana(int amount) {
			this.RestoreManaAmt = amount;
		}
		public void RestoreManaPlayer(Player player) {
			player.ManaPoints += RestoreManaAmt;
			if (player.ManaPoints > player.MaxManaPoints) {
				player.ManaPoints = player.MaxManaPoints;
			}
		}
	}
}