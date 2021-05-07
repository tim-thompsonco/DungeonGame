namespace DungeonGame.AttackOptions {
	public class AttackOption {
		public AttackType AttackCategory { get; set; }
		public int AttackIndex { get; set; }
		public int DamageAmount { get; set; }

		public AttackOption(AttackType attackCategory, int damageAmount, int attackIndex) {
			AttackCategory = attackCategory;
			DamageAmount = damageAmount;
			AttackIndex = attackIndex;
		}
	}
}