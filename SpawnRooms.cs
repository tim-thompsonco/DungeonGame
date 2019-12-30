using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class SpawnRooms {
		public List<IRoom> SpawnedRooms { get; set; } = new List<IRoom>();

		public SpawnRooms() {
			var room100 = new DungeonRoom(
				"A Dark Room",
				"You are in a dimly lit room. There is a lantern burning on a hook on the opposite wall. Water drips " + 
				Environment.NewLine +
				"from a few stalactites hanging from the ceiling, which is about 12 feet high. You appear to be in a " +
				Environment.NewLine +
				"dungeon. A moaning wail echoes in the distance through an open doorway in front of you. Glinting " +
				Environment.NewLine +
				"red eyes seem to be visible in the distance. There is an unsettling sound of a heavy metal object " +
				Environment.NewLine +
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
				false); 
			this.SpawnedRooms.Add(room100);
			var room101 = new DungeonRoom(
				"Dimly Lit Platform",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				Environment.NewLine +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your northwest " +
				Environment.NewLine +
				"and northeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				Environment.NewLine +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				Environment.NewLine +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				Environment.NewLine +
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
				new Monster(
				"rotting zombie",
				"A rotting corpse stares at you, it's face frozen in a look of indifference to the fact a bug is crawling" +
				Environment.NewLine +
				" out of it's empty eye sockets. In one hand, it drags a weapon against the ground, as it stares at you " +
				Environment.NewLine +
				"menacingly. Bones, muscle and tendons are visible through many gashes and tears in it's rotting skin.",
				1, 
				10, 
				80, 
				100, 
					new Weapon(
						"notched axe",
						14, 
						24, 
						24, 
						1.2, 
						true,
						Weapon.WeaponType.Axe))); 
			this.SpawnedRooms.Add(room101);
			var room102 = new DungeonRoom(
				"Corridor",
				"You are at the start of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				Environment.NewLine +
				"Torches in holders along the wall illuminate the hallway. The corridor stretches on for an unknown distance, " +
				Environment.NewLine +
				"as the light from the torches cannot penetrate further than about 20 feet ahead. A dark shape skitters from " +
				Environment.NewLine +
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
				new Monster(
				"huge spider",
				"A huge black spider about the size of a large bear skitters down the corridor towards you. " +
				Environment.NewLine +
				"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
				Environment.NewLine +
				"you, legs ending in sharp claws carrying it closer as it hisses hungrily.", 
				2, 
				0, 
				100, 
				120, 
					new Weapon(
						"venomous fang",
						19, 
						29, 
						29, 
						1.3, 
						true ,
						Weapon.WeaponType.Dagger),
					new Loot(
						"large venom sac",
						15))); 
			this.SpawnedRooms.Add(room102);
			var room103 = new DungeonRoom(
				"Corridor",
				"You are at the end of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				Environment.NewLine +
				"Torches in holders along the wall illuminate the hallway. The corridor ends in a pathway leading to the " +
				Environment.NewLine +
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
				new Monster(
				"skeleton warrior",
				"A skeleton stands in front of you. His bones look worn and damaged from years of fighting. A ghastly " +
				Environment.NewLine +
				"yellow glow surrounds him, which is the only indication of the magic that must exist to reanimate this " +
				Environment.NewLine +
				"undead warrior. His chest and ribcage are fused together in a single, solid piece of armor and he raises " +
				Environment.NewLine +
				"a sword menacingly towards you.", 
				2, 
				15, 
				100, 
				120, 
					new Weapon(
						"dull sword",
						15, 
						22, 
						22, 
						1.2, 
						true ,
						Weapon.WeaponType.OneHandedSword),
					new Armor(
						"bony chestplate",
						Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Plate, 
						16, 
						11, 
						16, 
						true),
					new Consumable(
						"minor health potion",
						3, 
						Consumable.PotionType.Health, 
						50))); 
			this.SpawnedRooms.Add(room103);
			var room104 = new DungeonRoom(
				"Dimly Lit Platform",
				"You are standing on a platform of smooth rock in a cavern. The ceiling is roughly 40 feet high, with " +
				Environment.NewLine +
				"stalactites hanging from the ceiling as in the room behind you that you just left. To your southwest " +
				Environment.NewLine +
				"and southeast are pathways leading down corridors. Torches in holders along the corridors illuminate " +
				Environment.NewLine +
				"the way. In front of you is a deep pit encased in darkness. Above it on the other side of the cavern " +
				Environment.NewLine +
				"is another platform. The air seems colder and thicker than it otherwise should be. Various shapes " +
				Environment.NewLine +
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
				new Monster(
				"skeleton guardian",
				"A skeleton stands in front of you. His bones look stronger than that of a normal skeleton. A ghastly " +
				Environment.NewLine +
				"yellow glow surrounds him, which is the only indication of the magic that must exist to reanimate this " +
				Environment.NewLine +
				"undead warrior. A shoddy, bronze helmet adorns his head and a well-crafted bronze longsword is clutched by " +
				Environment.NewLine +
				"his skeletal fingers, which tighten their grip as he shuffles towards you and grunts.", 
				2, 
				20, 
				120, 
				160, 
					new Weapon(
						"bronze longsword",
						23, 
						29, 
						29, 
						1.2, 
						true ,
						Weapon.WeaponType.OneHandedSword),
					new Armor(
						"bronze helmet",
						Armor.ArmorSlot.Head, 
						Armor.ArmorType.Plate, 
						12, 
						10, 
						12, 
						true),
					new Consumable(
						"minor mana potion",
						3, 
						Consumable.PotionType.Mana, 
						50))); 
			this.SpawnedRooms.Add(room104);
			var room105 = new DungeonRoom(
				"Corridor",
				"You are at the end of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				Environment.NewLine +
				"Torches in holders along the wall illuminate the hallway. The corridor ends in a pathway leading to the " +
				Environment.NewLine +
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
				new Monster(
				"skeleton warrior",
				"A skeleton stands in front of you. His bones look worn and damaged from years of fighting. A ghastly " +
				Environment.NewLine +
				"yellow glow surrounds him, which is the only indication of the magic that must exist to reanimate this " +
				Environment.NewLine +
				"undead warrior. His chest and ribcage are fused together in a single, solid piece of armor and he raises " +
				Environment.NewLine +
				"a sword menacingly towards you.", 
				2, 
				15, 
				100, 
				120, 
					new Weapon(
						"dull sword",
						15, 
						22, 
						22, 
						1.2, 
						true ,
						Weapon.WeaponType.OneHandedSword),
					new Armor(
						"bony chestplate",
						Armor.ArmorSlot.Chest, 
						Armor.ArmorType.Plate, 
						16, 
						11, 
						16, 
						true),
					new Consumable(
						"minor health potion",
						3, 
						Consumable.PotionType.Health, 
						50))); 
			this.SpawnedRooms.Add(room105);
			var room106 = new DungeonRoom(
				"Corridor",
				"You are at the start of a corridor carved out of smooth rock approximately 6 feet wide and 10 feet high. " +
				Environment.NewLine +
				"Torches in holders along the wall illuminate the hallway. The corridor stretches on for an unknown distance, " +
				Environment.NewLine +
				"as the light from the torches cannot penetrate further than about 20 feet ahead. A dark shape skitters from " +
				Environment.NewLine +
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
				new Monster(
				"huge spider",
				"A huge black spider about the size of a large bear skitters down the corridor towards you. " +
				Environment.NewLine +
				"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
				Environment.NewLine +
				"you, legs ending in sharp claws carrying it closer as it hisses hungrily.", 
				2, 
				0, 
				100, 
				120, 
					new Weapon(
						"venomous fang",
						19,  
						29, 
						29, 
						1.3, 
						true ,
						Weapon.WeaponType.Dagger),
					new Loot(
						"large venom sac",
						15))); 
			this.SpawnedRooms.Add(room106);
			var room107 = new DungeonRoom(
				"Pathway to Sunken Pit",
				"You walk down an incline of smooth rock down a pathway to arrive at a sunken pit. You can see the other " +
				Environment.NewLine +
				"platform above you on the other side of the cavern. There is almost complete darkness in front of you, with no " +
				Environment.NewLine +
				" torches lighting the way as before. A large shape seems to be pacing back and forth in the shadows but you " +
				Environment.NewLine +
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
				false); 
			this.SpawnedRooms.Add(room107);
			var room108 = new DungeonRoom(
				"Sunken Pit",
				"You are at the bottom of the downward inclined pathway and are standing in a pit below the cavern pathways to " +
				Environment.NewLine +
				"either side above you. Bones and skulls adorn the pit, as you carefully pick your way in between them to avoid " +
				Environment.NewLine +
				"making a crunching sound. There is a red hue around the room yet it's not created by any torches or natural " +
				Environment.NewLine +
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
				new Monster(
				"horned demon",
				"A massive red demon stands before you with two horns sticking out of it's head. It's eyes glint " +
				Environment.NewLine +
				"yellow and a look of pure hatred adorns its face. Leathery wings spread out on either side of its " +
				Environment.NewLine +
				"back as it rises up to its full height of 8 feet and growls at you.", 
				4, 
				50, 
				200, 
				250, 
					new Weapon(
						"obsidian axe",
						29, 
						35, 
						35, 
						1.1, 
						true ,
						Weapon.WeaponType.Axe),
					new Loot(
						"ruby",
						50))); 
			this.SpawnedRooms.Add(room108);
			var room109 = new DungeonRoom(
				"Stairway Leading Up",
				"You are at the base of a long stairway leading up towards an old, wooden door. Torches line the walls to either " +
				Environment.NewLine +
				"side and a few drops of blood stain some of the steps. There appears to be a skeleton stretched out before the " +
				Environment.NewLine +
				"steps, one hand reaching out and it's wrist bones crushed by a large object, as if someone was trying to flee " +
				Environment.NewLine +
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
				true); 
			this.SpawnedRooms.Add(room109);
			var room110 = new TownRoom(
				"Outside Dungeon Entrance",
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with no " +
				Environment.NewLine +
				"lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. Around you " +
				Environment.NewLine +
				"is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what looks like a " +
				Environment.NewLine +
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
				Environment.NewLine +
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
				Environment.NewLine +
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
				Environment.NewLine +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				Environment.NewLine +
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
				Environment.NewLine +
				"and a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				Environment.NewLine +
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
					Environment.NewLine +
					"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
					Environment.NewLine +
					"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
					"Armor")); 
			this.SpawnedRooms.Add(room114);
			var room115 = new TownRoom(
			"Town - East",
			"You are in the east part of the town. A large man is in front of a building sharpening a sword against " +
			Environment.NewLine +
			"a grindstone. To the south, you can see a small building with a forge and furnace outside. There is " +
			Environment.NewLine +
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
			Environment.NewLine +
			"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
			Environment.NewLine +
			"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
			"Weapon")); 
			this.SpawnedRooms.Add(room115);
			var room116 = new TownRoom(
				"Town - Center",
				"You are in the central part of the town. There is a wrinkled old man standing in front of " +
				Environment.NewLine +
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
				Environment.NewLine +
				"of all your pain every so often. In fact, he may even provide you with some help that will " +
				Environment.NewLine +
				"be invaluable in your travels. You can buy <item>, sell <item>, or <show forsale> to see what " +
				Environment.NewLine +
				"he has for sale. You can also try to ask him to <heal> you.", 
				"Healer")); 
			this.SpawnedRooms.Add(room116);
			var room117 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. A woman stands in front of a building with displays " +
				Environment.NewLine +
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
				Environment.NewLine +
				"an item to show an example of what she has for sale. You can buy <item>, sell <item>, or " +
				Environment.NewLine +
				"<show forsale> to see what she has for sale.", 
				"Shopkeeper")); 
			this.SpawnedRooms.Add(room117);
			var room118 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. There is a large, wooden building in front of you with a " +
				Environment.NewLine +
				"sign out front that reads 'Training'. Depending on what class you are, it appears that this " +
				Environment.NewLine +
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