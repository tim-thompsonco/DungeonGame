using System;

namespace DungeonGame {
  class Chest_Armor {
    private static readonly Random rndGenerate = new Random();
    private int ArmorRating = rndGenerate.Next(5, 15);

    public int GetArmorRating() {
      return ArmorRating;
    }
  }
}