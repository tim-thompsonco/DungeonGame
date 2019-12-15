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
				"Dimly lit platform", // Name
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your northwest " +
				"and northeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				"that should not exist move slowly in the shadows.", //Description
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
					100, // Experience provided
					new Weapon(
						"notched axe", // Name
						25, // Regular damage
						10, // Item value
						1.2, // Crit multiplier
						true // Equipped bool
						)
					),
				false, // goNorth bool
				true, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				false, // goSouthWest bool
				true, // goNorthEast bool
				false, // goSouthEast bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room101);
			var room102 = new DungeonRoom(
				"Corridor", // Name
				"You are at the start of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor stretches on for an unknown distance, " +
				"as the light from the torches cannot penetrate further than about 20 feet ahead. A dark shape skitters from " +
				"the end of the hallway towards you.", // Description
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
					100, // Max HP
					120, // Experience provided
					new Weapon(
						"venomous fang", // Name
						30, // Regular damage
						25, // Item value
						1.2, // Crit multiplier
						true // Equipped bool
						),
					new Loot(
						"large venom sac", // Name
						15 // Item value
						)
					), 
				true, // goNorth bool
				false, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				true, // goSouthWest bool
				false, // goNorthEast bool
				false, // goSouthEast bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room102);
			var room103 = new DungeonRoom(
				"Corridor", // Name
				"You are at the end of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor ends in a pathway leading to the " +
				"west, where the other smooth rock platform that you saw previously is." , // Description
				1, // X coordinate
				3, // Y coordinate
				0, // Z coordinate
				new Monster(
					"skeleton warrior", // Name
					"A skeleton stands in front of you. His bones look worn and damaged from years of fighting. A ghastly " +
					"yellow glow surrounds him, which is the only indication of the magic that must exist to reanimate this " +
					"undead warrior. His chest and ribcage are fused together in a single, solid piece of armor and he raises " +
					"a sword menacingly towards you.", // Description
					2, // Level
					15, // Gold
					100, // Max HP
					120, // Experience provided
					new Weapon(
						"dull sword", // Name
						25, // Regular damage
						25, // Item value
						1.2, // Crit multiplier
						true // Equipped bool
						),
					new Armor(
						"bony chestplate", // Name
						Armor.ArmorSlot.Chest, // Armor slot
						10, // Item value
						5, // Low end of armor value range
						10, // High end of armor value range
						true // Equipped bool
						),
					new Consumable(
						"minor health potion", // Name
						3, // Item value
						Consumable.PotionType.Health, // Consumable type
						50 // Amount that consumable affects, IE restores 50 health if health potion
						)
					),
				false, // goNorth bool
				true, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				true, // goNorthWest bool
				false, // goSouthWest bool
				false, // goNorthEast bool
				false, // goSouthEast bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room103);
			var room104 = new DungeonRoom(
				"Dimly lit platform", // Name
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your southwest " +
				"and southeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				"that should not exist move slowly in the shadows.", //Description
				0, // X coordinate
				4, // Y coordinate
				0, // Z coordinate
				new Monster(
					"skeleton guardian", // Name
					"A skeleton stands in front of you. His bones look stronger than that of a normal skeleton. A ghastly " +
					"yellow glow surrounds him, which is the only indication of the magic that must exist to reanimate this " +
					"undead warrior. A shoddy, iron helmet adorns his head and a well-crafted iron longsword is clutched by " +
					"his skeletal fingers, which tighten their grip as he shuffles towards you and grunts.", // Description
					2, // Level
					20, // Gold
					120, // Max HP
					160, // Experience provided
					new Weapon(
						"iron longsword", // Name
						28, // Regular damage
						30, // Item value
						1.2, // Crit multiplier
						true // Equipped bool
						),
					new Armor(
						"iron helmet", // Name
						Armor.ArmorSlot.Chest, // Armor slot
						15, // Item value
						8, // Low end of armor value range
						12, // High end of armor value range
						true // Equipped bool
						),
					new Consumable(
						"minor mana potion", // Name
						3, // Item value
						Consumable.PotionType.Mana, // Consumable type
						50 // Amount that consumable affects, IE restores 50 health if health potion
						)
					),
				false, // goNorth bool
				false, // goSouth bool
				false, // goEast bool
				false, // goWest bool
				false, // goNorthWest bool
				false, // goSouthWest bool
				false, // goNorthEast bool
				true, // goSouthEast bool
				false, // goUp bool
				false); // goDown bool
			this.SpawnedRooms.Add(room104);
		}
		// Method to retrieve room list
		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedRooms;
		}
	}
}