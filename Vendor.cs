using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonGame {
	public class Vendor : IVendor {
		public string Name { get; set; }
		public string Desc { get; set; }
		public string BuySellType { get; set; }
		public List<IEquipment> VendorItems { get; set; } = new List<IEquipment>();
		
		public Vendor(string name, string desc, string buySellType) {
			this.Name = name;
			this.Desc = desc;
			this.BuySellType = buySellType;
			if (name == "armorer") {
				VendorItems.Add(
					new Armor(
						"leather vest", // Name
						Armor.ArmorSlot.Chest, // Armor slot
						18, // Item value
						6, // Low armor rating
						10, // High armor rating
						false // Equipped
						));
				VendorItems.Add(
					new Armor(
						"leather helm", // Name
						Armor.ArmorSlot.Head, // Armor slot
						6, // Item value
						2, // Low armor rating
						5, // High armor rating
						false // Equipped
						));
				VendorItems.Add(
					new Armor(
						"leather leggings", // Name
						Armor.ArmorSlot.Legs, // Armor slot
						10, // Item value
						3, // Low armor rating
						7, // High armor rating
						false // Equipped
						));
			}
		}

		public void DisplayGearForSale(Player player) {
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("The {0} has the following items for sale:\n", this.Name);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IEquipment item in this.VendorItems) {
				var itemInfo = new StringBuilder();
				try {
					itemInfo.Append(item.GetName().ToString());
					if (item.IsEquipped()) {
						itemInfo.Append(" <Equipped>");
					}
				}
				catch (NullReferenceException) { }
				Armor isItemArmor = item as Armor;
				if (isItemArmor != null) {
					itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + " Cost: " + isItemArmor.ItemValue + ")");
				}
				Weapon isItemWeapon = item as Weapon;
				if (isItemWeapon != null) {
					itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + " Cost: " + isItemWeapon.ItemValue + ")");
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				Console.WriteLine(itemName);
			}
		}
		public void BuyItemCheck(Player player, string[] userInput) {
			var inputString = new StringBuilder();
			for (int i = 1; i < userInput.Length; i++) {
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var index = 0;
			index = this.VendorItems.FindIndex(f => f.GetName() == inputName);
			if (index != -1) {
				switch(this.BuySellType) {
					case "Armor":
						Armor buyArmor = this.VendorItems[index] as Armor;
						this.BuyItem(player, userInput, buyArmor, index);
						break;
					case "Weapon":
						Weapon buyWeapon = this.VendorItems[index] as Weapon;
						this.BuyItem(player, userInput, buyWeapon, index);
						break;
					default:
						break;
				}
			}
			else {
				Console.WriteLine("The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Inventory.Add(buyItem);
				this.VendorItems.RemoveAt(index);
				Console.WriteLine("You purchased {0} from the vendor.", buyItem.Name);
			}
		}
		public void SellItemCheck(Player player, string[] userInput) {
			var inputString = new StringBuilder();
			for (int i = 1; i < userInput.Length; i++) {
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var index = 0;
			index = player.Inventory.FindIndex(f => f.GetName() == inputName);
			if (index != -1) {
				switch (this.BuySellType) {
					case "Armor":
						Armor sellArmor = player.Inventory[index] as Armor;
						this.SellItem(player, userInput, sellArmor, index);
						break;
					case "Weapon":
						Weapon sellWeapon = player.Inventory[index] as Weapon;
						this.SellItem(player, userInput, sellWeapon, index);
						break;
					default:
						break;
				}
			}
			else {
				Console.WriteLine("You don't have that to sell!");
			}
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index) {
			if(!sellItem.IsEquipped()) {
				player.Gold += sellItem.ItemValue;
				player.Inventory.RemoveAt(index);
				this.VendorItems.Add(sellItem);
				Console.WriteLine("You sold {0} to the vendor.", sellItem.Name);
				return;
			}
			Console.WriteLine("You have to unequip that first!");
		}
		public string GetName() {
			return this.Name.ToString();
		}
	}
}