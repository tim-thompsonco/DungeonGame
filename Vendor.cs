using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame
{
	public class Vendor : IRoomInteraction, IQuestGiver
	{
		public enum VendorType
		{
			Armorer,
			Weaponsmith,
			Healer,
			Shopkeeper
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public VendorType _VendorCategory { get; set; }
		public List<IEquipment> _VendorItems { get; set; }
		public List<Quest> _AvailableQuests { get; set; }

		// Default constructor for JSON serialization
		public Vendor() { }
		public Vendor(string name, string desc, VendorType vendorCategory)
		{
			_Name = name;
			_Desc = desc;
			_VendorItems = new List<IEquipment>();
			_VendorCategory = vendorCategory;
			switch (_VendorCategory)
			{
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
					_VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
					break;
				case VendorType.Healer:
					_VendorItems.Add(
						new Consumable(1, Consumable.PotionType.Health));
					_VendorItems.Add(
						new Consumable(1, Consumable.PotionType.Mana));
					break;
				case VendorType.Shopkeeper:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayGearForSale()
		{
			string forSaleString = $"The {_Name} has the following items for sale:";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IEquipment item in _VendorItems)
			{
				StringBuilder itemInfo = new StringBuilder();
				itemInfo.Append(item._Name);
				if (item.Equipped)
				{
					itemInfo.Append(" <Equipped>");
				}
				switch (item)
				{
					case Armor isItemArmor:
						itemInfo.Append($" (AR: {isItemArmor.ArmorRating} Cost: {isItemArmor.ItemValue})");
						break;
					case Weapon isItemWeapon:
						itemInfo.Append($" (DMG: {isItemWeapon.RegDamage} CR: {isItemWeapon.CritMultiplier} Cost: {isItemWeapon.ItemValue})");
						break;
					case Consumable isItemConsumable:
						if (item._Name.Contains("arrow"))
						{
							itemInfo.Append($" ({isItemConsumable.Arrow.Quantity})");
						}
						itemInfo.Append($" (Cost: {isItemConsumable.ItemValue})");
						break;
				}
				string itemName = textInfo.ToTitleCase(itemInfo.ToString());
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemName);
			}
		}
		public void BuyItem(Player player, string[] userInput, int quantity)
		{
			if (quantity == 0)
			{
				return;
			}

			string inputName = InputHandler.ParseInput(userInput);
			int index = -1;
			if (_VendorCategory == VendorType.Healer)
			{
				index = _VendorItems.FindIndex(
					f => f._Name == inputName || f._Name.Contains(inputName));
			}
			else
			{
				index = _VendorItems.FindIndex(
					f => f._Name == inputName || f._Name.Contains(userInput.Last()));
			}
			if (index == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"The vendor doesn't have that available for sale!");
				return;
			}
			IEquipment buyItem = _VendorItems[index];
			if (player._Gold >= buyItem.ItemValue && quantity > 0)
			{
				player._Gold -= buyItem.ItemValue;
				if (buyItem is Consumable item)
				{
					player._Consumables.Add(item);
				}
				else
				{
					player._Inventory.Add(buyItem);
				}
				_VendorItems.RemoveAt(index);
				string purchaseString = $"You purchased {buyItem._Name} from the vendor for {buyItem.ItemValue} gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					purchaseString);
				if (!(buyItem is Consumable))
				{
					return;
				}

				if (_VendorCategory == VendorType.Healer)
				{
					RepopulateHealerPotion(player, inputName);
				}
				else
				{
					RepopulateArrows(inputName);
				}
				quantity--;
				BuyItem(player, userInput, quantity);
			}
			else
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
			}
		}
		private static int FindInventoryItemIndex(Player player, string[] userInput)
		{
			bool indexNumberSearch = int.TryParse(userInput.Last(), out int sellItemIndex);
			if (!indexNumberSearch)
			{
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try
			{
				// Find desired object to sell
				string inputName = InputHandler.ParseInput(userInput);
				List<IEquipment> indexList = player._Inventory.FindAll(f => f._Name == inputName || f._Name.Contains(inputName));
				IEquipment itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player._Inventory.IndexOf(itemMatch);
			}
			catch (ArgumentOutOfRangeException)
			{
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		private static int FindConsumableItemIndex(Player player, string[] userInput)
		{
			bool indexNumberSearch = int.TryParse(userInput.Last(), out int sellItemIndex);
			if (!indexNumberSearch)
			{
				// If the last word in user input is not a number, return -1, to indicate there's no specific position
				return -1;
			}
			// Player with two capes will input sell cape 2 to sell 2nd cape, but indices start from 0, so adjust value
			sellItemIndex--;
			try
			{
				// Find desired object to sell
				string inputName = InputHandler.ParseInput(userInput);
				List<Consumable> indexList = player._Consumables.FindAll(f => f._Name == inputName || f._Name.Contains(inputName));
				Consumable itemMatch = indexList[sellItemIndex];
				// Return index in player inventory of desired object to sell
				return player._Consumables.IndexOf(itemMatch);
			}
			catch (ArgumentOutOfRangeException)
			{
				// Using -2 to indicate that this is a multiple item sell attempt but the item does not exist
				return -2;
			}
		}
		public void SellItem(Player player, string[] userInput)
		{
			string inputName = InputHandler.ParseInput(userInput);
			int multipleItemIndex = FindInventoryItemIndex(player, userInput);
			if (multipleItemIndex == -2)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that item to sell!");
				return;
			}
			int index;
			if (multipleItemIndex != -1)
			{
				index = multipleItemIndex;
			}
			else
			{
				index = player._Inventory.FindIndex(
					f => f._Name == inputName || f._Name.Contains(inputName));
			}
			IEquipment sellItem;
			try
			{
				sellItem = player._Inventory[index];
				switch (sellItem)
				{
					case Armor _ when _VendorCategory == VendorType.Healer || _VendorCategory == VendorType.Weaponsmith:
						Messages.InvalidVendorSell();
						return;
					case Weapon _ when _VendorCategory == VendorType.Healer || _VendorCategory == VendorType.Armorer:
						Messages.InvalidVendorSell();
						return;
				}
				if (index != -1)
				{
					if (!sellItem.Equipped)
					{
						player._Gold += sellItem switch
						{
							Armor armor => (int)(sellItem.ItemValue * (armor.Durability / 100.0)),
							Weapon weapon => (int)(sellItem.ItemValue * (weapon.Durability / 100.0)),
							_ => sellItem.ItemValue
						};
						player._Inventory.RemoveAt(index);
						string soldString = $"You sold {sellItem._Name} to the vendor for {sellItem.ItemValue} gold.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							soldString);
					}
					else
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You have to unequip that first!");
					}
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				multipleItemIndex = FindConsumableItemIndex(player, userInput);
				if (multipleItemIndex != -1)
				{
					index = multipleItemIndex;
				}
				else
				{
					index = player._Consumables.FindIndex(
						f => f._Name == inputName || f._Name.Contains(inputName));
				}
				if (index == -1)
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You don't have that to sell!");
					return;
				}
				sellItem = player._Consumables[index];
				if (_VendorCategory == VendorType.Armorer || _VendorCategory == VendorType.Weaponsmith)
				{
					Messages.InvalidVendorSell();
					return;
				}
				player._Consumables.RemoveAt(index);
				if (_VendorItems.Count == 5)
				{
					_VendorItems.RemoveAt(_VendorItems[0]._Name.Contains("arrow") ? 1 : 0);
				}

				_VendorItems.Add(sellItem);
				string soldString = $"You sold {sellItem._Name} to the vendor for {sellItem.ItemValue} gold.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					soldString);
			}
		}
		public void RepairItem(Player player, string[] userInput, bool repairAll)
		{
			string parsedInput = InputHandler.ParseInput(userInput);
			int index = player._Inventory.FindIndex(
				f => f._Name == parsedInput || f._Name.Contains(userInput.Last()));
			if (index != -1)
			{
				switch (_VendorCategory)
				{
					case VendorType.Armorer:
						if (player._Inventory[index] is Armor repairArmor && repairArmor.Equipped)
						{
							int durabilityRepairArmor = 100 - repairArmor.Durability;
							float repairCostArmor = repairArmor.ItemValue * (durabilityRepairArmor / 100f);
							if (player._Gold >= (int)repairCostArmor)
							{
								player._Gold -= (int)repairCostArmor;
								repairArmor.Durability = 100;
								string repairArmorString = $"Your {repairArmor._Name} has been repaired for {(int)repairCostArmor} gold.";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairArmorString);
							}
							else
							{
								string cantAffordArmorString = $"You can't afford to repair {repairArmor._Name}!";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordArmorString);
							}
						}
						else
						{
							if (repairAll)
							{
								break;
							}

							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Weaponsmith:
						if (player._Inventory[index] is Weapon repairWeapon)
						{
							int durabilityRepairWeapon = 100 - repairWeapon.Durability;
							float repairCostWeapon = repairWeapon.ItemValue * (durabilityRepairWeapon / 100f);
							if (player._Gold >= repairCostWeapon)
							{
								player._Gold -= (int)repairCostWeapon;
								repairWeapon.Durability = 100;
								string repairWeaponString = $"Your {repairWeapon._Name} has been repaired for {(int)repairCostWeapon} gold.";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatSuccessOutputText(),
									Settings.FormatDefaultBackground(),
									repairWeaponString);
							}
							else
							{
								string cantAffordWeaponString = $"You can't afford to repair {repairWeapon._Name}!";
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									cantAffordWeaponString);
							}
						}
						else
						{
							if (repairAll)
							{
								break;
							}

							OutputHandler.Display.StoreUserOutput(
								Settings.FormatFailureOutputText(),
								Settings.FormatDefaultBackground(),
								"The vendor doesn't repair that type of equipment.");
						}
						break;
					case VendorType.Healer:
					case VendorType.Shopkeeper:
						string noRepairString = $"{_VendorCategory}s don't repair equipment.";
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
		public void RestorePlayer(Player player)
		{
			if (_VendorCategory == VendorType.Healer)
			{
				player._HitPoints = player._MaxHitPoints;
				player._RagePoints = player._MaxRagePoints;
				player._ManaPoints = player._MaxManaPoints;
				player._ComboPoints = player._MaxComboPoints;
				string restoreString = $"You have been restored by the {_Name}.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					restoreString);
				return;
			}
			string noRestoreString = $"The {_Name} cannot restore you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noRestoreString);
		}
		private void RepopulateHealerPotion(Player player, string inputName)
		{
			int potionIndex = _VendorItems.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (potionIndex != -1)
			{
				return;
			}

			if (inputName.Contains("mana"))
			{
				_VendorItems.Add(new Consumable(player._Level, Consumable.PotionType.Mana));
			}
			else if (inputName.Contains("health"))
			{
				_VendorItems.Add(new Consumable(player._Level, Consumable.PotionType.Health));
			}
		}
		private void RepopulateArrows(string inputName)
		{
			int arrowIndex = _VendorItems.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (arrowIndex != -1)
			{
				return;
			}

			_VendorItems.Add(new Consumable("arrows", 15, Consumable.ArrowType.Standard));
		}
		public void PopulateQuests(Player player)
		{
			_AvailableQuests = new List<Quest>();
			Armor.ArmorType questArmorGroup = player._PlayerClass switch
			{
				Player.PlayerClassType.Mage => Armor.ArmorType.Cloth,
				Player.PlayerClassType.Warrior => Armor.ArmorType.Plate,
				Player.PlayerClassType.Archer => Armor.ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (_VendorCategory)
			{
				case VendorType.Armorer:
					_AvailableQuests.Add(new Quest(
						"Bring The Mallet To Them",
						"I'm fixing armor for everyone in this town and you wouldn't believe the dents I've banged " +
						"out from travelers who have gone down into that dungeon. You know what would be great? If someone " +
						"went down there and put some dents in them for a change. I can't do that myself, I'm not a fighter, " +
						"but you look like you are. Go get 'em tiger.",
						Quest.QuestType.KillMonster,
						new Armor(questArmorGroup, Armor.ArmorSlot.Head, true, player),
						_Name));
					_AvailableQuests.Add(new Quest(
						"A Deadly Encounter",
						"I've got too much work on my hands because people like you keep going down into that dungeon " +
						"and come out looking like they got thrown around by something big. Maybe if you go clear out one of " +
						"those levels, it'll reduce how many people are getting their armor trashed around here, so I can get " +
						"caught up on my work. If it doesn't help, well you'll get some pretty gear out of it at least.",
						Quest.QuestType.ClearLevel,
						new Armor(questArmorGroup, Armor.ArmorSlot.Legs, true, player),
						_Name));
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
						_Name));
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
						_Name));
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
						_Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player)
		{
			if (_AvailableQuests == null)
			{
				PopulateQuests(player);
			}

			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Available Quests:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (Quest quest in _AvailableQuests)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(quest._Name));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input)
		{
			string userInput = InputHandler.ParseInput(input);
			int questIndex = _AvailableQuests.FindIndex(
				f => f._Name.ToLowerInvariant().Contains(userInput));
			if (questIndex != -1)
			{
				_AvailableQuests[questIndex].ShowQuest();
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputHandler.ShowUserOutput(player);
				OutputHandler.Display.ClearUserOutput();
				string[] questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLowerInvariant() != "y" && questInput[0].ToLowerInvariant() != "yes" &&
					   questInput[0].ToLowerInvariant() != "n" && questInput[0].ToLowerInvariant() != "no")
				{
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(_AvailableQuests[questIndex]._Name) + " Consideration:");
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputHandler.ShowUserOutput(player);
					OutputHandler.Display.ClearUserOutput();
					questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes")
				{
					player._QuestLog.Add(_AvailableQuests[questIndex]);
					_AvailableQuests.RemoveAt(questIndex);
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				}
				else
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			}
			else
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input)
		{
			string userInput = InputHandler.ParseInput(input);
			int questIndex = player._QuestLog.FindIndex(
				f => f._Name.ToLowerInvariant().Contains(userInput));
			Quest quest = player._QuestLog[questIndex];
			if (questIndex != -1)
			{
				if (quest._QuestGiver == _Name)
				{
					if (quest._QuestCompleted)
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"Congratulations on finishing {quest._Name}! Here's your reward.");
						player._Inventory.Add(quest._QuestRewardItem);
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearHandler.StoreRainbowGearOutput(GearHandler.GetItemDetails(quest._QuestRewardItem));
						player._Gold += quest._QuestRewardGold;
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							$"{quest._QuestRewardGold} gold coins.");
						player._QuestLog.RemoveAt(questIndex);
					}
					else
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				}
				else
				{
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						$"I didn't give you that quest. {textInfo.ToTitleCase(quest._QuestGiver)} did.");
				}
			}
			else
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}