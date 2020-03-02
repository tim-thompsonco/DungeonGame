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
		public int ItemValue { get; set; }
		public bool Equipped { get; set; }
		public ArrowType ArrowCategory { get; set; }
		public PotionType PotionCategory { get; set; }
		private PotionLevel PotionStrength { get; set; }
		public KitLevel KitStrength { get; set; }
		public KitType KitCategory { get; set; }
		public RestoreHealth RestoreHealth { get; set; }
		public RestoreMana RestoreMana { get; set; }
		public ChangeStat ChangeStat { get; set; }
		public ChangeArmor ChangeArmor { get; set; }
		public Arrow Arrow { get; set; }
		public int Weight { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() {}
		public Consumable(int level, PotionType potionType) {
			this.PotionCategory = potionType;
			this.Weight = 1;
			int amount;
			string name;
			if (level <= 3) {
				this.PotionStrength = PotionLevel.Minor;
				name = PotionLevel.Minor + " " + potionType + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 50 : 5;
			}
			else if (level > 6) {
				this.PotionStrength = PotionLevel.Greater;
				name = PotionLevel.Greater + " " + potionType + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 150 : 15;
			}
			else {
				this.PotionStrength = PotionLevel.Normal;
				name = potionType + " potion";
				amount = this.PotionCategory == PotionType.Health || this.PotionCategory == PotionType.Mana ? 100 : 10;
			}
			this.ItemValue = amount / 2;
			this.Name = name.ToLower();
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
		}
		public Consumable(KitLevel kitLevel, KitType kitType) {
			this.Name = kitLevel + " " + kitType + " kit";
			this.Weight = 1;
			this.KitStrength = kitLevel;
			var amount = this.KitStrength switch {
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			this.ItemValue = amount * 10;
			var randomNum = GameHandler.GetRandomNumber(1, 3);
			switch (this.KitCategory) {
				case KitType.Armor:
					this.ChangeArmor = new ChangeArmor(amount, randomNum switch {
						1 => ChangeArmor.KitType.Cloth,
						2 => ChangeArmor.KitType.Leather,
						3 => ChangeArmor.KitType.Plate,
						_ => throw new ArgumentOutOfRangeException()});
					break;
				case KitType.Weapon:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
