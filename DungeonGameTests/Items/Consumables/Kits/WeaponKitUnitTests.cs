using DungeonGame.Helpers;
using DungeonGame.Items.Consumables.Kits;
using DungeonGame.Items.WeaponObjects;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits {
	internal class WeaponKitUnitTests {
		private WeaponKit _weaponKit;
		private Weapon _weapon;
		private readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup() {
			_weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.WeaponKitType.Grindstone);
			_weapon = new Weapon(3, WeaponType.Axe);
		}

		[Test]
		public void LightKitCreationTest() {
			Assert.AreEqual(1, _weaponKit.Weight);
			Assert.AreEqual(1, _weaponKit.KitAugmentAmount);
			Assert.AreEqual(false, _weaponKit.KitHasBeenUsed);
			Assert.AreEqual(_weaponKit.KitAugmentAmount * 10, _weaponKit.ItemValue);
		}

		[Test]
		public void MediumKitCreationTest() {
			_weaponKit = new WeaponKit(KitLevel.Medium, WeaponKit.WeaponKitType.Bowstring);

			Assert.AreEqual(2, _weaponKit.KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest() {
			_weaponKit = new WeaponKit(KitLevel.Heavy, WeaponKit.WeaponKitType.Bowstring);

			Assert.AreEqual(3, _weaponKit.KitAugmentAmount);
		}

		[Test]
		public void GrindstoneKitAugmentAxeSucceeds() {
			OutputHelper.Display.ClearUserOutput();
			int baseWeaponItemValue = _weapon.ItemValue;
			int baseWeaponDamage = _weapon.RegDamage;
			string displayMessage = $"You upgraded {_textInfo.ToTitleCase(_weapon.Name)} with a weapon kit.";

			_weaponKit.AttemptAugmentPlayerWeapon(_weapon);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, _weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + _weaponKit.ItemValue, _weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage + _weaponKit.KitAugmentAmount, _weapon.RegDamage);
		}

		[Test]
		public void GrindstoneKitAugmentBowFails() {
			OutputHelper.Display.ClearUserOutput();
			_weapon = new Weapon(3, WeaponType.Bow);
			int baseWeaponItemValue = _weapon.ItemValue;
			int baseWeaponDamage = _weapon.RegDamage;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_weapon.Name)} with that!";

			_weaponKit.AttemptAugmentPlayerWeapon(_weapon);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, _weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage, _weapon.RegDamage);
		}

		[Test]
		public void BowstringKitAugmentAxeFails() {
			OutputHelper.Display.ClearUserOutput();
			_weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.WeaponKitType.Bowstring);
			int baseWeaponItemValue = _weapon.ItemValue;
			int baseWeaponDamage = _weapon.RegDamage;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_weapon.Name)} with that!";

			_weaponKit.AttemptAugmentPlayerWeapon(_weapon);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, _weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage, _weapon.RegDamage);
		}

		[Test]
		public void BowstringKitAugmentBowSucceeds() {
			OutputHelper.Display.ClearUserOutput();
			_weapon = new Weapon(3, WeaponType.Bow);
			_weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.WeaponKitType.Bowstring);
			int baseWeaponItemValue = _weapon.ItemValue;
			int baseWeaponDamage = _weapon.RegDamage;
			string displayMessage = $"You upgraded {_textInfo.ToTitleCase(_weapon.Name)} with a weapon kit.";

			_weaponKit.AttemptAugmentPlayerWeapon(_weapon);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, _weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + _weaponKit.ItemValue, _weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage + _weaponKit.KitAugmentAmount, _weapon.RegDamage);
		}
	}
}
