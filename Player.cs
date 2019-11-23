using System;

namespace DungeonGame
{
	class Player
	{
		private int HitPoints { get; set; } = 100;
		private int ManaPoints { get; set; } = 100;
		private readonly AxeWeapon Weapon = new AxeWeapon();

		public void TakeDamage(int weaponDamage)
		{
			this.HitPoints -= weaponDamage;
		}
		public int CheckHealth()
		{
			return this.HitPoints;
		}
		public int Attack()
		{
			return Weapon.Attack();
		}
		public void Combat(Monster opponent)
		{
			do
			{
				Console.WriteLine("HP: {0} / 100 - MP: {1} / 100", this.HitPoints, this.ManaPoints);
				Console.WriteLine("Opponent HP: {0} / 100", opponent.CheckHealth());
				Console.WriteLine("Commands: 'Attack'");
				var input = Console.ReadLine();
				var inputL = input.ToLower();
				if (inputL == "attack")
				{
					var attackDamage = this.Attack();
					if (attackDamage == 0)
					{
						Console.WriteLine("You missed!");
					}
					else
					{
						Console.WriteLine("You hit the monster for {0} damage.", attackDamage);
						opponent.TakeDamage(attackDamage);
					}
					if (opponent.CheckHealth() <= 0)
					{
						Console.WriteLine("You have defeated the monster!");
						break;
					}
					var attackDamageM = opponent.Attack();
					if (attackDamageM == 0)
					{
						Console.WriteLine("They missed!");
					}
					else
					{
						Console.WriteLine("The monster hits you for {0} damage.", attackDamageM);
						this.TakeDamage(attackDamageM);
					}
				}
			} while (this.HitPoints > 0 || opponent.CheckHealth() > 0);
		}
	}
}