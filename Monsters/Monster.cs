using DungeonGame.AttackOptions;
using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Interfaces;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Players;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame.Monsters {
	public class Monster : IName, IEffectHolder {
		public enum MonsterType {
			Skeleton,
			Zombie,
			Spider,
			Demon,
			Elemental,
			Vampire,
			Troll,
			Dragon
		}
		public enum ElementalType {
			Fire,
			Air,
			Water
		}
		public enum SkeletonType {
			Warrior,
			Archer,
			Mage
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitPoints { get; set; }
		public int MaxEnergyPoints { get; set; }
		public int EnergyPoints { get; set; }
		public int FireResistance { get; set; }
		public int FrostResistance { get; set; }
		public int ArcaneResistance { get; set; }
		public int ExperienceProvided { get; set; }
		public int Gold { get; set; }
		public bool WasLooted { get; set; }
		public bool InCombat { get; set; }
		public bool IsStunned { get; set; }
		public int StatReplenishInterval { get; set; }
		public MonsterType MonsterCategory { get; set; }
		public ElementalType? ElementalCategory { get; set; }
		public SkeletonType? SkeletonCategory { get; set; }
		public int UnarmedAttackDamage { get; set; }
		public Weapon MonsterWeapon { get; set; }
		public Quiver MonsterQuiver { get; set; }
		public Armor MonsterHeadArmor { get; set; }
		public Armor MonsterBackArmor { get; set; }
		public Armor MonsterChestArmor { get; set; }
		public Armor MonsterWristArmor { get; set; }
		public Armor MonsterHandsArmor { get; set; }
		public Armor MonsterWaistArmor { get; set; }
		public Armor MonsterLegArmor { get; set; }
		public List<IItem> MonsterItems { get; set; }
		public List<IEffect> Effects { get; set; }
		public List<MonsterSpell> Spellbook { get; set; }
		public List<MonsterAbility> Abilities { get; set; }

		public Monster(int level, MonsterType monsterType) {
			MonsterItems = new List<IItem>();
			Effects = new List<IEffect>();
			StatReplenishInterval = 3;
			UnarmedAttackDamage = 5;
			Level = level;
			FireResistance = Level * 5;
			FrostResistance = Level * 5;
			ArcaneResistance = Level * 5;
			MonsterCategory = monsterType;
			int randomNumHitPoint = GameController.GetRandomNumber(20, 40);
			int maxHitPoints = 80 + ((Level - 1) * randomNumHitPoint);
			MaxHitPoints = GameController.RoundNumber(maxHitPoints);
			HitPoints = MaxHitPoints;
			if (MonsterCategory == MonsterType.Spider) {
				Gold = 0;
			} else {
				int randomNumGold = GameController.GetRandomNumber(5, 10);
				Gold = 10 + ((Level - 1) * randomNumGold);
			}
			int expProvided = MaxHitPoints;
			ExperienceProvided = GameController.RoundNumber(expProvided);
			MaxEnergyPoints = 100 + (Level * 10);
			EnergyPoints = MaxEnergyPoints;
			switch (MonsterCategory) {
				case MonsterType.Skeleton:
					int randomSkeletonType = GameController.GetRandomNumber(1, 3);
					SkeletonCategory = randomSkeletonType switch {
						1 => SkeletonType.Archer,
						2 => SkeletonType.Warrior,
						3 => SkeletonType.Mage,
						_ => throw new ArgumentOutOfRangeException()
					};
					switch (SkeletonCategory) {
						case SkeletonType.Warrior:
						case SkeletonType.Archer:
							break;
						case SkeletonType.Mage:
							Spellbook = new List<MonsterSpell> {
								new MonsterSpell("frostbolt", 50, MonsterSpell.SpellType.Frostbolt, Level),
								new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, Level)};
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case MonsterType.Zombie:
					break;
				case MonsterType.Spider:
					Abilities = new List<MonsterAbility> {
						new MonsterAbility("poison bite", 50, MonsterAbility.Ability.PoisonBite, Level)};
					break;
				case MonsterType.Demon:
					Spellbook = new List<MonsterSpell> {
						new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, Level)};
					break;
				case MonsterType.Elemental:
					int randomNum = GameController.GetRandomNumber(1, 3);
					int randomPhysicalDmg = GameController.GetRandomNumber(20, 26);
					UnarmedAttackDamage = randomNum switch {
						1 => randomPhysicalDmg + ((level - 1) * 1),
						2 => randomPhysicalDmg + ((level - 1) * 2),
						3 => randomPhysicalDmg + ((level - 1) * 3),
						_ => throw new ArgumentOutOfRangeException()
					};
					int randomElementalType = GameController.GetRandomNumber(1, 3);
					ElementalCategory = randomElementalType switch {
						1 => ElementalType.Air,
						2 => ElementalType.Fire,
						3 => ElementalType.Water,
						_ => throw new ArgumentOutOfRangeException()
					};
					Spellbook = ElementalCategory switch {
						ElementalType.Fire => new List<MonsterSpell> {
								new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, Level)},
						ElementalType.Air => new List<MonsterSpell> {
								new MonsterSpell("lightning", 50, MonsterSpell.SpellType.Lightning, Level)},
						ElementalType.Water => new List<MonsterSpell> {
								new MonsterSpell("frostbolt", 50, MonsterSpell.SpellType.Frostbolt, Level)},
						_ => throw new ArgumentOutOfRangeException(),
					};
					break;
				case MonsterType.Vampire:
					Abilities = new List<MonsterAbility> {
						new MonsterAbility("blood leech", 50, MonsterAbility.Ability.BloodLeech, Level)};
					break;
				case MonsterType.Troll:
					break;
				case MonsterType.Dragon:
					Abilities = new List<MonsterAbility> {
						new MonsterAbility("tail whip", 50, MonsterAbility.Ability.TailWhip, Level)};
					Spellbook = new List<MonsterSpell> {
						new MonsterSpell("fire breath", 50, MonsterSpell.SpellType.Fireball, Level)};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public AttackOption DetermineAttack(Player player, bool addRandomChance) {
			List<AttackOption> attackOptions = new List<AttackOption>();
			if (MonsterWeapon != null && MonsterWeapon.Equipped) {
				attackOptions.Add(new
					AttackOption(AttackType.Physical,
						MonsterWeapon._RegDamage - player.ArmorRating(this), -1));
			} else {
				attackOptions.Add(new
					AttackOption(AttackType.Physical, UnarmedAttackDamage, -1));
			}
			if (Spellbook != null) {
				for (int i = 0; i < Spellbook.Count; i++) {
					if (EnergyPoints < Spellbook[i]._EnergyCost) {
						continue;
					}

					switch (Spellbook[i]._SpellCategory) {
						case MonsterSpell.SpellType.Fireball:
						case MonsterSpell.SpellType.Frostbolt:
						case MonsterSpell.SpellType.Lightning:
							int spellTotalDamage = 0;
							if (Spellbook[i]._Offensive._AmountOverTime == 0) {
								spellTotalDamage = Spellbook[i]._Offensive._Amount;
							} else {
								spellTotalDamage = Spellbook[i]._Offensive._Amount + (Spellbook[i]._Offensive._AmountOverTime *
									Spellbook[i]._Offensive._AmountMaxRounds);
							}
							attackOptions.Add(new
								AttackOption(AttackType.Spell, spellTotalDamage, i));
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			if (Abilities != null) {
				for (int i = 0; i < Abilities.Count; i++) {
					if (EnergyPoints < Abilities[i].EnergyCost) {
						continue;
					}

					switch (Abilities[i].AbilityCategory) {
						case MonsterAbility.Ability.PoisonBite:
						case MonsterAbility.Ability.BloodLeech:
						case MonsterAbility.Ability.TailWhip:
							int abilityTotalDamage = 0;
							if (Abilities[i].Offensive._AmountOverTime == 0) {
								abilityTotalDamage = Abilities[i].Offensive._Amount * 2;
							} else {
								abilityTotalDamage = Abilities[i].Offensive._Amount + (Abilities[i].Offensive._AmountOverTime *
									Abilities[i].Offensive._AmountMaxRounds);
							}
							attackOptions.Add(new
								AttackOption(AttackType.Ability, abilityTotalDamage, i));
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			// Sort and order attack options so that the one which causes the most damage is at the top
			attackOptions = attackOptions.OrderByDescending(attack => attack.DamageAmount).ToList();

			if (addRandomChance) {
				int randomMonsterAttack = GameController.GetRandomNumber(1, 10);

				// If an element of random chance is required, provide a 50% chance of an attack being selected at random
				if (randomMonsterAttack > 5) {
					int randomAttackChoice = GameController.GetRandomNumber(0, attackOptions.Count - 1);

					return attackOptions[randomAttackChoice];
				}
			}

			// If an element of random chance is not selected, the attack option which causes the most damage is returned
			return attackOptions[0];
		}
		public void Attack(Player player) {
			AttackOption attackOption = DetermineAttack(player, true);
			switch (attackOption.AttackCategory) {
				case AttackType.Physical:
					PhysicalAttack(player);
					break;
				case AttackType.Spell:
					switch (Spellbook[attackOption.AttackIndex]._SpellCategory) {
						case MonsterSpell.SpellType.Fireball:
							Spellbook[attackOption.AttackIndex].CastFireOffense(this, player, attackOption.AttackIndex);
							break;
						case MonsterSpell.SpellType.Frostbolt:
							Spellbook[attackOption.AttackIndex].CastFrostOffense(this, player, attackOption.AttackIndex);
							break;
						case MonsterSpell.SpellType.Lightning:
							Spellbook[attackOption.AttackIndex].CastArcaneOffense(this, player, attackOption.AttackIndex);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case AttackType.Ability:
					switch (Abilities[attackOption.AttackIndex].AbilityCategory) {
						case MonsterAbility.Ability.PoisonBite:
						case MonsterAbility.Ability.TailWhip:
							Abilities[attackOption.AttackIndex].UseOffenseDamageAbility(this, player, attackOption.AttackIndex);
							break;
						case MonsterAbility.Ability.BloodLeech:
							Abilities[attackOption.AttackIndex].UseBloodLeechAbility(this, player, attackOption.AttackIndex);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			GearController.DecreaseArmorDurability(player);
		}
		private void PhysicalAttack(Player player) {
			int attackAmount = 0;

			try {
				if (MonsterWeapon.Equipped && MonsterWeapon._WeaponGroup != Weapon.WeaponType.Bow) {
					attackAmount += MonsterWeapon.Attack();
				}
				if (MonsterWeapon.Equipped &&
					MonsterWeapon._WeaponGroup == Weapon.WeaponType.Bow &&
					MonsterQuiver.HaveArrows()) {
					MonsterQuiver.UseArrow();
					attackAmount += MonsterWeapon.Attack();
				}
				if (MonsterWeapon.Equipped &&
					MonsterWeapon._WeaponGroup == Weapon.WeaponType.Bow &&
					!MonsterQuiver.HaveArrows()) {
					attackAmount += UnarmedAttackDamage;
				}
			} catch (NullReferenceException) {
				if (MonsterCategory == MonsterType.Elemental) {
					attackAmount += UnarmedAttackDamage;
				} else {
					string monsterDisarmed = $"The {Name} is disarmed! They are going hand to hand!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						monsterDisarmed);
					attackAmount += UnarmedAttackDamage;
				}
			}

			int randomChanceToHit = GameController.GetRandomNumber(1, 100);
			double chanceToDodge = player.DodgeChance;
			if (chanceToDodge > 50) {
				chanceToDodge = 50;
			}

			if (randomChanceToHit <= chanceToDodge) {
				string missString = $"The {Name} missed you!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					missString);
				return;
			}

			int baseAttackAmount = attackAmount;

			foreach (IEffect effect in player.Effects.Where(effect => effect.IsEffectExpired is false).ToList()) {
				if (effect is FrozenEffect frozenEffect) {
					attackAmount = frozenEffect.GetIncreasedDamageFromFrozen(attackAmount);

					frozenEffect.ProcessFrozenRound();
				}

				if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
					attackAmount = changeMonsterDmgEffect.GetUpdatedDamageFromChange(attackAmount);

					changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
				}

				if (effect is IChangeDamageEffect changeDamageEffect) {
					int incomingDamage = attackAmount;

					attackAmount = changeDamageEffect.GetChangedDamageFromEffect(incomingDamage);

					changeDamageEffect.ProcessChangeDamageRound(incomingDamage);
				}

				if (effect is ReflectDamageEffect reflectDamageEffect) {
					int reflectAmount = reflectDamageEffect.GetReflectedDamageAmount(attackAmount);

					HitPoints -= reflectAmount;

					reflectDamageEffect.ProcessReflectDamageRound(reflectAmount);

					attackAmount -= reflectAmount;
				}

				if (baseAttackAmount > attackAmount && attackAmount - player.ArmorRating(this) <= 0) {
					string effectAbsorbString = $"Your {effect.Name} absorbed all of {Name}'s attack!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);
					return;
				}

				GameController.RemovedExpiredEffectsAsync(this);
			}

			if (attackAmount - player.ArmorRating(this) <= 0) {
				string armorAbsorbString = $"Your armor absorbed all of {Name}'s attack!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					armorAbsorbString);
			} else if (attackAmount - player.ArmorRating(this) > 0) {
				attackAmount -= player.ArmorRating(this);
				string hitString = $"The {Name} hits you for {attackAmount} physical damage.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					hitString);
				player.HitPoints -= attackAmount;
			}
		}
		public int ArmorRating(Player player) {
			int totalArmorRating = MonsterController.CheckArmorRating(this);
			int levelDiff = player.Level - Level;
			double armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			double adjArmorRating = totalArmorRating * armorMultiplier;
			if (ElementalCategory != null) {
				adjArmorRating += 20;
			}

			return (int)adjArmorRating;
		}
		public void MonsterDeath(Player player) {
			player.InCombat = false;
			InCombat = false;
			Effects.Clear();
			string defeatString = $"You have defeated the {Name}!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				defeatString);
			string expGainString = $"You have gained {ExperienceProvided} experience!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				expGainString);
			foreach (IItem loot in MonsterItems.Where(item => item is IEquipment)) {
				IEquipment equippableItem = loot as IEquipment;
				equippableItem.Equipped = false;
			}
			Name = $"Dead {Name}";
			Desc = "A corpse of a monster you killed.";
			player.Experience += ExperienceProvided;
			PlayerController.LevelUpCheck(player);
			if (player.QuestLog == null) {
				return;
			}

			foreach (Quest quest in player.QuestLog) {
				quest.UpdateQuestProgress(this);
			}
		}
	}
}