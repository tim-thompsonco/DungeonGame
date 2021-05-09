using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Consumables.Arrow;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Items.Equipment;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Helpers {
	public static class PlayerHelper {
		public static bool OutOfArrows(Player player) {
			return !player.PlayerQuiver.HaveArrows();
		}
		public static void LookAtObject(Player player, string[] input) {
			string parsedInput = InputHelper.ParseInput(input);
			IRoom playerRoom = RoomHelper.Rooms[player.PlayerLocation];
			int roomMatch = playerRoom.RoomObjects.FindIndex(f =>
				f.Name.Contains(parsedInput));
			if (roomMatch != -1) {
				playerRoom.LookNpc(input, player);
				return;
			}
			int playerInvIndex = player.Inventory.FindIndex(f => f.Name.Contains(input[1]) ||
																				 f.Name == parsedInput);
			if (playerInvIndex != -1) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player.Inventory[playerInvIndex].Desc);
				return;
			}
			int playerConsIndex = player.Inventory.FindIndex(f => f.Name.Contains(input[1]) ||
																	f.Name == parsedInput);
			if (playerConsIndex != -1) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player.Inventory[playerConsIndex].Desc);
			}
		}
		public static int GetInventoryWeight(Player player) {
			return player.Inventory.Sum(item => item.Weight) +
				   player.Inventory.Sum(consumable => consumable.Weight);
		}
		public static void ShowInventory(Player player) {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Your inventory contains:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IItem item in player.Inventory) {
				if (!(item is IEquipment equippableItem && equippableItem.Equipped)) {
					continue;
				}

				string itemName = GearHelper.GetItemDetails(item);
				StringBuilder itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver")) {
					itemInfo.Append($" (Arrows: {player.PlayerQuiver.Quantity}/{player.PlayerQuiver.MaxQuantity})");
				}

				itemInfo.Append(" <_Equipped>");
				if (item is Armor || item is Weapon) {
					IRainbowGear playerItem = item as IRainbowGear;
					if (playerItem.IsRainbowGear) {
						GearHelper.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			foreach (IItem item in player.Inventory) {
				if (item is IEquipment equippableItem && equippableItem.Equipped) {
					continue;
				}

				string itemName = GearHelper.GetItemDetails(item);
				StringBuilder itemInfo = new StringBuilder(itemName);
				if (player.PlayerQuiver?.Name == itemName) {
					itemInfo.Append($"Arrows: {player.PlayerQuiver.Quantity}/{player.PlayerQuiver.MaxQuantity}");
				}

				if (item is Armor || item is Weapon) {
					IRainbowGear playerItem = item as IRainbowGear;
					if (playerItem.IsRainbowGear) {
						GearHelper.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			Dictionary<string, int> consumableDict = new Dictionary<string, int>();
			foreach (IItem item in player.Inventory) {
				StringBuilder itemInfo = new StringBuilder();
				itemInfo.Append(item.Name);
				if (item.Name.Contains("potion")) {
					if (item is HealthPotion) {
						HealthPotion potion = item as HealthPotion;
						itemInfo.Append($" (+{potion.HealthAmount} health)");
					}

					if (item is ManaPotion) {
						ManaPotion potion = item as ManaPotion;
						itemInfo.Append($" (+{potion.ManaAmount} mana)");
					}

					if (item is StatPotion) {
						StatPotion potion = item as StatPotion;
						itemInfo.Append($" (+{potion.StatAmount} {potion.StatPotionType})");
					}
				}
				if (item.GetType() == typeof(Arrows)) {
					Arrows arrows = item as Arrows;
					itemInfo.Append($" ({arrows.Quantity})");
				}
				string itemName = textInfo.ToTitleCase(itemInfo.ToString());
				if (!consumableDict.ContainsKey(itemName)) {
					consumableDict.Add(itemName, 1);
					continue;
				}
				int dictValue = consumableDict[itemName];
				dictValue += 1;
				consumableDict[itemName] = dictValue;
			}
			foreach (KeyValuePair<string, int> consumable in consumableDict) {
				string Inventorytring = $"{consumable.Key} (Quantity: {consumable.Value})";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					Inventorytring);
			}
			string goldString = $"_Gold: {player.Gold} coins.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				goldString);
			string weightString = $"_Weight: {GetInventoryWeight(player)}/{player.MaxCarryWeight}";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				weightString);
		}

		public static void LevelUpCheck(Player player) {
			if (player.Experience < player.ExperienceToLevel || player.Level == 10) {
				return;
			}

			foreach (IEffect effect in player.Effects.ToList()) {
				effect.IsEffectExpired = true;
			}

			player.Level++;
			player.Experience -= player.ExperienceToLevel;
			player.ExperienceToLevel = Settings.GetBaseExperienceToLevel() * player.Level;
			string levelUpString = $"You have leveled! You are now level {player.Level}.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatLevelUpText(),
				Settings.FormatDefaultBackground(),
				levelUpString);
			int statsToAssign = 5;
			while (statsToAssign > 0) {
				string levelUpStatString = $"Please choose {statsToAssign} stats to raise. Your choices are: str, dex, int, const.";
				const string levelUpStatInfo =
					"You may raise a stat more than once by putting a number after the stat, IE str 2.";
				const string levelUpStatStr = "Str will increase your max carrying weight and warrior abilities.";
				const string levelUpStatDex = "Dex will increase your dodge chance and archer abilities";
				const string levelUpStatInt =
					"Int will increase your mana and decrease your training cost for spells and abilities.";
				const string levelUpStatConst = "Const will increase your max hit points.";
				DisplayPlayerStats(player);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatString);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInfo);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatStr);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatDex);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInt);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatConst);
				OutputHelper.Display.RetrieveUserOutput();
				OutputHelper.Display.ClearUserOutput();
				int statNumber = 0;
				try {
					string[] input = InputHelper.GetFormattedInput(Console.ReadLine());
					if (input.Length > 1) {
						if (GameHelper.IsWholeNumber(input[1]) == false) {
							continue;
						}

						statNumber = Convert.ToInt32(input[1]);
					}
					switch (input[0]) {
						case "str":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Strength += statNumber;
								statsToAssign -= statNumber;
							} else {
								player.Strength++;
								statsToAssign--;
							}
							break;
						case "dex":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Dexterity += statNumber;
								statsToAssign -= statNumber;
							} else {
								player.Dexterity++;
								statsToAssign--;
							}
							break;
						case "int":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Intelligence += statNumber;
								statsToAssign -= statNumber;
							} else {
								player.Intelligence++;
								statsToAssign--;
							}
							break;
						case "const":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Constitution += statNumber;
								statsToAssign -= statNumber;
							} else {
								player.Constitution++;
								statsToAssign--;
							}
							break;
					}
				} catch (IndexOutOfRangeException) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),
						Settings.FormatDefaultBackground(),
						"You did not select an appropriate stat!");
				}
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(),
				"All stats have been assigned!");
			player.FireResistance += 5;
			player.FrostResistance += 5;
			player.ArcaneResistance += 5;
			CalculatePlayerStats(player);
			// Leveling sets player back to max stats
			player.HitPoints = player.MaxHitPoints;
			player.RagePoints = player.MaxRagePoints;
			player.ComboPoints = player.MaxComboPoints;
			player.ManaPoints = player.MaxManaPoints;
			// Update level for rainbow gear if any in player inventory
			foreach (IItem item in player.Inventory) {
				if (item is Armor || item is Weapon) {
					IRainbowGear rainbowItem = (IRainbowGear)item;
					if (rainbowItem != null && rainbowItem.IsRainbowGear) {
						rainbowItem.UpdateRainbowStats(player);
					}
				}
			}
		}
		public static void CalculatePlayerStats(Player player) {
			switch (player.PlayerClass) {
				case PlayerClassType.Mage:
					player.MaxManaPoints = player.Intelligence * 10;
					if (player.ManaPoints > player.MaxManaPoints) {
						player.ManaPoints = player.MaxManaPoints;
					}

					break;
				case PlayerClassType.Warrior:
					player.MaxRagePoints = player.Strength * 10;
					if (player.RagePoints > player.MaxRagePoints) {
						player.RagePoints = player.MaxRagePoints;
					}

					break;
				case PlayerClassType.Archer:
					player.MaxComboPoints = player.Dexterity * 10;
					if (player.ComboPoints > player.MaxComboPoints) {
						player.ComboPoints = player.MaxComboPoints;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			player.MaxHitPoints = player.Constitution * 10;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}

			player.MaxCarryWeight = (int)(player.Strength * 2.5);
			player.DodgeChance = player.Dexterity * 1.5;
		}
		public static void DisplayPlayerStats(Player player) {
			Settings.FormatGeneralInfoText();
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			string playerHealthString = $"Health: {player.HitPoints}/{player.MaxHitPoints} ";
			List<string> healLineOutput = new List<string>() {
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				playerHealthString};
			int hitPointMaxUnits = player.MaxHitPoints / 10;
			int hitPointUnits = player.HitPoints / hitPointMaxUnits;
			for (int i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputHelper.Display.StoreUserOutput(healLineOutput);
			switch (player.PlayerClass) {
				case PlayerClassType.Mage:
					string playerManaString = $"Mana: {player.ManaPoints}/{player.MaxManaPoints} ";
					List<string> manaLineOutput = new List<string>
					{
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						playerManaString
					};
					int manaBufferAmount = playerHealthString.Length - playerManaString.Length;
					StringBuilder manaBufferStringBuilder = new StringBuilder();
					for (int b = 0; b < manaBufferAmount; b++) {
						manaBufferStringBuilder.Append(" ");
					}
					manaLineOutput.Add(Settings.FormatGeneralInfoText());
					manaLineOutput.Add(Settings.FormatDefaultBackground());
					manaLineOutput.Add(manaBufferStringBuilder.ToString());
					int? manaPointMaxUnits = player.MaxManaPoints / 10;
					int? manaPointUnits = player.ManaPoints / manaPointMaxUnits;
					for (int i = 0; i < manaPointUnits; i++) {
						manaLineOutput.Add(Settings.FormatGeneralInfoText());
						manaLineOutput.Add(Settings.FormatManaBackground());
						manaLineOutput.Add("    ");
					}
					OutputHelper.Display.StoreUserOutput(manaLineOutput);
					break;
				case PlayerClassType.Warrior:
					string playerRageString = $"Rage: {player.RagePoints}/{player.MaxRagePoints} ";
					List<string> rageLineOutput = new List<string>
					{
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						playerRageString
					};
					int rageBufferAmount = playerHealthString.Length - playerRageString.Length;
					StringBuilder rageBufferStringBuilder = new StringBuilder();
					for (int b = 0; b < rageBufferAmount; b++) {
						rageBufferStringBuilder.Append(" ");
					}
					rageLineOutput.Add(Settings.FormatGeneralInfoText());
					rageLineOutput.Add(Settings.FormatDefaultBackground());
					rageLineOutput.Add(rageBufferStringBuilder.ToString());
					int? ragePointMaxUnits = player.MaxRagePoints / 10;
					int? ragePointUnits = player.RagePoints / ragePointMaxUnits;
					for (int i = 0; i < ragePointUnits; i++) {
						rageLineOutput.Add(Settings.FormatGeneralInfoText());
						rageLineOutput.Add(Settings.FormatRageBackground());
						rageLineOutput.Add("    ");
					}
					OutputHelper.Display.StoreUserOutput(rageLineOutput);
					break;
				case PlayerClassType.Archer:
					string playerComboString = $"Combo: {player.ComboPoints}/{player.MaxComboPoints} ";
					List<string> comboLineOutput = new List<string>
					{
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						playerComboString
					};
					int comboBufferAmount = playerHealthString.Length - playerComboString.Length;
					StringBuilder comboBufferStringBuilder = new StringBuilder();
					for (int b = 0; b < comboBufferAmount; b++) {
						comboBufferStringBuilder.Append(" ");
					}
					comboLineOutput.Add(Settings.FormatGeneralInfoText());
					comboLineOutput.Add(Settings.FormatDefaultBackground());
					comboLineOutput.Add(comboBufferStringBuilder.ToString());
					int? comboPointMaxUnits = player.MaxComboPoints / 10;
					int? comboPointUnits = player.ComboPoints / comboPointMaxUnits;
					for (int i = 0; i < comboPointUnits; i++) {
						comboLineOutput.Add(Settings.FormatGeneralInfoText());
						comboLineOutput.Add(Settings.FormatComboBackground());
						comboLineOutput.Add("    ");
					}
					OutputHelper.Display.StoreUserOutput(comboLineOutput);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			string expString = $"EXP: {player.Experience}";
			List<string> expLineOutput =
				new List<string> { Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), expString };
			int expBufferAmount = playerHealthString.Length - expString.Length;
			StringBuilder expBufferStringBuilder = new StringBuilder();
			for (int b = 0; b < expBufferAmount; b++) {
				expBufferStringBuilder.Append(" ");
			}
			expLineOutput.Add(Settings.FormatGeneralInfoText());
			expLineOutput.Add(Settings.FormatDefaultBackground());
			expLineOutput.Add(expBufferStringBuilder.ToString());
			int expPointMaxUnits = player.ExperienceToLevel / 10;
			int expPointUnits = player.Experience / expPointMaxUnits;
			for (int i = 0; i < expPointUnits; i++) {
				expLineOutput.Add(Settings.FormatGeneralInfoText());
				expLineOutput.Add(Settings.FormatExpBackground());
				expLineOutput.Add("    ");
			}
			OutputHelper.Display.StoreUserOutput(expLineOutput);
			string baseStatsString = $"Str: {player.Strength} Int: {player.Intelligence} Dex: {player.Dexterity} _Level: {player.Level}";
			StringBuilder statsSb = new StringBuilder(baseStatsString);
			if (player.PlayerClass == PlayerClassType.Archer) {
				statsSb.Append($" Arrows: {player.PlayerQuiver?.Quantity}");
			}

			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				statsSb.ToString());
			string resistString = $"Fire Resist: {player.FireResistance} Frost Resist: {player.FrostResistance} Arcane Resist: {player.ArcaneResistance}";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				resistString);
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
		}
		public static void ListAbilities(Player player) {
			if (player.PlayerClass != PlayerClassType.Mage) {
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"You have the following abilities:");
				foreach (PlayerAbility ability in player.Abilities) {
					string abilityName = textInfo.ToTitleCase(ability.Name);
					string abilityString = $"{abilityName}, Rank {ability.Rank}";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityString);
				}
			} else if (player.PlayerClass == PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void ListSpells(Player player) {
			if (player.PlayerClass == PlayerClassType.Mage) {
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your spellbook contains:");
				foreach (PlayerSpell spell in player.Spellbook) {
					string spellName = textInfo.ToTitleCase(spell.Name);
					string spellString = $"{spellName}, Rank {spell.Rank}";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellString);
				}
			} else if (player.PlayerClass != PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input) {
			string inputName = InputHelper.ParseInput(input);
			int index = player.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == PlayerClassType.Warrior) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player.Abilities[index].Name));
				string rankString = $"Rank: {player.Abilities[index].Rank}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string rageCostString = $"Rage Cost: {player.Abilities[index].RageCost}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rageCostString);
				switch (player.Abilities[index].WarAbilityCategory) {
					case WarriorAbility.Slash:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case WarriorAbility.Rend:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case WarriorAbility.Charge:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case WarriorAbility.Block:
						PlayerAbility.DefenseAbilityInfo(player, index);
						break;
					case WarriorAbility.Berserk:
						PlayerAbility.BerserkAbilityInfo(player, index);
						break;
					case WarriorAbility.Disarm:
						PlayerAbility.DisarmAbilityInfo(player, index);
						break;
					case WarriorAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case WarriorAbility.PowerAura:
						PlayerAbility.PowerAuraAbilityInfo(player, index);
						break;
					case WarriorAbility.WarCry:
						PlayerAbility.WarCryAbilityInfo(player, index);
						break;
					case WarriorAbility.Onslaught:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player.PlayerClass == PlayerClassType.Archer) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player.Abilities[index].Name));
				string rankString = $"Rank: {player.Abilities[index].Rank}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string comboCostString = $"Combo Cost: {player.Abilities[index].ComboCost}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					comboCostString);
				switch (player.Abilities[index].ArcAbilityCategory) {
					case ArcherAbility.Distance:
						PlayerAbility.DistanceAbilityInfo(player, index);
						break;
					case ArcherAbility.Gut:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case ArcherAbility.Precise:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case ArcherAbility.Stun:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case ArcherAbility.Double:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case ArcherAbility.Wound:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case ArcherAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case ArcherAbility.SwiftAura:
						PlayerAbility.SwiftAuraAbilityInfo(player, index);
						break;
					case ArcherAbility.ImmolatingArrow:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case ArcherAbility.Ambush:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player.PlayerClass == PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that ability.");
			}
		}
		public static void SpellInfo(Player player, string[] input) {
			StringBuilder inputName = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
				if (i != input.Length - 1) {
					inputName.Append(" ");
				}
			}
			int index = player.Spellbook.FindIndex(f =>
				f.Name == inputName.ToString() || f.Name == input[1] || f.Name.Contains(inputName.ToString()));
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player.Spellbook[index].Name));
				string rankString = $"Rank: {player.Spellbook[index].Rank}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string manaCostString = $"Mana Cost: {player.Spellbook[index].ManaCost}";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					manaCostString);
				switch (player.Spellbook[index].SpellCategory) {
					case SpellType.Fireball:
						PlayerSpell.FireOffenseSpellInfo(player, index);
						break;
					case SpellType.Frostbolt:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					case SpellType.Lightning:
						PlayerSpell.ArcaneOffenseSpellInfo(player, index);
						break;
					case SpellType.Heal:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case SpellType.Rejuvenate:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case SpellType.Diamondskin:
						PlayerSpell.AugmentArmorSpellInfo(player, index);
						break;
					case SpellType.TownPortal:
						PlayerSpell.PortalSpellInfo();
						break;
					case SpellType.Reflect:
						PlayerSpell.ReflectDamageSpellInfo(player, index);
						break;
					case SpellType.ArcaneIntellect:
						PlayerSpell.ArcaneIntellectSpellInfo(player, index);
						break;
					case SpellType.FrostNova:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player.PlayerClass != PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that spell.");
			}
		}
		public static void QuestInfo(Player player, string[] input) {
			StringBuilder inputName = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
				if (i != input.Length - 1) {
					inputName.Append(" ");
				}
			}
			int index = player.QuestLog.FindIndex(f =>
				f.Name.ToLower() == inputName.ToString() || f.Name.ToLower() == input[1] ||
				f.Name.ToLower().Contains(inputName.ToString()));
			if (index != -1) {
				player.QuestLog[index].ShowQuest();
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that quest.");
			}
		}
		public static int CalculateAbilityDamage(Player player, Monster opponent, int index) {
			if (player.Abilities[index].DamageGroup == DamageType.Physical) {
				return player.Abilities[index].Offensive.Amount;
			}
			double damageReductionPercentage = player.Abilities[index].DamageGroup switch {
				DamageType.Fire => opponent.FireResistance / 100.0,
				DamageType.Frost => opponent.FrostResistance / 100.0,
				DamageType.Arcane => opponent.ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(player.Abilities[index].Offensive.Amount * (1 - damageReductionPercentage));
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index) {
			if (player.Spellbook[index].DamageGroup == DamageType.Physical) {
				return player.Spellbook[index].Offensive.Amount;
			}
			double damageReductionPercentage = player.Spellbook[index].DamageGroup switch {
				DamageType.Fire => opponent.FireResistance / 100.0,
				DamageType.Frost => opponent.FrostResistance / 100.0,
				DamageType.Arcane => opponent.ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(player.Spellbook[index].Offensive.Amount * (1 - damageReductionPercentage));
		}
	}
}