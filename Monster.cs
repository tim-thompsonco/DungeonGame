using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class Monster : IMonster, IRoomInteraction {
    public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
    public int HitPoints { get; set; }
    public int ExperienceProvided { get; set; }
    public int Gold { get; set; }
    public bool OnFire { get; set; } = false;
    public bool WasLooted { get; set; } = false;
		public List<IRoomInteraction> MonsterItems { get; set; } = new List<IRoomInteraction>();
		public Item Item { get; set; }
		public Weapon Weapon { get; set; }

    // Constructor with weapon
    public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon) {
      this.Name = name;
			this.Desc = desc;
			this.Level = level;
      this.Gold = GoldCoins;
			this.MaxHitPoints = MaxHP;
			this.HitPoints = MaxHP;
      this.ExperienceProvided = ExpProvided;
      this.Weapon = weapon;
			this.MonsterItems.Add((DungeonGame.IRoomInteraction)this.Weapon);
		}
		// Constructor with weapon and loot
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Item item) {
			this.Name = name;
			this.Desc = desc;
			this.Level = level;
			this.Gold = GoldCoins;
			this.MaxHitPoints = MaxHP;
			this.HitPoints = MaxHP;
			this.ExperienceProvided = ExpProvided;
			this.Weapon = weapon;
			this.Item = item;
			this.MonsterItems.Add((DungeonGame.IRoomInteraction)this.Weapon);
			this.MonsterItems.Add((DungeonGame.IRoomInteraction)this.Item);
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