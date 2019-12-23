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
		public int MaxHitPoints { get; set; } = 100;
		public int MaxManaPoints { get; set; } = 100;
		public int HitPoints { get; set; } = 100;
		public int ManaPoints { get; set; } = 100;
		public int Gold { get; set; } = 0;
		public int Experience { get; set; } = 0;
		public int ExperienceToLevel { get; set; } = 500;
		public int Level { get; set; } = 1;
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Z { get; set; } = 0;
		public bool CanSave { get; set; }
		public bool CanWearCloth { get; set; }
		public bool CanWearLeather { get; set; }
		public bool CanWearPlate { get; set; }
		public bool IsAugmented { get; set; }
		public int AugmentAmount { get; set; }
		public int AugmentCurRound { get; set; }
		public int AugmentMaxRound { get; set; }
		public PlayerClassType PlayerClass { get; set; }
		public Armor Player_Chest_Armor { get; set; }
		public Armor Player_Head_Armor { get; set; }
		public Armor Player_Legs_Armor { get; set; }
		public Weapon Player_Weapon { get; set; }
		public List<Spell> Spellbook { get; set; }
		public List<Consumable> Consumables { get; set; }
		public List<IEquipment> Inventory { get; set; }

		[JsonConstructor]
		public Player(string name, PlayerClassType playerClass) {
			this.Name = name;
			this.PlayerClass = playerClass;
			this.Spellbook = new List<Spell>();
			this.Consumables = new List<Consumable>();
			this.Inventory = new List<IEquipment>();
			this.Consumables.Add(new Consumable("minor health potion", 3, Consumable.PotionType.Health, 50));
			this.Consumables.Add(new Consumable("minor mana potion", 3, Consumable.PotionType.Mana, 50));
			if (PlayerClass == PlayerClassType.Mage) {
				this.CanWearCloth = true;
				this.Inventory.Add(new Weapon("dagger", 15, 15, 15, 1.2, false));
				this.Inventory.Add(new Armor("cloth vest", Armor.ArmorSlot.Chest, Armor.ArmorType.Cloth, 10, 5, 10, false));
				this.Inventory.Add(new Armor("cloth cap", Armor.ArmorSlot.Head, Armor.ArmorType.Cloth, 3, 1, 3, false));
				this.Inventory.Add(new Armor("cloth leggings", Armor.ArmorSlot.Legs, Armor.ArmorType.Cloth, 7, 3, 7, false));
				this.Spellbook.Add(
					new Spell(
					"fireball", // Name
					25, // Mana cost
					1, // Rank
					Spell.SpellType.FireOffense // Spell type
					));
				this.Spellbook.Add(
					new Spell(
					"heal", // Name
					25, // Mana cost
					1, // Rank
					Spell.SpellType.Healing // Spell type
					));
				this.Spellbook.Add(
					new Spell(
					"diamondskin", // Name
					25, // Mana cost
					1, // Rank
					Spell.SpellType.Defense // Spell type
					));
			}
			this.EquipInitialGear();
		}

		public void DecreaseArmorDurability() {
			if (this.Player_Chest_Armor != null) {
				this.Player_Chest_Armor.DecreaseDurability();
			}
			if (this.Player_Head_Armor != null) {
				this.Player_Head_Armor.DecreaseDurability();
			}
			if (this.Player_Legs_Armor != null) {
				this.Player_Legs_Armor.DecreaseDurability();
			}
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
			foreach (IEquipment item in this.Inventory) {
				if (item.IsEquipped()) {
					string itemName = this.GetInventoryName(item);
					var itemInfo = new StringBuilder(itemName);
					itemInfo.Append(" <Equipped>");
					Console.WriteLine(itemInfo);
				}
			}
			foreach (IEquipment item in this.Inventory) {
				if (!item.IsEquipped()) {
					string itemName = this.GetInventoryName(item);
					Console.WriteLine(itemName);
				}
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (Consumable item in this.Consumables) {
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
						break;
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
			Armor isItemArmor = item as Armor;
			if (isItemArmor != null) {
				itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + ")");
			}
			Weapon isItemWeapon = item as Weapon;
			if (isItemWeapon != null) {
				itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + ")");
			}
			var itemName = textInfo.ToTitleCase(itemInfo.ToString());
			return itemName;
		}
		public void LevelUpCheck() {
			if (this.Experience >= this.ExperienceToLevel) {
				this.Level += 1;
				this.Experience -= this.ExperienceToLevel;
				this.ExperienceToLevel *= 2;
				// Increase HP and MP from level
				this.MaxHitPoints += 20;
				this.MaxManaPoints += 20;
				// Leveling sets player back to max HP/MP
				this.HitPoints = this.MaxHitPoints;
				this.ManaPoints = this.MaxManaPoints;
				Helper.FormatLevelUpText();
				Console.WriteLine("You have leveled! You are now level {0}.", this.Level);
			}
		}
		public void DisplayPlayerStats() {
			Helper.FormatGeneralInfoText();
			Console.WriteLine("==================================================");
			Console.WriteLine("HP: {0}/{1} MP: {2}/{3} EXP: {4} LVL: {5}",
				this.HitPoints, this.MaxHitPoints, this.ManaPoints, this.MaxManaPoints,
				this.Experience, this.Level);
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
			if (this.Player_Chest_Armor != null && this.Player_Chest_Armor.IsEquipped()) {
				totalArmorRating += (int)this.Player_Chest_Armor.GetArmorRating();
			}
			if (this.Player_Head_Armor != null && this.Player_Head_Armor.IsEquipped()) {
				totalArmorRating += (int)this.Player_Head_Armor.GetArmorRating();
			}
			if (this.Player_Legs_Armor != null && this.Player_Legs_Armor.IsEquipped()) {
				totalArmorRating += (int)this.Player_Legs_Armor.GetArmorRating();
			}
			if (IsAugmented) {
				totalArmorRating += AugmentAmount;
			}
			return totalArmorRating;
		}
		public int ArmorRating(IMonster opponent) {
			int totalArmorRating = CheckArmorRating();
			int levelDiff = opponent.Level - this.Level;
			double armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			double adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public int Attack() {
			try {
				if (this.Player_Weapon.IsEquipped()) {
					return this.Player_Weapon.Attack();
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
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			foreach (var item in Inventory) {
				var itemName = item.GetName().Split(' ');
				var itemType = item.GetType().Name;
				var itemFound = false;
				if ((itemName.Last() == inputName || item.GetName() == inputName)) {
					itemFound = true;
				}
				if (itemFound == true && input[0] == "equip") {
					if (Helper.IsWearable(item) && item.IsEquipped() == false) {
						if (itemType == "Weapon") {
							this.EquipWeapon((Weapon)item);
						}
						else if (itemType == "Armor") {
							this.EquipArmor((Armor)item);
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
					if (Helper.IsWearable(item)) {
						if (itemType == "Weapon") {
							this.UnequipWeapon((Weapon)item);
						}
						else if (itemType == "Armor") {
							this.UnequipArmor((Armor)item);
						}
						return;
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
			Console.WriteLine("You have unequipped {0}.", this.Player_Weapon.GetName());
			this.Player_Weapon = null;
		}
		public void EquipWeapon(Weapon weapon) {
			if (weapon.IsEquipped() == true) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", weapon.GetName());
				return;
			}
			if (this.Player_Weapon != null && this.Player_Weapon.Equipped == true) {
				this.UnequipWeapon(this.Player_Weapon);
			}
			this.Player_Weapon = weapon;
			weapon.Equipped = true;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have equipped {0}.", this.Player_Weapon.GetName());
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
					Console.WriteLine("You have unequipped {0}.", this.Player_Head_Armor.GetName());
					this.Player_Head_Armor = null;
					break;
				case "Chest":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", this.Player_Chest_Armor.GetName());
					this.Player_Chest_Armor = null;
					break;
				case "Legs":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", this.Player_Legs_Armor.GetName());
					this.Player_Legs_Armor = null;
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
			switch (itemSlot) {
				case "Head":
					if (this.Player_Head_Armor != null && this.Player_Head_Armor.Equipped == true) {
						this.UnequipArmor(this.Player_Head_Armor);
					}
					this.Player_Head_Armor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.Player_Head_Armor.GetName());
					break;
				case "Chest":
					if (this.Player_Chest_Armor != null && this.Player_Chest_Armor.Equipped == true) {
						this.UnequipArmor(this.Player_Chest_Armor);
					}
					this.Player_Chest_Armor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.Player_Chest_Armor.GetName());
					break;
				case "Legs":
					if (this.Player_Legs_Armor != null && this.Player_Legs_Armor.Equipped == true) {
						this.UnequipArmor(this.Player_Legs_Armor);
					}
					this.Player_Legs_Armor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", this.Player_Legs_Armor.GetName());
					break;
				default:
					break;
			}
		}
		public void CastSpell(string inputName) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && this.ManaPoints >= this.Spellbook[index].ManaCost && this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.Healing:
						this.Spellbook[index].CastHealing(this, index);
						break;
					case Spell.SpellType.Defense:
						this.Spellbook[index].CastDefense(this, index);
						break;
					default:
						break;
				}
			}
			else if (this.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't cast spells. You're not a mage!");
			}
			else if (index != -1) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You do not have enough mana to cast that spell!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that spell in your spellbook.");
			}
		}
		public void CastSpell(IMonster opponent, string inputName) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && this.ManaPoints >= this.Spellbook[index].ManaCost && this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.FireOffense:
						this.Spellbook[index].CastFireOffense(opponent, this, index);
						break;
					case Spell.SpellType.Healing:
						this.Spellbook[index].CastHealing(this, index);
						break;
					case Spell.SpellType.Defense:
						this.Spellbook[index].CastDefense(this, index);
						break;
					default:
						break;
				}
			}
			else if (this.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't cast spells. You're not a mage!");
			}
			else if (index != -1) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You do not have enough mana to cast that spell!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You don't have that spell in your spellbook.");
			}
		}
		public void ListSpells(string input) {
			if (input == "spells" && this.PlayerClass == Player.PlayerClassType.Mage) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				Helper.FormatInfoText();
				Console.WriteLine("Your spellbook contains:");
				foreach (var spell in this.Spellbook) {
					var spellName = textInfo.ToTitleCase(spell.GetName().ToString());
					Console.WriteLine("{0}, Rank {1}", spellName, spell.Rank);
				}
			}
			else if (input == "spells" && this.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You can't list that.");
			}
		}
		public void SpellInfo(string input) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == input);
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && this.PlayerClass == Player.PlayerClassType.Mage) {
				Helper.FormatInfoText();
				Console.WriteLine(textInfo.ToTitleCase(this.Spellbook[index].Name.ToString()));
				Console.WriteLine("Rank: {0}", this.Spellbook[index].Rank);
				Console.WriteLine("Mana Cost: {0}", this.Spellbook[index].ManaCost);
				switch(this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.FireOffense:
						this.Spellbook[index].FireOffenseSpellInfo(this, index);
						break;
					case Spell.SpellType.Healing:
						this.Spellbook[index].HealingSpellInfo(this, index);
						break;
					case Spell.SpellType.Defense:
						this.Spellbook[index].DefenseSpellInfo(this, index);
						break;
					default:
						break;
				}
			}
			else if (index != -1 && this.PlayerClass != Player.PlayerClassType.Mage) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You're not a mage!");
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("That spell is not in your spellbook.");
			}
		}
		public void SetAugmentArmor(bool isAugmented, int augmentAmount, int augmentCurRound, int augmentMaxRound) {
			this.IsAugmented = isAugmented;
			this.AugmentAmount = augmentAmount;
			this.AugmentCurRound = augmentCurRound;
			this.AugmentMaxRound = augmentMaxRound;
		}
		public void AugmentArmorRound() {
			this.AugmentCurRound += 1;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("Your armor is augmented by {0} for {1} more rounds.", this.AugmentAmount, this.AugmentMaxRound - this.AugmentCurRound + 1);
			if (this.AugmentCurRound > this.AugmentMaxRound) {
				this.IsAugmented = false;
				this.AugmentCurRound = 1;
			}
		}
	}
}