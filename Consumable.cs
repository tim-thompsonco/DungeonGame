using System;

namespace DungeonGame {
	public class Consumable : IEquipment {
		public enum ArrowType {
			Standard
		}
		public enum PotionType {
			Health,
			Mana
		}
		public string Name { get; set; }
		public int ItemValue { get; set; }
		public bool Equipped { get; set; }
		public ArrowType ArrowCategory { get; set; }
		public PotionType PotionCategory { get; set; }
		public RestoreHealth RestoreHealth { get; set; }
		public RestoreMana RestoreMana { get; set; }
		public Arrow Arrow { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() {}

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
					throw new ArgumentOutOfRangeException();
			}
		}
		public Consumable(
			string name,
			int itemValue,
			ArrowType arrowType,
			int amount
		) {
			this.Name = name;
			this.ItemValue = itemValue;
			this.ArrowCategory = arrowType;
			this.Arrow = new Arrow(50);
		}
		
		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}
