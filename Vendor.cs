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
						false // Equipped bool
					));
					break;
				case "healer":
					VendorItems.Add(
					new Consumable(
						"minor health potion",
						50,
						Consumable.PotionType.Health,
						50
					));
					VendorItems.Add(
					new Consumable(
						"minor mana potion",
						50,
						Consumable.PotionType.Mana,
						50
					));
					break;
				default:
					break;
			}
		}

		public void DisplayGearForSale(Player player) {
			Helper.FormatInfoText();
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
				if (isItemConsumable != null) {
					itemInfo.Append(" (Cost: " + isItemConsumable.ItemValue + ")");
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				Console.WriteLine(itemName);
			}
		}
		public void BuyItemCheck(Player player, string[] userInput) {
			var inputName = Helper.ParseInput(userInput);
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
							this.BuyItem(player, userInput, buyConsumable, index, inputName);
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
				Helper.FormatFailureOutputText();
				Console.WriteLine("The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index) {
			Helper.FormatSuccessOutputText();
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Inventory.Add(buyItem);
				this.VendorItems.RemoveAt(index);
				Console.WriteLine("You purchased {0} from the vendor for {1} gold.", buyItem.Name, buyItem.ItemValue);
				return;
			}
			Console.WriteLine("You can't afford that!");
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index, string inputName) {
			Helper.FormatSuccessOutputText();
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Consumables.Add(buyItem as Consumable);
				this.VendorItems.RemoveAt(index);
				Console.WriteLine("You purchased {0} from the vendor for {1} gold.", buyItem.Name, buyItem.ItemValue);
				this.RepopulateHealerPotion(inputName);
				return;
			}
			Console.WriteLine("You can't afford that!");
		}
		public void SellItemCheck(Player player, string[] userInput) {
			var inputName = Helper.ParseInput(userInput);
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
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that to sell!");
			}
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index) {
			Helper.FormatSuccessOutputText();
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
			var parsedInput = Helper.ParseInput(userInput);
			var index = 0;
			index = player.Inventory.FindIndex(f => f.GetName() == parsedInput || f.GetName().Contains(userInput.Last()));
			if (index != -1) {
				switch (this.BuySellType) {
					case "Armor":
						Armor repairArmor = player.Inventory[index] as Armor;
						if (repairArmor != null && repairArmor.IsEquipped()) {
							var durabilityRepairArmor = 100 - repairArmor.Durability;
							var repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							if (player.Gold >= (int)repairCostArmor) {
								player.Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								Helper.FormatSuccessOutputText();
								Console.WriteLine("Your {0} has been repaired for {1} gold.", repairArmor.Name, (int)repairCostArmor);
								break;
							}
							Helper.FormatFailureOutputText();
							Console.WriteLine("You can't afford to repair {0}", repairArmor.Name);
						}
						Helper.FormatFailureOutputText();
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
								Helper.FormatSuccessOutputText();
								Console.WriteLine("Your {0} has been repaired for {1} gold.", repairWeapon.Name, (int)repairCostWeapon);
								break;
							}
							Helper.FormatFailureOutputText();
							Console.WriteLine("You can't afford to repair {0}", repairWeapon.Name);
						}
						Helper.FormatFailureOutputText();
						Console.WriteLine("The vendor doesn't repair that type of equipment.");
						break;
					case "Healer":
					case "Shopkeeper":
						Helper.FormatFailureOutputText();
						Console.WriteLine("{0}s don't repair equipment.", this.BuySellType);
						break;
					default:
						break;
				}
				return;
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("That item is not in your inventory.");
		}
		public string GetName() {
			return this.Name.ToString();
		}
		public void HealPlayer(Player player) {
			if (this.BuySellType == "Healer") {
				player.HitPoints = player.MaxHitPoints;
				Helper.FormatSuccessOutputText();
				Console.WriteLine("You have been restored to full health by the {0}.", this.Name);
				return;
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("The {0} cannot heal you!", this.Name);
		}
		public void RepopulateHealerPotion(string inputName) {
			var potionIndex = this.VendorItems.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (potionIndex == -1) {
				if (inputName.Contains("mana")) {
					VendorItems.Add(
					new Consumable(
						"minor mana potion",
						50,
						Consumable.PotionType.Mana,
						50
					));
				}
				else if (inputName.Contains("health")) {
					VendorItems.Add(
					new Consumable(
						"minor health potion",
						50,
						Consumable.PotionType.Health,
						50
					));
				}
			}
		}
	}
}