using System;

namespace DungeonGame {
  public class Room100 : DungeonRoom, IRoom {
    //Constructor
    public Room100(string Name, string Desc, IMonster monster)
      : base(Name, Desc) {
      base.LocationKey = 100;
      base._monster = monster;
      base.GoNorth = true;
    }
    public override void ShowDirections() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("Available Directions: ");
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("North");
    }
  }
}