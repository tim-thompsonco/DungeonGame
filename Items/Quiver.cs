using DungeonGame.Controllers;

namespace DungeonGame.Items {
	public class Quiver : IItem {
		public bool _Equipped { get; set; }
		public int ItemValue { get; set; }
		public int _Quantity { get; set; }
		public int _MaxQuantity { get; set; }
		public int Weight { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }

		public Quiver(string name, int maxQuantity, int itemValue) {
			Name = name;
			_MaxQuantity = maxQuantity;
			_Quantity = _MaxQuantity;
			ItemValue = itemValue;
			Weight = 1;
			Desc = $"A {Name} that can hold {_MaxQuantity} arrows.";
		}

		public bool HaveArrows() {
			return _Quantity > 0;
		}

		public void UseArrow() {
			_Quantity -= 1;
		}

		public static void DisplayOutOfArrowsMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackFailText(),
				Settings.FormatDefaultBackground(),
				"You ran out of arrows! Going hand to hand!");
		}
	}
}