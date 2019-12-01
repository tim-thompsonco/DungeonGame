using System;

namespace DungeonGame {
  public class Armor : IRoomInteraction {
    private static readonly Random RndGenerate = new Random();
    public string Name { get; }
    public int ArmorRating { get; }

		public Armor(string name, int armorRatingLow, int armorRatingHigh) {
			this.Name = name;
			this.ArmorRating = RndGenerate.Next(armorRatingLow, armorRatingHigh);
		}

    public string GetName() {
      return this.Name;
    }
  }
}