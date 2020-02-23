namespace DungeonGame {
	public static class Messages {
		public static void RequestCommand() {
			OutputHandler.Display.StoreUserOutput(Settings.FormatAnnounceText(), 
				Settings.FormatDefaultBackground(), "Your command: ");
		}
		public static void PlayerDeath() {
			OutputHandler.Display.StoreUserOutput(Settings.FormatAnnounceText(), 
				Settings.FormatDefaultBackground(), "You have died.");
		}
		public static void GameOver() {
			OutputHandler.Display.StoreUserOutput(Settings.FormatAnnounceText(), 
				Settings.FormatDefaultBackground(), "Game over.");
		}
		public static void InvalidCommand() {
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), 
				Settings.FormatDefaultBackground(),"Not a valid command.");
		}
		public static void InvalidDirection() {
			const string outputString = "You can't go that way!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static void InvalidVendorSell() {
			const string outputString = "The vendor doesn't want that.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), outputString);
		}
		public static void GameIntro() {
			const string gameIntroString = 
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot" +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.";
			for (var i = 0; i < gameIntroString.Length; i += Settings.GetGameWidth()) {
				if (gameIntroString.Length - i < Settings.GetGameWidth()) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
						gameIntroString.Substring(i, gameIntroString.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
					gameIntroString.Substring(i, Settings.GetGameWidth()));
			}
		}
		public static void ShowCommandHelp() {
			const string commandHelpString = 
				"Commands: Players may move in any direction of the game using a shortkey or the full direction name. " +
				"For example, if you wish to go north, you may type either 'N' or 'North'. If a player wishes to look " +
				"at something, they can use 'l' or 'look' and then the name of what they want to look at. For example " +
				"'l zombie' or 'look zombie' would allow you to look at a zombie in the room. The same commands will  " +
				"work to loot a monster that you have killed. Look or 'L' by itself will look at the room. Other common " +
				"commands will be shown to the player. Any object that is consumable, such as a potion, can be drank " +
				"by typing 'drink' and then the name of the potion or object. To use armor or weapons, you must 'equip' " +
				"them. You can 'unequip' them as well.";
			for (var i = 0; i < commandHelpString.Length; i += Settings.GetGameWidth()) {
				if (commandHelpString.Length - i < Settings.GetGameWidth()) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),Settings.FormatDefaultBackground(), 
						commandHelpString.Substring(i, commandHelpString.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), 
					commandHelpString.Substring(i, Settings.GetGameWidth()));
			}
		}
	}
}