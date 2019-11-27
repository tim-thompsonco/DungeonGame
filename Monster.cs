using System;

namespace DungeonGame {
  public class Monster : IMonster, IRoomInteraction {
    public string name { get; set; }
    public int maxHitPoints { get; set; }
    public int hitPoints { get; set; }
    public int experienceProvided { get; set; }
    public int gold { get; set; }
    public bool onFire { get; set; } = false;
    public bool wasLooted { get; set; } = false;
    private readonly Weapon _weapon;

    // Constructor
    public Monster(string name, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon) {
      this.name = name;
      this.gold = GoldCoins;
      this.hitPoints = MaxHP;
      this.maxHitPoints = MaxHP;
      this.experienceProvided = ExpProvided;
      this._weapon = weapon;
    }

    // Implement method from IMonster
    public virtual void TakeDamage(int weaponDamage) {
      hitPoints -= weaponDamage;
    }
    // Implement method from IMonster
    public virtual void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", hitPoints, maxHitPoints);
      Console.WriteLine("==================================================");
    }
    
    // Implement method from IMonster
    public virtual int Attack() {
      return _weapon.Attack();
    }
    // Implement method from IRoomInteraction
    public string GetName() {
      return this.name.ToString();
    }
  }
}