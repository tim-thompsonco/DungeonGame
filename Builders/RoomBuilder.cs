using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
	public class RoomBuilder {
		private int Size { get; set; }
		private int Levels { get; set; }
		private int CurrentLevel { get; set; }
		private List<IRoom> SpawnedDungeonRooms { get; set; }

		public RoomBuilder(int size, int levels, int startX, int startY, int startZ) {
			// Create town to connect dungeon to
			this.SpawnedDungeonRooms = BuildTown();
			// Dungeon build settings
			this.Size = size;
			this.Levels = levels;
			var levelSize = size / levels;
			for (var i = 0; i < levels; i++) {
				var levelRangeLowForLevel = i - 1 >= 1 ? i - 1 : 1;
				var levelRangeHighForLevel = i + 2 <= 10 ? i + 1: 10;
				for (var j = 0; j < levelSize; j++) {
					if (i == 0 && j == 0) {
						/* To connect static room to dynamic dungeon build, always have first room go down one, and the static
						 room must always be up by one for the two to connect */
						var firstRoom = new DungeonRoom(
							startX, startY,startZ - 1, levelRangeLowForLevel, levelRangeHighForLevel);
						this.SpawnedDungeonRooms[0].Down = firstRoom;
						firstRoom.Up = this.SpawnedDungeonRooms[0];
						this.SpawnedDungeonRooms.Add(firstRoom);
						this.CurrentLevel--;
						continue;
					}
					if (i > 0 && j == 0) {
						/* To connect upper level to lower level, always have first room go down one, and the upper level
						 room must always be up by one for the two to connect*/
						var oldRoom = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1];
						var newLevelRoom = new DungeonRoom(
							oldRoom.X, oldRoom.Y, oldRoom.Z - 1, levelRangeLowForLevel, levelRangeHighForLevel);
						oldRoom.Down = newLevelRoom;
						newLevelRoom.Up = oldRoom;
						this.SpawnedDungeonRooms.Add(newLevelRoom);
						this.CurrentLevel--;
						continue;
					}
					this.SpawnedDungeonRooms.Add(this.GenerateDungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel));
				}
			}
			foreach (var room in this.SpawnedDungeonRooms.Where(
				room => room.GetType() == typeof(DungeonRoom))) {
				DetermineDungeonRoomCategory(room as DungeonRoom);
				room.Name = RoomBuilderHelper.PopulateDungeonRoomName(room);
				room.Desc = RoomBuilderHelper.PopulateDungeonRoomDesc(room);
			}
		}

		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedDungeonRooms;
		}
		private DungeonRoom GenerateDungeonRoom(int levelRangeLow, int levelRangeHigh) {
			var oldRoom = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1];
			while (true) {
				var randomNum = GameHandler.GetRandomNumber(1, 6);
				switch (randomNum) {
					case 1:
						if (oldRoom.North == null) {
							var newRoom = new DungeonRoom(oldRoom.X, oldRoom.Y + 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.North = newRoom;
							newRoom.South = oldRoom;
							return newRoom;
						}
						break;
					case 2:
						if (oldRoom.NorthEast == null) {
							var newRoom = new DungeonRoom(
								oldRoom.X + 1, oldRoom.Y + 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.NorthEast = newRoom;
							newRoom.SouthWest = oldRoom;
							return newRoom;
						}
						break;
					case 3:
						if (oldRoom.NorthWest == null) {
							var newRoom = new DungeonRoom(
								oldRoom.X - 1, oldRoom.Y + 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.NorthWest = newRoom;
							newRoom.SouthEast = oldRoom;
							return newRoom;
						}
						break;
					case 4:
						if (oldRoom.South == null) {
							var newRoom = new DungeonRoom(oldRoom.X, oldRoom.Y - 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.South = newRoom;
							newRoom.North = oldRoom;
							return newRoom;
						}
						break;
					case 5:
						if (oldRoom.SouthEast == null) {
							var newRoom = new DungeonRoom(
								oldRoom.X + 1, oldRoom.Y - 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.SouthEast = newRoom;
							newRoom.NorthWest = oldRoom;
							return newRoom;
						}
						break;
					case 6:
						if (oldRoom.SouthWest == null) {
							var newRoom = new DungeonRoom(
								oldRoom.X - 1, oldRoom.Y - 1, oldRoom.Z, levelRangeLow, levelRangeHigh);
							oldRoom.SouthWest = newRoom;
							newRoom.NorthEast = oldRoom;
							return newRoom;
						}
						break;
				}
			}
		}
		private static void DetermineDungeonRoomCategory(DungeonRoom room) {
			var directionCount = 0;
			if (room.Up != null) directionCount++;
			if (room.Down != null) directionCount++;
			if (room.North != null) directionCount++;
			if (room.NorthEast != null) directionCount++;
			if (room.NorthWest != null) directionCount++;
			if (room.South != null) directionCount++;
			if (room.SouthEast != null) directionCount++;
			if (room.SouthWest != null) directionCount++;
			if (directionCount == 0) throw new ArgumentOutOfRangeException();
			if (room.Up != null || room.Down != null) {
				room.RoomCategory = DungeonRoom.RoomType.Stairs;
				return;
			}
			switch (directionCount) {
				case 1:
					room.RoomCategory = DungeonRoom.RoomType.Corner;
					return;
				case 2:
					room.RoomCategory = DungeonRoom.RoomType.Corridor;
					return;
				case 3:
					room.RoomCategory = DungeonRoom.RoomType.Intersection;
					return;
				default:
					room.RoomCategory = DungeonRoom.RoomType.Openspace;
					break;
			}
		}
		private static List<IRoom> BuildTown() {
			var town = new List<IRoom>();
			var name = string.Empty;
			var desc = string.Empty;
			name = "Outside Dungeon Entrance";
			desc =
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with " +
				"no lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. " +
				"Around you is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what " +
				"looks like a town. Smoke rises from a few chimneys in the distance.";
			town.Add(new TownRoom(0, 4, 0, name, desc));
			name = "Cobblestone Path";
			desc =
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			town.Add(new TownRoom(0, 5, 0, name, desc));
			town[0].North = town[1];
			town[1].South = town[0];
			desc =
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. Around you " +
				"is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			town.Add(new TownRoom(0, 6, 0, name, desc));
			town[1].North = town[2];
			town[2].South = town[1];
			name = "Town Entrance";
			desc =
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what " +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				"northwest are a few buildings with signs outside that you can't read from this distance.";
			town.Add(new TownRoom(0, 7, 0, name, desc));
			town[2].North = town[3];
			town[3].South = town[2];
			name = "Town - East";
			desc =
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside and " +
				" a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				"running a sword against a grindstone to sharpen it.";
			var npcName = "armorer";
			var npcDesc = "A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow " +
			              "as you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
			              "buy <item>, sell <item>, or <show forsale> to see what he has for sale.";
			town.Add(new TownRoom(1, 8, 0, name, desc, 
				new Vendor(npcName, npcDesc,Vendor.VendorType.Armorer)));
			town[3].NorthEast = town[4];
			town[4].SouthWest = town[3];
			desc =
				"You are in the east part of the town. A large man is in front of a building sharpening a sword against a " +
				"grindstone. To the south, you can see a small building with a forge and furnace outside. There is another " +
				"large man in front of it pounding away at a chestplate with a hammer.";
			npcName = "weaponsmith";
			npcDesc = "A large man covered in sweat sharpening a sword against a grindstone. He wipes his brow as you " +
			          "approach and wonders whether you're going to make him a little bit richer or not. You can: buy " +
			          "<item>, sell <item>, or <show forsale> to see what he has for sale.";
			town.Add(new TownRoom(1, 9, 0, name, desc, 
				new Vendor(npcName, npcDesc, Vendor.VendorType.Weaponsmith)));
			town[4].North = town[5];
			town[5].South = town[4];
			name = "Town - Center";
			desc =
				"You are in the central part of the town. There is a wrinkled old man standing in front of a small hut, his " +
				"hands clasped in the arms of his robes, as he gazes around the town calmly.";
			npcName = "healer";
			npcDesc = "An old man covered in robes looks you up and raises an eyebrow questioningly. He can rid you of all " +
			          "your pain every so often. In fact, he may even provide you with some help that will be invaluable in " +
			          "your travels. You can buy <item>, sell <item>, or <show forsale> to see what he has for sale. You can " +
			          "also try to ask him to <restore> you.";
			town.Add(new TownRoom(0, 10, 0, name, desc, 
				new Vendor(npcName, npcDesc, Vendor.VendorType.Healer)));
			town[5].NorthWest = town[6];
			town[6].SouthEast = town[5];
			name = "Town - West";
			desc =
				"You are in the west part of the town. A woman stands in front of a building with displays of various items in " +
				"front of it. It looks like she buys and sells a little bit of everything.";
			npcName = "shopkeeper";
			npcDesc =
				"A woman in casual work clothes looks at you and asks if you want to buy anything. She raises an item " +
				"to show an example of what she has for sale. You can buy <item>, sell <item>, or <show forsale> to " +
				"see what she has for sale.";
			town.Add(new TownRoom(-1, 9, 0, name, desc, 
				new Vendor(npcName, npcDesc, Vendor.VendorType.Shopkeeper)));
			town[6].SouthWest = town[7];
			town[7].NorthEast = town[6];
			desc =
				"You are in the west part of the town. There is a large, wooden building southwest of you with a sign out " +
				"front that reads 'Training'. Depending on what class you are, it appears that this place might have some " +
				"people who can help you learn more.";
			town.Add(new TownRoom(-1, 8, 0, name, desc));
			town[7].South = town[8];
			town[8].North = town[7];
			town[8].SouthEast = town[3];
			town[3].NorthWest = town[8];
			name = "Training Hall - Entrance";
			desc =
				"You are in the entrance of the training hall. To your west is a large room with training dummies and several " +
				"people hitting them with various swords, axes and other melee weapons. To your east is another large room with " +
				"training dummies. There are numerous arrows sticking out of the dummies and several people shooting the dummies " +
				"with bows. To your south is one more large room with dummies. The dummies are charred because there is someone " +
				"in a robe torching them with a fire spell.";
			town.Add(new TownRoom(-2, 7, 0, name, desc));
			town[8].SouthWest = town[9];
			town[9].NorthEast = town[8];
			name = "Training Hall - Warrior Guild";
			desc =
				"You are in a large room with training dummies and several people hitting them with various swords, axes and " +
				"other melee weapons. A grizzled old man watches the practice, his arms folded across his chest,  sometimes " +
				"nodding his head while other times cringing in disbelief. He looks like he could teach you a few things if " +
				"you have the money for lessons.";
			npcName = "warrior grandmaster";
			npcDesc =
				"A grizzled old man in a leather vest and plate gauntlets looks you up and down and wonders if you have what it " +
				"takes to be a warrior. If you're ready, he can let you train <abilityname> to learn something new or upgrade " +
				"<abilityname> to increase the rank on an ability that you already have. You can <show upgrades> to see the " +
				"full list of options.";
			town.Add(new TownRoom(-3, 7, 0, name, desc, 
				new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Warrior)));
			town[9].West = town[10];
			town[10].East = town[9];
			name = "Training Hall - Mage Guild";
			desc =
				"You are in a large room with training dummies. The dummies are being charred by a person in a robe casting a " +
				"fire spell. A middle-aged woman in an expensive-looking robe watches quietly, holding a staff upright in one " +
				"hand, while she points with her other hand at the dummy and provides corrections to the trainee's incantation. " +
				"She looks like she could teach you a few things if you have the money for lessons.";
			npcName = "mage grandmaster";
			npcDesc =
				"A middle-aged woman in an expensive-looking robe, holding a staff upright, looks you up and down and wonders " +
				"if you have the intelligence to be a mage. If you're ready, she can let you train <spellname> to learn a new " +
				"spell or upgrade <spellname> to increase the rank on a spell that you already have. You can <show upgrades> " +
				"to see the full list of options.";
			town.Add(new TownRoom(-2, 6, 0, name, desc, 
				new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Mage)));
			town[9].South = town[11];
			town[11].North = town[9];
			name = "Training Hall - Archer Guild";
			desc =
				"You are in a large room with training dummies. There are numerous arrows sticking out of the dummies and " +
				"several people shooting the dummies with bows. A young woman in leather armor looks on, voicing encouragement " +
				"to the trainees, and scolding them when she spots a bad habit. She looks like she could teach you a few things " +
				"if you have the money for lessons.";
			npcName = "archer grandmaster";
			npcDesc =
				"A young woman in leather armor glances at you while keeping a keen eye on her students. She looks like she " +
				"has extremely fast reflexes but that glance suggested that she thought you did not. She can let you train " +
				"<abilityname> to learn something new or upgrade <abilityname> to increase the rank on an ability that you " +
				"already have. You can <show upgrades> to see the full list of options.";
			town.Add(new TownRoom(-1, 7, 0, name, desc, 
				new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Archer)));
			town[9].East = town[12];
			town[12].West = town[9];
			return town;
		}
	}
}