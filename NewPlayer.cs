using System;

namespace DungeonGame {
	class NewPlayer {
		// Attributes for new player
		private readonly string Name;
		private int MaxHitPoints {get; set; } = 100;
		private int MaxManaPoints {get; set; } = 100;
		private int HitPoints { get; set; } = 100;
		private int ManaPoints { get; set; } = 100;
		private int Experience {get; set; } = 0;
		private int Level {get; set; } = 1;
    private Chest_Armor Player_Chest_Armor = new Chest_Armor(); 
		private AxeWeapon Player_Weapon = new AxeWeapon();

		// Constructor for new player creation
		public NewPlayer (string name) {
			this.Name = name;
		}

		// Methods for new player
		public void DisplayPlayerStats() {
			Console.WriteLine("HP: {0} / {1}", this.HitPoints, this.MaxHitPoints);
			Console.WriteLine("MP: {0} / {1}", this.ManaPoints, this.MaxManaPoints);
		}
		public void DisplayPlayerExpLevel() {
			Console.WriteLine("Experience: {0}", this.Experience);
			Console.WriteLine("Level: {0}", this.Level);
		}
		public void DisplayPlayerStatsAll() {
			DisplayPlayerStats();
			DisplayPlayerExpLevel();
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
      var totalArmorRating = Player_Chest_Armor.GetArmorRating();
      return totalArmorRating;
    }
		public int Attack() {
			return Player_Weapon.Attack();
		}
		public bool Combat(Monster opponent) {
			while(true) {
				DisplayPlayerStats();
				Console.WriteLine("Opponent HP: {0} / {1}", opponent.CheckHealth(), opponent.CheckMaxHealth());
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