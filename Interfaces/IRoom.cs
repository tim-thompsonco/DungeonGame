using System.Collections.Generic;

namespace DungeonGame {
	public interface IRoom {
		bool IsDiscovered { get; set; }
		bool GoNorth { get; set; }
		bool GoSouth { get; set; }
		bool GoEast { get; set; }
		bool GoWest { get; set; }
		bool GoNorthWest { get; set; }
		bool GoSouthWest { get; set; }
		bool GoNorthEast { get; set; }
		bool GoSouthEast { get; set; }
		bool GoUp { get; set; }
		bool GoDown { get; set; }
		string Name { get; set; }
		string Desc { get; set; }
		int X { get; set; }
		int Y { get; set; }
		int Z { get; set; }
		List<IRoomInteraction> RoomObjects { get; set; }

		IMonster GetMonster();
		bool AttackOpponent(
			Player player,
			string[] input,
			UserOutput output,
			UserOutput mapOutput,
			List<IRoom> roomList);
		void LootCorpse(Player player, string[] input, UserOutput output);
		void RebuildRoomObjects();
		void ShowDirections(UserOutput output);
		void ShowCommands(UserOutput output);
		void LookRoom(UserOutput output);
		void LookNpc(string[] input, UserOutput output);
	}
}