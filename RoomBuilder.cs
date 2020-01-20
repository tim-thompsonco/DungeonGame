using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DungeonGame {
	public class RoomBuilder {
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
		public List<IRoom> SpawnedDungeonRooms { get; set; }

		public RoomBuilder() {
			this.SpawnedDungeonRooms = new List<IRoom>();
			var townRooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRoom>>(File.ReadAllText(
				"townrooms.json"), new Newtonsoft.Json.JsonSerializerSettings {
				TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
			});
			foreach (var room in townRooms) {
				this.SpawnedDungeonRooms.Add(room);
			}
		}
		public RoomBuilder(int size, int levels, int levelRangeLow, int levelRangeHigh, 
			int startX, int startY, int startZ, StartDirection startDir)
			: this() {
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
			foreach (var room in this.SpawnedDungeonRooms.Where(
				room => room.GetType() == typeof(DungeonRoom))) {
				this.DetermineDungeonRoomCategory(room);
				room.Name = RoomBuilderHelper.PopulateDungeonRoomName(room);
				room.Desc = RoomBuilderHelper.PopulateDungeonRoomDesc(room);
			}
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
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X + 1);
				this.yCoord = Helper.GetRandomNumber(
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y - 1, 
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y + 1);
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
		private void DetermineDungeonRoomCategory(IRoom originalRoom) {
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
			switch (directionCount) {
				case 1:
					castedRoom.RoomCategory = DungeonRoom.RoomType.Corner;
					return;
				case 2:
					castedRoom.RoomCategory = DungeonRoom.RoomType.Corridor;
					return;
				case 3:
					castedRoom.RoomCategory = DungeonRoom.RoomType.Intersection;
					return;
				default:
					castedRoom.RoomCategory = DungeonRoom.RoomType.Openspace;
					break;
			}
		}
	}
}