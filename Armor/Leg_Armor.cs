using System;

namespace DungeonGame {
  class Leg_Armor {
    private static readonly Random rndGenerate = new Random();
    public string Name { get; } = "Some legplates.";
    public int ArmorRating = rndGenerate.Next(3, 8);
  }
}