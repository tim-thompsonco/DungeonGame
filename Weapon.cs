using System;
using System.Text;

namespace DungeonGame {
	public class Weapon : IEquipment {
		public enum WeaponType {
			Dagger,
			OneHandedSword,
			TwoHandedSword,
			Axe,
			Bow
		}
		public string Name { get; set; }
		public int RegDamage { get; set; }
		public int ItemValue { get; set; }
		public WeaponType WeaponGroup { get; set; }
		public double CritMultiplier { get; set; }
		public bool Equipped { get; set; }
		public int Durability { get; set; }

		// Default constructor for JSON serialization
		public Weapon() {}
		public Weapon(int level, WeaponType weaponType) {
			this.WeaponGroup = weaponType;
			this.Durability = 100;
			var randomNum = Helper.GetRandomNumber(1, 100);
			var randomWeaponDmg = Helper.GetRandomNumber(14, 24);
			if (randomNum < 5) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 3);
				this.CritMultiplier = 1.3;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "obsidian axe",
					WeaponType.Dagger => "obsidian dagger",
					WeaponType.OneHandedSword => "obsidian sword (1H)",
					WeaponType.TwoHandedSword => "obsidian claymore (2H)",
					WeaponType.Bow => "great oak bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			else if (randomNum < 40) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 2);
				this.CritMultiplier = 1.2;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "bronze axe",
					WeaponType.Dagger => "bronze dagger",
					WeaponType.OneHandedSword => "bronze sword (1H)",
					WeaponType.TwoHandedSword => "bronze claymore (2H)",
					WeaponType.Bow => "oak bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			else if (randomNum <= 100) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 1);
				this.CritMultiplier = 1.1;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "iron axe",
					WeaponType.Dagger => "iron dagger",
					WeaponType.OneHandedSword => "iron sword (1H)",
					WeaponType.TwoHandedSword => "iron claymore (2H)",
					WeaponType.Bow => "pine bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			this.ItemValue = this.RegDamage;
		}
		public Weapon(int level, WeaponType weaponType, Monster.MonsterType monsterType) {
			this.WeaponGroup = weaponType;
			this.Durability = 100;
			var randomNum = Helper.GetRandomNumber(1, 100);
			var randomWeaponDmg = Helper.GetRandomNumber(14, 24);
			if (randomNum < 20) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 3);
				this.CritMultiplier = 1.3;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "obsidian axe",
					WeaponType.Dagger => "obsidian dagger",
					WeaponType.OneHandedSword => "obsidian sword (1H)",
					WeaponType.TwoHandedSword => "obsidian claymore (2H)",
					WeaponType.Bow => "great oak bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			else if (randomNum < 40) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 2);
				this.CritMultiplier = 1.2;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "bronze axe",
					WeaponType.Dagger => "bronze dagger",
					WeaponType.OneHandedSword => "bronze sword (1H)",
					WeaponType.TwoHandedSword => "bronze claymore (2H)",
					WeaponType.Bow => "oak bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			else if (randomNum <= 100) {
				this.RegDamage = randomWeaponDmg + ((level - 1) * 1);
				this.CritMultiplier = 1.1;
				this.Name = this.WeaponGroup switch {
					WeaponType.Axe => "iron axe",
					WeaponType.Dagger => "iron dagger",
					WeaponType.OneHandedSword => "iron sword (1H)",
					WeaponType.TwoHandedSword => "iron claymore (2H)",
					WeaponType.Bow => "pine bow",
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			if (monsterType == Monster.MonsterType.Spider) this.Name = "venomous fang";
			this.ItemValue = this.RegDamage;
		}

		public int Attack() {
			if (!this.Equipped) return 0;
			var attackDamage = 0f;
			var attackType = Helper.GetRandomNumber(1, 12); // Creates a random number to determine attack type
			// Main attack
			if (attackType < 6) {
				attackDamage = this.RegDamage;
			}
			// Stronger attack
			else if (attackType < 11) {
				attackDamage = (int)((double)this.RegDamage * this.CritMultiplier);
			}
			// If RNG didn't cause main or stronger attack, it's a miss
			this.Durability -= 1;
			attackDamage *= this.Durability / 100f;
			return (int)attackDamage;
		}
		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}