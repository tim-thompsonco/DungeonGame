using DungeonGame.Coordinates;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System.Collections.Generic;

namespace DungeonGame.Helpers {
	public static class RoomHelper {
		public static Dictionary<Coordinate, IRoom> Rooms { get; set; }

		public static void ChangeRoom(Player player, Coordinate newCoord) {
			player.PlayerLocation = newCoord;
			IRoom playerRoom = Rooms[player.PlayerLocation];
			playerRoom.LookRoom();
			if (!playerRoom.IsDiscovered) {
				playerRoom.IsDiscovered = true;
			}

			player.CanSave = playerRoom is TownRoom;
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z) {
			Coordinate findCoord = new Coordinate(x, y, z);
			IRoom room = Rooms[findCoord];
			if (room != null) {
				player.PlayerLocation = findCoord;
			} else {
				return;
			}
			if (!room.IsDiscovered) {
				room.IsDiscovered = true;
			}

			player.CanSave = room is TownRoom;
		}
	}
}