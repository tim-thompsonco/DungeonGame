using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame {
	public class PlayerBuilder {
		public Player BuildNewPlayer() {
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			Helper.Display.StoreUserOutput(
				Helper.FormatAnnounceText(), Helper.FormatDefaultBackground(), "Please enter a player name.");
			string playerName;
			while (true) {
				var sameLineOutput = new List<string>();
				sameLineOutput.Add(Helper.FormatAnnounceText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("Player name: ");
				Helper.Display.StoreUserOutput(sameLineOutput);
				Helper.Display.RetrieveUserOutput();
				playerName = textInfo.ToTitleCase(Console.ReadLine());
				Helper.Display.ClearUserOutput();
				var playerNameString = "Your player name is " + playerName + ", is that correct? [Y] or [N].";
				Helper.Display.StoreUserOutput(
					Helper.FormatAnnounceText(), Helper.FormatDefaultBackground(), playerNameString);
				Helper.Display.RetrieveUserOutput();
				Helper.RequestCommand();
				var input = Helper.GetFormattedInput(Console.ReadLine());
				Helper.Display.ClearUserOutput();
				if (input[0] == "y") {
					break;
				}
			}
			while (true) {
				Helper.Display.StoreUserOutput(
					Helper.FormatAnnounceText(),
					Helper.FormatDefaultBackground(),
					"Please enter your class. You can select Mage, Warrior, or Archer.");
				var sameLineOutputClass = new List<string>();
				sameLineOutputClass.Add(Helper.FormatAnnounceText());
				sameLineOutputClass.Add(Helper.FormatDefaultBackground());
				sameLineOutputClass.Add("Player class: ");
				Helper.Display.StoreUserOutput(sameLineOutputClass);
				Helper.Display.RetrieveUserOutput();
				var userInput = Helper.GetFormattedInput(Console.ReadLine());
				Helper.Display.ClearUserOutput();
				var playerClassInput = textInfo.ToTitleCase(userInput[0].ToString());
				if (playerClassInput != "Mage" && playerClassInput != "Warrior" && playerClassInput != "Archer") {
					Helper.Display.StoreUserOutput(
						Helper.FormatAnnounceText(), 
						Helper.FormatDefaultBackground(), 
						"Invalid selection. Please enter Mage, Warrior, or Archer for your class.");
					Helper.Display.RetrieveUserOutput();
					continue;
				}
				var playerClass = playerClassInput;
				var playerClassString = "Your player class is " + playerClass + ", is that correct? [Y] or [N].";
				Helper.Display.StoreUserOutput(
					Helper.FormatAnnounceText(), Helper.FormatDefaultBackground(), playerClassString);
				Helper.RequestCommand();
				Helper.Display.RetrieveUserOutput();
				var input = Helper.GetFormattedInput(Console.ReadLine());
				if (input[0] == "y") {
					Helper.Display.ClearUserOutput();
					switch(playerClass) {
						case "Archer":
							var playerArcher = new Player(playerName, Player.PlayerClassType.Archer);
							var archerString = "You have selected Archer. You can 'use' an ability, for example " +
							                  "'use gut', if you have an ability named gut shot in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability distance'. " +
							                  "To use a bow, you must have a quiver equipped, and it must not be empty. To reload " +
							                  "your quiver, you can 'reload'.";
							for (var i = 0; i < archerString.Length; i += Helper.GetGameWidth()) {
								if (archerString.Length - i < Helper.GetGameWidth()) {
									Helper.Display.StoreUserOutput(
										Helper.FormatAnnounceText(), 
										Helper.FormatDefaultBackground(), 
										archerString.Substring(i, archerString.Length - i));
									continue;
								}
								Helper.Display.StoreUserOutput(
									Helper.FormatAnnounceText(), 
									Helper.FormatDefaultBackground(), 
									archerString.Substring(i, Helper.GetGameWidth()));
							}
							return playerArcher;
						case "Mage":
							var playerMage = new Player(playerName, Player.PlayerClassType.Mage);
							var mageString = "You have selected Mage. You can 'cast' a spell, for example " +
							                  "'cast fireball', if you have a spell named fireball in your spellbook. To see " +
							                  "the list of spells in your spellbook, you can 'list spells'. To view info " +
							                  "about a spell, you can 'spell' the spell name. For example, 'spell fireball'.";
							for (var i = 0; i < mageString.Length; i += Helper.GetGameWidth()) {
								if (mageString.Length - i < Helper.GetGameWidth()) {
									Helper.Display.StoreUserOutput(
										Helper.FormatAnnounceText(), 
										Helper.FormatDefaultBackground(), 
										mageString.Substring(i, mageString.Length - i));
									continue;
								}
								Helper.Display.StoreUserOutput(
									Helper.FormatAnnounceText(), 
									Helper.FormatDefaultBackground(), 
									mageString.Substring(i, Helper.GetGameWidth()));
							}
							return playerMage;
							case "Warrior":
							var playerWarrior = new Player(playerName, Player.PlayerClassType.Warrior);
							var warriorString = "You have selected Warrior. You can 'use' an ability, for example " +
							                  "'use charge', if you have an ability named charge in your abilities. To see " +
							                  "the list of abilities you have available, you can 'list abilities'. To view info " +
							                  "about an ability, you can 'ability' the ability name. For example, 'ability charge'.";
							for (var i = 0; i < warriorString.Length; i += Helper.GetGameWidth()) {
								if (warriorString.Length - i < Helper.GetGameWidth()) {
									Helper.Display.StoreUserOutput(
										Helper.FormatAnnounceText(), 
										Helper.FormatDefaultBackground(), 
										warriorString.Substring(i, warriorString.Length - i));
									continue;
								}
								Helper.Display.StoreUserOutput(
									Helper.FormatAnnounceText(), 
									Helper.FormatDefaultBackground(), 
									warriorString.Substring(i, Helper.GetGameWidth()));
							}
							return playerWarrior;
					}
				}
			}
		}
	}
}