using Newtonsoft.Json;

namespace DungeonGame {
	public class Offensive {
		public int DamageAmount { get; set; }
		public int DamageOverTime { get; set; }
		public int DamageCurRounds { get; set; }
		public int DamageMaxRounds { get; set; }

		public Offensive(int damageAmount) {
			this.DamageAmount = damageAmount;
		}
		[JsonConstructor]
		public Offensive(int damageAmount, int damageOverTime, int damageCurRounds, int damageMaxRounds) {
			this.DamageAmount = damageAmount;
			this.DamageOverTime = damageOverTime;
			this.DamageCurRounds = damageCurRounds;
			this.DamageMaxRounds = damageMaxRounds;
		}
	}
}