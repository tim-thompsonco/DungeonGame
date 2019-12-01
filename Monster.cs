using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class Monster : IMonster, IRoomInteraction {
    public string Name { get; set; }
    public int MaxHitPoints { get; set; }
    public int HitPoints { get; set; }
    public int ExperienceProvided { get; set; }
    public int Gold { get; set; }
    public bool OnFire { get; set; } = false;
    public bool WasLooted { get; set; } = false;
		public List<IRoomInteraction> MonsterItems { get; set; } = new List<IRoomInteraction>();
		public Weapon Weapon;

    // Constructor
    public Monster(string name, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon) {
      this.Name = name;
      this.Gold = GoldCoins;
      this.HitPoints = MaxHP;
      this.MaxHitPoints = MaxHP;
      this.ExperienceProvided = ExpProvided;
      this.Weapon = weapon;
			this.MonsterItems.Add((DungeonGame.IRoomInteraction)this.Weapon);
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
      return Weapon.Attack();
    }
		// Implement method from IRoomInteraction
		public string GetName() {
      return this.Name.ToString();
    }
	}
}