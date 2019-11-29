using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class NewPlayer {
		// Attributes for new player
		public string name { get; }
		public int maxHitPoints { get; set; } = 100;
		public int maxManaPoints { get; set; } = 100;
		public int hitPoints { get; set; } = 100;
		public int manaPoints { get; set; } = 100;
    public int gold { get; set; } = 0;
    public int experience { get; set; } = 0;
		public int level { get; set; } = 1;
    public int x { get; set; } = 0;
		public int y { get; set; } = 0;
		public int z { get; set; } = 0;
    // Initial items created for player
    private Armor _player_Chest_Armor;
    private Armor _player_Head_Armor;
    private Armor _player_Leg_Armor;
		private Weapon _player_Weapon;
		// Initial spells for player
		public Spell _player_Spell;
		// Inventory
    public List<IRoomInteraction> _inventory { get; set; } = new List<IRoomInteraction>();

    // Constructor for new player creation
    public NewPlayer (string name) {
      // Set player name
			this.name = name;
			// Set player initial weapon and armor
			this._player_Weapon = new Weapon("Sword", 25, 1.2);
			this._player_Chest_Armor = new Armor("Chestplate", 5, 15);
			this._player_Head_Armor = new Armor("Helmet", 1, 5);
			this._player_Leg_Armor = new Armor("Legplates", 3, 8);
			// Build inventory for player based on initial items provided
			this.RebuildInventory();
			// Assign player fireball spell
			this._player_Spell = new Spell("Fireball", 30, 5, 1, 3, 1);
		}

		// Methods for new player
    public void RebuildInventory() {
      this._inventory.Add((DungeonGame.IRoomInteraction)this._player_Chest_Armor);
      this._inventory.Add((DungeonGame.IRoomInteraction)this._player_Head_Armor);
      this._inventory.Add((DungeonGame.IRoomInteraction)this._player_Leg_Armor);
      this._inventory.Add((DungeonGame.IRoomInteraction)this._player_Weapon);
    }
    public void ShowInventory(NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("Your inventory contains:\n");
      foreach (IRoomInteraction item in this._inventory) {
        Console.WriteLine(string.Join(", ", item.GetName()));
      }
      Console.WriteLine("Gold: " + this.gold + " coins.");
    }
    public void LevelUpCheck() {
      if (this.experience >= 500) {
        this.level += 1;
        this.experience -= 500;
        // Increase HP and MP from level
        this.maxHitPoints += 20;
        this.maxManaPoints += 20;
        // Leveling sets player back to max HP/MP
        this.hitPoints = this.maxHitPoints;
        this.manaPoints = this.maxManaPoints;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("You have leveled! You are now level {0}.", this.level);
      }
    }
		public void DisplayPlayerStats() {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
			Console.WriteLine("HP: {0}/{1} MP: {2}/{3} EXP: {4} LVL: {5}",
        this.hitPoints, this.maxHitPoints, this.manaPoints, this.maxManaPoints,
        this.experience, this.level);
      Console.WriteLine("==================================================");
    }
		public void GainExperience(int experience) {
			this.experience += experience;
		}
		public void TakeDamage(int weaponDamage) {
			this.hitPoints -= weaponDamage;
		}
    public int ArmorRating() {
      var totalArmorRating =
        this._player_Chest_Armor.armorRating +
        this._player_Head_Armor.armorRating +
        this._player_Leg_Armor.armorRating;
      return totalArmorRating;
    }
		public int Attack() {
			return this._player_Weapon.Attack();
		}
		public int CastFireball() {
			return this._player_Spell.blastDamage;
		}
		public int FireballBurnDamage() {
			return this._player_Spell.burnDamage;
		}
	}
}