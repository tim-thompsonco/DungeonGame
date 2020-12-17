using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Kits
{
	public class ArmorKit : Kit
	{
		public enum KitType
		{
			Cloth,
			Leather,
			Plate
		}
		private readonly KitType _KitType;

		public ArmorKit(KitLevel kitLevel, KitType kitType) : base(kitLevel)
		{
			_KitType = kitType;
			_Name = $"{kitLevel.ToString().ToLower()} {_KitType.ToString().ToLower()} armor kit";
			_Desc = $"A single-use {_Name} that increases armor rating by {_KitAugmentAmount} for one armor item.";
		}

		public Armor AttemptAugmentArmorPlayer(Armor armor)
		{
			if (ArmorKitMatchesArmorType(armor))
			{
				AugmentArmorRating(armor);
				AugmentArmorItemValue(armor);
				SetKitAsUsed();
				DisplayAugmentSuccessMessage(armor);
			}
			else
			{
				DisplayAugmentFailMessage(armor);
			}

			return armor;
		}

		private bool ArmorKitMatchesArmorType(Armor armor)
		{
			if (_KitType == KitType.Cloth && armor._ArmorGroup == Armor.ArmorType.Cloth)
			{
				return true;
			}
			else if (_KitType == KitType.Leather && armor._ArmorGroup == Armor.ArmorType.Leather)
			{
				return true;
			}
			else if (_KitType == KitType.Plate && armor._ArmorGroup == Armor.ArmorType.Plate)
			{
				return true;
			}

			return false;
		}

		private Armor AugmentArmorRating(Armor armor)
		{
			armor._ArmorRating += _KitAugmentAmount;

			return armor;
		}

		private Armor AugmentArmorItemValue(Armor armor)
		{
			armor._ItemValue += _ItemValue;

			return armor;
		}

		protected override void SetKitAsUsed()
		{
			_KitHasBeenUsed = true;
		}

		private void DisplayAugmentFailMessage(Armor armor)
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				$"You can't upgrade {_TextInfo.ToTitleCase(armor._Name)} with that!");
		}

		private void DisplayAugmentSuccessMessage(Armor armor)
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You upgraded {_TextInfo.ToTitleCase(armor._Name)} with an armor kit.");
		}
	}
}
