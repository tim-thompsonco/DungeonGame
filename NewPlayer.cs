using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame {
	public class NewPlayer {
		public string Name { get; }
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
    private Armor Player_Chest_Armor;
    private Armor Player_Head_Armor;
    private Armor Player_Leg_Armor;
		private Weapon Player_Weapon;
		public Spell Player_Spell;
		public Consumable HealthPotion;
		public Consumable ManaPotion;
		public List<IEquipment> Inventory { get; set; } = new List<IEquipment>();

    public NewPlayer (string name) {
      // Set player name
			this.Name = name;
			// Set player initial weapon and armor
			this.Player_Weapon = new Weapon("bronze sword", 25, 25, 1.2, true);
			this.Player_Chest_Armor = new Armor("bronze chestplate", 1, 35, 5, 15, true);
			this.Player_Head_Armor = new Armor("bronze helmet", 0, 12, 1, 5, true);
			this.Player_Leg_Armor = new Armor("bronze legplates", 2, 20, 3, 8, true);
			// Set initial consumables for player
			this.HealthPotion = new Consumable("minor health potion", 3, 0, 50);
			this.ManaPotion = new Consumable("minor mana potion", 3, 1, 50);
			// Build inventory for player based on initial items provided
			this.BuildInventory();
			// Assign player fireball spell
			this.Player_Spell = new Spell("Fireball", 50, 0, 1);
		}

    public void BuildInventory() {
      this.Inventory.Add((DungeonGame.IEquipment)this.Player_Chest_Armor);
      this.Inventory.Add((DungeonGame.IEquipment)this.Player_Head_Armor);
      this.Inventory.Add((DungeonGame.IEquipment)this.Player_Leg_Armor);
      this.Inventory.Add((DungeonGame.IEquipment)this.Player_Weapon);
			this.Inventory.Add((DungeonGame.IEquipment)this.HealthPotion);
			this.Inventory.Add((DungeonGame.IEquipment)this.ManaPotion);
			}
    public void ShowInventory(NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("Your inventory contains:\n");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IEquipment item in this.Inventory) {
				try {
					var itemTitle = item.GetName().ToString();
					if (item.IsEquipped()) {
						itemTitle = textInfo.ToTitleCase(itemTitle) + " <Equipped>";
					}
					else {
						itemTitle = textInfo.ToTitleCase(itemTitle);
					}
					Console.WriteLine(itemTitle);
				}
				catch(NullReferenceException) {
				}
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
			catch(NullReferenceException) {
			}
			try {
				if (this.Player_Head_Armor.IsEquipped()) {
					totalArmorRating += this.Player_Head_Armor.ArmorRating;
				}
			}
			catch(NullReferenceException) {
			}
			try {
				if (this.Player_Leg_Armor.IsEquipped()) {
					totalArmorRating += this.Player_Leg_Armor.ArmorRating;
				}
			}
			catch(NullReferenceException) {
			}
			return totalArmorRating;
		}
    public int ArmorRating(IMonster opponent) {
			var totalArmorRating = CheckArmorRating();
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 10);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
    }
		public int Attack() {
			if(this.Player_Weapon.IsEquipped()) {
				return this.Player_Weapon.Attack();
			}
			else {
				Console.WriteLine("Your weapon is not equipped!");
				return 0;
			}
		}
	}
}