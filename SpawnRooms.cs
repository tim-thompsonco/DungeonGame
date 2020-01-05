using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class SpawnRooms {
		public List<IRoom> SpawnedRooms { get; set; } = new List<IRoom>();

		public SpawnRooms() {
			var room100 = new DungeonRoom(
				"A Dark Room",
				"You are in a dimly lit room. There is a lantern burning on a hook on the opposite wall. Water drips " + 
				"from a few stalactites hanging from the ceiling, which is about 12 feet high. You appear to be in a " +
				"dungeon. A moaning wail echoes in the distance through an open doorway in front of you. Glinting " +
				"red eyes seem to be visible in the distance. There is an unsettling sound of a heavy metal object " +
				"being dragged on the ground by a faint shape beyond the doorway. You can't make out what it is.",
				0, 
				0, 
				0, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				1, 
				3); 
			this.SpawnedRooms.Add(room100);
			var room101 = new DungeonRoom(
				"Dimly Lit Platform",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your northwest " +
				"and northeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				"that should not exist move slowly in the shadows.", 
				0, 
				1, 
				0, 
				false, 
				true, 
				false, 
				false, 
				true, 	
				false, 
				true, 
				false,
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room101);
			var room102 = new DungeonRoom(
				"Corridor",
				"You are at the start of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor stretches on for an unknown distance, " +
				"as the light from the torches cannot penetrate further than about 20 feet ahead. A dark shape skitters from " +
				"the end of the hallway towards you.", 
				1, 
				2, 
				0, 
				true, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room102);
			var room103 = new DungeonRoom(
				"Corridor",
				"You are at the end of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor ends in a pathway leading to the " +
				"west, where the other smooth rock platform that you saw previously is.", 
				1, 
				3, 
				0, 
				false, 
				true, 
				false, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room103);
			var room104 = new DungeonRoom(
				"Dimly Lit Platform",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your southwest " +
				"and southeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				"that should not exist move slowly in the shadows.", 
				0, 
				4, 
				0, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				true, 
				true, 
				true,
				1,
				3); 
			this.SpawnedRooms.Add(room104);
			var room105 = new DungeonRoom(
				"Corridor",
				"You are at the end of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor ends in a pathway leading to the " +
				"west, where the other smooth rock platform that you saw previously is.", 
				-1, 
				3, 
				0, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room105);
			var room106 = new DungeonRoom(
				"Corridor",
				"You are at the start of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				"Torches in holders along the wall illuminate the hallway. The corridor stretches on for an unknown distance, " +
				"as the light from the torches cannot penetrate further than about 20 feet ahead. A dark shape skitters from " +
				"the end of the hallway towards you.", 
				-1, 
				2, 
				0, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room106);
			var room107 = new DungeonRoom(
				"Pathway to Sunken Pit",
				"You walk down an incline of smooth rock down a pathway to arrive at a sunken pit. You can see the other " +
				"platform above you on the other side of the cavern. There is almost complete darkness in front of you, with no " +
				" torches lighting the way as before. A large shape seems to be pacing back and forth in the shadows but you " +
				"can't make out many details unless you walk closer.", 
				0, 
				4, 
				-1, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room107);
			var room108 = new DungeonRoom(
				"Sunken Pit",
				"You are at the bottom of the downward inclined pathway and are standing in a pit below the cavern pathways to " +
				"either side above you. Bones and skulls adorn the pit, as you carefully pick your way in between them to avoid " +
				"making a crunching sound. There is a red hue around the room yet it's not created by any torches or natural " +
				"light. A deep growl emanates from somewhere in front of you, the sound vibrating the ground with it's force.", 
				0, 
				3, 
				-1, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false,
				1,
				3); 
			this.SpawnedRooms.Add(room108);
			var room109 = new DungeonRoom(
				"Stairway Leading Up",
				"You are at the base of a long stairway leading up towards an old, wooden door. Torches line the walls to either " +
				"side and a few drops of blood stain some of the steps. There appears to be a skeleton stretched out before the " +
				"steps, one hand reaching out and it's wrist bones crushed by a large object, as if someone was trying to flee " +
				"and didn't make it.", 
				0, 
				4, 
				1, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				true,
				1,
				3); 
			this.SpawnedRooms.Add(room109);
			var room110 = new TownRoom(
				"Outside Dungeon Entrance",
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with no " +
				"lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. Around you " +
				"is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what looks like a " +
				"town. Smoke rises from a few chimneys in the distance.", 
				0, 
				4, 
				2, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true); 
			this.SpawnedRooms.Add(room110);
			var room111 = new TownRoom(
				"Cobblestone Path",
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock. ", 
				0, 
				5, 
				2, 
				true, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false); 
			this.SpawnedRooms.Add(room111);
			var room112 = new TownRoom(
				"Cobblestone Path",
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock. ", 
				0, 
				6, 
				2, 
				true, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false); 
			this.SpawnedRooms.Add(room112);
			var room113 = new TownRoom(
				"Town Entrance",
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what " +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				"northwest are a few buildings with signs outside that you can't read from this distance.", 
				0, 
				7, 
				2, 
				false, 
				true, 
				false, 
				false, 
				true, 
				false, 
				true, 
				false, 
				false, 
				false); 
			this.SpawnedRooms.Add(room113);
			var room114 = new TownRoom(
				"Town - East",
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside " +
				"and a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				"running a sword against a grindstone to sharpen it.", 
				1, 
				8, 
				2, 
				true, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false,
				new Vendor(
					"armorer",
					"A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow as " +
					"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
					"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
					"Armor")); 
			this.SpawnedRooms.Add(room114);
			var room115 = new TownRoom(
			"Town - East",
			"You are in the east part of the town. A large man is in front of a building sharpening a sword against " +
			"a grindstone. To the south, you can see a small building with a forge and furnace outside. There is " +
			"another large man in front of it pounding away at a chestplate with a hammer.", 
			1, 
			9, 
			2, 
			false, 
			true, 
			false, 
			false, 
			true, 
			false, 
			false, 
			false, 
			false, 
			false,
			new Vendor(
			"weaponsmith",
			"A large man covered in sweat sharpening a sword against a grindstone. He wipes his brow as " +
			"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
			"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
			"Weapon")); 
			this.SpawnedRooms.Add(room115);
			var room116 = new TownRoom(
				"Town - Center",
				"You are in the central part of the town. There is a wrinkled old man standing in front of " +
				"a small hut, his hands clasped in the arms of his robes, as he gazes around the town calmly. ", 
				0, 
				10, 
				2, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				true, 
				false, 
				false,
				new Vendor(
				"healer",
				"An old man covered in robes looks you up and raises an eyebrow questioningly. He can rid you " +
				"of all your pain every so often. In fact, he may even provide you with some help that will " +
				"be invaluable in your travels. You can buy <item>, sell <item>, or <show forsale> to see what " +
				"he has for sale. You can also try to ask him to <heal> you.", 
				"Healer")); 
			this.SpawnedRooms.Add(room116);
			var room117 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. A woman stands in front of a building with displays " +
				"of various items in front of it. It looks like she buys and sells a little bit of everything.", 
				-1, 
				9, 
				2, 
				false, 
				true, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false, 
				false,
				new Vendor(
				"shopkeeper",
				"A woman in casual work clothes looks at you and asks if you want to buy anything. She raises " +
				"an item to show an example of what she has for sale. You can buy <item>, sell <item>, or " +
				"<show forsale> to see what she has for sale.", 
				"Shopkeeper")); 
			this.SpawnedRooms.Add(room117);
			var room118 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. There is a large, wooden building in front of you with a " +
				"sign out front that reads 'Training'. Depending on what class you are, it appears that this " +
				"place might have some people who can help you learn more.", 
				-1, 
				8, 
				2, 
				true, 
				false, 
				false, 
				false, 
				false, 
				false, 
				false, 
				true, 
				false, 
				false);
			this.SpawnedRooms.Add(room118);
		}

		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedRooms;
		}
	}
}