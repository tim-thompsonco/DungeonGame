using System;

namespace DungeonGame.Items.Consumables
{
	public class Potion : Consumable
	{
		public enum PotionType
		{
			Health,
			Mana,
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public enum PotionStrength
		{
			Minor,
			Normal,
			Greater
		}
		public PotionType _PotionCategory { get; set; }
		public PotionStrength _PotionStrength { get; set; }
		public RestoreHealth _RestoreHealth { get; set; }
		public RestoreMana _RestoreMana { get; set; }
		public ChangeStat _ChangeStat { get; set; }
		private readonly int _PotionAmount;
		
		public Potion(int level, PotionType potionType) : base()
		{
			_Weight = 1;
			_PotionCategory = potionType;
			_PotionStrength = GetPotionStrength(level);
			_Name = GetPotionName(level);
			_PotionAmount = GetPotionAmount(level);
			_ItemValue = GetPotionItemValue();
			_Desc = GetPotionDesc();
			SetPotionEffect();
		}

		private string GetPotionName(int level)
		{
			// Potion naming format is "<potion strength> <potion type> potion" for lvl 1-3 or 7+ potions
			if (level <= 3)
			{
				return $"{PotionStrength.Minor.ToString().ToLower()} {_PotionCategory.ToString().ToLower()} potion";
			}
			else if (level > 6)
			{
				return $"{PotionStrength.Greater.ToString().ToLower()} {_PotionCategory.ToString().ToLower()} potion";
			}
			// Potion naming format is "<potion type> potion" for lvl 4-6 potions
			else
			{
				return $"{_PotionCategory.ToString().ToLower()} potion";
			}
		}

		private PotionStrength GetPotionStrength(int level)
		{
			if (level <= 3)
			{
				return PotionStrength.Minor;
			}
			else if (level > 6)
			{
				return PotionStrength.Greater;
			}
			else
			{
				return PotionStrength.Normal;
			}
		}

		private int GetPotionAmount(int level)
		{
			if (_PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana)
			{
				return GetHealthOrManaPotionAmount(level);
			}
			else
			{
				return GetStatPotionAmount(level);
			}
		}

		private int GetHealthOrManaPotionAmount(int level)
		{
			if (level <= 3)
			{
				return 50;
			}
			else if (level > 6)
			{
				return 150;
			}
			else
			{
				return 100;
			}
		}

		private int GetStatPotionAmount(int level)
		{
			if (level <= 3)
			{
				return 5;
			}
			else if (level > 6)
			{
				return 15;
			}
			else
			{
				return 10;
			}
		}

		private int GetPotionItemValue()
		{
			if (_PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana)
			{
				return _PotionAmount / 2;
			}
			else
			{
				return _PotionAmount * 10 / 2;
			}
		}

		private string GetPotionDesc()
		{
			if (_PotionCategory == PotionType.Health || _PotionCategory == PotionType.Mana)
			{
				return $"A {_Name} that restores {_PotionAmount} {_PotionCategory.ToString().ToLower()}.";
			}
			else
			{
				return $"A {_Name} that increases {_PotionAmount} {_PotionCategory.ToString().ToLower()}.";
			}
		}

		private void SetPotionEffect()
		{
			switch (_PotionCategory)
			{
				case PotionType.Health:
					_RestoreHealth = new RestoreHealth(_PotionAmount);
					break;
				case PotionType.Mana:
					_RestoreMana = new RestoreMana(_PotionAmount);
					break;
				case PotionType.Intelligence:
					_ChangeStat = new ChangeStat(_PotionAmount, ChangeStat.StatType.Intelligence);
					break;
				case PotionType.Strength:
					_ChangeStat = new ChangeStat(_PotionAmount, ChangeStat.StatType.Strength);
					break;
				case PotionType.Dexterity:
					_ChangeStat = new ChangeStat(_PotionAmount, ChangeStat.StatType.Dexterity);
					break;
				case PotionType.Constitution:
					_ChangeStat = new ChangeStat(_PotionAmount, ChangeStat.StatType.Constitution);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_PotionCategory), _PotionCategory, null);
			}
		}
	}
}