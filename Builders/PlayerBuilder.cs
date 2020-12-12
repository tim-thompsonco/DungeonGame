using DungeonGame.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DungeonGame
{
	public class PlayerBuilder
	{
		public Player BuildNewPlayer()
		{
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), "Please enter a player name.");
			string playerName;
			while (true)
			{
				var sameLineOutput = new List<string> {
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), "Player name: "
				};
				OutputController.Display.StoreUserOutput(sameLineOutput);
				OutputController.Display.RetrieveUserOutput();
				playerName = textInfo.ToTitleCase(Console.ReadLine());
				OutputController.Display.ClearUserOutput();
				var playerNameString = "Your player name is " + playerName + ", is that correct? [Y] or [N].";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), playerNameString);
				OutputController.Display.RetrieveUserOutput();
				Messages.RequestCommand();
				var input = InputController.GetFormattedInput(Console.ReadLine());
				OutputController.Display.ClearUserOutput();
				if (input[0] == "y")
				{
					break;
				}
			}
			while (true)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					"Please enter your class. You can select Mage, Warrior, or Archer.");
				var sameLineOutputClass = new List<string> {
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), "Player class: "
				};
				OutputController.Display.StoreUserOutput(sameLineOutputClass);
				OutputController.Display.RetrieveUserOutput();
				var userInput = InputController.GetFormattedInput(Console.ReadLine());
				OutputController.Display.ClearUserOutput();
				var playerClassInput = textInfo.ToTitleCase(userInput[0]);
				if (playerClassInput != "Mage" && playerClassInput != "Warrior" && playerClassInput != "Archer")
				{
					OutputController.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),
						Settings.FormatDefaultBackground(),
						"Invalid selection. Please enter Mage, Warrior, or Archer for your class.");
					OutputController.Display.RetrieveUserOutput();
					continue;
				}
				var playerClass = playerClassInput;
				var playerClassString = "Your player class is " + playerClass + ", is that correct? [Y] or [N].";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(), Settings.FormatDefaultBackground(), playerClassString);
				Messages.RequestCommand();
				OutputController.Display.RetrieveUserOutput();
				var input = InputController.GetFormattedInput(Console.ReadLine());
				if (input[0] == "y")
				{
					OutputController.Display.ClearUserOutput();
					switch (playerClass)
					{
						case "Archer":
							var playerArcher = new Player(playerName, Player.PlayerClassType.Archer);
							const string archerString =
								"You have selected Archer. You can 'use' an ability, for example " +
								"'use gut', if you have an ability named gut shot in your abilities. To see " +
								"the list of abilities you have available, you can 'list abilities'. To view info " +
								"about an ability, you can 'ability' the ability name. For example, 'ability distance'. " +
								"To use a bow, you must have a quiver equipped, and it must not be empty. To reload " +
								"your quiver, you can 'reload'.";
							for (var i = 0; i < archerString.Length; i += Settings.GetGameWidth())
							{
								if (archerString.Length - i < Settings.GetGameWidth())
								{
									OutputController.Display.StoreUserOutput(
										Settings.FormatAnnounceText(),
										Settings.FormatDefaultBackground(),
										archerString.Substring(i, archerString.Length - i));
									continue;
								}
								OutputController.Display.StoreUserOutput(
									Settings.FormatAnnounceText(),
									Settings.FormatDefaultBackground(),
									archerString.Substring(i, Settings.GetGameWidth()));
							}
							return playerArcher;
						case "Mage":
							var playerMage = new Player(playerName, Player.PlayerClassType.Mage);
							const string mageString =
								"You have selected Mage. You can 'cast' a spell, for example " +
								"'cast fireball', if you have a spell named fireball in your spellbook. To see " +
								"the list of spells in your spellbook, you can 'list spells'. To view info " +
								"about a spell, you can 'spell' the spell name. For example, 'spell fireball'.";
							for (var i = 0; i < mageString.Length; i += Settings.GetGameWidth())
							{
								if (mageString.Length - i < Settings.GetGameWidth())
								{
									OutputController.Display.StoreUserOutput(
										Settings.FormatAnnounceText(),
										Settings.FormatDefaultBackground(),
										mageString.Substring(i, mageString.Length - i));
									continue;
								}
								OutputController.Display.StoreUserOutput(
									Settings.FormatAnnounceText(),
									Settings.FormatDefaultBackground(),
									mageString.Substring(i, Settings.GetGameWidth()));
							}
							return playerMage;
						case "Warrior":
							var playerWarrior = new Player(playerName, Player.PlayerClassType.Warrior);
							const string warriorString =
								"You have selected Warrior. You can 'use' an ability, for example " +
								"'use charge', if you have an ability named charge in your abilities. To see " +
								"the list of abilities you have available, you can 'list abilities'. To view info " +
								"about an ability, you can 'ability' the ability name. For example, 'ability charge'.";
							for (var i = 0; i < warriorString.Length; i += Settings.GetGameWidth())
							{
								if (warriorString.Length - i < Settings.GetGameWidth())
								{
									OutputController.Display.StoreUserOutput(
										Settings.FormatAnnounceText(),
										Settings.FormatDefaultBackground(),
										warriorString.Substring(i, warriorString.Length - i));
									continue;
								}
								OutputController.Display.StoreUserOutput(
									Settings.FormatAnnounceText(),
									Settings.FormatDefaultBackground(),
									warriorString.Substring(i, Settings.GetGameWidth()));
							}
							return playerWarrior;
					}
				}
			}
		}
	}
}