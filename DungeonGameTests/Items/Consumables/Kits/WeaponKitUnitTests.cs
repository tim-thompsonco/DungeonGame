using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits {
	class WeaponKitUnitTests {
		WeaponKit weaponKit;
		Weapon weapon;
		readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup() {
			weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.KitType.Grindstone);
			weapon = new Weapon(3, Weapon.WeaponType.Axe);
		}

		[Test]
		public void LightKitCreationTest() {
			Assert.AreEqual(1, weaponKit.Weight);
			Assert.AreEqual(1, weaponKit.KitAugmentAmount);
			Assert.AreEqual(false, weaponKit.KitHasBeenUsed);
			Assert.AreEqual(weaponKit.KitAugmentAmount * 10, weaponKit.ItemValue);
		}

		[Test]
		public void MediumKitCreationTest() {
			weaponKit = new WeaponKit(KitLevel.Medium, WeaponKit.KitType.Bowstring);

			Assert.AreEqual(2, weaponKit.KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest() {
			weaponKit = new WeaponKit(KitLevel.Heavy, WeaponKit.KitType.Bowstring);

			Assert.AreEqual(3, weaponKit.KitAugmentAmount);
		}

		[Test]
		public void GrindstoneKitAugmentAxeSucceeds() {
			OutputController.Display.ClearUserOutput();
			int baseWeaponItemValue = weapon.ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(weapon.Name)} with a weapon kit.";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(true, weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + weaponKit.ItemValue, weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage + weaponKit.KitAugmentAmount, weapon._RegDamage);
		}

		[Test]
		public void GrindstoneKitAugmentBowFails() {
			OutputController.Display.ClearUserOutput();
			weapon = new Weapon(3, Weapon.WeaponType.Bow);
			int baseWeaponItemValue = weapon.ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(weapon.Name)} with that!";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(false, weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage, weapon._RegDamage);
		}

		[Test]
		public void BowstringKitAugmentAxeFails() {
			OutputController.Display.ClearUserOutput();
			weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.KitType.Bowstring);
			int baseWeaponItemValue = weapon.ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(weapon.Name)} with that!";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(false, weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage, weapon._RegDamage);
		}

		[Test]
		public void BowstringKitAugmentBowSucceeds() {
			OutputController.Display.ClearUserOutput();
			weapon = new Weapon(3, Weapon.WeaponType.Bow);
			weaponKit = new WeaponKit(KitLevel.Light, WeaponKit.KitType.Bowstring);
			int baseWeaponItemValue = weapon.ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(weapon.Name)} with a weapon kit.";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(true, weaponKit.KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + weaponKit.ItemValue, weapon.ItemValue);
			Assert.AreEqual(baseWeaponDamage + weaponKit.KitAugmentAmount, weapon._RegDamage);
		}
	}
}
