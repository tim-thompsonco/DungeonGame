using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class HealthPotion : Potion
	{
		public int _HealthAmount { get; }

		public HealthPotion(int level) : base(level)
		{
			_Name = GetPotionName(level);
			_HealthAmount = GetPotionHealthAmount(level);
			_ItemValue = _HealthAmount / 2;
			_Desc = $"A {_Name} that restores {_HealthAmount} health.";
		}

		protected override string GetPotionName(int level)
		{
			// Potion naming format is "<potion strength> <potion type> potion" for lvl 1-3 or 7+ potions
			if (level <= 3)
			{
				return $"{_PotionStrength.ToString().ToLower()} health potion";
			}
			else if (level > 6)
			{
				return $"{_PotionStrength.ToString().ToLower()} health potion";
			}
			// Potion naming format is "<potion type> potion" for lvl 4-6 potions
			else
			{
				return $"health potion";
			}
		}

		private int GetPotionHealthAmount(int level)
		{
			if (level <= 3)
			{
				return 50;
			}
			else if (level > 6)
			{
				return 150;
			}
			else
			{
				return 100;
			}
		}

		public override void DrinkPotion(Player player)
		{
			RestoreHealthPlayer(player);
			DisplayDrankPotionMessage();
		}

		private Player RestoreHealthPlayer(Player player)
		{
			if (player._HitPoints + _HealthAmount > player._MaxHitPoints)
			{
				player._HitPoints = player._MaxHitPoints;
			}
			else
			{
				player._HitPoints += _HealthAmount;
			}

			return player;
		}

		protected override void DisplayDrankPotionMessage()
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {_HealthAmount} health.");
		}
	}
}
