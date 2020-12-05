using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Vendor : IRoomInteraction, IQuestGiver {
		public enum VendorType {
			Armorer,
			Weaponsmith,
			Healer,
			Shopkeeper
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public VendorType VendorCategory { get; set; }
		public List<IEquipment> VendorItems { get; set; }
		public List<Quest> AvailableQuests { get; set; }

		// Default constructor for JSON serialization
		public Vendor() { }
		public Vendor(string name, string desc, VendorType vendorCategory) {
			this.Name = name;
			this.Desc = desc;
			this.VendorItems = new List<IEquipment>();
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
			var forSaleString = $"The {this.Name} has the following items for sale:"; 
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
						itemInfo.Append($" (AR: {isItemArmor.ArmorRating} Cost: {isItemArmor.ItemValue})");
						break;
					case Weapon isItemWeapon:
						itemInfo.Append($" (DMG: {isItemWeapon.RegDamage} CR: {isItemWeapon.CritMultiplier} Cost: {isItemWeapon.ItemValue})");
						break;
					case Consumable isItemConsumable:
						if (item.Name.Contains("arrow")) {
							itemInfo.Append($" ({isItemConsumable.Arrow.Quantity})");
						}
						itemInfo.Append($" (Cost: {isItemConsumable.ItemValue})");
						break;
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItem(Player player, string[] userInput, int quantity) {
			if (quantity == 0) return;
			var inputName = InputHandler.ParseInput(userInput);
			var index = -1;
			if (this.VendorCategory == VendorType.Healer) {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			}
			else {
				index = this.VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(userInput.Last()));
			}
			if (index == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
				return;
			}
			var buyItem = this.VendorItems[index];
			if (player.Gold >= buyItem.ItemValue && quantity > 0) {
				player.Gold -= buyItem.ItemValue;
				if (buyItem is Consumable item) {
					player.Consumables.Add(item);
				}
				else {
					player.Inventory.Add(buyItem);	
				}
				this.VendorItems.RemoveAt(index);
				var purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (!(buyItem is Consumable)) return;
				if (this.VendorCategory == VendorType.Healer) {
					this.RepopulateHealerPotion(player, inputName);
				}
				else {
					this.RepopulateArrows(inputName);
				}
				quantity--;
				this.BuyItem(player, userInput, quantity);
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");	
			}
		}
		private static int FindInventoryItemIndex(Player player, string[] userInput) {
			var indexNumberSearch = int.TryParse(userInput.Last(), out var sellItemIndex);
			if (!indexNumberSearch) {
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try {
				// Find desired object to sell
				var inputName = InputHandler.ParseInput(userInput);
				var indexList = player.Inventory.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				var itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player.Inventory.IndexOf(itemMatch);
			}
			catch (ArgumentOutOfRangeException) {
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		private static int FindConsumableItemIndex(Player player, string[] userInput) {
			var indexNumberSearch = int.TryParse(userInput.Last(), out var sellItemIndex);
			if (!indexNumberSearch) {
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try {
				// Find desired object to sell
				var inputName = InputHandler.ParseInput(userInput);
				var indexList = player.Consumables.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				var itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player.Consumables.IndexOf(itemMatch);
			}
			catch (ArgumentOutOfRangeException) {
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		public void SellItem(Player player, string[] userInput) {
			var inputName = InputHandler.ParseInput(userInput);
			var multipleItemIndex = FindInventoryItemIndex(player, userInput);
			if (multipleItemIndex == -2) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that item to sell!");
				return;
			}
			int index;
			if (multipleItemIndex != -1) {
				index = multipleItemIndex;
			}
			else {
				index = player.Inventory.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			}
			IEquipment sellItem;
			try {
				sellItem = player.Inventory[index];
				switch (sellItem) {
					case Armor _ when this.VendorCategory == VendorType.Healer || this.VendorCategory == VendorType.Weaponsmith:
						Messages.InvalidVendorSell();
						return;
					case Weapon _ when this.VendorCategory == VendorType.Healer || this.VendorCategory == VendorType.Armorer:
						Messages.InvalidVendorSell();
						return;
				}
				if (index != -1) {
					if (!sellItem.Equipped) {
						player.Gold += sellItem switch {
							Armor armor => (int) (sellItem.ItemValue * (armor.Durability / 100.0)),
							Weapon weapon => (int) (sellItem.ItemValue * (weapon.Durability / 100.0)),
							_ => sellItem.ItemValue
						};
						player.Inventory.RemoveAt(index);
						var soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							soldString);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You have to unequip that first!");
					}
				}
			}
			catch (ArgumentOutOfRangeException) {
				multipleItemIndex = FindConsumableItemIndex(player, userInput);
				if (multipleItemIndex != -1) {
					index = multipleItemIndex;
				}
				else {
					index = player.Consumables.FindIndex(
						f => f.Name == inputName || f.Name.Contains(inputName));
				}
				if (index == -1) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You don't have that to sell!");
					return;
				}
				sellItem = player.Consumables[index];
				if (this.VendorCategory == VendorType.Armorer || this.VendorCategory == VendorType.Weaponsmith) {
					Messages.InvalidVendorSell();
					return;
				}
				player.Consumables.RemoveAt(index);
				if (this.VendorItems.Count == 5) this.VendorItems.RemoveAt(this.VendorItems[0].Name.Contains("arrow") ? 1 : 0);
				this.VendorItems.Add(sellItem);
				var soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					soldString);
			}
		}
		public void RepairItem(Player player, string[] userInput, bool repairAll) {
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
								var repairArmorString = $"Your {repairArmor.Name} has been repaired for {(int) repairCostArmor} gold."; 
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairArmorString);
							}
							else {
								var cantAffordArmorString = $"You can't afford to repair {repairArmor.Name}!";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordArmorString);
							}
						}
						else {
							if (repairAll) break;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Weaponsmith:
						if (player.Inventory[index] is Weapon repairWeapon) {
							var durabilityRepairWeapon = 100 - repairWeapon.Durability;
							var repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player.Gold >= repairCostWeapon) {
								player.Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								var repairWeaponString = $"Your {repairWeapon.Name} has been repaired for {(int)repairCostWeapon} gold.";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairWeaponString);
							}
							else {
								var cantAffordWeaponString = $"You can't afford to repair {repairWeapon.Name}!";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordWeaponString);
							}
						}
						else {
							if (repairAll) break;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Healer:
					case VendorType.Shopkeeper:
						var noRepairString = $"{this.VendorCategory}s don't repair equipment.";
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
				var restoreString = $"You have been restored by the {this.Name}.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					restoreString);
				return;
			}
			var noRestoreString = $"The {this.Name} cannot restore you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noRestoreString);
		}
		private void RepopulateHealerPotion(Player player, string inputName) {
			var potionIndex = this.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (potionIndex != -1) return;
			if (inputName.Contains("mana")) {
				this.VendorItems.Add(new Consumable(player.Level, Consumable.PotionType.Mana));
			}
			else if (inputName.Contains("health")) {
				this.VendorItems.Add(new Consumable(player.Level, Consumable.PotionType.Health));
			}
		}
		private void RepopulateArrows(string inputName) {
			var arrowIndex = this.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (arrowIndex != -1) return;
			this.VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
		}
		public void PopulateQuests(Player player) {
			this.AvailableQuests = new List<Quest>();
			var questArmorGroup = player.PlayerClass switch {
				Player.PlayerClassType.Mage => Armor.ArmorType.Cloth,
				Player.PlayerClassType.Warrior => Armor.ArmorType.Plate,
				Player.PlayerClassType.Archer => Armor.ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (this.VendorCategory) {
				case VendorType.Armorer:
					this.AvailableQuests.Add(new Quest(
						"Bring The Mallet To Them",
						"I'm fixing armor for everyone in this town and you wouldn't believe the dents I've banged " + 
						"out from travelers who have gone down into that dungeon. You know what would be great? If someone " +
						"went down there and put some dents in them for a change. I can't do that myself, I'm not a fighter, " +
						"but you look like you are. Go get 'em tiger.",
						Quest.QuestType.KillMonster, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Head, true, player), 
						this.Name));
					this.AvailableQuests.Add(new Quest(
						"A Deadly Encounter",
						"I've got too much work on my hands because people like you keep going down into that dungeon " + 
						"and come out looking like they got thrown around by something big. Maybe if you go clear out one of " +
						"those levels, it'll reduce how many people are getting their armor trashed around here, so I can get " +
						"caught up on my work. If it doesn't help, well you'll get some pretty gear out of it at least.",
						Quest.QuestType.ClearLevel, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Legs, true, player), 
						this.Name));
					break;
				case VendorType.Weaponsmith:
					this.AvailableQuests.Add(new Quest(
						"Live And Let Die",
						"You want me to make you a weapon? A really good weapon that will be really shiny with great " + 
						"stats? Well, nothing is free pal. I'm not going to ask you to go talk to someone standing right next " +
						"to me, but I will give you a bounty. Go kill a bunch of monsters for me to prove you're worthy of what " +
						"I can make. Do that and you'll get a hell of a weapon.",
						Quest.QuestType.KillMonster, 
						new Weapon(Weapon.WeaponType.OneHandedSword, true, player), 
						this.Name));
					break;
				case VendorType.Healer:
					this.AvailableQuests.Add(new Quest(
						"Stop The Pain",
						"I can't recall how many travelers like you I've patched up who went down into that dungeon, " + 
						"tangled with the wrong monster, and came back missing a limb covered in blood. There's only so much " +
						"suffering I can heal. Would you do me a favor? Go kill some of those monsters, so they stop hurting " +
						"people like you, and I can rest easy for a while because I'm not constantly busy.",
						Quest.QuestType.ClearLevel, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Wrist, true, player), 
						this.Name));
					break;
				case VendorType.Shopkeeper:
					this.AvailableQuests.Add(new Quest(
						"Buyer's Market",
						"I wish I had more stuff to sell. I'd love to sell you something, but I have nothing to offer. " + 
						"You know what would fix that? If you went and killed some monsters then sold me their gear. I'm not " +
						"expecting you to buy that stuff, but someone else will, and don't you worry about how much I'm going " +
						"to mark up that stuff. Help me help you by buying your loot at a, uh, fair price.",
						Quest.QuestType.KillMonster, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Waist, true, player), 
						this.Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (this.AvailableQuests == null) this.PopulateQuests(player);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				"Available Quests:");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var quest in this.AvailableQuests) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(), 
					Settings.FormatDefaultBackground(), 
					textInfo.ToTitleCase(quest.Name));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			var userInput = InputHandler.ParseInput(input);
			var questIndex = this.AvailableQuests.FindIndex(
				f => f.Name.ToLowerInvariant().Contains(userInput));
			if (questIndex != -1) {
				this.AvailableQuests[questIndex].ShowQuest();
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputHandler.ShowUserOutput(player);
				OutputHandler.Display.ClearUserOutput();
				var questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLowerInvariant() != "y" && questInput[0].ToLowerInvariant() != "yes" &&
				       questInput[0].ToLowerInvariant() != "n" && questInput[0].ToLowerInvariant() != "no") {
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(this.AvailableQuests[questIndex].Name) + " Consideration:");
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputHandler.ShowUserOutput(player);
					OutputHandler.Display.ClearUserOutput();
					questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player.QuestLog.Add(this.AvailableQuests[questIndex]);
					this.AvailableQuests.RemoveAt(questIndex);
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			var userInput = InputHandler.ParseInput(input);
			var questIndex = player.QuestLog.FindIndex(
				f => f.Name.ToLowerInvariant().Contains(userInput));
			var quest = player.QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest.QuestGiver == this.Name) {
					if (quest.QuestCompleted) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"Congratulations on finishing {quest.Name}! Here's your reward.");
						player.Inventory.Add(quest.QuestRewardItem);
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearHandler.StoreRainbowGearOutput(GearHandler.GetItemDetails(quest.QuestRewardItem));
						player.Gold += quest.QuestRewardGold;
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"{quest.QuestRewardGold} gold coins.");
						player.QuestLog.RemoveAt(questIndex);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				}
				else {
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						$"I didn't give you that quest. {textInfo.ToTitleCase(quest.QuestGiver)} did.");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}