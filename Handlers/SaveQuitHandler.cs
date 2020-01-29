using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DungeonGame {
	public static class SaveQuitHandler {
		public static bool QuitGame(Player player) {
			RoomHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(), "Are you sure you want to quit?");
			RoomHandler.Display.BuildUserOutput();
			RoomHandler.Display.RetrieveUserOutput();
			RoomHandler.Display.ClearUserOutput();
			var input = InputHandler.GetFormattedInput(Console.ReadLine());
			if (input[0] == "yes" || input[0] == "y") {
				RoomHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), 
					Settings.FormatDefaultBackground(), "Quitting the game.");
				player.CanSave = true;
				SaveGame(player);
				return true;
			}
			return false;
		}
		public static void SaveGame(Player player) {
			string outputString;
			if (player.CanSave) {
				var serializerPlayer = new JsonSerializer();
				serializerPlayer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializerPlayer.NullValueHandling = NullValueHandling.Ignore;
				serializerPlayer.TypeNameHandling = TypeNameHandling.Auto;
				serializerPlayer.Formatting = Formatting.Indented;
				serializerPlayer.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (var sw = new StreamWriter("playersave.json"))
				using (var writer = new JsonTextWriter(sw)) {
					serializerPlayer.Serialize(writer, player, typeof(Player));
				}
				var serializerRooms = new JsonSerializer();
				serializerRooms.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
				serializerRooms.NullValueHandling = NullValueHandling.Ignore;
				serializerRooms.TypeNameHandling = TypeNameHandling.Auto;
				serializerRooms.Formatting = Formatting.Indented;
				serializerRooms.PreserveReferencesHandling = PreserveReferencesHandling.All;
				using (var sw = new StreamWriter("gamesave.json"))
				using (var writer = new JsonTextWriter(sw)) {
					serializerPlayer.Serialize(writer, RoomHandler.Rooms, typeof(List<IRoom>));
				}
				outputString = "Your game has been saved.";
				RoomHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
				RoomHandler.Display.BuildUserOutput();
				RoomHandler.Display.RetrieveUserOutput();
				RoomHandler.Display.ClearUserOutput();
				return;
			}
			outputString = "You can't save inside a dungeon! Go outside first.";
			RoomHandler.Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), outputString);
		}
	}
}