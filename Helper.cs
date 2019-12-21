using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace DungeonGame {
  public static class Helper {
    public static string GetFormattedInput() {
      var input = Console.ReadLine();
      var inputFormatted = input.ToLower().Trim();
      return inputFormatted;
    }
    public static void RequestCommand() {
      Console.Write("Your command: ");
    }
    public static void PlayerDeath() {
      Console.WriteLine("You have died. Game over.");
    }
		public static void GameIntro() {
			Console.WriteLine(
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot " +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games.\n");
			Console.WriteLine(
				"Commands: Players may move in any direction of the game using a shortkey or the full direction name. " +
				"For example, if you wish to go north, you may type either 'N' or 'North'. If a player wishes to look " +
				"at something, they can use 'l' or 'look' and then the name of what they want to look at. For example " +
				"'l zombie' or 'look zombie' would allow you to look at a zombie in the room. The same commands will  " +
				"work to loot a monster that you have killed. Look or 'L' by itself will look at the room. Other common " +
				"commands will be shown to the player. Any object that is consumable, such as a potion, can be drank " +
				"by typing 'drink' and then the name of the potion or object.");
			Console.WriteLine("For now, please enter a player name.\n");
		}
		public static string FetchPlayerName() {
			while(true) {
				Console.Write("Player name: ");
				var playerName = Console.ReadLine();
				Console.WriteLine("Your player name is {0}, is that correct? [Y] or [N].", playerName);
				RequestCommand();
				string input = GetFormattedInput();
				if(input == "y") {
					return playerName;
				}
			}
		}
    public static void InvalidCommand() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine("Not a valid command.");
    }
    public static int ChangeRoom(List<IRoom> roomList, Player player, int x, int y, int z) {
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Set player location to location of room found in search
      IRoom roomName = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
      var roomIndex = roomList.IndexOf(roomName);
			roomList[roomIndex].LookRoom();
			var roomType = roomList[roomIndex].GetType().Name;
			if (roomType == "DungeonRoom") {
				player.CanSave = false;
			}
			else {
				player.CanSave = true;
			}
			return roomIndex;
		}
		public static void InvalidDirection() {
			Console.WriteLine("You can't go that way!");
		}
		public static bool QuitGame(Player player) {
			Console.WriteLine("Are you sure you want to quit?");
			string input = Helper.GetFormattedInput();
			if (input == "yes" || input == "y") {
				Console.WriteLine("Quitting the game.");
				player.CanSave = true;
				Helper.SaveGame(player);
				return true;
			}
			return false;
		}
		public static bool IsWearable(IEquipment item) {
			if (item.GetType().Name == "Armor" || item.GetType().Name == "Weapon") {
				return true;
			}
			return false;
		}
		public static void SaveGame(Player player) {
			if (player.CanSave == true) {
				var serializer = new Newtonsoft.Json.JsonSerializer();
				serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
				serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
				serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (StreamWriter sw = new StreamWriter("savegame.json"))
				using (var writer = new Newtonsoft.Json.JsonTextWriter(sw)) {
					serializer.Serialize(writer, player, typeof(Player));
				}
				Console.WriteLine("Your game has been saved.");
				return;
			}
			Console.WriteLine("You can't save inside a dungeon! Go outside first.");
		}
  }
}