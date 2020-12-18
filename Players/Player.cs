using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Monsters;
using DungeonGame.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame.Players {
	public class Player {
		public enum PlayerClassType {
			Mage,
			Warrior,
			Archer
		}
		public string _Name { get; set; }
		public int _MaxHitPoints { get; set; }
		public int _HitPoints { get; set; }
		public int? _MaxRagePoints { get; set; }
		public int? _RagePoints { get; set; }
		public int? _MaxComboPoints { get; set; }
		public int? _ComboPoints { get; set; }
		public int? _MaxManaPoints { get; set; }
		public int? _ManaPoints { get; set; }
		public int _FireResistance { get; set; }
		public int _FrostResistance { get; set; }
		public int _ArcaneResistance { get; set; }
		public int _Strength { get; set; }
		public int _Intelligence { get; set; }
		public int _Dexterity { get; set; }
		public int _Constitution { get; set; }
		public int _MaxCarryWeight { get; set; }
		public int _Gold { get; set; }
		public int _Experience { get; set; }
		public int _ExperienceToLevel { get; set; }
		public int _Level { get; set; }
		public bool _InCombat { get; set; }
		public bool _CanSave { get; set; }
		public bool _CanWearCloth { get; set; }
		public bool _CanWearLeather { get; set; }
		public bool _CanWearPlate { get; set; }
		public bool _CanUseDagger { get; set; }
		public bool _CanUseOneHandedSword { get; set; }
		public bool _CanUseTwoHandedSword { get; set; }
		public bool _CanUseAxe { get; set; }
		public bool _CanUseBow { get; set; }
		public int _StatReplenishInterval { get; set; }
		public double _DodgeChance { get; set; }
		public PlayerClassType _PlayerClass { get; set; }
		public Quiver _PlayerQuiver { get; set; }
		public Armor _PlayerHeadArmor { get; set; }
		public Armor _PlayerBackArmor { get; set; }
		public Armor _PlayerChestArmor { get; set; }
		public Armor _PlayerWristArmor { get; set; }
		public Armor _PlayerHandsArmor { get; set; }
		public Armor _PlayerWaistArmor { get; set; }
		public Armor _PlayerLegsArmor { get; set; }
		public Weapon _PlayerWeapon { get; set; }
		public Coordinate _PlayerLocation { get; set; }
		public List<Effect> _Effects { get; set; }
		public List<PlayerSpell> _Spellbook { get; set; }
		public List<PlayerAbility> _Abilities { get; set; }
		public List<IItem> _Inventory { get; set; }
		public List<Quest> _QuestLog { get; set; }

		// Default constructor for JSON serialization
		public Player() { }
		public Player(string name, PlayerClassType playerClass) {
			_Name = name;
			_PlayerClass = playerClass;
			_StatReplenishInterval = 3;
			_Level = 1;
			_ExperienceToLevel = Settings.GetBaseExperienceToLevel();
			_Inventory = new List<IItem>();
			_Effects = new List<Effect>();
			_QuestLog = new List<Quest>();
			_FireResistance = 5;
			_FrostResistance = 5;
			_ArcaneResistance = 5;
			switch (_PlayerClass) {
				case PlayerClassType.Mage:
					for (int i = 0; i < 3; i++) {
						_Inventory.Add(new ManaPotion(PotionStrength.Minor));
					}
					_Spellbook = new List<PlayerSpell>();
					_Strength = 10;
					_Dexterity = 5;
					_Intelligence = 15;
					_Constitution = 10;
					_MaxManaPoints = _Intelligence * 10;
					_ManaPoints = _MaxManaPoints;
					_CanWearCloth = true;
					_CanUseDagger = true;
					_CanUseOneHandedSword = true;
					_Inventory.Add(new Weapon(_Level, Weapon.WeaponType.Dagger));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Chest));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Legs));
					_Spellbook.Add(new PlayerSpell(
						"fireball", 35, 1, PlayerSpell.SpellType.Fireball, 1));
					_Spellbook.Add(new PlayerSpell(
						"heal", 25, 1, PlayerSpell.SpellType.Heal, 1));
					_Spellbook.Add(new PlayerSpell(
						"diamondskin", 25, 1, PlayerSpell.SpellType.Diamondskin, 1));
					_Spellbook.Add(new PlayerSpell(
						"frostbolt", 25, 1, PlayerSpell.SpellType.Frostbolt, 1));
					_Spellbook.Add(new PlayerSpell(
						"lightning", 25, 1, PlayerSpell.SpellType.Lightning, 1));
					_Spellbook.Add(new PlayerSpell(
						"rejuvenate", 25, 1, PlayerSpell.SpellType.Rejuvenate, 1));
					break;
				case PlayerClassType.Warrior:
					for (int i = 0; i < 3; i++) {
						_Inventory.Add(new HealthPotion(PotionStrength.Minor));
					}
					_Abilities = new List<PlayerAbility>();
					_Strength = 15;
					_Dexterity = 5;
					_Intelligence = 5;
					_Constitution = 15;
					_MaxRagePoints = _Strength * 10;
					_RagePoints = _MaxRagePoints;
					_CanWearCloth = true;
					_CanWearLeather = true;
					_CanWearPlate = true;
					_CanUseAxe = true;
					_CanUseDagger = true;
					_CanUseBow = true;
					_CanUseOneHandedSword = true;
					_CanUseTwoHandedSword = true;
					_Inventory.Add(new Weapon(_Level, Weapon.WeaponType.TwoHandedSword));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Head));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Chest));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs));
					_Abilities.Add(new PlayerAbility(
						"charge", 25, 1, PlayerAbility.WarriorAbility.Charge, 1));
					_Abilities.Add(new PlayerAbility(
						"slash", 40, 1, PlayerAbility.WarriorAbility.Slash, 1));
					_Abilities.Add(new PlayerAbility(
						"rend", 25, 1, PlayerAbility.WarriorAbility.Rend, 1));
					_Abilities.Add(new PlayerAbility(
						"block", 25, 1, PlayerAbility.WarriorAbility.Block, 1));
					_Abilities.Add(new PlayerAbility(
						"berserk", 40, 1, PlayerAbility.WarriorAbility.Berserk, 1));
					_Abilities.Add(new PlayerAbility(
						"disarm", 25, 1, PlayerAbility.WarriorAbility.Disarm, 1));
					break;
				case PlayerClassType.Archer:
					for (int i = 0; i < 3; i++) {
						_Inventory.Add(new HealthPotion(PotionStrength.Minor));
					}
					_Abilities = new List<PlayerAbility>();
					_Strength = 10;
					_Dexterity = 15;
					_Intelligence = 5;
					_Constitution = 10;
					_MaxComboPoints = _Dexterity * 10;
					_ComboPoints = _MaxComboPoints;
					_CanWearCloth = true;
					_CanWearLeather = true;
					_CanUseBow = true;
					_CanUseDagger = true;
					_CanUseOneHandedSword = true;
					_Inventory.Add(new Weapon(_Level, Weapon.WeaponType.Bow));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Head));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest));
					_Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Legs));
					_Inventory.Add(new Quiver("basic quiver", 50, 15));
					_Abilities.Add(new PlayerAbility("precise shot", 40, 1,
						PlayerAbility.ArcherAbility.Precise, 1));
					_Abilities.Add(new PlayerAbility(
						"gut shot", 25, 1, PlayerAbility.ArcherAbility.Gut, 1));
					_Abilities.Add(new PlayerAbility(
						"stun shot", 25, 1, PlayerAbility.ArcherAbility.Stun, 1));
					_Abilities.Add(new PlayerAbility("double shot", 25, 1,
						PlayerAbility.ArcherAbility.Double, 1));
					_Abilities.Add(new PlayerAbility("wound shot", 40, 1,
						PlayerAbility.ArcherAbility.Wound, 1));
					_Abilities.Add(new PlayerAbility("distance shot", 25, 1,
						PlayerAbility.ArcherAbility.Distance, 1));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_MaxHitPoints = _Constitution * 10;
			_HitPoints = _MaxHitPoints;
			_MaxCarryWeight = (int)(_Strength * 2.5);
			_DodgeChance = _Dexterity * 1.5;
		}

		public int ArmorRating(Monster opponent) {
			int totalArmorRating = GearController.CheckArmorRating(this);
			int levelDiff = opponent._Level - _Level;
			double armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			double adjArmorRating = totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public int PhysicalAttack(Monster opponent) {
			int attackAmount = 0;
			try {
				if (_PlayerWeapon._Equipped && _PlayerWeapon._WeaponGroup != Weapon.WeaponType.Bow) {
					attackAmount = _PlayerWeapon.Attack();
				}
				if (_PlayerWeapon._Equipped &&
					_PlayerWeapon._WeaponGroup == Weapon.WeaponType.Bow &&
					_PlayerQuiver.HaveArrows()) {
					_PlayerQuiver.UseArrow();
					attackAmount = _PlayerWeapon.Attack();
				}
				if (_PlayerWeapon._Equipped &&
					_PlayerWeapon._WeaponGroup == Weapon.WeaponType.Bow &&
					!_PlayerQuiver.HaveArrows()) {
					Quiver.DisplayOutOfArrowsMessage();
					attackAmount = 5;
				}
			} catch (NullReferenceException) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your weapon is not equipped! Going hand to hand!");
				attackAmount = 5;
			}
			foreach (Effect effect in _Effects) {
				switch (effect._EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						attackAmount += effect._EffectAmountOverTime;
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			foreach (Effect effect in opponent._Effects) {
				switch (effect._EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						double frozenAttackAmount = attackAmount * effect._EffectMultiplier;
						attackAmount = (int)frozenAttackAmount;
						effect.FrozenRound(opponent);
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return attackAmount;
		}

		public void AttemptDrinkPotion(string input) {
			int index = _Inventory.FindIndex(item => item is IPotion && item._Name.Contains(input));

			if (index == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					Settings.DrinkPotionFailMessage());
			} else {
				IPotion potionToDrink = _Inventory[index] as IPotion;
				potionToDrink.DrinkPotion(this);

				_Inventory.RemoveAt(index);
			}
		}

		public void ReloadQuiver() {
			int index = _Inventory.FindIndex(f => f.GetType() == typeof(Arrows));
			if (index != -1) {
				Arrows arrows = _Inventory[index] as Arrows;
				arrows.LoadPlayerQuiverWithArrows(this);
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You reloaded your quiver.");
				if (arrows._Quantity == 0) {
					_Inventory.RemoveAt(index);
				}
			} else {
				OutputController.Display.StoreUserOutput(
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
			int index = _Abilities.FindIndex(
				f => f._Name == inputName.ToString() || f._Name.Contains(input[1]));
			if (index != -1 &&
				_RagePoints >= _Abilities[index]._RageCost &&
				_PlayerClass == PlayerClassType.Warrior) {
				switch (_Abilities[index]._WarAbilityCategory) {
					case PlayerAbility.WarriorAbility.Slash:
					case PlayerAbility.WarriorAbility.Rend:
					case PlayerAbility.WarriorAbility.Charge:
					case PlayerAbility.WarriorAbility.Block:
					case PlayerAbility.WarriorAbility.Berserk:
					case PlayerAbility.WarriorAbility.Disarm:
					case PlayerAbility.WarriorAbility.Onslaught:
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that ability outside combat!");
						return;
					case PlayerAbility.WarriorAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case PlayerAbility.WarriorAbility.PowerAura:
						PlayerAbility.UsePowerAura(this, index);
						return;
					case PlayerAbility.WarriorAbility.WarCry:
						PlayerAbility.UseWarCry(this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
				_ComboPoints >= _Abilities[index]._ComboCost &&
				_PlayerClass == PlayerClassType.Archer) {
				switch (_Abilities[index]._ArcAbilityCategory) {
					case PlayerAbility.ArcherAbility.Distance:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						string direction = input.Last();
						PlayerAbility.UseDistanceAbility(this, index, direction);
						return;
					case PlayerAbility.ArcherAbility.Gut:
					case PlayerAbility.ArcherAbility.Precise:
					case PlayerAbility.ArcherAbility.Stun:
					case PlayerAbility.ArcherAbility.Double:
					case PlayerAbility.ArcherAbility.Wound:
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that ability outside combat!");
						return;
					case PlayerAbility.ArcherAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case PlayerAbility.ArcherAbility.SwiftAura:
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
			int index = _Abilities.FindIndex(
				f => f._Name == inputName.ToString() || f._Name == input[1] || f._Name.Contains(input[1]) ||
					 f._Name.Contains(inputName.ToString()));
			if (index != -1 &&
				_RagePoints >= _Abilities[index]._RageCost &&
				_PlayerClass == PlayerClassType.Warrior) {
				switch (_Abilities[index]._WarAbilityCategory) {
					case PlayerAbility.WarriorAbility.Slash:
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.WarriorAbility.Rend:
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.WarriorAbility.Charge:
						PlayerAbility.UseStunAbility(opponent, this, index);
						return;
					case PlayerAbility.WarriorAbility.Block:
						PlayerAbility.UseDefenseAbility(this, index);
						return;
					case PlayerAbility.WarriorAbility.Berserk:
						PlayerAbility.UseBerserkAbility(this, index);
						return;
					case PlayerAbility.WarriorAbility.Disarm:
						PlayerAbility.UseDisarmAbility(opponent, this, index);
						return;
					case PlayerAbility.WarriorAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case PlayerAbility.WarriorAbility.PowerAura:
						PlayerAbility.UsePowerAura(this, index);
						return;
					case PlayerAbility.WarriorAbility.WarCry:
						PlayerAbility.UseWarCry(this, index);
						return;
					case PlayerAbility.WarriorAbility.Onslaught:
						for (int i = 0; i < 2; i++) {
							if (_RagePoints >= _Abilities[index]._RageCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							} else {
								OutputController.Display.StoreUserOutput(
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
				_ComboPoints >= _Abilities[index]._ComboCost &&
				_PlayerClass == PlayerClassType.Archer) {
				switch (_Abilities[index]._ArcAbilityCategory) {
					case PlayerAbility.ArcherAbility.Distance:
						return;
					case PlayerAbility.ArcherAbility.Gut:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Precise:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Stun:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseStunAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Double:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						for (int i = 0; i < 2; i++) {
							if (_ComboPoints >= _Abilities[index]._ComboCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							} else {
								OutputController.Display.StoreUserOutput(
									Settings.FormatAttackFailText(),
									Settings.FormatDefaultBackground(),
									"You didn't have enough combo points for the second shot!");
							}
						}
						return;
					case PlayerAbility.ArcherAbility.Wound:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case PlayerAbility.ArcherAbility.SwiftAura:
						PlayerAbility.UseSwiftAura(this, index);
						return;
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Ambush:
						if (_PlayerWeapon?._WeaponGroup != Weapon.WeaponType.Bow) {
							throw new InvalidOperationException();
						}

						if (!_InCombat) {
							PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						} else {
							OutputController.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								$"You can't ambush {opponent._Name}, you're already in combat!");
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
			int index = _Spellbook.FindIndex(f => f._Name == inputName);
			if (index != -1 &&
				_ManaPoints >= _Spellbook[index]._ManaCost &&
				_PlayerClass == PlayerClassType.Mage) {
				switch (_Spellbook[index]._SpellCategory) {
					case PlayerSpell.SpellType.Heal:
						PlayerSpell.CastHealing(this, index);
						return;
					case PlayerSpell.SpellType.Rejuvenate:
						PlayerSpell.CastHealing(this, index);
						return;
					case PlayerSpell.SpellType.Diamondskin:
						PlayerSpell.CastAugmentArmor(this, index);
						return;
					case PlayerSpell.SpellType.TownPortal:
						PlayerSpell.CastTownPortal(this, index);
						return;
					case PlayerSpell.SpellType.Reflect:
						PlayerSpell.CastReflectDamage(this, index);
						return;
					case PlayerSpell.SpellType.Fireball:
					case PlayerSpell.SpellType.Frostbolt:
					case PlayerSpell.SpellType.Lightning:
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackFailText(),
							Settings.FormatDefaultBackground(),
							"You cannot use that spell outside combat!");
						return;
					case PlayerSpell.SpellType.ArcaneIntellect:
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
			int index = _Spellbook.FindIndex(f => f._Name == inputName);
			if (index != -1 &&
				_ManaPoints >= _Spellbook[index]._ManaCost &&
				_PlayerClass == PlayerClassType.Mage) {
				switch (_Spellbook[index]._SpellCategory) {
					case PlayerSpell.SpellType.Fireball:
						PlayerSpell.CastFireOffense(opponent, this, index);
						return;
					case PlayerSpell.SpellType.Frostbolt:
						PlayerSpell.CastFrostOffense(opponent, this, index);
						return;
					case PlayerSpell.SpellType.Lightning:
						PlayerSpell.CastArcaneOffense(opponent, this, index);
						return;
					case PlayerSpell.SpellType.Heal:
						PlayerSpell.CastHealing(this, index);
						return;
					case PlayerSpell.SpellType.Rejuvenate:
						PlayerSpell.CastHealing(this, index);
						return;
					case PlayerSpell.SpellType.Diamondskin:
						PlayerSpell.CastAugmentArmor(this, index);
						return;
					case PlayerSpell.SpellType.Reflect:
						PlayerSpell.CastReflectDamage(this, index);
						return;
					case PlayerSpell.SpellType.TownPortal:
						OutputController.Display.StoreUserOutput(
							Settings.FormatAttackSuccessText(),
							Settings.FormatDefaultBackground(),
							"You cannot cast a portal during combat!");
						return;
					case PlayerSpell.SpellType.ArcaneIntellect:
						PlayerSpell.CastArcaneIntellect(this, index);
						return;
					case PlayerSpell.SpellType.FrostNova:
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