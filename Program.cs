using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame
{
  class MainClass {
  public static void Main(string[] args) {
    var player = new NewPlayer();
    Helper.RequestCommand();
    MonsterEncounter(player);
  }
	static void MonsterEncounter(Player player) {
		var zombie = new Monster();
		Console.WriteLine("You encountered a monster. What do you do? '[F]ight' would be a good idea.");
    Helper.RequestCommand();
    var input = Helper.GetFormattedInput();
    if (input == "f") {
      player.Combat(zombie);
      }
    if (player.CheckHealth() <= 0) {
      Helper.PlayerDeath();
			return;
      }
	}
}
}