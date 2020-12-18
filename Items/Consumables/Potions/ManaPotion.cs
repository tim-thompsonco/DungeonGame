using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class ManaPotion : IItem, IPotion
	{
		public PotionStrength _PotionStrength { get; set; }
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public int _ManaAmount { get; }

		public ManaPotion(PotionStrength potionStrength)
		{
			_Weight = 1;
			_PotionStrength = potionStrength;
			_Name = GetPotionName();
			_ManaAmount = GetPotionManaAmount();
			_ItemValue = _ManaAmount / 2;
			_Desc = $"A {_Name} that restores {_ManaAmount} mana.";
		}

		public string GetPotionName()
		{
			// Potion naming format is "<potion type> potion" for normal potion
			if (_PotionStrength == PotionStrength.Normal)
			{
				return $"mana potion";
			}
			// Potion naming format is "<potion strength> <potion type> potion" for minor or greater potions
			else
			{
				return $"{_PotionStrength.ToString().ToLower()} mana potion";
			}
		}

		private int GetPotionManaAmount()
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
			RestoreManaPlayer(player);
			DisplayDrankPotionMessage();
		}

		private Player RestoreManaPlayer(Player player)
		{
			if (player._ManaPoints + _ManaAmount > player._MaxManaPoints)
			{
				player._ManaPoints = player._MaxManaPoints;
			}
			else
			{
				player._ManaPoints += _ManaAmount;
			}

			return player;
		}

		public void DisplayDrankPotionMessage()
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {_ManaAmount} mana.");
		}
	}
}
