using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Items.Equipment;
using DungeonGame.Players;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Vendor : IQuestGiver {
		public enum VendorType {
			Armorer,
			Weaponsmith,
			Healer,
			Shopkeeper
		}
		public string Name { get; set; }
		public string _Desc { get; set; }
		public VendorType _VendorCategory { get; set; }
		public List<IItem> _VendorItems { get; set; }
		public List<Quest> _AvailableQuests { get; set; }

		// Default constructor for JSON serialization
		public Vendor() { }
		public Vendor(string name, string desc, VendorType vendorCategory) {
			Name = name;
			_Desc = desc;
			_VendorItems = new List<IItem>();
			_VendorCategory = vendorCategory;
			switch (_VendorCategory) {
				case VendorType.Armorer:
					_VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Head));
					_VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest));
					_VendorItems.Add(
						new Armor(1, Armor.ArmorType.Leather, Armor.ArmorSlot.Legs));
					break;
				case VendorType.Weaponsmith:
					_VendorItems.Add(new Weapon(1, Weapon.WeaponType.OneHandedSword));
					_VendorItems.Add(new Arrows("arrows", 15, Arrows.ArrowType.Standard));
					break;
				case VendorType.Healer:
					_VendorItems.Add(new HealthPotion(PotionStrength.Minor));
					_VendorItems.Add(new ManaPotion(PotionStrength.Minor));
					break;
				case VendorType.Shopkeeper:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayGearForSale() {
			string forSaleString = $"The {Name} has the following items for sale:";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IItem item in _VendorItems) {
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
						itemInfo.Append($" (DMG: {isItemWeapon._RegDamage} CR: {isItemWeapon._CritMultiplier} Cost: {isItemWeapon.ItemValue})");
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
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItem(Player player, string[] userInput, int quantity) {
			if (quantity == 0) {
				return;
			}

			string inputName = InputController.ParseInput(userInput);
			int index = -1;
			if (_VendorCategory == VendorType.Healer) {
				index = _VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			} else {
				index = _VendorItems.FindIndex(
					f => f.Name == inputName || f.Name.Contains(userInput.Last()));
			}
			if (index == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
				return;
			}
			IItem buyItem = _VendorItems[index];
			if (player._Gold >= buyItem.ItemValue && quantity > 0) {
				player._Gold -= buyItem.ItemValue;
				player._Inventory.Add(buyItem);
				_VendorItems.RemoveAt(index);
				string purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (!(buyItem is IPotion || buyItem is Arrows)) {
					return;
				}

				if (_VendorCategory == VendorType.Healer) {
					RepopulateHealerPotion(inputName);
				} else {
					RepopulateArrows(inputName);
				}
				quantity--;
				BuyItem(player, userInput, quantity);
			} else {
				OutputController.Display.StoreUserOutput(
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
				string inputName = InputController.ParseInput(userInput);
				List<IItem> indexList = player._Inventory.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				IItem itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player._Inventory.IndexOf(itemMatch);
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
				string inputName = InputController.ParseInput(userInput);
				List<IItem> indexList = player._Inventory.FindAll(f => f.Name == inputName || f.Name.Contains(inputName));
				IItem itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player._Inventory.IndexOf(itemMatch);
			} catch (ArgumentOutOfRangeException) {
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		public void SellItem(Player player, string[] userInput) {
			string inputName = InputController.ParseInput(userInput);
			int multipleItemIndex = FindInventoryItemIndex(player, userInput);
			if (multipleItemIndex == -2) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that item to sell!");
				return;
			}
			int index;
			if (multipleItemIndex != -1) {
				index = multipleItemIndex;
			} else {
				index = player._Inventory.FindIndex(
					f => f.Name == inputName || f.Name.Contains(inputName));
			}
			IItem sellItem;
			try {
				sellItem = player._Inventory[index];
				switch (sellItem) {
					case Armor _ when _VendorCategory == VendorType.Healer || _VendorCategory == VendorType.Weaponsmith:
						Messages.InvalidVendorSell();
						return;
					case Weapon _ when _VendorCategory == VendorType.Healer || _VendorCategory == VendorType.Armorer:
						Messages.InvalidVendorSell();
						return;
				}
				if (index != -1) {
					if (!(sellItem is IEquipment equippableItem && equippableItem.Equipped)) {
						player._Gold += sellItem switch {
							Armor armor => (int)(sellItem.ItemValue * (armor.Durability / 100.0)),
							Weapon weapon => (int)(sellItem.ItemValue * (weapon._Durability / 100.0)),
							_ => sellItem.ItemValue
						};
						player._Inventory.RemoveAt(index);
						string soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
						OutputController.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							soldString);
					} else {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You have to unequip that first!");
					}
				}
			} catch (ArgumentOutOfRangeException) {
				multipleItemIndex = FindConsumableItemIndex(player, userInput);
				if (multipleItemIndex != -1) {
					index = multipleItemIndex;
				} else {
					index = player._Inventory.FindIndex(
						f => f.Name == inputName || f.Name.Contains(inputName));
				}
				if (index == -1) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You don't have that to sell!");
					return;
				}
				sellItem = player._Inventory[index];
				if (_VendorCategory == VendorType.Armorer || _VendorCategory == VendorType.Weaponsmith) {
					Messages.InvalidVendorSell();
					return;
				}
				player._Inventory.RemoveAt(index);
				if (_VendorItems.Count == 5) {
					_VendorItems.RemoveAt(_VendorItems[0].Name.Contains("arrow") ? 1 : 0);
				}

				_VendorItems.Add(sellItem);
				string soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					soldString);
			}
		}
		public void RepairItem(Player player, string[] userInput, bool repairAll) {
			string parsedInput = InputController.ParseInput(userInput);
			int index = player._Inventory.FindIndex(
				f => f.Name == parsedInput || f.Name.Contains(userInput.Last()));
			if (index != -1) {
				switch (_VendorCategory) {
					case VendorType.Armorer:
						if (player._Inventory[index] is Armor repairArmor && repairArmor.Equipped) {
							int durabilityRepairArmor = 100 - repairArmor.Durability;
							float repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							if (player._Gold >= (int)repairCostArmor) {
								player._Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								string repairArmorString = $"Your {repairArmor.Name} has been repaired for {(int)repairCostArmor} gold.";
								OutputController.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairArmorString);
							} else {
								string cantAffordArmorString = $"You can't afford to repair {repairArmor.Name}!";
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordArmorString);
							}
						} else {
							if (repairAll) {
								break;
							}

							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Weaponsmith:
						if (player._Inventory[index] is Weapon repairWeapon) {
							int durabilityRepairWeapon = 100 - repairWeapon._Durability;
							float repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player._Gold >= repairCostWeapon) {
								player._Gold -= (int)repairCostWeapon;
								repairWeapon._Durability = 100;
								string repairWeaponString = $"Your {repairWeapon.Name} has been repaired for {(int)repairCostWeapon} gold.";
								OutputController.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairWeaponString);
							} else {
								string cantAffordWeaponString = $"You can't afford to repair {repairWeapon.Name}!";
								OutputController.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordWeaponString);
							}
						} else {
							if (repairAll) {
								break;
							}

							OutputController.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Healer:
					case VendorType.Shopkeeper:
						string noRepairString = $"{_VendorCategory}s don't repair equipment.";
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							noRepairString);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				return;
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"That item is not in your inventory.");
		}
		public void RestorePlayer(Player player) {
			if (_VendorCategory == VendorType.Healer) {
				player._HitPoints = player._MaxHitPoints;
				player._RagePoints = player._MaxRagePoints;
				player._ManaPoints = player._MaxManaPoints;
				player._ComboPoints = player._MaxComboPoints;
				string restoreString = $"You have been restored by the {Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					restoreString);
				return;
			}
			string noRestoreString = $"The {Name} cannot restore you!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noRestoreString);
		}

		private void RepopulateHealerPotion(string inputName) {
			int potionIndex = _VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (potionIndex != -1) {
				return;
			}

			if (inputName.Contains("mana")) {
				_VendorItems.Add(new ManaPotion(PotionStrength.Minor));
			} else if (inputName.Contains("health")) {
				_VendorItems.Add(new HealthPotion(PotionStrength.Minor));
			}
		}

		private void RepopulateArrows(string inputName) {
			int arrowIndex = _VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (arrowIndex != -1) {
				return;
			}

			_VendorItems.Add(new Arrows("arrows", 15, Arrows.ArrowType.Standard));
		}
		public void PopulateQuests(Player player) {
			_AvailableQuests = new List<Quest>();
			Armor.ArmorType questArmorGroup = player._PlayerClass switch {
				Player.PlayerClassType.Mage => Armor.ArmorType.Cloth,
				Player.PlayerClassType.Warrior => Armor.ArmorType.Plate,
				Player.PlayerClassType.Archer => Armor.ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (_VendorCategory) {
				case VendorType.Armorer:
					_AvailableQuests.Add(new Quest(
						"Bring The Mallet To Them",
						"I'm fixing armor for everyone in this town and you wouldn't believe the dents I've banged " +
						"out from travelers who have gone down into that dungeon. You know what would be great? If someone " +
						"went down there and put some dents in them for a change. I can't do that myself, I'm not a fighter, " +
						"but you look like you are. Go get 'em tiger.",
						Quest.QuestType.KillMonster,
						new Armor(questArmorGroup, Armor.ArmorSlot.Head, true, player),
						Name));
					_AvailableQuests.Add(new Quest(
						"A Deadly Encounter",
						"I've got too much work on my hands because people like you keep going down into that dungeon " +
						"and come out looking like they got thrown around by something big. Maybe if you go clear out one of " +
						"those levels, it'll reduce how many people are getting their armor trashed around here, so I can get " +
						"caught up on my work. If it doesn't help, well you'll get some pretty gear out of it at least.",
						Quest.QuestType.ClearLevel,
						new Armor(questArmorGroup, Armor.ArmorSlot.Legs, true, player),
						Name));
					break;
				case VendorType.Weaponsmith:
					_AvailableQuests.Add(new Quest(
						"Live And Let Die",
						"You want me to make you a weapon? A really good weapon that will be really shiny with great " +
						"stats? Well, nothing is free pal. I'm not going to ask you to go talk to someone standing right next " +
						"to me, but I will give you a bounty. Go kill a bunch of monsters for me to prove you're worthy of what " +
						"I can make. Do that and you'll get a hell of a weapon.",
						Quest.QuestType.KillMonster,
						new Weapon(Weapon.WeaponType.OneHandedSword, true, player),
						Name));
					break;
				case VendorType.Healer:
					_AvailableQuests.Add(new Quest(
						"Stop The Pain",
						"I can't recall how many travelers like you I've patched up who went down into that dungeon, " +
						"tangled with the wrong monster, and came back missing a limb covered in blood. There's only so much " +
						"suffering I can heal. Would you do me a favor? Go kill some of those monsters, so they stop hurting " +
						"people like you, and I can rest easy for a while because I'm not constantly busy.",
						Quest.QuestType.ClearLevel,
						new Armor(questArmorGroup, Armor.ArmorSlot.Wrist, true, player),
						Name));
					break;
				case VendorType.Shopkeeper:
					_AvailableQuests.Add(new Quest(
						"Buyer's Market",
						"I wish I had more stuff to sell. I'd love to sell you something, but I have nothing to offer. " +
						"You know what would fix that? If you went and killed some monsters then sold me their gear. I'm not " +
						"expecting you to buy that stuff, but someone else will, and don't you worry about how much I'm going " +
						"to mark up that stuff. Help me help you by buying your loot at a, uh, fair price.",
						Quest.QuestType.KillMonster,
						new Armor(questArmorGroup, Armor.ArmorSlot.Waist, true, player),
						Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (_AvailableQuests == null) {
				PopulateQuests(player);
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Available Quests:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (Quest quest in _AvailableQuests) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(quest._Name));
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			string userInput = InputController.ParseInput(input);
			int questIndex = _AvailableQuests.FindIndex(
				f => f._Name.ToLower().Contains(userInput));
			if (questIndex != -1) {
				_AvailableQuests[questIndex].ShowQuest();
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputController.ShowUserOutput(player);
				OutputController.Display.ClearUserOutput();
				string[] questInput = InputController.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLower() != "y" && questInput[0].ToLower() != "yes" &&
					   questInput[0].ToLower() != "n" && questInput[0].ToLower() != "no") {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(_AvailableQuests[questIndex]._Name) + " Consideration:");
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputController.ShowUserOutput(player);
					OutputController.Display.ClearUserOutput();
					questInput = InputController.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player._QuestLog.Add(_AvailableQuests[questIndex]);
					_AvailableQuests.RemoveAt(questIndex);
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			string userInput = InputController.ParseInput(input);
			int questIndex = player._QuestLog.FindIndex(
				f => f._Name.ToLower().Contains(userInput));
			Quest quest = player._QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest._QuestGiver == Name) {
					if (quest._QuestCompleted) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"Congratulations on finishing {quest._Name}! Here's your reward.");
						player._Inventory.Add(quest._QuestRewardItem);
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearController.StoreRainbowGearOutput(GearController.GetItemDetails(quest._QuestRewardItem));
						player._Gold += quest._QuestRewardGold;
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"{quest._QuestRewardGold} gold coins.");
						player._QuestLog.RemoveAt(questIndex);
					} else {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				} else {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						$"I didn't give you that quest. {textInfo.ToTitleCase(quest._QuestGiver)} did.");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}