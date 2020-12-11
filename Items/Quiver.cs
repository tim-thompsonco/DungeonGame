namespace DungeonGame
{
	public class Quiver : IEquipment
	{
		public bool _Equipped { get; set; }
		public int _ItemValue { get; set; }
		public int _Quantity { get; set; }
		public int _MaxQuantity { get; set; }
		public int _Weight { get; set; }
		public string _Name { get; set; }
		public string _Desc { get; set; }

		// Default constructor for JSON serialization
		public Quiver() { }
		public Quiver(string name, int quantity, int maxQuantity, int itemValue)
		{
			_Name = name;
			_Quantity = quantity;
			_MaxQuantity = maxQuantity;
			_ItemValue = itemValue;
			_Weight = 1;
			_Desc = $"A {_Name} that can hold {_MaxQuantity} arrows.";
		}

		public bool HaveArrows()
		{
			return _Quantity > 0;
		}
		public void UseArrow()
		{
			_Quantity -= 1;
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