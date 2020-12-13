using System;

namespace DungeonGame.Items.Consumables
{
	public class Potion : Consumable
	{
		protected enum PotionStrength
		{
			Minor,
			Normal,
			Greater
		}
		protected PotionStrength _PotionStrength { get; set; }
		
		public Potion(int level) : base()
		{
			_Weight = 1;
			_PotionStrength = GetPotionStrength(level);
		}

		private PotionStrength GetPotionStrength(int level)
		{
			if (level <= 3)
			{
				return PotionStrength.Minor;
			}
			else if (level > 6)
			{
				return PotionStrength.Greater;
			}
			else
			{
				return PotionStrength.Normal;
			}
		}

		protected virtual string GetPotionName(int level)
		{
			throw new NotImplementedException("Potion name has not been implemented.");
		}

		public virtual void DrinkPotion(Player player)
		{
			throw new NotImplementedException("Drink potion has not been implemented.");
		}

		protected virtual void DisplayDrankPotionMessage()
		{
			throw new NotImplementedException("Drink potion message has not been implemented.");
		}
	}
}