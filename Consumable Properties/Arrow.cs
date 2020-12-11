namespace DungeonGame
{
	public class Arrow
	{
		public int _Quantity { get; set; }

		// Default constructor for JSON serialization
		public Arrow() { }
		public Arrow(int quantity)
		{
			_Quantity = quantity;
		}

		public void LoadArrowsPlayer(Player player)
		{
			if (player._PlayerQuiver == null)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have a quiver to reload!");

				return;
			}

			int amountCanLoad = player._PlayerQuiver._MaxQuantity - player._PlayerQuiver._Quantity;
			
			if (_Quantity < amountCanLoad)
			{
				player._PlayerQuiver._Quantity += _Quantity;
				_Quantity = 0;

				return;
			}

			player._PlayerQuiver._Quantity += amountCanLoad;
			_Quantity -= amountCanLoad;
		}
	}
}