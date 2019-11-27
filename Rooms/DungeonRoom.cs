using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class DungeonRoom : IRoom {
    public bool goNorth { get; set; }
    public bool goSouth { get; set; }
    public string name { get; set; }
    public string desc { get; set; }
    public int locationKey { get; set; }
    public String[] commands { get; set; } = new string[5] {
      "Check [I]nventory",
      "[L]ook",
      "[L]oot [C]orpse",
      "[F]ight",
      "[Q]uit"};
    // List of objects in room (including monsters)
    private readonly List<IRoomInteraction> _roomObjects = new List<IRoomInteraction>();
    public IMonster _monster;

    //Constructor
    public DungeonRoom (
      string name,
      string desc,
      int locationKey,
      IMonster monster,
      bool goNorth,
      bool goSouth
      ) {
      this.name = name;
      this.desc = desc;
      this.locationKey = locationKey;
      this._monster = monster;
      this.goNorth = goNorth;
      this.goSouth = goSouth;
    }

    // Implement method from IRoom
    public virtual void MonsterFight(NewPlayer player) {
      if (this._monster.hitPoints > 0) {
        var fightEvent = new CombatHelper();
        var outcome = fightEvent.SingleCombat(_monster, player);
        if (outcome == false) {
          Helper.PlayerDeath();
        }
      }
      else {
        Console.WriteLine("The {0} is already dead.", this._monster.name);
      }
    }
    // Implement method from IRoom
    public virtual void LootCorpse(NewPlayer player) {
      if (_monster.hitPoints <= 0 && _monster.wasLooted == false) {
        var goldLooted = _monster.gold;
        player.gold += _monster.gold;
        _monster.gold = 0;
        _monster.wasLooted = true;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You looted {0} gold coins from the {1}!", goldLooted, this._monster.name);
      }
      else if (_monster.wasLooted) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You already looted {0}!", this._monster.name);
      }
      else {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You cannot loot something that isn't dead!");
      }
    }
    // Implement method from IRoom
    public virtual void RebuildRoomObjects() {
      _roomObjects.Clear();
      _roomObjects.Add((DungeonGame.IRoomInteraction)_monster);
    }
    // Implement method from IRoom
    public virtual void ShowCommands() {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.Write("Available Commands: ");
      Console.WriteLine(String.Join(", ", this.commands));
    }
    // Implement method from IRoom
    public virtual void ShowDirections() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("Available Directions: ");
      Console.ForegroundColor = ConsoleColor.White;
      if (this.goNorth) {
        Console.Write("North");
      }
      if (this.goSouth) {
        Console.Write("South");
      }
      Console.WriteLine();
    }
    // Implement method from IRoom
    public virtual void LookRoom() {
      this.RebuildRoomObjects();
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.name);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.desc);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      foreach (IRoomInteraction item in _roomObjects) {
        Console.Write("Room Contents: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("A ");
        Console.Write(string.Join(", ", item.GetName()));
      }
      Console.WriteLine("."); // Add period at end of list of objects in room
      this.ShowDirections();
    }
  }
}