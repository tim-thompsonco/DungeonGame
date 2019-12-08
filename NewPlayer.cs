using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame {
	public class NewPlayer {
		// Attributes for new player
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
    // Initial items created for player
    private Armor Player_Chest_Armor;
    private Armor Player_Head_Armor;
    private Armor Player_Leg_Armor;
		private Weapon Player_Weapon;
		// Initial spells for player
		public Spell Player_Spell;
		// Initial consumables for player
		public Consumable HealthPotion;
		// Inventory
		public List<IRoomInteraction> Inventory { get; set; } = new List<IRoomInteraction>();

    // Constructor for new player creation
    public NewPlayer (string name) {
      // Set player name
			this.Name = name;
			// Set player initial weapon and armor
			this.Player_Weapon = new Weapon("bronze sword", 25, 25, 1.2);
			this.Player_Chest_Armor = new Armor("bronze chestplate", 35, 5, 15);
			this.Player_Head_Armor = new Armor("bronze helmet", 12, 1, 5);
			this.Player_Leg_Armor = new Armor("bronze legplates", 20, 3, 8);
			// Set initial consumables for player
			this.HealthPotion = new Consumable("minor health potion", 3, 0, 50);
			// Build inventory for player based on initial items provided
			this.BuildInventory();
			// Assign player fireball spell
			this.Player_Spell = new Spell("Fireball", 50, 0, 1);
		}

		// Methods for new player
    public void BuildInventory() {
      this.Inventory.Add((DungeonGame.IRoomInteraction)this.Player_Chest_Armor);
      this.Inventory.Add((DungeonGame.IRoomInteraction)this.Player_Head_Armor);
      this.Inventory.Add((DungeonGame.IRoomInteraction)this.Player_Leg_Armor);
      this.Inventory.Add((DungeonGame.IRoomInteraction)this.Player_Weapon);
			if(this.HealthPotion.Quantity >= 1) {
				this.Inventory.Add((DungeonGame.IRoomInteraction)this.HealthPotion);
			}
		}
    public void ShowInventory(NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("Your inventory contains:\n");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (IRoomInteraction item in this.Inventory) {
				var itemTitle = item.GetName().ToString();
				itemTitle = textInfo.ToTitleCase(itemTitle);
				Console.WriteLine(itemTitle);
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
    public int ArmorRating(IMonster opponent) {
      var totalArmorRating =
        this.Player_Chest_Armor.ArmorRating +
        this.Player_Head_Armor.ArmorRating +
        this.Player_Leg_Armor.ArmorRating;
			var levelDiff = opponent.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 10);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
    }
		public int Attack() {
			return this.Player_Weapon.Attack();
		}
	}
}