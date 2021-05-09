using DungeonGame.Helpers;
using DungeonGame.Items.ArmorObjects;
using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public partial class ArmorKit : IItem, IKit {
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public bool KitHasBeenUsed { get; set; }
		public KitLevel KitLevel { get; }
		public int KitAugmentAmount { get; set; }
		public TextInfo TextInfo { get; set; }
		private readonly ArmorKitType _kitType;

		public ArmorKit(KitLevel kitLevel, ArmorKitType kitType) {
			KitLevel = kitLevel;
			_kitType = kitType;
			TextInfo = new CultureInfo("en-US", false).TextInfo;
			Weight = 1;
			KitAugmentAmount = GetKitAugmentAmount();
			ItemValue = KitAugmentAmount * 10;
			Name = $"{kitLevel.ToString().ToLower()} {_kitType.ToString().ToLower()} armor kit";
			Desc = $"A single-use {Name} that increases armor rating by {KitAugmentAmount} for one armor item.";
		}

		public int GetKitAugmentAmount() {
			if (KitLevel == KitLevel.Light) {
				return 1;
			} else if (KitLevel == KitLevel.Medium) {
				return 2;
			} else {
				// If kit level is not light or medium, then it is heavy
				return 3;
			}
		}

		public Armor AttemptAugmentArmorPlayer(Armor armor) {
			if (ArmorKitMatchesArmorType(armor)) {
				AugmentArmorRating(armor);
				AugmentArmorItemValue(armor);
				SetKitAsUsed();
				DisplayAugmentSuccessMessage(armor);
			} else {
				DisplayAugmentFailMessage(armor);
			}

			return armor;
		}

		private bool ArmorKitMatchesArmorType(Armor armor) {
			if (_kitType == ArmorKitType.Cloth && armor.ArmorGroup == ArmorType.Cloth) {
				return true;
			} else if (_kitType == ArmorKitType.Leather && armor.ArmorGroup == ArmorType.Leather) {
				return true;
			} else if (_kitType == ArmorKitType.Plate && armor.ArmorGroup == ArmorType.Plate) {
				return true;
			}

			return false;
		}

		private Armor AugmentArmorRating(Armor armor) {
			armor.ArmorRating += KitAugmentAmount;

			return armor;
		}

		private Armor AugmentArmorItemValue(Armor armor) {
			armor.ItemValue += ItemValue;

			return armor;
		}

		public void SetKitAsUsed() {
			KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Armor armor) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {TextInfo.ToTitleCase(armor.Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Armor armor) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {TextInfo.ToTitleCase(armor.Name)} with an armor kit.");
		}
	}
}
