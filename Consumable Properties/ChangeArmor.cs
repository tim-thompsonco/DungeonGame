using System;

namespace DungeonGame {
	public class ChangeArmor {
		public enum KitType {
			Cloth,
			Leather,
			Plate
		}
		public int ChangeAmount { get; set; }
		public KitType KitCategory { get; set; }

		public ChangeArmor(int amount, KitType kitType) {
			this.ChangeAmount = amount;
			this.KitCategory = kitType;
		}
		
		public void ChangeArmorPlayer(Armor armor) {
			if (!armor.Equipped) {
				var inputValid = false;
				while (!inputValid) {
					var armorString = armor.Name + " is not equipped. Are you sure you want to upgrade that?";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(), 
						Settings.FormatDefaultBackground(),
						armorString);
					OutputHandler.Display.BuildUserOutput();
					OutputHandler.Display.ClearUserOutput();
					var input = InputHandler.ParseInput(InputHandler.GetFormattedInput(Console.ReadLine()));
					if (input == "no" || input == "n") return;
					if (input == "yes" || input == "y") inputValid = true;
				}
			}
			switch (this.KitCategory) {
				case KitType.Cloth:
					if (armor.ArmorGroup == Armor.ArmorType.Cloth) {
						armor.ArmorRating += this.ChangeAmount;
					}
					return;
				case KitType.Leather:
					if (armor.ArmorGroup == Armor.ArmorType.Leather) {
						armor.ArmorRating += this.ChangeAmount;
					}
					return;
				case KitType.Plate:
					if (armor.ArmorGroup == Armor.ArmorType.Plate) {
						armor.ArmorRating += this.ChangeAmount;
					}
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}