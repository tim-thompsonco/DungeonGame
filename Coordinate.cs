using System;
using System.ComponentModel;
using System.Globalization;

namespace DungeonGame {
	[TypeConverter(typeof(CoordinateConverter))]
	public class Coordinate : IEquatable<Coordinate> {
		public int _X { get; }
		public int _Y { get; }
		public int _Z { get; }

		public Coordinate(int x, int y, int z) {
			_X = x;
			_Y = y;
			_Z = z;
		}

		public override int GetHashCode() {
			return (_X + 100) ^ (_Y + 100) ^ (_Z + 100);
		}
		public override bool Equals(object obj) {
			return Equals(obj as Coordinate);
		}
		public bool Equals(Coordinate obj) {
			return obj != null && obj._X == _X && obj._Y == _Y && obj._Z == _Z;
		}
	}

	public class CoordinateConverter : TypeConverter {
		// Overrides the CanConvertFrom method of TypeConverter.
		// The ITypeDescriptorContext interface provides the context for the
		// conversion. Typically, this interface is used at design time to 
		// provide information about the design-time container.
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}
		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo culture, object value) {
			if (value is string str) {
				string[] v = str.Split(new char[] { ',' });
				return new Coordinate(int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]));
			}
			return base.ConvertFrom(context, culture, value);
		}
		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				return ((Coordinate)value)._X + "," + ((Coordinate)value)._Y + "," + ((Coordinate)value)._Z;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}