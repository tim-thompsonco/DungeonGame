﻿using DungeonGame.Helpers;
using DungeonGame.Items.WeaponObjects;
using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public partial class WeaponKit : IItem, IKit {
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public bool KitHasBeenUsed { get; set; }
		public KitLevel WeaponKitLevel { get; set; }
		public WeaponKitType KitType { get; set; }
		public int KitAugmentAmount { get; set; }
		public TextInfo TextInfo { get; set; }

		public WeaponKit(KitLevel kitLevel, WeaponKitType kitType) {
			WeaponKitLevel = kitLevel;
			KitType = kitType;
			TextInfo = new CultureInfo("en-US", false).TextInfo;
			Weight = 1;
			KitAugmentAmount = GetKitAugmentAmount();
			ItemValue = KitAugmentAmount * 10;
			Name = $"{kitLevel.ToString().ToLower()} {KitType.ToString().ToLower()} weapon kit";
			Desc = $"A single-use {Name} that increases weapon damage by {KitAugmentAmount} for one weapon item.";
		}

		public int GetKitAugmentAmount() {
			if (WeaponKitLevel == KitLevel.Light) {
				return 1;
			} else if (WeaponKitLevel == KitLevel.Medium) {
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
			if (KitType == WeaponKitType.Bowstring && weapon.WeaponGroup == WeaponType.Bow) {
				return true;
			}
			// A grindstone can be used on any weapon group except bow
			else if (KitType == WeaponKitType.Grindstone && weapon.WeaponGroup != WeaponType.Bow) {
				return true;
			}

			return false;
		}

		private Weapon AugmentWeaponDamage(Weapon weapon) {
			weapon.RegDamage += KitAugmentAmount;

			return weapon;
		}

		private Weapon AugmentWeaponItemValue(Weapon weapon) {
			weapon.ItemValue += ItemValue;

			return weapon;
		}

		public void SetKitAsUsed() {
			KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Weapon weapon) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {TextInfo.ToTitleCase(weapon.Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Weapon weapon) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {TextInfo.ToTitleCase(weapon.Name)} with a weapon kit.");
		}
	}
}
