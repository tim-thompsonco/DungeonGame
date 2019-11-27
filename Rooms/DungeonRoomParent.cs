using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class DungeonRoom : IRoom {
		public bool GoEast { get; set; }
    public bool GoWest { get; set; }
    public bool GoNorth { get; set; }
    public bool GoSouth { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int LocationKey { get; set; }
    public String[] Commands { get; set; } = new string[5] {
      "Check [I]nventory",
      "[L]ook",
      "[L]oot [C]orpse",
      "[F]ight",
      "[Q]uit"};
    // List of objects in room (including monsters)
    private readonly List<IRoomInteraction> _RoomObjects = new List<IRoomInteraction>();
    public IMonster _monster;

    //Constructor
    protected DungeonRoom(string Name, string Desc) {
      this.Name = Name;
      this.Desc = Desc;
    }

    // Implement method from IRoom
    public virtual void MonsterFight(NewPlayer player) {
      if (this._monster.HitPoints > 0) {
        var fightEvent = new CombatHelper();
        var outcome = fightEvent.SingleCombat(_monster, player);
        if (outcome == false) {
          Helper.PlayerDeath();
        }
      }
      else {
        Console.WriteLine("The {0} is already dead.", this._monster.Name);
      }
    }
    // Implement method from IRoom
    public virtual void LootCorpse(NewPlayer player) {
      if (_monster.HitPoints <= 0 && _monster.WasLooted == false) {
        var goldLooted = _monster.Gold;
        player.Gold += _monster.Gold;
        _monster.Gold = 0;
        _monster.WasLooted = true;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You looted {0} gold coins from the {1}!", goldLooted, this._monster.Name);
      }
      else if (_monster.WasLooted) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You already looted {0}!", this._monster.Name);
      }
      else {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You cannot loot something that isn't dead!");
      }
    }
    // Implement method from IRoom
    public virtual void RebuildRoomObjects() {
      _RoomObjects.Clear();
      _RoomObjects.Add((DungeonGame.IRoomInteraction)_monster);
    }
    // Implement method from IRoom
    public virtual void ShowCommands() {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.Write("Available Commands: ");
      Console.WriteLine(String.Join(", ", this.Commands));
    }
    // Implement method from IRoom
    public virtual void ShowDirections() {
    }
    // Implement method from IRoom
    public virtual void LookRoom() {
      this.RebuildRoomObjects();
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
      foreach (IRoomInteraction item in _RoomObjects) {
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