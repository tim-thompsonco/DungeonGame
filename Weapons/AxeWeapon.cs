using System;

namespace DungeonGame
{
	class AxeWeapon
	{
    private readonly Random rndUse = new Random();
    private int SwingDamage { get; } = 25;
		private int SlashDamage { get; } = 35;

		public int Attack()
		{
			var attackDamage = 0;
			var attackType = rndUse.Next(1, 12); // Creates a random number to determine attack type
			if (attackType < 6)
			{
				attackDamage = this.SwingAxe();
			}
			else if (attackType < 10) {
				attackDamage = this.SlashAxe();
			}
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