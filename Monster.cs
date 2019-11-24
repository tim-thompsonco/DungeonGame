using System;

namespace DungeonGame {
  public class Monster {
    private string Name { get; set; } = "Zombie";
    private int MaxHitPoints { get; set; } = 100;
    private int HitPoints { get; set; } = 100;
		private int ExperienceProvided { get; } = 500;
    private readonly AxeWeapon Weapon = new AxeWeapon();

    public void TakeDamage(int weaponDamage) {
      this.HitPoints -= weaponDamage;
    }
    public int CheckMaxHealth() {
      return this.MaxHitPoints;
    }
    public int CheckHealth() {
      return this.HitPoints;
    }
    public int Attack() {
      return Weapon.Attack();
    }
		public int GiveExperience() {
			return this.ExperienceProvided;
		}
    public string GetName() {
      return this.Name;
    }
    public void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", this.CheckHealth(), this.CheckMaxHealth());
      Console.WriteLine("==================================================");
    }
  }
}