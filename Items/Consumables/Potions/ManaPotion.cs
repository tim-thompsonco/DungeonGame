using DungeonGame.Controllers;

namespace DungeonGame.Items.Consumables.Potions
{
	public class ManaPotion : Potion
	{
		public int _ManaAmount { get; }

		public ManaPotion(int level) : base(level)
		{
			_Name = GetPotionName(level);
			_ManaAmount = GetPotionManaAmount(level);
			_ItemValue = _ManaAmount / 2;
			_Desc = $"A {_Name} that restores {_ManaAmount} mana.";
		}

		protected override string GetPotionName(int level)
		{
			// Potion naming format is "<potion strength> <potion type> potion" for lvl 1-3 or 7+ potions
			if (level <= 3)
			{
				return $"{_PotionStrength.ToString().ToLower()} mana potion";
			}
			else if (level > 6)
			{
				return $"{_PotionStrength.ToString().ToLower()} mana potion";
			}
			// Potion naming format is "<potion type> potion" for lvl 4-6 potions
			else
			{
				return $"mana potion";
			}
		}

		private int GetPotionManaAmount(int level)
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
