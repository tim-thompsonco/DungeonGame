namespace DungeonGame {
	public class Defense {
		public int AugmentAmount { get; set; }
		public int AugmentCurRounds { get; set; }
		public int AugmentMaxRounds { get; set; }

		public Defense(int augmentAmount, int augmentCurRounds, int augmentMaxRounds) {
			this.AugmentAmount = augmentAmount;
			this.AugmentCurRounds = augmentCurRounds;
			this.AugmentMaxRounds = augmentMaxRounds;
		}
	}
}