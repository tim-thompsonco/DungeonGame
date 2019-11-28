using System;

namespace DungeonGame {
  public class Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string name { get; }
    public int armorRating { get; }

		public Armor(string name, int armorRatingLow, int armorRatingHigh) {
			this.name = name;
			this.armorRating = rndGenerate.Next(armorRatingLow, armorRatingHigh);
		}

    public string GetName() {
      return this.name;
    }
  }
}