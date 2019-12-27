namespace DungeonGame {
	public class ChangeArmor {
		public int ChangeArmorAmount { get; set; }
		public int ChangeCurRound { get; set; }
		public int ChangeMaxRound { get; set; }

		public ChangeArmor(int changeArmorAmount, int changeCurRound, int changeMaxRound) {
			this.ChangeArmorAmount = changeArmorAmount;
			this.ChangeCurRound = changeCurRound;
			this.ChangeMaxRound = changeMaxRound;
		}
	}
}