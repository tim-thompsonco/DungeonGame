using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
	public class RoomBuilder {
		private int Size { get; set; }
		private int Levels { get; set; }
		private int CurrentLevel { get; set; }
		private Coordinate LastRoomCoord { get; set; }
		private Dictionary<Coordinate, IRoom> SpawnedDungeonRooms { get; set; }

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
						var firstRoomCoord = new Coordinate(startX, startY, startZ - 1);
						var firstRoom = new DungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
						var startCoord = new Coordinate(startX, startY, startZ);
						this.SpawnedDungeonRooms[startCoord].Down = firstRoom;
						firstRoom.Up = this.SpawnedDungeonRooms[startCoord];
						this.SpawnedDungeonRooms.Add(firstRoomCoord, firstRoom);
						this.LastRoomCoord = firstRoomCoord;
						this.GenerateRoomDirections();
						this.GenerateStairwayRoomDirections();
						this.CurrentLevel--;
						continue;
					}
					if (i > 0 && j == 0) {
						/* To connect upper level to lower level, always have first room go down one, and the upper level
						 room must always be up by one for the two to connect*/
						var oldRoom = this.SpawnedDungeonRooms[this.LastRoomCoord];
						var oldCoord = this.LastRoomCoord;
						var newRoomCoord = new Coordinate(oldCoord.X, oldCoord.Y, oldCoord.Z - 1);
						var newLevelRoom = new DungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
						oldRoom.Down = newLevelRoom;
						newLevelRoom.Up = oldRoom;
						this.SpawnedDungeonRooms.Add(newRoomCoord, newLevelRoom);
						this.LastRoomCoord = newRoomCoord;
						this.CurrentLevel--;
						continue;
					}
					this.GenerateDungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
					this.GenerateRoomDirections();
				}
			}
			foreach (var room in this.SpawnedDungeonRooms.Values.Where(
				room => room.GetType() == typeof(DungeonRoom))) {
				DetermineDungeonRoomCategory(room as DungeonRoom);
				room.Name = RoomBuilderHelper.PopulateDungeonRoomName(room);
				room.Desc = RoomBuilderHelper.PopulateDungeonRoomDesc(room);
			}
		}

		public Dictionary<Coordinate, IRoom> RetrieveSpawnRooms() {
			return this.SpawnedDungeonRooms;
		}
		private void GenerateRoomDirections() {
			var room = this.SpawnedDungeonRooms[this.LastRoomCoord];
			var coordWest = new Coordinate(this.LastRoomCoord.X - 1, this.LastRoomCoord.Y, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordWest)) {
				var westRoom = this.SpawnedDungeonRooms[coordWest];
				room.West = westRoom;
				westRoom.East = room;
			}
			var coordNorthWest = new Coordinate(
				this.LastRoomCoord.X - 1, this.LastRoomCoord.Y + 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordNorthWest)) {
				var northWestRoom = this.SpawnedDungeonRooms[coordNorthWest];
				room.NorthWest = northWestRoom;
				northWestRoom.SouthEast = room;
			}
			var coordNorth = new Coordinate(this.LastRoomCoord.X, this.LastRoomCoord.Y + 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordNorth)) {
				var northRoom = this.SpawnedDungeonRooms[coordNorth];
				room.North = northRoom;
				northRoom.South = room;
			}
			var coordNorthEast = new Coordinate(
				this.LastRoomCoord.X + 1, this.LastRoomCoord.Y + 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordNorthEast)) {
				var northEastRoom = this.SpawnedDungeonRooms[coordNorthEast];
				room.NorthEast = northEastRoom;
				northEastRoom.SouthWest = room;
			}
			var coordEast = new Coordinate(this.LastRoomCoord.X + 1, this.LastRoomCoord.Y, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordEast)) {
				var eastRoom = this.SpawnedDungeonRooms[coordEast];
				room.East = eastRoom;
				eastRoom.West = room;
			}
			var coordSouthEast = new Coordinate(
				this.LastRoomCoord.X + 1, this.LastRoomCoord.Y - 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordSouthEast)) {
				var southEastRoom = this.SpawnedDungeonRooms[coordSouthEast];
				room.SouthEast = southEastRoom;
				southEastRoom.NorthWest = room;
			}
			var coordSouth = new Coordinate(this.LastRoomCoord.X, this.LastRoomCoord.Y - 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordSouth)) {
				var southRoom = this.SpawnedDungeonRooms[coordSouth];
				room.South = southRoom;
				southRoom.North = room;
			}
			var coordSouthWest = new Coordinate(
				this.LastRoomCoord.X - 1, this.LastRoomCoord.Y - 1, this.LastRoomCoord.Z);
			if (this.SpawnedDungeonRooms.ContainsKey(coordSouthWest)) {
				var southWestRoom = this.SpawnedDungeonRooms[coordSouthWest];
				room.SouthWest = southWestRoom;
				southWestRoom.NorthEast = room;
			}
		}
		private void GenerateStairwayRoomDirections() {
			var room = this.SpawnedDungeonRooms[this.LastRoomCoord];
			var coordUp = new Coordinate(this.LastRoomCoord.X, this.LastRoomCoord.Y, this.LastRoomCoord.Z + 1);
			if (this.SpawnedDungeonRooms.ContainsKey(coordUp)) {
				var upRoom = this.SpawnedDungeonRooms[coordUp];
				room.Up = upRoom;
				upRoom.Down = room;
			}
			var coordDown = new Coordinate(this.LastRoomCoord.X, this.LastRoomCoord.Y, this.LastRoomCoord.Z - 1);
			if (this.SpawnedDungeonRooms.ContainsKey(coordDown)) {
				var downRoom = this.SpawnedDungeonRooms[coordDown];
				room.Down = downRoom;
				downRoom.Up = room;
			}
		}
		private void GenerateDungeonRoom(int levelRangeLow, int levelRangeHigh) {
			var oldCoord = this.LastRoomCoord;
			var oldRoom = this.SpawnedDungeonRooms[oldCoord];
			var newRoomCoord = this.LastRoomCoord; // Placeholder until value found in while loop
			var newRoom = new DungeonRoom(levelRangeLow, levelRangeHigh);
			var roomCreated = false;
			while (!roomCreated) {
				var randomNum = GameHandler.GetRandomNumber(1, 6);
				switch (randomNum) {
					case 1:
						if (oldRoom.North == null) {
							newRoomCoord = new Coordinate(oldCoord.X, oldCoord.Y + 1, oldCoord.Z);
							oldRoom.North = newRoom;
							newRoom.South = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
					case 2:
						if (oldRoom.NorthEast == null) {
							newRoomCoord = new Coordinate(oldCoord.X + 1, oldCoord.Y + 1, oldCoord.Z);
							oldRoom.NorthEast = newRoom;
							newRoom.SouthWest = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
					case 3:
						if (oldRoom.NorthWest == null) {
							newRoomCoord = new Coordinate(oldCoord.X - 1, oldCoord.Y + 1, oldCoord.Z);
							oldRoom.NorthWest = newRoom;
							newRoom.SouthEast = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
					case 4:
						if (oldRoom.South == null) {
							newRoomCoord = new Coordinate(oldCoord.X, oldCoord.Y - 1, oldCoord.Z);
							oldRoom.South = newRoom;
							newRoom.North = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
					case 5:
						if (oldRoom.SouthEast == null) {
							newRoomCoord = new Coordinate(oldCoord.X + 1, oldCoord.Y - 1, oldCoord.Z);
							oldRoom.SouthEast = newRoom;
							newRoom.NorthWest = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
					case 6:
						if (oldRoom.SouthWest == null) {
							newRoomCoord = new Coordinate(oldCoord.X - 1, oldCoord.Y - 1, oldCoord.Z);
							oldRoom.SouthWest = newRoom;
							newRoom.NorthEast = oldRoom;
							if (!this.SpawnedDungeonRooms.ContainsKey(newRoomCoord)) roomCreated = true;
						}
						break;
				}
			}
			this.SpawnedDungeonRooms.Add(newRoomCoord, newRoom);
			this.LastRoomCoord = newRoomCoord;
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
		private static Dictionary<Coordinate, IRoom> BuildTown() {
			var town = new Dictionary<Coordinate, IRoom>();
			var name = string.Empty;
			var desc = string.Empty;
			name = "Outside Dungeon Entrance";
			desc =
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with " +
				"no lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. " +
				"Around you is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what " +
				"looks like a town. Smoke rises from a few chimneys in the distance.";
			var newRoomCoord = new Coordinate(0, 4, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			name = "Cobblestone Path";
			desc =
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			var oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 5, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord].North = town[newRoomCoord];
			town[newRoomCoord].South = town[oldRoomCoord];
			desc =
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. Around you " +
				"is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 6, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord].North = town[newRoomCoord];
			town[newRoomCoord].South = town[oldRoomCoord];
			name = "Town Entrance";
			desc =
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what " +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				"northwest are a few buildings with signs outside that you can't read from this distance.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 7, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord].North = town[newRoomCoord];
			town[newRoomCoord].South = town[oldRoomCoord];
			name = "Town - East";
			desc =
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside and " +
				" a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				"running a sword against a grindstone to sharpen it.";
			var npcName = "armorer";
			var npcDesc = "A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow " +
			              "as you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
			              "buy <item>, sell <item>, or <show forsale> to see what he has for sale.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(1, 8, 0);
			town.Add(newRoomCoord, 
				new TownRoom(name, desc, new Vendor(npcName, npcDesc,Vendor.VendorType.Armorer)));
			town[oldRoomCoord].NorthEast = town[newRoomCoord];
			town[newRoomCoord].SouthWest = town[oldRoomCoord];
			desc =
				"You are in the east part of the town. A large man is in front of a building sharpening a sword against a " +
				"grindstone. To the south, you can see a small building with a forge and furnace outside. There is another " +
				"large man in front of it pounding away at a chestplate with a hammer.";
			npcName = "weaponsmith";
			npcDesc = "A large man covered in sweat sharpening a sword against a grindstone. He wipes his brow as you " +
			          "approach and wonders whether you're going to make him a little bit richer or not. You can: buy " +
			          "<item>, sell <item>, or <show forsale> to see what he has for sale.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(1, 9, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Vendor(npcName, npcDesc, Vendor.VendorType.Weaponsmith)));
			town[oldRoomCoord].North = town[newRoomCoord];
			town[newRoomCoord].South = town[oldRoomCoord];
			name = "Town - Center";
			desc =
				"You are in the central part of the town. There is a wrinkled old man standing in front of a small hut, his " +
				"hands clasped in the arms of his robes, as he gazes around the town calmly.";
			npcName = "healer";
			npcDesc = "An old man covered in robes looks you up and raises an eyebrow questioningly. He can rid you of all " +
			          "your pain every so often. In fact, he may even provide you with some help that will be invaluable in " +
			          "your travels. You can buy <item>, sell <item>, or <show forsale> to see what he has for sale. You can " +
			          "also try to ask him to <restore> you.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 10, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Vendor(npcName, npcDesc, Vendor.VendorType.Healer)));
			town[oldRoomCoord].NorthWest = town[newRoomCoord];
			town[newRoomCoord].SouthEast = town[oldRoomCoord];
			name = "Town - West";
			desc =
				"You are in the west part of the town. A woman stands in front of a building with displays of various items in " +
				"front of it. It looks like she buys and sells a little bit of everything.";
			npcName = "shopkeeper";
			npcDesc =
				"A woman in casual work clothes looks at you and asks if you want to buy anything. She raises an item " +
				"to show an example of what she has for sale. You can buy <item>, sell <item>, or <show forsale> to " +
				"see what she has for sale.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(-1, 9, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Vendor(npcName, npcDesc, Vendor.VendorType.Shopkeeper)));
			town[oldRoomCoord].SouthWest = town[newRoomCoord];
			town[newRoomCoord].NorthEast = town[oldRoomCoord];
			desc =
				"You are in the west part of the town. There is a large, wooden building southwest of you with a sign out " +
				"front that reads 'Training'. Depending on what class you are, it appears that this place might have some " +
				"people who can help you learn more.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(-1, 8, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			var roomThreeCoord = new Coordinate(0, 7, 0);
			town[oldRoomCoord].South = town[newRoomCoord];
			town[newRoomCoord].North = town[oldRoomCoord];
			town[newRoomCoord].SouthEast = town[roomThreeCoord];
			town[roomThreeCoord].NorthWest = town[newRoomCoord];
			name = "Training Hall - Entrance";
			desc =
				"You are in the entrance of the training hall. To your west is a large room with training dummies and several " +
				"people hitting them with various swords, axes and other melee weapons. To your east is another large room with " +
				"training dummies. There are numerous arrows sticking out of the dummies and several people shooting the dummies " +
				"with bows. To your south is one more large room with dummies. The dummies are charred because there is someone " +
				"in a robe torching them with a fire spell.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(-2, 7, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord].SouthWest = town[newRoomCoord];
			town[newRoomCoord].NorthEast = town[oldRoomCoord];
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
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(-3, 7, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Warrior)));
			town[oldRoomCoord].West = town[newRoomCoord];
			town[newRoomCoord].East = town[oldRoomCoord];
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
			oldRoomCoord = new Coordinate(-2, 7, 0);
			newRoomCoord = new Coordinate(-2, 6, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Mage)));
			town[oldRoomCoord].South = town[newRoomCoord];
			town[newRoomCoord].North = town[oldRoomCoord];
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
			newRoomCoord = new Coordinate(-1, 7, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Trainer(npcName, npcDesc, Trainer.TrainerCategory.Archer)));
			var roomNineCoord = new Coordinate(-2, 7, 0);
			town[roomNineCoord].East = town[newRoomCoord];
			town[newRoomCoord].West = town[roomNineCoord];
			return town;
		}
	}
}