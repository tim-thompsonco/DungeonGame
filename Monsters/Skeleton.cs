namespace DungeonGame {
  public class Skeleton : Monster, IMonster, IRoomInteraction {
    // Constructor
    public Skeleton(string name, int GoldCoins, int MaxHP, int ExpProvided, MainWeapon weapon)
      : base(name, GoldCoins, MaxHP, ExpProvided, weapon) {
    }
  }
}