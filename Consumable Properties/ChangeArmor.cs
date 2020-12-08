using System;

namespace DungeonGame
{
	public class ChangeArmor
	{
		public enum KitType
		{
			Cloth,
			Leather,
			Plate
		}
		public int ChangeAmount { get; set; }
		public KitType KitCategory { get; set; }

		// Default constructor for JSON serialization
		public ChangeArmor() { }
		public ChangeArmor(int amount, KitType kitType)
		{
			this.ChangeAmount = amount;
			this.KitCategory = kitType;
		}

		public void ChangeArmorPlayer(Armor armor)
		{
			switch (this.KitCategory)
			{
				case KitType.Cloth:
					if (armor._ArmorGroup == Armor.ArmorType.Cloth)
					{
						armor._ArmorRating += this.ChangeAmount;
					}
					break;
				case KitType.Leather:
					if (armor._ArmorGroup == Armor.ArmorType.Leather)
					{
						armor._ArmorRating += this.ChangeAmount;
					}
					break;
				case KitType.Plate:
					if (armor._ArmorGroup == Armor.ArmorType.Plate)
					{
						armor._ArmorRating += this.ChangeAmount;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}