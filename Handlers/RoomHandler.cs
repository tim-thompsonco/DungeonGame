using System.Collections.Generic;

namespace DungeonGame {
	public static class RoomHandler {
		public static List<IRoom> Rooms { get; set; }
		
		public static void ChangeRoom(Player player, IRoom room) {
			player.PlayerLocation = room;
			player.PlayerLocation.LookRoom();
			if (!player.PlayerLocation.IsDiscovered) player.PlayerLocation.IsDiscovered = true;
			player.CanSave = player.PlayerLocation is TownRoom;
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z) {
			var room = Rooms.Find(room => room.X == x && room.Y == y && room.Z == z);
			if (room != null) {
				player.PlayerLocation = room;
			}
			else {
				return;
			}
			if (!player.PlayerLocation.IsDiscovered) player.PlayerLocation.IsDiscovered = true;
			player.CanSave = player.PlayerLocation is TownRoom;
		}
	}
}