namespace DungeonGame
{
  class Monster {
    private int MaxHitPoints { get; set; } = 100;
    private int HitPoints { get; set; } = 100;
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
  }
}