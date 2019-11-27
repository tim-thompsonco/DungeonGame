//using System;
//using System.Collections.Generic;

//namespace DungeonGame {
//  public class Room101 : DungeonRoom, IRoom {
//    //Constructor
//    public Room101(string Name, string Desc, IMonster monster)
//      : base(Name, Desc) {
//      base.LocationKey = 101;
//      base._monster = monster;
//			base.GoSouth = true;
//    }
//		public override void ShowDirections() {
//      Console.ForegroundColor = ConsoleColor.DarkCyan;
//      Console.Write("Available Directions: ");
//      Console.ForegroundColor = ConsoleColor.White;
//      Console.WriteLine("South");
//    }
//  }
//}