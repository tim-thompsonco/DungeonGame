using System;

namespace DungeonGame {
  class Leg_Armor {
    private static readonly Random rndGenerate = new Random();
    private string Name { get; } = "Some legplates.";
    private int ArmorRating = rndGenerate.Next(3, 8);

    public int GetArmorRating() {
      return ArmorRating;
    }
  }
}