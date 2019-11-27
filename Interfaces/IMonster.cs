namespace DungeonGame {
  public interface IMonster {
    string name { get; set; }
    int maxHitPoints { get; set; }
    int hitPoints { get; set; }
    int experienceProvided { get; set; }
    int gold { get; set; }
    bool onFire { get; set; }
    bool wasLooted { get; set; }

    void TakeDamage(int weaponDamage);
    void DisplayStats();
    
    int Attack();
  }
}