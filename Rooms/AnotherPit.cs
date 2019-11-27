using System;
using System.Collections.Generic;

namespace DungeonGame {
  public class DungeonRoom101 : Room {
    public String[] Commands { get; set; } = new string[5] {
      "Check [I]nventory",
      "[L]ook",
      "[L]oot [C]orpse",
      "[F]ight",
      "[Q]uit"};
    private Zombie monster1 = new Zombie("rotting zombie");
    // List of objects in room (including monsters)
    private List<RoomInteraction> RoomObjects = new List<RoomInteraction>();

    public DungeonRoom101(string Name, string Desc, int LocationKey) {
      this.Name = Name;
      this.Desc = Desc;
      this.LocationKey = LocationKey;
    }

    public void RebuildRoomObjects() {
      RoomObjects.Clear();
      RoomObjects.Add(((DungeonGame.RoomInteraction)monster1));
    }
    public void ShowCommands() {
      Console.Write("Available Commands: ");
      Console.WriteLine(String.Join(", ", this.Commands));
    }
    public void LookRoom() {
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
      foreach (RoomInteraction item in RoomObjects) {
        Console.Write("Room Contents: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("A ");
        Console.Write(string.Join(", ", item.GetName()));
      }
      Console.WriteLine("."); // Add period at end of list of objects in room
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("Available Directions: ");
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("North");
    }
    public void PlayerInRoom(NewPlayer player) {
      while (true) {
        player.DisplayPlayerStats();
        this.ShowCommands();
        var input = Helper.GetFormattedInput();
        switch (input) {
          case "f":
            if (this.monster1.HitPoints > 0) {
              var fightEvent = new CombatHelper();
              var outcome = fightEvent.SingleCombat(this.monster1, player);
              if (outcome == true) {
                break;
              }
              else {
                Helper.PlayerDeath();
                return;
              }
            }
            else {
              Console.WriteLine("The {0} is already dead.", this.monster1.Name);
              break;
            }
          case "i":
            player.ShowInventory(player);
            break;
          case "q":
            return;
          case "l":
            this.LookRoom();
            break;
          case "lc":
            if (monster1.HitPoints <= 0 && monster1.WasLooted == false) {
              var goldLooted = monster1.LootCorpse();
              player.Gold += goldLooted;
              monster1.WasLooted = true;
              Console.ForegroundColor = ConsoleColor.Blue;
              Console.WriteLine("You looted {0} gold coins from the {1}!", goldLooted, monster1.Name);
            }
            else if (monster1.WasLooted) {
              Console.ForegroundColor = ConsoleColor.Blue;
              Console.WriteLine("You already looted {0}!", monster1.Name);
            }
            else {
              Console.ForegroundColor = ConsoleColor.Blue;
              Console.WriteLine("You cannot loot something that isn't dead!");
            }
            break;
          case "n":

          default:
            break;
        }
      }
    }
  }
}