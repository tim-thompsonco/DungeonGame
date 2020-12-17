using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using DungeonGame.Items.Consumables.Kits;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits
{
	class WeaponKitUnitTests
	{
		WeaponKit weaponKit;
		Weapon weapon;
		TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup()
		{
			weaponKit = new WeaponKit(Kit.KitLevel.Light, WeaponKit.KitType.Grindstone);
			weapon = new Weapon(3, Weapon.WeaponType.Axe);
		}

		[Test]
		public void LightKitCreationTest()
		{
			Assert.AreEqual(1, weaponKit._Weight);
			Assert.AreEqual(1, weaponKit._KitAugmentAmount);
			Assert.AreEqual(false, weaponKit._KitHasBeenUsed);
			Assert.AreEqual(weaponKit._KitAugmentAmount * 10, weaponKit._ItemValue);
		}

		[Test]
		public void MediumKitCreationTest()
		{
			weaponKit = new WeaponKit(Kit.KitLevel.Medium, WeaponKit.KitType.Bowstring);

			Assert.AreEqual(2, weaponKit._KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest()
		{
			weaponKit = new WeaponKit(Kit.KitLevel.Heavy, WeaponKit.KitType.Bowstring);

			Assert.AreEqual(3, weaponKit._KitAugmentAmount);
		}

		[Test]
		public void GrindstoneKitAugmentAxeSucceeds()
		{
			OutputController.Display.ClearUserOutput();
			int baseWeaponItemValue = weapon._ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string weaponName = weapon._Name;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(weaponName)} with a weapon kit.";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(true, weaponKit._KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + weaponKit._ItemValue, weapon._ItemValue);
			Assert.AreEqual(baseWeaponDamage + weaponKit._KitAugmentAmount, weapon._RegDamage);
		}

		[Test]
		public void GrindstoneKitAugmentBowFails()
		{
			OutputController.Display.ClearUserOutput();
			weapon = new Weapon(3, Weapon.WeaponType.Bow);
			int baseWeaponItemValue = weapon._ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string weaponName = weapon._Name;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(weaponName)} with that!";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, weaponKit._KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, weapon._ItemValue);
			Assert.AreEqual(baseWeaponDamage, weapon._RegDamage);
		}

		[Test]
		public void BowstringKitAugmentAxeFails()
		{
			OutputController.Display.ClearUserOutput();
			weaponKit = new WeaponKit(Kit.KitLevel.Light, WeaponKit.KitType.Bowstring);
			int baseWeaponItemValue = weapon._ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string weaponName = weapon._Name;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(weaponName)} with that!";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, weaponKit._KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue, weapon._ItemValue);
			Assert.AreEqual(baseWeaponDamage, weapon._RegDamage);
		}

		[Test]
		public void BowstringKitAugmentBowSucceeds()
		{
			OutputController.Display.ClearUserOutput();
			weapon = new Weapon(3, Weapon.WeaponType.Bow);
			weaponKit = new WeaponKit(Kit.KitLevel.Light, WeaponKit.KitType.Bowstring);
			int baseWeaponItemValue = weapon._ItemValue;
			int baseWeaponDamage = weapon._RegDamage;
			string weaponName = weapon._Name;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(weaponName)} with a weapon kit.";

			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(true, weaponKit._KitHasBeenUsed);
			Assert.AreEqual(baseWeaponItemValue + weaponKit._ItemValue, weapon._ItemValue);
			Assert.AreEqual(baseWeaponDamage + weaponKit._KitAugmentAmount, weapon._RegDamage);
		}
	}
}
