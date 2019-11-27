using System;

namespace DungeonGame {
	public class MainWeapon : IRoomInteraction {
    private readonly Random rndUse = new Random();
    public string Name { get; } = "An axe.";
    public int SwingDamage { get; } = 25;
		public int SlashDamage { get; } = 35;

		public int Attack() {
			var attackDamage = 0;
			var attackType = rndUse.Next(1, 12); // Creates a random number to determine attack type
      // Main attack
			if (attackType < 6)
			{
				attackDamage = this.SwingDamage;
			}
      // Stronger attack
			else if (attackType < 11) {
				attackDamage = this.SlashDamage;
			}
      // If RNG didn't cause main or stronger attack, it's a miss
			return attackDamage;
		}
    public string GetName() {
      return this.Name;
    }
  }
}