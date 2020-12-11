using System;

namespace DungeonGame
{
	public class Consumable : IEquipment
	{
		public enum ArrowType
		{
			Standard
		}
		public enum PotionType
		{
			Health,
			Mana,
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public enum PotionLevel
		{
			Minor,
			Normal,
			Greater
		}
		public enum KitLevel
		{
			Light,
			Medium,
			Heavy
		}
		public enum KitType
		{
			Armor,
			Weapon
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public bool _Equipped { get; set; }
		public ArrowType? _ArrowCategory { get; set; }
		public PotionType? _PotionCategory { get; set; }
		public PotionLevel? _PotionStrength { get; set; }
		public KitLevel? _KitStrength { get; set; }
		public KitType? _KitCategory { get; set; }
		public RestoreHealth _RestoreHealth { get; set; }
		public RestoreMana _RestoreMana { get; set; }
		public ChangeStat _ChangeStat { get; set; }
		public ChangeArmor _ChangeArmor { get; set; }
		public ChangeWeapon _ChangeWeapon { get; set; }
		public Arrow _Arrow { get; set; }
		public int _Weight { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() { }
		public Consumable(int level, PotionType potionType)
		{
			_PotionCategory = potionType;
			_Weight = 1;
			int amount;
			string name;
			if (level <= 3)
			{
				_PotionStrength = PotionLevel.Minor;
				name = $"{PotionLevel.Minor.ToString().ToLowerInvariant()} {potionType.ToString().ToLowerInvariant()} potion";
				amount = _PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana ? 50 : 5;
			}
			else if (level > 6)
			{
				_PotionStrength = PotionLevel.Greater;
				name = $"{PotionLevel.Greater.ToString().ToLowerInvariant()} {potionType.ToString().ToLowerInvariant()} potion";
				amount = _PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana ? 150 : 15;
			}
			else
			{
				_PotionStrength = PotionLevel.Normal;
				name = $"{potionType.ToString().ToLowerInvariant()} potion";
				amount = _PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana ? 100 : 10;
			}
			_ItemValue = _PotionCategory == PotionType.Health ||
							 _PotionCategory == PotionType.Mana ? amount / 2 : amount * 10 / 2;
			_Name = name;
			_Desc = _PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana
				? $"A {name} that restores {amount} {_PotionCategory.ToString().ToLowerInvariant()}."
				: $"A {name} that increases {amount} {_PotionCategory.ToString().ToLowerInvariant()}.";
			switch (_PotionCategory)
			{
				case PotionType.Health:
					_RestoreHealth = new RestoreHealth(amount);
					break;
				case PotionType.Mana:
					_RestoreMana = new RestoreMana(amount);
					break;
				case PotionType.Intelligence:
					_ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Intelligence);
					break;
				case PotionType.Strength:
					_ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Strength);
					break;
				case PotionType.Dexterity:
					_ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Dexterity);
					break;
				case PotionType.Constitution:
					_ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Constitution);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(potionType), potionType, null);
			}
		}
		public Consumable(string name, int itemValue, ArrowType arrowType)
		{
			_Name = name;
			_Weight = 1;
			_ItemValue = itemValue;
			_ArrowCategory = arrowType;
			_Arrow = new Arrow(50);
			_Desc = $"A bundle of {_Arrow._Quantity} arrows.";
		}
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeArmor.KitType kitCategory)
		{
			_KitCategory = kitType;
			_Name = $"{kitLevel.ToString().ToLowerInvariant()} {kitCategory.ToString().ToLowerInvariant()} {kitType.ToString().ToLowerInvariant()} kit";
			_Weight = 1;
			_KitStrength = kitLevel;
			int amount = _KitStrength switch
			{
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			_ItemValue = amount * 10;
			_ChangeArmor = kitCategory switch
			{
				ChangeArmor.KitType.Cloth => new ChangeArmor(amount, ChangeArmor.KitType.Cloth),
				ChangeArmor.KitType.Leather => new ChangeArmor(amount, ChangeArmor.KitType.Leather),
				ChangeArmor.KitType.Plate => new ChangeArmor(amount, ChangeArmor.KitType.Plate),
				_ => throw new ArgumentOutOfRangeException()
			};
			_Desc = $"A single-use {_Name} that increases armor rating by {amount} for one armor item.";
		}
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeWeapon.KitType kitCategory)
		{
			_KitCategory = kitType;
			_Name = $"{kitLevel.ToString().ToLowerInvariant()} {kitCategory.ToString().ToLowerInvariant()} {kitType.ToString().ToLowerInvariant()} kit";
			_Weight = 1;
			_KitStrength = kitLevel;
			int amount = _KitStrength switch
			{
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			_ItemValue = amount * 10;
			_ChangeWeapon = kitCategory switch
			{
				ChangeWeapon.KitType.Grindstone => new ChangeWeapon(amount, ChangeWeapon.KitType.Grindstone),
				ChangeWeapon.KitType.Bowstring => new ChangeWeapon(amount, ChangeWeapon.KitType.Bowstring),
				_ => throw new ArgumentOutOfRangeException()
			};
			_Desc = $"A single-use {_Name} that increases weapon damage by {amount} for one weapon item.";
		}
	}
}
