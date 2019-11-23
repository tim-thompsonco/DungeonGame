using System;

namespace DungeonGame
{
	class AxeWeapon
	{
    private readonly Random rndUse = new Random();
    private string Name { get; } = "An axe.";
    private int SwingDamage { get; } = 25;
		private int SlashDamage { get; } = 35;

		public int Attack()
		{
			var attackDamage = 0;
			var attackType = rndUse.Next(1, 12); // Creates a random number to determine attack type
      // Main attack
			if (attackType < 6)
			{
				attackDamage = this.SwingAxe();
			}
      // Stronger attack
			else if (attackType < 11) {
				attackDamage = this.SlashAxe();
			}
      // If RNG didn't cause main or stronger attack, it's a miss
			return attackDamage;
		}
		private int SwingAxe() {
			return this.SwingDamage;
		}
		private int SlashAxe() {
			return this.SlashDamage;
		}
	}
}