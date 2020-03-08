using System;

namespace DungeonGame {
	public class MonsterSpell {
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning,
			Heal,
			Rejuvenate,
		}
		public string Name { get; set; }
		public SpellType SpellCategory { get; set; }
		public Offensive Offensive { get; set; }
		public Healing Healing { get; set; }
		public int EnergyCost { get; set; }

		public MonsterSpell(string name, int energyCost, SpellType spellType, int monsterLevel) {
			this.Name = name;
			this.EnergyCost = energyCost;
			this.SpellCategory = spellType;
			switch(this.SpellCategory) {
				case SpellType.Fireball:
					this.Offensive = new Offensive(
						25 + (monsterLevel - 1) * 10, 5 + (monsterLevel - 1) * 5, 
						1, 3, Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					this.Offensive = new Offensive(15 + (monsterLevel - 1) * 10, 1, 2);
					break;
				case SpellType.Lightning:
					this.Offensive = new Offensive(35 + (monsterLevel - 1) * 10);
					break;
				case SpellType.Heal:
					this.Healing = new Healing(50 + (monsterLevel - 1) * 10);
					break;
				case SpellType.Rejuvenate:
					this.Healing = new Healing(20 + (monsterLevel - 1) * 10, 
						10 + (monsterLevel - 1) * 5, 1, 3);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}