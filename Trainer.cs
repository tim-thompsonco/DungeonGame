using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Trainer : ITrainer {
		public enum TrainerCategory {
			Archer,
			Warrior,
			Mage
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public TrainerCategory TrainerGroup { get; set; }
		public int BaseCost { get; set; }
		public List<Ability> TrainableAbilities { get; set; }
		public List<Spell> TrainableSpells { get; set; }

		public Trainer(string name, string desc, TrainerCategory trainerCategory) {
			this.Name = name;
			this.Desc = desc;
			this.BaseCost = 25;
			this.TrainerGroup = trainerCategory;
			switch (this.TrainerGroup) {
				case TrainerCategory.Archer:
					this.TrainableAbilities = new List<Ability>();
					this.TrainableAbilities.Add(new Ability(
						"bandage", 25, 1, Ability.ArcherAbility.Bandage, 2));
					break;
				case TrainerCategory.Warrior:
					this.TrainableAbilities = new List<Ability>();
					this.TrainableAbilities.Add(new Ability(
						"bandage", 25, 1, Ability.WarriorAbility.Bandage, 2));
					break;
				case TrainerCategory.Mage:
					this.TrainableSpells = new List<Spell>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string GetName() {
			return this.Name;
		}
		public void DisplayAvailableUpgrades(Player player, UserOutput output) {
			var forSaleString = "The " + this.Name + " has the following upgrades available:"; 
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				forSaleString);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (this.TrainerGroup == TrainerCategory.Mage) {
				foreach (var spellName in from spell in this.TrainableSpells
					where player.Level >= spell.MinLevel select textInfo.ToTitleCase(
						spell.GetName() + " (Rank: " + spell.Rank + ")")) {
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						spellName);
				}
				foreach (var spellName in from spell in player.Spellbook
					where player.Level >= spell.MinLevel select textInfo.ToTitleCase(
						spell.GetName() + " (Rank: " + spell.Rank + ")")) {
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						spellName);
				}
			}
			else {
				foreach (var abilityName in from ability in this.TrainableAbilities where 
					player.Level >= ability.MinLevel select textInfo.ToTitleCase(
					ability.GetName() + " (Rank: " + ability.Rank + ")")) {
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						abilityName);
				}
				foreach (var abilityName in from ability in player.Abilities where 
					player.Level >= ability.MinLevel select textInfo.ToTitleCase(
					ability.GetName() + " (Rank: " + ability.Rank + ")")) {
					output.StoreUserOutput(
						Helper.FormatInfoText(),
						Helper.FormatDefaultBackground(),
						abilityName);
				}
			}
		}
		public void TrainAbility(Player player, string inputName, UserOutput output) {
		}
		public void TrainSpell(Player player, string inputName, UserOutput output) {
		}
		public void UpgradeSpell(Player player, string inputName, UserOutput output) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't upgrade spells. You're not a mage!");
				return;
			}
			var spellIndex = player.Spellbook.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (spellIndex != -1 && player.Level >= player.Spellbook[spellIndex].Rank + 1) {
				var trainingCost = (int)((player.Spellbook[spellIndex].Rank + 1.0) * this.BaseCost * 
				                      (1.0 - (player.Intelligence / 100.0)));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook[spellIndex].Rank++;
					switch (player.Spellbook[spellIndex].SpellCategory) {
						case Spell.SpellType.Fireball:
							player.Spellbook[spellIndex].FireOffense.BlastDamage += 10;
							player.Spellbook[spellIndex].FireOffense.BurnDamage += 5;
							break;
						case Spell.SpellType.Frostbolt:
							player.Spellbook[spellIndex].FrostOffense.FrostDamage += 10;
							break;
						case Spell.SpellType.Lightning:
							player.Spellbook[spellIndex].ArcaneOffense.ArcaneDamage += 10;
							break;
						case Spell.SpellType.Heal:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case Spell.SpellType.Rejuvenate:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case Spell.SpellType.Diamondskin:
							player.Spellbook[spellIndex].Defense.AugmentAmount += 10;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					var purchaseString = "You upgraded " + player.Spellbook[spellIndex].Name + " to level " + 
					                     player.Spellbook[spellIndex].Rank + " for " + trainingCost + " gold.";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						purchaseString);
					return;
				}
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (spellIndex != -1) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You are not ready to upgrade that spell. You need to level up first!");
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You don't have that spell to train!");
		}
		public void UpgradeAbility(Player player, string inputName, UserOutput output) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't upgrade abilities. You're not a warrior or archer!");
				return;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (abilityIndex != -1 && player.Level >= player.Abilities[abilityIndex].Rank + 1) {
				var trainingCost = (int)((player.Abilities[abilityIndex].Rank + 1.0) * this.BaseCost * 
				                      (1.0 - (player.Intelligence / 100.0)));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities[abilityIndex].Rank++;
					if (player.PlayerClass == Player.PlayerClassType.Archer) {
						switch (player.Abilities[abilityIndex].ArcAbilityCategory) {
							case Ability.ArcherAbility.Distance:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 5;
								break;
							case Ability.ArcherAbility.Gut:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case Ability.ArcherAbility.Precise:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case Ability.ArcherAbility.Stun:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case Ability.ArcherAbility.Double:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case Ability.ArcherAbility.Wound:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case Ability.ArcherAbility.Bandage:
								player.Abilities[abilityIndex].Bandage.HealAmount += 10;
								player.Abilities[abilityIndex].Bandage.HealOverTime += 5;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						var purchaseString = "You upgraded " + player.Abilities[abilityIndex].Name + " to level " + 
						                     player.Abilities[abilityIndex].Rank + " for " + trainingCost + " gold.";
						output.StoreUserOutput(
							Helper.FormatSuccessOutputText(),
							Helper.FormatDefaultBackground(),
							purchaseString);
						return;
					}
					if (player.PlayerClass == Player.PlayerClassType.Warrior) {
						switch (player.Abilities[abilityIndex].WarAbilityCategory) {
							case Ability.WarriorAbility.Slash:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case Ability.WarriorAbility.Rend:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case Ability.WarriorAbility.Charge:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case Ability.WarriorAbility.Block:
								player.Abilities[abilityIndex].Defensive.AbsorbDamage += 10;
								break;
							case Ability.WarriorAbility.Berserk:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].ChangeArmor.ChangeArmorAmount += 10;
								break;
							case Ability.WarriorAbility.Disarm:
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 10;
								break;
							case Ability.WarriorAbility.Bandage:
								player.Abilities[abilityIndex].Bandage.HealAmount += 10;
								player.Abilities[abilityIndex].Bandage.HealOverTime += 5;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (abilityIndex != -1) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You are not ready to upgrade that ability. You need to level up first!");
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You don't have that ability to train!");
		}
	}
}