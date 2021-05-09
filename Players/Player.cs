using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Helpers;
using DungeonGame.Interfaces;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Consumables.Arrow;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Monsters;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame.Players {
	public class Player : IEffectHolder {
		public string Name { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitPoints { get; set; }
		public int? MaxRagePoints { get; set; }
		public int? RagePoints { get; set; }
		public int? MaxComboPoints { get; set; }
		public int? ComboPoints { get; set; }
		public int? MaxManaPoints { get; set; }
		public int? ManaPoints { get; set; }
		public int FireResistance { get; set; }
		public int FrostResistance { get; set; }
		public int ArcaneResistance { get; set; }
		public int Strength { get; set; }
		public int Intelligence { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int MaxCarryWeight { get; set; }
		public int Gold { get; set; }
		public int Experience { get; set; }
		public int ExperienceToLevel { get; set; }
		public int Level { get; set; }
		public bool InCombat { get; set; }
		public bool CanSave { get; set; }
		public bool CanWearCloth { get; set; }
		public bool CanWearLeather { get; set; }
		public bool CanWearPlate { get; set; }
		public bool CanUseDagger { get; set; }
		public bool CanUseOneHandedSword { get; set; }
		public bool CanUseTwoHandedSword { get; set; }
		public bool CanUseAxe { get; set; }
		public bool CanUseBow { get; set; }
		public int StatReplenishInterval { get; set; }
		public double DodgeChance { get; set; }
		public PlayerClassType PlayerClass { get; set; }
		public Quiver PlayerQuiver { get; set; }
		public Armor PlayerHeadArmor { get; set; }
		public Armor PlayerBackArmor { get; set; }
		public Armor PlayerChestArmor { get; set; }
		public Armor PlayerWristArmor { get; set; }
		public Armor PlayerHandsArmor { get; set; }
		public Armor PlayerWaistArmor { get; set; }
		public Armor PlayerLegsArmor { get; set; }
		public Weapon PlayerWeapon { get; set; }
		public Coordinate PlayerLocation { get; set; }
		public List<IEffect> Effects { get; set; }
		public List<PlayerSpell> Spellbook { get; set; }
		public List<PlayerAbility> Abilities { get; set; }
		public List<IItem> Inventory { get; set; }
		public List<Quest> QuestLog { get; set; }

		// Default constructor for JSON serialization
		public Player() { }
		public Player(string name, PlayerClassType playerClass) {
			Name = name;
			PlayerClass = playerClass;
			StatReplenishInterval = 3;
			Level = 1;
			ExperienceToLevel = Settings.GetBaseExperienceToLevel();
			Inventory = new List<IItem>();
			Effects = new List<IEffect>();
			QuestLog = new List<Quest>();
			FireResistance = 5;
			FrostResistance = 5;
			ArcaneResistance = 5;
			switch (PlayerClass) {
				case PlayerClassType.Mage:
					for (int i = 0; i < 3; i++) {
						Inventory.Add(new ManaPotion(PotionStrength.Minor));
					}
					Spellbook = new List<PlayerSpell>();
					Strength = 10;
					Dexterity = 5;
					Intelligence = 15;
					Constitution = 10;
					MaxManaPoints = Intelligence * 10;
					ManaPoints = MaxManaPoints;
					CanWearCloth = true;
					CanUseDagger = true;
					CanUseOneHandedSword = true;
					Inventory.Add(new Weapon(Level, WeaponType.Dagger));
					Inventory.Add(new Armor(
						1, ArmorType.Cloth, ArmorSlot.Head));
					Inventory.Add(new Armor(
						1, ArmorType.Cloth, ArmorSlot.Chest));
					Inventory.Add(new Armor(
						1, ArmorType.Cloth, ArmorSlot.Legs));
					Spellbook.Add(new PlayerSpell(
						"fireball", 35, 1, SpellType.Fireball, 1));
					Spellbook.Add(new PlayerSpell(
						"heal", 25, 1, SpellType.Heal, 1));
					Spellbook.Add(new PlayerSpell(
						"diamondskin", 25, 1, SpellType.Diamondskin, 1));
					Spellbook.Add(new PlayerSpell(
						"frostbolt", 25, 1, SpellType.Frostbolt, 1));
					Spellbook.Add(new PlayerSpell(
						"lightning", 25, 1, SpellType.Lightning, 1));
					Spellbook.Add(new PlayerSpell(
						"rejuvenate", 25, 1, SpellType.Rejuvenate, 1));
					break;
				case PlayerClassType.Warrior:
					for (int i = 0; i < 3; i++) {
						Inventory.Add(new HealthPotion(PotionStrength.Minor));
					}
					Abilities = new List<PlayerAbility>();
					Strength = 15;
					Dexterity = 5;
					Intelligence = 5;
					Constitution = 15;
					MaxRagePoints = Strength * 10;
					RagePoints = MaxRagePoints;
					CanWearCloth = true;
					CanWearLeather = true;
					CanWearPlate = true;
					CanUseAxe = true;
					CanUseDagger = true;
					CanUseBow = true;
					CanUseOneHandedSword = true;
					CanUseTwoHandedSword = true;
					Inventory.Add(new Weapon(Level, WeaponType.TwoHandedSword));
					Inventory.Add(new Armor(
						1, ArmorType.Plate, ArmorSlot.Head));
					Inventory.Add(new Armor(
						1, ArmorType.Plate, ArmorSlot.Chest));
					Inventory.Add(new Armor(
						1, ArmorType.Plate, ArmorSlot.Legs));
					Abilities.Add(new PlayerAbility(
						"charge", 25, 1, WarriorAbility.Charge, 1));
					Abilities.Add(new PlayerAbility(
						"slash", 40, 1, WarriorAbility.Slash, 1));
					Abilities.Add(new PlayerAbility(
						"rend", 25, 1, WarriorAbility.Rend, 1));
					Abilities.Add(new PlayerAbility(
						"block", 25, 1, WarriorAbility.Block, 1));
					Abilities.Add(new PlayerAbility(
						"berserk", 40, 1, WarriorAbility.Berserk, 1));
					Abilities.Add(new PlayerAbility(
						"disarm", 25, 1, WarriorAbility.Disarm, 1));
					break;
				case PlayerClassType.Archer:
					for (int i = 0; i < 3; i++) {
						Inventory.Add(new HealthPotion(PotionStrength.Minor));
					}
					Abilities = new List<PlayerAbility>();
					Strength = 10;
					Dexterity = 15;
					Intelligence = 5;
					Constitution = 10;
					MaxComboPoints = Dexterity * 10;
					ComboPoints = MaxComboPoints;
					CanWearCloth = true;
					CanWearLeather = true;
					CanUseBow = true;
					CanUseDagger = true;
					CanUseOneHandedSword = true;
					Inventory.Add(new Weapon(Level, WeaponType.Bow));
					Inventory.Add(new Armor(
						1, ArmorType.Leather, ArmorSlot.Head));
					Inventory.Add(new Armor(
						1, ArmorType.Leather, ArmorSlot.Chest));
					Inventory.Add(new Armor(
						1, ArmorType.Leather, ArmorSlot.Legs));
					Inventory.Add(new Quiver("basic quiver", 50, 15));
					Abilities.Add(new PlayerAbility("precise shot", 40, 1,
						ArcherAbility.Precise, 1));
					Abilities.Add(new PlayerAbility(
						"gut shot", 25, 1, ArcherAbility.Gut, 1));
					Abilities.Add(new PlayerAbility(
						"stun shot", 25, 1, ArcherAbility.Stun, 1));
					Abilities.Add(new PlayerAbility("double shot", 25, 1,
						ArcherAbility.Double, 1));
					Abilities.Add(new PlayerAbility("wound shot", 40, 1,
						ArcherAbility.Wound, 1));
					Abilities.Add(new PlayerAbility("distance shot", 25, 1,
						ArcherAbility.Distance, 1));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			MaxHitPoints = Constitution * 10;
			HitPoints = MaxHitPoints;
			MaxCarryWeight = (int)(Strength * 2.5);
			DodgeChance = Dexterity * 1.5;
		}

		public int ArmorRating(Monster opponent) {
			int totalArmorRating = GearHelper.CheckArmorRating(this);
			int levelDiff = opponent.Level - Level;
			double armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			double adjArmorRating = totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}

		public int PhysicalAttack(Monster monster) {
			int attackAmount = 0;

			try {
				if (PlayerWeapon.Equipped && PlayerWeapon.WeaponGroup != WeaponType.Bow) {
					attackAmount = PlayerWeapon.Attack();
				}
				if (PlayerWeapon.Equipped &&
					PlayerWeapon.WeaponGroup == WeaponType.Bow &&
					PlayerQuiver.HaveArrows()) {
					PlayerQuiver.UseArrow();
					attackAmount = PlayerWeapon.Attack();
				}
				if (PlayerWeapon.Equipped &&
					PlayerWeapon.WeaponGroup == WeaponType.Bow &&
					!PlayerQuiver.HaveArrows()) {
					Quiver.DisplayOutOfArrowsMessage();
					attackAmount = 5;
				}
			} catch (NullReferenceException) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your weapon is not equipped! Going hand to hand!");
				attackAmount = 5;
			}

			foreach (IEffect effect in Effects) {
				if (effect is ChangePlayerDamageEffect changeEffect) {
					attackAmount = changeEffect.GetUpdatedDamageFromChange(attackAmount);
				}
			}

			foreach (IEffect effect in monster.Effects) {
				if (effect is FrozenEffect frozenEffect) {
					attackAmount = frozenEffect.GetIncreasedDamageFromFrozen(attackAmount);
				}
			}

			return attackAmount;
		}

		public void AttemptDrinkPotion(string input) {
			int index = Inventory.FindIndex(item => item is IPotion && item.Name.Contains(input));

			if (index == -1) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					Settings.DrinkPotionFailMessage());
			} else {
				IPotion potionToDrink = Inventory[index] as IPotion;
				potionToDrink.DrinkPotion(this);

				Inventory.RemoveAt(index);
			}
		}

		public void ReloadQuiver() {
			int index = Inventory.FindIndex(f => f.GetType() == typeof(Arrows));
			if (index != -1) {
				Arrows arrows = Inventory[index] as Arrows;
				arrows.LoadPlayerQuiverWithArrows(this);
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You reloaded your quiver.");
				if (arrows.Quantity == 0) {
					Inventory.RemoveAt(index);
				}
			} else {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have any arrows!");
			}
		}
		public void UseAbility(string[] input) {
			StringBuilder inputName = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
			}
			int index = Abilities.FindIndex(
				f => f.Name == inputName.ToString() || f.Name.Contains(input[1]));
			if (index != -1 &&
				RagePoints >= Abilities[index].RageCost &&
				PlayerClass == PlayerClassType.Warrior) {
				switch (Abilities[index].WarAbilityCategory) {
					case WarriorAbility.Slash:
					case WarriorAbility.Rend:
					case WarriorAbility.Charge:
					case WarriorAbility.Block:
					case WarriorAbility.Berserk:
					case WarriorAbility.Disarm:
					case WarriorAbility.Onslaught:
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that ability outside combat!");
						return;
					case WarriorAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case WarriorAbility.PowerAura:
						PlayerAbility.UsePowerAura(this, index);
						return;
					case WarriorAbility.WarCry:
						PlayerAbility.UseWarCry(this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
				ComboPoints >= Abilities[index].ComboCost &&
				PlayerClass == PlayerClassType.Archer) {
				switch (Abilities[index].ArcAbilityCategory) {
					case ArcherAbility.Distance:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						string direction = input.Last();
						PlayerAbility.UseDistanceAbility(this, index, direction);
						return;
					case ArcherAbility.Gut:
					case ArcherAbility.Precise:
					case ArcherAbility.Stun:
					case ArcherAbility.Double:
					case ArcherAbility.Wound:
					case ArcherAbility.ImmolatingArrow:
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that ability outside combat!");
						return;
					case ArcherAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case ArcherAbility.SwiftAura:
						PlayerAbility.UseSwiftAura(this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
		public void UseAbility(Monster opponent, string[] input) {
			StringBuilder inputName = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
				if (i != input.Length - 1) {
					inputName.Append(" ");
				}
			}
			int index = Abilities.FindIndex(
				f => f.Name == inputName.ToString() || f.Name == input[1] || f.Name.Contains(input[1]) ||
					 f.Name.Contains(inputName.ToString()));
			if (index != -1 &&
				RagePoints >= Abilities[index].RageCost &&
				PlayerClass == PlayerClassType.Warrior) {
				switch (Abilities[index].WarAbilityCategory) {
					case WarriorAbility.Slash:
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case WarriorAbility.Rend:
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case WarriorAbility.Charge:
						PlayerAbility.UseStunAbility(opponent, this, index);
						return;
					case WarriorAbility.Block:
						PlayerAbility.UseDefenseAbility(this, index);
						return;
					case WarriorAbility.Berserk:
						PlayerAbility.UseBerserkAbility(this, index);
						return;
					case WarriorAbility.Disarm:
						PlayerAbility.UseDisarmAbility(opponent, this, index);
						return;
					case WarriorAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case WarriorAbility.PowerAura:
						PlayerAbility.UsePowerAura(this, index);
						return;
					case WarriorAbility.WarCry:
						PlayerAbility.UseWarCry(this, index);
						return;
					case WarriorAbility.Onslaught:
						for (int i = 0; i < 2; i++) {
							if (RagePoints >= Abilities[index].RageCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							} else {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatAttackFailText(),
									Settings.FormatDefaultBackground(),
									"You didn't have enough rage points for the second attack!");
							}
						}
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
				ComboPoints >= Abilities[index].ComboCost &&
				PlayerClass == PlayerClassType.Archer) {
				switch (Abilities[index].ArcAbilityCategory) {
					case ArcherAbility.Distance:
						return;
					case ArcherAbility.Gut:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case ArcherAbility.Precise:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case ArcherAbility.Stun:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseStunAbility(opponent, this, index);
						return;
					case ArcherAbility.Double:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						for (int i = 0; i < 2; i++) {
							if (ComboPoints >= Abilities[index].ComboCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							} else {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatAttackFailText(),
									Settings.FormatDefaultBackground(),
									"You didn't have enough combo points for the second shot!");
							}
						}
						return;
					case ArcherAbility.Wound:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case ArcherAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case ArcherAbility.SwiftAura:
						PlayerAbility.UseSwiftAura(this, index);
						return;
					case ArcherAbility.ImmolatingArrow:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case ArcherAbility.Ambush:
						if (PlayerWeapon?.WeaponGroup != WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						if (!InCombat) {
							PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						} else {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								$"You can't ambush {opponent.Name}, you're already in combat!");
						}
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
		public void CastSpell(string inputName) {
			int index = Spellbook.FindIndex(f => f.Name == inputName);
			if (index != -1 &&
				ManaPoints >= Spellbook[index].ManaCost &&
				PlayerClass == PlayerClassType.Mage) {
				switch (Spellbook[index].SpellCategory) {
					case SpellType.Heal:
						PlayerSpell.CastHealing(this, index);
						return;
					case SpellType.Rejuvenate:
						PlayerSpell.CastHealing(this, index);
						return;
					case SpellType.Diamondskin:
						PlayerSpell.CastAugmentArmor(this, index);
						return;
					case SpellType.TownPortal:
						PlayerSpell.CastTownPortal(this, index);
						return;
					case SpellType.Reflect:
						PlayerSpell.CastReflectDamage(this, index);
						return;
					case SpellType.Fireball:
					case SpellType.Frostbolt:
					case SpellType.Lightning:
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that spell outside combat!");
						return;
					case SpellType.ArcaneIntellect:
						PlayerSpell.CastArcaneIntellect(this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
		public void CastSpell(Monster opponent, string inputName) {
			int index = Spellbook.FindIndex(f => f.Name == inputName);
			if (index != -1 &&
				ManaPoints >= Spellbook[index].ManaCost &&
				PlayerClass == PlayerClassType.Mage) {
				switch (Spellbook[index].SpellCategory) {
					case SpellType.Fireball:
						PlayerSpell.CastFireOffense(opponent, this, index);
						return;
					case SpellType.Frostbolt:
						PlayerSpell.CastFrostOffense(opponent, this, index);
						return;
					case SpellType.Lightning:
						PlayerSpell.CastArcaneOffense(opponent, this, index);
						return;
					case SpellType.Heal:
						PlayerSpell.CastHealing(this, index);
						return;
					case SpellType.Rejuvenate:
						PlayerSpell.CastHealing(this, index);
						return;
					case SpellType.Diamondskin:
						PlayerSpell.CastAugmentArmor(this, index);
						return;
					case SpellType.Reflect:
						PlayerSpell.CastReflectDamage(this, index);
						return;
					case SpellType.TownPortal:
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							"You cannot cast a portal during combat!");
						return;
					case SpellType.ArcaneIntellect:
						PlayerSpell.CastArcaneIntellect(this, index);
						return;
					case SpellType.FrostNova:
						PlayerSpell.CastFrostOffense(opponent, this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
	}
}