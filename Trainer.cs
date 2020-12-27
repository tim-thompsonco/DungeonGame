using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Players;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame {
	public class Trainer : IQuestGiver {
		public enum TrainerCategory {
			Archer,
			Warrior,
			Mage
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public TrainerCategory _TrainerGroup { get; set; }
		public int _BaseCost { get; set; }
		public List<PlayerAbility> _TrainableAbilities { get; set; }
		public List<PlayerSpell> _TrainableSpells { get; set; }
		public List<Quest> _AvailableQuests { get; set; }

		// Default constructor for JSON serialization
		public Trainer() { }
		public Trainer(string name, string desc, TrainerCategory trainerCategory) {
			_Name = name;
			_Desc = desc;
			_BaseCost = 25;
			_TrainerGroup = trainerCategory;
			switch (_TrainerGroup) {
				case TrainerCategory.Archer:
					_TrainableAbilities = new List<PlayerAbility>
					{
						new PlayerAbility(
						"bandage", 25, 1, PlayerAbility.ArcherAbility.Bandage, 2),
						new PlayerAbility(
						"ambush", 75, 1, PlayerAbility.ArcherAbility.Ambush, 4),
						new PlayerAbility(
						"swift aura", 150, 1, PlayerAbility.ArcherAbility.SwiftAura, 6),
						new PlayerAbility(
						"immolating arrow", 35, 1, PlayerAbility.ArcherAbility.ImmolatingArrow, 8)
					};
					break;
				case TrainerCategory.Warrior:
					_TrainableAbilities = new List<PlayerAbility>
					{
						new PlayerAbility(
						"bandage", 25, 1, PlayerAbility.WarriorAbility.Bandage, 2),
						new PlayerAbility(
						"war cry", 50, 1, PlayerAbility.WarriorAbility.WarCry, 4),
						new PlayerAbility(
						"power aura", 150, 1, PlayerAbility.WarriorAbility.PowerAura, 6),
						new PlayerAbility(
						"onslaught", 25, 1, PlayerAbility.WarriorAbility.Onslaught, 8)
					};
					break;
				case TrainerCategory.Mage:
					_TrainableSpells = new List<PlayerSpell>
					{
						new PlayerSpell(
						"town portal", 100, 1, PlayerSpell.SpellType.TownPortal, 2),
						new PlayerSpell(
						"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 4),
						new PlayerSpell(
						"arcane intellect", 150, 1, PlayerSpell.SpellType.ArcaneIntellect, 6),
						new PlayerSpell(
						"frost nova", 50, 1, PlayerSpell.SpellType.FrostNova, 8)
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DisplayAvailableUpgrades(Player player) {
			switch (player._PlayerClass) {
				case Player.PlayerClassType.Mage:
					if (_TrainerGroup != TrainerCategory.Mage) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a mage grandmaster!");
						return;
					}
					break;
				case Player.PlayerClassType.Warrior:
					if (_TrainerGroup != TrainerCategory.Warrior) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a warrior grandmaster!");
						return;
					}
					break;
				case Player.PlayerClassType.Archer:
					if (_TrainerGroup != TrainerCategory.Archer) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"They are not a trainer for your class. Go find a archer grandmaster!");
						return;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			string forSaleString = "The " + _Name + " has the following upgrades available:";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				forSaleString);
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			if (_TrainerGroup == TrainerCategory.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New Spells: ");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (_TrainableSpells?.Count == 0) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				} else {
					try {
						int newSpellsToTrain = 0;
						foreach (PlayerSpell spell in _TrainableSpells) {
							if (player._Level < spell._MinLevel) {
								continue;
							}

							double trainingReduction = 1.0 - (player._Intelligence / 100.0);
							if (trainingReduction < 0.5) {
								trainingReduction = 0.5;
							}

							int trainingCost = (int)(spell._MinLevel * _BaseCost * trainingReduction);
							string spellName = textInfo.ToTitleCase(spell._Name +
																 " (Rank: " + spell._Rank + ") (Cost: " + trainingCost + ")");
							newSpellsToTrain++;
							OutputController.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								spellName);
						}
						if (newSpellsToTrain == 0) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					} catch (ArgumentNullException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing Spells: ");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				int spellsToTrain = 0;
				foreach (PlayerSpell spell in player._Spellbook) {
					if (player._Level == spell._Rank) {
						continue;
					}

					double trainingReduction = 1.0 - (player._Intelligence / 100.0);
					if (trainingReduction < 0.5) {
						trainingReduction = 0.5;
					}

					int trainingCost = (int)((spell._Rank + 1.0) * _BaseCost * trainingReduction);
					string spellName = textInfo.ToTitleCase(spell._Name +
														 " (Rank: " + spell._Rank + ") (Cost: " + trainingCost + ")");
					spellsToTrain++;
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellName);
				}
				if (spellsToTrain == 0) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"New _Abilities: ");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				if (_TrainableAbilities?.Count == 0) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				} else {
					try {
						int newAbilitiesToTrain = 0;
						foreach (PlayerAbility ability in _TrainableAbilities) {
							if (player._Level < ability._MinLevel) {
								continue;
							}

							double trainingReduction = 1.0 - (player._Intelligence / 100.0);
							if (trainingReduction < 0.5) {
								trainingReduction = 0.5;
							}

							int trainingCost = (int)(ability._MinLevel * _BaseCost * trainingReduction);
							string abilityName = textInfo.ToTitleCase(ability._Name +
																 " (Rank: " + ability._Rank + ") (Cost: " + trainingCost + ")");
							newAbilitiesToTrain++;
							OutputController.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								abilityName);
						}
						if (newAbilitiesToTrain == 0) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatInfoText(),
								Settings.FormatDefaultBackground(),
								"None.");
						}
					} catch (ArgumentNullException) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatInfoText(),
							Settings.FormatDefaultBackground(),
							"None.");
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Existing _Abilities: ");
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"");
				int abilitiesToTrain = 0;
				foreach (PlayerAbility ability in player._Abilities) {
					if (player._Level == ability._Rank) {
						continue;
					}

					double trainingReduction = 1.0 - (player._Intelligence / 100.0);
					if (trainingReduction < 0.5) {
						trainingReduction = 0.5;
					}

					int trainingCost = (int)((ability._Rank + 1.0) * _BaseCost * trainingReduction);
					string abilityName = textInfo.ToTitleCase(ability._Name +
														   " (Rank: " + ability._Rank + ") (Cost: " + trainingCost + ")");
					abilitiesToTrain++;
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityName);
				}
				if (abilitiesToTrain == 0) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						"None.");
				}
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				string.Empty);
			string playerGold = "Player _Gold: " + player._Gold;
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				playerGold);
		}
		public void TrainAbility(Player player, string inputName) {
			if (player._PlayerClass == Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train abilities. You're not a warrior or archer!");
				return;
			}
			int abilityIndex = _TrainableAbilities.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (abilityIndex != -1 && player._Level >= _TrainableAbilities[abilityIndex]._MinLevel) {
				double trainingReduction = 1.0 - (player._Intelligence / 100.0);
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)(_TrainableAbilities[abilityIndex]._MinLevel * _BaseCost * trainingReduction);
				if (player._Gold >= trainingCost) {
					player._Gold -= trainingCost;
					player._Abilities.Add(_TrainableAbilities[abilityIndex]);
					_TrainableAbilities.RemoveAt(abilityIndex);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string abilityName = textInfo.ToTitleCase(player._Abilities[player._Abilities.Count - 1]._Name);
					string purchaseString = "You purchased " + abilityName + " (Rank " +
										 player._Abilities[player._Abilities.Count - 1]._Rank + ") for " + trainingCost + " gold.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1 ?
						"You are not ready to train that ability. You need to level up first!" : "Train what?");
			}
		}
		public void TrainSpell(Player player, string inputName) {
			if (player._PlayerClass != Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't train spells. You're not a mage!");
				return;
			}
			int spellIndex = _TrainableSpells.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (spellIndex != -1 && player._Level >= _TrainableSpells[spellIndex]._MinLevel) {
				double trainingReduction = 1.0 - (player._Intelligence / 100.0);
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)(_TrainableSpells[spellIndex]._MinLevel * _BaseCost * trainingReduction);
				if (player._Gold >= trainingCost) {
					player._Gold -= trainingCost;
					player._Spellbook.Add(_TrainableSpells[spellIndex]);
					_TrainableSpells.RemoveAt(spellIndex);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string spellName = textInfo.ToTitleCase(player._Spellbook[player._Spellbook.Count - 1]._Name);
					string purchaseString = "You purchased " + spellName + " (Rank " +
										 player._Spellbook[player._Spellbook.Count - 1]._Rank + ") for " + trainingCost + " gold.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1 ?
						"You are not ready to train that spell. You need to level up first!" : "Train what?");
			}
		}
		public void UpgradeSpell(Player player, string inputName) {
			if (player._PlayerClass != Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade spells. You're not a mage!");
				return;
			}
			int spellIndex = player._Spellbook.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (spellIndex != -1 && player._Level >= player._Spellbook[spellIndex]._Rank + 1) {
				double trainingReduction = 1.0 - (player._Intelligence / 100.0);
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)((player._Spellbook[spellIndex]._Rank + 1.0) * _BaseCost * trainingReduction);
				if (player._Gold >= trainingCost) {
					player._Gold -= trainingCost;
					player._Spellbook[spellIndex]._Rank++;
					player._Spellbook[spellIndex]._ManaCost += 10;
					switch (player._Spellbook[spellIndex]._SpellCategory) {
						case PlayerSpell.SpellType.Fireball:
							player._Spellbook[spellIndex]._Offensive._Amount += 10;
							player._Spellbook[spellIndex]._Offensive._AmountOverTime += 5;
							break;
						case PlayerSpell.SpellType.Frostbolt:
							player._Spellbook[spellIndex]._Offensive._Amount += 10;
							break;
						case PlayerSpell.SpellType.Lightning:
							player._Spellbook[spellIndex]._Offensive._Amount += 10;
							break;
						case PlayerSpell.SpellType.Heal:
							player._Spellbook[spellIndex]._Healing._HealAmount += 10;
							player._Spellbook[spellIndex]._Healing._HealOverTime += 5;
							break;
						case PlayerSpell.SpellType.Rejuvenate:
							player._Spellbook[spellIndex]._Healing._HealAmount += 10;
							player._Spellbook[spellIndex]._Healing._HealOverTime += 5;
							break;
						case PlayerSpell.SpellType.Diamondskin:
							player._Spellbook[spellIndex]._ChangeAmount._Amount += 10;
							break;
						case PlayerSpell.SpellType.TownPortal:
							player._Spellbook[spellIndex]._ManaCost -= 15;
							break;
						case PlayerSpell.SpellType.Reflect:
							player._Spellbook[spellIndex]._ChangeAmount._Amount += 10;
							break;
						case PlayerSpell.SpellType.ArcaneIntellect:
							player._Spellbook[spellIndex]._ChangeAmount._Amount += 5;
							break;
						case PlayerSpell.SpellType.FrostNova:
							player._Spellbook[spellIndex]._Offensive._Amount += 10;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string spellName = textInfo.ToTitleCase(player._Spellbook[spellIndex]._Name);
					string purchaseString = "You upgraded " + spellName + " to Rank " +
										 player._Spellbook[spellIndex]._Rank + " for " + trainingCost + " gold.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					spellIndex != -1
						? "You are not ready to upgrade that spell. You need to level up first!"
						: "You don't have that spell to train!");
			}
		}
		public void UpgradeAbility(Player player, string inputName) {
			if (player._PlayerClass == Player.PlayerClassType.Mage) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade abilities. You're not a warrior or archer!");
				return;
			}
			int abilityIndex = player._Abilities.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (abilityIndex != -1 && player._Level >= player._Abilities[abilityIndex]._Rank + 1) {
				double trainingReduction = 1.0 - (player._Intelligence / 100.0);
				if (trainingReduction < 0.5) {
					trainingReduction = 0.5;
				}

				int trainingCost = (int)((player._Abilities[abilityIndex]._Rank + 1.0) * _BaseCost * trainingReduction);
				if (player._Gold >= trainingCost) {
					player._Gold -= trainingCost;
					player._Abilities[abilityIndex]._Rank++;
					if (player._PlayerClass == Player.PlayerClassType.Archer) {
						player._Abilities[abilityIndex]._ComboCost += 10;
						switch (player._Abilities[abilityIndex]._ArcAbilityCategory) {
							case PlayerAbility.ArcherAbility.Distance:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._Offensive._ChanceToSucceed += 5;
								break;
							case PlayerAbility.ArcherAbility.Gut:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._Offensive._AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Precise:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								break;
							case PlayerAbility.ArcherAbility.Stun:
								player._Abilities[abilityIndex]._Stun._DamageAmount += 10;
								break;
							case PlayerAbility.ArcherAbility.Double:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								break;
							case PlayerAbility.ArcherAbility.Wound:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._Offensive._AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Bandage:
								player._Abilities[abilityIndex]._Healing._HealAmount += 10;
								player._Abilities[abilityIndex]._Healing._HealOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.SwiftAura:
								player._Abilities[abilityIndex]._ChangeAmount._Amount += 5;
								break;
							case PlayerAbility.ArcherAbility.ImmolatingArrow:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._Offensive._AmountOverTime += 5;
								break;
							case PlayerAbility.ArcherAbility.Ambush:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					} else {
						player._Abilities[abilityIndex]._RageCost += 10;
						switch (player._Abilities[abilityIndex]._WarAbilityCategory) {
							case PlayerAbility.WarriorAbility.Slash:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								break;
							case PlayerAbility.WarriorAbility.Rend:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._Offensive._AmountOverTime += 5;
								break;
							case PlayerAbility.WarriorAbility.Charge:
								player._Abilities[abilityIndex]._Stun._DamageAmount += 10;
								break;
							case PlayerAbility.WarriorAbility.Block:
								player._Abilities[abilityIndex]._Defensive._BlockDamage += 10;
								break;
							case PlayerAbility.WarriorAbility.Berserk:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								player._Abilities[abilityIndex]._ChangeAmount._Amount -= 10;
								break;
							case PlayerAbility.WarriorAbility.Disarm:
								player._Abilities[abilityIndex]._Offensive._ChanceToSucceed += 10;
								break;
							case PlayerAbility.WarriorAbility.Bandage:
								player._Abilities[abilityIndex]._Healing._HealAmount += 10;
								player._Abilities[abilityIndex]._Healing._HealOverTime += 5;
								break;
							case PlayerAbility.WarriorAbility.PowerAura:
								player._Abilities[abilityIndex]._ChangeAmount._Amount += 5;
								break;
							case PlayerAbility.WarriorAbility.WarCry:
								player._Abilities[abilityIndex]._ChangeAmount._Amount -= 10;
								break;
							case PlayerAbility.WarriorAbility.Onslaught:
								player._Abilities[abilityIndex]._Offensive._Amount += 10;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					string abilityName = textInfo.ToTitleCase(player._Abilities[abilityIndex]._Name);
					string purchaseString = "You upgraded " + abilityName + " to Rank " +
										 player._Abilities[abilityIndex]._Rank + " for " + trainingCost + " gold.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						purchaseString);
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You can't afford that!");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					abilityIndex != -1
						? "You are not ready to upgrade that ability. You need to level up first!"
						: "You don't have that ability to train!");
			}
		}
		public void PopulateQuests(Player player) {
			_AvailableQuests = new List<Quest>();
			Armor.ArmorType questArmorGroup = player._PlayerClass switch {
				Player.PlayerClassType.Mage => Armor.ArmorType.Cloth,
				Player.PlayerClassType.Warrior => Armor.ArmorType.Plate,
				Player.PlayerClassType.Archer => Armor.ArmorType.Leather,
				_ => throw new ArgumentOutOfRangeException()
			};
			switch (_TrainerGroup) {
				case TrainerCategory.Archer:
					_AvailableQuests.Add(new Quest(
						"Slaughterhouse",
						"Look. I'm busy here teaching these kids how to defend themselves. I need you to get some " +
						"practice with that yourself. Why don't you go down into that dungeon and clear a level of it? If it " +
						"moves, kill it. Be like a house cat. Don't ask questions, just kill it, and keep doing that until " +
						"there's nothing left. Do that, come back here, and you'll get a reward. ",
						Quest.QuestType.ClearLevel,
						new Armor(questArmorGroup, Armor.ArmorSlot.Hands, true, player),
						_Name));
					break;
				case TrainerCategory.Warrior:
					_AvailableQuests.Add(new Quest(
						"Hunter Killer",
						"We need some hard souls to do some house cleaning in the dungeon. There's a lot of monsters " +
						"down there and some of them have been wandering out of the dungeon at night to terrorize this town. " +
						"I want you to return the favor. Hunt down and kill a bunch of them. Surely if we thin the ranks down " +
						"then they won't be so likely to stray from the dungeon. Go take care of this for me will you? ",
						Quest.QuestType.KillMonster,
						new Armor(questArmorGroup, Armor.ArmorSlot.Back, true, player),
						_Name));
					break;
				case TrainerCategory.Mage:
					_AvailableQuests.Add(new Quest(
						"Kill Them All",
						"I need you to do something for me. I'm busy training mages here all day but it isn't lost " +
						"on me how many evil creatures are down in that dungeon killing unsuspecting travelers. I want you " +
						"to go in there and kill as many of them as you can. You look like you might have a slightly higher " +
						"chance of surviving, but if you don't, I promise I'll find and bury you someday ok? That seems like " +
						"a reasonable offer to me.",
						Quest.QuestType.KillCount,
						new Armor(questArmorGroup, Armor.ArmorSlot.Chest, true, player),
						_Name));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ShowQuestList(Player player) {
			if (_AvailableQuests == null) {
				PopulateQuests(player);
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Available Quests:");
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (Quest quest in _AvailableQuests) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(quest._Name));
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"You can <consider> <quest name> if you want to obtain quest details.");
		}
		public void OfferQuest(Player player, string[] input) {
			string userInput = InputController.ParseInput(input);
			int questIndex = _AvailableQuests.FindIndex(
				f => f._Name.ToLower().Contains(userInput));
			if (questIndex != -1) {
				_AvailableQuests[questIndex].ShowQuest();
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Will you accept this quest?");
				Console.Clear();
				OutputController.ShowUserOutput(player);
				OutputController.Display.ClearUserOutput();
				string[] questInput = InputController.GetFormattedInput(Console.ReadLine());
				while (questInput[0].ToLower() != "y" && questInput[0].ToLower() != "yes" &&
					   questInput[0].ToLower() != "n" && questInput[0].ToLower() != "no") {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						textInfo.ToTitleCase(_AvailableQuests[questIndex]._Name) + " Consideration:");
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I need either a yes or no answer here.");
					Console.Clear();
					OutputController.ShowUserOutput(player);
					OutputController.Display.ClearUserOutput();
					questInput = InputController.GetFormattedInput(Console.ReadLine());
				}
				if (questInput[0] == "y" || questInput[0] == "yes") {
					player._QuestLog.Add(_AvailableQuests[questIndex]);
					_AvailableQuests.RemoveAt(questIndex);
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"My hero. I am adding the particulars to your quest log.");
				} else {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"Let me know if you change your mind later.");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"I don't have that quest to offer!");
			}
		}
		public void CompleteQuest(Player player, string[] input) {
			string userInput = InputController.ParseInput(input);
			int questIndex = player._QuestLog.FindIndex(
				f => f._Name.ToLower().Contains(userInput));
			Quest quest = player._QuestLog[questIndex];
			if (questIndex != -1) {
				if (quest._QuestGiver == _Name) {
					if (quest._QuestCompleted) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"Congratulations on finishing " + quest._Name + "! Here's your reward.");
						player._Inventory.Add(quest._QuestRewardItem);
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							"You have received: ");
						GearController.StoreRainbowGearOutput(GearController.GetItemDetails(quest._QuestRewardItem));
						player._Gold += quest._QuestRewardGold;
						OutputController.Display.StoreUserOutput(
							Settings.FormatGeneralInfoText(),
							Settings.FormatDefaultBackground(),
							quest._QuestRewardGold + " gold coins.");
						player._QuestLog.RemoveAt(questIndex);
					} else {
						OutputController.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You haven't finished that quest yet!");
					}
				} else {
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"I didn't give you that quest. " + textInfo.ToTitleCase(quest._QuestGiver) + " did.");
				}
			} else {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What quest did you want to turn in?");
			}
		}
	}
}