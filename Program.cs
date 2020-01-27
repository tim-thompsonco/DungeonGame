using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DungeonGame {
	class MainClass {
		public static void Main(string[] args) {
			try {
				Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			}
			catch (Exception) {}
			while (true) {
				// Game loading commands
				Helper.GameIntro();
				Player player;
				List<IRoom> spawnedRooms;
				try {
					player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(File.ReadAllText(
						"playersave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					spawnedRooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRoom>>(File.ReadAllText(
						"gamesave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					// Insert blank space before game reload info for formatting
					Helper.Display.StoreUserOutput(
						Helper.FormatGeneralInfoText(),
						Helper.FormatDefaultBackground(),
						"");
					Helper.Display.StoreUserOutput(
						Helper.FormatGeneralInfoText(), 
						Helper.FormatDefaultBackground(), 
						"Reloading your saved game.");
					// Insert blank space after game reload info for formatting
					Helper.Display.StoreUserOutput(
						Helper.FormatGeneralInfoText(),
						Helper.FormatDefaultBackground(),
						"");
				}
				catch (FileNotFoundException) {
					// Create dungeon
					spawnedRooms = new RoomBuilder(
						100, 5, 1, 3, 
						0, 4, 0, RoomBuilder.StartDirection.Down).RetrieveSpawnRooms();
					var playerBuilder = new PlayerBuilder();
					player = playerBuilder.BuildNewPlayer();
					GearHelper.EquipInitialGear(player);
					// Begin game by putting player at coords 0, 7, 0, town entrance
					Helper.SetPlayerLocation(spawnedRooms, player, 0, 7, 0);
					if (Helper.RoomIndex == -1) throw new InvalidOperationException();
				}
				/* Set initial room condition for player
					On loading game, display room that player starts in */
				Helper.ShowUserOutput(spawnedRooms, player);
				// Check every second to see if any effects expired or events need to execute
				var globalTimer = new Timer(
					e => Helper.CheckStatus(player, spawnedRooms), 
					null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
				while (!Helper.IsGameOver) {
					var input = Helper.GetFormattedInput(Console.ReadLine());
					InputHelper.ProcessUserInput(spawnedRooms, player, input, globalTimer);
					Console.Clear();
					Helper.ShowUserOutput(spawnedRooms, player);
				}
			}
		}
	}
}