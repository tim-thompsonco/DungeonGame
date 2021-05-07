using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public class HealthPotion : IItem, IPotion {
		public PotionStrength HealthPotionStrength { get; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public int HealthAmount { get; }

		public HealthPotion(PotionStrength potionStrength) {
			Weight = 1;
			HealthPotionStrength = potionStrength;
			Name = GetPotionName();
			HealthAmount = GetPotionHealthAmount();
			ItemValue = HealthAmount / 2;
			Desc = $"A {Name} that restores {HealthAmount} health.";
		}

		public string GetPotionName() {
			// Potion naming format is "<potion type> potion" for normal potion
			if (HealthPotionStrength == PotionStrength.Normal) {
				return $"health potion";
			} else {
				// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
				return $"{HealthPotionStrength.ToString().ToLower()} health potion";
			}
		}

		public int GetPotionHealthAmount() {
			if (HealthPotionStrength == PotionStrength.Minor) {
				return 50;
			} else if (HealthPotionStrength == PotionStrength.Normal) {
				return 100;
			} else {
				// If potion strength is not minor or normal, then it is greater
				return 150;
			}
		}

		public void DrinkPotion(Player player) {
			RestoreHealthPlayer(player);
			DisplayDrankPotionMessage();
		}

		private Player RestoreHealthPlayer(Player player) {
			if (player._HitPoints + HealthAmount > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			} else {
				player._HitPoints += HealthAmount;
			}

			return player;
		}

		public void DisplayDrankPotionMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {HealthAmount} health.");
		}
	}
}
