using DungeonGame.Helpers;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Consumables.Kits;
using NUnit.Framework;
using System.Globalization;

namespace DungeonGameTests.Items.Consumables.Kits {
	internal class ArmorKitUnitTests {
		private ArmorKit _armorKit;
		private Armor _armor;
		private readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;

		[SetUp]
		public void Setup() {
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Cloth);
			_armor = new Armor(3, ArmorType.Cloth, ArmorSlot.Chest);
		}

		[Test]
		public void LightKitCreationTest() {
			Assert.AreEqual(1, _armorKit.Weight);
			Assert.AreEqual(1, _armorKit.KitAugmentAmount);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(_armorKit.KitAugmentAmount * 10, _armorKit.ItemValue);
		}

		[Test]
		public void MediumKitCreationTest() {
			_armorKit = new ArmorKit(KitLevel.Medium, ArmorKit.ArmorKitType.Cloth);

			Assert.AreEqual(2, _armorKit.KitAugmentAmount);
		}

		[Test]
		public void HeavyKitCreationTest() {
			_armorKit = new ArmorKit(KitLevel.Heavy, ArmorKit.ArmorKitType.Cloth);

			Assert.AreEqual(3, _armorKit.KitAugmentAmount);
		}

		[Test]
		public void ClothKitAugmentClothSucceeds() {
			OutputHelper.Display.ClearUserOutput();
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You upgraded {_textInfo.ToTitleCase(_armor.Name)} with an armor kit.";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + _armorKit.ItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating + _armorKit.KitAugmentAmount, _armor.ArmorRating);
		}

		[Test]
		public void ClothKitAugmentLeatherFails() {
			OutputHelper.Display.ClearUserOutput();
			_armor = new Armor(3, ArmorType.Leather, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}

		[Test]
		public void ClothKitAugmentPlateFails() {
			OutputHelper.Display.ClearUserOutput();
			_armor = new Armor(3, ArmorType.Plate, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentLeatherSucceeds() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Leather);
			_armor = new Armor(3, ArmorType.Leather, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You upgraded {_textInfo.ToTitleCase(_armor.Name)} with an armor kit.";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + _armorKit.ItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating + _armorKit.KitAugmentAmount, _armor.ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentClothFails() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Leather);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}

		[Test]
		public void LeatherKitAugmentPlateFails() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Leather);
			_armor = new Armor(3, ArmorType.Plate, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}

		[Test]
		public void PlateKitAugmentPlateSucceeds() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Plate);
			_armor = new Armor(3, ArmorType.Plate, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You upgraded {_textInfo.ToTitleCase(_armor.Name)} with an armor kit.";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue + _armorKit.ItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating + _armorKit.KitAugmentAmount, _armor.ArmorRating);
		}

		[Test]
		public void PlateKitAugmentClothFails() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Plate);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}

		[Test]
		public void PlateKitAugmentLeatherFails() {
			OutputHelper.Display.ClearUserOutput();
			_armorKit = new ArmorKit(KitLevel.Light, ArmorKit.ArmorKitType.Plate);
			_armor = new Armor(3, ArmorType.Leather, ArmorSlot.Chest);
			int baseArmorItemValue = _armor.ItemValue;
			int baseArmorRating = _armor.ArmorRating;
			string displayMessage = $"You can't upgrade {_textInfo.ToTitleCase(_armor.Name)} with that!";

			_armorKit.AttemptAugmentArmorPlayer(_armor);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(false, _armorKit.KitHasBeenUsed);
			Assert.AreEqual(baseArmorItemValue, _armor.ItemValue);
			Assert.AreEqual(baseArmorRating, _armor.ArmorRating);
		}
	}
}
