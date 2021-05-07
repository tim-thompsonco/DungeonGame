using DungeonGame.Controllers;

namespace DungeonGame.Items {
	public class Quiver : IItem {
		public string Desc { get; set; }
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public int MaxQuantity { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public int Weight { get; set; } = 1;

		public Quiver(string name, int maxQuantity, int itemValue) {
			Name = name;
			MaxQuantity = maxQuantity;
			Quantity = MaxQuantity;
			ItemValue = itemValue;
			Desc = $"A {Name} that can hold {MaxQuantity} arrows.";
		}

		public bool HaveArrows() {
			return Quantity > 0;
		}

		public void UseArrow() {
			Quantity -= 1;
		}

		public static void DisplayOutOfArrowsMessage() {
			const string attackFailMessage = "You ran out of arrows! Going hand to hand!";

			OutputController.StoreAttackFailMessage(attackFailMessage);
		}
	}
}