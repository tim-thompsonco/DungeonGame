namespace DungeonGame {
  public class Zombie : Monster, IMonster, IRoomInteraction {
    // Constructor
    public Zombie(string name, int GoldCoins, int MaxHP, int ExpProvided, MainWeapon weapon)
      : base(name, GoldCoins, MaxHP, ExpProvided, weapon) {
    }
  }
}