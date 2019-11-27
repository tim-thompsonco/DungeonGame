using System;

namespace DungeonGame {
  public class Leg_Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string name { get; } = "Some legplates.";
    public int armorRating = rndGenerate.Next(3, 8);

    public string GetName() {
      return this.name;
    }
  }
}