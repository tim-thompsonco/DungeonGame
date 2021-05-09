using DungeonGame.Helpers;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Players;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame.Trainers {
	public class Trainer : IQuestGiver {
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
			Name = name;
			Desc = desc;
			BaseCost = 25;
			TrainerGroup = trainerCategory;
			switch (TrainerGroup) {
				case TrainerCategory.Archer:
					TrainableAbilities = new List<PlayerAbility>
					{
						new PlayerAbility(
						"bandage", 25, 1, ArcherAbility.Bandage, 2),
						new PlayerAbility(
						"ambush", 75, 1, ArcherAbility.Ambush, 4),
						new PlayerAbility(
						"swift aura", 150, 1, ArcherAbility.SwiftAura, 6),
						new PlayerAbility(
						"immolating arrow", 35, 1, ArcherAbility.ImmolatingArrow, 8)
					};
					break;
				case TrainerCategory.Warrior:
					TrainableAbilities = new List<PlayerAbility>
					{
						new PlayerAbility(
						"bandage", 25, 1, WarriorAbility.Bandage, 2),
						new PlayerAbility(
						"war cry", 50, 1, WarriorAbility.WarCry, 4),
						new PlayerAbility(
						"power aura", 150, 1, WarriorAbility.PowerAura, 6),
						new PlayerAbility(
						"onslaught", 25, 1, WarriorAbility.Onslaught, 8)
					};
					break;
				case TrainerCategory.Mage:
					TrainableSpells = new List<PlayerSpell>
					{
						new PlayerSpell(
						"town portal", 100, 1, SpellType.TownPortal, 2),
						new PlayerSpell(
						"reflect", 100, 1, SpellType.Reflect, 4),
						new PlayerSpell(
						"arcane intellect", 150, 1, SpellType.ArcaneIntellect, 6),
						new PlayerSpell(
						"frost nova", 50, 1, SpellType.FrostNova, 8)
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayAvailableUpgrades(Player player) {
			switch (player.PlayerClass) {
				case PlayerClassType.Mage:
					if (TrainerGroup != TrainerCategory.Mage) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a mage grandmaster!");
						return;
					}
					break;
				case PlayerClassType.Warrior:
					if (TrainerGroup != TrainerCategory.Warrior) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a warrior grandmaster!");
						return;
					}
					break;
				case PlayerClassType.Archer:
					if (TrainerGroup != TrainerCategory.Archer) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a archer grandmaster!");
						return;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			string forSaleString = "The " + Name + " has the following upgrades available:";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (TrainerGroup == TrainerCategory.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New Spells: ");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (TrainableSpells?.Count == 0) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				} else {
					try {
						int newSpellsToTrain = 0;
						foreach (PlayerSpell spell in TrainableSpells) {
							if (player.Level < spell.MinLevel) {
								continue;
							}

							double trainingReduction = 1.0 - player.Intelligence / 100.0;
							if (trainingReduction < 0.5) {
								trainingReduction = 0.5;
							}

							int trainingCost = (int)(spell.MinLevel * BaseCost * trainingReduction);
							string spellName = textInfo.ToTitleCase(spell.Name +
																 " (Rank: " + spell.Rank + ") (Cost: " + trainingCost + ")");
							newSpellsToTrain++;
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								spellName);
						}
						if (newSpellsToTrain == 0) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					} catch (ArgumentNullException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing Spells: ");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				int spellsToTrain = 0;
				foreach (PlayerSpell spell in player.Spellbook) {
					if (player.Level == spell.Rank) {
						continue;
					}

					double trainingReduction = 1.0 - player.Intelligence / 100.0;
					if (trainingReduction < 0.5) {
						trainingReduction = 0.5;
					}

					int trainingCost = (int)((spell.Rank + 1.0) * BaseCost * trainingReduction);
					string spellName = textInfo.ToTitleCase(spell.Name +
														 " (Rank: " + spell.Rank + ") (Cost: " + trainingCost + ")");
					spellsToTrain++;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellName);
				}
				if (spellsToTrain == 0) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New _Abilities: ");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (TrainableAbilities?.Count == 0) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				} else {
					try {
						int newAbilitiesToTrain = 0;
						foreach (PlayerAbility ability in TrainableAbilities) {
							if (player.Level < ability.MinLevel) {
								continue;
							}

							double trainingReduction = 1.0 - player.Intelligence / 100.0;
							if (trainingReduction < 0.5) {
								trainingReduction = 0.5;
							}

							int trainingCost = (int)(ability.MinLevel * BaseCost * trainingReduction);
							string abilityName = textInfo.ToTitleCase(ability.Name +
																 " (Rank: " + ability.Rank + ") (Cost: " + trainingCost + ")");
							newAbilitiesToTrain++;
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								abilityName);
						}
						if (newAbilitiesToTrain == 0) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					} catch (ArgumentNullException) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing _Abilities: ");
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				int abilitiesToTrain = 0;
				foreach (PlayerAbility ability in player.Abilities) {
					if (player.Level == ability.Rank) {
						continue;
					}

					double trainingReduction = 1.0 - player.Intelligence / 100.0;
					if (trainingReduction < 0.5) {
						trainingReduction = 0.5;
					}

					int trainingCost = (int)((ability.Rank + 1.0) * BaseCost * trainingReduction);
					string abilityName = textInfo.ToTitleCase(ability.Name +
														   " (Rank: " + ability.Rank + ") (Cost: " + trainingCost + ")");
					abilitiesToTrain++;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityName);
				}
				if (abilitiesToTrain == 0) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				string.Empty);
			string playerGold = "Player _Gold: " + player.Gold;
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				playerGold);
		}
		public void TrainAbility(Player player, string inputName) {
			if (player.PlayerClass == PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train abilities. You're not a warrior or archer!");
				return;
			}
			int abilityIndex = TrainableAbilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (abilityIndex != -1 && player.Level >= TrainableAbilities[abilityIndex].MinLevel) {
				double trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)(TrainableAbilities[abilityIndex].MinLevel * BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities.Add(TrainableAbilities[abilityIndex]);
					TrainableAbilities.RemoveAt(abilityIndex);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string abilityName = textInfo.ToTitleCase(player.Abilities[player.Abilities.Count - 1].Name);
					string purchaseString = "You purchased " + abilityName + " (Rank " +
										 player.Abilities[player.Abilities.Count - 1].Rank + ") for " + trainingCost + " gold.";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1 ?
						"You are not ready to train that ability. You need to level up first!" : "Train what?");
			}
		}
		public void TrainSpell(Player player, string inputName) {
			if (player.PlayerClass != PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train spells. You're not a mage!");
				return;
			}
			int spellIndex = TrainableSpells.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (spellIndex != -1 && player.Level >= TrainableSpells[spellIndex].MinLevel) {
				double trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)(TrainableSpells[spellIndex].MinLevel * BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook.Add(TrainableSpells[spellIndex]);
					TrainableSpells.RemoveAt(spellIndex);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string spellName = textInfo.ToTitleCase(player.Spellbook[player.Spellbook.Count - 1].Name);
					string purchaseString = "You purchased " + spellName + " (Rank " +
										 player.Spellbook[player.Spellbook.Count - 1].Rank + ") for " + trainingCost + " gold.";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1 ?
						"You are not ready to train that spell. You need to level up first!" : "Train what?");
			}
		}
		public void UpgradeSpell(Player player, string inputName) {
			if (player.PlayerClass != PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade spells. You're not a mage!");
				return;
			}
			int spellIndex = player.Spellbook.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (spellIndex != -1 && player.Level >= player.Spellbook[spellIndex].Rank + 1) {
				double trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)((player.Spellbook[spellIndex].Rank + 1.0) * BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Spellbook[spellIndex].Rank++;
					player.Spellbook[spellIndex].ManaCost += 10;
					switch (player.Spellbook[spellIndex].SpellCategory) {
						case SpellType.Fireball:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							player.Spellbook[spellIndex].Offensive.AmountOverTime += 5;
							break;
						case SpellType.Frostbolt:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						case SpellType.Lightning:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						case SpellType.Heal:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case SpellType.Rejuvenate:
							player.Spellbook[spellIndex].Healing.HealAmount += 10;
							player.Spellbook[spellIndex].Healing.HealOverTime += 5;
							break;
						case SpellType.Diamondskin:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 10;
							break;
						case SpellType.TownPortal:
							player.Spellbook[spellIndex].ManaCost -= 15;
							break;
						case SpellType.Reflect:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 10;
							break;
						case SpellType.ArcaneIntellect:
							player.Spellbook[spellIndex].ChangeAmount.Amount += 5;
							break;
						case SpellType.FrostNova:
							player.Spellbook[spellIndex].Offensive.Amount += 10;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string spellName = textInfo.ToTitleCase(player.Spellbook[spellIndex].Name);
					string purchaseString = "You upgraded " + spellName + " to Rank " +
										 player.Spellbook[spellIndex].Rank + " for " + trainingCost + " gold.";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1
						? "You are not ready to upgrade that spell. You need to level up first!"
						: "You don't have that spell to train!");
			}
		}
		public void UpgradeAbility(Player player, string inputName) {
			if (player.PlayerClass == PlayerClassType.Mage) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade abilities. You're not a warrior or archer!");
				return;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (abilityIndex != -1 && player.Level >= player.Abilities[abilityIndex].Rank + 1) {
				double trainingReduction = 1.0 - player.Intelligence / 100.0;
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)((player.Abilities[abilityIndex].Rank + 1.0) * BaseCost * trainingReduction);
				if (player.Gold >= trainingCost) {
					player.Gold -= trainingCost;
					player.Abilities[abilityIndex].Rank++;
					if (player.PlayerClass == PlayerClassType.Archer) {
						player.Abilities[abilityIndex].ComboCost += 10;
						switch (player.Abilities[abilityIndex].ArcAbilityCategory) {
							case ArcherAbility.Distance:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 5;
								break;
							case ArcherAbility.Gut:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case ArcherAbility.Precise:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case ArcherAbility.Stun:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case ArcherAbility.Double:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case ArcherAbility.Wound:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case ArcherAbility.Bandage:
								player.Abilities[abilityIndex].Healing.HealAmount += 10;
								player.Abilities[abilityIndex].Healing.HealOverTime += 5;
								break;
							case ArcherAbility.SwiftAura:
								player.Abilities[abilityIndex].ChangeAmount.Amount += 5;
								break;
							case ArcherAbility.ImmolatingArrow:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case ArcherAbility.Ambush:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					} else {
						player.Abilities[abilityIndex].RageCost += 10;
						switch (player.Abilities[abilityIndex].WarAbilityCategory) {
							case WarriorAbility.Slash:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							case WarriorAbility.Rend:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].Offensive.AmountOverTime += 5;
								break;
							case WarriorAbility.Charge:
								player.Abilities[abilityIndex].Stun.DamageAmount += 10;
								break;
							case WarriorAbility.Block:
								player.Abilities[abilityIndex].Defensive.BlockDamage += 10;
								break;
							case WarriorAbility.Berserk:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								player.Abilities[abilityIndex].ChangeAmount.Amount -= 10;
								break;
							case WarriorAbility.Disarm:
								player.Abilities[abilityIndex].Offensive.ChanceToSucceed += 10;
								break;
							case WarriorAbility.Bandage:
								player.Abilities[abilityIndex].Healing.HealAmount += 10;
								player.Abilities[abilityIndex].Healing.HealOverTime += 5;
								break;
							case WarriorAbility.PowerAura:
								player.Abilities[abilityIndex].ChangeAmount.Amount += 5;
								break;
							case WarriorAbility.WarCry:
								player.Abilities[abilityIndex].ChangeAmount.Amount -= 10;
								break;
							case WarriorAbility.Onslaught:
								player.Abilities[abilityIndex].Offensive.Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string abilityName = textInfo.ToTitleCase(player.Abilities[abilityIndex].Name);
					string purchaseString = "You upgraded " + abilityName + " to Rank " +
										 player.Abilities[abilityIndex].Rank + " for " + trainingCost + " gold.";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1
						? "You are not ready to upgrade that ability. You need to level up first!"
						: "You don't have that ability to train!");
			}
		}
		public void PopulateQuests(Player player) {
			AvailableQuests = new List<Quest>();
			ArmorType questArmorGroup = player.PlayerClass switch {
				PlayerClassType.Mage => ArmorType.Cloth,
				PlayerClassType.Warrior => ArmorType.Plate,
				PlayerClassType.Archer => ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (TrainerGroup) {
				case TrainerCategory.Archer:
					AvailableQuests.Add(new Quest(
						"Slaughterhouse",
						"Look. I'm busy here teaching these kids how to defend themselves. I need you to get some " +
						"practice with that yourself. Why don't you go down into that dungeon and clear a level of it? If it " +
						"moves, kill it. Be like a house cat. Don't ask questions, just kill it, and keep doing that until " +
						"there's nothing left. Do that, come back here, and you'll get a reward. ",
						QuestType.ClearLevel,
						new Armor(questArmorGroup, ArmorSlot.Hands, true, player),
						Name));
					break;
				case TrainerCategory.Warrior:
					AvailableQuests.Add(new Quest(
						"Hunter Killer",
						"We need some hard souls to do some house cleaning in the dungeon. There's a lot of monsters " +
						"down there and some of them have been wandering out of the dungeon at night to terrorize this town. " +
						"I want you to return the favor. Hunt down and kill a bunch of them. Surely if we thin the ranks down " +
						"then they won't be so likely to stray from the dungeon. Go take care of this for me will you? ",
						QuestType.KillMonster,
						new Armor(questArmorGroup, ArmorSlot.Back, true, player),
						Name));
					break;
				case TrainerCategory.Mage:
					AvailableQuests.Add(new Quest(
						"Kill Them All",
						"I need you to do something for me. I'm busy training mages here all day but it isn't lost " +
						"on me how many evil creatures are down in that dungeon killing unsuspecting travelers. I want you " +
						"to go in there and kill as many of them as you can. You look like you might have a slightly higher " +
						"chance of surviving, but if you don't, I promise I'll find and bury you someday ok? That seems like " +
						"a reasonable offer to me.",
						QuestType.KillCount,
						new Armor(questArmorGroup, ArmorSlot.Chest, true, player),
						Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (AvailableQuests == null) {
				PopulateQuests(player);
			}

			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Available Quests:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (Quest quest in AvailableQuests) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(quest.Name));
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			string userInput = InputHelper.ParseInput(input);
			int questIndex = AvailableQuests.FindIndex(
				f => f.Name.ToLower().Contains(userInput));
			if (questIndex != -1) {
				AvailableQuests[questIndex].ShowQuest();
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputHelper.ShowUserOutput(player);
				OutputHelper.Display.ClearUserOutput();
				string[] questInput = InputHelper.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLower() != "y" && questInput[0].ToLower() != "yes" &&
					   questInput[0].ToLower() != "n" && questInput[0].ToLower() != "no") {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(AvailableQuests[questIndex].Name) + " Consideration:");
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputHelper.ShowUserOutput(player);
					OutputHelper.Display.ClearUserOutput();
					questInput = InputHelper.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player.QuestLog.Add(AvailableQuests[questIndex]);
					AvailableQuests.RemoveAt(questIndex);
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			string userInput = InputHelper.ParseInput(input);
			int questIndex = player.QuestLog.FindIndex(
				f => f.Name.ToLower().Contains(userInput));
			Quest quest = player.QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest.QuestGiver == Name) {
					if (quest.QuestCompleted) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"Congratulations on finishing " + quest.Name + "! Here's your reward.");
						player.Inventory.Add(quest.QuestRewardItem);
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearHelper.StoreRainbowGearOutput(GearHelper.GetItemDetails(quest.QuestRewardItem));
						player.Gold += quest.QuestRewardGold;
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							quest.QuestRewardGold + " gold coins.");
						player.QuestLog.RemoveAt(questIndex);
					} else {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				} else {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I didn't give you that quest. " + textInfo.ToTitleCase(quest.QuestGiver) + " did.");
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}