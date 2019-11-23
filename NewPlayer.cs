using System;
using System.Collections.Generic;

namespace DungeonGame {
	class NewPlayer {
		// Attributes for new player
		private readonly string Name;
		private int MaxHitPoints { get; set; } = 100;
		private int MaxManaPoints { get; set; } = 100;
		private int HitPoints { get; set; } = 100;
		private int ManaPoints { get; set; } = 100;
		private int Experience { get; set; } = 0;
		private int Level { get; set; } = 1;
    // Initial items created for player
    private Chest_Armor Player_Chest_Armor { get; set; } = new Chest_Armor();
    private Head_Armor Player_Head_Armor { get; set; } = new Head_Armor();
    private Leg_Armor Player_Leg_Armor { get; set; } = new Leg_Armor();
		private AxeWeapon Player_Weapon { get; set; } = new AxeWeapon();
    private List<object> Inventory { get; set; } = new List<object>();

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
    public string GetName() {
      return this.Name;
    }
    public List<object> ShowInventory() {
      return this.Inventory;
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
        Console.WriteLine("You have leveled! You are now level {0}.", this.Level);
      }
    }
		public void DisplayPlayerStats() {
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
		public int CheckHealth() {
			return this.HitPoints;
		}
    public int ArmorRating() {
      var totalArmorRating =
        Player_Chest_Armor.GetArmorRating() +
        Player_Head_Armor.GetArmorRating() +
        Player_Leg_Armor.GetArmorRating();
      return totalArmorRating;
    }
		public int Attack() {
			return Player_Weapon.Attack();
		}
		public bool Combat(Monster opponent) {
			while(true) {
				DisplayPlayerStats();
				Console.WriteLine("Opponent HP: {0} / {1}", opponent.CheckHealth(), opponent.CheckMaxHealth());
        Console.WriteLine("==================================================");
        Console.WriteLine("Commands: '[A]ttack'");
        Helper.RequestCommand();
				var input = Helper.GetFormattedInput();
				Console.WriteLine(); // To add a blank space between the command and fight sequence
				switch(input) {
					case "a":
					  var attackDamage = this.Attack();
					  if (attackDamage == 0) {
						  Console.WriteLine("You missed!");
					  }
					  else {
						  Console.WriteLine("You hit the monster for {0} damage.", attackDamage);
						  opponent.TakeDamage(attackDamage);
					  }
					  if (opponent.CheckHealth() <= 0) {
						  return true;
            }
					  var attackDamageM = opponent.Attack();
					  if (attackDamageM == 0) {
						  Console.WriteLine("They missed!");
            }
					  else {
              Console.WriteLine("The monster hits you for {0} damage.", attackDamageM - this.ArmorRating());
              this.TakeDamage(attackDamageM - this.ArmorRating());
              if (this.CheckHealth() <= 0) {
								return false;
						  }
            }
					  Console.WriteLine(); // To add a blank space between the command and fight sequence
					  break;
				  default:
					  Console.WriteLine("Not a valid command.");
					  break;
				}
			}
		}
	}
}