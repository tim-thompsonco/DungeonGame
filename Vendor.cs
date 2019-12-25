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
						"leather vest",
						Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Leather, 
						12, 
						7,
						12,
						false
					));
					VendorItems.Add(
					new Armor(
						"leather helm",
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Leather, 
						6, 
						3,
						6,
						false
					));
					VendorItems.Add(
					new Armor(
						"leather leggings",
						Armor.ArmorSlot.Legs, 
						Armor.ArmorType.Leather, 
						10, 
						5,
						10,
						false
					));
					break;
				case "weaponsmith":
					VendorItems.Add(
					new Weapon(
						"notched sword",
						13, 
						20, 
						20, 
						1.1, 
						false 
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
			foreach (var item in this.VendorItems) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName().ToString());
				if (item.IsEquipped()) {
					itemInfo.Append(" <Equipped>");
				}
				var isItemArmor = item as Armor;
				if (isItemArmor != null) {
					itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + " Cost: " + isItemArmor.ItemValue + ")");
				}
				var isItemWeapon = item as Weapon;
				if (isItemWeapon != null) {
					itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + " Cost: " + isItemWeapon.ItemValue + ")");
				}
				var isItemConsumable = item as Consumable;
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
				var buyArmor = this.VendorItems[index] as Armor;
				var buyWeapon = this.VendorItems[index] as Weapon;
				var buyConsumable = this.VendorItems[index] as Consumable;
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
			var invIndex = player.Inventory.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName) && f.IsEquipped() == false);
			var conIndex = player.Consumables.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName) && f.IsEquipped() == false);
			if (conIndex != -1) {
				var sellConsumable = player.Consumables[conIndex] as Consumable;
				switch (this.BuySellType) {
					case "Healer":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, conIndex);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Shopkeeper":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, conIndex);
						}
						break;
				}
			}
			if (invIndex != -1) {
				var sellArmor = player.Inventory[invIndex] as Armor;
				var sellWeapon = player.Inventory[invIndex] as Weapon;
				var sellLoot = player.Inventory[invIndex] as Loot;
				switch (this.BuySellType) {
					case "Armor":
						if (sellArmor != null) {
							this.SellItem(player, userInput, sellArmor, invIndex);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Weapon":
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, invIndex);
							break;
						}
						Helper.InvalidVendorSell();
						break;
					case "Shopkeeper":
						if (sellArmor != null) {
							this.SellItem(player, userInput, sellArmor, invIndex);
						}
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, invIndex);
						}
						if (sellLoot != null) {
							this.SellItem(player, userInput, sellLoot, invIndex);
						}
						break;
				}
			}
			if (invIndex == -1 && conIndex == -1) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that to sell!");
			}
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index) {
			Helper.FormatSuccessOutputText();
			if (!sellItem.IsEquipped()) {
				player.Gold += sellItem.ItemValue;
				if (sellItem.GetType().Name == "Consumable") {
					player.Consumables.RemoveAt(index);
				}
				else {
					player.Inventory.RemoveAt(index);
				}
				this.VendorItems.Add(sellItem);
				Console.WriteLine("You sold {0} to the vendor for {1} gold.", sellItem.Name, sellItem.ItemValue);
				return;
			}
			Console.WriteLine("You have to unequip that first!");
		}
		public void RepairItem(Player player, string[] userInput) {
			var parsedInput = Helper.ParseInput(userInput);
			var index = player.Inventory.FindIndex(f => f.GetName() == parsedInput || f.GetName().Contains(userInput.Last()));
			if (index != -1) {
				switch (this.BuySellType) {
					case "Armor":
						var repairArmor = player.Inventory[index] as Armor;
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
						var repairWeapon = player.Inventory[index] as Weapon;
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
		public void RestorePlayer(Player player) {
			if (this.BuySellType == "Healer") {
				player.HitPoints = player.MaxHitPoints;
				player.RagePoints = player.MaxRagePoints;
				player.ManaPoints = player.MaxManaPoints;
				Helper.FormatSuccessOutputText();
				Console.WriteLine("You have been restored by the {0}.", this.Name);
				return;
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("The {0} cannot restore you!", this.Name);
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