using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

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
		public int Gold { get; set; }
		public int Experience { get; set; }
		public int ExperienceToLevel { get; set; }
		public int Level { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public bool CanSave { get; set; }
		public bool CanWearCloth { get; set; }
		public bool CanWearLeather { get; set; }
		public bool CanWearPlate { get; set; }
		public bool IsHealing { get; set; }
		public int HealAmount { get; set; }
		public int HealCurRound { get; set; }
		public int HealMaxRound { get; set; }
		public bool IsArmorChanged { get; set; }
		public int AbsorbDamageAmount { get; set; }
		public bool IsDamageChanged { get; set; }
		public int ChangeDamageAmount { get; set; }
		public int ChangeDamageCurRound { get; set; }
		public int ChangeDamageMaxRound { get; set; }
		public int ChangeArmorAmount { get; set; }
		public int ChangeArmorCurRound { get; set; }
		public int ChangeArmorMaxRound { get; set; }
		public PlayerClassType PlayerClass { get; set; }
		public Armor PlayerHeadArmor { get; set; }
		public Armor PlayerChestArmor { get; set; }
		public Armor PlayerLegsArmor { get; set; }
		public Weapon PlayerWeapon { get; set; }
		public List<Spell> Spellbook { get; set; }
		public List<Ability> Abilities { get; set; }
		public List<Consumable> Consumables { get; set; }
		public List<IEquipment> Inventory { get; set; }

		[JsonConstructor]
		public Player(string name, PlayerClassType playerClass) {
			this.Name = name;
			this.PlayerClass = playerClass;
			this.Level = 1;
			this.ExperienceToLevel = 500;
			this.Consumables = new List<Consumable>();
			this.Inventory = new List<IEquipment>();
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable("minor mana potion", 3, Consumable.PotionType.Mana, 50));
					}
					this.Spellbook = new List<Spell>();
					this.MaxHitPoints = 100;
					this.HitPoints = this.MaxHitPoints;
					this.MaxManaPoints = 150;
					this.ManaPoints = this.MaxManaPoints;
					this.CanWearCloth = true;
					this.Inventory.Add(new Weapon(
						"dagger", 
						14, 
						17, 
						18, 
						1.2, 
						false));
					this.Inventory.Add(new Armor(
						"cloth vest", Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Cloth, 
						10, 
						7, 
						10, 
						false));
					this.Inventory.Add(new Armor(
						"cloth cap", 
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Cloth, 
						3, 
						1, 
						4, 
						false));
					this.Inventory.Add(new Armor(
						"cloth leggings", 
						Armor.ArmorSlot.Legs, 
						Armor.ArmorType.Cloth, 
						7, 
						4, 
						7, 
						false));
					this.Spellbook.Add(new Spell("fireball", 35, 1, Spell.SpellType.Fireball));
					this.Spellbook.Add(new Spell("heal", 25, 1, Spell.SpellType.Heal));
					this.Spellbook.Add(new Spell("diamondskin", 25, 1, Spell.SpellType.Diamondskin));
					this.Spellbook.Add(new Spell("frostbolt", 25, 1, Spell.SpellType.Frostbolt));
					this.Spellbook.Add(new Spell("lightning", 25, 1, Spell.SpellType.Lightning));
					this.Spellbook.Add(new Spell("rejuvenate", 25, 1, Spell.SpellType.Rejuvenate));
					break;
				case PlayerClassType.Warrior:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable("minor health potion", 3, Consumable.PotionType.Health, 50));
					}
					this.Abilities = new List<Ability>();
					this.MaxHitPoints = 150;
					this.HitPoints = this.MaxHitPoints;
					this.MaxRagePoints = 100;
					this.RagePoints = this.MaxRagePoints;
					this.CanWearCloth = true;
					this.CanWearLeather = true;
					this.CanWearPlate = true;
					this.Inventory.Add(new Weapon(
						"iron sword", 
						24, 
						27, 
						27, 
						1.3, 
						false));
					this.Inventory.Add(new Armor(
						"iron chestplate", Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Plate, 
						11, 
						8, 
						11, 
						false));
					this.Inventory.Add(new Armor(
						"iron helmet", 
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Plate, 
						6, 
						3, 
						6, 
						false));
					this.Inventory.Add(new Armor(
						"iron legplates", 
						Armor.ArmorSlot.Legs, 
						Armor.ArmorType.Plate, 
						8, 
						5, 
						8, 
						false));
					this.Abilities.Add(new Ability("charge", 25, 1, Ability.AbilityType.Charge));
					this.Abilities.Add(new Ability("slash", 25, 1, Ability.AbilityType.Slash));
					this.Abilities.Add(new Ability("rend", 25, 1, Ability.AbilityType.Rend));
					this.Abilities.Add(new Ability("block", 25, 1, Ability.AbilityType.Block));
					this.Abilities.Add(new Ability("berserk", 50, 1, Ability.AbilityType.Berserk));
					this.Abilities.Add(new Ability("disarm", 25, 1, Ability.AbilityType.Disarm));
					break;
				case PlayerClassType.Archer:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable("minor health potion", 3, Consumable.PotionType.Health, 50));
					}
					this.Abilities = new List<Ability>();
					this.MaxHitPoints = 100;
					this.HitPoints = this.MaxHitPoints;
					this.MaxComboPoints = 150;
					this.ComboPoints = this.MaxComboPoints;
					this.CanWearCloth = true;
					this.CanWearLeather = true;
					this.Inventory.Add(new Weapon(
						"short bow", 
						20, 
						23, 
						23, 
						1.3, 
						false));
					this.Inventory.Add(new Armor(
						"leather vest", Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Leather, 
						8, 
						5, 
						8, 
						false));
					this.Inventory.Add(new Armor(
						"leather cap", 
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Leather, 
						4, 
						2, 
						4, 
						false));
					this.Inventory.Add(new Armor(
						"leather leggings", 
						Armor.ArmorSlot.Legs, 
						Armor.ArmorType.Leather, 
						5, 
						2, 
						5, 
						false));
					this.Abilities.Add(new Ability("precision shot", 25, 1, Ability.ShotType.Precise));
					this.Abilities.Add(new Ability("gut shot", 25, 1, Ability.ShotType.Gut));
					this.Abilities.Add(new Ability("stun shot", 25, 1, Ability.ShotType.Stun));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public void GainExperience(int experience) {
			this.Experience += experience;
		}
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public int ArmorRating(IMonster opponent) {
			var totalArmorRating = GearHelper.CheckArmorRating(this);
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public int Attack() {
			try {
				if (this.PlayerWeapon.IsEquipped()) {
					return this.PlayerWeapon.Attack();
				}
			}
			catch (NullReferenceException) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("Your weapon is not equipped! Going hand to hand!");
			}
			return 5;
		}
		public void DrinkPotion(string[] userInput) {
			var index = 0;
			switch (userInput[1]) {
				case "health":
					index = this.Consumables.FindIndex(f => f.PotionCategory.ToString() == "Health");
					if (index != -1) {
						this.Consumables[index].RestoreHealth.RestoreHealthPlayer(this);
						Helper.FormatSuccessOutputText();
						Console.WriteLine("You drank a potion and replenished {0} health.", this.Consumables[index].RestoreHealth.RestoreHealthAmt);
						this.Consumables.RemoveAt(index);
					}
					else {
						Helper.FormatFailureOutputText();
						Console.WriteLine("You don't have any health potions!");
					}
					break;
				case "mana":
					index = this.Consumables.FindIndex(f => f.PotionCategory.ToString() == "Mana");
					if (index != -1) {
						this.Consumables[index].RestoreMana.RestoreManaPlayer(this);
						Helper.FormatSuccessOutputText();
						Console.WriteLine("You drank a potion and replenished {0} mana.", this.Consumables[index].RestoreMana.RestoreManaAmt);
						this.Consumables.RemoveAt(index);
					}
					else {
						Helper.FormatFailureOutputText();
						Console.WriteLine("You don't have any mana potions!");
					}
					break;
				default:
					Helper.FormatFailureOutputText();
					Console.WriteLine("What potion did you want to drink?");
					break;
			}
		}
		public void UseAbility(IMonster opponent, string inputName) {
			var index = this.Abilities.FindIndex(f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (index != -1 && this.RagePoints >= this.Abilities[index].RageCost && this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						this.Abilities[index].UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.AbilityType.Rend:
						this.Abilities[index].UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.AbilityType.Charge:
						this.Abilities[index].UseStunAbility(opponent, this, index);
						return;
					case Ability.AbilityType.Block:
						this.AbsorbDamageAmount = this.Abilities[index].UseDefenseAbility(this, index);
						return;
					case Ability.AbilityType.Berserk:
						var berserkValues = this.Abilities[index].UseBerserkAbility(opponent, this, index);
						this.SetChangeArmor(
							true, 
							berserkValues[1], 
							berserkValues[2],
							berserkValues[3]);
						this.SetChangeDamage(
							true,
							berserkValues[0],
							berserkValues[2],
							berserkValues[3]);
						return;
					case Ability.AbilityType.Disarm:
						this.Abilities[index].UseDisarmAbility(opponent, this, index);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 && this.ComboPoints >= this.Abilities[index].ComboCost && this.PlayerClass == PlayerClassType.Archer) {
				switch (this.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						return;
					case Ability.ShotType.Gut:
						this.Abilities[index].UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.ShotType.Precise:
						this.Abilities[index].UseOffenseDamageAbility(opponent, this, index);
						return;
					case Ability.ShotType.Stun:
						this.Abilities[index].UseStunAbility(opponent, this, index);
						return;
					case Ability.ShotType.Double:
						return;
					case Ability.ShotType.Poison:
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
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && this.ManaPoints >= this.Spellbook[index].ManaCost && this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Heal:
						this.Spellbook[index].CastHealing(this, index);
						return;
					case Spell.SpellType.Rejuvenate:
						this.Spellbook[index].CastHealing(this, index);
						return;
					case Spell.SpellType.Diamondskin:
						this.Spellbook[index].CastDefense(this, index);
						return;
					case Spell.SpellType.Fireball:
						return;
					case Spell.SpellType.Frostbolt:
						return;
					case Spell.SpellType.Lightning:
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
		public void CastSpell(IMonster opponent, string inputName) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && this.ManaPoints >= this.Spellbook[index].ManaCost && this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						this.Spellbook[index].CastFireOffense(opponent, this, index);
						return;
					case Spell.SpellType.Frostbolt:
						this.Spellbook[index].CastFrostOffense(opponent, this, index);
						return;
					case Spell.SpellType.Lightning:
						this.Spellbook[index].CastArcaneOffense(opponent, this, index);
						return;
					case Spell.SpellType.Heal:
						this.Spellbook[index].CastHealing(this, index);
						return;
					case Spell.SpellType.Rejuvenate:
						this.Spellbook[index].CastHealing(this, index);
						return;
					case Spell.SpellType.Diamondskin:
						this.Spellbook[index].CastDefense(this, index);
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
		public void SetHealing(bool isHealing, int healAmount, int healCurRound, int healMaxRound) {
			this.IsHealing = isHealing;
			this.HealAmount = healAmount;
			this.HealCurRound = healCurRound;
			this.HealMaxRound = healMaxRound;
		}
		public void SetChangeArmor(bool isArmorChanged, int changeArmorAmount, int changeArmorCurRound, int changeArmorMaxRound) {
			this.IsArmorChanged = isArmorChanged;
			this.ChangeArmorAmount = changeArmorAmount;
			this.ChangeArmorCurRound = changeArmorCurRound;
			this.ChangeArmorMaxRound = changeArmorMaxRound;
		}
		public void SetChangeDamage(bool isDamageChanged, int changeDamageAmount, int changeDamageCurRound, int changeDamageMaxRound) {
			this.IsDamageChanged = isDamageChanged;
			this.ChangeDamageAmount = changeDamageAmount;
			this.ChangeDamageCurRound = changeDamageCurRound;
			this.ChangeDamageMaxRound = changeDamageMaxRound;
		}
		public void ReplenishStatsOverTime() {
			if (this.HitPoints == this.MaxHitPoints) return;
			this.HitPoints += 1;
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					if (this.ManaPoints == this.MaxManaPoints) return;
					this.ManaPoints += 1;
					break;
				case PlayerClassType.Warrior:
					if (this.RagePoints == this.MaxRagePoints) return;
					this.RagePoints += 1;
					break;
				case PlayerClassType.Archer:
					if (this.ComboPoints == this.MaxComboPoints) return;
					this.ComboPoints += 1;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}