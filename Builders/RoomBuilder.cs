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

		private StartDirection StartDir { get; set; }
		private int Size { get; set; }
		private int Levels { get; set; }
		private int XCoord { get; set; }
		private int YCoord { get; set; }
		private int ZCoord { get; set; }
		private int CurrentLevel { get; set; }
		private bool GoNorth { get; set; }
		private bool GoSouth { get; set; }
		private bool GoEast { get; set; }
		private bool GoWest { get; set; }
		private bool GoNorthWest { get; set; }
		private bool GoSouthWest { get; set; }
		private bool GoNorthEast { get; set; }
		private bool GoSouthEast { get; set; }
		private bool GoUp { get; set; }
		private bool GoDown { get; set; }
		private List<IRoom> SpawnedDungeonRooms { get; set; }

		private RoomBuilder() {
			this.SpawnedDungeonRooms = new List<IRoom>();
			var townRooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRoom>>(File.ReadAllText(
				"townrooms.json"), new Newtonsoft.Json.JsonSerializerSettings {
				TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
			});
			foreach (var room in townRooms) {
				this.SpawnedDungeonRooms.Add(room);
			}
		}
		public RoomBuilder(int size, int levels, int startX, int startY, int startZ, StartDirection startDir)
			: this() {
			/* Dungeon difficulty settings
			Commenting out for now because I am setting each level to be equal to difficulty of i, subject to some
			variation, and later on if I build out this project more I will reuse this code to make the level ranges
			dynamic again when I build multiple dungeons
			this.LevelRangeLow = levelRangeLow;
			this.LevelRangeHigh = levelRangeHigh; */
			// Dungeon build settings
			this.StartDir = startDir;
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
						this.GenerateStartRoomDirections();
						var firstRoom = new DungeonRoom(startX, startY,startZ - 1, 
							this.GoNorth, this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, 
							this.GoNorthEast, this.GoSouthEast, this.GoUp, this.GoDown, levelRangeLowForLevel, levelRangeHighForLevel);
						this.SpawnedDungeonRooms.Add(firstRoom);
						this.ResetRoomDirections();
						this.CurrentLevel--;
						continue;
					}
					if (i > 0 && j == 0) {
						/* To connect upper level to lower level, always have first room go down one, and the upper level
						 room must always be up by one for the two to connect, so X/Y coords are same but Z coord is -1
						 from beginning Z coord */
						this.XCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X;
						this.YCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y;
						this.ZCoord = this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Z - 1;
						this.GenerateStairwayRoomDirections();
						var firstRoom = new DungeonRoom(this.XCoord, this.YCoord, this.ZCoord, this.GoNorth, 
							this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, this.GoNorthEast, 
							this.GoSouthEast, this.GoUp, this.GoDown, levelRangeLowForLevel, levelRangeHighForLevel);
						this.SpawnedDungeonRooms.Add(firstRoom);
						this.ResetRoomDirections();
						this.CurrentLevel--;
						continue;
					}
					this.GenerateRandomCoords();
					this.GenerateRoomDirections();
					var newRoom = new DungeonRoom(this.XCoord, this.YCoord, this.ZCoord, this.GoNorth, 
						this.GoSouth, this.GoEast, this.GoWest, this.GoNorthWest, this.GoSouthWest, this.GoNorthEast, 
						this.GoSouthEast, this.GoUp, this.GoDown, levelRangeLowForLevel, levelRangeHighForLevel);
					this.SpawnedDungeonRooms.Add(newRoom);
					this.ResetRoomDirections();
				}
			}
			foreach (var room in this.SpawnedDungeonRooms.Where(
				room => room.GetType() == typeof(DungeonRoom))) {
				DetermineDungeonRoomCategory(room);
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
				this.XCoord = GameHandler.GetRandomNumber(
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X - 1, 
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].X + 1);
				this.YCoord = GameHandler.GetRandomNumber(
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y - 1, 
					this.SpawnedDungeonRooms[this.SpawnedDungeonRooms.Count - 1].Y + 1);
				this.ZCoord = this.CurrentLevel;
				if (this.FindRoomCordsIndex(this.XCoord, this.YCoord, this.ZCoord) != -1) continue;
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
			var checkWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord, this.ZCoord);
			if (checkWest != -1) {
				this.SpawnedDungeonRooms[checkWest].GoEast = true;
				this.GoWest = true;
			}
			var checkNorthWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord + 1, this.ZCoord);
			if (checkNorthWest != -1) {
				this.SpawnedDungeonRooms[checkNorthWest].GoSouthEast = true;
				this.GoNorthWest = true;
			}
			var checkNorth = this.FindRoomCordsIndex(this.XCoord, this.YCoord + 1, this.ZCoord);
			if (checkNorth != -1) {
				this.SpawnedDungeonRooms[checkNorth].GoSouth = true;
				this.GoNorth = true;
			}
			var checkNorthEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord + 1, this.ZCoord);
			if (checkNorthEast != -1) {
				this.SpawnedDungeonRooms[checkNorthEast].GoSouthWest = true;
				this.GoNorthEast = true;
			}
			var checkEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord, this.ZCoord);
			if (checkEast != -1) {
				this.SpawnedDungeonRooms[checkEast].GoWest = true;
				this.GoEast = true;
			}
			var checkSouthEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord - 1, this.ZCoord);
			if (checkSouthEast != -1) {
				this.SpawnedDungeonRooms[checkSouthEast].GoNorthWest = true;
				this.GoSouthEast = true;
			}
			var checkSouth = this.FindRoomCordsIndex(this.XCoord, this.YCoord - 1, this.ZCoord);
			if (checkSouth != -1) {
				this.SpawnedDungeonRooms[checkSouth].GoNorth = true;
				this.GoSouth = true;
			}
			var checkSouthWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord - 1, this.ZCoord);
			if (checkSouthWest == -1) return;
			this.SpawnedDungeonRooms[checkSouthWest].GoNorthEast = true;
			this.GoSouthWest = true;
		}
		private void GenerateStairwayRoomDirections() {
			var checkWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord, this.ZCoord);
			if (checkWest != -1) {
				this.SpawnedDungeonRooms[checkWest].GoEast = true;
				this.GoWest = true;
			}
			var checkNorthWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord + 1, this.ZCoord);
			if (checkNorthWest != -1) {
				this.SpawnedDungeonRooms[checkNorthWest].GoSouthEast = true;
				this.GoNorthWest = true;
			}
			var checkNorth = this.FindRoomCordsIndex(this.XCoord, this.YCoord + 1, this.ZCoord);
			if (checkNorth != -1) {
				this.SpawnedDungeonRooms[checkNorth].GoSouth = true;
				this.GoNorth = true;
			}
			var checkNorthEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord + 1, this.ZCoord);
			if (checkNorthEast != -1) {
				this.SpawnedDungeonRooms[checkNorthEast].GoSouthWest = true;
				this.GoNorthEast = true;
			}
			var checkEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord, this.ZCoord);
			if (checkEast != -1) {
				this.SpawnedDungeonRooms[checkEast].GoWest = true;
				this.GoEast = true;
			}
			var checkSouthEast = this.FindRoomCordsIndex(this.XCoord + 1, this.YCoord - 1, this.ZCoord);
			if (checkSouthEast != -1) {
				this.SpawnedDungeonRooms[checkSouthEast].GoNorthWest = true;
				this.GoSouthEast = true;
			}
			var checkSouth = this.FindRoomCordsIndex(this.XCoord, this.YCoord - 1, this.ZCoord);
			if (checkSouth != -1) {
				this.SpawnedDungeonRooms[checkSouth].GoNorth = true;
				this.GoSouth = true;
			}
			var checkSouthWest = this.FindRoomCordsIndex(this.XCoord - 1, this.YCoord - 1, this.ZCoord);
			if (checkSouthWest != -1) {
				this.SpawnedDungeonRooms[checkSouthWest].GoNorthEast = true;
				this.GoSouthWest = true;
			}
			var checkUp = this.FindRoomCordsIndex(this.XCoord, this.YCoord, this.ZCoord + 1);
			if (checkUp != -1) {
				this.SpawnedDungeonRooms[checkUp].GoDown = true;
				this.GoUp = true;
			}
			var checkDown = this.FindRoomCordsIndex(this.XCoord, this.YCoord, this.ZCoord - 1);
			if (checkDown == -1) return;
			this.SpawnedDungeonRooms[checkDown].GoUp = true;
			this.GoDown = true;
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
		private static void DetermineDungeonRoomCategory(IRoom originalRoom) {
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