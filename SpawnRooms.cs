using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class SpawnRooms {
		public enum StartDirection {
			Up,
			Down
		}
		public StartDirection StartDir { get; set; }
		public int Size { get; set; }
		public int Levels { get; set; }
		public int LevelRangeLow { get; set; }
		public int LevelRangeHigh { get; set; }
		public int xCoord { get; set; }
		public int yCoord { get; set; }
		public int zCoord { get; set; }
		public int CurrentLevel { get; set; }
		public bool GoNorth { get; set; }
		public bool GoSouth { get; set; }
		public bool GoEast { get; set; }
		public bool GoWest { get; set; }
		public bool GoNorthWest { get; set; }
		public bool GoSouthWest { get; set; }
		public bool GoNorthEast { get; set; }
		public bool GoSouthEast { get; set; }
		public bool GoUp { get; set; }
		public bool GoDown { get; set; }
		public List<IRoom> SpawnedDungeonRooms { get; set; } = new List<IRoom>();

		public SpawnRooms(int size, int levels, int levelRangeLow, int levelRangeHigh, 
			int startX, int startY, int startZ, StartDirection startDir) {
			// Dungeon difficulty settings
			this.LevelRangeLow = levelRangeLow;
			this.LevelRangeHigh = levelRangeHigh;
			// Dungeon build settings
			this.StartDir = startDir;
			this.Size = size;
			this.Levels = levels;
			var levelSize = size / levels;
			for (var i = 0; i < levels; i++) {
				for (var j = 0; j < levelSize; j++) {
					if (i == 0 && j == 0) {
						/* To connect static room to dynamic dungeon build, always have first room go down one, and the static
						 room must always be up by one for the two to connect */
						this.GenerateStartRoomDirections();
						var firstRoom = new DungeonRoom(startX, startY,startZ - 1, 
							this.GoNorth, this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, 
							this.GoNorthEast, this.GoSouthEast, this.GoUp, this.GoDown, this.LevelRangeLow, this.LevelRangeHigh);
						this.SpawnedDungeonRooms.Add(firstRoom);
						this.ResetRoomDirections();
						this.CurrentLevel--;
						continue;
					}
					if (i > 0 && j == 0) {
						/* To connect upper level to lower level, always have first room go down one, and the upper level
						 room must always be up by one for the two to connect, so X/Y coords are same but Z coord is -1
						 from beginning Z coord */
						this.xCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X;
						this.yCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y;
						this.zCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Z - 1;
						this.GenerateStairwayRoomDirections();
						var firstRoom = new DungeonRoom(this.xCoord, this.yCoord, this.zCoord, this.GoNorth, 
							this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, this.GoNorthEast, 
							this.GoSouthEast, this.GoUp, this.GoDown, this.LevelRangeLow, this.LevelRangeHigh);
						this.SpawnedDungeonRooms.Add(firstRoom);
						this.ResetRoomDirections();
						this.CurrentLevel--;
						continue;
					}
					this.GenerateRandomCoords();
					this.GenerateRoomDirections();
					var newRoom = new DungeonRoom(this.xCoord, this.yCoord, this.zCoord, this.GoNorth, 
						this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, this.GoNorthEast, 
						this.GoSouthEast, this.GoUp, this.GoDown, this.LevelRangeLow, this.LevelRangeHigh);
					this.SpawnedDungeonRooms.Add(newRoom);
					this.ResetRoomDirections();
				}
			}
			foreach (var room in this.SpawnedDungeonRooms) {
				this.DetermineRoomCategory(room);
				room.Name = RoomHelper.PopulateDungeonRoomName(room);
				room.Desc = RoomHelper.PopulateDungeonRoomDesc(room);
			}
			var room110 = new TownRoom(
				"Outside Dungeon Entrance",
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with no " +
				"lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. Around you " +
				"is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what looks like a " +
				"town. Smoke rises from a few chimneys in the distance.", 
				0, 
				4, 
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
				true); 
			this.SpawnedDungeonRooms.Add(room110);
			var room111 = new TownRoom(
				"Cobblestone Path",
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock. ", 
				0, 
				5, 
				0, 
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
			this.SpawnedDungeonRooms.Add(room111);
			var room112 = new TownRoom(
				"Cobblestone Path",
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. " +
				"Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock. ", 
				0, 
				6, 
				0, 
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
			this.SpawnedDungeonRooms.Add(room112);
			var room113 = new TownRoom(
				"Town Entrance",
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what " +
				"sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the " +
				"northwest are a few buildings with signs outside that you can't read from this distance.", 
				0, 
				7, 
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
				false); 
			this.SpawnedDungeonRooms.Add(room113);
			var room114 = new TownRoom(
				"Town - East",
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside " +
				"and a large man pounding away at a chestplate with a hammer. One building over you can see another large man " +
				"running a sword against a grindstone to sharpen it.", 
				1, 
				8, 
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
				new Vendor(
					"armorer",
					"A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow as " +
					"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
					"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
					"Armor")); 
			this.SpawnedDungeonRooms.Add(room114);
			var room115 = new TownRoom(
			"Town - East",
			"You are in the east part of the town. A large man is in front of a building sharpening a sword against " +
			"a grindstone. To the south, you can see a small building with a forge and furnace outside. There is " +
			"another large man in front of it pounding away at a chestplate with a hammer.", 
			1, 
			9, 
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
			new Vendor(
			"weaponsmith",
			"A large man covered in sweat sharpening a sword against a grindstone. He wipes his brow as " +
			"you approach and wonders whether you're going to make him a little bit richer or not. You can: " +
			"buy <item>, sell <item>, or <show forsale> to see what he has for sale.", 
			"Weapon")); 
			this.SpawnedDungeonRooms.Add(room115);
			var room116 = new TownRoom(
				"Town - Center",
				"You are in the central part of the town. There is a wrinkled old man standing in front of " +
				"a small hut, his hands clasped in the arms of his robes, as he gazes around the town calmly. ", 
				0, 
				10, 
				0, 
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
			this.SpawnedDungeonRooms.Add(room116);
			var room117 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. A woman stands in front of a building with displays " +
				"of various items in front of it. It looks like she buys and sells a little bit of everything.", 
				-1, 
				9, 
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
				new Vendor(
				"shopkeeper",
				"A woman in casual work clothes looks at you and asks if you want to buy anything. She raises " +
				"an item to show an example of what she has for sale. You can buy <item>, sell <item>, or " +
				"<show forsale> to see what she has for sale.", 
				"Shopkeeper")); 
			this.SpawnedDungeonRooms.Add(room117);
			var room118 = new TownRoom(
				"Town - West",
				"You are in the west part of the town. There is a large, wooden building in front of you with a " +
				"sign out front that reads 'Training'. Depending on what class you are, it appears that this " +
				"place might have some people who can help you learn more.", 
				-1, 
				8, 
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
				false);
			this.SpawnedDungeonRooms.Add(room118);
		}

		public List<IRoom> RetrieveSpawnRooms() {
			return this.SpawnedDungeonRooms;
		}
		private int FindRoomCordsIndex(int xCoord, int yCoord, int zCoord) {
			var roomName = this.SpawnedDungeonRooms.Find(
				f => f.X == xCoord && f.Y == yCoord && f.Z == zCoord);
			var roomIndex = this.SpawnedDungeonRooms.IndexOf(roomName);
			return roomIndex;
		}
		private void GenerateRandomCoords() {
			while (true) {
				this.xCoord = Helper.GetRandomNumber(
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X - 1, 
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X + 2);
				this.yCoord = Helper.GetRandomNumber(
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y - 1, 
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y + 2);
				this.zCoord = this.CurrentLevel;
				if (this.FindRoomCordsIndex(this.xCoord, this.yCoord, this.zCoord) != -1) continue;
				break;
			}
		}
		private void GenerateStartRoomDirections() {
			switch (this.StartDir) {
				case StartDirection.Up:
					this.GoDown = true;
					break;
				case StartDirection.Down:
					this.GoUp = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}			
		}
		private void GenerateRoomDirections() {
			var checkWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord, this.zCoord);
			if (checkWest != -1) {
				this.SpawnedDungeonRooms[checkWest].GoEast = true;
				this.GoWest = true;
			}
			var checkNorthWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord + 1, this.zCoord);
			if (checkNorthWest != -1) {
				this.SpawnedDungeonRooms[checkNorthWest].GoSouthEast = true;
				this.GoNorthWest = true;
			}
			var checkNorth = this.FindRoomCordsIndex(this.xCoord, this.yCoord + 1, this.zCoord);
			if (checkNorth != -1) {
				this.SpawnedDungeonRooms[checkNorth].GoSouth = true;
				this.GoNorth = true;
			}
			var checkNorthEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord + 1, this.zCoord);
			if (checkNorthEast != -1) {
				this.SpawnedDungeonRooms[checkNorthEast].GoSouthWest = true;
				this.GoNorthEast = true;
			}
			var checkEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord, this.zCoord);
			if (checkEast != -1) {
				this.SpawnedDungeonRooms[checkEast].GoWest = true;
				this.GoEast = true;
			}
			var checkSouthEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord - 1, this.zCoord);
			if (checkSouthEast != -1) {
				this.SpawnedDungeonRooms[checkSouthEast].GoNorthWest = true;
				this.GoSouthEast = true;
			}
			var checkSouth = this.FindRoomCordsIndex(this.xCoord, this.yCoord - 1, this.zCoord);
			if (checkSouth != -1) {
				this.SpawnedDungeonRooms[checkSouth].GoNorth = true;
				this.GoSouth = true;
			}
			var checkSouthWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord - 1, this.zCoord);
			if (checkSouthWest != -1) {
				this.SpawnedDungeonRooms[checkSouthWest].GoNorthEast = true;
				this.GoSouthWest = true;
			}
		}
		private void GenerateStairwayRoomDirections() {
			var checkWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord, this.zCoord);
			if (checkWest != -1) {
				this.SpawnedDungeonRooms[checkWest].GoEast = true;
				this.GoWest = true;
			}
			var checkNorthWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord + 1, this.zCoord);
			if (checkNorthWest != -1) {
				this.SpawnedDungeonRooms[checkNorthWest].GoSouthEast = true;
				this.GoNorthWest = true;
			}
			var checkNorth = this.FindRoomCordsIndex(this.xCoord, this.yCoord + 1, this.zCoord);
			if (checkNorth != -1) {
				this.SpawnedDungeonRooms[checkNorth].GoSouth = true;
				this.GoNorth = true;
			}
			var checkNorthEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord + 1, this.zCoord);
			if (checkNorthEast != -1) {
				this.SpawnedDungeonRooms[checkNorthEast].GoSouthWest = true;
				this.GoNorthEast = true;
			}
			var checkEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord, this.zCoord);
			if (checkEast != -1) {
				this.SpawnedDungeonRooms[checkEast].GoWest = true;
				this.GoEast = true;
			}
			var checkSouthEast = this.FindRoomCordsIndex(this.xCoord + 1, this.yCoord - 1, this.zCoord);
			if (checkSouthEast != -1) {
				this.SpawnedDungeonRooms[checkSouthEast].GoNorthWest = true;
				this.GoSouthEast = true;
			}
			var checkSouth = this.FindRoomCordsIndex(this.xCoord, this.yCoord - 1, this.zCoord);
			if (checkSouth != -1) {
				this.SpawnedDungeonRooms[checkSouth].GoNorth = true;
				this.GoSouth = true;
			}
			var checkSouthWest = this.FindRoomCordsIndex(this.xCoord - 1, this.yCoord - 1, this.zCoord);
			if (checkSouthWest != -1) {
				this.SpawnedDungeonRooms[checkSouthWest].GoNorthEast = true;
				this.GoSouthWest = true;
			}
			var checkUp = this.FindRoomCordsIndex(this.xCoord, this.yCoord, this.zCoord + 1);
			if (checkUp != -1) {
				this.SpawnedDungeonRooms[checkUp].GoDown = true;
				this.GoUp = true;
			}
			var checkDown = this.FindRoomCordsIndex(this.xCoord, this.yCoord, this.zCoord - 1);
			if (checkDown != -1) {
				this.SpawnedDungeonRooms[checkDown].GoUp = true;
				this.GoDown = true;
			}
		}
		private void ResetRoomDirections() {
			this.GoEast = false;
			this.GoNorthEast = false;
			this.GoSouthEast = false;
			this.GoWest = false;
			this.GoNorthWest = false;
			this.GoSouthWest = false;
			this.GoNorth = false;
			this.GoSouth = false;
			this.GoUp = false;
			this.GoDown = false;
		}
		private void DetermineRoomCategory(IRoom originalRoom) {
			var castedRoom = originalRoom as DungeonRoom;
			var directionCount = 0;
			if (castedRoom.GoUp) directionCount++;
			if (castedRoom.GoDown) directionCount++;
			if (castedRoom.GoEast) directionCount++;
			if (castedRoom.GoWest) directionCount++;
			if (castedRoom.GoNorth) directionCount++;
			if (castedRoom.GoSouth) directionCount++;
			if (castedRoom.GoNorthEast) directionCount++;
			if (castedRoom.GoNorthWest) directionCount++;
			if (castedRoom.GoSouthEast) directionCount++;
			if (castedRoom.GoSouthWest) directionCount++;
			if (directionCount == 0) throw new ArgumentOutOfRangeException();
			if (castedRoom.GoUp || castedRoom.GoDown) {
				castedRoom.RoomCategory = DungeonRoom.RoomType.Stairs;
				return;
			}
			if (directionCount <= 2) {
				castedRoom.RoomCategory = DungeonRoom.RoomType.Corridor;
			}
			else {
				castedRoom.RoomCategory = DungeonRoom.RoomType.Openspace;
			}
		}
	}
}