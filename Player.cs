using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

	namespace DungeonGame {
	public class Player {
		public enum PlayerClassType {
			Mage,
			Warrior,
			Archer
		}
		public string Name { get; set; }
		public int MaxHitPoints { get; set; }
		public int MaxRagePoints { get; set; }
		public int MaxComboPoints { get; set; }
		public int MaxManaPoints { get; set; }
		public int HitPoints { get; set; }
		public int RagePoints { get; set; }
		public int ComboPoints { get; set; }
		public int ManaPoints { get; set; }
		public int Strength { get; set; }
		public int Intelligence { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int MaxCarryWeight { get; set; }
		public int Gold { get; set; }
		public int Experience { get; set; }
		public int ExperienceToLevel { get; set; }
		public int Level { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public bool InCombat { get; set; }
		public bool IsReflectingDamage { get; set; }
		public bool CanSave { get; set; }
		public bool CanWearCloth { get; set; }
		public bool CanWearLeather { get; set; }
		public bool CanWearPlate { get; set; }
		public bool CanUseDagger { get; set; }
		public bool CanUseOneHandedSword { get; set; }
		public bool CanUseTwoHandedSword { get; set; }
		public bool CanUseAxe { get; set; }
		public bool CanUseBow { get; set; }
		public int AbsorbDamageAmount { get; set; }
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
		public List<Effect> Effects { get; set; }
		public List<Spell> Spellbook { get; set; }
		public List<Ability> Abilities { get; set; }
		public List<Consumable> Consumables { get; set; }
		public List<IEquipment> Inventory { get; set; }

		[JsonConstructor]
		public Player(string name, PlayerClassType playerClass) {
			this.Name = name;
			this.PlayerClass = playerClass;
			this.StatReplenishInterval = 3;
			this.Level = 1;
			this.ExperienceToLevel = 500;
			this.Consumables = new List<Consumable>();
			this.Inventory = new List<IEquipment>();
			this.Effects = new List<Effect>();
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Mana));
					}
					this.Spellbook = new List<Spell>();
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
					this.Spellbook.Add(new Spell(
						"fireball", 35, 1, Spell.SpellType.Fireball, 1));
					this.Spellbook.Add(new Spell(
						"heal", 25, 1, Spell.SpellType.Heal, 1));
					this.Spellbook.Add(new Spell(
						"diamondskin", 25, 1, Spell.SpellType.Diamondskin, 1));
					this.Spellbook.Add(new Spell(
						"frostbolt", 25, 1, Spell.SpellType.Frostbolt, 1));
					this.Spellbook.Add(new Spell(
						"lightning", 25, 1, Spell.SpellType.Lightning, 1));
					this.Spellbook.Add(new Spell(
						"rejuvenate", 25, 1, Spell.SpellType.Rejuvenate, 1));
					break;
				case PlayerClassType.Warrior:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Health));
					}
					this.Abilities = new List<Ability>();
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
					this.Abilities.Add(new Ability(
						"charge", 25, 1, Ability.WarriorAbility.Charge, 1));
					this.Abilities.Add(new Ability(
						"slash", 40, 1, Ability.WarriorAbility.Slash, 1));
					this.Abilities.Add(new Ability(
						"rend", 25, 1, Ability.WarriorAbility.Rend, 1));
					this.Abilities.Add(new Ability(
						"block", 25, 1, Ability.WarriorAbility.Block, 1));
					this.Abilities.Add(new Ability(
						"berserk", 40, 1, Ability.WarriorAbility.Berserk, 1));
					this.Abilities.Add(new Ability(
						"disarm", 25, 1, Ability.WarriorAbility.Disarm, 1));
					break;
				case PlayerClassType.Archer:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(1, Consumable.PotionType.Health));
					}
					this.Abilities = new List<Ability>();
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
					this.Abilities.Add(new Ability("precision shot", 40, 1, 
						Ability.ArcherAbility.Precise, 1));
					this.Abilities.Add(new Ability(
						"gut shot", 25, 1, Ability.ArcherAbility.Gut, 1));
					this.Abilities.Add(new Ability(
						"stun shot", 25, 1, Ability.ArcherAbility.Stun, 1));
					this.Abilities.Add(new Ability("double shot", 25, 1,
						Ability.ArcherAbility.Double, 1));
					this.Abilities.Add(new Ability("wound shot", 40, 1,
						Ability.ArcherAbility.Wound, 1));
					this.Abilities.Add(new Ability("distance shot", 25, 1,
						Ability.ArcherAbility.Distance, 1));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.MaxHitPoints = this.Constitution * 10;
			this.HitPoints = this.MaxHitPoints;
			this.MaxCarryWeight = this.Strength * 2;
			this.DodgeChance = this.Dexterity * 1.5;
		}
		
		public void GainExperience(int experience) {
			this.Experience += experience;
		}
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public int ArmorRating(Monster opponent) {
			var totalArmorRating = GearHandler.CheckArmorRating(this);
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public int Attack(Monster opponent) {
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
					this.PlayerQuiver.OutOfArrows();
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
			GameHandler.RemovedExpiredEffects(this);
			foreach (var effect in this.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangeDamage:
						attackAmount += effect.EffectAmountOverTime;
						effect.ChangeDamageRound(this);
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.AbsorbDamage:
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
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangeDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.AbsorbDamage:
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
					default:
						throw new ArgumentOutOfRangeException();
				}
				GameHandler.RemovedExpiredEffects(this);
			}
			return attackAmount;
		}
		public void DrinkPotion(string[] userInput) {
			var index = 0;
			switch (userInput[1]) {
				case "health":
					index = this.Consumables.FindIndex(
						f => f.PotionCategory.ToString() == "Health" && f.Name.Contains(userInput[1]));
					if (index != -1) {
						this.Consumables[index].RestoreHealth.RestoreHealthPlayer(this);
						var drankHealthString = "You drank a potion and replenished " +
						                  this.Consumables[index].RestoreHealth.RestoreHealthAmt + " health.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							drankHealthString);
						this.Consumables.RemoveAt(index);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have any health potions!");
					}
					break;
				case "mana":
					index = this.Consumables.FindIndex(
						f => f.PotionCategory.ToString() == "Mana" && f.Name.Contains(userInput[1]));
					if (index != -1) {
						this.Consumables[index].RestoreMana.RestoreManaPlayer(this);
						var drankManaString = "You drank a potion and replenished " +
						                      this.Consumables[index].RestoreMana.RestoreManaAmt + " mana.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							drankManaString);
						this.Consumables.RemoveAt(index);
					}
					else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You don't have any mana potions!");
					}
					break;
				default:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"What potion did you want to drink?");
					break;
			}
		}
		public void ReloadQuiver() {
			var index = 0;
			index = this.Consumables.FindIndex(
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
			var index = this.Abilities.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			var direction = input.Last();
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].WarAbilityCategory) {
					case Ability.WarriorAbility.Slash:
						return;
					case Ability.WarriorAbility.Rend:
						return;
					case Ability.WarriorAbility.Charge:
						return;
					case Ability.WarriorAbility.Block:
						return;
					case Ability.WarriorAbility.Berserk:
						return;
					case Ability.WarriorAbility.Disarm:
						return;
					case Ability.WarriorAbility.Bandage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer && 
			    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow) {
				switch (this.Abilities[index].ArcAbilityCategory) {
					case Ability.ArcherAbility.Distance:
						Ability.UseDistanceAbility(this, index, direction);
						return;
					case Ability.ArcherAbility.Gut:
						return;
					case Ability.ArcherAbility.Precise:
						return;
					case Ability.ArcherAbility.Stun:
						return;
					case Ability.ArcherAbility.Double:
						return;
					case Ability.ArcherAbility.Wound:
						return;
					case Ability.ArcherAbility.Bandage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
		public void UseAbility(string inputName) {
			var index = this.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].WarAbilityCategory) {
					case Ability.WarriorAbility.Slash:
						return;
					case Ability.WarriorAbility.Rend:
						return;
					case Ability.WarriorAbility.Charge:
						return;
					case Ability.WarriorAbility.Block:
						return;
					case Ability.WarriorAbility.Berserk:
						return;
					case Ability.WarriorAbility.Disarm:
						return;
					case Ability.WarriorAbility.Bandage:
						Ability.UseBandageAbility(this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
			    this.ComboPoints >= this.Abilities[index].ComboCost &&
			    this.PlayerClass == PlayerClassType.Archer &&
			    this.Abilities[index].ArcAbilityCategory == Ability.ArcherAbility.Bandage) {
				Ability.UseBandageAbility(this, index);
			}
			else if (index != -1) {
				throw new InvalidOperationException();
			}
			else {
				throw new IndexOutOfRangeException();
			}
		}
		public void UseAbility(Monster opponent, string inputName) {
			var index = this.Abilities.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].WarAbilityCategory) {
					case Ability.WarriorAbility.Slash:
						Ability.UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.WarriorAbility.Rend:
						Ability.UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.WarriorAbility.Charge:
						Ability.UseStunAbility(opponent, this, index);
						return;
					case Ability.WarriorAbility.Block:
						this.AbsorbDamageAmount = Ability.UseDefenseAbility(this, index);
						return;
					case Ability.WarriorAbility.Berserk:
						Ability.UseBerserkAbility(this, index);
						return;
					case Ability.WarriorAbility.Disarm:
						Ability.UseDisarmAbility(opponent, this, index);
						return;
					case Ability.WarriorAbility.Bandage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer && 
			    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow) {
				switch (this.Abilities[index].ArcAbilityCategory) {
					case Ability.ArcherAbility.Distance:
						return;
					case Ability.ArcherAbility.Gut:
						Ability.UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.ArcherAbility.Precise:
						Ability.UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.ArcherAbility.Stun:
						Ability.UseStunAbility(opponent, this, index);
						return;
					case Ability.ArcherAbility.Double:
						for (var i = 0; i < 2; i++) {
							if (this.ComboPoints >= this.Abilities[index].ComboCost) {
								Ability.UseOffenseDamageAbility(opponent, this, index);
							}
							else {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatAttackFailText(),
									Settings.FormatDefaultBackground(),
									"You didn't have enough combo points for the second shot!");
							}
						}
						return;
					case Ability.ArcherAbility.Wound:
						Ability.UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.ArcherAbility.Bandage:
						Ability.UseBandageAbility(this, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1) {
				throw new InvalidOperationException();
			}
			else {
				throw new IndexOutOfRangeException();
			}
		}
		public void CastSpell(string inputName) {
			var index = this.Spellbook.FindIndex(f => f.Name == inputName);
			if (index != -1 &&
			    this.ManaPoints >= this.Spellbook[index].ManaCost &&
			    this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Heal:
						Spell.CastHealing(this, index);
						return;
					case Spell.SpellType.Rejuvenate:
						Spell.CastHealing(this, index);
						return;
					case Spell.SpellType.Diamondskin:
						Spell.CastAugmentArmor(this, index);
						return;
					case Spell.SpellType.TownPortal:
						Spell.CastTownPortal(this, index);
						return;
					case Spell.SpellType.Reflect:
						Spell.CastReflectDamage(this, index);
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
					case Spell.SpellType.Fireball:
						Spell.CastFireOffense(opponent, this, index);
						return;
					case Spell.SpellType.Frostbolt:
						Spell.CastFrostOffense(opponent, this, index);
						return;
					case Spell.SpellType.Lightning:
						Spell.CastArcaneOffense(opponent, this, index);
						return;
					case Spell.SpellType.Heal:
						Spell.CastHealing(this, index);
						return;
					case Spell.SpellType.Rejuvenate:
						Spell.CastHealing(this, index);
						return;
					case Spell.SpellType.Diamondskin:
						Spell.CastAugmentArmor(this, index);
						return;
					case Spell.SpellType.Reflect:
						Spell.CastReflectDamage(this, index);
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