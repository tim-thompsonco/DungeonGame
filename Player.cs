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
		public bool CanUseDagger { get; set; }
		public bool CanUseOneHandedSword { get; set; }
		public bool CanUseTwoHandedSword { get; set; }
		public bool CanUseAxe { get; set; }
		public bool CanUseBow { get; set; }
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
		public Quiver PlayerQuiver { get; set; }
		public Armor PlayerHeadArmor { get; set; }
		public Armor PlayerBackArmor { get; set; }
		public Armor PlayerChestArmor { get; set; }
		public Armor PlayerWristArmor { get; set; }
		public Armor PlayerWaistArmor { get; set; }
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
						this.Consumables.Add(new Consumable(
							"minor mana potion", 
							3, 
							Consumable.PotionType.Mana, 
							50));
					}
					this.Spellbook = new List<Spell>();
					this.MaxHitPoints = 100;
					this.HitPoints = this.MaxHitPoints;
					this.MaxManaPoints = 150;
					this.ManaPoints = this.MaxManaPoints;
					this.CanWearCloth = true;
					this.CanUseDagger = true;
					this.CanUseOneHandedSword = true;
					this.Inventory.Add(new Weapon(this.Level, Weapon.WeaponType.Dagger));
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
						this.Consumables.Add(new Consumable(
							"minor health potion", 
							3,
							Consumable.PotionType.Health, 
							50));
					}
					this.Abilities = new List<Ability>();
					this.MaxHitPoints = 150;
					this.HitPoints = this.MaxHitPoints;
					this.MaxRagePoints = 100;
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
					this.Abilities.Add(new Ability("slash", 40, 1, Ability.AbilityType.Slash));
					this.Abilities.Add(new Ability("rend", 25, 1, Ability.AbilityType.Rend));
					this.Abilities.Add(new Ability("block", 25, 1, Ability.AbilityType.Block));
					this.Abilities.Add(new Ability("berserk", 40, 1, Ability.AbilityType.Berserk));
					this.Abilities.Add(new Ability("disarm", 25, 1, Ability.AbilityType.Disarm));
					break;
				case PlayerClassType.Archer:
					for (var i = 0; i < 3; i++) {
						this.Consumables.Add(new Consumable(
							"minor health potion", 
							3, 
							Consumable.PotionType.Health, 
							50));
					}
					this.Abilities = new List<Ability>();
					this.MaxHitPoints = 100;
					this.HitPoints = this.MaxHitPoints;
					this.MaxComboPoints = 150;
					this.ComboPoints = this.MaxComboPoints;
					this.CanWearCloth = true;
					this.CanWearLeather = true;
					this.CanUseBow = true;
					this.CanUseDagger = true;
					this.CanUseOneHandedSword = true;
					this.Inventory.Add(new Weapon(this.Level, Weapon.WeaponType.Bow));
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
					this.Inventory.Add( new Quiver(
						"basic quiver",
						50,
						50,
						15));
					this.Abilities.Add(new Ability(
						"precision shot", 
						40, 
						1, 
						Ability.ShotType.Precise));
					this.Abilities.Add(new Ability(
						"gut shot", 
						25, 
						1,
						Ability.ShotType.Gut));
					this.Abilities.Add(new Ability(
						"stun shot", 
						25, 
						1, 
						Ability.ShotType.Stun));
					this.Abilities.Add(new Ability(
						"double shot", 
						25, 
						1, 
						Ability.ShotType.Double));
					this.Abilities.Add(new Ability(
						"wound shot", 
						40, 
						1, 
						Ability.ShotType.Wound));
					this.Abilities.Add(new Ability(
						"distance shot", 
						25, 
						1, 
						Ability.ShotType.Distance));
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
		public int Attack(UserOutput output) {
			try {
				if (this.PlayerWeapon.IsEquipped() && this.PlayerWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
					return this.PlayerWeapon.Attack();
				}
				if (this.PlayerWeapon.IsEquipped() &&
				    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    this.PlayerQuiver.HaveArrows()) {
					this.PlayerQuiver.UseArrow();
					return this.PlayerWeapon.Attack();
				}
				this.PlayerQuiver.OutOfArrows(output);
			}
			catch (NullReferenceException) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"Your weapon is not equipped! Going hand to hand!");
			}
			return 5;
		}
		public void DrinkPotion(string[] userInput, UserOutput output) {
			var index = 0;
			switch (userInput[1]) {
				case "health":
					index = this.Consumables.FindIndex(
						f => f.PotionCategory.ToString() == "Health" && f.Name.Contains(userInput[1]));
					if (index != -1) {
						this.Consumables[index].RestoreHealth.RestoreHealthPlayer(this);
						var drankHealthString = "You drank a potion and replenished " +
						                  this.Consumables[index].RestoreHealth.RestoreHealthAmt + " health.";
						output.StoreUserOutput(
							Helper.FormatSuccessOutputText(),
							Helper.FormatDefaultBackground(),
							drankHealthString);
						this.Consumables.RemoveAt(index);
					}
					else {
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
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
						output.StoreUserOutput(
							Helper.FormatSuccessOutputText(),
							Helper.FormatDefaultBackground(),
							drankManaString);
						this.Consumables.RemoveAt(index);
					}
					else {
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You don't have any mana potions!");
					}
					break;
				default:
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						"What potion did you want to drink?");
					break;
			}
		}
		public void ReloadQuiver(UserOutput output) {
			var index = 0;
			index = this.Consumables.FindIndex(
				f => f.ArrowCategory == Consumable.ArrowType.Standard && f.Name.Contains("arrow"));
			if (index != -1) {
				this.Consumables[index].Arrow.LoadArrowsPlayer(this, output);
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					"You reloaded your quiver.");
				if (this.Consumables[index].Arrow.Quantity == 0) this.Consumables.RemoveAt(index);
			}
			else {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You don't have any arrows!");
			}
		}
		public void UseAbility(List<IRoom> spawnedRooms, string[] input, UserOutput output) {
			var index = this.Abilities.FindIndex(
				f => f.GetName() == input[1] || f.GetName().Contains(input[1]));
			var direction = input.Last();
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						return;
					case Ability.AbilityType.Rend:
						return;
					case Ability.AbilityType.Charge:
						return;
					case Ability.AbilityType.Block:
						return;
					case Ability.AbilityType.Berserk:
						return;
					case Ability.AbilityType.Disarm:
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer && 
			    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow) {
				switch (this.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						Ability.UseDistanceAbility(spawnedRooms, this, index, direction, output);
						return;
					case Ability.ShotType.Gut:
						return;
					case Ability.ShotType.Precise:
						return;
					case Ability.ShotType.Stun:
						return;
					case Ability.ShotType.Double:
						return;
					case Ability.ShotType.Wound:
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
		public void UseAbility(IMonster opponent, string inputName, UserOutput output) {
			var index = this.Abilities.FindIndex(
				f => f.GetName() == inputName || f.GetName().Contains(inputName));
			if (index != -1 && 
			    this.RagePoints >= this.Abilities[index].RageCost && 
			    this.PlayerClass == PlayerClassType.Warrior) {
				switch (this.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						Ability.UseOffenseDamageAbility(opponent, this, index, output);
						return;
					case Ability.AbilityType.Rend:
						Ability.UseOffenseDamageAbility(opponent, this, index, output);
						return;
					case Ability.AbilityType.Charge:
						Ability.UseStunAbility(opponent, this, index, output);
						return;
					case Ability.AbilityType.Block:
						this.AbsorbDamageAmount = Ability.UseDefenseAbility(this, index);
						return;
					case Ability.AbilityType.Berserk:
						var berserkValues = Ability.UseBerserkAbility(opponent, this, index);
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
						Ability.UseDisarmAbility(opponent, this, index, output);
						return;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (index != -1 &&
			    this.ComboPoints >= this.Abilities[index].ComboCost && 
			    this.PlayerClass == PlayerClassType.Archer && 
			    this.PlayerWeapon.WeaponGroup == Weapon.WeaponType.Bow) {
				switch (this.Abilities[index].ShotCategory) {
					case Ability.ShotType.Distance:
						return;
					case Ability.ShotType.Gut:
						Ability.UseOffenseDamageAbility(opponent, this, index, output);
						return;
					case Ability.ShotType.Precise:
						Ability.UseOffenseDamageAbility(opponent, this, index, output);
						return;
					case Ability.ShotType.Stun:
						Ability.UseStunAbility(opponent, this, index, output);
						return;
					case Ability.ShotType.Double:
						for (var i = 0; i < 2; i++) {
							Ability.UseOffenseDamageAbility(opponent, this, index, output);
						}
						return;
					case Ability.ShotType.Wound:
						Ability.UseOffenseDamageAbility(opponent, this, index, output);
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
		public void CastSpell(string inputName, UserOutput output) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 &&
			    this.ManaPoints >= this.Spellbook[index].ManaCost &&
			    this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Heal:
						Spell.CastHealing(this, index, output);
						return;
					case Spell.SpellType.Rejuvenate:
						Spell.CastHealing(this, index, output);
						return;
					case Spell.SpellType.Diamondskin:
						Spell.CastDefense(this, index, output);
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
		public void CastSpell(IMonster opponent, string inputName, UserOutput output) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && 
			    this.ManaPoints >= this.Spellbook[index].ManaCost && 
			    this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						Spell.CastFireOffense(opponent, this, index, output);
						return;
					case Spell.SpellType.Frostbolt:
						Spell.CastFrostOffense(opponent, this, index, output);
						return;
					case Spell.SpellType.Lightning:
						Spell.CastArcaneOffense(opponent, this, index, output);
						return;
					case Spell.SpellType.Heal:
						Spell.CastHealing(this, index, output);
						return;
					case Spell.SpellType.Rejuvenate:
						Spell.CastHealing(this, index, output);
						return;
					case Spell.SpellType.Diamondskin:
						Spell.CastDefense(this, index, output);
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
		public void SetChangeArmor(
			bool isArmorChanged,
			int changeArmorAmount,
			int changeArmorCurRound, 
			int changeArmorMaxRound) {
			this.IsArmorChanged = isArmorChanged;
			this.ChangeArmorAmount = changeArmorAmount;
			this.ChangeArmorCurRound = changeArmorCurRound;
			this.ChangeArmorMaxRound = changeArmorMaxRound;
		}
		public void SetChangeDamage(
			bool isDamageChanged, 
			int changeDamageAmount,
			int changeDamageCurRound,
			int changeDamageMaxRound) {
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