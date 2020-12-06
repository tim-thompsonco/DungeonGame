using System.Collections.Generic;

namespace DungeonGame
{
	public static class RoomHandler
	{
		public static Dictionary<Coordinate, IRoom> Rooms { get; set; }

		public static void ChangeRoom(Player player, Coordinate newCoord)
		{
			player._PlayerLocation = newCoord;
			var playerRoom = Rooms[player._PlayerLocation];
			playerRoom.LookRoom();
			if (!playerRoom._IsDiscovered) playerRoom._IsDiscovered = true;
			player._CanSave = playerRoom is TownRoom;
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z)
		{
			var findCoord = new Coordinate(x, y, z);
			var room = Rooms[findCoord];
			if (room != null)
			{
				player._PlayerLocation = findCoord;
			}
			else
			{
				return;
			}
			if (!room._IsDiscovered) room._IsDiscovered = true;
			player._CanSave = room is TownRoom;
		}
	}
}