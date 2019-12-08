namespace DungeonGame {
	public class RestoreMana {
		public int RestoreManaAmt { get; }

		public RestoreMana(int amount) {
			this.RestoreManaAmt = amount;
		}
		public void RestoreManaPlayer(NewPlayer player) {
			player.ManaPoints += RestoreManaAmt;
			if (player.ManaPoints > player.MaxManaPoints) {
				player.ManaPoints = player.MaxManaPoints;
			}
		}
	}
}