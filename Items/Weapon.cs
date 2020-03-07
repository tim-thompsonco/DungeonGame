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
		public string Desc { get; set; }
		public int RegDamage { get; set; }
		public int ItemValue { get; set; }
		public WeaponType WeaponGroup { get; set; }
		public double CritMultiplier { get; set; }
		public bool Equipped { get; set; }
		private int Level { get; set; }
		public int Durability { get; set; }
		public int Quality { get; set; }
		public int Weight { get; set; }

		// Default constructor for JSON serialization
		public Weapon() {}
		public Weapon(int level, WeaponType weaponType) {
			this.Level = level;
			this.WeaponGroup = weaponType;
			this.Weight = this.WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			this.Durability = 100;
			var randomNum = GameHandler.GetRandomNumber(1, 100);
			var randomWeaponDmg = GameHandler.GetRandomNumber(18, 24);
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
		public Weapon(int level, WeaponType weaponType, Monster.MonsterType monsterType)
			: this(level, weaponType) {
			if (monsterType != Monster.MonsterType.Spider) return;
			var sb = new StringBuilder();
			sb.Append(this.Quality switch {
				1 => "",
				2 => "sturdy ",
				3 => "fine ",
				_ => ""
			});
			sb.Append("venomous fang");
			this.Name = sb.ToString();
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