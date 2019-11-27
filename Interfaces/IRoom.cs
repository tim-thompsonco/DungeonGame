namespace DungeonGame {
  public interface IRoom {
		bool goNorth { get; set; }
    bool goSouth { get; set; }
    string name { get; set; }
    string desc { get; set; }
    int locationKey { get; set; }

    void LootCorpse(NewPlayer player);
    void MonsterFight(NewPlayer player);
    void RebuildRoomObjects();
    void ShowDirections();
    void ShowCommands();
    void LookRoom();
  }
}