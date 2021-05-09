using DungeonGame.Effects;
using DungeonGame.Helpers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public class StatPotion : IItem, IPotion {
		public PotionStrength PotionStrength { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public int StatAmount { get; }
		public StatType StatPotionType { get; }
		private readonly int _statEffectDurationInSeconds;

		public StatPotion(PotionStrength potionStrength, StatType statType) {
			Weight = 1;
			PotionStrength = potionStrength;
			StatPotionType = statType;
			Name = GetPotionName();
			StatAmount = GetStatPotionAmount();
			ItemValue = StatAmount * 10 / 2;
			Desc = $"A {Name} that increases {StatPotionType.ToString().ToLower()} by {StatAmount}.";
			_statEffectDurationInSeconds = 600;
		}

		public string GetPotionName() {
			// Potion naming format is "<potion type> potion" for normal potion
			if (PotionStrength == PotionStrength.Normal) {
				return $"{StatPotionType.ToString().ToLower()} potion";
			} else {
				// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
				return $"{PotionStrength.ToString().ToLower()} {StatPotionType.ToString().ToLower()} potion";
			}
		}

		private int GetStatPotionAmount() {
			if (PotionStrength == PotionStrength.Minor) {
				return 5;
			} else if (PotionStrength == PotionStrength.Normal) {
				return 10;
			} else {
				// If potion strength is not minor or normal, then it is greater
				return 15;
			}
		}

		public void DrinkPotion(Player player) {
			AugmentPlayerStat(player);
			DisplayDrankPotionMessage();
		}

		private Player AugmentPlayerStat(Player player) {
			// Set effectStatCategory to Constitution by default so it is initialized
			StatType effectStatCategory = StatType.Constitution;

			switch (StatPotionType) {
				case StatType.Constitution:
					player.Constitution += StatAmount;
					break;
				case StatType.Dexterity:
					player.Dexterity += StatAmount;
					effectStatCategory = StatType.Dexterity;
					break;
				case StatType.Intelligence:
					player.Intelligence += StatAmount;
					effectStatCategory = StatType.Intelligence;
					break;
				case StatType.Strength:
					player.Strength += StatAmount;
					effectStatCategory = StatType.Strength;
					break;
			}

			PlayerHelper.CalculatePlayerStats(player);

			string effectName = $"{StatPotionType} (+{StatAmount})";

			player.Effects.Add(new ChangeStatEffect(effectName, _statEffectDurationInSeconds, effectStatCategory, StatAmount));

			return player;
		}

		public void DisplayDrankPotionMessage() {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and increased {StatPotionType} by {StatAmount}.");
		}
	}
}
