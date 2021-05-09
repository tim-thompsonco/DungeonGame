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
		public enum RoomType {
			Corridor,
			Openspace,
			Corner,
			Intersection,
			Stairs
		}
		public RoomType RoomCategory { get; set; }
		public bool _IsDiscovered { get; set; }
		public IRoom _North { get; set; }
		public IRoom _South { get; set; }
		public IRoom _East { get; set; }
		public IRoom _West { get; set; }
		public IRoom _NorthWest { get; set; }
		public IRoom _SouthWest { get; set; }
		public IRoom _NorthEast { get; set; }
		public IRoom _SouthEast { get; set; }
		public IRoom _Up { get; set; }
		public IRoom _Down { get; set; }
		public string Name { get; set; }
		public string _Desc { get; set; }
		public int DungeonLevel { get; set; }
		public List<string> Commands { get; set; }
		public List<string> CombatCommands { get; set; }
		public List<IName> _RoomObjects { get; set; }
		public Monster _Monster { get; set; }

		// Default constructor for JSON deserialization
		public DungeonRoom() { }
		public DungeonRoom(int levelRangeLow, int levelRangeHigh) {
			_RoomObjects = new List<IName>();
			Commands = new List<string> { "[I]nventory", "Save", "[Q]uit" };
			CombatCommands = new List<string> { "[F]ight", "[I]nventory", "Flee" };
			DungeonLevel = GameHelper.GetRandomNumber(levelRangeLow, levelRangeHigh);
			int randomNum = GameHelper.GetRandomNumber(1, 100);
			// Reserving numbers 80-100 for chance of room not having a monster
			if (randomNum <= 16) {
				// 20% chance of spawning based on cumulative 0.2 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Zombie);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 32) {
				// 20% chance of spawning based on cumulative 0.4 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Skeleton);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 40) {
				// 10% chance of spawning based on cumulative 0.5 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Elemental);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 48) {
				// 10% chance of spawning based on cumulative 0.6 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Vampire);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 60) {
				// 15% chance of spawning based on cumulative 0.75 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Troll);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 68) {
				// 10% chance of spawning based on cumulative 0.85 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Demon);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 76) {
				// 10% chance of spawning based on cumulative 0.95 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Spider);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
			} else if (randomNum <= 80) {
				// 5% chance of spawning based on cumulative 1 * 80
				_Monster = new Monster(DungeonLevel, Monster.MonsterType.Dragon);
				MonsterBuilder.BuildMonster(_Monster);
				_RoomObjects.Add(_Monster);
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
				string[] monsterName = _Monster.Name.Split(' ');
				if (monsterName.Last() == inputName || _Monster.Name == inputName ||
					_Monster.Name.Contains(input.Last()) || _Monster != null) {
					if (_Monster.HitPoints > 0) {
						player.InCombat = true;
						_Monster.InCombat = true;
						CombatHelper.StartCombat(player, _Monster);
						if (player.HitPoints <= 0) {
							Messages.PlayerDeath();
						}
					} else {
						string monsterDeadString = $"The {_Monster.Name} is already dead.";
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
			if (_Monster != null && _Monster.InCombat && CombatCommands != null) {
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
			if (_North != null) {
				roomDirs.Append("[N]orth ");
			}
			if (_South != null) {
				roomDirs.Append("[S]outh ");
			}
			if (_East != null) {
				roomDirs.Append("[E]ast ");
			}
			if (_West != null) {
				roomDirs.Append("[W]est ");
			}
			if (_NorthWest != null) {
				roomDirs.Append("[N]orth[W]est ");
			}
			if (_SouthWest != null) {
				roomDirs.Append("[S]outh[W]est ");
			}
			if (_NorthEast != null) {
				roomDirs.Append("[N]orth[E]ast ");
			}
			if (_SouthEast != null) {
				roomDirs.Append("[S]outh[E]ast ");
			}
			if (_Up != null) {
				roomDirs.Append("[U]p ");
			}
			if (_Down != null) {
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
			for (int i = 0; i < _Desc.Length; i += Settings.GetGameWidth()) {
				if (_Desc.Length - i < Settings.GetGameWidth()) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						_Desc.Substring(i, _Desc.Length - i));
					continue;
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					_Desc.Substring(i, Settings.GetGameWidth()));
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			List<string> sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "};
			if (_RoomObjects.Count > 0 && _RoomObjects[0] != null) {
				int objCount = _RoomObjects.Count;
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (IName item in _RoomObjects) {
					StringBuilder sb = new StringBuilder();
					string itemTitle = item.Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (_RoomObjects[objCount - 1] != item) {
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
			string[] monsterName = _Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || _Monster.Name.Contains(inputName)) {
				if (_Monster.HitPoints <= 0 && _Monster.WasLooted == false) {
					int goldLooted = _Monster.Gold;
					player.Gold += _Monster.Gold;
					try {
						_Monster.Gold = 0;
						string lootGoldString = $"You looted {goldLooted} gold coins from the {_Monster.Name}!";
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							lootGoldString);
						while (_Monster.MonsterItems.Count > 0) {
							int playerWeight = PlayerHelper.GetInventoryWeight(player);
							int itemWeight = _Monster.MonsterItems[0].Weight;
							if (playerWeight + itemWeight > player.MaxCarryWeight) {
								OutputHelper.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't carry that much!");
								return;
							} else {
								player.Inventory.Add(_Monster.MonsterItems[0]);
							}
							string lootItemString = $"You looted {_Monster.MonsterItems[0].Name} from the {_Monster.Name}!";
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatSuccessOutputText(),
								Settings.FormatDefaultBackground(),
								lootItemString);
							_Monster.MonsterItems.RemoveAt(0);
						}
						_Monster.WasLooted = true;
						int monsterIndex = _RoomObjects.FindIndex(
							f => f.Name == _Monster.Name);
						if (monsterIndex != -1) {
							_RoomObjects.RemoveAt(monsterIndex);
						}
					} catch (InvalidOperationException) {
					}
				} else if (_Monster.WasLooted) {
					string alreadyLootString = $"You already looted {_Monster.Name}!";
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
			if (_Monster == null) {
				return null;
			}

			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string monsterName = textInfo.ToTitleCase(_Monster.Name);
			int levelDiff = player.Level - _Monster.Level;
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
			string[] monsterName = _Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || _Monster.Name.Contains(inputName)) {
				for (int i = 0; i < _Monster.Desc.Length; i += Settings.GetGameWidth()) {
					if (_Monster.Desc.Length - i < Settings.GetGameWidth()) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							_Monster.Desc.Substring(i, _Monster.Desc.Length - i));
						continue;
					}
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						_Monster.Desc.Substring(i, Settings.GetGameWidth()));
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
				if (_Monster.MonsterItems == null) {
					return;
				}

				foreach (IItem item in _Monster.MonsterItems) {
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