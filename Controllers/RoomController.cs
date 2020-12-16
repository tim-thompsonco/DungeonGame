using System.Collections.Generic;

namespace DungeonGame.Controllers
{
	public static class RoomController
	{
		public static Dictionary<Coordinate, IRoom> _Rooms { get; set; }

		public static void ChangeRoom(Player player, Coordinate newCoord)
		{
			player._PlayerLocation = newCoord;
			IRoom playerRoom = _Rooms[player._PlayerLocation];
			playerRoom.LookRoom();
			if (!playerRoom._IsDiscovered)
			{
				playerRoom._IsDiscovered = true;
			}

			player._CanSave = playerRoom is TownRoom;
		}
		public static void SetPlayerLocation(Player player, int x, int y, int z)
		{
			Coordinate findCoord = new Coordinate(x, y, z);
			IRoom room = _Rooms[findCoord];
			if (room != null)
			{
				player._PlayerLocation = findCoord;
			}
			else
			{
				return;
			}
			if (!room._IsDiscovered)
			{
				room._IsDiscovered = true;
			}

			player._CanSave = room is TownRoom;
		}
	}
}