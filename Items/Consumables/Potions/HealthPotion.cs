using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions
{
	public class HealthPotion : IItem, IPotion
	{
		public PotionStrength _PotionStrength { get; }
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public int _HealthAmount { get; }

		public HealthPotion(PotionStrength potionStrength)
		{
			_Weight = 1;
			_PotionStrength = potionStrength;
			_Name = GetPotionName();
			_HealthAmount = GetPotionHealthAmount();
			_ItemValue = _HealthAmount / 2;
			_Desc = $"A {_Name} that restores {_HealthAmount} health.";
		}

		public string GetPotionName()
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

		public int GetPotionHealthAmount()
		{
			if (_PotionStrength == PotionStrength.Minor)
			{
				return 50;
			}
			else if (_PotionStrength == PotionStrength.Normal)
			{
				return 100;
			}
			// If potion strength is not minor or normal, then it is greater
			else
			{
				return 150;
			}
		}

		public void DrinkPotion(Player player)
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

		public void DisplayDrankPotionMessage()
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {_HealthAmount} health.");
		}
	}
}
