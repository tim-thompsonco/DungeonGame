using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
			switch (name) {
				case "armorer":
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
					break;
				case "weaponsmith":
					VendorItems.Add(
					new Weapon(
						"notched sword", // Name
						11, // Low end of damage value range
						18, // High end of damage value range
						15, // Item value
						1.1, // Crit multiplier
						true // Equipped bool
					));
					break;
				case "healer":
					for (int i = 1; i <= 5; i++) {
						VendorItems.Add(
						new Consumable(
							"minor health potion",
							50,
							Consumable.PotionType.Health,
							50
						));
					}
					for (int i = 1; i <= 5; i++) {
						VendorItems.Add(
						new Consumable(
							"minor mana potion",
							50,
							Consumable.PotionType.Mana,
							50
						));
					}
					break;
				default:
					break;
			}
		}

		public void DisplayGearForSale(Player player) {
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("The {0} has the following items for sale:\n", this.Name);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IEquipment item in this.VendorItems) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName().ToString());
				if (item.IsEquipped()) {
					itemInfo.Append(" <Equipped>");
				}
				Armor isItemArmor = item as Armor;
				if (isItemArmor != null) {
					itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + " Cost: " + isItemArmor.ItemValue + ")");
				}
				Weapon isItemWeapon = item as Weapon;
				if (isItemWeapon != null) {
					itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + " Cost: " + isItemWeapon.ItemValue + ")");
				}
				Consumable isItemConsumable = item as Consumable;
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
			if (this.BuySellType == "Healer") {
				index = this.VendorItems.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName));
			}
			else {
				index = this.VendorItems.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(userInput.Last()));
			}
			if (index != -1) {
				Armor buyArmor = this.VendorItems[index] as Armor;
				Weapon buyWeapon = this.VendorItems[index] as Weapon;
				Consumable buyConsumable = this.VendorItems[index] as Consumable;
				switch (this.BuySellType) {
					case "Armor":
						if (buyArmor != null) {
							this.BuyItem(player, userInput, buyArmor, index);
						}
						break;
					case "Weapon":
						if (buyWeapon != null) {
							this.BuyItem(player, userInput, buyWeapon, index);
						}
						break;
					case "Healer":
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index);
						}
						break;
					case "Shopkeeper":
						if (buyArmor != null) {
							this.BuyItem(player, userInput, buyArmor, index);
						}
						if (buyWeapon != null) {
							this.BuyItem(player, userInput, buyWeapon, index);
						}
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index);
						}
						break;
					default:
						break;
				}
			}
			else {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				if (buyItem.GetType().Name == "Consumable") {
					player.Consumables.Add(buyItem as Consumable);
				}
				else {
					player.Inventory.Add(buyItem);
				}
				this.VendorItems.RemoveAt(index);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("You purchased {0} from the vendor for {1} gold.", buyItem.Name, buyItem.ItemValue);
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
			index = player.Inventory.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(userInput.Last()) && f.IsEquipped() == false);
			if (index != -1) {
				Armor sellArmor = player.Inventory[index] as Armor;
				Weapon sellWeapon = player.Inventory[index] as Weapon;
				Consumable sellConsumable = player.Inventory[index] as Consumable;
				Loot sellLoot = player.Inventory[index] as Loot;
				switch (this.BuySellType) {
					case "Armor":
						if (sellArmor != null) {
							this.SellItem(player, userInput, sellArmor, index);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Weapon":
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, index);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Healer":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, index);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Shopkeeper":
						if (sellArmor != null) {
							this.SellItem(player, userInput, sellArmor, index);
						}
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, index);
						}
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, index);
						}
						if (sellLoot != null) {
							this.SellItem(player, userInput, sellLoot, index);
						}
						break;
					default:
						break;
				}
			}
			else {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("You don't have that to sell!");
			}
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index) {
			Console.ForegroundColor = ConsoleColor.Green;
			if (!sellItem.IsEquipped()) {
				player.Gold += sellItem.ItemValue;
				player.Inventory.RemoveAt(index);
				this.VendorItems.Add(sellItem);
				Console.WriteLine("You sold {0} to the vendor for {1} gold.", sellItem.Name, sellItem.ItemValue);
				return;
			}
			Console.WriteLine("You have to unequip that first!");
		}
		public void RepairItem(Player player, string[] userInput) {
			var inputString = new StringBuilder();
			for (int i = 1; i < userInput.Length; i++) {
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var index = 0;
			index = player.Inventory.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(userInput.Last()));
			if (index != -1) {
				Console.ForegroundColor = ConsoleColor.Green;
				switch (this.BuySellType) {
					case "Armor":
						Armor repairArmor = player.Inventory[index] as Armor;
						if (repairArmor != null && repairArmor.IsEquipped()) {
							var durabilityRepairArmor = 100 - repairArmor.Durability;
							var repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							Console.ForegroundColor = ConsoleColor.Green;
							if (player.Gold >= (int)repairCostArmor) {
								player.Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								Console.WriteLine("Your {0} has been repaired for {1} gold.", repairArmor.Name, (int)repairCostArmor);
								break;
							}
							Console.WriteLine("You can't afford to repair {0}", repairArmor.Name);
						}
						Console.WriteLine("The vendor doesn't repair that type of equipment.");
						break;
					case "Weapon":
						Weapon repairWeapon = player.Inventory[index] as Weapon;
						if (repairWeapon != null && repairWeapon.IsEquipped()) {
							var durabilityRepairWeapon = 100 - repairWeapon.Durability;
							var repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player.Gold >= repairCostWeapon) {
								player.Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								Console.WriteLine("Your {0} has been repaired for {1} gold.", repairWeapon.Name, (int)repairCostWeapon);
								break;
							}
							Console.WriteLine("You can't afford to repair {0}", repairWeapon.Name);
						}
						Console.WriteLine("The vendor doesn't repair that type of equipment.");
						break;
					case "Healer":
						Console.WriteLine("Healers don't repair equipment.");
						break;
					default:
						break;
				}
			}
		}
		public string GetName() {
			return this.Name.ToString();
		}
		public void HealPlayer(Player player) {
			Console.ForegroundColor = ConsoleColor.Green;
			if (this.BuySellType == "Healer") {
				player.HitPoints = player.MaxHitPoints;
				Console.WriteLine("You have been restored to full health by the {0}.", this.Name);
				return;
			}
			Console.WriteLine("The {0} cannot heal you!", this.Name);
		}
	}
}