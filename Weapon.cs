using System;

namespace DungeonGame {
	public class Weapon : IRoomInteraction {
    private readonly Random rndUse = new Random();
    public string name { get; } = "An axe.";
    public int swingDamage { get; } = 25;
		public int slashDamage { get; } = 35;

		public int Attack() {
			var attackDamage = 0;
			var attackType = rndUse.Next(1, 12); // Creates a random number to determine attack type
      // Main attack
			if (attackType < 6)
			{
				attackDamage = this.swingDamage;
			}
      // Stronger attack
			else if (attackType < 11) {
				attackDamage = this.slashDamage;
			}
      // If RNG didn't cause main or stronger attack, it's a miss
			return attackDamage;
		}
    public string GetName() {
      return this.name;
    }
  }
}