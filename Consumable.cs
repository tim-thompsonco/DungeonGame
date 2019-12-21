namespace DungeonGame {
	public class Consumable : IEquipment {
		public enum PotionType {
			Health,
			Mana
		}
		public string Name { get; set; }
		public int ItemValue { get; set; }
		public bool Equipped { get; set; }
		public PotionType PotionCategory { get; set; }
		public RestoreHealth RestoreHealth { get; set; }
		public RestoreMana RestoreMana { get; set; }

		public Consumable(
			string name,
			int itemValue,
			PotionType potionType,
			int amount
			) {
			this.Name = name;
			this.ItemValue = itemValue;
			this.PotionCategory = potionType;
			switch (this.PotionCategory) {
				case PotionType.Health:
					this.RestoreHealth = new RestoreHealth(amount);
					break;
				case PotionType.Mana:
					this.RestoreMana = new RestoreMana(amount);
					break;
				default:
					break;
			}
		}
		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}
