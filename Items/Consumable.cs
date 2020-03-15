using System;

namespace DungeonGame {
	public class Consumable : IEquipment {
		public enum ArrowType {
			Standard
		}
		public enum PotionType {
			Health,
			Mana,
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		private enum PotionLevel {
			Minor,
			Normal,
			Greater
		}
		public enum KitLevel {
			Light,
			Medium,
			Heavy
		}
		public enum KitType {
			Armor,
			Weapon
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public int ItemValue { get; set; }
		public bool Equipped { get; set; }
		public ArrowType? ArrowCategory { get; set; }
		public PotionType? PotionCategory { get; set; }
		private PotionLevel? PotionStrength { get; set; }
		public KitLevel? KitStrength { get; set; }
		public KitType? KitCategory { get; set; }
		public RestoreHealth RestoreHealth { get; set; }
		public RestoreMana RestoreMana { get; set; }
		public ChangeStat ChangeStat { get; set; }
		public ChangeArmor ChangeArmor { get; set; }
		public ChangeWeapon ChangeWeapon { get; set; }
		public Arrow Arrow { get; set; }
		public int Weight { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() {}
		public Consumable(int level, PotionType potionType) {
			this.PotionCategory = potionType;
			this.Weight = 1;
			int amount;
			var name = string.Empty;
			if (level <= 3) {
				this.PotionStrength = PotionLevel.Minor;
				name = PotionLevel.Minor.ToString().ToLowerInvariant() + " " + 
				       potionType.ToString().ToLowerInvariant() + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 50 : 5;
			}
			else if (level > 6) {
				this.PotionStrength = PotionLevel.Greater;
				name = PotionLevel.Greater.ToString().ToLowerInvariant() + " " + 
				       potionType.ToString().ToLowerInvariant() + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 150 : 15;
			}
			else {
				this.PotionStrength = PotionLevel.Normal;
				name = potionType.ToString().ToLowerInvariant() + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 100 : 10;
			}
			this.ItemValue = this.PotionCategory == PotionType.Health || 
			                 this.PotionCategory == PotionType.Mana ? amount / 2 : amount * 10 / 2;
			this.Name = name;
			this.Desc = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana
				? "A " + name + " that restores " + amount + " " + this.PotionCategory.ToString().ToLowerInvariant() + "."
				: "A " + name + " that increases " + amount + " " + this.PotionCategory.ToString().ToLowerInvariant() + ".";
			switch (this.PotionCategory) {
				case PotionType.Health:
					this.RestoreHealth = new RestoreHealth(amount);
					break;
				case PotionType.Mana:
					this.RestoreMana = new RestoreMana(amount);
					break;
				case PotionType.Intelligence:
					this.ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Intelligence);
					break;
				case PotionType.Strength:
					this.ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Strength);
					break;
				case PotionType.Dexterity:
					this.ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Dexterity);
					break;
				case PotionType.Constitution:
					this.ChangeStat = new ChangeStat(amount, ChangeStat.StatType.Constitution);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(potionType), potionType, null);
			}
		}
		public Consumable(string name, int itemValue, ArrowType arrowType) {
			this.Name = name;
			this.Weight = 1;
			this.ItemValue = itemValue;
			this.ArrowCategory = arrowType;
			this.Arrow = new Arrow(50);
			this.Desc = "A bundle of " + this.Arrow.Quantity + " arrows.";
		}
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeArmor.KitType kitCategory) {
			this.KitCategory = kitType;
			this.Name = kitLevel.ToString().ToLowerInvariant() + " " + kitCategory.ToString().ToLowerInvariant() + " " +
			            kitType.ToString().ToLowerInvariant() + " kit";
			this.Weight = 1;
			this.KitStrength = kitLevel;
			var amount = this.KitStrength switch {
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			this.ItemValue = amount * 10;
			this.ChangeArmor = kitCategory switch {
				ChangeArmor.KitType.Cloth => new ChangeArmor(amount, ChangeArmor.KitType.Cloth),
				ChangeArmor.KitType.Leather => new ChangeArmor(amount, ChangeArmor.KitType.Leather),
				ChangeArmor.KitType.Plate => new ChangeArmor(amount, ChangeArmor.KitType.Plate),
				_ => throw new ArgumentOutOfRangeException()
			};
			this.Desc = "A single-use " + this.Name +  " that increases armor rating by " + amount + " for one armor item.";
		}
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeWeapon.KitType kitCategory) {
			this.KitCategory = kitType;
			this.Name = kitLevel.ToString().ToLowerInvariant() + " " + kitCategory.ToString().ToLowerInvariant() + " " +
			            kitType.ToString().ToLowerInvariant() + " kit";
			this.Weight = 1;
			this.KitStrength = kitLevel;
			var amount = this.KitStrength switch {
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			this.ItemValue = amount * 10;
			this.ChangeWeapon = kitCategory switch {
				ChangeWeapon.KitType.Grindstone => new ChangeWeapon(amount, ChangeWeapon.KitType.Grindstone),
				ChangeWeapon.KitType.Bowstring => new ChangeWeapon(amount, ChangeWeapon.KitType.Bowstring),
				_ => throw new ArgumentOutOfRangeException()
			};
			this.Desc = "A single-use " + this.Name + " that increases weapon damage by " + amount + " for one weapon item.";
		}
	}
}
