using System;

namespace DungeonGame
{
	class NewPlayer
	{
		private int MaxHitPoints {get; set; } = 100;
		private int MaxManaPoints {get; set; } = 100;
		private int HitPoints { get; set; } = 100;
		private int ManaPoints { get; set; } = 100;
		private readonly AxeWeapon Weapon = new AxeWeapon();

		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public int CheckHealth() {
			return this.HitPoints;
		}
		public int Attack() {
			return Weapon.Attack();
		}
		public void Combat(Monster opponent) {
			do {
				Console.WriteLine("HP: {0} / {1} - MP: {2} / {3}",
					this.HitPoints, this.MaxHitPoints, this.ManaPoints, this.MaxManaPoints);
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
						  Console.WriteLine("You have defeated the monster!");
						  return;
            }
					  var attackDamageM = opponent.Attack();
					  if (attackDamageM == 0) {
						  Console.WriteLine("They missed!");
            }
					  else {
						  Console.WriteLine("The monster hits you for {0} damage.", attackDamageM);
						  this.TakeDamage(attackDamageM);
            }
					  Console.WriteLine(); // To add a blank space between the command and fight sequence
					  break;
				  default:
					  Console.WriteLine("Not a valid command.");
					  break;
				}
			} while (this.HitPoints > 0 || opponent.CheckHealth() > 0);
		}
	}
}