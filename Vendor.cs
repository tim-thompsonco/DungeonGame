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
					this.VendorItems.Add(
					new Armor(
						"leather vest",
						Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Leather, 
						12, 
						7,
						12,
						false
					));
					this.VendorItems.Add(
					new Armor(
						"leather helm",
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Leather, 
						6, 
						3,
						6,
						false
					));
					this.VendorItems.Add(
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
					this.VendorItems.Add(new Weapon(1, Weapon.WeaponType.OneHandedSword));
					this.VendorItems.Add(
						new Consumable(
						"arrows",
						15,
						Consumable.ArrowType.Standard,
						50));
					break;
				case "healer":
					this.VendorItems.Add(
					new Consumable(
						"minor health potion",
						50,
						Consumable.PotionType.Health,
						50
					));
					this.VendorItems.Add(
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

		public void DisplayGearForSale(Player player, UserOutput output) {
			var forSaleString = "The " + this.Name + " has the following items for sale:"; 
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				forSaleString);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in this.VendorItems) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName().ToString());
				if (item.IsEquipped()) {
					itemInfo.Append(" <Equipped>");
				}
				switch (item) {
					case Armor isItemArmor:
						itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + " Cost: " + isItemArmor.ItemValue + ")");
						break;
					case Weapon isItemWeapon:
						itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + 
						                " Cost: " + isItemWeapon.ItemValue + ")");
						break;
					case Consumable isItemConsumable:
						if (item.Name.Contains("arrow")) {
							itemInfo.Append(" (" + isItemConsumable.Arrow.Quantity + ")");
						}
						itemInfo.Append(" (Cost: " + isItemConsumable.ItemValue + ")");
						break;
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItemCheck(Player player, string[] userInput, UserOutput output) {
			var inputName = Helper.ParseInput(userInput);
			var index = 0;
			if (this.BuySellType == "Healer") {
				index = this.VendorItems.FindIndex(
					f => f.GetName() == inputName || f.GetName().Contains(inputName));
			}
			else {
				index = this.VendorItems.FindIndex(
					f => f.GetName() == inputName || f.GetName().Contains(userInput.Last()));
			}
			if (index != -1) {
				var buyArmor = this.VendorItems[index] as Armor;
				var buyWeapon = this.VendorItems[index] as Weapon;
				var buyConsumable = this.VendorItems[index] as Consumable;
				switch (this.BuySellType) {
					case "Armor":
						if (buyArmor != null) {
							this.BuyItem(player, userInput, buyArmor, index, output);
						}
						break;
					case "Weapon":
						if (buyWeapon != null) {
							this.BuyItem(player, userInput, buyWeapon, index, output);
						}
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index, inputName, output);
						}
						break;
					case "Healer":
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index, inputName, output);
						}
						break;
					case "Shopkeeper":
						if (buyArmor != null) {
							this.BuyItem(player, userInput, buyArmor, index, output);
						}
						if (buyWeapon != null) {
							this.BuyItem(player, userInput, buyWeapon, index, output);
						}
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index, output);
						}
						break;
				}
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index, UserOutput output) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Inventory.Add(buyItem);
				this.VendorItems.RemoveAt(index);
				var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					purchaseString);
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You can't afford that!");
		}
		public void BuyItem(
			Player player, string[] userInput, IEquipment buyItem, int index, string inputName, UserOutput output) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Consumables.Add(buyItem as Consumable);
				this.VendorItems.RemoveAt(index);
				var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					purchaseString);
				this.RepopulateHealerPotion(inputName);
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You can't afford that!");
		}
		public void SellItemCheck(Player player, string[] userInput, UserOutput output) {
			var inputName = Helper.ParseInput(userInput);
			var invIndex = player.Inventory.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName) && f.IsEquipped() == false);
			var conIndex = player.Consumables.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName) && f.IsEquipped() == false);
			if (conIndex != -1) {
				var sellConsumable = player.Consumables[conIndex] as Consumable;
				switch (this.BuySellType) {
					case "Healer":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, conIndex, output);
							break;
						}
						Helper.InvalidVendorSell(output);
						break;
					case "Shopkeeper":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, conIndex, output);
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
							this.SellItem(player, userInput, sellArmor, invIndex, output);
							break;
						}
						Helper.InvalidVendorSell(output);
						break;
					case "Weapon":
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, invIndex, output);
							break;
						}
						Helper.InvalidVendorSell(output);
						break;
					case "Shopkeeper":
						if (sellArmor != null) {
							this.SellItem(player, userInput, sellArmor, invIndex, output);
						}
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, invIndex, output);
						}
						if (sellLoot != null) {
							this.SellItem(player, userInput, sellLoot, invIndex, output);
						}
						break;
				}
			}
			if (invIndex != -1 || conIndex != -1) return;
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You don't have that to sell!");
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index, UserOutput output) {
			if (!sellItem.IsEquipped()) {
				player.Gold += sellItem.ItemValue;
				if (sellItem.GetType().Name == "Consumable") {
					player.Consumables.RemoveAt(index);
				}
				else {
					player.Inventory.RemoveAt(index);
				}
				this.VendorItems.Add(sellItem);
				var soldString = "You sold " + sellItem.Name + " to the vendor for " + sellItem.ItemValue + " gold.";
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					soldString);
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You have to unequip that first!");
		}
		public void RepairItem(Player player, string[] userInput, UserOutput output) {
			var parsedInput = Helper.ParseInput(userInput);
			var index = player.Inventory.FindIndex(
				f => f.GetName() == parsedInput || f.GetName().Contains(userInput.Last()));
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
								var repairArmorString = "Your " + repairArmor.Name + " has been repaired for " + (int) repairCostArmor +
								                   " gold."; 
								output.StoreUserOutput(
									Helper.FormatSuccessOutputText(),
									Helper.FormatDefaultBackground(),
									repairArmorString);
								break;
							}
							var cantAffordArmorString = "You can't afford to repair " + repairArmor.Name;
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								cantAffordArmorString);
						}
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"The vendor doesn't repair that type of equipment.");
						break;
					case "Weapon":
						var repairWeapon = player.Inventory[index] as Weapon;
						if (repairWeapon != null && repairWeapon.IsEquipped()) {
							var durabilityRepairWeapon = 100 - repairWeapon.Durability;
							var repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player.Gold >= repairCostWeapon) {
								player.Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								var repairWeaponString = "Your " + repairWeapon.Name + " has been repaired for " + 
								                         (int)repairCostWeapon + " gold.";
								output.StoreUserOutput(
									Helper.FormatSuccessOutputText(),
									Helper.FormatDefaultBackground(),
									repairWeaponString);
								break;
							}
							var cantAffordWeaponString = "You can't afford to repair " + repairWeapon.Name;
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								cantAffordWeaponString);
						}
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"The vendor doesn't repair that type of equipment.");
						break;
					case "Healer":
					case "Shopkeeper":
						var noRepairString = this.BuySellType + "s don't repair equipment.";
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							noRepairString);
						break;
					default:
						break;
				}
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"That item is not in your inventory.");
		}
		public string GetName() {
			return this.Name.ToString();
		}
		public void RestorePlayer(Player player, UserOutput output) {
			if (this.BuySellType == "Healer") {
				player.HitPoints = player.MaxHitPoints;
				player.RagePoints = player.MaxRagePoints;
				player.ManaPoints = player.MaxManaPoints;
				player.ComboPoints = player.MaxComboPoints;
				var restoreString = "You have been restored by the " + this.Name + ".";
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					restoreString);
				return;
			}
			var noRestoreString = "The " + this.Name + " cannot restore you!";
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				noRestoreString);
		}
		public void RepopulateHealerPotion(string inputName) {
			var potionIndex = this.VendorItems.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (potionIndex != -1) return;
			if (inputName.Contains("mana")) {
				this.VendorItems.Add(
					new Consumable(
						"minor mana potion",
						50,
						Consumable.PotionType.Mana,
						50
					));
			}
			else if (inputName.Contains("health")) {
				this.VendorItems.Add(
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