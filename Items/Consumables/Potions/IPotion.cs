using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public enum PotionStrength {
		Minor,
		Normal,
		Greater
	}

	public interface IPotion {
		string GetPotionName();
		void DrinkPotion(Player player);
		void DisplayDrankPotionMessage();
	}
}
