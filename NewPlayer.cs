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
    public int location { get; set; }
    // Initial items created for player
    private Armor player_Chest_Armor;
    private Armor player_Head_Armor;
    private Armor player_Leg_Armor;
		private Weapon player_Weapon;
		// Initial spells for player
		private Spell Player_Spell;
		// Inventory
    public List<IRoomInteraction> Inventory { get; set; } = new List<IRoomInteraction>();

    // Constructor for new player creation
    public NewPlayer (string name) {
      // Set player name
			this.name = name;
      this.location = 100100100;
			this.player_Weapon = new Weapon("Sword", 25, 1.2);
			this.player_Chest_Armor = new Armor("Chestplate", 5, 15);
			this.player_Head_Armor = new Armor("Helmet", 1, 5);
			this.player_Leg_Armor = new Armor("Legplates", 3, 8);
			// Build inventory for player based on initial items provided
			this.RebuildInventory();
			this.Player_Spell = new Spell();
		}

		// Methods for new player
    public void RebuildInventory() {
      Inventory.Add((DungeonGame.IRoomInteraction)player_Chest_Armor);
      Inventory.Add((DungeonGame.IRoomInteraction)player_Head_Armor);
      Inventory.Add((DungeonGame.IRoomInteraction)player_Leg_Armor);
      Inventory.Add((DungeonGame.IRoomInteraction)player_Weapon);
    }
    public void ShowInventory(NewPlayer player) {
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine("Your inventory contains:\n");
      foreach (IRoomInteraction item in Inventory) {
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
        player_Chest_Armor.armorRating +
        player_Head_Armor.armorRating +
        player_Leg_Armor.armorRating;
      return totalArmorRating;
    }
		public int Attack() {
			return player_Weapon.Attack();
		}
		public int CastFireball() {
			return Player_Spell.blastDamage;
		}
		public int FireballBurnDamage() {
			return Player_Spell.burnDamage;
		}
	}
}