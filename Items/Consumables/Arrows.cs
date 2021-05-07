using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables {
	public class Arrows : IItem {
		public enum ArrowType {
			Standard
		}
		public ArrowType ArrowCategory { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public int Weight { get; set; }
		public int Quantity { get; set; }

		public Arrows(string name, int itemValue, ArrowType arrowType) : base() {
			Name = name;
			ItemValue = itemValue;
			ArrowCategory = arrowType;
			Weight = 1;
			Quantity = 50;
			Desc = $"A bundle of {Quantity} arrows.";
		}

		public void LoadPlayerQuiverWithArrows(Player player) {
			if (player._PlayerQuiver == null) {
				DisplayPlayerHasNoQuiverMessage();
			} else {
				LoadQuiverWithArrows(player);
			}
		}

		private void DisplayPlayerHasNoQuiverMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have a quiver to reload!");
		}

		private void LoadQuiverWithArrows(Player player) {
			int arrowsToLoadToQuiver = player._PlayerQuiver._MaxQuantity - player._PlayerQuiver._Quantity;

			if (Quantity <= arrowsToLoadToQuiver) {
				LoadQuiverWithAllArrows(player);
			} else {
				LoadQuiverWithSomeArrows(player, arrowsToLoadToQuiver);
			}
		}

		private void LoadQuiverWithAllArrows(Player player) {
			player._PlayerQuiver._Quantity += Quantity;
			Quantity = 0;
		}

		private void LoadQuiverWithSomeArrows(Player player, int arrowsToLoad) {
			player._PlayerQuiver._Quantity += arrowsToLoad;
			Quantity -= arrowsToLoad;
		}
	}
}
