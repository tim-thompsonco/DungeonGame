using System;

namespace DungeonGame {
  public abstract class Monster : IMonster, IRoomInteraction {
    public string Name { get; set; }
    public int MaxHitPoints { get; set; }
    public int HitPoints { get; set; }
    public int ExperienceProvided { get; set; }
    public int Gold { get; set; }
    public bool OnFire { get; set; } = false;
    public bool WasLooted { get; set; } = false;
    private readonly MainWeapon _Weapon;

    // Constructor
    protected Monster(string name, int GoldCoins, int MaxHP, int ExpProvided, MainWeapon weapon) {
      this.Name = name;
      this.Gold = GoldCoins;
      this.HitPoints = MaxHP;
      this.MaxHitPoints = MaxHP;
      this.ExperienceProvided = ExpProvided;
      this._Weapon = weapon;
    }

    // Implement method from IMonster
    public virtual void TakeDamage(int weaponDamage) {
      HitPoints -= weaponDamage;
    }
    // Implement method from IMonster
    public virtual void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", HitPoints, MaxHitPoints);
      Console.WriteLine("==================================================");
    }
    
    // Implement method from IMonster
    public virtual int Attack() {
      return _Weapon.Attack();
    }
    // Implement method from IRoomInteraction
    public string GetName() {
      return this.Name.ToString();
    }
  }
}