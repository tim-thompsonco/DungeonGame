using System;
using System.Text;

namespace DungeonGame {
	public class Weapon : IEquipment {
		public enum DamageType {
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum WeaponType {
			Dagger,
			OneHandedSword,
			TwoHandedSword,
			Axe,
			Bow
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public int RegDamage { get; set; }
		public int ItemValue { get; set; }
		public DamageType DamageGroup { get; set; }
		public WeaponType WeaponGroup { get; set; }
		public double CritMultiplier { get; set; }
		public bool Equipped { get; set; }
		public int Level { get; set; }
		public int Durability { get; set; }
		public int Quality { get; set; }
		public int Weight { get; set; }
		public bool IsRainbowGear { get; set; }

		// Default constructor for JSON serialization
		public Weapon() {}
		public Weapon(int level, WeaponType weaponType) {
			this.Level = level;
			this.WeaponGroup = weaponType;
			this.DamageGroup = DamageType.Physical;
			this.Weight = this.WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			this.Durability = 100;
			var randomNum = GameHandler.GetRandomNumber(1, 100);
			var randomWeaponDmg = GameHandler.GetRandomNumber(20, 26);
			if (randomNum < 5) {
				this.RegDamage = randomWeaponDmg + (level - 1) * 3;
				this.CritMultiplier = 1.3;
				this.Quality = 3;
			}
			else if (randomNum < 40) {
				this.RegDamage = randomWeaponDmg + (level - 1) * 2;
				this.CritMultiplier = 1.2;
				this.Quality = 2;
			}
			else if (randomNum <= 100) {
				this.RegDamage = randomWeaponDmg + (level - 1) * 1;
				this.CritMultiplier = 1.1;
				this.Quality = 1;
			}
			this.ItemValue = this.RegDamage;
			this.BuildWeaponName();
			this.Desc = "A " + this.Name + " that causes damage when you hit stuff with it.";
		}
		public Weapon(WeaponType weaponType, bool isRainbowGear) {
			this.Level = 10;
			this.IsRainbowGear = isRainbowGear;
			this.WeaponGroup = weaponType;
			this.DamageGroup = DamageType.Physical;
			this.Weight = this.WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			this.Durability = 100;
			var randomNum = GameHandler.GetRandomNumber(1, 100);
			var randomWeaponDmg = GameHandler.GetRandomNumber(20, 26);
			if (randomNum < 5) {
				this.RegDamage = randomWeaponDmg + (this.Level - 1) * 3;
				this.CritMultiplier = 1.3;
				this.Quality = 3;
			}
			else if (randomNum < 40) {
				this.RegDamage = randomWeaponDmg + (this.Level - 1) * 2;
				this.CritMultiplier = 1.2;
				this.Quality = 2;
			}
			else if (randomNum <= 100) {
				this.RegDamage = randomWeaponDmg + (this.Level - 1) * 1;
				this.CritMultiplier = 1.1;
				this.Quality = 1;
			}
			// Add modifier for rainbow gear to enhance weapon damage
			this.RegDamage += 5;
			this.ItemValue = this.RegDamage;
			this.BuildWeaponName("rainbow");
			this.Desc = "A " + this.Name + " that causes damage when you hit stuff with it.";
		}
		public Weapon(int level, WeaponType weaponType, Monster.MonsterType monsterType)
			: this(level, weaponType) {
			var sb = new StringBuilder();
			switch (monsterType) {
				case Monster.MonsterType.Skeleton:
					return;
				case Monster.MonsterType.Zombie:
					return;
				case Monster.MonsterType.Spider:
					sb.Append(this.Quality switch {
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("venomous fang");
					this.Name = sb.ToString();
					return;
				case Monster.MonsterType.Demon:
					return;
				case Monster.MonsterType.Elemental:
					return;
				case Monster.MonsterType.Vampire:
					return;
				case Monster.MonsterType.Troll:
					return;
				case Monster.MonsterType.Dragon:
					sb.Append(this.Quality switch {
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("dragon fang");
					this.Name = sb.ToString();
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(monsterType), monsterType, null);
			}
		}

		private void BuildWeaponName() {
			var sb = new StringBuilder();
			if (this.WeaponGroup != WeaponType.Bow) {
				sb.Append(this.Level switch {
					1 => "chipped ",
					2 => "dull ",
					3 => "worn ",
					_ => ""
				});
			}
			else {
				sb.Append(this.Level switch {
					1 => "cracked ",
					2 => "worn ",
					3 => "solid ",
					_ => ""
				});
			}
			if (this.WeaponGroup != WeaponType.Bow) {
				sb.Append(this.Quality switch {
					1 => "",
					2 => "sturdy ",
					3 => "fine ",
					_ => ""
				});
			}
			else {
				sb.Append(this.Quality switch {
					1 => "pine ",
					2 => "oak ",
					3 => "great oak ",
					_ => ""
				});
			}
			sb.Append(this.WeaponGroup switch {
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			this.Name = sb.ToString();
		}
		private void BuildWeaponName(string rainbowName) {
			var sb = new StringBuilder();
			sb.Append(rainbowName + " ");
			sb.Append(this.WeaponGroup switch {
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			this.Name = sb.ToString();
		}
		public int Attack() {
			if (!this.Equipped) return 0;
			double attackDamage = this.RegDamage;
			var chanceToCrit = GameHandler.GetRandomNumber(1, 100);
			if (chanceToCrit <= 25) attackDamage *= this.CritMultiplier;
			this.Durability -= 1;
			attackDamage *= this.Durability / (double)100;
			return (int)attackDamage;
		}
	}
}