using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonGame {
	public static class PlayerHelper {
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
			// Increase stats from level
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					player.MaxHitPoints += 15;
					player.MaxManaPoints += 35;
					break;
				case Player.PlayerClassType.Warrior:
					player.MaxHitPoints += 35;
					player.MaxRagePoints += 15;
					break;
				case Player.PlayerClassType.Archer:
					player.MaxHitPoints += 25;
					player.MaxComboPoints += 25;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			// Leveling sets player back to max stats
			player.HitPoints = player.MaxHitPoints;
			player.RagePoints = player.MaxRagePoints;
			player.ComboPoints = player.MaxComboPoints;
			player.ManaPoints = player.MaxManaPoints;
			var levelUpString = "You have leveled! You are now level " + player.Level + ".";
			output.StoreUserOutput(
				Helper.FormatLevelUpText(), 
				Helper.FormatDefaultBackground(), 
				levelUpString); 
		}
		public static void DisplayPlayerStats(Player player, UserOutput output) {
			Helper.FormatGeneralInfoText();
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				"==================================================");
			var playerHealthString = "Health: " + player.HitPoints + "/" + player.MaxHitPoints + " ";
			var sameLineOutput = new List<string>() {
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				playerHealthString};
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					var playerManaString = "Mana: " + player.ManaPoints + "/" + player.MaxManaPoints + " ";
					sameLineOutput.Add(Helper.FormatGeneralInfoText());
					sameLineOutput.Add(Helper.FormatDefaultBackground());
					sameLineOutput.Add(playerManaString);
					break;
				case Player.PlayerClassType.Warrior:
					var playerRageString = "Rage: " + player.RagePoints + "/" + player.MaxRagePoints + " ";
					sameLineOutput.Add(Helper.FormatGeneralInfoText());
					sameLineOutput.Add(Helper.FormatDefaultBackground());
					sameLineOutput.Add(playerRageString);
					break;
				case Player.PlayerClassType.Archer:
					var playerComboString = "Combo: " + player.ComboPoints + "/" + player.MaxComboPoints + " ";
					sameLineOutput.Add(Helper.FormatGeneralInfoText());
					sameLineOutput.Add(Helper.FormatDefaultBackground());
					sameLineOutput.Add(playerComboString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var expLevelString = "EXP: " +  player.Experience + " LVL: " + player.Level;
			sameLineOutput.Add(Helper.FormatGeneralInfoText());
			sameLineOutput.Add(Helper.FormatDefaultBackground());
			sameLineOutput.Add(expLevelString);
			output.StoreUserOutput(sameLineOutput);
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				"==================================================");
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
				switch (player.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.AbilityType.Rend:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.AbilityType.Charge:
						Ability.StunAbilityInfo(player, index, output);
						break;
					case Ability.AbilityType.Block:
						Ability.DefenseAbilityInfo(player, index, output);
						break;
					case Ability.AbilityType.Berserk:
						Ability.BerserkAbilityInfo(player, index, output);
						break;
					case Ability.AbilityType.Disarm:
						Ability.DisarmAbilityInfo(player, index, output);
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
				switch (player.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						break;
					case Ability.ShotType.Gut:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ShotType.Precise:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ShotType.Stun:
						Ability.StunAbilityInfo(player, index, output);
						break;
					case Ability.ShotType.Double:
						Ability.OffenseDamageAbilityInfo(player, index, output);
						break;
					case Ability.ShotType.Wound:
						Ability.OffenseDamageAbilityInfo(player, index, output);
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