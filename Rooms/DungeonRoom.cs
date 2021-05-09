using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame.Rooms {
	public class DungeonRoom : IRoom {
		public RoomType RoomCategory { get; set; }
		public bool IsDiscovered { get; set; }
		public IRoom North { get; set; }
		public IRoom South { get; set; }
		public IRoom East { get; set; }
		public IRoom West { get; set; }
		public IRoom NorthWest { get; set; }
		public IRoom SouthWest { get; set; }
		public IRoom NorthEast { get; set; }
		public IRoom SouthEast { get; set; }
		public IRoom Up { get; set; }
		public IRoom Down { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int DungeonLevel { get; set; }
		public List<string> Commands { get; set; }
		public List<string> CombatCommands { get; set; }
		public List<IName> RoomObjects { get; set; }
		public Monster Monster { get; set; }

		// Default constructor for JSON deserialization
		public DungeonRoom() { }
		public DungeonRoom(int levelRangeLow, int levelRangeHigh) {
			RoomObjects = new List<IName>();
			Commands = new List<string> { "[I]nventory", "Save", "[Q]uit" };
			CombatCommands = new List<string> { "[F]ight", "[I]nventory", "Flee" };
			DungeonLevel = GameHelper.GetRandomNumber(levelRangeLow, levelRangeHigh);
			int randomNum = GameHelper.GetRandomNumber(1, 100);
			// Reserving numbers 80-100 for chance of room not having a monster
			if (randomNum <= 16) {
				// 20% chance of spawning based on cumulative 0.2 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Zombie);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 32) {
				// 20% chance of spawning based on cumulative 0.4 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Skeleton);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 40) {
				// 10% chance of spawning based on cumulative 0.5 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Elemental);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 48) {
				// 10% chance of spawning based on cumulative 0.6 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Vampire);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 60) {
				// 15% chance of spawning based on cumulative 0.75 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Troll);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 68) {
				// 10% chance of spawning based on cumulative 0.85 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Demon);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 76) {
				// 10% chance of spawning based on cumulative 0.95 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Spider);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			} else if (randomNum <= 80) {
				// 5% chance of spawning based on cumulative 1 * 80
				Monster = new Monster(DungeonLevel, Monsters.MonsterType.Dragon);
				MonsterBuilder.BuildMonster(Monster);
				RoomObjects.Add(Monster);
			}
		}

		public void AttackOpponent(Player player, string[] input, Timer globalTimer) {
			globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
			try {
				StringBuilder inputString = new StringBuilder();
				for (int i = 1; i < input.Length; i++) {
					inputString.Append(input[i]);
					inputString.Append(' ');
				}
				string inputName = inputString.ToString().Trim();
				string[] monsterName = Monster.Name.Split(' ');
				if (monsterName.Last() == inputName || Monster.Name == inputName ||
					Monster.Name.Contains(input.Last()) || Monster != null) {
					if (Monster.HitPoints > 0) {
						player.InCombat = true;
						Monster.InCombat = true;
						CombatHelper.StartCombat(player, Monster);
						if (player.HitPoints <= 0) {
							Messages.PlayerDeath();
						}
					} else {
						string monsterDeadString = $"The {Monster.Name} is already dead.";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							monsterDeadString);
					}
				} else {
					string noMonsterString = $"There is no {inputName} to attack.";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noMonsterString);
				}
			} catch (IndexOutOfRangeException) {
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack that.");
			}
			globalTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}
		public void ShowCommands() {
			List<string> sameLineOutput = new List<string> {
			Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available _Commands: "};
			if (Monster != null && Monster.InCombat && CombatCommands != null) {
				int objCombatCount = CombatCommands.Count;
				foreach (string command in CombatCommands) {
					StringBuilder sb = new StringBuilder();
					sb.Append(command);
					if (CombatCommands[objCombatCount - 1] != command) {
						sb.Append(", ");
					}
					if (CombatCommands[objCombatCount - 1] == command) {
						sb.Append(".");
					}

					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			} else if (Commands != null) {
				int objCount = Commands.Count;
				foreach (string command in Commands) {
					StringBuilder sb = new StringBuilder();
					sb.Append(command);
					if (Commands[objCount - 1] != command) {
						sb.Append(", ");
					}
					if (Commands[objCount - 1] == command) {
						sb.Append(".");
					}

					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			OutputHelper.Display.StoreUserOutput(sameLineOutput);
		}
		public void ShowDirections() {
			const string directionList = "Available Directions: ";
			List<string> sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(),
				Settings.FormatDefaultBackground(),
				directionList};
			StringBuilder roomDirs = new StringBuilder();
			if (North != null) {
				roomDirs.Append("[N]orth ");
			}
			if (South != null) {
				roomDirs.Append("[S]outh ");
			}
			if (East != null) {
				roomDirs.Append("[E]ast ");
			}
			if (West != null) {
				roomDirs.Append("[W]est ");
			}
			if (NorthWest != null) {
				roomDirs.Append("[N]orth[W]est ");
			}
			if (SouthWest != null) {
				roomDirs.Append("[S]outh[W]est ");
			}
			if (NorthEast != null) {
				roomDirs.Append("[N]orth[E]ast ");
			}
			if (SouthEast != null) {
				roomDirs.Append("[S]outh[E]ast ");
			}
			if (Up != null) {
				roomDirs.Append("[U]p ");
			}
			if (Down != null) {
				roomDirs.Append("[D]own");
			}
			if (directionList.Length + roomDirs.ToString().Length > Settings.GetGameWidth()) {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString().Substring(
					0, Settings.GetGameWidth() - directionList.Length));
				OutputHelper.Display.StoreUserOutput(sameLineOutput);
			} else {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString());
				OutputHelper.Display.StoreUserOutput(sameLineOutput);
				return;
			}
			string remainingRoomDirs = roomDirs.ToString().Substring(Settings.GetGameWidth() - directionList.Length);
			for (int i = 0; i < remainingRoomDirs.Length; i += Settings.GetGameWidth()) {
				if (remainingRoomDirs.Length - i < Settings.GetGameWidth()) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						remainingRoomDirs.Substring(i, remainingRoomDirs.Length - i));
					continue;
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					remainingRoomDirs.Substring(i, Settings.GetGameWidth()));
			}
		}
		public void LookRoom() {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatRoomOutputText(),
				Settings.FormatDefaultBackground(),
				Name);
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			for (int i = 0; i < Desc.Length; i += Settings.GetGameWidth()) {
				if (Desc.Length - i < Settings.GetGameWidth()) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						Desc.Substring(i, Desc.Length - i));
					continue;
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					Desc.Substring(i, Settings.GetGameWidth()));
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			List<string> sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "};
			if (RoomObjects.Count > 0 && RoomObjects[0] != null) {
				int objCount = RoomObjects.Count;
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (IName item in RoomObjects) {
					StringBuilder sb = new StringBuilder();
					string itemTitle = item.Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (RoomObjects[objCount - 1] != item) {
						sb.Append(", ");
					}
					sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			} else {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add("There is nothing in the room.");
			}
			OutputHelper.Display.StoreUserOutput(sameLineOutput);
			ShowDirections();
		}
		public void LootCorpse(Player player, string[] input) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			string inputName = inputString.ToString().Trim();
			string[] monsterName = Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || Monster.Name.Contains(inputName)) {
				if (Monster.HitPoints <= 0 && Monster.WasLooted == false) {
					int goldLooted = Monster.Gold;
					player.Gold += Monster.Gold;
					try {
						Monster.Gold = 0;
						string lootGoldString = $"You looted {goldLooted} gold coins from the {Monster.Name}!";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							lootGoldString);
						while (Monster.MonsterItems.Count > 0) {
							int playerWeight = PlayerHelper.GetInventoryWeight(player);
							int itemWeight = Monster.MonsterItems[0].Weight;
							if (playerWeight + itemWeight > player.MaxCarryWeight) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't carry that much!");
								return;
							} else {
								player.Inventory.Add(Monster.MonsterItems[0]);
							}
							string lootItemString = $"You looted {Monster.MonsterItems[0].Name} from the {Monster.Name}!";
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatSuccessOutputText(),
								Settings.FormatDefaultBackground(),
								lootItemString);
							Monster.MonsterItems.RemoveAt(0);
						}
						Monster.WasLooted = true;
						int monsterIndex = RoomObjects.FindIndex(
							f => f.Name == Monster.Name);
						if (monsterIndex != -1) {
							RoomObjects.RemoveAt(monsterIndex);
						}
					} catch (InvalidOperationException) {
					}
				} else if (Monster.WasLooted) {
					string alreadyLootString = $"You already looted {Monster.Name}!";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						alreadyLootString);
				} else {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot loot something that isn't dead!");
				}
			} else {
				string noLootString = $"There is no {inputName} in the room!";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noLootString);
			}
		}
		private string CalculateNpcLevelDiff(Player player) {
			if (Monster == null) {
				return null;
			}

			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string monsterName = textInfo.ToTitleCase(Monster.Name);
			int levelDiff = player.Level - Monster.Level;
			StringBuilder difficultyStringBuilder = new StringBuilder();
			difficultyStringBuilder.Append("Difficulty: ");
			if (levelDiff >= 3) {
				difficultyStringBuilder.Append($"You will crush {monsterName} like a bug.");
			} else {
				switch (levelDiff) {
					case 2:
						difficultyStringBuilder.Append($"You have an edge over {monsterName}.");
						break;
					case 1:
						difficultyStringBuilder.Append($"You have a slight edge over {monsterName}.");
						break;
					case 0:
						difficultyStringBuilder.Append($"You're about even with {monsterName}.");
						break;
					default: {
							switch (levelDiff) {
								case -1:
									difficultyStringBuilder.Append($"{monsterName} has a slight edge over you.");
									break;
								case -2:
									difficultyStringBuilder.Append($"{monsterName} has an edge over you.");
									break;
								default: {
										if (levelDiff <= -3) {
											difficultyStringBuilder.Append($"{monsterName} will crush you like a bug.");
										}
										break;
									}
							}
							break;
						}
				}
			}

			return difficultyStringBuilder.ToString();
		}
		public void LookNpc(string[] input, Player player) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			string inputName = inputString.ToString().Trim();
			string[] monsterName = Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || Monster.Name.Contains(inputName)) {
				for (int i = 0; i < Monster.Desc.Length; i += Settings.GetGameWidth()) {
					if (Monster.Desc.Length - i < Settings.GetGameWidth()) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							Monster.Desc.Substring(i, Monster.Desc.Length - i));
						continue;
					}
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						Monster.Desc.Substring(i, Settings.GetGameWidth()));
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					CalculateNpcLevelDiff(player));
				List<string> sameLineOutput = new List<string> {
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					"It is carrying: "};
				OutputHelper.Display.StoreUserOutput(sameLineOutput);
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				if (Monster.MonsterItems == null) {
					return;
				}

				foreach (IItem item in Monster.MonsterItems) {
					List<string> sameLineOutputItem = new List<string>();
					StringBuilder sb = new StringBuilder();
					string itemTitle = item.Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sameLineOutputItem.Add(Settings.FormatRoomOutputText());
					sameLineOutputItem.Add(Settings.FormatDefaultBackground());
					sameLineOutputItem.Add(sb.ToString());
					OutputHelper.Display.StoreUserOutput(sameLineOutputItem);
				}
			} else {
				string noNpcString = $"There is no {inputName} in the room!";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noNpcString);
			}
		}
	}
}