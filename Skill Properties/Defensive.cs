namespace DungeonGame {
	public class Defensive {
		public int BlockDamage { get; set; }

		// Default constructor for JSON serialization
		public Defensive() { }
		public Defensive(int blockDamage) {
			BlockDamage = blockDamage;
		}
	}
}