using DungeonGame.Controllers;
using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public class WeaponKit : IItem, IKit {
		public enum KitType {
			Grindstone,
			Bowstring
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public bool _KitHasBeenUsed { get; set; }
		public KitLevel _KitLevel { get; set; }
		public KitType _KitType { get; set; }
		public int _KitAugmentAmount { get; set; }
		public TextInfo _TextInfo { get; set; }

		public WeaponKit(KitLevel kitLevel, KitType kitType) {
			_KitLevel = kitLevel;
			_KitType = kitType;
			_TextInfo = new CultureInfo("en-US", false).TextInfo;
			_Weight = 1;
			_KitAugmentAmount = GetKitAugmentAmount();
			_ItemValue = _KitAugmentAmount * 10;
			_Name = $"{kitLevel.ToString().ToLower()} {_KitType.ToString().ToLower()} weapon kit";
			_Desc = $"A single-use {_Name} that increases weapon damage by {_KitAugmentAmount} for one weapon item.";
		}

		public int GetKitAugmentAmount() {
			if (_KitLevel == KitLevel.Light) {
				return 1;
			} else if (_KitLevel == KitLevel.Medium) {
				return 2;
			} else {
				// If kit level is not light or medium, then it is heavy
				return 3;
			}
		}

		public Weapon AttemptAugmentPlayerWeapon(Weapon weapon) {
			if (WeaponKitMatchesWeaponType(weapon)) {
				AugmentWeaponDamage(weapon);
				AugmentWeaponItemValue(weapon);
				SetKitAsUsed();
				DisplayAugmentSuccessMessage(weapon);
			} else {
				DisplayAugmentFailMessage(weapon);
			}

			return weapon;
		}

		private bool WeaponKitMatchesWeaponType(Weapon weapon) {
			if (_KitType == KitType.Bowstring && weapon._WeaponGroup == Weapon.WeaponType.Bow) {
				return true;
			}
			// A grindstone can be used on any weapon group except bow
			else if (_KitType == KitType.Grindstone && weapon._WeaponGroup != Weapon.WeaponType.Bow) {
				return true;
			}

			return false;
		}

		private Weapon AugmentWeaponDamage(Weapon weapon) {
			weapon._RegDamage += _KitAugmentAmount;

			return weapon;
		}

		private Weapon AugmentWeaponItemValue(Weapon weapon) {
			weapon._ItemValue += _ItemValue;

			return weapon;
		}

		public void SetKitAsUsed() {
			_KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Weapon weapon) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {_TextInfo.ToTitleCase(weapon._Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Weapon weapon) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {_TextInfo.ToTitleCase(weapon._Name)} with a weapon kit.");
		}
	}
}
