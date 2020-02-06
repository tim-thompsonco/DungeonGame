using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Vendor : IRoomInteraction {
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
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Head));
					this.VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest));
					this.VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Legs));
					break;
				case "weaponsmith":
					this.VendorItems.Add(new Weapon(1, Weapon.WeaponType.OneHandedSword));
					this.VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
					break;
				case "healer":
					this.VendorItems.Add(
					new Consumable(1, Consumable.PotionType.Health));
					this.VendorItems.Add(
					new Consumable(1, Consumable.PotionType.Mana));
					break;
			}
		}

		public void DisplayGearForSale() {
			var forSaleString = "The " + this.Name + " has the following items for sale:"; 
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in this.VendorItems) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.Name);
				if (item.Equipped) {
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
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItemCheck(Player player, string[] userInput) {
			var inputName = InputHandler.ParseInput(userInput);
			var index = 0;
			if (this.BuySellType == "Healer") {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			}
			else {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(userInput.Last()));
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
						if (buyConsumable != null) {
							this.BuyItem(player, userInput, buyConsumable, index, inputName);
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
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Inventory.Add(buyItem);
				this.VendorItems.RemoveAt(index);
				var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You can't afford that!");
		}
		public void BuyItem(
			Player player, string[] userInput, IEquipment buyItem, int index, string inputName) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Consumables.Add(buyItem as Consumable);
				this.VendorItems.RemoveAt(index);
				var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (this.BuySellType == "Healer") {
					this.RepopulateHealerPotion(inputName);
				}
				else {
					this.RepopulateArrows(inputName);
				}
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You can't afford that!");
		}
		public void SellItemCheck(Player player, string[] userInput) {
			var inputName = InputHandler.ParseInput(userInput);
			var invIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName) && f.Equipped == false);
			var conIndex = player.Consumables.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName) && f.Equipped == false);
			if (conIndex != -1) {
				var sellConsumable = player.Consumables[conIndex] as Consumable;
				switch (this.BuySellType) {
					case "Healer":
						if (sellConsumable != null) {
							this.SellItem(player, userInput, sellConsumable, conIndex);
							break;
						}
						Messages.InvalidVendorSell();
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
						Messages.InvalidVendorSell();
						break;
					case "Weapon":
						if (sellWeapon != null) {
							this.SellItem(player, userInput, sellWeapon, invIndex);
							break;
						}
						Messages.InvalidVendorSell();
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
			if (invIndex != -1 || conIndex != -1) return;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that to sell!");
		}
		public void SellItem(Player player, string[] userInput, IEquipment sellItem, int index) {
			if (!sellItem.Equipped) {
				player.Gold += sellItem.ItemValue;
				if (sellItem.GetType().Name == "Consumable") {
					player.Consumables.RemoveAt(index);
				}
				else {
					player.Inventory.RemoveAt(index);
				}
				this.VendorItems.Add(sellItem);
				var soldString = "You sold " + sellItem.Name + " to the vendor for " + sellItem.ItemValue + " gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					soldString);
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You have to unequip that first!");
		}
		public void RepairItem(Player player, string[] userInput) {
			var parsedInput = InputHandler.ParseInput(userInput);
			var index = player.Inventory.FindIndex(
				f => f.Name == parsedInput || f.Name.Contains(userInput.Last()));
			if (index != -1) {
				switch (this.BuySellType) {
					case "Armor":
						var repairArmor = player.Inventory[index] as Armor;
						if (repairArmor != null && repairArmor.Equipped) {
							var durabilityRepairArmor = 100 - repairArmor.Durability;
							var repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							if (player.Gold >= (int)repairCostArmor) {
								player.Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								var repairArmorString = "Your " + repairArmor.Name + " has been repaired for " + (int) repairCostArmor +
								                   " gold."; 
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairArmorString);
								break;
							}
							var cantAffordArmorString = "You can't afford to repair " + repairArmor.Name;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								cantAffordArmorString);
						}
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"The vendor doesn't repair that type of equipment.");
						break;
					case "Weapon":
						var repairWeapon = player.Inventory[index] as Weapon;
						if (repairWeapon != null && repairWeapon.Equipped) {
							var durabilityRepairWeapon = 100 - repairWeapon.Durability;
							var repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player.Gold >= repairCostWeapon) {
								player.Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								var repairWeaponString = "Your " + repairWeapon.Name + " has been repaired for " + 
								                         (int)repairCostWeapon + " gold.";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairWeaponString);
								break;
							}
							var cantAffordWeaponString = "You can't afford to repair " + repairWeapon.Name;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								cantAffordWeaponString);
						}
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"The vendor doesn't repair that type of equipment.");
						break;
					case "Healer":
					case "Shopkeeper":
						var noRepairString = this.BuySellType + "s don't repair equipment.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							noRepairString);
						break;
				}
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"That item is not in your inventory.");
		}
		public string GetName() {
			return this.Name;
		}
		public void RestorePlayer(Player player) {
			if (this.BuySellType == "Healer") {
				player.HitPoints = player.MaxHitPoints;
				player.RagePoints = player.MaxRagePoints;
				player.ManaPoints = player.MaxManaPoints;
				player.ComboPoints = player.MaxComboPoints;
				var restoreString = "You have been restored by the " + this.Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					restoreString);
				return;
			}
			var noRestoreString = "The " + this.Name + " cannot restore you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noRestoreString);
		}
		public void RepopulateHealerPotion(string inputName) {
			var potionIndex = this.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (potionIndex != -1) return;
			if (inputName.Contains("mana")) {
				this.VendorItems.Add(new Consumable(1, Consumable.PotionType.Mana));
			}
			else if (inputName.Contains("health")) {
				this.VendorItems.Add(new Consumable(1, Consumable.PotionType.Health));
			}
		}
		public void RepopulateArrows(string inputName) {
			var arrowIndex = this.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (arrowIndex != -1) return;
			this.VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
		}
	}
}