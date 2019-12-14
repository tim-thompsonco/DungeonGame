using System;
using System.Linq;

namespace DungeonGame {
  class MainClass {
    public static void Main(string[] args) {
      // Game loading commands
			Console.ForegroundColor = ConsoleColor.Gray;
      Helper.GameIntro();
      var player = new Player(Helper.FetchPlayerName());
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
				var inputParse = input.Split(' ');
        // Obtain player command and process command
        switch (inputParse[0]) {
					case "a":
          case "attack":
						try {
							if (inputParse[1] != null) {
								try {
									spawnedRooms[roomIndex].AttackMonster(player, inputParse);
								}
								catch (Exception) {
									Console.WriteLine("An error has occurred.");
								}
							}
						}
						catch (IndexOutOfRangeException) {
							Console.WriteLine("You can't attack that.");
						}
            break;
					case "equip":
					case "unequip":
						player.EquipItem(inputParse);
						break;
          case "i":
					case "inventory":
						player.ShowInventory(player);
						break;
					case "q":
					case "quit":
						Helper.GameOver();
						return;
					case "l":
					case "look":
						try {
							if (inputParse[1] != null) {
								try {
									spawnedRooms[roomIndex].LookMonster(inputParse);
								}
								catch (Exception) {
									Console.WriteLine("An error has occurred.");
								}
							}
						}
						catch(IndexOutOfRangeException) {
							spawnedRooms[roomIndex].LookRoom();
						}
            break;
          case "loot":
						try {
							if (inputParse[1] != null) {
								try {
									spawnedRooms[roomIndex].LootCorpse(player, inputParse);
								}
								catch (Exception) {
									Console.WriteLine("An error has occurred.");
								}
							}
						}
						catch (IndexOutOfRangeException) {
							Console.WriteLine("Loot what?");
						}
						break;
					case "drink":
						if (inputParse.Last() == "potion") {
							player.DrinkPotion(inputParse);
						}
						else {
							Console.WriteLine("You can't drink that!");
						}
						break;
					case "n":
					case "north":
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
					case "south":
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
					case "e":
					case "east":
						if (spawnedRooms[roomIndex].GoEast) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 0, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "w":
					case "west":
						if (spawnedRooms[roomIndex].GoWest) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 0, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "ne":
					case "northeast":
						if (spawnedRooms[roomIndex].GoNorthEast) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, 1, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "nw":
					case "northwest":
						if (spawnedRooms[roomIndex].GoNorthWest) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, 1, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "se":
					case "southeast":
						if (spawnedRooms[roomIndex].GoSouthEast) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, 1, -1, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "sw":
					case "southwest":
						if (spawnedRooms[roomIndex].GoSouthWest) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, -1, -1, 0);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "u":
					case "up":
						if (spawnedRooms[roomIndex].GoUp) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, 1);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
								Helper.InvalidDirection();
							}
						}
						else {
							Helper.InvalidDirection();
						}
						break;
					case "d":
					case "down":
						if (spawnedRooms[roomIndex].GoDown) {
							try {
								roomIndex = Helper.ChangeRoom(spawnedRooms, player, 0, 0, -1);
								spawnedRooms[roomIndex].LookRoom();
							}
							catch (ArgumentOutOfRangeException) {
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