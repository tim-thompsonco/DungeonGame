using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DungeonGame {
	public class Trainer : IRoomInteraction, IQuestGiver {
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
		public List<Quest> AvailableQuests { get; set; }

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
							var trainingReduction = 1.0 - player.Intelligence / 100.0;
							if (trainingReduction < 0.5) trainingReduction = 0.5;
							var trainingCost = (int)(spell.MinLevel * this.BaseCost * trainingReduction);
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
					if (player.Level == spell.Rank) continue;
					var trainingReduction = 1.0 - player.Intelligence / 100.0;
					if (trainingReduction < 0.5) trainingReduction = 0.5;
					var trainingCost = (int)((spell.Rank + 1.0) * this.BaseCost * trainingReduction);
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
							var trainingReduction = 1.0 - player.Intelligence / 100.0;
							if (trainingReduction < 0.5) trainingReduction = 0.5;
							var trainingCost = (int)(ability.MinLevel * this.BaseCost * trainingReduction);
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
					if (player.Level == ability.Rank) continue;
					var trainingReduction = 1.0 - player.Intelligence / 100.0;
					if (trainingReduction < 0.5) trainingReduction = 0.5;
					var trainingCost = (int)((ability.Rank + 1.0) * this.BaseCost * trainingReduction);
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
				var trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) trainingReduction = 0.5;
				var trainingCost = (int)(this.TrainableAbilities[abilityIndex].MinLevel * this.BaseCost * trainingReduction);
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
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1 ? 
						"You are not ready to train that ability. You need to level up first!" : "Train what?");
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
				var trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) trainingReduction = 0.5;
				var trainingCost = (int)(this.TrainableSpells[spellIndex].MinLevel * this.BaseCost * trainingReduction);
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
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1 ? 
						"You are not ready to train that spell. You need to level up first!" : "Train what?");
			}
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
				var trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) trainingReduction = 0.5;
				var trainingCost = (int)((player.Spellbook[spellIndex].Rank + 1.0) * this.BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook[spellIndex].Rank++;
					player.Spellbook[spellIndex].ManaCost += 10;
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
							player.Spellbook[spellIndex].ManaCost -= 15;
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
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1
						? "You are not ready to upgrade that spell. You need to level up first!"
						: "You don't have that spell to train!");
			}
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
				var trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) trainingReduction = 0.5;
				var trainingCost = (int)((player.Abilities[abilityIndex].Rank + 1.0) * this.BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities[abilityIndex].Rank++;
					if (player.PlayerClass == Player.PlayerClassType.Archer) {
						player.Abilities[abilityIndex].ComboCost += 10;
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
					}
					else {
						player.Abilities[abilityIndex].RageCost += 10;
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
								player.Abilities[abilityIndex].ChangeAmount.Amount -= 10;
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
								player.Abilities[abilityIndex].ChangeAmount.Amount -= 10;
								break;
							case PlayerAbility.WarriorAbility.Onslaught:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					var abilityName = textInfo.ToTitleCase(player.Abilities[abilityIndex].Name);
					var purchaseString = "You upgraded " + abilityName + " to Rank " + 
					                     player.Abilities[abilityIndex].Rank + " for " + trainingCost + " gold.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1
						? "You are not ready to upgrade that ability. You need to level up first!"
						: "You don't have that ability to train!");
			}
		}
		public void PopulateQuests(Player player) {
			this.AvailableQuests = new List<Quest>();
			var questArmorGroup = player.PlayerClass switch {
				Player.PlayerClassType.Mage => Armor.ArmorType.Cloth,
				Player.PlayerClassType.Warrior => Armor.ArmorType.Plate,
				Player.PlayerClassType.Archer => Armor.ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (this.TrainerGroup) {
				case TrainerCategory.Archer:
					this.AvailableQuests.Add(new Quest(
						"Slaughterhouse",
						"Look. I'm busy here teaching these kids how to defend themselves. I need you to get some " + 
						"practice with that yourself. Why don't you go down into that dungeon and clear a level of it? If it " +
						"moves, kill it. Be like a house cat. Don't ask questions, just kill it, and keep doing that until " +
						"there's nothing left. Do that, come back here, and you'll get a reward. ",
						Quest.QuestType.ClearLevel, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Hands, true, player), 
						this.Name));
					break;
				case TrainerCategory.Warrior:
					this.AvailableQuests.Add(new Quest(
						"Hunter Killer",
						"We need some hard souls to do some house cleaning in the dungeon. There's a lot of monsters " + 
						"down there and some of them have been wandering out of the dungeon at night to terrorize this town. " +
						"I want you to return the favor. Hunt down and kill a bunch of them. Surely if we thin the ranks down " +
						"then they won't be so likely to stray from the dungeon. Go take care of this for me will you? ",
						Quest.QuestType.KillMonster, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Back, true, player), 
						this.Name));
					break;
				case TrainerCategory.Mage:
					this.AvailableQuests.Add(new Quest(
						"Kill Them All",
						"I need you to do something for me. I'm busy training mages here all day but it isn't lost " + 
						"on me how many evil creatures are down in that dungeon killing unsuspecting travelers. I want you " +
						"to go in there and kill as many of them as you can. You look like you might have a slightly higher " +
						"chance of surviving, but if you don't, I promise I'll find and bury you someday ok? That seems like " +
						"a reasonable offer to me.", 
						Quest.QuestType.KillCount, 
						new Armor(questArmorGroup, Armor.ArmorSlot.Chest, true, player), 
						this.Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (this.AvailableQuests == null) this.PopulateQuests(player);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				"Available Quests:");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var quest in this.AvailableQuests) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(), 
					Settings.FormatDefaultBackground(), 
					textInfo.ToTitleCase(quest.Name));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			var userInput = InputHandler.ParseInput(input);
			var questIndex = this.AvailableQuests.FindIndex(
				f => f.Name.ToLowerInvariant().Contains(userInput));
			if (questIndex != -1) {
				this.AvailableQuests[questIndex].ShowQuest();
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputHandler.ShowUserOutput(player);
				OutputHandler.Display.ClearUserOutput();
				var questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLowerInvariant() != "y" && questInput[0].ToLowerInvariant() != "yes" &&
				       questInput[0].ToLowerInvariant() != "n" && questInput[0].ToLowerInvariant() != "no") {
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(this.AvailableQuests[questIndex].Name) + " Consideration:");
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputHandler.ShowUserOutput(player);
					OutputHandler.Display.ClearUserOutput();
					questInput = InputHandler.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player.QuestLog.Add(this.AvailableQuests[questIndex]);
					this.AvailableQuests.RemoveAt(questIndex);
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				}
				else {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			var userInput = InputHandler.ParseInput(input);
			var questIndex = player.QuestLog.FindIndex(
				f => f.Name.ToLowerInvariant().Contains(userInput));
			var quest = player.QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest.QuestGiver == this.Name) {
					if (quest.QuestCompleted) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"Congratulations on finishing " + quest.Name + "! Here's your reward.");
						player.Inventory.Add(quest.QuestRewardItem);
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearHandler.StoreRainbowGearOutput(GearHandler.GetItemDetails(quest.QuestRewardItem));
						player.Gold += quest.QuestRewardGold;
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							quest.QuestRewardGold + " gold coins.");
						player.QuestLog.RemoveAt(questIndex);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				}
				else {
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I didn't give you that quest. " + textInfo.ToTitleCase(quest.QuestGiver) + " did.");
				}
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}