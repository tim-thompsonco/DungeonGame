using System.Globalization;

namespace DungeonGame.Items.Consumables.Kits {
	public enum KitLevel {
		Light,
		Medium,
		Heavy
	}

	public interface IKit {
		int _KitAugmentAmount { get; }
		bool _KitHasBeenUsed { get; set; }
		TextInfo _TextInfo { get; }

		int GetKitAugmentAmount();
		void SetKitAsUsed();
	}
}
