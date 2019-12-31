using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonGame {
	public static class PlayerHelper {
		public static void ShowInventory(Player player, UserOutput output) {
			output.StoreUserOutput("white", "black","Your inventory contains:" );
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in player.Inventory) {
				if (!item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				var itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver"))
					itemInfo.Append(" (Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity + ")");
				itemInfo.Append(" <Equipped>");
				output.StoreUserOutput("white", "black",itemInfo.ToString());
			}
			foreach (var item in player.Inventory) {
				if (item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				var itemInfo = new StringBuilder(itemName);
				if (player.PlayerQuiver.Name == itemName)
					itemInfo.Append("Arrows: " + player.PlayerQuiver.Quantity + "/" + player.PlayerQuiver.MaxQuantity);
				output.StoreUserOutput("white", "black",itemInfo.ToString());
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
				output.StoreUserOutput("white", "black",consumableString);
			}
			var goldString = "Gold: " + player.Gold + " coins.";
			output.StoreUserOutput("white", "black",goldString);
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
			output.StoreUserOutput("cyan", "black", levelUpString);
		}
		public static void DisplayPlayerStats(Player player, UserOutput output) {
			Helper.FormatGeneralInfoText();
			output.StoreUserOutput(
				"darkgreen",
				"black",
				"==================================================");
			var playerHealthString = "Health: " + player.HitPoints + "/" + player.MaxHitPoints + " ";
			var sameLineOutput = new List<string>() {"darkgreen", "black", playerHealthString};
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					var playerManaString = "Mana: " + player.ManaPoints + "/" + player.MaxManaPoints + " ";
					sameLineOutput.Add("darkgreen");
					sameLineOutput.Add("black");
					sameLineOutput.Add(playerManaString);
					break;
				case Player.PlayerClassType.Warrior:
					var playerRageString = "Rage: " + player.RagePoints + "/" + player.MaxRagePoints + " ";
					sameLineOutput.Add("darkgreen");
					sameLineOutput.Add("black");
					sameLineOutput.Add(playerRageString);
					break;
				case Player.PlayerClassType.Archer:
					var playerComboString = "Combo: " + player.ComboPoints + "/" + player.MaxComboPoints + " ";
					sameLineOutput.Add("darkgreen");
					sameLineOutput.Add("black");
					sameLineOutput.Add(playerComboString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var expLevelString = "EXP: " +  player.Experience + " LVL: " + player.Level;
			sameLineOutput.Add("darkgreen");
			sameLineOutput.Add("black");
			sameLineOutput.Add(expLevelString);
			output.StoreUserOutput(sameLineOutput);
			output.StoreUserOutput(
				"darkgreen",
				"black",
				"==================================================");
		}
		public static void ListAbilities(Player player) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("You have the following abilities:");
				foreach (var ability in player.Abilities) {
					var abilityName = textInfo.ToTitleCase(ability.GetName());
					Console.WriteLine("{0}, Rank {1}", abilityName, ability.Rank);
				}
			}
			else if (player.PlayerClass == Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a warrior or archer!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't list that.");
			}
		}
		public static void ListSpells(Player player) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("Your spellbook contains:");
				foreach (var spell in player.Spellbook) {
					var spellName = textInfo.ToTitleCase(spell.GetName());
					Console.WriteLine("{0}, Rank {1}", spellName, spell.Rank);
				}
			}
			else if (player.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input, UserOutput output) {
			var inputName = Helper.ParseInput(input);
			var index = player.Abilities.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Warrior) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(player.Abilities[index].Name));
				Console.WriteLine("Rank: {0}", player.Abilities[index].Rank);
				Console.WriteLine("Rage Cost: {0}", player.Abilities[index].RageCost);
				switch (player.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Rend:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Charge:
						Ability.StunAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Block:
						Ability.DefenseAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Berserk:
						Ability.BerserkAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Disarm:
						Ability.DisarmAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Archer) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(player.Abilities[index].Name));
				Console.WriteLine("Rank: {0}", player.Abilities[index].Rank);
				Console.WriteLine("Combo Cost: {0}", player.Abilities[index].ComboCost);
				switch (player.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						break;
					case Ability.ShotType.Gut:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ShotType.Precise:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ShotType.Stun:
						Ability.StunAbilityInfo(player, index);
						break;
					case Ability.ShotType.Double:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ShotType.Wound:
						Ability.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a warrior or archer!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that ability.");
			}
		}
		public static void SpellInfo(Player player, string input) {
			var index = player.Spellbook.FindIndex(f => f.GetName() == input);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Mage) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(player.Spellbook[index].Name));
				Console.WriteLine("Rank: {0}", player.Spellbook[index].Rank);
				Console.WriteLine("Mana Cost: {0}", player.Spellbook[index].ManaCost);
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
						Spell.DefenseSpellInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that spell.");
			}
		}
	}
}