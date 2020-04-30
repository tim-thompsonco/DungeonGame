using System;
using System.Text;

namespace DungeonGame {
	public class Armor : IEquipment {
		public enum ArmorSlot {
			Head,
			Back,
			Chest,
			Wrist,
			Hands,
			Waist,
			Legs
		}
		public enum ArmorType {
			Cloth,
			Leather,
			Plate,
			Rainbow
		}
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

		// Default constructor for JSON serialization
		public Armor() {}
		// Constructor to randomly create armor for monsters
		public Armor(int level, ArmorSlot armorCategory) {
			this.Level = level;
			var randomArmorNum = GameHandler.GetRandomNumber(1, 3);
			this.ArmorGroup = randomArmorNum switch {
				1 => ArmorType.Cloth,
				2 => ArmorType.Leather,
				3 => ArmorType.Plate,
				_ => throw new ArgumentOutOfRangeException()
			};
			this.ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			this.ArmorRating = this.ArmorCategory switch {
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
			this.ArmorRating += this.ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			this.ArmorRating += GameHandler.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			this.ArmorRating += level - 1;
			this.ItemValue = this.ArmorRating;
			this.Durability = 100;
			this.BuildArmorName();
			this.SetArmorWeight();
			this.Desc = "A " + this.Name + ".";
		}
		// Constructor to define specific armor slot for players, vendors
		public Armor(int level, ArmorType armorGroup, ArmorSlot armorCategory) {
			this.Level = level;
			this.ArmorGroup = armorGroup;
			this.ArmorCategory = armorCategory;
			// Base armor rating before random attribute or armor type
			this.ArmorRating = this.ArmorCategory switch {
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
			this.ArmorRating += this.ArmorGroup switch {
				ArmorType.Cloth => 0,
				ArmorType.Leather => 2,
				ArmorType.Plate => 4,
				_ => throw new ArgumentOutOfRangeException()
			};
			// Add random attribute to armor rating
			this.ArmorRating += GameHandler.GetRandomNumber(2, 4);
			// Add level adjustment to armor rating
			this.ArmorRating += (level - 1) * 3;
			this.ItemValue = this.ArmorRating;
			this.Durability = 100;
			this.BuildArmorName();
			this.SetArmorWeight();
			this.Desc = "A " + this.Name + ".";
		}

		private void SetArmorWeight() {
			switch (this.ArmorGroup) {
				case ArmorType.Cloth:
					if (this.ArmorCategory == ArmorSlot.Chest || this.ArmorCategory == ArmorSlot.Legs) {
						this.Weight = 2;
					}
					this.Weight = 1;
					break;
				case ArmorType.Leather:
					this.Weight = this.ArmorCategory switch {
						ArmorSlot.Chest => 3,
						ArmorSlot.Legs => 2,
						_ => 1
					};
					break;
				case ArmorType.Plate:
					this.Weight = this.ArmorCategory switch {
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
			var sb = new StringBuilder();
			switch (this.ArmorGroup) {
				case ArmorType.Cloth:
					sb.Append(this.Level switch {
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
					switch (this.ArmorCategory) {
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
					sb.Append(this.Level switch {
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
					switch (this.ArmorCategory) {
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
					sb.Append(this.Level switch {
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
					switch (this.ArmorCategory) {
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
			this.Name = sb.ToString();
		}
		public void DecreaseDurability() {
			this.Durability -= 1;
		}
		public float GetArmorRating() {
			var adjArmorRating = this.ArmorRating * (this.Durability / 100f);
			return adjArmorRating;
		}
	}
}