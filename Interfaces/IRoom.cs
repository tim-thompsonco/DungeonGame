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

		IMonster GetMonster();
		bool AttackOpponent(Player player, string[] input);
		void LootCorpse(Player player, string[] input);
		void RebuildRoomObjects();
		void ShowDirections();
		void ShowCommands();
		void LookRoom();
		void LookNpc(string[] input);
	}
}