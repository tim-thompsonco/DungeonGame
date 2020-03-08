using System;

namespace DungeonGame {
	public class MonsterAbility {
		public enum Ability {
			PoisonBite,
			BloodLeech
		}
		public string Name { get; set; }
		public Ability AbilityCategory { get; set; }
		public Offensive Offensive { get; set; }
		public int EnergyCost { get; set; }
		
		public MonsterAbility(string name, int energyCost, Ability abilityCategory, int monsterLevel) {
			this.Name = name;
			this.EnergyCost = energyCost;
			this.AbilityCategory = abilityCategory;
			switch (this.AbilityCategory) {
				case Ability.PoisonBite:
					this.Offensive = new Offensive(
						15 + (monsterLevel - 1) * 10, 5 + (monsterLevel - 1) * 5, 
						1, 3, Offensive.OffensiveType.Bleed);
					break;
				case Ability.BloodLeech:
					this.Offensive = new Offensive(10 + (monsterLevel - 1) * 10);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}