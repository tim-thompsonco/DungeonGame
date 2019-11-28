using System;

namespace DungeonGame {
	public class Weapon : IRoomInteraction {
    private readonly Random rndUse = new Random();
    public string name { get; }
    public int regDamage { get; }
		public double critMultiplier { get; }

		public Weapon(string name, int regDamage, double critMultiplier) {
			this.name = name;
			this.regDamage = regDamage;
			this.critMultiplier = critMultiplier;
		}

		public int Attack() {
			var attackDamage = 0;
			var attackType = rndUse.Next(1, 12); // Creates a random number to determine attack type
      // Main attack
			if (attackType < 6)
			{
				attackDamage = this.regDamage;
			}
      // Stronger attack
			else if (attackType < 11) {
				attackDamage = (int)((double)this.regDamage * this.critMultiplier);
			}
      // If RNG didn't cause main or stronger attack, it's a miss
			return attackDamage;
		}
    public string GetName() {
      return this.name;
    }
  }
}