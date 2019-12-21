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
		public Loot Item;
		public Consumable Consumable;
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
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Loot item)
			: this(name, desc, level, GoldCoins, MaxHP, ExpProvided, weapon) {
			this.Item = item;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Item);
		}
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Armor armor)
			: this(name, desc, level, GoldCoins, MaxHP, ExpProvided, weapon) {
			this.Monster_Chest_Armor = armor;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Chest_Armor);
		}
		public Monster(string name, string desc, int level, int GoldCoins, int MaxHP, int ExpProvided, Weapon weapon, Armor armor, Consumable consumable)
			: this(name, desc, level, GoldCoins, MaxHP, ExpProvided, weapon) {
			this.Monster_Chest_Armor = armor;
			this.Consumable = consumable;
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Monster_Chest_Armor);
			this.MonsterItems.Add((DungeonGame.IEquipment)this.Consumable);
		}

		public void TakeDamage(int weaponDamage) {
      HitPoints -= weaponDamage;
    }
    public void DisplayStats() {
      Console.WriteLine("Opponent HP: {0} / {1}", HitPoints, MaxHitPoints);
      Console.WriteLine("==================================================");
    }
    public int Attack() {
      return Monster_Weapon.Attack();
    }
		public int CheckArmorRating() {
			var totalArmorRating = 0;
			if (this.Monster_Chest_Armor != null && this.Monster_Chest_Armor.IsEquipped()) {
				totalArmorRating += this.Monster_Chest_Armor.ArmorRating;
			}
			if (this.Monster_Head_Armor != null && this.Monster_Head_Armor.IsEquipped()) {
				totalArmorRating += this.Monster_Head_Armor.ArmorRating;
			}
			if (this.Monster_Leg_Armor != null && this.Monster_Leg_Armor.IsEquipped()) {
				totalArmorRating += this.Monster_Leg_Armor.ArmorRating;
			}
			return totalArmorRating;
		}
		public int ArmorRating(Player player) {
			int totalArmorRating = CheckArmorRating();
			int levelDiff = player.Level - this.Level;
			double armorMultiplier = 1.00 + (-(double)levelDiff / 10);
			double adjArmorRating = (double)totalArmorRating * armorMultiplier;
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