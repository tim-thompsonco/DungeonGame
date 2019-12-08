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
					case "dhp":
						if (player.HealthPotion.Quantity >= 1) {
							player.HealthPotion.RestoreHealth.RestoreHealthPlayer(player);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("You drank a potion and replenished {0} health.", player.HealthPotion.RestoreHealth.RestoreHealthAmt);
							player.HealthPotion.Quantity -= 1;
							player.Inventory.Remove((DungeonGame.IEquipment)player.HealthPotion);
						}
						else {
							Console.WriteLine("You don't have any health potions!");
						}
						break;
					case "dmp":
						if (player.ManaPotion.Quantity >= 1) {
							player.ManaPotion.RestoreMana.RestoreManaPlayer(player);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("You drank a potion and replenished {0} mana.", player.ManaPotion.RestoreMana.RestoreManaAmt);
							player.ManaPotion.Quantity -= 1;
							player.Inventory.Remove((DungeonGame.IEquipment)player.ManaPotion);
						}
						else {
							Console.WriteLine("You don't have any mana potions!");
						}
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
					case "e":
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