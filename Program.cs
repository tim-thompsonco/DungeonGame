using System;
using System.Collections.Generic;

namespace DungeonGame {
  class MainClass {
    public static void Main(string[] args) {
      // Game loading commands
			Console.ForegroundColor = ConsoleColor.Gray;
      Helper.GameIntro();
      var player = new NewPlayer(Helper.FetchPlayerName());
			var spawnedRooms = new SpawnRooms().RetrieveSpawnRooms();
      // Set initial room condition
			// On loading game, display room that player starts in
      // Begin game by putting player in room 100
      var roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 0);
      spawnedRooms[roomIndex].LookRoom();
      // While loop to continue obtaining input from player
      while (true) {
        player.DisplayPlayerStats();
        spawnedRooms[roomIndex].ShowCommands();
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
					case "lm":
						spawnedRooms[roomIndex].LookMonster();
						break;
          case "n":
            if(spawnedRooms[roomIndex].GoNorth) {
							try {
                roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 1, 0);
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
						if(spawnedRooms[roomIndex].GoSouth) {
							try {
                roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, -1, 0);
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
						Helper.InvalidCommand();
            break;
        }
      }
    }
  }
}