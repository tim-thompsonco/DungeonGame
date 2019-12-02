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
				new Monster(
					"A rotting zombie", // Name
					"A rotting corpse stares at you, it's face frozen in a look of indifference to the fact a bug is crawling out of it's empty eye sockets. " +
					"In one hand, it drags a weapon against the ground, as it stares at you menacingly. Bones, muscle and tendons are visible through many " +
					"gashes and tears in it's rotting skin.", // Description
					1, // Level
					25, // Gold
					160, // Max HP
					1000, // Experience provided
					new Weapon("A notched axe", 25, 15, 1.2)), // Weapon for monster
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