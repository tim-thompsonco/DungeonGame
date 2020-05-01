using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Player {
		public enum PlayerClassType {
			Mage,
			Warrior,
			Archer
		}
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
		public List<Effect> Effects { get; set; }
		public List<PlayerSpell> Spellbook { get; set; }
		public List<PlayerAbility> Abilities { get; set; }
		public List<Consumable> Consumables { get; set; }
		public List<IEquipment> Inventory { get; set; }
		public List<Quest> QuestLog { get; set; }

		// Default constructor for JSON serialization
		public Player() { }
		public Player(string name, PlayerClassType playerClass) {
			this.Name = name;
			this.PlayerClass = playerClass;
			this.StatReplenishInterval = 3;
			this.Level = 1;
			this.ExperienceToLevel = Settings.GetBaseExperienceToLevel();
			this.Consumables = new List<Consumable>();
			this.Inventory = new List<IEquipment>();
			this.Effects = new List<Effect>();
			this.QuestLog = new List<Quest>();
			this.FireResistance = 5;
			this.FrostResistance = 5;
			this.ArcaneResistance = 5;
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Mana));
					}
					this.Spellbook = new List<PlayerSpell>();
					this.Strength = 10;
					this.Dexterity = 5;
					this.Intelligence = 15;
					this.Constitution = 10;
					this.MaxManaPoints = this.Intelligence * 10;
					this.ManaPoints = this.MaxManaPoints;
					this.CanWearCloth = true;
					this.CanUseDagger = true;
					this.CanUseOneHandedSword = true;
					this.Inventory.Add(new Weapon(this.Level, Weapon.WeaponType.Dagger));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Chest));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Legs));
					this.Spellbook.Add(new PlayerSpell(
						"fireball", 35, 1, PlayerSpell.SpellType.Fireball, 1));
					this.Spellbook.Add(new PlayerSpell(
						"heal", 25, 1, PlayerSpell.SpellType.Heal, 1));
					this.Spellbook.Add(new PlayerSpell(
						"diamondskin", 25, 1, PlayerSpell.SpellType.Diamondskin, 1));
					this.Spellbook.Add(new PlayerSpell(
						"frostbolt", 25, 1, PlayerSpell.SpellType.Frostbolt, 1));
					this.Spellbook.Add(new PlayerSpell(
						"lightning", 25, 1, PlayerSpell.SpellType.Lightning, 1));
					this.Spellbook.Add(new PlayerSpell(
						"rejuvenate", 25, 1, PlayerSpell.SpellType.Rejuvenate, 1));
					break;
				case PlayerClassType.Warrior:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Health));
					}
					this.Abilities = new List<PlayerAbility>();
					this.Strength = 15;
					this.Dexterity = 5;
					this.Intelligence = 5;
					this.Constitution = 15;
					this.MaxRagePoints = this.Strength * 10;
					this.RagePoints = this.MaxRagePoints;
					this.CanWearCloth = true;
					this.CanWearLeather = true;
					this.CanWearPlate = true;
					this.CanUseAxe = true;
					this.CanUseDagger = true;
					this.CanUseBow = true;
					this.CanUseOneHandedSword = true;
					this.CanUseTwoHandedSword = true;
					this.Inventory.Add(new Weapon(this.Level, Weapon.WeaponType.TwoHandedSword));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Head));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Chest));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs));
					this.Abilities.Add(new PlayerAbility(
						"charge", 25, 1, PlayerAbility.WarriorAbility.Charge, 1));
					this.Abilities.Add(new PlayerAbility(
						"slash", 40, 1, PlayerAbility.WarriorAbility.Slash, 1));
					this.Abilities.Add(new PlayerAbility(
						"rend", 25, 1, PlayerAbility.WarriorAbility.Rend, 1));
					this.Abilities.Add(new PlayerAbility(
						"block", 25, 1, PlayerAbility.WarriorAbility.Block, 1));
					this.Abilities.Add(new PlayerAbility(
						"berserk", 40, 1, PlayerAbility.WarriorAbility.Berserk, 1));
					this.Abilities.Add(new PlayerAbility(
						"disarm", 25, 1, PlayerAbility.WarriorAbility.Disarm, 1));
					break;
				case PlayerClassType.Archer:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Health));
					}
					this.Abilities = new List<PlayerAbility>();
					this.Strength = 10;
					this.Dexterity = 15;
					this.Intelligence = 5;
					this.Constitution = 10;
					this.MaxComboPoints = this.Dexterity * 10;
					this.ComboPoints = this.MaxComboPoints;
					this.CanWearCloth = true;
					this.CanWearLeather = true;
					this.CanUseBow = true;
					this.CanUseDagger = true;
					this.CanUseOneHandedSword = true;
					this.Inventory.Add(new Weapon(this.Level, Weapon.WeaponType.Bow));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Head));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest));
					this.Inventory.Add(new Armor(
						1, Armor.ArmorType.Leather, Armor.ArmorSlot.Legs));
					this.Inventory.Add( new Quiver("basic quiver", 50, 50, 15));
					this.Abilities.Add(new PlayerAbility("precise shot", 40, 1, 
						PlayerAbility.ArcherAbility.Precise, 1));
					this.Abilities.Add(new PlayerAbility(
						"gut shot", 25, 1, PlayerAbility.ArcherAbility.Gut, 1));
					this.Abilities.Add(new PlayerAbility(
						"stun shot", 25, 1, PlayerAbility.ArcherAbility.Stun, 1));
					this.Abilities.Add(new PlayerAbility("double shot", 25, 1,
						PlayerAbility.ArcherAbility.Double, 1));
					this.Abilities.Add(new PlayerAbility("wound shot", 40, 1,
						PlayerAbility.ArcherAbility.Wound, 1));
					this.Abilities.Add(new PlayerAbility("distance shot", 25, 1,
						PlayerAbility.ArcherAbility.Distance, 1));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.Inventory.Add(new Armor(Armor.ArmorType.Cloth, Armor.ArmorSlot.Chest, true));
			this.MaxHitPoints = this.Constitution * 10;
			this.HitPoints = this.MaxHitPoints;
			this.MaxCarryWeight = (int)(this.Strength * 2.5);
			this.DodgeChance = this.Dexterity * 1.5;
		}
		
		public int ArmorRating(Monster opponent) {
			var totalArmorRating = GearHandler.CheckArmorRating(this);
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + -(double)levelDiff / 5;
			var adjArmorRating = totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public int PhysicalAttack(Monster opponent) {
			var attackAmount = 0;
			try {
				if (this.PlayerWeapon.Equipped && this.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
					attackAmount = this.PlayerWeapon.Attack();
				}
				if (this.PlayerWeapon.Equipped &&
				    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    this.PlayerQuiver.HaveArrows()) {
					this.PlayerQuiver.UseArrow();
					attackAmount = this.PlayerWeapon.Attack();
				}
				if (this.PlayerWeapon.Equipped &&
				    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    !this.PlayerQuiver.HaveArrows()) {
					Quiver.OutOfArrows();
					attackAmount = 5;
				}
			}
			catch (NullReferenceException) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your weapon is not equipped! Going hand to hand!");
				attackAmount = 5;
			}
			foreach (var effect in this.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						attackAmount += effect.EffectAmountOverTime;
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
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
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
						var frozenAttackAmount = attackAmount * effect.EffectMultiplier;
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
		public void DrinkPotion(string input) {
			var index = this.Consumables.FindIndex(
				f => f.Name.Contains(input));
			if (index == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What potion did you want to drink?");
				return;
			}
			switch (this.Consumables[index].PotionCategory) {
				case Consumable.PotionType.Health:
					this.Consumables[index].RestoreHealth.RestoreHealthPlayer(this);
					var drankHealthString = "You drank a potion and replenished " +
					                        this.Consumables[index].RestoreHealth.RestoreHealthAmt + " health.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						drankHealthString);
					this.Consumables.RemoveAt(index);
					break;
				case Consumable.PotionType.Mana:
					this.Consumables[index].RestoreMana.RestoreManaPlayer(this);
					var drankManaString = "You drank a potion and replenished " +
					                      this.Consumables[index].RestoreMana.RestoreManaAmt + " mana.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						drankManaString);
					this.Consumables.RemoveAt(index);
					break;
				case Consumable.PotionType.Intelligence:
				case Consumable.PotionType.Strength:
				case Consumable.PotionType.Dexterity:
				case Consumable.PotionType.Constitution:
					this.Consumables[index].ChangeStat.ChangeStatPlayer(this);
					var drankStatString = "You drank a potion and increased " + this.Consumables[index].ChangeStat.StatCategory +
					                      " by " + this.Consumables[index].ChangeStat.ChangeAmount + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						drankStatString);
					this.Consumables.RemoveAt(index);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ReloadQuiver() {
			var index = this.Consumables.FindIndex(
				f => f.ArrowCategory == Consumable.ArrowType.Standard && f.Name.Contains("arrow"));
			if (index != -1) {
				this.Consumables[index].Arrow.LoadArrowsPlayer(this);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					"You reloaded your quiver.");
				if (this.Consumables[index].Arrow.Quantity == 0) this.Consumables.RemoveAt(index);
			}
			else {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have any arrows!");
			}
		}
		public void UseAbility(string[] input) {
			var inputName = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
			}
			var index = this.Abilities.FindIndex(
				f => f.Name == inputName.ToString() || f.Name.Contains(input[1]));
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].WarAbilityCategory) {
					case PlayerAbility.WarriorAbility.Slash:
					case PlayerAbility.WarriorAbility.Rend:
					case PlayerAbility.WarriorAbility.Charge:
					case PlayerAbility.WarriorAbility.Block:
					case PlayerAbility.WarriorAbility.Berserk:
					case PlayerAbility.WarriorAbility.Disarm:
					case PlayerAbility.WarriorAbility.Onslaught:
						OutputHandler.Display.StoreUserOutput(
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
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer) {
				switch (this.Abilities[index].ArcAbilityCategory) {
					case PlayerAbility.ArcherAbility.Distance:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						var direction = input.Last();
						PlayerAbility.UseDistanceAbility(this, index, direction);
						return;
					case PlayerAbility.ArcherAbility.Gut:
					case PlayerAbility.ArcherAbility.Precise:
					case PlayerAbility.ArcherAbility.Stun:
					case PlayerAbility.ArcherAbility.Double:
					case PlayerAbility.ArcherAbility.Wound:
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						OutputHandler.Display.StoreUserOutput(
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
			var inputName = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputName.Append(input[i]);
				if (i != input.Length - 1) inputName.Append(" ");
			}
			var index = this.Abilities.FindIndex(
				f => f.Name == inputName.ToString() || f.Name == input[1] || f.Name.Contains(input[1]) || 
				     f.Name.Contains(inputName.ToString()));
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].WarAbilityCategory) {
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
						for (var i = 0; i < 2; i++) {
							if (this.RagePoints >= this.Abilities[index].RageCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							}
							else {
								OutputHandler.Display.StoreUserOutput(
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
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer) {
				switch (this.Abilities[index].ArcAbilityCategory) {
					case PlayerAbility.ArcherAbility.Distance:
						return;
					case PlayerAbility.ArcherAbility.Gut:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Precise:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Stun:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						PlayerAbility.UseStunAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Double:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						for (var i = 0; i < 2; i++) {
							if (this.ComboPoints >= this.Abilities[index].ComboCost) {
								PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
							}
							else {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatAttackFailText(),
									Settings.FormatDefaultBackground(),
									"You didn't have enough combo points for the second shot!");
							}
						}
						return;
					case PlayerAbility.ArcherAbility.Wound:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Bandage:
						PlayerAbility.UseBandageAbility(this, index);
						return;
					case PlayerAbility.ArcherAbility.SwiftAura:
						PlayerAbility.UseSwiftAura(this, index);
						return;
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						return;
					case PlayerAbility.ArcherAbility.Ambush:
						if (this.PlayerWeapon?.WeaponGroup != Weapon.WeaponType.Bow) throw new InvalidOperationException();
						if (!this.InCombat) {
							PlayerAbility.UseOffenseDamageAbility(opponent, this, index);
						}
						else {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatAttackFailText(),
								Settings.FormatDefaultBackground(),
								"You can't ambush " + opponent.Name + ", you're already in combat!");
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
			var index = this.Spellbook.FindIndex(f => f.Name == inputName);
			if (index != -1 &&
			    this.ManaPoints >= this.Spellbook[index].ManaCost &&
			    this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
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
						OutputHandler.Display.StoreUserOutput(
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
			var index = this.Spellbook.FindIndex(f => f.Name == inputName);
			if (index != -1 && 
			    this.ManaPoints >= this.Spellbook[index].ManaCost && 
			    this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
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
						OutputHandler.Display.StoreUserOutput(
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