using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonGame {
	public static class PlayerHelper {
		public static void ShowInventory(Player player) {
			Helper.FormatInfoText();
			Console.WriteLine("Your inventory contains:\n");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in player.Inventory) {
				if (!item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				var itemInfo = new StringBuilder(itemName);
				itemInfo.Append(" <Equipped>");
				Console.WriteLine(itemInfo);
			}
			foreach (var item in player.Inventory) {
				if (item.IsEquipped()) continue;
				var itemName = GetInventoryName(player, item);
				Console.WriteLine(itemName);
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (var item in player.Consumables) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName().ToString());
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
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				if (!consumableDict.ContainsKey(itemName)) {
					consumableDict.Add(itemName, 1);
					continue;
				}
				var dictValue = consumableDict[itemName];
				dictValue += 1;
				consumableDict[itemName] = dictValue;
			}
			foreach (var potion in consumableDict) {
				Console.WriteLine(potion.Key + " (Quantity: {0})", potion.Value);
			}
			Console.WriteLine("Gold: " + player.Gold + " coins.");
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
		public static void LevelUpCheck(Player player) {
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
			Helper.FormatLevelUpText();
			Console.WriteLine("You have leveled! You are now level {0}.", player.Level);
		}
		public static void DisplayPlayerStats(Player player) {
			Helper.FormatGeneralInfoText();
			Console.WriteLine("==================================================");
			Console.Write("Health: {0}/{1} ", player.HitPoints, player.MaxHitPoints);
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					Console.Write("Mana: {0}/{1} ", player.ManaPoints, player.MaxManaPoints);
					break;
				case Player.PlayerClassType.Warrior:
					Console.Write("Rage: {0}/{1} ", player.RagePoints, player.MaxRagePoints);
					break;
				case Player.PlayerClassType.Archer:
					Console.Write("Combo: {0}/{1} ", player.ComboPoints, player.MaxComboPoints);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			Console.WriteLine("EXP: {0} LVL: {1}", player.Experience, player.Level);
			Console.WriteLine("==================================================");
		}
		public static void ListAbilities(Player player) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("You have the following abilities:");
				foreach (var ability in player.Abilities) {
					var abilityName = textInfo.ToTitleCase(ability.GetName().ToString());
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
					var spellName = textInfo.ToTitleCase(spell.GetName().ToString());
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
		public static void AbilityInfo(Player player, string[] input) {
			var inputName = Helper.ParseInput(input);
			var index = player.Abilities.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player.PlayerClass == Player.PlayerClassType.Warrior) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(player.Abilities[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", player.Abilities[index].Rank);
				Console.WriteLine("Rage Cost: {0}", player.Abilities[index].RageCost);
				switch (player.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						player.Abilities[index].OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Rend:
						player.Abilities[index].OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Charge:
						player.Abilities[index].StunAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Block:
						player.Abilities[index].DefenseAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Berserk:
						player.Abilities[index].BerserkAbilityInfo(player, index);
						break;
					case Ability.AbilityType.Disarm:
						player.Abilities[index].DisarmAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player.PlayerClass == Player.PlayerClassType.Archer) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(player.Abilities[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", player.Abilities[index].Rank);
				Console.WriteLine("Combo Cost: {0}", player.Abilities[index].ComboCost);
				switch (player.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						break;
					case Ability.ShotType.Gut:
						player.Abilities[index].OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ShotType.Precise:
						player.Abilities[index].OffenseDamageAbilityInfo(player, index);
						break;
					case Ability.ShotType.Stun:
						player.Abilities[index].StunAbilityInfo(player, index);
						break;
					case Ability.ShotType.Double:
						break;
					case Ability.ShotType.Poison:
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
				Console.WriteLine(textInfo.ToTitleCase(player.Spellbook[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", player.Spellbook[index].Rank);
				Console.WriteLine("Mana Cost: {0}", player.Spellbook[index].ManaCost);
				switch(player.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						player.Spellbook[index].FireOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Frostbolt:
						player.Spellbook[index].FrostOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Lightning:
						player.Spellbook[index].ArcaneOffenseSpellInfo(player, index);
						break;
					case Spell.SpellType.Heal:
						player.Spellbook[index].HealingSpellInfo(player, index);
						break;
					case Spell.SpellType.Rejuvenate:
						player.Spellbook[index].HealingSpellInfo(player, index);
						break;
					case Spell.SpellType.Diamondskin:
						player.Spellbook[index].DefenseSpellInfo(player, index);
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