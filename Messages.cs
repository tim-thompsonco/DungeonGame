using DungeonGame.Controllers;

namespace DungeonGame {
	public static class Messages {
		public static void RequestCommand() {
			OutputController.StoreAnnounceMessage("Your command: ");
		}

		public static void PlayerDeath() {
			OutputController.StoreAnnounceMessage("You have died.");
		}

		public static void GameOver() {
			OutputController.StoreAnnounceMessage("Game over.");
		}

		public static void InvalidCommand() {
			OutputController.StoreFailureMessage("Not a valid command.");
		}

		public static void InvalidDirection() {
			OutputController.StoreFailureMessage("You can't go that way!");
		}

		public static void InvalidVendorSell() {
			OutputController.StoreFailureMessage("The vendor doesn't want that.");
		}

		public static void ShowGameIntro() {
			OutputController.StoreFailureMessage(
				"_________ .__                  .__                 __________        .__      ___.                          ");
			OutputController.StoreFailureMessage(
				"\\_   ___ \\|  |__ _____    _____|__| ____    ____   \\______   \\_____  |__| ____\\_ |__   ______  _  ________  ");
			OutputController.StoreFailureMessage(
				"/    \\  \\/|  |  \\__  \\  /  ___/  |/    \\  / ___\\   |       _/\\__  \\ |  |/    \\| __ \\ /  _ \\ \\/ \\/ /  ___/  ");
			OutputController.StoreFailureMessage(
				"\\     \\___|   Y  \\/ __ \\_\\___ \\|  |   |  \\/ /_/  >  |    |   \\ / __ \\|  |   |  \\ \\_\\ (  <_> )     /\\___ \\ ");
			OutputController.StoreFailureMessage(
				"\\______  /___|  (____  /____  >__|___|  /\\___  /   |____|_  /(____  /__|___|  /___  /\\____/ \\/\\_//____  > ");
			OutputController.StoreFailureMessage(
				"	\\/     \\/     \\/     \\/        \\//_____/           \\/      \\/        \\/    \\/                  \\/       \n");

			const string gameIntroString =
				"Welcome to Chasing Rainbows! This is a text-based dungeon crawler game where you can fight monsters, get loot " +
				"and explore dungeons. Stuff you've probably done a million times already across various RPG games. At any time " +
				"you can get help on commands by typing 'help'.";

			for (int i = 0; i < gameIntroString.Length; i += Settings.GetGameWidth()) {
				SetSingleOutputLineForDisplay(gameIntroString, i);
			}
		}

		private static void SetSingleOutputLineForDisplay(string outputString, int index) {
			if (outputString.Length - index < Settings.GetGameWidth()) {
				OutputController.StoreAnnounceMessage(outputString.Substring(index, outputString.Length - index));
			} else {
				OutputController.StoreAnnounceMessage(outputString.Substring(index, Settings.GetGameWidth()));
			}
		}

		public static void ShowCommandHelp() {
			const string commandHelpString =
				"_Commands: Players may move in any direction of the game using a shortkey or the full direction name. " +
				"For example, if you wish to go north, you may type either 'N' or '_North'. If a player wishes to look " +
				"at something, they can use 'l' or 'look' and then the name of what they want to look at. For example " +
				"'l zombie' or 'look zombie' would allow you to look at a zombie in the room. The same commands will  " +
				"work to loot a monster that you have killed. Look or 'L' by itself will look at the room. Other common " +
				"commands will be shown to the player. Any object that is consumable, such as a potion, can be drank " +
				"by typing 'drink' and then the name of the potion or object. To use armor or weapons, you must 'equip' " +
				"them. You can 'unequip' them as well.";

			for (int i = 0; i < commandHelpString.Length; i += Settings.GetGameWidth()) {
				SetSingleOutputLineForDisplay(commandHelpString, i);
			}
		}
	}
}