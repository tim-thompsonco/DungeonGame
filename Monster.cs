using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class Monster : IMonster {
    public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
    public int HitPoints { get; set; }
    public int ExperienceProvided { get; set; }
    public int Gold { get; set; }
    public bool OnFire { get; set; } = false;
    public bool WasLooted { get; set; } = false;
		public List<IEquipment> MonsterItems { get; set; } = new List<IEquipment>();
		public Item Item;
		public Weapon Monster_Weapon;
		public Armor Monster_Chest_Armor;
		public Armor Monster_Head_Armor;
		public Armor Monster_Leg_Armor;

		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon) {
      this.Name = name;
			this.Desc = desc;
			this.Level = level;
      this.Gold = GoldCoins;
			this.MaxHitPoints = MaxHP;
			this.HitPoints = MaxHP;
      this.ExperienceProvided = ExpProvided;
      this.Monster_Weapon = weapon;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Weapon);
		}
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Item item) {
			this.Name = name;
			this.Desc = desc;
			this.Level = level;
			this.Gold = GoldCoins;
			this.MaxHitPoints = MaxHP;
			this.HitPoints = MaxHP;
			this.ExperienceProvided = ExpProvided;
			this.Monster_Weapon = weapon;
			this.Item = item;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Weapon);
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Item);
		}
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Armor armor) {
			this.Name = name;
			this.Desc = desc;
			this.Level = level;
			this.Gold = GoldCoins;
			this.MaxHitPoints = MaxHP;
			this.HitPoints = MaxHP;
			this.ExperienceProvided = ExpProvided;
			this.Monster_Weapon = weapon;
			this.Monster_Chest_Armor = armor;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Weapon);
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Chest_Armor);
		}

		public virtual void TakeDamage(int weaponDamage) {
      HitPoints -= weaponDamage;
    }
    public virtual void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", HitPoints, MaxHitPoints);
      Console.WriteLine("==================================================");
    }
    public virtual int Attack() {
      return Monster_Weapon.Attack();
    }
		public int CheckArmorRating() {
			var totalArmorRating = 0;
			try {
				if (this.Monster_Chest_Armor.IsEquipped()) {
					totalArmorRating += this.Monster_Chest_Armor.ArmorRating;
				}
			}
			catch (NullReferenceException) {
			}
			try {
				if (this.Monster_Head_Armor.IsEquipped()) {
					totalArmorRating += this.Monster_Head_Armor.ArmorRating;
				}
			}
			catch (NullReferenceException) {
			}
			try {
				if (this.Monster_Leg_Armor.IsEquipped()) {
					totalArmorRating += this.Monster_Leg_Armor.ArmorRating;
				}
			}
			catch (NullReferenceException) {
			}
			return totalArmorRating;
		}
		public int ArmorRating(NewPlayer player) {
			var totalArmorRating = CheckArmorRating();
			var levelDiff = player.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 10);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public string GetName() {
      return this.Name.ToString();
    }
		public bool IsEquipped() {
			return false;
		}
	}
}