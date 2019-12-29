using System;

namespace DungeonGame {
	public class Weapon : IEquipment {
		public enum WeaponType {
			Dagger,
			OneHandedSword,
			TwoHandedSword,
			Axe,
			Bow
		}
		private readonly Random _rndGenerate = new Random();
		public string Name { get; set; }
		public int RegDamage { get; set; }
		public int ItemValue { get; set; }
		public WeaponType WeaponGroup { get; set; }
		public double CritMultiplier { get; set; }
		public bool Equipped { get; set; }
		public int Durability { get; set; }

		public Weapon(
			string name, 
			int regDamageLow, 
			int regDamageHigh, 
			int itemValue, 
			double critMultiplier,
			bool equipped,
			WeaponType weaponGroup) {
			this.Name = name;
			this.RegDamage = this._rndGenerate.Next(regDamageLow, regDamageHigh);
			this.ItemValue = itemValue;
			this.CritMultiplier = critMultiplier;
			this.Equipped = equipped;
			this.Durability = 100;
			this.WeaponGroup = weaponGroup;
		}

		public int Attack() {
			if (!this.Equipped) return 0;
			var attackDamage = 0f;
			var attackType = this._rndGenerate.Next(1, 12); // Creates a random number to determine attack type
			// Main attack
			if (attackType < 6) {
				attackDamage = this.RegDamage;
			}
			// Stronger attack
			else if (attackType < 11) {
				attackDamage = (int)((double)this.RegDamage * this.CritMultiplier);
			}
			// If RNG didn't cause main or stronger attack, it's a miss
			this.Durability -= 1;
			attackDamage *= this.Durability / 100f;
			return (int)attackDamage;
		}
		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
	}
}