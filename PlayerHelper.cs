using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public static class PlayerHelper {
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
		public static void ShowInventory(Player player, UserOutput output) {
			output.StoreUserOutput(
				Helper.FormatInfoText(), 
				Helper.FormatDefaultBackground(),
				"Your inventory contains:" );
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in player.Inventory) {
				if (!item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				var itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver"))
					itemInfo.Append(" (Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity + ")");
				itemInfo.Append(" <Equipped>");
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			foreach (var item in player.Inventory) {
				if (item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				var itemInfo = new StringBuilder(itemName);
				if (player.PlayerQuiver?.Name == itemName)
					itemInfo.Append("Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity);
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (var item in player.Consumables) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName());
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
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(),
					consumableString);
			}
			var goldString = "Gold: " + player.Gold + " coins.";
			output.StoreUserOutput(
				Helper.FormatInfoText(), 
				Helper.FormatDefaultBackground(),
				goldString);
			var weightString = "Weight: " + GetInventoryWeight(player) + "/" + player.MaxCarryWeight;
			output.StoreUserOutput(
				Helper.FormatInfoText(), 
				Helper.FormatDefaultBackground(),
				weightString);
		}
		public static string GetInventoryName(Player player, IEquipment item) {
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var itemInfo = new StringBuilder();
			itemInfo.Append(item.GetName().ToString());
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
		public static void LevelUpCheck(Player player, UserOutput output) {
			if (player.Experience < player.ExperienceToLevel) return;
			player.Level += 1;
			player.Experience -= player.ExperienceToLevel;
			player.ExperienceToLevel *= 2;
			var levelUpString = "You have leveled! You are now level " + player.Level + ".";
			output.StoreUserOutput(
				Helper.FormatLevelUpText(), 
				Helper.FormatDefaultBackground(), 
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
				DisplayPlayerStats(player, output);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatString);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatInfo);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatStr);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatDex);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatInt);
				output.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					levelUpStatConst);
				output.RetrieveUserOutput();
				output.ClearUserOutput();
				var statNumber = 0;
				try {
					var input = Helper.GetFormattedInput(Console.ReadLine());
					if (input.Length > 1) {
						if (Helper.IsWholeNumber(input[1]) == false) continue;
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
					output.StoreUserOutput(
						Helper.FormatAnnounceText(),
						Helper.FormatDefaultBackground(),
						"You did not select an appropriate stat!");
				}
			}
			output.StoreUserOutput(
				Helper.FormatAnnounceText(),
				Helper.FormatDefaultBackground(),
				"All stats have been assigned!");
			// Increase stats from level
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
			player.MaxCarryWeight = player.Strength * 6;
			player.DodgeChance = player.Dexterity * 1.5;
			// Leveling sets player back to max stats
			player.HitPoints = player.MaxHitPoints;
			player.RagePoints = player.MaxRagePoints;
			player.ComboPoints = player.MaxComboPoints;
			player.ManaPoints = player.MaxManaPoints;
			
		}
		public static void DisplayPlayerStats(Player player, UserOutput output) {
			Helper.FormatGeneralInfoText();
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				Helper.FormatTextBorder());
			var playerHealthString = "Health: " + player.HitPoints + "/" + player.MaxHitPoints + " ";
			var healLineOutput = new List<string>() {
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				playerHealthString};
			var hitPointMaxUnits = player.MaxHitPoints / 10;
			var hitPointUnits = player.HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Helper.FormatGeneralInfoText());
				healLineOutput.Add(Helper.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			output.StoreUserOutput(healLineOutput);
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					var playerManaString = "Mana: " + player.ManaPoints + "/" + player.MaxManaPoints + " ";
					var manaLineOutput = new List<string>();
					manaLineOutput.Add(Helper.FormatGeneralInfoText());
					manaLineOutput.Add(Helper.FormatDefaultBackground());
					manaLineOutput.Add(playerManaString);
					var manaBufferAmount = playerHealthString.Length - playerManaString.Length;
					var manaBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < manaBufferAmount; b++) {
						manaBufferStringBuilder.Append(" ");
					}
					manaLineOutput.Add(Helper.FormatGeneralInfoText());
					manaLineOutput.Add(Helper.FormatDefaultBackground());
					manaLineOutput.Add(manaBufferStringBuilder.ToString());
					var manaPointMaxUnits = player.MaxManaPoints / 10;
					var manaPointUnits = player.ManaPoints / manaPointMaxUnits;
					for (var i = 0; i < manaPointUnits; i++) {
						manaLineOutput.Add(Helper.FormatGeneralInfoText());
						manaLineOutput.Add(Helper.FormatManaBackground());
						manaLineOutput.Add("    ");
					}
					output.StoreUserOutput(manaLineOutput);
					break;
				case Player.PlayerClassType.Warrior:
					var playerRageString = "Rage: " + player.RagePoints + "/" + player.MaxRagePoints + " ";
					var rageLineOutput = new List<string>();
					rageLineOutput.Add(Helper.FormatGeneralInfoText());
					rageLineOutput.Add(Helper.FormatDefaultBackground());
					rageLineOutput.Add(playerRageString);
					var rageBufferAmount = playerHealthString.Length - playerRageString.Length;
					var rageBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < rageBufferAmount; b++) {
						rageBufferStringBuilder.Append(" ");
					}
					rageLineOutput.Add(Helper.FormatGeneralInfoText());
					rageLineOutput.Add(Helper.FormatDefaultBackground());
					rageLineOutput.Add(rageBufferStringBuilder.ToString());
					var ragePointMaxUnits = player.MaxRagePoints / 10;
					var ragePointUnits = player.RagePoints / ragePointMaxUnits;
					for (var i = 0; i < ragePointUnits; i++) {
						rageLineOutput.Add(Helper.FormatGeneralInfoText());
						rageLineOutput.Add(Helper.FormatRageBackground());
						rageLineOutput.Add("    ");
					}
					output.StoreUserOutput(rageLineOutput);
					break;
				case Player.PlayerClassType.Archer:
					var playerComboString = "Combo: " + player.ComboPoints + "/" + player.MaxComboPoints + " ";
					var comboLineOutput = new List<string>();
					comboLineOutput.Add(Helper.FormatGeneralInfoText());
					comboLineOutput.Add(Helper.FormatDefaultBackground());
					comboLineOutput.Add(playerComboString);
					var comboBufferAmount = playerHealthString.Length - playerComboString.Length;
					var comboBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < comboBufferAmount; b++) {
						comboBufferStringBuilder.Append(" ");
					}
					comboLineOutput.Add(Helper.FormatGeneralInfoText());
					comboLineOutput.Add(Helper.FormatDefaultBackground());
					comboLineOutput.Add(comboBufferStringBuilder.ToString());
					var comboPointMaxUnits = player.MaxComboPoints / 10;
					var comboPointUnits = player.ComboPoints / comboPointMaxUnits;
					for (var i = 0; i < comboPointUnits; i++) {
						comboLineOutput.Add(Helper.FormatGeneralInfoText());
						comboLineOutput.Add(Helper.FormatComboBackground());
						comboLineOutput.Add("    ");
					}
					output.StoreUserOutput(comboLineOutput);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var expString = "EXP: " +  player.Experience;
			var expLineOutput = new List<string>();
			expLineOutput.Add(Helper.FormatGeneralInfoText());
			expLineOutput.Add(Helper.FormatDefaultBackground());
			expLineOutput.Add(expString);
			var expBufferAmount = playerHealthString.Length - expString.Length;
			var expBufferStringBuilder = new StringBuilder();
			for (var b = 0; b < expBufferAmount; b++) {
				expBufferStringBuilder.Append(" ");
			}
			expLineOutput.Add(Helper.FormatGeneralInfoText());
			expLineOutput.Add(Helper.FormatDefaultBackground());
			expLineOutput.Add(expBufferStringBuilder.ToString());
			var expPointMaxUnits = player.ExperienceToLevel / 10;
			var expPointUnits = player.Experience / expPointMaxUnits;
			for (var i = 0; i < expPointUnits; i++) {
				expLineOutput.Add(Helper.FormatGeneralInfoText());
				expLineOutput.Add(Helper.FormatExpBackground());
				expLineOutput.Add("    ");
			}
			output.StoreUserOutput(expLineOutput);
			var statsString =  "Str: " + player.Strength + " Int: " + player.Intelligence +
			                  " Dex: " + player.Dexterity + " Level: " + player.Level;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				statsString);
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				Helper.FormatTextBorder());
		}
		public static void ListAbilities(Player player, UserOutput output) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(), 
					"You have the following abilities:");
				foreach (var ability in player.Abilities) {
					var abilityName = textInfo.ToTitleCase(ability.GetName());
					var abilityString = abilityName + ", Rank " + ability.Rank;
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						abilityString);
				}
			}
			else if (player.PlayerClass == Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You're not a warrior or archer!");
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You can't list that.");
			}
		}
		public static void ListSpells(Player player, UserOutput output) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"Your spellbook contains:");
				foreach (var spell in player.Spellbook) {
					var spellName = textInfo.ToTitleCase(spell.GetName());
					var spellString = spellName + ", Rank " + spell.Rank;
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						spellString);
				}
			}
			else if (player.PlayerClass != Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You're not a mage!");
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input, UserOutput output) {
			var inputName = Helper.ParseInput(input);
			var index = player.Abilities.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Warrior) {
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Abilities[index].Name));
				var rankString = "Rank: " + player.Abilities[index].Rank;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					rankString);
				var rageCostString = "Rage Cost: " + player.Abilities[index].RageCost;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					rageCostString);
				switch (player.Abilities[index].WarAbilityCategory) {
					case Ability.WarriorAbility.Slash:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Rend:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Charge:
						Ability.StunAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Block:
						Ability.DefenseAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Berserk:
						Ability.BerserkAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Disarm:
						Ability.DisarmAbilityInfo(player, index, output);
						break;
					case Ability.WarriorAbility.Bandage:
						Ability.BandageAbilityInfo(player, index, output);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Archer) {
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Abilities[index].Name));
				var rankString = "Rank: " + player.Abilities[index].Rank;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					rankString);
				var comboCostString = "Combo Cost: " + player.Abilities[index].ComboCost;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					comboCostString);
				switch (player.Abilities[index].ArcAbilityCategory) {
					case Ability.ArcherAbility.Distance:
						Ability.DistanceAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Gut:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Precise:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Stun:
						Ability.StunAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Double:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Wound:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ArcherAbility.Bandage:
						Ability.BandageAbilityInfo(player, index, output);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You're not a warrior or archer!");
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You don't have that ability.");
			}
		}
		public static void SpellInfo(Player player, string input, UserOutput output) {
			var index = player.Spellbook.FindIndex(f => f.GetName() == input);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(), 
					textInfo.ToTitleCase(player.Spellbook[index].Name));
				var rankString = "Rank: " + player.Spellbook[index].Rank;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					rankString);
				var manaCostString = "ManaCost: " + player.Spellbook[index].ManaCost;
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					manaCostString);
				switch(player.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						Spell.FireOffenseSpellInfo(player, index, output);
						break;
					case Spell.SpellType.Frostbolt:
						Spell.FrostOffenseSpellInfo(player, index, output);
						break;
					case Spell.SpellType.Lightning:
						Spell.ArcaneOffenseSpellInfo(player, index, output);
						break;
					case Spell.SpellType.Heal:
						Spell.HealingSpellInfo(player, index, output);
						break;
					case Spell.SpellType.Rejuvenate:
						Spell.HealingSpellInfo(player, index, output);
						break;
					case Spell.SpellType.Diamondskin:
						Spell.DefenseSpellInfo(player, index, output);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass != Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You're not a mage!");
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(), 
					Helper.FormatDefaultBackground(), 
					"You don't have that spell.");
			}
		}
	}
}