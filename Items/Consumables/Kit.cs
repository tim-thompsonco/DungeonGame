using System.Globalization;

namespace DungeonGame.Items.Consumables
{
	public abstract class Kit : Consumable
	{
		public enum KitLevel
		{
			Light,
			Medium,
			Heavy
		}
		public int _KitAugmentAmount { get; set; }
		public bool _KitHasBeenUsed { get; set; }
		protected KitLevel _KitLevel { get; set; }
		protected readonly TextInfo _TextInfo = new CultureInfo("en-US", false).TextInfo;

		public Kit(KitLevel kitLevel)
		{
			_KitLevel = kitLevel;
			_Weight = 1;
			_KitAugmentAmount = GetKitAugmentAmount();
			_ItemValue = _KitAugmentAmount * 10;
		}

		private int GetKitAugmentAmount()
		{
			if (_KitLevel == KitLevel.Light)
			{
				return 1;
			}
			else if (_KitLevel == KitLevel.Medium)
			{
				return 2;
			}
			// If kit level is not light or medium, then it is heavy
			else
			{
				return 3;
			}
		}

		protected abstract void SetKitAsUsed();
	}
}
