using System;

namespace DungeonGame {
  class Head_Armor {
    private static readonly Random rndGenerate = new Random();
    public string Name { get; } = "A helmet.";
    public int ArmorRating { get; } = rndGenerate.Next(1, 5);
  }
}