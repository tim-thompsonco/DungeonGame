using System;

namespace DungeonGame {
  class Head_Armor {
    private static readonly Random rndGenerate = new Random();
    private string Name { get; } = "A helmet.";
    private int ArmorRating = rndGenerate.Next(1, 5);

    public int GetArmorRating() {
      return ArmorRating;
    }
  }
}