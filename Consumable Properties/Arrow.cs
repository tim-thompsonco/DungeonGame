using System;

namespace DungeonGame {
	public class Arrow {
		public int Quantity { get; set; }

		public Arrow(int quantity) {
			this.Quantity = quantity;
		}

		public void LoadArrowsPlayer(Player player) {
			if (player.PlayerQuiver == null) {
				Console.WriteLine("You don't have a quiver to reload!");
				return;
			}
			var amountCanLoad = player.PlayerQuiver.MaxQuantity - player.PlayerQuiver.Quantity;
			if (this.Quantity < amountCanLoad) {
				player.PlayerQuiver.Quantity += this.Quantity;
				this.Quantity = 0;
			}
			player.PlayerQuiver.Quantity += amountCanLoad;
			this.Quantity -= amountCanLoad;
		}
	}
}