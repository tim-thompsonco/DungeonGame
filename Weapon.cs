using System;

namespace DungeonGame {
	public class Weapon : IEquipment {
    private readonly Random RndGenerate = new Random();
    public string Name { get; set; }
    public int RegDamage { get; set; }
		public int ItemValue { get; set; }
		public double CritMultiplier { get; set; }
		public bool Equipped { get; set; }

		public Weapon(string name, int regDamage, int itemValue, double critMultiplier, bool equipped) {
			this.Name = name;
			this.RegDamage = regDamage;
			this.ItemValue = itemValue;
			this.CritMultiplier = critMultiplier;
			this.Equipped = equipped;
		}

		public int Attack() {
			var attackDamage = 0;
			var attackType = RndGenerate.Next(1, 12); // Creates a random number to determine attack type
      // Main attack
			if (attackType < 6)
			{
				attackDamage = this.RegDamage;
			}
      // Stronger attack
			else if (attackType < 11) {
				attackDamage = (int)((double)this.RegDamage * this.CritMultiplier);
			}
      // If RNG didn't cause main or stronger attack, it's a miss
			return attackDamage;
		}
    public string GetName() {
      return this.Name;
    }
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}