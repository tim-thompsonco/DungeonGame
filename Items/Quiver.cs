namespace DungeonGame
{
	public class Quiver : IEquipment
	{
		public bool _Equipped { get; set; }
		public int _ItemValue { get; set; }
		public int Quantity { get; set; }
		public int MaxQuantity { get; set; }
		public int _Weight { get; set; }
		public string _Name { get; set; }
		public string _Desc { get; set; }

		// Default constructor for JSON serialization
		public Quiver() { }
		public Quiver(string name, int quantity, int maxQuantity, int itemValue)
		{
			this._Name = name;
			this.Quantity = quantity;
			this.MaxQuantity = maxQuantity;
			this._ItemValue = itemValue;
			this._Weight = 1;
			this._Desc = "A " + this._Name + " that can hold " + this.MaxQuantity + " arrows.";
		}

		public bool HaveArrows()
		{
			return this.Quantity > 0;
		}
		public void UseArrow()
		{
			this.Quantity -= 1;
		}
		public static void OutOfArrows()
		{
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackFailText(),
				Settings.FormatDefaultBackground(),
				"You ran out of arrows! Going hand to hand!");
		}
	}
}