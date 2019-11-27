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
      var room100 = new Room100(
        "The Pit",
        "A starting room and you werrrreee in the pit!",
        new Skeleton("rotting skeleton", 10, 80, 400, new MainWeapon())); // Starting room
      var room101 = new Room101(
        "Another pit",
        "Some other room innnn the piiiiiiit!",
        new Zombie("rotting zombie", 25, 160, 1000, new MainWeapon())); // Second room
      var SpawnedRooms = new List<IRoom> {
        room100,
        room101
      };
      var roomIndex = SpawnedRooms.IndexOf(room100);
      SpawnedRooms[roomIndex].LookRoom();
      // While loop to continue obtaining input from player
      while (true) {
        player.DisplayPlayerStats();
        SpawnedRooms[roomIndex].ShowCommands();
        // On loading game, display room that player starts in
        // Begin game by putting player in room 100
        var input = Helper.GetFormattedInput();
        // Obtain player command and process command
        switch (input) {
          case "f":
            SpawnedRooms[roomIndex].MonsterFight(player);
            break;
          case "i":
            player.ShowInventory(player);
            break;
          case "q":
            Console.WriteLine("Game over.");
            return;
          case "l":
            SpawnedRooms[roomIndex].LookRoom();
            break;
          case "lc":
            SpawnedRooms[roomIndex].LootCorpse(player);
            break;
          case "n":
            if(SpawnedRooms[roomIndex].GoNorth) {
							try {
								roomIndex += 1;
								SpawnedRooms[roomIndex].LookRoom();
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
						if(SpawnedRooms[roomIndex].GoSouth) {
							try {
								roomIndex -= 1;
								SpawnedRooms[roomIndex].LookRoom();
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