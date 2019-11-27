using System;

namespace DungeonGame {
  public class Leg_Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string Name { get; } = "Some legplates.";
    public int ArmorRating = rndGenerate.Next(3, 8);

    public string GetName() {
      return this.Name;
    }
  }
}