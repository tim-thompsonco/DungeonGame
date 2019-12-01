using System;

namespace DungeonGame {
	public class Consumable : IRoomInteraction {
		public enum PotionType {
			Health,
			Mana
		}
		public string name { get; set; }
		public int quantity { get; set; }
		public PotionType _potionType { get; set; }
		public RestoreHealth _restoreHealth { get; set; }

		// Constructor
		public Consumable(string name, PotionType potionType, int amount) {
			this.quantity = 1;
			this.name = name + " (" + this.quantity + ")";
			this._potionType = potionType;
			if (this._potionType == PotionType.Health) {
				this._restoreHealth = new RestoreHealth(amount);
			}
		}
		public string GetName() {
			return this.name;
		}
	}
	public class RestoreHealth {
		public int restoreHealthAmt { get; }

		// Constructor
		public RestoreHealth(int amount) {
			this.restoreHealthAmt = amount;
		}
		public void RestoreHealthPlayer(NewPlayer player) {
			player.hitPoints += restoreHealthAmt;
			if(player.hitPoints > player.maxHitPoints) {
				player.hitPoints = player.maxHitPoints;
			}
		}
	}
}
