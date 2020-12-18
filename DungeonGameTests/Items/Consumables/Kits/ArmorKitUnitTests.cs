using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits
{
	class ArmorKitUnitTests
	{
		ArmorKit armorKit;
		Armor armor;
		TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup()
		{
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Cloth);
			armor = new Armor(3, Armor.ArmorType.Cloth, Armor.ArmorSlot.Chest);
		}

		[Test]
		public void LightKitCreationTest()
		{
			Assert.AreEqual(1, armorKit._Weight);
			Assert.AreEqual(1, armorKit._KitAugmentAmount);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(armorKit._KitAugmentAmount * 10, armorKit._ItemValue);
		}

		[Test]
		public void MediumKitCreationTest()
		{
			armorKit = new ArmorKit(KitLevel.Medium, ArmorKit.KitType.Cloth);

			Assert.AreEqual(2, armorKit._KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest()
		{
			armorKit = new ArmorKit(KitLevel.Heavy, ArmorKit.KitType.Cloth);

			Assert.AreEqual(3, armorKit._KitAugmentAmount);
		}
	}
}
