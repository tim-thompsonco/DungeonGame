using System;

namespace DungeonGame.Items.Consumables
{
	public class Potion : Consumable
	{
		public enum PotionStrength
		{
			Minor,
			Normal,
			Greater
		}
		protected PotionStrength _PotionStrength { get; set; }
		
		public Potion(PotionStrength potionStrength) : base()
		{
			_Weight = 1;
			_PotionStrength = potionStrength;
		}

		protected virtual string GetPotionName()
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