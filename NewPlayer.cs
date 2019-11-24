using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class NewPlayer {
		// Attributes for new player
		public string Name { get; }
		public int MaxHitPoints { get; set; } = 100;
		public int MaxManaPoints { get; set; } = 100;
		public int HitPoints { get; set; } = 100;
		public int ManaPoints { get; set; } = 100;
		public int Experience { get; set; } = 0;
		public int Level { get; set; } = 1;
    // Initial items created for player
    private Chest_Armor Player_Chest_Armor = new Chest_Armor();
    private Head_Armor Player_Head_Armor = new Head_Armor();
    private Leg_Armor Player_Leg_Armor = new Leg_Armor();
		private MainWeapon Player_Weapon = new MainWeapon();
		// Initial spells for player
		private Spell Player_Spell = new Spell();
		// Inventory
    public List<object> Inventory { get; set; } = new List<object>();

    // Constructor for new player creation
    public NewPlayer (string name) {
      // Set player name
			this.Name = name;
      // Build inventory for player based on initial items provided
      this.RebuildInventory();
		}

		// Methods for new player
    public void RebuildInventory() {
      Inventory.Add(Player_Chest_Armor);
      Inventory.Add(Player_Head_Armor);
      Inventory.Add(Player_Leg_Armor);
      Inventory.Add(Player_Weapon);
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
    public int ArmorRating() {
      var totalArmorRating =
        Player_Chest_Armor.ArmorRating +
        Player_Head_Armor.ArmorRating +
        Player_Leg_Armor.ArmorRating;
      return totalArmorRating;
    }
		public int Attack() {
			return Player_Weapon.Attack();
		}
		public int CastFireball() {
			return Player_Spell.BlastDamage;
		}
		public int FireballBurnDamage() {
			return Player_Spell.BurnDamage;
		}
	}
}