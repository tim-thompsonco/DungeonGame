using System;

namespace DungeonGame {
  public class Armor : IRoomInteraction, IEquipment {
		public enum ArmorSlot {
			Head,
			Chest,
			Legs
		}
		private static readonly Random RndGenerate = new Random();
    public string Name { get; set; }
		public ArmorSlot ArmorCategory { get; set; }
		public int ItemValue { get; set; }
		public int ArmorRating { get; set; }
		public bool Equipped { get; set; }

		public Armor(
			string name,
			ArmorSlot armorSlot,
			int itemValue,
			int armorRatingLow,
			int armorRatingHigh,
			bool equipped
			) {
			this.Name = name;
			this.ArmorCategory = armorSlot;
			this.ItemValue = itemValue;
			this.ArmorRating = RndGenerate.Next(armorRatingLow, armorRatingHigh);
			this.Equipped = equipped;
		}

		public string GetName() {
      return this.Name;
    }
		public bool IsEquipped() {
			return this.Equipped;
		}
  }
}