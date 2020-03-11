using System;

namespace DungeonGame {
	public class ChangeWeapon {
		public enum KitType {
			Grindstone,
			Bowstring
		}
		public int ChangeAmount { get; set; }
		public KitType KitCategory { get; set; }

		// Default constructor for JSON serialization
		public ChangeWeapon() { }
		public ChangeWeapon(int amount, KitType kitType) {
			this.ChangeAmount = amount;
			this.KitCategory = kitType;
		}

		public void ChangeWeaponPlayer(Weapon weapon) {
			switch (this.KitCategory) {
				case KitType.Grindstone:
					if (weapon.WeaponGroup != Weapon.WeaponType.Bow) {
						weapon.RegDamage += this.ChangeAmount;
					}
					break;
				case KitType.Bowstring:
					if (weapon.WeaponGroup == Weapon.WeaponType.Bow) {
						weapon.RegDamage += this.ChangeAmount;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}