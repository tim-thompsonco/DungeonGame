namespace DungeonGame {
	public static class RoomHelper {
		public static string PopulateDungeonRoomName(IRoom originalRoom) {
			var castedRoom = originalRoom as DungeonRoom;
			return castedRoom.RoomCategory switch {
				DungeonRoom.RoomType.Corridor => "Corridor",
				DungeonRoom.RoomType.Openspace => "Dimly Lit Cavern",
				DungeonRoom.RoomType.Stairs => "Stairway",
				_ => "room type not found"
			};
		}
		public static string PopulateDungeonRoomDesc(IRoom originalRoom) {
			var castedRoom = originalRoom as DungeonRoom;
			return castedRoom.RoomCategory switch {
				DungeonRoom.RoomType.Corridor => (
					"You are in a corridor carved out of smooth rock approximately 6 feet wide and 10 feet " +
					"high. Torches in holders along the wall illuminate the hallway. The corridor stretches on for an " +
					"unknown distance, as the light from the torches cannot penetrate further than about 20 feet ahead."),
				DungeonRoom.RoomType.Openspace => (
					"You are standing in an open space in a cavern. The flooring is smooth rock. The ceiling is roughly " +
					"40 feet high and stalactites hang from the ceiling. Torches in holders along the edges of the open " +
					"space provide some illumination but it is very dark in most parts of the cavern. It almost seems " +
					"as if some of the shadows are moving. The air seems colder and thicker than it otherwise should be."),
				DungeonRoom.RoomType.Stairs => (
					"You are on a stairway. Torches line the walls to either side and a few drops of blood stain some of " +
					" the steps. There appears to be a skeleton stretched out at the bottom of the stairs, one hand " +
					"reaching out and it's wrist bones crushed by a large object, as if someone was trying to flee " +
					"and didn't make it."),
				_ => "room type not found"
			};
		}
	}
}