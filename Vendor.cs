﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Vendor : IRoomInteraction {
		public enum VendorType {
			Armorer,
			Weaponsmith,
			Healer,
			Shopkeeper
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public VendorType VendorCategory { get; set; }
		public List<IEquipment> VendorItems { get; set; } = new List<IEquipment>();

		// Default constructor for JSON serialization
		public Vendor() { }
		public Vendor(string name, string desc, VendorType vendorCategory) {
			this.Name = name;
			this.Desc = desc;
			this.VendorCategory = vendorCategory;
			switch (this.VendorCategory) {
				case VendorType.Armorer:
					this.VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Head));
					this.VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest));
					this.VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Legs));
					break;
				case VendorType.Weaponsmith:
					this.VendorItems.Add(new Weapon(1, Weapon.WeaponType.OneHandedSword));
					this.VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
					break;
				case VendorType.Healer:
					this.VendorItems.Add(
						new Consumable(1, Consumable.PotionType.Health));
					this.VendorItems.Add(
						new Consumable(1, Consumable.PotionType.Mana));
					break;
				case VendorType.Shopkeeper:
					break;
				default:
					throw new ArgumentOutOfRangeException();
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
			if (this.VendorCategory == VendorType.Healer) {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			}
			else {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(userInput.Last()));
			}
			if (index != -1) {
				var buyArmor = this.VendorItems[index] as Armor;
				var buyLoot = this.VendorItems[index] as Loot;
				var buyWeapon = this.VendorItems[index] as Weapon;
				var buyConsumable = this.VendorItems[index] as Consumable;
				switch (this.VendorCategory) {
					case VendorType.Armorer:
						if (buyArmor != null) {
							this.BuyItem(player, buyArmor, index);
						}
						break;
					case VendorType.Weaponsmith:
						if (buyWeapon != null) {
							this.BuyItem(player, buyWeapon, index);
						}
						if (buyConsumable != null) {
							this.BuyItem(player, buyConsumable, index, inputName);
						}
						break;
					case VendorType.Healer:
						if (buyConsumable != null) {
							this.BuyItem(player, buyConsumable, index, inputName);
						}
						break;
					case VendorType.Shopkeeper:
						if (buyArmor != null) {
							this.BuyItem(player, buyArmor, index);
						}
						if (buyWeapon != null) {
							this.BuyItem(player, buyWeapon, index);
						}
						if (buyConsumable != null) {
							this.BuyItem(player, buyConsumable, index);
						}
						if (buyLoot != null) {
							this.BuyItem(player, buyLoot, index);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
			}
		}
		public void BuyItem(Player player, IEquipment buyItem, int index) {
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
		private void BuyItem(
			Player player, IEquipment buyItem, int index, string inputName) {
			if (player.Gold >= buyItem.ItemValue) {
				player.Gold -= buyItem.ItemValue;
				player.Consumables.Add(buyItem as Consumable);
				this.VendorItems.RemoveAt(index);
				var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (this.VendorCategory == VendorType.Healer) {
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
				var sellConsumable = player.Consumables[conIndex];
				switch (this.VendorCategory) {
					case VendorType.Armorer:
						break;
					case VendorType.Weaponsmith:
						break;
					case VendorType.Healer:
						if (sellConsumable != null) {
							this.SellItem(player, sellConsumable, conIndex);
							break;
						}
						Messages.InvalidVendorSell();
						break;
					case VendorType.Shopkeeper:
						if (sellConsumable != null) {
							this.SellItem(player, sellConsumable, conIndex);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (invIndex != -1) {
				var sellArmor = player.Inventory[invIndex] as Armor;
				var sellWeapon = player.Inventory[invIndex] as Weapon;
				var sellLoot = player.Inventory[invIndex] as Loot;
				switch (this.VendorCategory) {
					case VendorType.Armorer:
						if (sellArmor != null) {
							this.SellItem(player, sellArmor, invIndex);
							break;
						}
						Messages.InvalidVendorSell();
						break;
					case VendorType.Weaponsmith:
						if (sellWeapon != null) {
							this.SellItem(player, sellWeapon, invIndex);
							break;
						}
						Messages.InvalidVendorSell();
						break;
					case VendorType.Healer:
						break;
					case VendorType.Shopkeeper:
						if (sellArmor != null) {
							this.SellItem(player, sellArmor, invIndex);
						}
						if (sellWeapon != null) {
							this.SellItem(player, sellWeapon, invIndex);
						}
						if (sellLoot != null) {
							this.SellItem(player, sellLoot, invIndex);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (invIndex != -1 || conIndex != -1) return;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that to sell!");
		}
		private void SellItem(Player player, IEquipment sellItem, int index) {
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
				switch (this.VendorCategory) {
					case VendorType.Armorer:
						if (player.Inventory[index] is Armor repairArmor && repairArmor.Equipped) {
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
					case VendorType.Weaponsmith:
						if (player.Inventory[index] is Weapon repairWeapon && repairWeapon.Equipped) {
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
					case VendorType.Healer:
						break;
					case VendorType.Shopkeeper:
						var noRepairString = this.VendorCategory+ "s don't repair equipment.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							noRepairString);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"That item is not in your inventory.");
		}
		public void RestorePlayer(Player player) {
			if (this.VendorCategory == VendorType.Healer) {
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
		private void RepopulateHealerPotion(string inputName) {
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
		private void RepopulateArrows(string inputName) {
			var arrowIndex = this.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (arrowIndex != -1) return;
			this.VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
		}
	}
}