using System;

namespace DungeonGame {
  public class Chest_Armor : IRoomInteraction {
    private static readonly Random rndGenerate = new Random();
    public string name { get; } = "A chestplate.";
    public int armorRating { get; } = rndGenerate.Next(5, 15);

    public string GetName() {
      return this.name;
    }
  }
}