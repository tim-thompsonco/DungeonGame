namespace DungeonGame {
	public class AttackOption {
		public enum AttackType {
			Physical,
			Spell,
			Ability
		}
		public AttackType AttackCategory { get; set; }
		public int DamageAmount { get; set; }
		public int AttackIndex { get; set; }
		
		public AttackOption(AttackType attackCategory, int damageAmount, int attackIndex) {
			this.AttackCategory = attackCategory;
			this.DamageAmount = damageAmount;
			this.AttackIndex = attackIndex;
		}
	}
}