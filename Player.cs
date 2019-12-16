using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class Player {
		public string Name { get; set; }
		public int MaxHitPoints { get; set; } = 100;
		public int MaxManaPoints { get; set; } = 100;
		public int HitPoints { get; set; } = 100;
		public int ManaPoints { get; set; } = 100;
    public int Gold { get; set; } = 0;
    public int Experience { get; set; } = 0;
		public int Level { get; set; } = 1;
    public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Z { get; set; } = 0;
    private Armor Player_Chest_Armor { get; set; }
    private Armor Player_Head_Armor { get; set; }
    private Armor Player_Legs_Armor { get; set; }
		private Weapon Player_Weapon { get; set; }
		public Spell Player_Spell { get; set; }
		public List<Consumable> Consumables { get; set; }
		public List<IEquipment> Inventory { get; set; }

		[JsonConstructor]
    public Player (string name) {
			this.Name = name;
			this.Consumables = new List<Consumable>();
			this.Inventory = new List<IEquipment>();
			this.Player_Weapon = new Weapon("bronze sword", 22, 28, 25, 1.2, true);
			this.Player_Chest_Armor = new Armor("bronze chestplate", Armor.ArmorSlot.Chest, 35, 10, 20, true);
			this.Player_Head_Armor = new Armor("bronze helmet", Armor.ArmorSlot.Head, 12, 3, 7, true);
			this.Player_Legs_Armor = new Armor("bronze legplates", Armor.ArmorSlot.Legs, 20, 5, 10, true);
			this.Consumables.Add(new Consumable("minor health potion", 3, Consumable.PotionType.Health, 50));
			this.Consumables.Add(new Consumable("minor mana potion", 3, Consumable.PotionType.Mana, 50));
			this.Player_Spell = new Spell("Fireball", 50, 0, 1);
			this.BuildInventory();
		}
    public void BuildInventory() {
			this.Inventory.Add(this.Player_Chest_Armor);
			this.Inventory.Add(this.Player_Head_Armor);
			this.Inventory.Add(this.Player_Legs_Armor);
			this.Inventory.Add(this.Player_Weapon);
		}
    public void ShowInventory(Player player) {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("Your inventory contains:\n");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IEquipment item in this.Inventory) {
				var itemInfo = new StringBuilder();
				try {
					itemInfo.Append(item.GetName().ToString());
					if (item.IsEquipped()) {
						itemInfo.Append(" <Equipped>");
					}
				}
				catch(NullReferenceException) {}
				var isItemArmor = item as Armor;
				if (isItemArmor != null) {
					itemInfo.Append(" (AR: " + isItemArmor.ArmorRating + ")");
				}
				var isItemWeapon = item as Weapon;
				if (isItemWeapon != null) {
					itemInfo.Append(" (DMG: " + isItemWeapon.RegDamage + " CR: " + isItemWeapon.CritMultiplier + ")");
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				Console.WriteLine(itemName);
			}
			foreach (Consumable item in this.Consumables) {
				var itemInfo = new StringBuilder();
				try {
					itemInfo.Append(item.GetName().ToString());
				}
				catch (NullReferenceException) { }
				switch(item.PotionCategory.ToString()) {
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
				Console.WriteLine(itemName);
			}
			Console.WriteLine("Gold: " + this.Gold + " coins.");
    }
    public void LevelUpCheck() {
      if (this.Experience >= 500) {
        this.Level += 1;
        this.Experience -= 500;
        // Increase HP and MP from level
        this.MaxHitPoints += 20;
        this.MaxManaPoints += 20;
        // Leveling sets player back to max HP/MP
        this.HitPoints = this.MaxHitPoints;
        this.ManaPoints = this.MaxManaPoints;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("You have leveled! You are now level {0}.", this.Level);
      }
    }
		public void DisplayPlayerStats() {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
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
			try {
				if (this.Player_Chest_Armor.IsEquipped()) {
					totalArmorRating += this.Player_Chest_Armor.ArmorRating;
				}
			}
			catch(NullReferenceException) {}
			try {
				if (this.Player_Head_Armor.IsEquipped()) {
					totalArmorRating += this.Player_Head_Armor.ArmorRating;
				}
			}
			catch(NullReferenceException) {}
			try {
				if (this.Player_Legs_Armor.IsEquipped()) {
					totalArmorRating += this.Player_Legs_Armor.ArmorRating;
				}
			}
			catch(NullReferenceException) {}
			return totalArmorRating;
		}
    public int ArmorRating(IMonster opponent) {
			var totalArmorRating = CheckArmorRating();
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 20);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
    }
		public int Attack() {
			try {
				if (this.Player_Weapon.IsEquipped()) {
					return this.Player_Weapon.Attack();
				}
			}
			catch(NullReferenceException) {
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
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("You drank a potion and replenished {0} health.", this.Consumables[index].RestoreHealth.RestoreHealthAmt);
						this.Consumables.RemoveAt(index);
					}
					else {
						Console.WriteLine("You don't have any health potions!");
					}
					break;
				case "mana":
					index = this.Consumables.FindIndex(f => f.PotionCategory.ToString() == "Mana");
					if (index != -1) {
						this.Consumables[index].RestoreMana.RestoreManaPlayer(this);
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("You drank a potion and replenished {0} mana.", this.Consumables[index].RestoreMana.RestoreManaAmt);
						this.Consumables.RemoveAt(index);
					}
					else {
						Console.WriteLine("You don't have any mana potions!");
					}
					break;
				default:
					Console.WriteLine("What potion did you want to drink?");
					break;
			}
		}
		public void EquipItem(string[] input) {
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
						Console.WriteLine("You have already equipped that.");
						return;
					}
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
			Console.WriteLine("You don't have {0} in your inventory!", inputName);
		}
		public void UnequipWeapon(Weapon weapon) {
			if (weapon.IsEquipped() == false) {
				Console.WriteLine("You have already unequipped {0}.", weapon.GetName());
				return;
			}
			weapon.Equipped = false;
			Console.WriteLine("You have unequipped {0}.", this.Player_Weapon.GetName());
			this.Player_Weapon = null;
		}
		public void EquipWeapon(Weapon weapon) {
			if (weapon.IsEquipped() == true) {
				Console.WriteLine("You have already equipped {0}.", weapon.GetName());
				return;
			}
			try {
				if (this.Player_Weapon.Equipped == true) {
					this.Player_Weapon.Equipped = false;
					Console.WriteLine("You have unequipped {0}.", this.Player_Weapon.GetName());
				}
			}
			catch(NullReferenceException) {}
			weapon.Equipped = true;
			this.Player_Weapon = weapon;
			Console.WriteLine("You have equipped {0}.", this.Player_Weapon.GetName());
		}
		public void UnequipArmor(Armor armor) {
			if (armor.IsEquipped() == false) {
				Console.WriteLine("You have already unequipped {0}.", armor.GetName());
				return;
			}
			armor.Equipped = false;
			var itemSlot = armor.ArmorCategory.ToString();
			switch (itemSlot) {
				case "Head":
					this.Player_Head_Armor.Equipped = false;
					Console.WriteLine("You have unequipped {0}.", this.Player_Head_Armor.GetName());
					this.Player_Head_Armor = null;
					break;
				case "Chest":
					this.Player_Chest_Armor.Equipped = false;
					Console.WriteLine("You have unequipped {0}.", this.Player_Chest_Armor.GetName());
					this.Player_Chest_Armor = null;
					break;
				case "Legs":
					this.Player_Legs_Armor.Equipped = false;
					Console.WriteLine("You have unequipped {0}.", this.Player_Legs_Armor.GetName());
					this.Player_Legs_Armor = null;
					break;
				default:
					break;
			}
		}
		public void EquipArmor(Armor armor) {
			if (armor.IsEquipped() == true) {
				Console.WriteLine("You have already equipped {0}.", armor.GetName());
				return;
			}
			armor.Equipped = true;
			var itemSlot = armor.ArmorCategory.ToString();
			switch(itemSlot) {
				case "Head":
					try {
						if (this.Player_Head_Armor.Equipped == true) {
							this.Player_Head_Armor.Equipped = false;
							Console.WriteLine("You have unequipped {0}.", this.Player_Head_Armor.GetName());
						}
					}
					catch(NullReferenceException) {}
					this.Player_Head_Armor = armor;
					Console.WriteLine("You have equipped {0}.", this.Player_Head_Armor.GetName());
					break;
				case "Chest":
					try {
						if (this.Player_Chest_Armor.Equipped == true) {
							this.Player_Chest_Armor.Equipped = false;
							Console.WriteLine("You have unequipped {0}.", this.Player_Chest_Armor.GetName());
						}
					}
					catch (NullReferenceException) {}
					this.Player_Chest_Armor = armor;
					Console.WriteLine("You have equipped {0}.", this.Player_Chest_Armor.GetName());
					break;
				case "Legs":
					try {
						if (this.Player_Legs_Armor.Equipped == true) {
							this.Player_Legs_Armor.Equipped = false;
							Console.WriteLine("You have unequipped {0}.", this.Player_Legs_Armor.GetName());
						}
					}
					catch (NullReferenceException) {}
					this.Player_Legs_Armor = armor;
					Console.WriteLine("You have equipped {0}.", this.Player_Legs_Armor.GetName());
					break;
				default:
					break;
			}
		}
	}
}