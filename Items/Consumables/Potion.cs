namespace DungeonGame.Items.Consumables
{
	public abstract class Potion : Consumable
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

		protected abstract string GetPotionName();

		public abstract void DrinkPotion(Player player);

		protected abstract void DisplayDrankPotionMessage();
	}
}