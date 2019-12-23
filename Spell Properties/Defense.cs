namespace DungeonGame {
	public class Defense {
		public int AugmentArmor { get; set; }
		public int AugmentCurRounds { get; set; }
		public int AugmentMaxRounds { get; set; }

		public Defense(int augmentArmor, int augmentCurRounds, int augmentMaxRounds) {
			this.AugmentArmor = augmentArmor;
			this.AugmentCurRounds = augmentCurRounds;
			this.AugmentMaxRounds = augmentMaxRounds;
		}
	}
}