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
		public int _ChangeAmount { get; set; }
		public KitType _KitCategory { get; set; }

		public ChangeArmor(int amount, KitType kitType)
		{
			_ChangeAmount = amount;
			_KitCategory = kitType;
		}

		public void ChangeArmorPlayer(Armor armor)
		{
			switch (_KitCategory)
			{
				case KitType.Cloth:
					if (armor._ArmorGroup == Armor.ArmorType.Cloth)
					{
						armor._ArmorRating += _ChangeAmount;
					}
					break;
				case KitType.Leather:
					if (armor._ArmorGroup == Armor.ArmorType.Leather)
					{
						armor._ArmorRating += _ChangeAmount;
					}
					break;
				case KitType.Plate:
					if (armor._ArmorGroup == Armor.ArmorType.Plate)
					{
						armor._ArmorRating += _ChangeAmount;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}