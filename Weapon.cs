using System;

namespace DungeonGame {
	public class Weapon : IRoomInteraction {
    private readonly Random RndGenerate = new Random();
    public string Name { get; }
    public int RegDamage { get; }
		public int ItemValue { get; }
		public double CritMultiplier { get; }

		public Weapon(string name, int regDamage, int itemValue, double critMultiplier) {
			this.Name = name;
			this.RegDamage = regDamage;
			this.ItemValue = itemValue;
			this.CritMultiplier = critMultiplier;
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
  }
}