using System;

namespace DungeonGame {
	public class ChangeArmor {
		public enum KitType {
			Cloth,
			Leather,
			Plate
		}
		public int ChangeAmount { get; set; }
		public KitType KitCategory { get; set; }

		public ChangeArmor(int amount, KitType kitType) {
			this.ChangeAmount = amount;
			this.KitCategory = kitType;
		}
		
		public void ChangeArmorPlayer(Armor armor) {
			switch (this.KitCategory) {
				case KitType.Cloth:
					if (armor.ArmorGroup == Armor.ArmorType.Cloth) {
						armor.ArmorRating += this.ChangeAmount;
					}
					break;
				case KitType.Leather:
					if (armor.ArmorGroup == Armor.ArmorType.Leather) {
						armor.ArmorRating += this.ChangeAmount;
					}
					break;
				case KitType.Plate:
					if (armor.ArmorGroup == Armor.ArmorType.Plate) {
						armor.ArmorRating += this.ChangeAmount;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}