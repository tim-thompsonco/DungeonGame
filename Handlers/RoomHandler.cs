using System.Collections.Generic;
using System.Text;

namespace DungeonGame {
	public static class RoomHandler {
		public static UserOutput Display = new UserOutput();
		public static UserOutput MapDisplay = new UserOutput();
		public static UserOutput EffectDisplay = new UserOutput();
		public static List<IRoom> Rooms { get; set; }
		public static int RoomIndex { get; set;}
		
		public static string[] GetFormattedInput(string userInput) {
			var inputFormatted = userInput.ToLower().Trim();
			var inputParse = inputFormatted.Split(' ');
			return inputParse;
		}
		public static string ParseInput(string[] userInput) {
			var inputString = new StringBuilder();
			for (var i = 1; i < userInput.Length; i++) {
				inputString.Append(userInput[i]);
				inputString.Append(' ');
			}
			var parsedInput = inputString.ToString().Trim();
			return parsedInput;
		}
		public static void ChangeRoom(Player player, int x, int y, int z) {
			// Player location is changed to the new coordinates
			player.X += x;
			player.Y += y;
			player.Z += z;
			// Room at new coordinates is found and room description displayed for user
			var room = Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			if (Rooms.IndexOf(room) != -1) RoomIndex = Rooms.IndexOf(room);
			Rooms[RoomIndex].LookRoom();
			if (!Rooms[RoomIndex].IsDiscovered) Rooms[RoomIndex].IsDiscovered = true;
			var roomType = Rooms[RoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom"; 
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z) {
			player.X = x;
			player.Y = y;
			player.Z = z;
			// Room at new coordinates is found and room description displayed for user
			var room = Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var setRoomIndex = Rooms.IndexOf(room);
			if (setRoomIndex == -1) return;
			if (!Rooms[setRoomIndex].IsDiscovered) Rooms[setRoomIndex].IsDiscovered = true;
			var roomType = Rooms[setRoomIndex].GetType().Name;
			player.CanSave = roomType != "DungeonRoom";
			RoomIndex = setRoomIndex;
		}
		public static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void ShowUserOutput(Player player, Monster opponent) {
			PlayerHandler.DisplayPlayerStats(player);
			opponent.DisplayStats();
			Rooms[RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
		public static void ShowUserOutput(Player player) {
			PlayerHandler.DisplayPlayerStats(player);
			Rooms[RoomIndex].ShowCommands();
			MapDisplay = MapOutput.BuildMap(player, Settings.GetMiniMapHeight(), Settings.GetMiniMapWidth());
			EffectDisplay = EffectOutput.ShowEffects(player);
			Display.BuildUserOutput();
			Display.RetrieveUserOutput();
			Display.ClearUserOutput();
		}
	}
}