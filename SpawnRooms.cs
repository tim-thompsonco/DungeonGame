using System.Collections.Generic;

namespace DungeonGame {
	public class SpawnRooms {
		public List<IRoom> SpawnedRooms { get; set; } = new List<IRoom>();

		public SpawnRooms() {
			var room100 = new DungeonRoom(
				"A dark room",
				"You are in a dimly lit room. There is a lantern burning on a hook on the opposite wall. Water drips " +
				"from a few stalactites hanging from the ceiling, which is about 12 feet high. You appear to be in a " +
				"dungeon. A moaning wail echoes in the distance through an open doorway in front of you. Glinting " +
				"red eyes seem to be visible in the distance. There is an unsettling sound of a heavy metal object " +
				"being dragged on the ground by a faint shape beyond the doorway. You can't make out what it is.",
				0, // X coordinate
				0, // Y coordinate
				0, // Z coordinate
				true, // goNorth bool
				false); // goSouth bool
			this.SpawnedRooms.Add(room100);
			var room101 = new DungeonRoom(
				"Dimly lit platform",
				"Some other room...to be continued",
				0, // X coordinate
				1, // Y coordinate
				0, // Z coordinate
				new Monster("rotting zombie", 25, 160, 1000, new Weapon("A notched axe", 25, 15, 1.2)),
				false, // goNorth bool
				true); // goSouth bool
			this.SpawnedRooms.Add(room101);
			}
		// Method to retrieve room list
		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedRooms;
			}
		}
	}