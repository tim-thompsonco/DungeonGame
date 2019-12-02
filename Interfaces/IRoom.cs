namespace DungeonGame {
  public interface IRoom {
		bool GoNorth { get; set; }
    bool GoSouth { get; set; }
    string Name { get; set; }
    string Desc { get; set; }
    int X { get; set; }
		int Y { get; set; }
		int Z { get; set; }

    void MonsterFight(NewPlayer player);
		void LootCorpse(NewPlayer player);
		void RebuildRoomObjects();
    void ShowDirections();
    void ShowCommands();
    void LookRoom();
		void LookMonster();
  }
}