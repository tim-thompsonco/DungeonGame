using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Kits
{
	public class WeaponKit : Kit
	{
		public enum KitType
		{
			Grindstone,
			Bowstring
		}
		private readonly KitType _KitType;

		public WeaponKit(KitLevel kitLevel, KitType kitType) : base(kitLevel)
		{
			_KitType = kitType;
			_Name = $"{kitLevel.ToString().ToLower()} {_KitType.ToString().ToLower()} weapon kit";
			_Desc = $"A single-use {_Name} that increases weapon damage by {_KitAugmentAmount} for one weapon item.";
		}

		public Weapon AttemptAugmentPlayerWeapon(Weapon weapon)
		{
			if (WeaponKitMatchesWeaponType(weapon))
			{
				AugmentWeaponDamage(weapon);
				AugmentWeaponItemValue(weapon);
				SetKitAsUsed();
				DisplayAugmentSuccessMessage(weapon);
			}
			else
			{
				DisplayAugmentFailMessage(weapon);
			}

			return weapon;
		}

		private bool WeaponKitMatchesWeaponType(Weapon weapon)
		{
			if (_KitType == KitType.Bowstring && weapon._WeaponGroup == Weapon.WeaponType.Bow)
			{
				return true;
			}
			// A grindstone can be used on any weapon group except bow
			else if (_KitType == KitType.Grindstone && weapon._WeaponGroup != Weapon.WeaponType.Bow)
			{
				return true;
			}

			return false;
		}

		private Weapon AugmentWeaponDamage(Weapon weapon)
		{
			weapon._RegDamage += _KitAugmentAmount;

			return weapon;
		}

		private Weapon AugmentWeaponItemValue(Weapon weapon)
		{
			weapon._ItemValue += _ItemValue;

			return weapon;
		}

		protected override void SetKitAsUsed()
		{
			_KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Weapon weapon)
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {_TextInfo.ToTitleCase(weapon._Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Weapon weapon)
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {_TextInfo.ToTitleCase(weapon._Name)} with a weapon kit.");
		}
	}
}
