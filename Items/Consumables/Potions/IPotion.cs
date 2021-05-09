using DungeonGame.Players;

namespace DungeonGame.Items.Consumables.Potions {
	public interface IPotion {
		string GetPotionName();
		void DrinkPotion(Player player);
		void DisplayDrankPotionMessage();
	}
}
