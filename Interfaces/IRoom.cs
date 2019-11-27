namespace DungeonGame {
  public interface IRoom {
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