namespace DungeonGame
{
	public class Arrow
	{
		public int Quantity { get; set; }

		// Default constructor for JSON serialization
		public Arrow() { }
		public Arrow(int quantity)
		{
			this.Quantity = quantity;
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
			var amountCanLoad = player._PlayerQuiver.MaxQuantity - player._PlayerQuiver.Quantity;
			if (this.Quantity < amountCanLoad)
			{
				player._PlayerQuiver.Quantity += this.Quantity;
				this.Quantity = 0;
				return;
			}
			player._PlayerQuiver.Quantity += amountCanLoad;
			this.Quantity -= amountCanLoad;
		}
	}
}