using System;

namespace DungeonGame {
  public class Chest_Armor {
    private static readonly Random rndGenerate = new Random();
    public string Name { get; } = "A chestplate.";
    public int ArmorRating { get; } = rndGenerate.Next(5, 15);
  }
}