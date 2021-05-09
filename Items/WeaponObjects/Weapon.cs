using DungeonGame.Helpers;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;
using System.Text;

namespace DungeonGame.Items.WeaponObjects {
	public partial class Weapon : IItem, IRainbowGear, IEquipment {
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
		public Weapon() { }
		public Weapon(int level, WeaponType weaponType) {
			Level = level;
			WeaponGroup = weaponType;
			DamageGroup = DamageType.Physical;
			Weight = WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			Durability = 100;
			int randomNum = GameHelper.GetRandomNumber(1, 100);
			int randomWeaponDmg = GameHelper.GetRandomNumber(20, 26);
			if (randomNum < 5) {
				RegDamage = randomWeaponDmg + (level - 1) * 3;
				CritMultiplier = 1.3;
				Quality = 3;
			} else if (randomNum < 40) {
				RegDamage = randomWeaponDmg + (level - 1) * 2;
				CritMultiplier = 1.2;
				Quality = 2;
			} else if (randomNum <= 100) {
				RegDamage = randomWeaponDmg + (level - 1) * 1;
				CritMultiplier = 1.1;
				Quality = 1;
			}
			ItemValue = RegDamage;
			BuildWeaponName();
			Desc = $"A {Name} that causes damage when you hit stuff with it.";
		}
		public Weapon(WeaponType weaponType, bool isRainbowGear, Player player) {
			Level = player.Level;
			IsRainbowGear = isRainbowGear;
			WeaponGroup = weaponType;
			DamageGroup = DamageType.Physical;
			Weight = WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			Durability = 100;
			int randomNum = GameHelper.GetRandomNumber(1, 100);
			int randomWeaponDmg = GameHelper.GetRandomNumber(20, 26);
			if (randomNum < 5) {
				RegDamage = randomWeaponDmg + (Level - 1) * 3;
				CritMultiplier = 1.3;
				Quality = 3;
			} else if (randomNum < 40) {
				RegDamage = randomWeaponDmg + (Level - 1) * 2;
				CritMultiplier = 1.2;
				Quality = 2;
			} else if (randomNum <= 100) {
				RegDamage = randomWeaponDmg + (Level - 1) * 1;
				CritMultiplier = 1.1;
				Quality = 1;
			}
			// Add modifier for rainbow gear to enhance weapon damage
			RegDamage += 15;
			ItemValue = RegDamage;
			BuildWeaponName("rainbow");
			Desc = $"A {Name} that causes damage when you hit stuff with it.";
		}
		public Weapon(int level, WeaponType weaponType, MonsterType monsterType)
			: this(level, weaponType) {
			StringBuilder sb = new StringBuilder();
			switch (monsterType) {
				case MonsterType.Skeleton:
					return;
				case MonsterType.Zombie:
					return;
				case MonsterType.Spider:
					sb.Append(Quality switch {
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("venomous fang");
					Name = sb.ToString();
					return;
				case MonsterType.Demon:
					return;
				case MonsterType.Elemental:
					return;
				case MonsterType.Vampire:
					return;
				case MonsterType.Troll:
					return;
				case MonsterType.Dragon:
					sb.Append(Quality switch {
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("dragon fang");
					Name = sb.ToString();
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(monsterType), monsterType, null);
			}
		}

		private void BuildWeaponName() {
			StringBuilder sb = new StringBuilder();
			if (WeaponGroup != WeaponType.Bow) {
				sb.Append(Level switch {
					1 => "chipped ",
					2 => "dull ",
					3 => "worn ",
					_ => ""
				});
			} else {
				sb.Append(Level switch {
					1 => "cracked ",
					2 => "worn ",
					3 => "solid ",
					_ => ""
				});
			}
			if (WeaponGroup != WeaponType.Bow) {
				sb.Append(Quality switch {
					1 => "",
					2 => "sturdy ",
					3 => "fine ",
					_ => ""
				});
			} else {
				sb.Append(Quality switch {
					1 => "pine ",
					2 => "oak ",
					3 => "great oak ",
					_ => ""
				});
			}
			sb.Append(WeaponGroup switch {
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			Name = sb.ToString();
		}
		private void BuildWeaponName(string rainbowName) {
			StringBuilder sb = new StringBuilder();
			sb.Append($"{rainbowName} ");
			sb.Append(WeaponGroup switch {
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			Name = sb.ToString();
		}
		public int Attack() {
			if (!Equipped) {
				return 0;
			}

			double attackDamage = RegDamage;
			int chanceToCrit = GameHelper.GetRandomNumber(1, 100);
			if (chanceToCrit <= 25) {
				attackDamage *= CritMultiplier;
			}

			Durability -= 1;
			attackDamage *= Durability / (double)100;
			return (int)attackDamage;
		}
		public void UpdateRainbowStats(Player player) {
			Level = player.Level;
			int randomWeaponDmg = GameHelper.GetRandomNumber(20, 26);
			RegDamage = randomWeaponDmg + (Level - 1) * 3;
			CritMultiplier = 1.3;
			Quality = 3;
			// Add modifier for rainbow gear to enhance weapon damage
			RegDamage += 15;
			ItemValue = RegDamage;
		}
	}
}