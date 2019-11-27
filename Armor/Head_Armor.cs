using System;

namespace DungeonGame {
  public class Head_Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string name { get; } = "A helmet.";
    public int armorRating { get; } = rndGenerate.Next(1, 5);

    public string GetName() {
      return this.name;
    }
  }
}