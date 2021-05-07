using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Controllers {
	public static class PlayerController {
		public static bool OutOfArrows(Player player) {
			return !player._PlayerQuiver.HaveArrows();
		}
		public static void LookAtObject(Player player, string[] input) {
			string parsedInput = InputController.ParseInput(input);
			IRoom playerRoom = RoomController._Rooms[player._PlayerLocation];
			int roomMatch = playerRoom._RoomObjects.FindIndex(f =>
				f.Name.Contains(parsedInput));
			if (roomMatch != -1) {
				playerRoom.LookNpc(input, player);
				return;
			}
			int playerInvIndex = player._Inventory.FindIndex(f => f.Name.Contains(input[1]) ||
																				 f.Name == parsedInput);
			if (playerInvIndex != -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player._Inventory[playerInvIndex].Desc);
				return;
			}
			int playerConsIndex = player._Inventory.FindIndex(f => f.Name.Contains(input[1]) ||
																	f.Name == parsedInput);
			if (playerConsIndex != -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player._Inventory[playerConsIndex].Desc);
			}
		}
		public static int GetInventoryWeight(Player player) {
			return player._Inventory.Sum(item => item.Weight) +
				   player._Inventory.Sum(consumable => consumable.Weight);
		}
		public static void ShowInventory(Player player) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Your inventory contains:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IItem item in player._Inventory) {
				if (!(item is IEquipment equippableItem && equippableItem.Equipped)) {
					continue;
				}

				string itemName = GearController.GetItemDetails(item);
				StringBuilder itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver")) {
					itemInfo.Append($" (Arrows: {player._PlayerQuiver.Quantity}/{player._PlayerQuiver.MaxQuantity})");
				}

				itemInfo.Append(" <_Equipped>");
				if (item is Armor || item is Weapon) {
					IRainbowGear playerItem = item as IRainbowGear;
					if (playerItem.IsRainbowGear) {
						GearController.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			foreach (IItem item in player._Inventory) {
				if (item is IEquipment equippableItem && equippableItem.Equipped) {
					continue;
				}

				string itemName = GearController.GetItemDetails(item);
				StringBuilder itemInfo = new StringBuilder(itemName);
				if (player._PlayerQuiver?.Name == itemName) {
					itemInfo.Append($"Arrows: {player._PlayerQuiver.Quantity}/{player._PlayerQuiver.MaxQuantity}");
				}

				if (item is Armor || item is Weapon) {
					IRainbowGear playerItem = item as IRainbowGear;
					if (playerItem.IsRainbowGear) {
						GearController.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			Dictionary<string, int> consumableDict = new Dictionary<string, int>();
			foreach (IItem item in player._Inventory) {
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
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					Inventorytring);
			}
			string goldString = $"_Gold: {player._Gold} coins.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				goldString);
			string weightString = $"_Weight: {GetInventoryWeight(player)}/{player._MaxCarryWeight}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				weightString);
		}

		public static void LevelUpCheck(Player player) {
			if (player._Experience < player._ExperienceToLevel || player._Level == 10) {
				return;
			}

			foreach (IEffect effect in player._Effects.ToList()) {
				effect.IsEffectExpired = true;
			}

			player._Level++;
			player._Experience -= player._ExperienceToLevel;
			player._ExperienceToLevel = Settings.GetBaseExperienceToLevel() * player._Level;
			string levelUpString = $"You have leveled! You are now level {player._Level}.";
			OutputController.Display.StoreUserOutput(
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
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatString);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInfo);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatStr);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatDex);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInt);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatConst);
				OutputController.Display.RetrieveUserOutput();
				OutputController.Display.ClearUserOutput();
				int statNumber = 0;
				try {
					string[] input = InputController.GetFormattedInput(Console.ReadLine());
					if (input.Length > 1) {
						if (GameController.IsWholeNumber(input[1]) == false) {
							continue;
						}

						statNumber = Convert.ToInt32(input[1]);
					}
					switch (input[0]) {
						case "str":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player._Strength += statNumber;
								statsToAssign -= statNumber;
							} else {
								player._Strength++;
								statsToAssign--;
							}
							break;
						case "dex":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player._Dexterity += statNumber;
								statsToAssign -= statNumber;
							} else {
								player._Dexterity++;
								statsToAssign--;
							}
							break;
						case "int":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player._Intelligence += statNumber;
								statsToAssign -= statNumber;
							} else {
								player._Intelligence++;
								statsToAssign--;
							}
							break;
						case "const":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player._Constitution += statNumber;
								statsToAssign -= statNumber;
							} else {
								player._Constitution++;
								statsToAssign--;
							}
							break;
					}
				} catch (IndexOutOfRangeException) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),
						Settings.FormatDefaultBackground(),
						"You did not select an appropriate stat!");
				}
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(),
				"All stats have been assigned!");
			player._FireResistance += 5;
			player._FrostResistance += 5;
			player._ArcaneResistance += 5;
			CalculatePlayerStats(player);
			// Leveling sets player back to max stats
			player._HitPoints = player._MaxHitPoints;
			player._RagePoints = player._MaxRagePoints;
			player._ComboPoints = player._MaxComboPoints;
			player._ManaPoints = player._MaxManaPoints;
			// Update level for rainbow gear if any in player inventory
			foreach (IItem item in player._Inventory) {
				if (item is Armor || item is Weapon) {
					IRainbowGear rainbowItem = (IRainbowGear)item;
					if (rainbowItem != null && rainbowItem.IsRainbowGear) {
						rainbowItem.UpdateRainbowStats(player);
					}
				}
			}
		}
		public static void CalculatePlayerStats(Player player) {
			switch (player._PlayerClass) {
				case Player.PlayerClassType.Mage:
					player._MaxManaPoints = player._Intelligence * 10;
					if (player._ManaPoints > player._MaxManaPoints) {
						player._ManaPoints = player._MaxManaPoints;
					}

					break;
				case Player.PlayerClassType.Warrior:
					player._MaxRagePoints = player._Strength * 10;
					if (player._RagePoints > player._MaxRagePoints) {
						player._RagePoints = player._MaxRagePoints;
					}

					break;
				case Player.PlayerClassType.Archer:
					player._MaxComboPoints = player._Dexterity * 10;
					if (player._ComboPoints > player._MaxComboPoints) {
						player._ComboPoints = player._MaxComboPoints;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			player._MaxHitPoints = player._Constitution * 10;
			if (player._HitPoints > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			}

			player._MaxCarryWeight = (int)(player._Strength * 2.5);
			player._DodgeChance = player._Dexterity * 1.5;
		}
		public static void DisplayPlayerStats(Player player) {
			Settings.FormatGeneralInfoText();
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			string playerHealthString = $"Health: {player._HitPoints}/{player._MaxHitPoints} ";
			List<string> healLineOutput = new List<string>() {
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				playerHealthString};
			int hitPointMaxUnits = player._MaxHitPoints / 10;
			int hitPointUnits = player._HitPoints / hitPointMaxUnits;
			for (int i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputController.Display.StoreUserOutput(healLineOutput);
			switch (player._PlayerClass) {
				case Player.PlayerClassType.Mage:
					string playerManaString = $"Mana: {player._ManaPoints}/{player._MaxManaPoints} ";
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
					int? manaPointMaxUnits = player._MaxManaPoints / 10;
					int? manaPointUnits = player._ManaPoints / manaPointMaxUnits;
					for (int i = 0; i < manaPointUnits; i++) {
						manaLineOutput.Add(Settings.FormatGeneralInfoText());
						manaLineOutput.Add(Settings.FormatManaBackground());
						manaLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(manaLineOutput);
					break;
				case Player.PlayerClassType.Warrior:
					string playerRageString = $"Rage: {player._RagePoints}/{player._MaxRagePoints} ";
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
					int? ragePointMaxUnits = player._MaxRagePoints / 10;
					int? ragePointUnits = player._RagePoints / ragePointMaxUnits;
					for (int i = 0; i < ragePointUnits; i++) {
						rageLineOutput.Add(Settings.FormatGeneralInfoText());
						rageLineOutput.Add(Settings.FormatRageBackground());
						rageLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(rageLineOutput);
					break;
				case Player.PlayerClassType.Archer:
					string playerComboString = $"Combo: {player._ComboPoints}/{player._MaxComboPoints} ";
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
					int? comboPointMaxUnits = player._MaxComboPoints / 10;
					int? comboPointUnits = player._ComboPoints / comboPointMaxUnits;
					for (int i = 0; i < comboPointUnits; i++) {
						comboLineOutput.Add(Settings.FormatGeneralInfoText());
						comboLineOutput.Add(Settings.FormatComboBackground());
						comboLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(comboLineOutput);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			string expString = $"EXP: {player._Experience}";
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
			int expPointMaxUnits = player._ExperienceToLevel / 10;
			int expPointUnits = player._Experience / expPointMaxUnits;
			for (int i = 0; i < expPointUnits; i++) {
				expLineOutput.Add(Settings.FormatGeneralInfoText());
				expLineOutput.Add(Settings.FormatExpBackground());
				expLineOutput.Add("    ");
			}
			OutputController.Display.StoreUserOutput(expLineOutput);
			string baseStatsString = $"Str: {player._Strength} Int: {player._Intelligence} Dex: {player._Dexterity} _Level: {player._Level}";
			StringBuilder statsSb = new StringBuilder(baseStatsString);
			if (player._PlayerClass == Player.PlayerClassType.Archer) {
				statsSb.Append($" Arrows: {player._PlayerQuiver?.Quantity}");
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				statsSb.ToString());
			string resistString = $"Fire Resist: {player._FireResistance} Frost Resist: {player._FrostResistance} Arcane Resist: {player._ArcaneResistance}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				resistString);
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
		}
		public static void ListAbilities(Player player) {
			if (player._PlayerClass != Player.PlayerClassType.Mage) {
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"You have the following abilities:");
				foreach (PlayerAbility ability in player._Abilities) {
					string abilityName = textInfo.ToTitleCase(ability._Name);
					string abilityString = $"{abilityName}, Rank {ability._Rank}";
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityString);
				}
			} else if (player._PlayerClass == Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void ListSpells(Player player) {
			if (player._PlayerClass == Player.PlayerClassType.Mage) {
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your spellbook contains:");
				foreach (PlayerSpell spell in player._Spellbook) {
					string spellName = textInfo.ToTitleCase(spell._Name);
					string spellString = $"{spellName}, Rank {spell._Rank}";
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellString);
				}
			} else if (player._PlayerClass != Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input) {
			string inputName = InputController.ParseInput(input);
			int index = player._Abilities.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player._PlayerClass == Player.PlayerClassType.Warrior) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Abilities[index]._Name));
				string rankString = $"Rank: {player._Abilities[index]._Rank}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string rageCostString = $"Rage Cost: {player._Abilities[index]._RageCost}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rageCostString);
				switch (player._Abilities[index]._WarAbilityCategory) {
					case PlayerAbility.WarriorAbility.Slash:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Rend:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Charge:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Block:
						PlayerAbility.DefenseAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Berserk:
						PlayerAbility.BerserkAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Disarm:
						PlayerAbility.DisarmAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.PowerAura:
						PlayerAbility.PowerAuraAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.WarCry:
						PlayerAbility.WarCryAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Onslaught:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player._PlayerClass == Player.PlayerClassType.Archer) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Abilities[index]._Name));
				string rankString = $"Rank: {player._Abilities[index]._Rank}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string comboCostString = $"Combo Cost: {player._Abilities[index]._ComboCost}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					comboCostString);
				switch (player._Abilities[index]._ArcAbilityCategory) {
					case PlayerAbility.ArcherAbility.Distance:
						PlayerAbility.DistanceAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Gut:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Precise:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Stun:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Double:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Wound:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.SwiftAura:
						PlayerAbility.SwiftAuraAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Ambush:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player._PlayerClass == Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			} else {
				OutputController.Display.StoreUserOutput(
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
			int index = player._Spellbook.FindIndex(f =>
				f._Name == inputName.ToString() || f._Name == input[1] || f._Name.Contains(inputName.ToString()));
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player._PlayerClass == Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Spellbook[index]._Name));
				string rankString = $"Rank: {player._Spellbook[index]._Rank}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				string manaCostString = $"Mana Cost: {player._Spellbook[index]._ManaCost}";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					manaCostString);
				switch (player._Spellbook[index]._SpellCategory) {
					case PlayerSpell.SpellType.Fireball:
						PlayerSpell.FireOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Frostbolt:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Lightning:
						PlayerSpell.ArcaneOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Heal:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Rejuvenate:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Diamondskin:
						PlayerSpell.AugmentArmorSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.TownPortal:
						PlayerSpell.PortalSpellInfo();
						break;
					case PlayerSpell.SpellType.Reflect:
						PlayerSpell.ReflectDamageSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.ArcaneIntellect:
						PlayerSpell.ArcaneIntellectSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.FrostNova:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else if (index != -1 && player._PlayerClass != Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			} else {
				OutputController.Display.StoreUserOutput(
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
			int index = player._QuestLog.FindIndex(f =>
				f._Name.ToLower() == inputName.ToString() || f._Name.ToLower() == input[1] ||
				f._Name.ToLower().Contains(inputName.ToString()));
			if (index != -1) {
				player._QuestLog[index].ShowQuest();
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that quest.");
			}
		}
		public static int CalculateAbilityDamage(Player player, Monster opponent, int index) {
			if (player._Abilities[index]._DamageGroup == PlayerAbility.DamageType.Physical) {
				return player._Abilities[index]._Offensive._Amount;
			}
			double damageReductionPercentage = player._Abilities[index]._DamageGroup switch {
				PlayerAbility.DamageType.Fire => opponent.FireResistance / 100.0,
				PlayerAbility.DamageType.Frost => opponent.FrostResistance / 100.0,
				PlayerAbility.DamageType.Arcane => opponent.ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(player._Abilities[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index) {
			if (player._Spellbook[index]._DamageGroup == PlayerSpell.DamageType.Physical) {
				return player._Spellbook[index]._Offensive._Amount;
			}
			double damageReductionPercentage = player._Spellbook[index]._DamageGroup switch {
				PlayerSpell.DamageType.Fire => opponent.FireResistance / 100.0,
				PlayerSpell.DamageType.Frost => opponent.FrostResistance / 100.0,
				PlayerSpell.DamageType.Arcane => opponent.ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(player._Spellbook[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
	}
}