namespace DungeonGame {
	public class ReflectDamage {
		public int ReflectAmount { get; set; }
		public int ReflectCurRounds { get; set; }
		public int ReflectMaxRounds { get; set; }
  
		public ReflectDamage(int reflectAmount, int reflectCurRounds, int reflectMaxRounds) {
			this.ReflectAmount = reflectAmount;
			this.ReflectCurRounds = reflectCurRounds;
			this.ReflectMaxRounds = reflectMaxRounds;
		}
	}
}