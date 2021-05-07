using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Items.Consumables {
	public class Arrows : IItem {
		public enum ArrowType {
			Standard
		}
		public ArrowType _ArrowCategory { get; set; }
		public string Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }

		public int _Quantity { get; set; }

		public Arrows(string name, int itemValue, ArrowType arrowType) : base() {
			Name = name;
			_ItemValue = itemValue;
			_ArrowCategory = arrowType;
			_Weight = 1;
			_Quantity = 50;
			_Desc = $"A bundle of {_Quantity} arrows.";
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

			if (_Quantity <= arrowsToLoadToQuiver) {
				LoadQuiverWithAllArrows(player);
			} else {
				LoadQuiverWithSomeArrows(player, arrowsToLoadToQuiver);
			}
		}

		private void LoadQuiverWithAllArrows(Player player) {
			player._PlayerQuiver._Quantity += _Quantity;
			_Quantity = 0;
		}

		private void LoadQuiverWithSomeArrows(Player player, int arrowsToLoad) {
			player._PlayerQuiver._Quantity += arrowsToLoad;
			_Quantity -= arrowsToLoad;
		}
	}
}
