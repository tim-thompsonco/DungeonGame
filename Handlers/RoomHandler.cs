using System.Collections.Generic;

namespace DungeonGame {
	public static class RoomHandler {
		public static List<IRoom> Rooms { get; set; }
		public static int RoomIndex { get; set;}
		
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
	}
}