using System;
using System.ComponentModel;

namespace DungeonGame.Coordinates {
	[TypeConverter(typeof(CoordinateConverter))]
	public class Coordinate : IEquatable<Coordinate> {
		public int X { get; }
		public int Y { get; }
		public int Z { get; }

		public Coordinate(int x, int y, int z) {
			X = x;
			Y = y;
			Z = z;
		}

		public override int GetHashCode() {
			return X + 100 ^ Y + 100 ^ Z + 100;
		}

		public override bool Equals(object obj) {
			return Equals(obj as Coordinate);
		}

		public bool Equals(Coordinate obj) {
			return obj != null && obj.X == X && obj.Y == Y && obj.Z == Z;
		}
	}
}