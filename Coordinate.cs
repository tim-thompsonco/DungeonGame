using System;

namespace DungeonGame {
	public class Coordinate : IEquatable<Coordinate> {
		public int X { get; }
		public int Y { get; }
		public int Z { get; }

		public Coordinate(int x, int y, int z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		public override int GetHashCode() {
			return (this.X + 100) ^ (this.Y + 100) ^ (this.Z + 100);
		}
		public override bool Equals(object obj) {
			return this.Equals(obj as Coordinate);
		}
		public bool Equals(Coordinate obj) {
			return obj != null && obj.X == this.X && obj.Y == this.Y && obj.Z == this.Z;
		}
	}
}