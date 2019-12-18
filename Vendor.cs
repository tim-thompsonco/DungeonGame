using System.Collections.Generic;

namespace DungeonGame {
	public class Vendor : IVendor {
		public string Name { get; set; }
		public string Desc { get; set; }
		public List<IEquipment> VendorItems { get; set; } = new List<IEquipment>();
		
		public Vendor(string name, string desc) {
			this.Name = name;
			this.Desc = desc;
			if (name == "armorer") {
				VendorItems.Add(
					new Armor(
						"Leather Vest", // Name
						Armor.ArmorSlot.Chest, // Armor slot
						18, // Item value
						6, // Low armor rating
						10, // High armor rating
						false // Equipped
						));
				VendorItems.Add(
					new Armor(
						"Leather Helm", // Name
						Armor.ArmorSlot.Head, // Armor slot
						6, // Item value
						2, // Low armor rating
						5, // High armor rating
						false // Equipped
						));
				VendorItems.Add(
					new Armor(
						"Leather Leggings", // Name
						Armor.ArmorSlot.Legs, // Armor slot
						10, // Item value
						3, // Low armor rating
						7, // High armor rating
						false // Equipped
						));
			}
		}

		public void DisplayGearForSale() { }
		public string GetName() {
			return this.Name.ToString();
		}
	}
}