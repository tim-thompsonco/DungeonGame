using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Consumables.Arrow;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Items.Equipment;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Players;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Vendors {
	public class Vendor : IQuestGiver {
		public string Name { get; set; }
		public string Desc { get; set; }
		public VendorType VendorCategory { get; set; }
		public List<IItem> VendorItems { get; set; }
		public List<Quest> AvailableQuests { get; set; }

		// Default constructor for JSON serialization
		public Vendor() { }
		public Vendor(string name, string desc, VendorType vendorCategory) {
			Name = name;
			Desc = desc;
			VendorItems = new List<IItem>();
			VendorCategory = vendorCategory;
			switch (VendorCategory) {
				case VendorType.Armorer:
					VendorItems.Add(
						new Armor(1, ArmorType.Leather, ArmorSlot.Head));
					VendorItems.Add(
						new Armor(1, ArmorType.Leather, ArmorSlot.Chest));
					VendorItems.Add(
						new Armor(1, ArmorType.Leather, ArmorSlot.Legs));
					break;
				case VendorType.Weaponsmith:
					VendorItems.Add(new Weapon(1, WeaponType.OneHandedSword));
					VendorItems.Add(new Arrows("arrows", 15, ArrowType.Standard));
					break;
				case VendorType.Healer:
					VendorItems.Add(new HealthPotion(PotionStrength.Minor));
					VendorItems.Add(new ManaPotion(PotionStrength.Minor));
					break;
				case VendorType.Shopkeeper:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayGearForSale() {
			string forSaleString = $"The {Name} has the following items for sale:";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IItem item in VendorItems) {
				StringBuilder itemInfo = new StringBuilder();
				itemInfo.Append(item.Name);
				if (item is IEquipment equippableItem && equippableItem.Equipped) {
					itemInfo.Append(" <Equipped>");
				}
				switch (item) {
					case Armor isItemArmor:
						itemInfo.Append($" (AR: {isItemArmor.ArmorRating} Cost: {isItemArmor.ItemValue})");
						break;
					case Weapon isItemWeapon:
						itemInfo.Append($" (DMG: {isItemWeapon.RegDamage} CR: {isItemWeapon.CritMultiplier} Cost: {isItemWeapon.ItemValue})");
						break;
					default:
						if (item.GetType() == typeof(Arrows)) {
							Arrows arrows = item as Arrows;
							itemInfo.Append($" ({arrows.Quantity})");
						}
						itemInfo.Append($" (Cost: {item.ItemValue})");
						break;
				}
				string itemName = textInfo.ToTitleCase(itemInfo.ToString());
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItem(Player player, string[] userInput, int quantity) {
			if (quantity == 0) {
				return;
			}

			string inputName = InputHelper.ParseInput(userInput);
			int index = -1;
			index = VendorCategory == VendorType.Healer
				? VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName))
				: VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(userInput.Last()));
			if (index == -1) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
				return;
			}
			IItem buyItem = VendorItems[index];
			if (player.Gold >= buyItem.ItemValue && quantity > 0) {
				player.Gold -= buyItem.ItemValue;
				player.Inventory.Add(buyItem);
				VendorItems.RemoveAt(index);
				string purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (!(buyItem is IPotion || buyItem is Arrows)) {
					return;
				}

				if (VendorCategory == VendorType.Healer) {
					RepopulateHealerPotion(inputName);
				} else {
					RepopulateArrows(inputName);
				}
				quantity--;
				BuyItem(player, userInput, quantity);
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
			}
		}
		private static int FindInventoryItemIndex(Player player, string[] userInput) {
			bool indexNumberSearch = int.TryParse(userInput.Last(), out int sellItemIndex);
			if (!indexNumberSearch) {
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try {
				// Find desired object to sell
				string inputName = InputHelper.ParseInput(userInput);
				List<IItem> indexList = player.Inventory.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				IItem itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player.Inventory.IndexOf(itemMatch);
			} catch (ArgumentOutOfRangeException) {
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		private static int FindConsumableItemIndex(Player player, string[] userInput) {
			bool indexNumberSearch = int.TryParse(userInput.Last(), out int sellItemIndex);
			if (!indexNumberSearch) {
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try {
				// Find desired object to sell
				string inputName = InputHelper.ParseInput(userInput);
				List<IItem> indexList = player.Inventory.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				IItem itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player.Inventory.IndexOf(itemMatch);
			} catch (ArgumentOutOfRangeException) {
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		public void SellItem(Player player, string[] userInput) {
			string inputName = InputHelper.ParseInput(userInput);
			int multipleItemIndex = FindInventoryItemIndex(player, userInput);
			if (multipleItemIndex == -2) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that item to sell!");
				return;
			}
			int index = multipleItemIndex != -1
				? multipleItemIndex
				: player.Inventory.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			IItem sellItem;
			try {
				sellItem = player.Inventory[index];
				switch (sellItem) {
					case Armor _ when VendorCategory == VendorType.Healer || VendorCategory == VendorType.Weaponsmith:
						Messages.InvalidVendorSell();
						return;
					case Weapon _ when VendorCategory == VendorType.Healer || VendorCategory == VendorType.Armorer:
						Messages.InvalidVendorSell();
						return;
				}
				if (index != -1) {
					if (!(sellItem is IEquipment equippableItem && equippableItem.Equipped)) {
						player.Gold += sellItem switch {
							Armor armor => (int)(sellItem.ItemValue * (armor.Durability / 100.0)),
							Weapon weapon => (int)(sellItem.ItemValue * (weapon.Durability / 100.0)),
							_ => sellItem.ItemValue
						};
						player.Inventory.RemoveAt(index);
						string soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							soldString);
					} else {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You have to unequip that first!");
					}
				}
			} catch (ArgumentOutOfRangeException) {
				multipleItemIndex = FindConsumableItemIndex(player, userInput);
				index = multipleItemIndex != -1
					? multipleItemIndex
					: player.Inventory.FindIndex(
						f => f.Name == inputName || f.Name.Contains(inputName));
				if (index == -1) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You don't have that to sell!");
					return;
				}
				sellItem = player.Inventory[index];
				if (VendorCategory == VendorType.Armorer || VendorCategory == VendorType.Weaponsmith) {
					Messages.InvalidVendorSell();
					return;
				}
				player.Inventory.RemoveAt(index);
				if (VendorItems.Count == 5) {
					VendorItems.RemoveAt(VendorItems[0].Name.Contains("arrow") ? 1 : 0);
				}

				VendorItems.Add(sellItem);
				string soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					soldString);
			}
		}
		public void RepairItem(Player player, string[] userInput, bool repairAll) {
			string parsedInput = InputHelper.ParseInput(userInput);
			int index = player.Inventory.FindIndex(
				f => f.Name == parsedInput || f.Name.Contains(userInput.Last()));
			if (index != -1) {
				switch (VendorCategory) {
					case VendorType.Armorer:
						if (player.Inventory[index] is Armor repairArmor && repairArmor.Equipped) {
							int durabilityRepairArmor = 100 - repairArmor.Durability;
							float repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							if (player.Gold >= (int)repairCostArmor) {
								player.Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								string repairArmorString = $"Your {repairArmor.Name} has been repaired for {(int)repairCostArmor} gold.";
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairArmorString);
							} else {
								string cantAffordArmorString = $"You can't afford to repair {repairArmor.Name}!";
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordArmorString);
							}
						} else {
							if (repairAll) {
								break;
							}

							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Weaponsmith:
						if (player.Inventory[index] is Weapon repairWeapon) {
							int durabilityRepairWeapon = 100 - repairWeapon.Durability;
							float repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player.Gold >= repairCostWeapon) {
								player.Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								string repairWeaponString = $"Your {repairWeapon.Name} has been repaired for {(int)repairCostWeapon} gold.";
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairWeaponString);
							} else {
								string cantAffordWeaponString = $"You can't afford to repair {repairWeapon.Name}!";
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordWeaponString);
							}
						} else {
							if (repairAll) {
								break;
							}

							OutputHelper.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Healer:
					case VendorType.Shopkeeper:
						string noRepairString = $"{VendorCategory}s don't repair equipment.";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							noRepairString);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				return;
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"That item is not in your inventory.");
		}
		public void RestorePlayer(Player player) {
			if (VendorCategory == VendorType.Healer) {
				player.HitPoints = player.MaxHitPoints;
				player.RagePoints = player.MaxRagePoints;
				player.ManaPoints = player.MaxManaPoints;
				player.ComboPoints = player.MaxComboPoints;
				string restoreString = $"You have been restored by the {Name}.";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					restoreString);
				return;
			}
			string noRestoreString = $"The {Name} cannot restore you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noRestoreString);
		}

		private void RepopulateHealerPotion(string inputName) {
			int potionIndex = VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (potionIndex != -1) {
				return;
			}

			if (inputName.Contains("mana")) {
				VendorItems.Add(new ManaPotion(PotionStrength.Minor));
			} else if (inputName.Contains("health")) {
				VendorItems.Add(new HealthPotion(PotionStrength.Minor));
			}
		}

		private void RepopulateArrows(string inputName) {
			int arrowIndex = VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (arrowIndex != -1) {
				return;
			}

			VendorItems.Add(new Arrows("arrows", 15, ArrowType.Standard));
		}
		public void PopulateQuests(Player player) {
			AvailableQuests = new List<Quest>();
			ArmorType questArmorGroup = player.PlayerClass switch {
				PlayerClassType.Mage => ArmorType.Cloth,
				PlayerClassType.Warrior => ArmorType.Plate,
				PlayerClassType.Archer => ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (VendorCategory) {
				case VendorType.Armorer:
					AvailableQuests.Add(new Quest(
						"Bring The Mallet To Them",
						"I'm fixing armor for everyone in this town and you wouldn't believe the dents I've banged " +
						"out from travelers who have gone down into that dungeon. You know what would be great? If someone " +
						"went down there and put some dents in them for a change. I can't do that myself, I'm not a fighter, " +
						"but you look like you are. Go get 'em tiger.",
						QuestType.KillMonster,
						new Armor(questArmorGroup, ArmorSlot.Head, true, player),
						Name));
					AvailableQuests.Add(new Quest(
						"A Deadly Encounter",
						"I've got too much work on my hands because people like you keep going down into that dungeon " +
						"and come out looking like they got thrown around by something big. Maybe if you go clear out one of " +
						"those levels, it'll reduce how many people are getting their armor trashed around here, so I can get " +
						"caught up on my work. If it doesn't help, well you'll get some pretty gear out of it at least.",
						QuestType.ClearLevel,
						new Armor(questArmorGroup, ArmorSlot.Legs, true, player),
						Name));
					break;
				case VendorType.Weaponsmith:
					AvailableQuests.Add(new Quest(
						"Live And Let Die",
						"You want me to make you a weapon? A really good weapon that will be really shiny with great " +
						"stats? Well, nothing is free pal. I'm not going to ask you to go talk to someone standing right next " +
						"to me, but I will give you a bounty. Go kill a bunch of monsters for me to prove you're worthy of what " +
						"I can make. Do that and you'll get a hell of a weapon.",
						QuestType.KillMonster,
						new Weapon(WeaponType.OneHandedSword, true, player),
						Name));
					break;
				case VendorType.Healer:
					AvailableQuests.Add(new Quest(
						"Stop The Pain",
						"I can't recall how many travelers like you I've patched up who went down into that dungeon, " +
						"tangled with the wrong monster, and came back missing a limb covered in blood. There's only so much " +
						"suffering I can heal. Would you do me a favor? Go kill some of those monsters, so they stop hurting " +
						"people like you, and I can rest easy for a while because I'm not constantly busy.",
						QuestType.ClearLevel,
						new Armor(questArmorGroup, ArmorSlot.Wrist, true, player),
						Name));
					break;
				case VendorType.Shopkeeper:
					AvailableQuests.Add(new Quest(
						"Buyer's Market",
						"I wish I had more stuff to sell. I'd love to sell you something, but I have nothing to offer. " +
						"You know what would fix that? If you went and killed some monsters then sold me their gear. I'm not " +
						"expecting you to buy that stuff, but someone else will, and don't you worry about how much I'm going " +
						"to mark up that stuff. Help me help you by buying your loot at a, uh, fair price.",
						QuestType.KillMonster,
						new Armor(questArmorGroup, ArmorSlot.Waist, true, player),
						Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (AvailableQuests == null) {
				PopulateQuests(player);
			}

			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Available Quests:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (Quest quest in AvailableQuests) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(quest.Name));
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			string userInput = InputHelper.ParseInput(input);
			int questIndex = AvailableQuests.FindIndex(
				f => f.Name.ToLower().Contains(userInput));
			if (questIndex != -1) {
				AvailableQuests[questIndex].ShowQuest();
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputHelper.ShowUserOutput(player);
				OutputHelper.Display.ClearUserOutput();
				string[] questInput = InputHelper.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLower() != "y" && questInput[0].ToLower() != "yes" &&
					   questInput[0].ToLower() != "n" && questInput[0].ToLower() != "no") {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(AvailableQuests[questIndex].Name) + " Consideration:");
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputHelper.ShowUserOutput(player);
					OutputHelper.Display.ClearUserOutput();
					questInput = InputHelper.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player.QuestLog.Add(AvailableQuests[questIndex]);
					AvailableQuests.RemoveAt(questIndex);
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			string userInput = InputHelper.ParseInput(input);
			int questIndex = player.QuestLog.FindIndex(
				f => f.Name.ToLower().Contains(userInput));
			Quest quest = player.QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest.QuestGiver == Name) {
					if (quest.QuestCompleted) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"Congratulations on finishing {quest.Name}! Here's your reward.");
						player.Inventory.Add(quest.QuestRewardItem);
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearHelper.StoreRainbowGearOutput(GearHelper.GetItemDetails(quest.QuestRewardItem));
						player.Gold += quest.QuestRewardGold;
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"{quest.QuestRewardGold} gold coins.");
						player.QuestLog.RemoveAt(questIndex);
					} else {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				} else {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						$"I didn't give you that quest. {textInfo.ToTitleCase(quest.QuestGiver)} did.");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}