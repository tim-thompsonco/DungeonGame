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
				false, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				false, // goSouthWest bool
				false, // goNorthEast bool
				false, // goNorthWest bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room100);
			var room101 = new DungeonRoom(
				"Dimly lit platform",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your northwest " +
				"and northeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				"that should not exist move slowly in the shadows.",
				0, // X coordinate
				1, // Y coordinate
				0, // Z coordinate
				new Monster(
					"rotting zombie", // Name
					"A rotting corpse stares at you, it's face frozen in a look of indifference to the fact a bug is crawling out of it's empty eye sockets. " +
					"In one hand, it drags a weapon against the ground, as it stares at you menacingly. Bones, muscle and tendons are visible through many " +
					"gashes and tears in it's rotting skin.", // Description
					1, // Level
					10, // Gold
					80, // Max HP
					500, // Experience provided
					new Weapon("notched axe", 25, 10, 1.2)), // Weapon for monster
				false, // goNorth bool
				true, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				false, // goSouthWest bool
				true, // goNorthEast bool
				false, // goNorthWest bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room101);
			var room102 = new DungeonRoom(
				"Corridor",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your northwest " +
				"and northeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. The air seems colder and thicker than it otherwise should be. Various shapes that should not " +
				"exist move slowly in the shadows.",
				1, // X coordinate
				2, // Y coordinate
				0, // Z coordinate
				new Monster(
					"huge spider", // Name
					"A huge black spider about the size of a large bear skitters down the corridor towards you. " +
					"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
					"you, legs ending in sharp claws carrying it closer as it hisses hungrily.", // Description
					2, // Level
					0, // Gold
					160, // Max HP
					1000, // Experience provided
					new Weapon("venomous fang", 35, 25, 1.2), // Weapon for monster
					new Item("large venom sac", 15) // Loot item
					), 
				false, // goNorth bool
				false, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				true, // goSouthWest bool
				false, // goNorthEast bool
				false, // goNorthWest bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room102);
			}
		// Method to retrieve room list
		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedRooms;
			}
		}
	}