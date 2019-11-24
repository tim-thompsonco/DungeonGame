using System;

namespace DungeonGame {
  public class Monster {
    public string Name { get; set; } = "zombie";
    public int MaxHitPoints { get; set; } = 100;
    public int HitPoints { get; set; } = 100;
		public int ExperienceProvided { get; } = 500;
		public bool OnFire { get; set; } = false;
    private readonly MainWeapon Weapon = new MainWeapon();

		public bool IsAlive() {
			if (this.HitPoints > 0) {
				return true;
			}
			else {
				return false;
			}
		}
    public void TakeDamage(int weaponDamage) {
      this.HitPoints -= weaponDamage;
    }
    public int Attack() {
      return Weapon.Attack();
    }
    public void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", this.HitPoints, this.MaxHitPoints);
      Console.WriteLine("==================================================");
    }
  }
}