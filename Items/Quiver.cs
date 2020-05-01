namespace DungeonGame {
	public class Quiver : IEquipment {
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public int Quantity { get; set; }
		public int MaxQuantity { get; set; }
		public int Weight { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public bool IsRainbowGear { get; set; }

		// Default constructor for JSON serialization
		public Quiver() { }
		public Quiver(string name, int quantity, int maxQuantity, int itemValue) {
			this.Name = name;
			this.Quantity = quantity;
			this.MaxQuantity = maxQuantity;
			this.ItemValue = itemValue;
			this.Weight = 1;
			this.Desc = "A " + this.Name + " that can hold " + this.MaxQuantity + " arrows.";
		}
		
		public bool HaveArrows() {
			return this.Quantity > 0;
		}
		public void UseArrow() {
			this.Quantity -= 1;
		}
		public static void OutOfArrows() {
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackFailText(),
				Settings.FormatDefaultBackground(),
				"You ran out of arrows! Going hand to hand!");
		}
	}
}