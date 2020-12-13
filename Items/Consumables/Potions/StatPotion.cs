using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class StatPotion : Potion
	{
		public enum StatType
		{
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public int _StatAmount { get; }
		public StatType _StatCategory { get; }
		private readonly int _StatEffectDurationInSeconds;

		public StatPotion(PotionStrength potionStrength, StatType statType) : base(potionStrength)
		{
			_StatCategory = statType;
			_Name = GetPotionName();
			_StatAmount = GetStatPotionAmount();
			_ItemValue = _StatAmount * 10 / 2;
			_Desc = $"A {_Name} that increases {_StatCategory.ToString().ToLower()} by {_StatAmount}.";
			_StatEffectDurationInSeconds = 600;
		}

		protected override string GetPotionName()
		{
			// Potion naming format is "<potion type> potion" for normal potion
			if (_PotionStrength == PotionStrength.Normal)
			{
				return $"{_StatCategory.ToString().ToLower()} potion";
			}
			// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
			else
			{
				return $"{_PotionStrength.ToString().ToLower()} {_StatCategory.ToString().ToLower()} potion";
			}
		}

		private int GetStatPotionAmount()
		{
			if (_PotionStrength == PotionStrength.Minor)
			{
				return 5;
			}
			else if (_PotionStrength == PotionStrength.Normal)
			{
				return 10;
			}
			// If potion strength is not minor or normal, then it is greater
			else
			{
				return 15;
			}
		}

		public override void DrinkPotion(Player player)
		{
			AugmentPlayerStat(player);
			DisplayDrankPotionMessage();
		}

		private Player AugmentPlayerStat(Player player)
		{
			// Set effectStatCategory to Constitution by default so it is initialized
			Effect.StatType effectStatCategory = Effect.StatType.Constitution;

			switch(_StatCategory)
			{
				case StatType.Constitution:
					player._Constitution += _StatAmount;
					break;
				case StatType.Dexterity:
					player._Dexterity += _StatAmount;
					effectStatCategory = Effect.StatType.Dexterity;
					break;
				case StatType.Intelligence:
					player._Intelligence += _StatAmount;
					effectStatCategory = Effect.StatType.Intelligence;
					break;
				case StatType.Strength:
					player._Strength += _StatAmount;
					effectStatCategory = Effect.StatType.Strength;
					break;
			}

			PlayerController.CalculatePlayerStats(player);

			string effectName = $"{_StatCategory} (+{_StatAmount})";
			player._Effects.Add(
				new Effect(effectName, Effect.EffectType.ChangeStat, _StatAmount,
					1, _StatEffectDurationInSeconds, 1, 1, false, effectStatCategory));

			return player;
		}

		protected override void DisplayDrankPotionMessage()
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and increased {_StatCategory} by {_StatAmount}.");
		}
	}
}
