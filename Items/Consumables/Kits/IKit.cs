using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public interface IKit {
		int KitAugmentAmount { get; }
		bool KitHasBeenUsed { get; set; }
		TextInfo TextInfo { get; }

		int GetKitAugmentAmount();
		void SetKitAsUsed();
	}
}
