namespace DungeonGame {
	public class AttackOption {
		public enum AttackType {
			Physical,
			Spell,
			Ability
		}
		public AttackType _AttackCategory { get; set; }
		public int _DamageAmount { get; set; }
		public int _AttackIndex { get; set; }

		public AttackOption(AttackType attackCategory, int damageAmount, int attackIndex) {
			_AttackCategory = attackCategory;
			_DamageAmount = damageAmount;
			_AttackIndex = attackIndex;
		}
	}
}