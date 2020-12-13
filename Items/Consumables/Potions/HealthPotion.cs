using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class HealthPotion : Potion
	{
		public int _HealthAmount { get; }

		public HealthPotion(PotionStrength potionStrength) : base(potionStrength)
		{
			_Name = GetPotionName();
			_HealthAmount = GetPotionHealthAmount();
			_ItemValue = _HealthAmount / 2;
			_Desc = $"A {_Name} that restores {_HealthAmount} health.";
		}

		protected override string GetPotionName()
		{
			// Potion naming format is "<potion type> potion" for normal potion
			if (_PotionStrength == PotionStrength.Normal)
			{
				return $"health potion";
			}
			// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
			else
			{
				return $"{_PotionStrength.ToString().ToLower()} health potion";
			}
		}

		private int GetPotionHealthAmount()
		{
			if (_PotionStrength == PotionStrength.Minor)
			{
				return 50;
			}
			else if (_PotionStrength == PotionStrength.Normal)
			{
				return 100;
			}
			else
			{
				return 150;
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
