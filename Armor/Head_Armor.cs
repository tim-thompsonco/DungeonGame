using System;

namespace DungeonGame {
  public class Head_Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string Name { get; } = "A helmet.";
    public int ArmorRating { get; } = rndGenerate.Next(1, 5);

    public string GetName() {
      return this.Name;
    }
  }
}