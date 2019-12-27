using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
		public int MaxRagePoints { get; set; }
		public int MaxManaPoints { get; set; }
		public int HitPoints { get; set; }
		public int RagePoints { get; set; }
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
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void DecreaseArmorDurability() {
			this.PlayerChestArmor?.DecreaseDurability();
			this.PlayerHeadArmor?.DecreaseDurability();
			this.PlayerLegsArmor?.DecreaseDurability();
		}
		public void EquipInitialGear() {
			this.EquipWeapon(this.Inventory[0] as Weapon);
			this.EquipArmor(this.Inventory[1] as Armor);
			this.EquipArmor(this.Inventory[2] as Armor);
			this.EquipArmor(this.Inventory[3] as Armor);
		}
		public void ShowInventory(Player player) {
			Helper.FormatInfoText();
			Console.WriteLine("Your inventory contains:\n");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in this.Inventory) {
				if (!item.IsEquipped()) continue;
				var itemName = this.GetInventoryName(item);
				var itemInfo = new StringBuilder(itemName);
				itemInfo.Append(" <Equipped>");
				Console.WriteLine(itemInfo);
			}
			foreach (var item in this.Inventory) {
				if (item.IsEquipped()) continue;
				var itemName = this.GetInventoryName(item);
				Console.WriteLine(itemName);
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (var item in this.Consumables) {
				var itemInfo = new StringBuilder();
				itemInfo.Append(item.GetName().ToString());
				switch (item.PotionCategory.ToString()) {
					case "Health":
						itemInfo.Append(" (" + item.RestoreHealth.RestoreHealthAmt + ")");
						break;
					case "Mana":
						itemInfo.Append(" (" + item.RestoreMana.RestoreManaAmt + ")");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				if (!consumableDict.ContainsKey(itemName)) {
					consumableDict.Add(itemName, 1);
					continue;
				}
				var dictValue = consumableDict[itemName];
				dictValue += 1;
				consumableDict[itemName] = dictValue;
			}
			foreach (var potion in consumableDict) {
				Console.WriteLine(potion.Key + " (Quantity: {0})", potion.Value);
			}
			Console.WriteLine("Gold: " + this.Gold + " coins.");
		}
		public string GetInventoryName(IEquipment item) {
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var itemInfo = new StringBuilder();
			itemInfo.Append(item.GetName().ToString());
			var isItemArmor = item as Armor;
			if (isItemArmor != null) {
				itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + ")");
			}
			var isItemWeapon = item as Weapon;
			if (isItemWeapon != null) {
				itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + ")");
			}
			var itemName = textInfo.ToTitleCase(itemInfo.ToString());
			return itemName;
		}
		public void LevelUpCheck() {
			if (this.Experience < this.ExperienceToLevel) return;
			this.Level += 1;
			this.Experience -= this.ExperienceToLevel;
			this.ExperienceToLevel *= 2;
			// Increase stats from level
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					this.MaxHitPoints += 15;
					this.MaxManaPoints += 35;
					break;
				case PlayerClassType.Warrior:
					this.MaxHitPoints += 35;
					this.MaxRagePoints += 15;
					break;
				case PlayerClassType.Archer:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			// Leveling sets player back to max stats
			this.HitPoints = this.MaxHitPoints;
			this.RagePoints = this.MaxRagePoints;
			this.ManaPoints = this.MaxManaPoints;
			Helper.FormatLevelUpText();
			Console.WriteLine("You have leveled! You are now level {0}.", this.Level);
		}
		public void DisplayPlayerStats() {
			Helper.FormatGeneralInfoText();
			Console.WriteLine("==================================================");
			Console.Write("Health: {0}/{1} ", this.HitPoints, this.MaxHitPoints);
			switch (this.PlayerClass) {
				case PlayerClassType.Mage:
					Console.Write("Mana: {0}/{1} ", this.ManaPoints, this.MaxManaPoints);
					break;
				case PlayerClassType.Warrior:
					Console.Write("Rage: {0}/{1} ", this.RagePoints, this.MaxRagePoints);
					break;
				case PlayerClassType.Archer:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			Console.WriteLine("EXP: {0} LVL: {1}", this.Experience, this.Level);
			Console.WriteLine("==================================================");
		}
		public void GainExperience(int experience) {
			this.Experience += experience;
		}
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public int CheckArmorRating() {
			var totalArmorRating = 0;
			if (this.PlayerChestArmor != null && this.PlayerChestArmor.IsEquipped()) {
				totalArmorRating += (int)this.PlayerChestArmor.GetArmorRating();
			}
			if (this.PlayerHeadArmor != null && this.PlayerHeadArmor.IsEquipped()) {
				totalArmorRating += (int)this.PlayerHeadArmor.GetArmorRating();
			}
			if (this.PlayerLegsArmor != null && this.PlayerLegsArmor.IsEquipped()) {
				totalArmorRating += (int)this.PlayerLegsArmor.GetArmorRating();
			}
			if (this.IsArmorChanged) {
				totalArmorRating += this.ChangeArmorAmount;
			}
			return totalArmorRating;
		}
		public int ArmorRating(IMonster opponent) {
			var totalArmorRating = this.CheckArmorRating();
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
		public void EquipItem(string[] input) {
			Console.ForegroundColor = ConsoleColor.Green;
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			foreach (var item in this.Inventory) {
				var itemName = item.GetName().Split(' ');
				var itemType = item.GetType().Name;
				var itemFound = (itemName.Last() == inputName || item.GetName() == inputName);
				if (itemFound == true && input[0] == "equip") {
					if (Helper.IsWearable(item) && item.IsEquipped() == false) {
						switch (itemType) {
							case "Weapon":
								this.EquipWeapon((Weapon)item);
								break;
							case "Armor":
								this.EquipArmor((Armor)item);
								break;
						}
						return;
					}
					else if (Helper.IsWearable(item)) {
						Helper.FormatFailureOutputText();
						Console.WriteLine("You have already equipped that.");
						return;
					}
					Helper.FormatFailureOutputText();
					Console.WriteLine("You can't equip that!");
					return;
				}
				else if (itemFound == true && input[0] == "unequip") {
					if (!Helper.IsWearable(item)) return;
					switch (itemType) {
						case "Weapon":
							this.UnequipWeapon((Weapon)item);
							break;
						case "Armor":
							this.UnequipArmor((Armor)item);
							break;
					}
					return;
				}
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("You don't have {0} in your inventory!", inputName);
		}
		public void UnequipWeapon(Weapon weapon) {
			if (weapon.IsEquipped() == false) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already unequipped {0}.", weapon.GetName());
				return;
			}
			weapon.Equipped = false;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have unequipped {0}.", this.PlayerWeapon.GetName());
			this.PlayerWeapon = null;
		}
		public void EquipWeapon(Weapon weapon) {
			if (weapon.IsEquipped() == true) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", weapon.GetName());
				return;
			}
			if (this.PlayerWeapon != null && this.PlayerWeapon.Equipped == true) {
				this.UnequipWeapon(this.PlayerWeapon);
			}
			this.PlayerWeapon = weapon;
			weapon.Equipped = true;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have equipped {0}.", this.PlayerWeapon.GetName());
		}
		public void UnequipArmor(Armor armor) {
			if (armor.IsEquipped() == false) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already unequipped {0}.", armor.GetName());
				return;
			}
			armor.Equipped = false;
			var itemSlot = armor.ArmorCategory.ToString();
			switch (itemSlot) {
				case "Head":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", this.PlayerHeadArmor.GetName());
					this.PlayerHeadArmor = null;
					break;
				case "Chest":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", this.PlayerChestArmor.GetName());
					this.PlayerChestArmor = null;
					break;
				case "Legs":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", this.PlayerLegsArmor.GetName());
					this.PlayerLegsArmor = null;
					break;
				default:
					break;
			}
		}
		public void EquipArmor(Armor armor) {
			if (armor.IsEquipped() == true) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", armor.GetName());
				return;
			}
			var itemSlot = armor.ArmorCategory.ToString();
			var itemType = armor.ArmorGroup.ToString();
			switch (itemType) {
				case "Cloth" when this.CanWearCloth:
					break;
				case "Leather" when this.CanWearLeather:
					break;
				case "Plate" when this.CanWearPlate:
					break;
				default:
					Console.WriteLine("You cannot wear that type of armor!");
					return;
			}
			switch (itemSlot) {
				case "Head":
					if (this.PlayerHeadArmor != null && this.PlayerHeadArmor.Equipped == true) {
						this.UnequipArmor(this.PlayerHeadArmor);
					}
					this.PlayerHeadArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.PlayerHeadArmor.GetName());
					break;
				case "Chest":
					if (this.PlayerChestArmor != null && this.PlayerChestArmor.Equipped == true) {
						this.UnequipArmor(this.PlayerChestArmor);
					}
					this.PlayerChestArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.PlayerChestArmor.GetName());
					break;
				case "Legs":
					if (this.PlayerLegsArmor != null && this.PlayerLegsArmor.Equipped == true) {
						this.UnequipArmor(this.PlayerLegsArmor);
					}
					this.PlayerLegsArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.PlayerLegsArmor.GetName());
					break;
			}
		}
		public void UseAbility(IMonster opponent, string inputName) {
			var index = this.Abilities.FindIndex(f => f.GetName() == inputName);
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
			else if (index != -1) {
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
			else if (index != -1) {
				throw new InvalidOperationException();
			}
			throw new IndexOutOfRangeException();
		}
		public void ListAbilities() {
			if (this.PlayerClass == PlayerClassType.Warrior) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("You have the following abilities:");
				foreach (var ability in this.Abilities) {
					var abilityName = textInfo.ToTitleCase(ability.GetName().ToString());
					Console.WriteLine("{0}, Rank {1}", abilityName, ability.Rank);
				}
			}
			else if (this.PlayerClass != PlayerClassType.Warrior) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a warrior!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't list that.");
			}
		}
		public void ListSpells() {
			if (this.PlayerClass == PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("Your spellbook contains:");
				foreach (var spell in this.Spellbook) {
					var spellName = textInfo.ToTitleCase(spell.GetName().ToString());
					Console.WriteLine("{0}, Rank {1}", spellName, spell.Rank);
				}
			}
			else if (this.PlayerClass != PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't list that.");
			}
		}
		public void AbilityInfo(string input) {
			var index = this.Abilities.FindIndex(f => f.GetName() == input);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && this.PlayerClass == PlayerClassType.Warrior) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(this.Abilities[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", this.Abilities[index].Rank);
				Console.WriteLine("Rage Cost: {0}", this.Abilities[index].RageCost);
				switch(this.Abilities[index].AbilityCategory) {
					case Ability.AbilityType.Slash:
						this.Abilities[index].OffenseDamageAbilityInfo(this, index);
						break;
					case Ability.AbilityType.Rend:
						this.Abilities[index].OffenseDamageAbilityInfo(this, index);
						break;
					case Ability.AbilityType.Charge:
						this.Abilities[index].StunAbilityInfo(this, index);
						break;
					case Ability.AbilityType.Block:
						this.Abilities[index].DefenseAbilityInfo(this, index);
						break;
					case Ability.AbilityType.Berserk:
						this.Abilities[index].BerserkAbilityInfo(this, index);
						break;
					case Ability.AbilityType.Disarm:
						this.Abilities[index].DisarmAbilityInfo(this, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && this.PlayerClass != PlayerClassType.Warrior) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a warrior!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that ability.");
			}
		}
		public void SpellInfo(string input) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == input);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && this.PlayerClass == PlayerClassType.Mage) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(this.Spellbook[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", this.Spellbook[index].Rank);
				Console.WriteLine("Mana Cost: {0}", this.Spellbook[index].ManaCost);
				switch(this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Fireball:
						this.Spellbook[index].FireOffenseSpellInfo(this, index);
						break;
					case Spell.SpellType.Frostbolt:
						this.Spellbook[index].FrostOffenseSpellInfo(this, index);
						break;
					case Spell.SpellType.Lightning:
						this.Spellbook[index].ArcaneOffenseSpellInfo(this, index);
						break;
					case Spell.SpellType.Heal:
						this.Spellbook[index].HealingSpellInfo(this, index);
						break;
					case Spell.SpellType.Rejuvenate:
						this.Spellbook[index].HealingSpellInfo(this, index);
						break;
					case Spell.SpellType.Diamondskin:
						this.Spellbook[index].DefenseSpellInfo(this, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && this.PlayerClass != PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that spell.");
			}
		}
		public void SetHealing(bool isHealing, int healAmount, int healCurRound, int healMaxRound) {
			this.IsHealing = isHealing;
			this.HealAmount = healAmount;
			this.HealCurRound = healCurRound;
			this.HealMaxRound = healMaxRound;
		}
		public void HealingRound() {
			this.HealCurRound += 1;
			Helper.FormatSuccessOutputText();
			this.HitPoints += this.HealAmount;
			if (this.HitPoints > this.MaxHitPoints) this.HitPoints = this.MaxHitPoints;
			Console.WriteLine("You have been healed for {0} health.", this.HealAmount);
			if (this.HealCurRound <= this.HealMaxRound) return;
			this.IsHealing = false;
			this.HealCurRound = 1;
		}
		public void SetChangeArmor(bool isArmorChanged, int changeArmorAmount, int changeArmorCurRound, int changeArmorMaxRound) {
			this.IsArmorChanged = isArmorChanged;
			this.ChangeArmorAmount = changeArmorAmount;
			this.ChangeArmorCurRound = changeArmorCurRound;
			this.ChangeArmorMaxRound = changeArmorMaxRound;
		}
		public void ChangeArmorRound() {
			this.ChangeArmorCurRound += 1;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("Your armor is augmented by {0}.", this.ChangeArmorAmount);
			if (this.ChangeArmorCurRound <= this.ChangeArmorMaxRound) return;
			this.IsArmorChanged = false;
			this.ChangeArmorCurRound = 1;
		}
		public void SetChangeDamage(bool isDamageChanged, int changeDamageAmount, int changeDamageCurRound, int changeDamageMaxRound) {
			this.IsDamageChanged = isDamageChanged;
			this.ChangeDamageAmount = changeDamageAmount;
			this.ChangeDamageCurRound = changeDamageCurRound;
			this.ChangeDamageMaxRound = changeDamageMaxRound;
		}
		public void ChangeDamageRound() {
			this.ChangeDamageCurRound += 1;
			Console.WriteLine(
				this.ChangeDamageAmount > 0 ? "Your damage is increased by {0}." : "Your damage is decreased by {0}",
				this.ChangeDamageAmount);
			if (this.ChangeDamageCurRound <= this.ChangeDamageMaxRound) return;
			this.IsDamageChanged = false;
			this.ChangeDamageCurRound = 1;
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
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}