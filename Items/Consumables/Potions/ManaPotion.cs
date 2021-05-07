using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public class ManaPotion : IItem, IPotion {
		public PotionStrength ManaPotionStrength { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public int ManaAmount { get; }

		public ManaPotion(PotionStrength potionStrength) {
			Weight = 1;
			ManaPotionStrength = potionStrength;
			Name = GetPotionName();
			ManaAmount = GetPotionManaAmount();
			ItemValue = ManaAmount / 2;
			Desc = $"A {Name} that restores {ManaAmount} mana.";
		}

		public string GetPotionName() {
			// Potion naming format is "<potion type> potion" for normal potion
			if (ManaPotionStrength == PotionStrength.Normal) {
				return $"mana potion";
			} else {
				// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
				return $"{ManaPotionStrength.ToString().ToLower()} mana potion";
			}
		}

		private int GetPotionManaAmount() {
			if (ManaPotionStrength == PotionStrength.Minor) {
				return 50;
			} else if (ManaPotionStrength == PotionStrength.Normal) {
				return 100;
			} else {
				// If potion strength is not minor or normal, then it is greater
				return 150;
			}
		}

		public void DrinkPotion(Player player) {
			RestoreManaPlayer(player);
			DisplayDrankPotionMessage();
		}

		private Player RestoreManaPlayer(Player player) {
			if (player._ManaPoints + ManaAmount > player._MaxManaPoints) {
				player._ManaPoints = player._MaxManaPoints;
			} else {
				player._ManaPoints += ManaAmount;
			}

			return player;
		}

		public void DisplayDrankPotionMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {ManaAmount} mana.");
		}
	}
}
