namespace DungeonGame {
  public interface IRoom {
		bool GoEast { get; set; }
    bool GoWest { get; set; }
		bool GoNorth { get; set; }
    bool GoSouth { get; set; }
    string Name { get; set; }
    string Desc { get; set; }
    int LocationKey { get; set; }

    void LootCorpse(NewPlayer player);
    void MonsterFight(NewPlayer player);
    void RebuildRoomObjects();
    void ShowDirections();
    void ShowCommands();
    void LookRoom();
  }
}