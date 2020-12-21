using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public class StatPotion : IItem, IPotion {
		public enum StatType {
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public PotionStrength _PotionStrength { get; set; }
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public int _StatAmount { get; }
		public StatType _StatType { get; }
		private readonly int _StatEffectDurationInSeconds;

		public StatPotion(PotionStrength potionStrength, StatType statType) {
			_Weight = 1;
			_PotionStrength = potionStrength;
			_StatType = statType;
			_Name = GetPotionName();
			_StatAmount = GetStatPotionAmount();
			_ItemValue = _StatAmount * 10 / 2;
			_Desc = $"A {_Name} that increases {_StatType.ToString().ToLower()} by {_StatAmount}.";
			_StatEffectDurationInSeconds = 600;
		}

		public string GetPotionName() {
			// Potion naming format is "<potion type> potion" for normal potion
			if (_PotionStrength == PotionStrength.Normal) {
				return $"{_StatType.ToString().ToLower()} potion";
			} else {
				// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
				return $"{_PotionStrength.ToString().ToLower()} {_StatType.ToString().ToLower()} potion";
			}
		}

		private int GetStatPotionAmount() {
			if (_PotionStrength == PotionStrength.Minor) {
				return 5;
			} else if (_PotionStrength == PotionStrength.Normal) {
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
			ChangeStatEffect.StatType effectStatCategory = ChangeStatEffect.StatType.Constitution;

			switch (_StatType) {
				case StatType.Constitution:
					player._Constitution += _StatAmount;
					break;
				case StatType.Dexterity:
					player._Dexterity += _StatAmount;
					effectStatCategory = ChangeStatEffect.StatType.Dexterity;
					break;
				case StatType.Intelligence:
					player._Intelligence += _StatAmount;
					effectStatCategory = ChangeStatEffect.StatType.Intelligence;
					break;
				case StatType.Strength:
					player._Strength += _StatAmount;
					effectStatCategory = ChangeStatEffect.StatType.Strength;
					break;
			}

			PlayerController.CalculatePlayerStats(player);

			string effectName = $"{_StatType} (+{_StatAmount})";

			player._Effects.Add(new ChangeStatEffect(effectName, _StatAmount, _StatEffectDurationInSeconds, effectStatCategory));

			return player;
		}

		public void DisplayDrankPotionMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and increased {_StatType} by {_StatAmount}.");
		}
	}
}
