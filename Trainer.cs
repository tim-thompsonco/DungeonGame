using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DungeonGame {
	public class Trainer : IRoomInteraction {
		public enum TrainerCategory {
			Archer,
			Warrior,
			Mage
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public TrainerCategory TrainerGroup { get; set; }
		public int BaseCost { get; set; }
		public List<PlayerAbility> TrainableAbilities { get; set; }
		public List<PlayerSpell> TrainableSpells { get; set; }

		// Default constructor for JSON serialization
		public Trainer() { }
		public Trainer(string name, string desc, TrainerCategory trainerCategory) {
			this.Name = name;
			this.Desc = desc;
			this.BaseCost = 25;
			this.TrainerGroup = trainerCategory;
			switch (this.TrainerGroup) {
				case TrainerCategory.Archer:
					this.TrainableAbilities = new List<PlayerAbility>();
					this.TrainableAbilities.Add(new PlayerAbility(
						"bandage", 25, 1, PlayerAbility.ArcherAbility.Bandage, 2));
					this.TrainableAbilities.Add(new PlayerAbility(
						"ambush", 75, 1, PlayerAbility.ArcherAbility.Ambush, 4));
					this.TrainableAbilities.Add(new PlayerAbility(
						"swift aura", 150, 1, PlayerAbility.ArcherAbility.SwiftAura, 6));
					this.TrainableAbilities.Add(new PlayerAbility(
						"immolating arrow", 35, 1, PlayerAbility.ArcherAbility.ImmolatingArrow, 8));
					break;
				case TrainerCategory.Warrior:
					this.TrainableAbilities = new List<PlayerAbility>();
					this.TrainableAbilities.Add(new PlayerAbility(
						"bandage", 25, 1, PlayerAbility.WarriorAbility.Bandage, 2));
					this.TrainableAbilities.Add(new PlayerAbility(
						"war cry", 50, 1, PlayerAbility.WarriorAbility.WarCry, 4));
					this.TrainableAbilities.Add(new PlayerAbility(
						"power aura", 150, 1, PlayerAbility.WarriorAbility.PowerAura, 6));
					this.TrainableAbilities.Add(new PlayerAbility(
						"onslaught", 25, 1, PlayerAbility.WarriorAbility.Onslaught, 8));
					break;
				case TrainerCategory.Mage:
					this.TrainableSpells = new List<PlayerSpell>();
					this.TrainableSpells.Add(new PlayerSpell(
						"town portal", 100, 1, PlayerSpell.SpellType.TownPortal, 2));
					this.TrainableSpells.Add(new PlayerSpell(
						"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 4));
					this.TrainableSpells.Add(new PlayerSpell(
						"arcane intellect", 150, 1, PlayerSpell.SpellType.ArcaneIntellect, 6));
					this.TrainableSpells.Add(new PlayerSpell(
						"frost nova", 50, 1, PlayerSpell.SpellType.FrostNova, 8));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayAvailableUpgrades(Player player) {
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					if (this.TrainerGroup != TrainerCategory.Mage) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a mage grandmaster!");
						return;
					}
					break;
				case Player.PlayerClassType.Warrior:
					if (this.TrainerGroup != TrainerCategory.Warrior) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a warrior grandmaster!");
						return;
					}
					break;
				case Player.PlayerClassType.Archer:
					if (this.TrainerGroup != TrainerCategory.Archer) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a archer grandmaster!");
						return;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var forSaleString = "The " + this.Name + " has the following upgrades available:"; 
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (this.TrainerGroup == TrainerCategory.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New Spells: ");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (this.TrainableSpells?.Count == 0) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
				else {
					try {
						var newSpellsToTrain = 0;
						foreach (var spell in this.TrainableSpells) {
							if (player.Level < spell.MinLevel) continue;
							var trainingCost = (int)(spell.MinLevel * this.BaseCost * (1.0 - player.Intelligence / 100.0));
							var spellName = textInfo.ToTitleCase(spell.Name + 
							                                     " (Rank: " + spell.Rank + ") (Cost: " + trainingCost + ")");
							newSpellsToTrain++;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								spellName);
						}
						if (newSpellsToTrain == 0) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					}
					catch (ArgumentNullException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing Spells: ");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				var spellsToTrain = 0;
				foreach (var spell in player.Spellbook) {
					if (player.Level < spell.MinLevel && player.Level < spell.Rank) continue;
					var trainingCost = (int)((spell.Rank + 1.0) * this.BaseCost * (1.0 - player.Intelligence / 100.0));
					var spellName = textInfo.ToTitleCase(spell.Name + 
					                                     " (Rank: " + spell.Rank + ") (Cost: " + trainingCost + ")");
					spellsToTrain++;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellName);
				}
				if (spellsToTrain == 0) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New Abilities: ");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (this.TrainableAbilities?.Count == 0) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
				else {
					try {
						var newAbilitiesToTrain = 0;
						foreach (var ability in this.TrainableAbilities) {
							if (player.Level < ability.MinLevel) continue;
							var trainingCost = (int)(ability.MinLevel * this.BaseCost * (1.0 - player.Intelligence / 100.0));
							var abilityName = textInfo.ToTitleCase(ability.Name + 
							                                     " (Rank: " + ability.Rank + ") (Cost: " + trainingCost + ")");
							newAbilitiesToTrain++;
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								abilityName);
						}
						if (newAbilitiesToTrain == 0) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					}
					catch (ArgumentNullException) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing Abilities: ");
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				var abilitiesToTrain = 0;
				foreach (var ability in player.Abilities) {
					if (player.Level < ability.MinLevel) continue;
					var trainingCost = (int)(ability.MinLevel * this.BaseCost * (1.0 - player.Intelligence / 100.0));
					var abilityName = textInfo.ToTitleCase(ability.Name + 
					                                       " (Rank: " + ability.Rank + ") (Cost: " + trainingCost + ")");
					abilitiesToTrain++;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityName);
				}
				if (abilitiesToTrain == 0) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				string.Empty);
			var playerGold = "Player Gold: " + player.Gold;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				playerGold);
		}
		public void TrainAbility(Player player, string inputName) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train abilities. You're not a warrior or archer!");
				return;
			}
			var abilityIndex = this.TrainableAbilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (abilityIndex != -1 && player.Level >= this.TrainableAbilities[abilityIndex].MinLevel) {
				var trainingCost = (int)(this.TrainableAbilities[abilityIndex].MinLevel * this.BaseCost * 
				                         (1.0 - player.Intelligence / 100.0));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities.Add(this.TrainableAbilities[abilityIndex]);
					this.TrainableAbilities.RemoveAt(abilityIndex);
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					var abilityName = textInfo.ToTitleCase(player.Abilities[player.Abilities.Count - 1].Name);
					var purchaseString = "You purchased " + abilityName + " (Rank " + 
					                     player.Abilities[player.Abilities.Count - 1].Rank + ") for " + trainingCost + " gold.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
					return;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (abilityIndex != -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You are not ready to train that ability. You need to level up first!");
			}
		}
		public void TrainSpell(Player player, string inputName) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train spells. You're not a mage!");
				return;
			}
			var spellIndex = this.TrainableSpells.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (spellIndex != -1 && player.Level >= this.TrainableSpells[spellIndex].MinLevel) {
				var trainingCost = (int)(this.TrainableSpells[spellIndex].MinLevel * this.BaseCost * 
				                      (1.0 - player.Intelligence / 100.0));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook.Add(this.TrainableSpells[spellIndex]);
					this.TrainableSpells.RemoveAt(spellIndex);
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					var spellName = textInfo.ToTitleCase(player.Spellbook[player.Spellbook.Count - 1].Name);
					var purchaseString = "You purchased " + spellName + " (Rank " + 
					                     player.Spellbook[player.Spellbook.Count - 1].Rank + ") for " + trainingCost + " gold.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
					return;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (spellIndex == -1) return;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You are not ready to train that spell. You need to level up first!");
		}
		public void UpgradeSpell(Player player, string inputName) {
			if (player.PlayerClass != Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade spells. You're not a mage!");
				return;
			}
			var spellIndex = player.Spellbook.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (spellIndex != -1 && player.Level >= player.Spellbook[spellIndex].Rank + 1) {
				var trainingCost = (int)((player.Spellbook[spellIndex].Rank + 1.0) * this.BaseCost * 
				                      (1.0 - player.Intelligence / 100.0));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook[spellIndex].Rank++;
					switch (player.Spellbook[spellIndex].SpellCategory) {
						case PlayerSpell.SpellType.Fireball:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							player.Spellbook[spellIndex].Offensive.AmountOverTime += 5;
							break;
						case PlayerSpell.SpellType.Frostbolt:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						case PlayerSpell.SpellType.Lightning:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						case PlayerSpell.SpellType.Heal:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case PlayerSpell.SpellType.Rejuvenate:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case PlayerSpell.SpellType.Diamondskin:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 10;
							break;
						case PlayerSpell.SpellType.TownPortal:
							player.Spellbook[spellIndex].ManaCost -= 5;
							break;
						case PlayerSpell.SpellType.Reflect:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 10;
							break;
						case PlayerSpell.SpellType.ArcaneIntellect:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 5;
							break;
						case PlayerSpell.SpellType.FrostNova:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					var spellName = textInfo.ToTitleCase(player.Spellbook[spellIndex].Name);
					var purchaseString = "You upgraded " + spellName + " to Rank " + 
					                     player.Spellbook[spellIndex].Rank + " for " + trainingCost + " gold.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
					return;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (spellIndex != -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You are not ready to upgrade that spell. You need to level up first!");
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that spell to train!");
		}
		public void UpgradeAbility(Player player, string inputName) {
			if (player.PlayerClass == Player.PlayerClassType.Mage) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade abilities. You're not a warrior or archer!");
				return;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (abilityIndex != -1 && player.Level >= player.Abilities[abilityIndex].Rank + 1) {
				var trainingCost = (int)((player.Abilities[abilityIndex].Rank + 1.0) * this.BaseCost * 
				                      (1.0 - player.Intelligence / 100.0));
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities[abilityIndex].Rank++;
					if (player.PlayerClass == Player.PlayerClassType.Archer) {
						switch (player.Abilities[abilityIndex].ArcAbilityCategory) {
							case PlayerAbility.ArcherAbility.Distance:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 5;
								break;
							case PlayerAbility.ArcherAbility.Gut:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Precise:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case PlayerAbility.ArcherAbility.Stun:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case PlayerAbility.ArcherAbility.Double:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case PlayerAbility.ArcherAbility.Wound:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Bandage:
								player.Abilities[abilityIndex].Healing.HealAmount += 10;
								player.Abilities[abilityIndex].Healing.HealOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.SwiftAura:
								player.Abilities[abilityIndex].ChangeAmount.Amount += 5;
								break;
							case PlayerAbility.ArcherAbility.ImmolatingArrow:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Ambush:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						var textInfo = new CultureInfo("en-US", false).TextInfo;
						var abilityName = textInfo.ToTitleCase(player.Abilities[abilityIndex].Name);
						var purchaseString = "You upgraded " + abilityName + " to Rank " + 
						                     player.Abilities[abilityIndex].Rank + " for " + trainingCost + " gold.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							purchaseString);
						return;
					}
					if (player.PlayerClass == Player.PlayerClassType.Warrior) {
						switch (player.Abilities[abilityIndex].WarAbilityCategory) {
							case PlayerAbility.WarriorAbility.Slash:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case PlayerAbility.WarriorAbility.Rend:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case PlayerAbility.WarriorAbility.Charge:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case PlayerAbility.WarriorAbility.Block:
								player.Abilities[abilityIndex].Defensive.BlockDamage += 10;
								break;
							case PlayerAbility.WarriorAbility.Berserk:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].ChangeAmount.Amount += 10;
								break;
							case PlayerAbility.WarriorAbility.Disarm:
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 10;
								break;
							case PlayerAbility.WarriorAbility.Bandage:
								player.Abilities[abilityIndex].Healing.HealAmount += 10;
								player.Abilities[abilityIndex].Healing.HealOverTime += 5;
								break;
							case PlayerAbility.WarriorAbility.PowerAura:
								player.Abilities[abilityIndex].ChangeAmount.Amount += 5;
								break;
							case PlayerAbility.WarriorAbility.WarCry:
								player.Abilities[abilityIndex].ChangeAmount.Amount += 10;
								break;
							case PlayerAbility.WarriorAbility.Onslaught:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't afford that!");
				return;
			}
			if (abilityIndex != -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You are not ready to upgrade that ability. You need to level up first!");
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that ability to train!");
		}
	}
}