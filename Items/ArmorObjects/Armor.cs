using DungeonGame.Helpers;
using DungeonGame.Items.Equipment;
using DungeonGame.Players;
using System;
using System.Text;

namespace DungeonGame.Items.ArmorObjects {
	public class Armor : IItem, IRainbowGear, IEquipment {
		public string Name { get; set; }
		public string Desc { get; set; }
		public ArmorSlot ArmorCategory { get; set; }
		public ArmorType ArmorGroup { get; set; }
		public int ItemValue { get; set; }
		public int ArmorRating { get; set; }
		public bool Equipped { get; set; }
		public int Durability { get; set; }
		public int Level { get; set; }
		public int Weight { get; set; }
		public bool IsRainbowGear { get; set; }

		// Default constructor for JSON serialization
		public Armor() { }
		// Constructor to randomly create armor for monsters
		public Armor(int level, ArmorSlot armorCategory) {
			Level = level;
			int randomArmorNum = GameHelper.GetRandomNumber(1, 3);
			ArmorGroup = randomArmorNum switch {
				1 => ArmorType.Cloth,
				2 => ArmorType.Leather,
				3 => ArmorType.Plate,
				_ => throw new ArgumentOutOfRangeException()
			};
			ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			ArmorRating = ArmorCategory switch {
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
			ArmorRating += ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			ArmorRating += GameHelper.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			ArmorRating += level - 1;
			ItemValue = ArmorRating;
			Durability = 100;
			BuildArmorName();
			SetArmorWeight();
			Desc = $"A {Name}.";
		}
		// Constructor to define specific armor slot for players, vendors
		public Armor(int level, ArmorType armorGroup, ArmorSlot armorCategory) {
			Level = level;
			ArmorGroup = armorGroup;
			ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			ArmorRating = ArmorCategory switch {
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
			ArmorRating += ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			ArmorRating += GameHelper.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			ArmorRating += (level - 1) * 3;
			ItemValue = ArmorRating;
			Durability = 100;
			BuildArmorName();
			SetArmorWeight();
			Desc = $"A {Name}.";
		}
		public Armor(ArmorType armorGroup, ArmorSlot armorCategory, bool isRainbowGear, Player player) {
			Level = player.Level;
			IsRainbowGear = isRainbowGear;
			ArmorGroup = armorGroup;
			ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			ArmorRating = ArmorCategory switch {
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
			ArmorRating += ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			ArmorRating += GameHelper.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			ArmorRating += (Level - 1) * 3;
			// Add modifier for rainbow gear to enhance armor rating
			ArmorRating += 5;
			ItemValue = ArmorRating;
			Durability = 100;
			BuildArmorName("rainbow");
			SetArmorWeight();
			Desc = $"A {Name}.";
		}

		private void SetArmorWeight() {
			switch (ArmorGroup) {
				case ArmorType.Cloth:
					if (ArmorCategory == ArmorSlot.Chest || ArmorCategory == ArmorSlot.Legs) {
						Weight = 2;
					}
					Weight = 1;
					break;
				case ArmorType.Leather:
					Weight = ArmorCategory switch {
						ArmorSlot.Chest => 3,
						ArmorSlot.Legs => 2,
						_ => 1
					};
					break;
				case ArmorType.Plate:
					Weight = ArmorCategory switch {
						ArmorSlot.Chest => 4,
						ArmorSlot.Legs => 3,
						_ => 2
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void BuildArmorName() {
			StringBuilder sb = new StringBuilder();
			switch (ArmorGroup) {
				case ArmorType.Cloth:
					sb.Append(Level switch {
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
					switch (ArmorCategory) {
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
					sb.Append(Level switch {
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
					switch (ArmorCategory) {
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
					sb.Append(Level switch {
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
					switch (ArmorCategory) {
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
			Name = sb.ToString();
		}
		private void BuildArmorName(string rainbowName) {
			StringBuilder sb = new StringBuilder();
			sb.Append(rainbowName + " ");
			switch (ArmorGroup) {
				case ArmorType.Cloth:
					switch (ArmorCategory) {
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
					switch (ArmorCategory) {
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
					switch (ArmorCategory) {
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
			Name = sb.ToString();
		}
		public void DecreaseDurability() {
			Durability -= 1;
		}
		public float GetArmorRating() {
			float adjArmorRating = ArmorRating * (Durability / 100f);
			return adjArmorRating;
		}
		public void UpdateRainbowStats(Player player) {
			Level = player.Level;
			// Base armor rating before random attribute or armor type
			ArmorRating = ArmorCategory switch {
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
			ArmorRating += ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			ArmorRating += GameHelper.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			ArmorRating += (Level - 1) * 3;
			// Add modifier for rainbow gear to enhance armor rating
			ArmorRating += 5;
			ItemValue = ArmorRating;
		}
	}
}