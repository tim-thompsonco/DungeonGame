using System;
using System.Collections.Generic;

namespace DungeonGame {
  class MainClass {
    public static void Main(string[] args) {
      // Establish game objects and game intro
      Console.ForegroundColor = ConsoleColor.Gray;
      Helper.GameIntro();
      // Game loading commands
      var player = new NewPlayer(Helper.FetchPlayerName());
      var room100 = new DungeonRoom(
        "A dark room",
        "You are in a dimly lit room. There is a lantern burning on a hook on the opposite wall. Water drips " +
				"from a few stalagmites hanging from the ceiling, which is about 12 feet high. You appear to be in a " +
				"dungeon. A moaning wail echoes in the distance through an open doorway in front of you. Glinting " +
				"red eyes seem to be visible in the distance. There is an unsettling sound of a heavy metal object " +
				"being dragged on the ground by a faint shape beyond the doorway. You can't make out what it is.",
        100100100,
        true,
        false); // Starting room
      var room101 = new DungeonRoom(
        "Dimly lit platform",
        "Some other room...to be continued",
        100101100,
        new Monster("A rotting zombie", 25, 160, 1000, new Weapon("A notched axe", 10, 1.2)),
        false,
        true); // Second room
      var spawnedRooms = new List<IRoom> {
        room100,
        room101
      };
      // Set initial room condition
      var roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0);
      spawnedRooms[roomIndex].LookRoom();
      // While loop to continue obtaining input from player
      while (true) {
        player.DisplayPlayerStats();
        spawnedRooms[roomIndex].ShowCommands();
        // On loading game, display room that player starts in
        // Begin game by putting player in room 100
        var input = Helper.GetFormattedInput();
        // Obtain player command and process command
        switch (input) {
          case "f":
            spawnedRooms[roomIndex].MonsterFight(player);
            break;
          case "i":
            player.ShowInventory(player);
            break;
          case "q":
            Helper.GameOver();
            return;
          case "l":
            spawnedRooms[roomIndex].LookRoom();
            break;
          case "lc":
            spawnedRooms[roomIndex].LootCorpse(player);
            break;
          case "n":
            if(spawnedRooms[roomIndex].goNorth) {
							try {
                roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1000);
                spawnedRooms[roomIndex].LookRoom();
              }
							catch(ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "s":
						if(spawnedRooms[roomIndex].goSouth) {
							try {
                roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1000);
                spawnedRooms[roomIndex].LookRoom();
              }
							catch(ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
          default:
            break;
        }
      }
    }
  }
}