using DungeonGame.Controllers;
using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public class ArmorKit : IItem, IKit {
		public enum KitType {
			Cloth,
			Leather,
			Plate
		}
		public string Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public bool _KitHasBeenUsed { get; set; }
		public KitLevel _KitLevel { get; }
		public int _KitAugmentAmount { get; set; }
		public TextInfo _TextInfo { get; set; }
		private readonly KitType _KitType;

		public ArmorKit(KitLevel kitLevel, KitType kitType) {
			_KitLevel = kitLevel;
			_KitType = kitType;
			_TextInfo = new CultureInfo("en-US", false).TextInfo;
			_Weight = 1;
			_KitAugmentAmount = GetKitAugmentAmount();
			_ItemValue = _KitAugmentAmount * 10;
			Name = $"{kitLevel.ToString().ToLower()} {_KitType.ToString().ToLower()} armor kit";
			_Desc = $"A single-use {Name} that increases armor rating by {_KitAugmentAmount} for one armor item.";
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
			if (_KitType == KitType.Cloth && armor._ArmorGroup == Armor.ArmorType.Cloth) {
				return true;
			} else if (_KitType == KitType.Leather && armor._ArmorGroup == Armor.ArmorType.Leather) {
				return true;
			} else if (_KitType == KitType.Plate && armor._ArmorGroup == Armor.ArmorType.Plate) {
				return true;
			}

			return false;
		}

		private Armor AugmentArmorRating(Armor armor) {
			armor._ArmorRating += _KitAugmentAmount;

			return armor;
		}

		private Armor AugmentArmorItemValue(Armor armor) {
			armor._ItemValue += _ItemValue;

			return armor;
		}

		public void SetKitAsUsed() {
			_KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Armor armor) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {_TextInfo.ToTitleCase(armor.Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Armor armor) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {_TextInfo.ToTitleCase(armor.Name)} with an armor kit.");
		}
	}
}
