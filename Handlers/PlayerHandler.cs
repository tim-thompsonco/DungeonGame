using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DungeonGame {
	public static class PlayerHandler {
		public static int GetInventoryWeight(Player player) {
			var weight = 0;
			foreach (var item in player.Inventory) {
				weight += item.Weight;
			}
			foreach (var consumable in player.Consumables) {
				weight += consumable.Weight;
			}
			return weight;
		}
		public static void ShowInventory(Player player) {
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(), 
				Settings.FormatDefaultBackground(),
				"Your inventory contains:" );
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in player.Inventory) {
				if (!item.Equipped) continue;
				var itemName = GetInventoryName(item);
				var itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver"))
					itemInfo.Append(" (Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity + ")");
				itemInfo.Append(" <Equipped>");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			foreach (var item in player.Inventory) {
				if (item.Equipped) continue;
				var itemName = GetInventoryName(item);
				var itemInfo = new StringBuilder(itemName);
				if (player.PlayerQuiver?.Name == itemName)
					itemInfo.Append("Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (var item in player.Consumables) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.Name);
				if (item.Name.Contains("potion")) {
					switch (item.PotionCategory.ToString()) {
						case "Health":
							itemInfo.Append(" (" + item.RestoreHealth.RestoreHealthAmt + ")");
							break;
						case "Mana":
							itemInfo.Append(" (" + item.RestoreMana.RestoreManaAmt + ")");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				if (item.Name.Contains("arrow")) {
					itemInfo.Append(" (" + item.Arrow.Quantity + ")");
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				if (!consumableDict.ContainsKey(itemName)) {
					consumableDict.Add(itemName, 1);
					continue;
				}
				var dictValue = consumableDict[itemName];
				dictValue += 1;
				consumableDict[itemName] = dictValue;
			}
			foreach (var consumable in consumableDict) {
				var consumableString = consumable.Key + " (Quantity: " + consumable.Value + ")";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(),
					consumableString);
			}
			var goldString = "Gold: " + player.Gold + " coins.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(), 
				Settings.FormatDefaultBackground(),
				goldString);
			var weightString = "Weight: " + GetInventoryWeight(player) + "/" + player.MaxCarryWeight;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(), 
				Settings.FormatDefaultBackground(),
				weightString);
		}
		public static string GetInventoryName(IEquipment item) {
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var itemInfo = new StringBuilder();
			itemInfo.Append(item.Name);
			var isItemArmor = item as Armor;
			if (isItemArmor != null) {
				itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + ")");
			}
			var isItemWeapon = item as Weapon;
			if (isItemWeapon != null) {
				itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + ")");
			}
			var itemName = textInfo.ToTitleCase(itemInfo.ToString());
			return itemName;
		}
		public static void LevelUpCheck(Player player) {
			if (player.Experience < player.ExperienceToLevel || player.Level == 10) return;
			player.Level += 1;
			player.Experience -= player.ExperienceToLevel;
			player.ExperienceToLevel *= 2;
			var levelUpString = "You have leveled! You are now level " + player.Level + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatLevelUpText(), 
				Settings.FormatDefaultBackground(), 
				levelUpString); 
			var statsToAssign = 5;
			while (statsToAssign > 0) {
				var levelUpStatString = "Please choose " + statsToAssign + 
				                        " stats to raise. Your choices are: str, dex, int, const.";
				const string levelUpStatInfo = 
					"You may raise a stat more than once by putting a number after the stat, IE str 2.";
				const string levelUpStatStr = "Str will increase your max carrying weight and warrior abilities.";
				const string levelUpStatDex = "Dex will increase your dodge chance and archer abilities";
				const string levelUpStatInt = 
					"Int will increase your mana and decrease your training cost for spells and abilities.";
				const string levelUpStatConst = "Const will increase your max hit points.";
				DisplayPlayerStats(player);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatString);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInfo);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatStr);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatDex);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInt);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatConst);
				OutputHandler.Display.BuildUserOutput();
				OutputHandler.Display.ClearUserOutput();
				var statNumber = 0;
				try {
					var input = InputHandler.GetFormattedInput(Console.ReadLine());
					if (input.Length > 1) {
						if (GameHandler.IsWholeNumber(input[1]) == false) continue;
						statNumber = Convert.ToInt32(input[1]);
					} 
					switch (input[0]) {
						case "str":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Strength += statNumber;
								statsToAssign -= statNumber;
							}
							else {
								player.Strength++;
								statsToAssign--;
							}
							break;
						case "dex":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Dexterity += statNumber;
								statsToAssign -= statNumber;
							}
							else {
								player.Dexterity++;
								statsToAssign--;
							}
							break;
						case "int":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Intelligence += statNumber;
								statsToAssign -= statNumber;
							}
							else {
								player.Intelligence++;
								statsToAssign--;
							}
							break;
						case "const":
							if (statNumber > 0 && statNumber <= statsToAssign) {
								player.Constitution += statNumber;
								statsToAssign -= statNumber;
							}
							else {
								player.Constitution++;
								statsToAssign--;
							}
							break;
					}
				}
				catch (IndexOutOfRangeException) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),
						Settings.FormatDefaultBackground(),
						"You did not select an appropriate stat!");
				}
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(),
				"All stats have been assigned!");
			CalculatePlayerStats(player);
			// Leveling sets player back to max stats
			player.HitPoints = player.MaxHitPoints;
			player.RagePoints = player.MaxRagePoints;
			player.ComboPoints = player.MaxComboPoints;
			player.ManaPoints = player.MaxManaPoints;
		}
		public static void CalculatePlayerStats(Player player) {
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					player.MaxManaPoints = player.Intelligence * 10;
					break;
				case Player.PlayerClassType.Warrior:
					player.MaxRagePoints = player.Strength * 10;
					break;
				case Player.PlayerClassType.Archer:
					player.MaxComboPoints = player.Dexterity * 10;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			player.MaxHitPoints = player.Constitution * 10;
			player.MaxCarryWeight = player.Strength * 2;
			player.DodgeChance = player.Dexterity * 1.5;			
		}
		public static void DisplayPlayerStats(Player player) {
			Settings.FormatGeneralInfoText();
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			var playerHealthString = "Health: " + player.HitPoints + "/" + player.MaxHitPoints + " ";
			var healLineOutput = new List<string>() {
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				playerHealthString};
			var hitPointMaxUnits = player.MaxHitPoints / 10;
			var hitPointUnits = player.HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputHandler.Display.StoreUserOutput(healLineOutput);
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					var playerManaString = "Mana: " + player.ManaPoints + "/" + player.MaxManaPoints + " ";
					var manaLineOutput = new List<string>();
					manaLineOutput.Add(Settings.FormatGeneralInfoText());
					manaLineOutput.Add(Settings.FormatDefaultBackground());
					manaLineOutput.Add(playerManaString);
					var manaBufferAmount = playerHealthString.Length - playerManaString.Length;
					var manaBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < manaBufferAmount; b++) {
						manaBufferStringBuilder.Append(" ");
					}
					manaLineOutput.Add(Settings.FormatGeneralInfoText());
					manaLineOutput.Add(Settings.FormatDefaultBackground());
					manaLineOutput.Add(manaBufferStringBuilder.ToString());
					var manaPointMaxUnits = player.MaxManaPoints / 10;
					var manaPointUnits = player.ManaPoints / manaPointMaxUnits;
					for (var i = 0; i < manaPointUnits; i++) {
						manaLineOutput.Add(Settings.FormatGeneralInfoText());
						manaLineOutput.Add(Settings.FormatManaBackground());
						manaLineOutput.Add("    ");
					}
					OutputHandler.Display.StoreUserOutput(manaLineOutput);
					break;
				case Player.PlayerClassType.Warrior:
					var playerRageString = "Rage: " + player.RagePoints + "/" + player.MaxRagePoints + " ";
					var rageLineOutput = new List<string>();
					rageLineOutput.Add(Settings.FormatGeneralInfoText());
					rageLineOutput.Add(Settings.FormatDefaultBackground());
					rageLineOutput.Add(playerRageString);
					var rageBufferAmount = playerHealthString.Length - playerRageString.Length;
					var rageBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < rageBufferAmount; b++) {
						rageBufferStringBuilder.Append(" ");
					}
					rageLineOutput.Add(Settings.FormatGeneralInfoText());
					rageLineOutput.Add(Settings.FormatDefaultBackground());
					rageLineOutput.Add(rageBufferStringBuilder.ToString());
					var ragePointMaxUnits = player.MaxRagePoints / 10;
					var ragePointUnits = player.RagePoints / ragePointMaxUnits;
					for (var i = 0; i < ragePointUnits; i++) {
						rageLineOutput.Add(Settings.FormatGeneralInfoText());
						rageLineOutput.Add(Settings.FormatRageBackground());
						rageLineOutput.Add("    ");
					}
					OutputHandler.Display.StoreUserOutput(rageLineOutput);
					break;
				case Player.PlayerClassType.Archer:
					var playerComboString = "Combo: " + player.ComboPoints + "/" + player.MaxComboPoints + " ";
					var comboLineOutput = new List<string>();
					comboLineOutput.Add(Settings.FormatGeneralInfoText());
					comboLineOutput.Add(Settings.FormatDefaultBackground());
					comboLineOutput.Add(playerComboString);
					var comboBufferAmount = playerHealthString.Length - playerComboString.Length;
					var comboBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < comboBufferAmount; b++) {
						comboBufferStringBuilder.Append(" ");
					}
					comboLineOutput.Add(Settings.FormatGeneralInfoText());
					comboLineOutput.Add(Settings.FormatDefaultBackground());
					comboLineOutput.Add(comboBufferStringBuilder.ToString());
					var comboPointMaxUnits = player.MaxComboPoints / 10;
					var comboPointUnits = player.ComboPoints / comboPointMaxUnits;
					for (var i = 0; i < comboPointUnits; i++) {
						comboLineOutput.Add(Settings.FormatGeneralInfoText());
						comboLineOutput.Add(Settings.FormatComboBackground());
						comboLineOutput.Add("    ");
					}
					OutputHandler.Display.StoreUserOutput(comboLineOutput);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var expString = "EXP: " +  player.Experience;
			var expLineOutput =
				new List<string> {Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), expString};
			var expBufferAmount = playerHealthString.Length - expString.Length;
			var expBufferStringBuilder = new StringBuilder();
			for (var b = 0; b < expBufferAmount; b++) {
				expBufferStringBuilder.Append(" ");
			}
			expLineOutput.Add(Settings.FormatGeneralInfoText());
			expLineOutput.Add(Settings.FormatDefaultBackground());
			expLineOutput.Add(expBufferStringBuilder.ToString());
			var expPointMaxUnits = player.ExperienceToLevel / 10;
			var expPointUnits = player.Experience / expPointMaxUnits;
			for (var i = 0; i < expPointUnits; i++) {
				expLineOutput.Add(Settings.FormatGeneralInfoText());
				expLineOutput.Add(Settings.FormatExpBackground());
				expLineOutput.Add("    ");
			}
			OutputHandler.Display.StoreUserOutput(expLineOutput);
			var statsString =  "Str: " + player.Strength + " Int: " + player.Intelligence +
			                  " Dex: " + player.Dexterity + " Level: " + player.Level;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				statsString);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
		}
		public static void ListAbilities(Player player) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(), 
					"You have the following abilities:");
				foreach (var ability in player.Abilities) {
					var abilityName = textInfo.ToTitleCase(ability.Name);
					var abilityString = abilityName + ", Rank " + ability.Rank;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityString);
				}
			}
			else if (player.PlayerClass == Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You're not a warrior or archer!");
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You can't list that.");
			}
		}
		public static void ListSpells(Player player) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"Your spellbook contains:");
				foreach (var spell in player.Spellbook) {
					var spellName = textInfo.ToTitleCase(spell.Name);
					var spellString = spellName + ", Rank " + spell.Rank;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellString);
				}
			}
			else if (player.PlayerClass != Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You're not a mage!");
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input) {
			var inputName = InputHandler.ParseInput(input);
			var index = player.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Warrior) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Abilities[index].Name));
				var rankString = "Rank: " + player.Abilities[index].Rank;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var rageCostString = "Rage Cost: " + player.Abilities[index].RageCost;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rageCostString);
				switch (player.Abilities[index].WarAbilityCategory) {
					case Ability.WarriorAbility.Slash:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Rend:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Charge:
						Ability.StunAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Block:
						Ability.DefenseAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Berserk:
						Ability.BerserkAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Disarm:
						Ability.DisarmAbilityInfo(player, index);
						break;
					case Ability.WarriorAbility.Bandage:
						Ability.BandageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Archer) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Abilities[index].Name));
				var rankString = "Rank: " + player.Abilities[index].Rank;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var comboCostString = "Combo Cost: " + player.Abilities[index].ComboCost;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					comboCostString);
				switch (player.Abilities[index].ArcAbilityCategory) {
					case Ability.ArcherAbility.Distance:
						Ability.DistanceAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Gut:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Precise:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Stun:
						Ability.StunAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Double:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Wound:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.Bandage:
						Ability.BandageAbilityInfo(player, index);
						break;
					case Ability.ArcherAbility.SwiftAura:
						Ability.SwiftAuraAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You're not a warrior or archer!");
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You don't have that ability.");
			}
		}
		public static void SpellInfo(Player player, string input) {
			var index = player.Spellbook.FindIndex(f => f.Name == input || 
			                                            f.SpellCategory == Spell.SpellType.TownPortal);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Spellbook[index].Name));
				var rankString = "Rank: " + player.Spellbook[index].Rank;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var manaCostString = "Mana Cost: " + player.Spellbook[index].ManaCost;
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					manaCostString);
				switch(player.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						Spell.FireOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Frostbolt:
						Spell.FrostOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Lightning:
						Spell.ArcaneOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Heal:
						Spell.HealingSpellInfo(player, index);
						break;
					case Spell.SpellType.Rejuvenate:
						Spell.HealingSpellInfo(player, index);
						break;
					case Spell.SpellType.Diamondskin:
						Spell.AugmentArmorSpellInfo(player, index);
						break;
					case Spell.SpellType.TownPortal:
						Spell.PortalSpellInfo();
						break;
					case Spell.SpellType.Reflect:
						Spell.ReflectDamageSpellInfo(player, index);
						break;
					case Spell.SpellType.ArcaneIntellect:
						Spell.ArcaneIntellectSpellInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass != Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You're not a mage!");
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(), 
					Settings.FormatDefaultBackground(), 
					"You don't have that spell.");
			}
		}
	}
}