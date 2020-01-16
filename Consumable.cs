﻿using System;

namespace DungeonGame {
	public class Consumable : IEquipment {
		public enum ArrowType {
			Standard
		}
		public enum PotionType {
			Health,
			Mana
		}
		public enum PotionLevel {
			Minor,
			Normal,
			Greater
		}
		public enum GemType {
			Ruby,
			Emerald,
			Diamond,
			Sapphire,
			Amethyst,
			Topaz
		}
		public enum GemLevel {
			Chipped,
			Dull,
			Normal
		}
		public string Name { get; set; }
		public int ItemValue { get; set; }
		public bool Equipped { get; set; }
		public ArrowType ArrowCategory { get; set; }
		public PotionType PotionCategory { get; set; }
		public PotionLevel PotionStrength { get; set; }
		public GemType GemCategory { get; set; }
		public GemLevel GemStrength { get; set; }
		public RestoreHealth RestoreHealth { get; set; }
		public RestoreMana RestoreMana { get; set; }
		public Arrow Arrow { get; set; }
		public int Weight { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() {}
		public Consumable(int level, PotionType potionType) {
			this.PotionCategory = potionType;
			this.ItemValue = level * 3;
			this.Weight = 1;
			int amount;
			string name;
			if (level <= 3) {
				this.PotionStrength = PotionLevel.Minor;
				name = PotionLevel.Minor + " " + potionType + " potion";
				amount = 50;
			}
			else if (level > 6) {
				this.PotionStrength = PotionLevel.Greater;
				name = PotionLevel.Greater + " " + potionType + " potion";
				amount = 150;
			}
			else {
				this.PotionStrength = PotionLevel.Normal;
				name = potionType + " potion";
				amount = 100;
			}
			this.Name = name.ToLower();
			switch (this.PotionCategory) {
				case PotionType.Health:
					this.RestoreHealth = new RestoreHealth(amount);
					break;
				case PotionType.Mana:
					this.RestoreMana = new RestoreMana(amount);
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
		public Consumable(int level, GemType gemType) {
			this.GemCategory = gemType;
			this.Weight = 1;
			this.ItemValue = level * 20;
			string name;
			if (level <= 3) {
				this.GemStrength = GemLevel.Chipped;
				name = GemLevel.Chipped + " " + gemType;
			}
			else if (level <= 6) {
				this.GemStrength = GemLevel.Dull;
				name = GemLevel.Dull + " " + gemType;
			}
			else {
				this.GemStrength = GemLevel.Normal;
				name = gemType.ToString();
			}
			this.Name = name.ToLower();
		}

		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}
