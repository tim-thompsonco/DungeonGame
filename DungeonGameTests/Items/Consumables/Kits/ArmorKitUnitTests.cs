using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits {
	class ArmorKitUnitTests {
		ArmorKit armorKit;
		Armor armor;
		readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup() {
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Cloth);
			armor = new Armor(3, Armor.ArmorType.Cloth, Armor.ArmorSlot.Chest);
		}

		[Test]
		public void LightKitCreationTest() {
			Assert.AreEqual(1, armorKit._Weight);
			Assert.AreEqual(1, armorKit._KitAugmentAmount);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(armorKit._KitAugmentAmount * 10, armorKit._ItemValue);
		}

		[Test]
		public void MediumKitCreationTest() {
			armorKit = new ArmorKit(KitLevel.Medium, ArmorKit.KitType.Cloth);

			Assert.AreEqual(2, armorKit._KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest() {
			armorKit = new ArmorKit(KitLevel.Heavy, ArmorKit.KitType.Cloth);

			Assert.AreEqual(3, armorKit._KitAugmentAmount);
		}

		[Test]
		public void ClothKitAugmentClothSucceeds() {
			OutputController.Display.ClearUserOutput();
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(armor._Name)} with an armor kit.";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(true, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + armorKit._ItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating + armorKit._KitAugmentAmount, armor._ArmorRating);
		}

		[Test]
		public void ClothKitAugmentLeatherFails() {
			OutputController.Display.ClearUserOutput();
			armor = new Armor(3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}

		[Test]
		public void ClothKitAugmentPlateFails() {
			OutputController.Display.ClearUserOutput();
			armor = new Armor(3, Armor.ArmorType.Plate, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentLeatherSucceeds() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Leather);
			armor = new Armor(3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(armor._Name)} with an armor kit.";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(true, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + armorKit._ItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating + armorKit._KitAugmentAmount, armor._ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentClothFails() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Leather);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentPlateFails() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Leather);
			armor = new Armor(3, Armor.ArmorType.Plate, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}

		[Test]
		public void PlateKitAugmentPlateSucceeds() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Plate);
			armor = new Armor(3, Armor.ArmorType.Plate, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You upgraded {textInfo.ToTitleCase(armor._Name)} with an armor kit.";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(true, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + armorKit._ItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating + armorKit._KitAugmentAmount, armor._ArmorRating);
		}

		[Test]
		public void PlateKitAugmentClothFails() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Plate);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}

		[Test]
		public void PlateKitAugmentLeatherFails() {
			OutputController.Display.ClearUserOutput();
			armorKit = new ArmorKit(KitLevel.Light, ArmorKit.KitType.Plate);
			armor = new Armor(3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			int baseArmorItemValue = armor._ItemValue;
			int baseArmorRating = armor._ArmorRating;
			string displayMessage = $"You can't upgrade {textInfo.ToTitleCase(armor._Name)} with that!";

			armorKit.AttemptAugmentArmorPlayer(armor);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(false, armorKit._KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, armor._ItemValue);
			Assert.AreEqual(baseArmorRating, armor._ArmorRating);
		}
	}
}
