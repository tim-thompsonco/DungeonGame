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
        "The Pit",
        "A starting room and you werrrreee in the pit!",
        100100100,
        new Monster("rotting skeleton", 10, 80, 400, new Weapon()),
        true,
        false); // Starting room
      var room101 = new DungeonRoom(
        "Another pit",
        "Some other room innnn the piiiiiiit!",
        100101100,
        new Monster("rotting zombie", 25, 160, 1000, new Weapon()),
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
            Console.WriteLine("Game over.");
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
								Console.WriteLine("You can't go that way!");
							}
						}
						else {
							Console.WriteLine("You can't go that way!");
						}
						break;
					case "s":
						if(spawnedRooms[roomIndex].goSouth) {
							try {
                roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1000);
                spawnedRooms[roomIndex].LookRoom();
              }
							catch(ArgumentOutOfRangeException) {
								Console.WriteLine("You can't go that way!");
							}
						}
						else {
							Console.WriteLine("You can't go that way!");
						}
						break;
          default:
            break;
        }
      }
    }
  }
}