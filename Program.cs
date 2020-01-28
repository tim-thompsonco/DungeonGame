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
			catch (Exception ex) {
				Helper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					"The game window could not be resized on your computer. Error: " + ex);
			}
			while (true) {
				// Game loading commands
				Helper.GameIntro();
				Player player;
				try {
					player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(File.ReadAllText(
						"playersave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					Helper.Rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IRoom>>(File.ReadAllText(
						"gamesave.json"), new Newtonsoft.Json.JsonSerializerSettings {
						TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
						NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					});
					// Insert blank space before game reload info for formatting
					Helper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						"");
					Helper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(), 
						Settings.FormatDefaultBackground(), 
						"Reloading your saved game.");
					// Insert blank space after game reload info for formatting
					Helper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						"");
				}
				catch (FileNotFoundException) {
					// Create dungeon
					Helper.Rooms = new RoomBuilder(
						100, 5, 1, 3, 
						0, 4, 0, RoomBuilder.StartDirection.Down).RetrieveSpawnRooms();
					var playerBuilder = new PlayerBuilder();
					player = playerBuilder.BuildNewPlayer();
					GearHelper.EquipInitialGear(player);
					// Begin game by putting player at coords 0, 7, 0, town entrance
					Helper.SetPlayerLocation(player, 0, 7, 0);
					if (Helper.RoomIndex == -1) throw new InvalidOperationException();
				}
				/* Set initial room condition for player
					On loading game, display room that player starts in */
				Helper.ShowUserOutput(player);
				// Check every second to see if any effects expired or events need to execute
				var globalTimer = new Timer(
					e => Helper.CheckStatus(player), 
					null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
				while (!Helper.IsGameOver) {
					var input = Helper.GetFormattedInput(Console.ReadLine());
					InputProcessor.ProcessUserInput(player, input, globalTimer);
					Console.Clear();
					Helper.ShowUserOutput(player);
				}
			}
		}
	}
}