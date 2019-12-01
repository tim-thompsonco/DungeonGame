﻿using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class DungeonRoom : IRoom {
    public bool GoNorth { get; set; }
    public bool GoSouth { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
    public String[] Commands { get; set; } = new string[5] {
      "Check [I]nventory",
      "[L]ook",
      "[L]oot [C]orpse",
      "[F]ight",
      "[Q]uit"};
    // List of objects in room (including monsters)
    private readonly List<IRoomInteraction> RoomObjects = new List<IRoomInteraction>();
    public IMonster Monster;

    //Constructor without monster
    public DungeonRoom (
      string name,
      string desc,
			int x,
			int y,
			int z,
      bool goNorth,
      bool goSouth
      ) {
      this.Name = name;
      this.Desc = desc;
      this.X = x;
			this.Y = y;
			this.Z = z;
      this.GoNorth = goNorth;
      this.GoSouth = goSouth;
    }
		//Constructor with monster
		public DungeonRoom(
			string name,
			string desc,
			int x,
			int y,
			int z,
			IMonster monster,
			bool goNorth,
			bool goSouth
			) {
			this.Name = name;
			this.Desc = desc;
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Monster = monster;
			this.GoNorth = goNorth;
			this.GoSouth = goSouth;
			}

		// Implement method from IRoom
		public void MonsterFight(NewPlayer player) {
      if (this.Monster.HitPoints > 0) {
        var fightEvent = new CombatHelper();
        var outcome = fightEvent.SingleCombat(Monster, player);
        if (outcome == false) {
          Helper.PlayerDeath();
        }
      }
      else {
        Console.WriteLine("The {0} is already dead.", this.Monster.Name);
      }
    }
    // Implement method from IRoom
    public void RebuildRoomObjects() {
			try {
				RoomObjects.Clear();
				if (this.Monster.HitPoints > 0) {
					RoomObjects.Add((DungeonGame.IRoomInteraction)Monster);
				}
			}
			catch(NullReferenceException) {
			}
    }
    // Implement method from IRoom
    public void ShowCommands() {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.Write("Available Commands: ");
      Console.WriteLine(String.Join(", ", this.Commands));
    }
    // Implement method from IRoom
    public void ShowDirections() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("Available Directions: ");
      Console.ForegroundColor = ConsoleColor.White;
      if (this.GoNorth) {
        Console.Write("North");
      }
      if (this.GoSouth) {
        Console.Write("South");
      }
      Console.WriteLine();
    }
    // Implement method from IRoom
    public void LookRoom() {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.Name);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.Desc);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("Room Contents: ");
			Console.ForegroundColor = ConsoleColor.White;
			this.RebuildRoomObjects();
			if(RoomObjects.Count > 0) {
				foreach (IRoomInteraction item in RoomObjects) {
					Console.Write(string.Join(", ", item.GetName()));
				}
			}
			else {
				Console.Write("There is nothing in the room");
			}
			Console.WriteLine("."); // Add period at end of list of objects in room
      this.ShowDirections();
    }
		// Implement method from IRoom
		public void LootCorpse(NewPlayer player) {
			if (Monster.HitPoints <= 0 && Monster.WasLooted == false) {
				var goldLooted = Monster.Gold;
				player.Gold += Monster.Gold;
				Monster.Gold = 0;
				Monster.WasLooted = true;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("You looted {0} gold coins from the {1}!", goldLooted, this.Monster.Name);
			}
			else if (Monster.WasLooted) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("You already looted {0}!", this.Monster.Name);
			}
			else {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("You cannot loot something that isn't dead!");
			}
		}
	}
}