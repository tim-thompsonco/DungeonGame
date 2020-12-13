using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class ManaPotion : Potion
	{
		public int _ManaAmount { get; }

		public ManaPotion(PotionStrength potionStrength) : base(potionStrength)
		{
			_Name = GetPotionName();
			_ManaAmount = GetPotionManaAmount();
			_ItemValue = _ManaAmount / 2;
			_Desc = $"A {_Name} that restores {_ManaAmount} mana.";
		}

		protected override string GetPotionName()
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
			else
			{
				return 150;
			}
		}

		public override void DrinkPotion(Player player)
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

		protected override void DisplayDrankPotionMessage()
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				$"You drank a potion and replenished {_ManaAmount} mana.");
		}
	}
}
