using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame {
  class MainClass {
		public static void Main(string[] args) {
			Helper.GameIntro();
			var player = new NewPlayer(Helper.FetchPlayerName());
			while(true) {
				player.DisplayPlayerStatsAll();
				var outcome = MonsterEncounter(player);
				if(outcome == true) {
					continue;
				}
				else {
					Helper.PlayerDeath();
					break;
				}
			}
		}
		static bool MonsterEncounter(NewPlayer player) {
			var zombie = new Monster();
			Console.WriteLine("You encountered a monster. What do you do? '[F]ight' would be a good idea.");
			Helper.RequestCommand();
			var input = Helper.GetFormattedInput();
			var outcome = false;
			if (input == "f") {
				outcome = player.Combat(zombie);
			}
			if (outcome == true) {
				Console.WriteLine("You have defeated the monster!");
				player.GainExperience(zombie.GiveExperience());
				return true;
			}
			else {
				return false;
			}
		}
	}
}