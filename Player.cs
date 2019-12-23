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
			this.Inventory.Add(new Weapon("iron sword", 19, 25, 25, 1.2, false));
			this.Inventory.Add(new Armor("iron chestplate", Armor.ArmorSlot.Chest, 35, 5, 10, false));
			this.Inventory.Add(new Armor("iron helmet", Armor.ArmorSlot.Head, 12, 1, 3, false));
			this.Inventory.Add(new Armor("iron legplates", Armor.ArmorSlot.Legs, 20, 3, 7, false));
			this.Consumables.Add(new Consumable("minor health potion", 3, Consumable.PotionType.Health, 50));
			this.Consumables.Add(new Consumable("minor mana potion", 3, Consumable.PotionType.Mana, 50));
			this.EquipInitialGear();
			if (PlayerClass == PlayerClassType.Mage) {
				this.Spellbook.Add(
					new Spell(
					"fireball", // Name
					50, // Mana cost
					1, // Rank
					Spell.SpellType.FireOffense // Spell type
					));
			}
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
		public void CastSpell(IMonster opponent, string inputName) {
			var index = this.Spellbook.FindIndex(f => f.GetName() == inputName);
			if (index != -1 && this.ManaPoints >= this.Spellbook[index].ManaCost && this.PlayerClass == PlayerClassType.Mage) {
				switch (this.Spellbook[index].SpellCategory) {
					case Spell.SpellType.FireOffense:
						this.ManaPoints -= this.Spellbook[index].ManaCost;
						var fireSpellDamage = this.Spellbook[index].FireOffense.BlastDamage;
						if (fireSpellDamage == 0) {
							Helper.FormatAttackFailText();
							Console.WriteLine("You missed!");
						}
						else {
							Helper.FormatAttackSuccessText();
							Console.WriteLine("You hit the {0} for {1} fire damage.", opponent.Name, fireSpellDamage);
							opponent.TakeDamage(fireSpellDamage);
							Helper.FormatOnFireText();
							Console.WriteLine("The {0} bursts into flame!", opponent.Name);
							opponent.SetOnFire(
								true, // Is monster on fire
								this.Spellbook[index].FireOffense.BurnDamage, // Burn damage
								this.Spellbook[index].FireOffense.BurnCurRounds, // Burn current round
								this.Spellbook[index].FireOffense.BurnMaxRounds // Burn max round
							);
						}
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
	}
}