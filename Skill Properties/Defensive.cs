namespace DungeonGame {
	public class Defensive {
		public int _BlockDamage { get; set; }

		// Default constructor for JSON serialization
		public Defensive() { }
		public Defensive(int blockDamage) {
			_BlockDamage = blockDamage;
		}
	}
}