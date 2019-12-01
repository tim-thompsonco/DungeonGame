using System;

namespace DungeonGame {
	public class Consumable : IRoomInteraction {
		public enum PotionType {
			Health,
			Mana
		}
		public string Name { get; set; }
		public int Quantity { get; set; }
		public int ItemValue { get; }
		public PotionType PotionCategory { get; set; }
		public RestoreHealth RestoreHealth { get; set; }

		public Consumable(string name, int itemValue, PotionType potionType, int amount) {
			this.Quantity = 1;
			this.Name = name + " (" + this.Quantity + ")";
			this.ItemValue = ItemValue;
			this.PotionCategory = potionType;
			if (this.PotionCategory == PotionType.Health) {
				this.RestoreHealth = new RestoreHealth(amount);
			}
		}
		public string GetName() {
			return this.Name;
		}
	}
	public class RestoreHealth {
		public int restoreHealthAmt { get; }

		public RestoreHealth(int amount) {
			this.restoreHealthAmt = amount;
		}
		public void RestoreHealthPlayer(NewPlayer player) {
			player.HitPoints += restoreHealthAmt;
			if(player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
		}
	}
}
