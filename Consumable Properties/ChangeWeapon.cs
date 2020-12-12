using DungeonGame.Items;
using System;

namespace DungeonGame
{
	public class ChangeWeapon
	{
		public enum KitType
		{
			Grindstone,
			Bowstring
		}
		public int _ChangeAmount { get; set; }
		public KitType _KitCategory { get; set; }

		public ChangeWeapon(int amount, KitType kitType)
		{
			_ChangeAmount = amount;
			_KitCategory = kitType;
		}

		public void ChangeWeaponPlayer(Weapon weapon)
		{
			switch (_KitCategory)
			{
				case KitType.Grindstone:
					if (weapon._WeaponGroup != Weapon.WeaponType.Bow)
					{
						weapon._RegDamage += _ChangeAmount;
					}
					break;
				case KitType.Bowstring:
					if (weapon._WeaponGroup == Weapon.WeaponType.Bow)
					{
						weapon._RegDamage += _ChangeAmount;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}