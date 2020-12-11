using System;
using System.Text;

namespace DungeonGame
{
	public class Weapon : IEquipment, IRainbowGear
	{
		public enum DamageType
		{
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum WeaponType
		{
			Dagger,
			OneHandedSword,
			TwoHandedSword,
			Axe,
			Bow
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _RegDamage { get; set; }
		public int _ItemValue { get; set; }
		public DamageType _DamageGroup { get; set; }
		public WeaponType _WeaponGroup { get; set; }
		public double _CritMultiplier { get; set; }
		public bool _Equipped { get; set; }
		public int _Level { get; set; }
		public int _Durability { get; set; }
		public int _Quality { get; set; }
		public int _Weight { get; set; }
		public bool _IsRainbowGear { get; set; }

		// Default constructor for JSON serialization
		public Weapon() { }
		public Weapon(int level, WeaponType weaponType)
		{
			_Level = level;
			_WeaponGroup = weaponType;
			_DamageGroup = DamageType.Physical;
			_Weight = _WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			_Durability = 100;
			int randomNum = GameHandler.GetRandomNumber(1, 100);
			int randomWeaponDmg = GameHandler.GetRandomNumber(20, 26);
			if (randomNum < 5)
			{
				_RegDamage = randomWeaponDmg + ((level - 1) * 3);
				_CritMultiplier = 1.3;
				_Quality = 3;
			}
			else if (randomNum < 40)
			{
				_RegDamage = randomWeaponDmg + ((level - 1) * 2);
				_CritMultiplier = 1.2;
				_Quality = 2;
			}
			else if (randomNum <= 100)
			{
				_RegDamage = randomWeaponDmg + ((level - 1) * 1);
				_CritMultiplier = 1.1;
				_Quality = 1;
			}
			_ItemValue = _RegDamage;
			BuildWeaponName();
			_Desc = $"A {_Name} that causes damage when you hit stuff with it.";
		}
		public Weapon(WeaponType weaponType, bool isRainbowGear, Player player)
		{
			_Level = player._Level;
			_IsRainbowGear = isRainbowGear;
			_WeaponGroup = weaponType;
			_DamageGroup = DamageType.Physical;
			_Weight = _WeaponGroup == WeaponType.TwoHandedSword ? 4 : 2;
			_Durability = 100;
			int randomNum = GameHandler.GetRandomNumber(1, 100);
			int randomWeaponDmg = GameHandler.GetRandomNumber(20, 26);
			if (randomNum < 5)
			{
				_RegDamage = randomWeaponDmg + ((_Level - 1) * 3);
				_CritMultiplier = 1.3;
				_Quality = 3;
			}
			else if (randomNum < 40)
			{
				_RegDamage = randomWeaponDmg + ((_Level - 1) * 2);
				_CritMultiplier = 1.2;
				_Quality = 2;
			}
			else if (randomNum <= 100)
			{
				_RegDamage = randomWeaponDmg + ((_Level - 1) * 1);
				_CritMultiplier = 1.1;
				_Quality = 1;
			}
			// Add modifier for rainbow gear to enhance weapon damage
			_RegDamage += 15;
			_ItemValue = _RegDamage;
			BuildWeaponName("rainbow");
			_Desc = $"A {_Name} that causes damage when you hit stuff with it.";
		}
		public Weapon(int level, WeaponType weaponType, Monster.MonsterType monsterType)
			: this(level, weaponType)
		{
			StringBuilder sb = new StringBuilder();
			switch (monsterType)
			{
				case Monster.MonsterType.Skeleton:
					return;
				case Monster.MonsterType.Zombie:
					return;
				case Monster.MonsterType.Spider:
					sb.Append(_Quality switch
					{
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("venomous fang");
					_Name = sb.ToString();
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
					sb.Append(_Quality switch
					{
						1 => "",
						2 => "sturdy ",
						3 => "fine ",
						_ => ""
					});
					sb.Append("dragon fang");
					_Name = sb.ToString();
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(monsterType), monsterType, null);
			}
		}

		private void BuildWeaponName()
		{
			StringBuilder sb = new StringBuilder();
			if (_WeaponGroup != WeaponType.Bow)
			{
				sb.Append(_Level switch
				{
					1 => "chipped ",
					2 => "dull ",
					3 => "worn ",
					_ => ""
				});
			}
			else
			{
				sb.Append(_Level switch
				{
					1 => "cracked ",
					2 => "worn ",
					3 => "solid ",
					_ => ""
				});
			}
			if (_WeaponGroup != WeaponType.Bow)
			{
				sb.Append(_Quality switch
				{
					1 => "",
					2 => "sturdy ",
					3 => "fine ",
					_ => ""
				});
			}
			else
			{
				sb.Append(_Quality switch
				{
					1 => "pine ",
					2 => "oak ",
					3 => "great oak ",
					_ => ""
				});
			}
			sb.Append(_WeaponGroup switch
			{
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			_Name = sb.ToString();
		}
		private void BuildWeaponName(string rainbowName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"{rainbowName} ");
			sb.Append(_WeaponGroup switch
			{
				WeaponType.Axe => "axe",
				WeaponType.Bow => "bow",
				WeaponType.Dagger => "dagger",
				WeaponType.OneHandedSword => "sword (1H)",
				WeaponType.TwoHandedSword => "sword (2H)",
				_ => ""
			});
			_Name = sb.ToString();
		}
		public int Attack()
		{
			if (!_Equipped)
			{
				return 0;
			}

			double attackDamage = _RegDamage;
			int chanceToCrit = GameHandler.GetRandomNumber(1, 100);
			if (chanceToCrit <= 25)
			{
				attackDamage *= _CritMultiplier;
			}

			_Durability -= 1;
			attackDamage *= _Durability / (double)100;
			return (int)attackDamage;
		}
		public void UpdateRainbowStats(Player player)
		{
			_Level = player._Level;
			int randomWeaponDmg = GameHandler.GetRandomNumber(20, 26);
			_RegDamage = randomWeaponDmg + ((_Level - 1) * 3);
			_CritMultiplier = 1.3;
			_Quality = 3;
			// Add modifier for rainbow gear to enhance weapon damage
			_RegDamage += 15;
			_ItemValue = _RegDamage;
		}
	}
}