using DungeonGame.Controllers;
using System;
using System.Text;

namespace DungeonGame.Items
{
	public class Armor : IEquipment, IRainbowGear
	{
		public enum ArmorSlot
		{
			Head,
			Back,
			Chest,
			Wrist,
			Hands,
			Waist,
			Legs
		}
		public enum ArmorType
		{
			Cloth,
			Leather,
			Plate
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public ArmorSlot _ArmorCategory { get; set; }
		public ArmorType _ArmorGroup { get; set; }
		public int _ItemValue { get; set; }
		public int _ArmorRating { get; set; }
		public bool _Equipped { get; set; }
		public int _Durability { get; set; }
		public int _Level { get; set; }
		public int _Weight { get; set; }
		public bool _IsRainbowGear { get; set; }

		// Default constructor for JSON serialization
		public Armor() { }
		// Constructor to randomly create armor for monsters
		public Armor(int level, ArmorSlot armorCategory)
		{
			_Level = level;
			int randomArmorNum = GameController.GetRandomNumber(1, 3);
			_ArmorGroup = randomArmorNum switch
			{
				1 => ArmorType.Cloth,
				2 => ArmorType.Leather,
				3 => ArmorType.Plate,
				_ => throw new ArgumentOutOfRangeException()
			};
			_ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			_ArmorRating = _ArmorCategory switch
			{
				ArmorSlot.Back => 1,
				ArmorSlot.Chest => 5,
				ArmorSlot.Head => 2,
				ArmorSlot.Legs => 3,
				ArmorSlot.Waist => 2,
				ArmorSlot.Wrist => 1,
				ArmorSlot.Hands => 1,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add armor type armor rating to base value
			_ArmorRating += _ArmorGroup switch
			{
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			_ArmorRating += GameController.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			_ArmorRating += level - 1;
			_ItemValue = _ArmorRating;
			_Durability = 100;
			BuildArmorName();
			SetArmorWeight();
			_Desc = $"A {_Name}.";
		}
		// Constructor to define specific armor slot for players, vendors
		public Armor(int level, ArmorType armorGroup, ArmorSlot armorCategory)
		{
			_Level = level;
			_ArmorGroup = armorGroup;
			_ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			_ArmorRating = _ArmorCategory switch
			{
				ArmorSlot.Back => 1,
				ArmorSlot.Chest => 5,
				ArmorSlot.Head => 2,
				ArmorSlot.Legs => 3,
				ArmorSlot.Waist => 2,
				ArmorSlot.Wrist => 1,
				ArmorSlot.Hands => 1,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add armor type armor rating to base value
			_ArmorRating += _ArmorGroup switch
			{
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			_ArmorRating += GameController.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			_ArmorRating += (level - 1) * 3;
			_ItemValue = _ArmorRating;
			_Durability = 100;
			BuildArmorName();
			SetArmorWeight();
			_Desc = $"A {_Name}.";
		}
		public Armor(ArmorType armorGroup, ArmorSlot armorCategory, bool isRainbowGear, Player player)
		{
			_Level = player._Level;
			_IsRainbowGear = isRainbowGear;
			_ArmorGroup = armorGroup;
			_ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			_ArmorRating = _ArmorCategory switch
			{
				ArmorSlot.Back => 1,
				ArmorSlot.Chest => 5,
				ArmorSlot.Head => 2,
				ArmorSlot.Legs => 3,
				ArmorSlot.Waist => 2,
				ArmorSlot.Wrist => 1,
				ArmorSlot.Hands => 1,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add armor type armor rating to base value
			_ArmorRating += _ArmorGroup switch
			{
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			_ArmorRating += GameController.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			_ArmorRating += (_Level - 1) * 3;
			// Add modifier for rainbow gear to enhance armor rating
			_ArmorRating += 5;
			_ItemValue = _ArmorRating;
			_Durability = 100;
			BuildArmorName("rainbow");
			SetArmorWeight();
			_Desc = $"A {_Name}.";
		}

		private void SetArmorWeight()
		{
			switch (_ArmorGroup)
			{
				case ArmorType.Cloth:
					if (_ArmorCategory == ArmorSlot.Chest || _ArmorCategory == ArmorSlot.Legs)
					{
						_Weight = 2;
					}
					_Weight = 1;
					break;
				case ArmorType.Leather:
					_Weight = _ArmorCategory switch
					{
						ArmorSlot.Chest => 3,
						ArmorSlot.Legs => 2,
						_ => 1
					};
					break;
				case ArmorType.Plate:
					_Weight = _ArmorCategory switch
					{
						ArmorSlot.Chest => 4,
						ArmorSlot.Legs => 3,
						_ => 2
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void BuildArmorName()
		{
			StringBuilder sb = new StringBuilder();
			switch (_ArmorGroup)
			{
				case ArmorType.Cloth:
					sb.Append(_Level switch
					{
						1 => "ripped cloth ",
						2 => "torn cloth ",
						3 => "tattered cloth ",
						4 => "frayed cloth ",
						5 => "worn cloth ",
						6 => "reinforced cloth ",
						7 => "fine cloth ",
						8 => "exceptional cloth ",
						9 => "magnificent cloth ",
						10 => "exquisite cloth ",
						_ => "cloth "
					});
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("cap");
							break;
						case ArmorSlot.Back:
							sb.Append("cape");
							break;
						case ArmorSlot.Chest:
							sb.Append("tunic");
							break;
						case ArmorSlot.Wrist:
							sb.Append("bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("gloves");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case ArmorType.Leather:
					sb.Append(_Level switch
					{
						1 => "ripped leather ",
						2 => "torn leather ",
						3 => "tattered leather ",
						4 => "frayed leather ",
						5 => "worn leather ",
						6 => "reinforced leather ",
						7 => "fine leather ",
						8 => "exceptional leather ",
						9 => "magnificent leather ",
						10 => "exquisite leather ",
						_ => "leather "
					});
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("helmet");
							break;
						case ArmorSlot.Back:
							sb.Append("cape");
							break;
						case ArmorSlot.Chest:
							sb.Append("vest");
							break;
						case ArmorSlot.Wrist:
							sb.Append("bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("gloves");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case ArmorType.Plate:
					sb.Append(_Level switch
					{
						1 => "cracked ",
						2 => "dented ",
						3 => "chipped ",
						4 => "dull ",
						5 => "polished ",
						6 => "shining ",
						7 => "lustrous ",
						8 => "exceptional ",
						9 => "magnificent ",
						10 => "exquisite ",
						_ => "plate "
					});
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("plate helmet");
							break;
						case ArmorSlot.Back:
							sb.Append("backplate");
							break;
						case ArmorSlot.Chest:
							sb.Append("chestplate");
							break;
						case ArmorSlot.Wrist:
							sb.Append("plate bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("plate belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("plate leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("plate gauntlets");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_Name = sb.ToString();
		}
		private void BuildArmorName(string rainbowName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(rainbowName + " ");
			switch (_ArmorGroup)
			{
				case ArmorType.Cloth:
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("cap");
							break;
						case ArmorSlot.Back:
							sb.Append("cape");
							break;
						case ArmorSlot.Chest:
							sb.Append("tunic");
							break;
						case ArmorSlot.Wrist:
							sb.Append("bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("gloves");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case ArmorType.Leather:
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("helmet");
							break;
						case ArmorSlot.Back:
							sb.Append("cape");
							break;
						case ArmorSlot.Chest:
							sb.Append("vest");
							break;
						case ArmorSlot.Wrist:
							sb.Append("bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("gloves");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case ArmorType.Plate:
					switch (_ArmorCategory)
					{
						case ArmorSlot.Head:
							sb.Append("plate helmet");
							break;
						case ArmorSlot.Back:
							sb.Append("backplate");
							break;
						case ArmorSlot.Chest:
							sb.Append("chestplate");
							break;
						case ArmorSlot.Wrist:
							sb.Append("plate bracers");
							break;
						case ArmorSlot.Waist:
							sb.Append("plate belt");
							break;
						case ArmorSlot.Legs:
							sb.Append("plate leggings");
							break;
						case ArmorSlot.Hands:
							sb.Append("plate gauntlets");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_Name = sb.ToString();
		}
		public void DecreaseDurability()
		{
			_Durability -= 1;
		}
		public float GetArmorRating()
		{
			float adjArmorRating = _ArmorRating * (_Durability / 100f);
			return adjArmorRating;
		}
		public void UpdateRainbowStats(Player player)
		{
			_Level = player._Level;
			// Base armor rating before random attribute or armor type
			_ArmorRating = _ArmorCategory switch
			{
				ArmorSlot.Back => 1,
				ArmorSlot.Chest => 5,
				ArmorSlot.Head => 2,
				ArmorSlot.Legs => 3,
				ArmorSlot.Waist => 2,
				ArmorSlot.Wrist => 1,
				ArmorSlot.Hands => 1,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add armor type armor rating to base value
			_ArmorRating += _ArmorGroup switch
			{
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			_ArmorRating += GameController.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			_ArmorRating += (_Level - 1) * 3;
			// Add modifier for rainbow gear to enhance armor rating
			_ArmorRating += 5;
			_ItemValue = _ArmorRating;
		}
	}
}