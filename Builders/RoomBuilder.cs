using DungeonGame.Controllers;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
	public class RoomBuilder {
		private Coordinate _LastRoomCoord { get; set; }
		private Dictionary<Coordinate, IRoom> _SpawnedDungeonRooms { get; set; }

		public RoomBuilder(int size, int levels, int startX, int startY, int startZ) {
			// Create town to connect dungeon to
			_SpawnedDungeonRooms = BuildTown();
			// Dungeon build settings
			int levelSize = size / levels;
			for (int i = 0; i < levels; i++) {
				int levelRangeLowForLevel = i - 1 >= 1 ? i - 1 : 1;
				int levelRangeHighForLevel = i + 2 <= 10 ? i + 1 : 10;
				for (int j = 0; j < levelSize; j++) {
					if (i == 0 && j == 0) {
						/* To connect static room to dynamic dungeon build, always have first room go down one, and the static
						 room must always be up by one for the two to connect */
						Coordinate firstRoomCoord = new Coordinate(startX, startY, startZ - 1);
						DungeonRoom firstRoom = new DungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
						Coordinate startCoord = new Coordinate(startX, startY, startZ);
						_SpawnedDungeonRooms[startCoord]._Down = firstRoom;
						firstRoom._Up = _SpawnedDungeonRooms[startCoord];
						_SpawnedDungeonRooms.Add(firstRoomCoord, firstRoom);
						_LastRoomCoord = firstRoomCoord;
						GenerateRoomDirections();
						GenerateStairwayRoomDirections();
						continue;
					}
					if (i > 0 && j == 0) {
						/* To connect upper level to lower level, always have first room go down one, and the upper level
						 room must always be up by one for the two to connect*/
						IRoom oldRoom = _SpawnedDungeonRooms[_LastRoomCoord];
						Coordinate oldCoord = _LastRoomCoord;
						Coordinate newRoomCoord = new Coordinate(oldCoord._X, oldCoord._Y, oldCoord._Z - 1);
						DungeonRoom newLevelRoom = new DungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
						oldRoom._Down = newLevelRoom;
						newLevelRoom._Up = oldRoom;
						_SpawnedDungeonRooms.Add(newRoomCoord, newLevelRoom);
						_LastRoomCoord = newRoomCoord;
						continue;
					}
					GenerateDungeonRoom(levelRangeLowForLevel, levelRangeHighForLevel);
					GenerateRoomDirections();
				}
			}
			foreach (IRoom room in _SpawnedDungeonRooms.Values.Where(
				room => room.GetType() == typeof(DungeonRoom))) {
				DetermineDungeonRoomCategory(room as DungeonRoom);
				room._Name = RoomBuilderHelper.PopulateDungeonRoomName(room);
				room._Desc = RoomBuilderHelper.PopulateDungeonRoomDesc(room);
			}
		}

		public Dictionary<Coordinate, IRoom> RetrieveSpawnRooms() {
			return _SpawnedDungeonRooms;
		}

		private void GenerateRoomDirections() {
			IRoom room = _SpawnedDungeonRooms[_LastRoomCoord];
			Coordinate coordWest = new Coordinate(_LastRoomCoord._X - 1, _LastRoomCoord._Y, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordWest)) {
				IRoom westRoom = _SpawnedDungeonRooms[coordWest];
				room._West = westRoom;
				westRoom._East = room;
			}
			Coordinate coordNorthWest = new Coordinate(
				_LastRoomCoord._X - 1, _LastRoomCoord._Y + 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordNorthWest)) {
				IRoom northWestRoom = _SpawnedDungeonRooms[coordNorthWest];
				room._NorthWest = northWestRoom;
				northWestRoom._SouthEast = room;
			}
			Coordinate coordNorth = new Coordinate(_LastRoomCoord._X, _LastRoomCoord._Y + 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordNorth)) {
				IRoom northRoom = _SpawnedDungeonRooms[coordNorth];
				room._North = northRoom;
				northRoom._South = room;
			}
			Coordinate coordNorthEast = new Coordinate(
				_LastRoomCoord._X + 1, _LastRoomCoord._Y + 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordNorthEast)) {
				IRoom northEastRoom = _SpawnedDungeonRooms[coordNorthEast];
				room._NorthEast = northEastRoom;
				northEastRoom._SouthWest = room;
			}
			Coordinate coordEast = new Coordinate(_LastRoomCoord._X + 1, _LastRoomCoord._Y, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordEast)) {
				IRoom eastRoom = _SpawnedDungeonRooms[coordEast];
				room._East = eastRoom;
				eastRoom._West = room;
			}
			Coordinate coordSouthEast = new Coordinate(
				_LastRoomCoord._X + 1, _LastRoomCoord._Y - 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordSouthEast)) {
				IRoom southEastRoom = _SpawnedDungeonRooms[coordSouthEast];
				room._SouthEast = southEastRoom;
				southEastRoom._NorthWest = room;
			}
			Coordinate coordSouth = new Coordinate(_LastRoomCoord._X, _LastRoomCoord._Y - 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordSouth)) {
				IRoom southRoom = _SpawnedDungeonRooms[coordSouth];
				room._South = southRoom;
				southRoom._North = room;
			}
			Coordinate coordSouthWest = new Coordinate(
				_LastRoomCoord._X - 1, _LastRoomCoord._Y - 1, _LastRoomCoord._Z);
			if (_SpawnedDungeonRooms.ContainsKey(coordSouthWest)) {
				IRoom southWestRoom = _SpawnedDungeonRooms[coordSouthWest];
				room._SouthWest = southWestRoom;
				southWestRoom._NorthEast = room;
			}
		}

		private void GenerateStairwayRoomDirections() {
			IRoom room = _SpawnedDungeonRooms[_LastRoomCoord];
			Coordinate coordUp = new Coordinate(_LastRoomCoord._X, _LastRoomCoord._Y, _LastRoomCoord._Z + 1);
			if (_SpawnedDungeonRooms.ContainsKey(coordUp)) {
				IRoom upRoom = _SpawnedDungeonRooms[coordUp];
				room._Up = upRoom;
				upRoom._Down = room;
			}
			Coordinate coordDown = new Coordinate(_LastRoomCoord._X, _LastRoomCoord._Y, _LastRoomCoord._Z - 1);
			if (_SpawnedDungeonRooms.ContainsKey(coordDown)) {
				IRoom downRoom = _SpawnedDungeonRooms[coordDown];
				room._Down = downRoom;
				downRoom._Up = room;
			}
		}

		private void GenerateDungeonRoom(int levelRangeLow, int levelRangeHigh) {
			Coordinate oldCoord = _LastRoomCoord;
			IRoom oldRoom = _SpawnedDungeonRooms[oldCoord];
			Coordinate newRoomCoord = _LastRoomCoord; // Placeholder until value found in while loop
			DungeonRoom newRoom = new DungeonRoom(levelRangeLow, levelRangeHigh);
			bool roomCreated = false;
			while (!roomCreated) {
				int randomNum = GameController.GetRandomNumber(1, 6);
				switch (randomNum) {
					case 1:
						if (oldRoom._North == null) {
							newRoomCoord = new Coordinate(oldCoord._X, oldCoord._Y + 1, oldCoord._Z);
							oldRoom._North = newRoom;
							newRoom._South = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
					case 2:
						if (oldRoom._NorthEast == null) {
							newRoomCoord = new Coordinate(oldCoord._X + 1, oldCoord._Y + 1, oldCoord._Z);
							oldRoom._NorthEast = newRoom;
							newRoom._SouthWest = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
					case 3:
						if (oldRoom._NorthWest == null) {
							newRoomCoord = new Coordinate(oldCoord._X - 1, oldCoord._Y + 1, oldCoord._Z);
							oldRoom._NorthWest = newRoom;
							newRoom._SouthEast = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
					case 4:
						if (oldRoom._South == null) {
							newRoomCoord = new Coordinate(oldCoord._X, oldCoord._Y - 1, oldCoord._Z);
							oldRoom._South = newRoom;
							newRoom._North = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
					case 5:
						if (oldRoom._SouthEast == null) {
							newRoomCoord = new Coordinate(oldCoord._X + 1, oldCoord._Y - 1, oldCoord._Z);
							oldRoom._SouthEast = newRoom;
							newRoom._NorthWest = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
					case 6:
						if (oldRoom._SouthWest == null) {
							newRoomCoord = new Coordinate(oldCoord._X - 1, oldCoord._Y - 1, oldCoord._Z);
							oldRoom._SouthWest = newRoom;
							newRoom._NorthEast = oldRoom;
							if (!_SpawnedDungeonRooms.ContainsKey(newRoomCoord)) {
								roomCreated = true;
							}
						}
						break;
				}
			}
			_SpawnedDungeonRooms.Add(newRoomCoord, newRoom);
			_LastRoomCoord = newRoomCoord;
		}
		private static void DetermineDungeonRoomCategory(DungeonRoom room) {
			int directionCount = 0;
			if (room._Up != null) {
				directionCount++;
			}

			if (room._Down != null) {
				directionCount++;
			}

			if (room._North != null) {
				directionCount++;
			}

			if (room._NorthEast != null) {
				directionCount++;
			}

			if (room._NorthWest != null) {
				directionCount++;
			}

			if (room._South != null) {
				directionCount++;
			}

			if (room._SouthEast != null) {
				directionCount++;
			}

			if (room._SouthWest != null) {
				directionCount++;
			}

			if (directionCount == 0) {
				throw new ArgumentOutOfRangeException();
			}

			if (room._Up != null || room._Down != null) {
				room._RoomCategory = DungeonRoom.RoomType.Stairs;
				return;
			}
			switch (directionCount) {
				case 1:
					room._RoomCategory = DungeonRoom.RoomType.Corner;
					return;
				case 2:
					room._RoomCategory = DungeonRoom.RoomType.Corridor;
					return;
				case 3:
					room._RoomCategory = DungeonRoom.RoomType.Intersection;
					return;
				default:
					room._RoomCategory = DungeonRoom.RoomType.Openspace;
					break;
			}
		}

		private static Dictionary<Coordinate, IRoom> BuildTown() {
			Dictionary<Coordinate, IRoom> town = new Dictionary<Coordinate, IRoom>();
			string name = "Outside Dungeon Entrance";
			string desc =
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with " +
				"no lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. " +
				"Around you is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what " +
				"looks like a town. Smoke rises from a few chimneys in the distance.";
			Coordinate newRoomCoord = new Coordinate(0, 4, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			name = "Cobblestone Path";
			desc =
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			Coordinate oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 5, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord]._North = town[newRoomCoord];
			town[newRoomCoord]._South = town[oldRoomCoord];
			desc =
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. Around you " +
				"is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 6, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord]._North = town[newRoomCoord];
			town[newRoomCoord]._South = town[oldRoomCoord];
			name = "Town Entrance";
			desc =
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what " +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				"northwest are a few buildings with signs outside that you can't read from this distance.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(0, 7, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			town[oldRoomCoord]._North = town[newRoomCoord];
			town[newRoomCoord]._South = town[oldRoomCoord];
			name = "Town - _East";
			desc =
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside and " +
				" a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				"running a sword against a grindstone to sharpen it.";
			string npcName = "armorer";
			string npcDesc = "A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow " +
						  "as you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
						  "buy <item>, sell <item>, or <show forsale> to see what he has for sale.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(1, 8, 0);
			town.Add(newRoomCoord,
				new TownRoom(name, desc, new Vendor(npcName, npcDesc, Vendor.VendorType.Armorer)));
			town[oldRoomCoord]._NorthEast = town[newRoomCoord];
			town[newRoomCoord]._SouthWest = town[oldRoomCoord];
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
			town[oldRoomCoord]._North = town[newRoomCoord];
			town[newRoomCoord]._South = town[oldRoomCoord];
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
			town[oldRoomCoord]._NorthWest = town[newRoomCoord];
			town[newRoomCoord]._SouthEast = town[oldRoomCoord];
			name = "Town - _West";
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
			town[oldRoomCoord]._SouthWest = town[newRoomCoord];
			town[newRoomCoord]._NorthEast = town[oldRoomCoord];
			desc =
				"You are in the west part of the town. There is a large, wooden building southwest of you with a sign out " +
				"front that reads 'Training'. Depending on what class you are, it appears that this place might have some " +
				"people who can help you learn more.";
			oldRoomCoord = newRoomCoord;
			newRoomCoord = new Coordinate(-1, 8, 0);
			town.Add(newRoomCoord, new TownRoom(name, desc));
			Coordinate roomThreeCoord = new Coordinate(0, 7, 0);
			town[oldRoomCoord]._South = town[newRoomCoord];
			town[newRoomCoord]._North = town[oldRoomCoord];
			town[newRoomCoord]._SouthEast = town[roomThreeCoord];
			town[roomThreeCoord]._NorthWest = town[newRoomCoord];
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
			town[oldRoomCoord]._SouthWest = town[newRoomCoord];
			town[newRoomCoord]._NorthEast = town[oldRoomCoord];
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
			town[oldRoomCoord]._West = town[newRoomCoord];
			town[newRoomCoord]._East = town[oldRoomCoord];
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
			town[oldRoomCoord]._South = town[newRoomCoord];
			town[newRoomCoord]._North = town[oldRoomCoord];
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
			Coordinate roomNineCoord = new Coordinate(-2, 7, 0);
			town[roomNineCoord]._East = town[newRoomCoord];
			town[newRoomCoord]._West = town[roomNineCoord];
			return town;
		}
	}
}