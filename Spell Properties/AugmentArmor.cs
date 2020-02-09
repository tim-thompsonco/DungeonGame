namespace DungeonGame {
	public class AugmentArmor {
		public int AugmentAmount { get; set; }
		public int AugmentCurRounds { get; set; }
		public int AugmentMaxRounds { get; set; }

		public AugmentArmor(int augmentAmount, int augmentCurRounds, int augmentMaxRounds) {
			this.AugmentAmount = augmentAmount;
			this.AugmentCurRounds = augmentCurRounds;
			this.AugmentMaxRounds = augmentMaxRounds;
		}
	}
}