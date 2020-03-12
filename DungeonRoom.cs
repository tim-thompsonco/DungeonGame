using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame {
	public class DungeonRoom : IRoom {
		public enum RoomType {
			Corridor,
			Openspace,
			Corner,
			Intersection,
			Stairs
		}
		public RoomType RoomCategory { get; set; }
		public bool IsDiscovered { get; set; }
		public bool GoNorth { get; set; }
		public bool GoSouth { get; set; }
		public bool GoEast { get; set; }
		public bool GoWest { get; set; }
		public bool GoNorthWest { get; set; }
		public bool GoSouthWest { get; set; }
		public bool GoNorthEast { get; set; }
		public bool GoSouthEast { get; set; }
		public bool GoUp { get; set; }
		public bool GoDown { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public List<string> Commands { get; set; }
		public List<string> CombatCommands { get; set; }
		public List<IRoomInteraction> RoomObjects { get; set; }
		public Monster Monster { get; set; }

		/* Default constructor for JSON deserialization to work since there isn't 1 main constructor that should be
		 used for JSON deserialization */
		public DungeonRoom() {}
		public DungeonRoom(int x, int y, int z, bool goNorth, bool goSouth, bool goEast, bool goWest, bool goNorthWest,
			bool goSouthWest, bool goNorthEast, bool goSouthEast, bool goUp, bool goDown, int levelRangeLow,
			int levelRangeHigh) {
			this.RoomObjects = new List<IRoomInteraction>();
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.GoNorth = goNorth;
			this.GoSouth = goSouth;
			this.GoEast = goEast;
			this.GoWest = goWest;
			this.GoNorthWest = goNorthWest;
			this.GoSouthWest = goSouthWest;
			this.GoNorthEast = goNorthEast;
			this.GoSouthEast = goSouthEast;
			this.GoUp = goUp;
			this.GoDown = goDown;
			this.Commands = new List<string> {"[I]nventory", "Save", "[Q]uit"};
			this.CombatCommands = new List<string> {"[F]ight", "[I]nventory", "Flee"};
			var randomNum = GameHandler.GetRandomNumber(1, 100);
			var randomNumLevel = GameHandler.GetRandomNumber(levelRangeLow, levelRangeHigh);
			// Reserving numbers 80-100 for chance of room not having a monster
			if (randomNum <= 16) {
				// 20% chance of spawning based on cumulative 0.2 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Zombie);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 32) {
				// 20% chance of spawning based on cumulative 0.4 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Skeleton);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 40) {
				// 10% chance of spawning based on cumulative 0.5 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Elemental);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 48) {
				// 10% chance of spawning based on cumulative 0.6 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Vampire);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 60) {
				// 15% chance of spawning based on cumulative 0.75 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Troll);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 68) {
				// 10% chance of spawning based on cumulative 0.85 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Demon);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 76) {
				// 10% chance of spawning based on cumulative 0.95 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Spider);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum <= 80) {
				// 5% chance of spawning based on cumulative 1 * 80
				this.Monster = new Monster(randomNumLevel, Monster.MonsterType.Dragon);
				MonsterBuilder.BuildMonster(this.Monster);
				this.RoomObjects.Add(this.Monster);
			}
		}
		
		public void AttackOpponent(Player player, string[] input, Timer globalTimer) {
			globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
			try {
				var inputString = new StringBuilder();
				for (var i = 1; i < input.Length; i++) {
					inputString.Append(input[i]);
					inputString.Append(' ');
				}
				var inputName = inputString.ToString().Trim();
				var monsterName = this.Monster.Name.Split(' ');
				if (monsterName.Last() == inputName || this.Monster.Name == inputName || 
				    this.Monster.Name.Contains(input.Last())) {
					if (this.Monster.HitPoints > 0) {
						var fightEvent = new CombatHandler(this.Monster, player);
						fightEvent.StartCombat();
						if (player.HitPoints <= 0) {
							Messages.PlayerDeath();
						}
					}
					else {
						var monsterDeadString = "The " + this.Monster.Name + " is already dead."; 
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							monsterDeadString);
					}
				}
				else {
					var noMonsterString = "There is no " + inputName + " to attack.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noMonsterString);
				}
			}
			catch (IndexOutOfRangeException) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack that.");
			}
			globalTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}
		public void ShowCommands() {
			var sameLineOutput = new List<string> {
			Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available Commands: "};
			if (this.Monster != null && this.Monster.InCombat && this.CombatCommands != null) {
				var objCombatCount = this.CombatCommands.Count;
				foreach (var command in this.CombatCommands) {
					var sb = new StringBuilder();
					sb.Append(command);
					if (this.CombatCommands[objCombatCount - 1] != command) {
						sb.Append(", ");
					}
					if (this.CombatCommands[objCombatCount - 1] == command) sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			else if (this.Commands != null) {
				var objCount = this.Commands.Count;
				foreach (var command in this.Commands) {
					var sb = new StringBuilder();
					sb.Append(command);
					if (this.Commands[objCount - 1] != command) {
						sb.Append(", ");
					}
					if (this.Commands[objCount - 1] == command) sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			OutputHandler.Display.StoreUserOutput(sameLineOutput);
		}
		public void ShowDirections() {
			const string directionList = "Available Directions: ";
			var sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), 
				Settings.FormatDefaultBackground(),
				directionList};
			var roomDirs = new StringBuilder();
			if (this.GoNorth) {
				roomDirs.Append("[N]orth ");
			}
			if (this.GoSouth) {
				roomDirs.Append("[S]outh ");
			}
			if (this.GoEast) {
				roomDirs.Append("[E]ast ");
			}
			if (this.GoWest) {
				roomDirs.Append("[W]est ");
			}
			if (this.GoNorthWest) {
				roomDirs.Append("[N]orth[W]est ");
			}
			if (this.GoSouthWest) {
				roomDirs.Append("[S]outh[W]est ");
			}
			if (this.GoNorthEast) {
				roomDirs.Append("[N]orth[E]ast ");
			}
			if (this.GoSouthEast) {
				roomDirs.Append("[S]outh[E]ast ");
			}
			if (this.GoUp) {
				roomDirs.Append("[U]p ");
			}
			if (this.GoDown) {
				roomDirs.Append("[D]own");
			}
			if (directionList.Length + roomDirs.ToString().Length > Settings.GetGameWidth()) {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString().Substring(
					0, Settings.GetGameWidth() - directionList.Length));
				OutputHandler.Display.StoreUserOutput(sameLineOutput);
			}
			else {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString());
				OutputHandler.Display.StoreUserOutput(sameLineOutput);
				return;
			}
			var remainingRoomDirs = roomDirs.ToString().Substring(Settings.GetGameWidth() - directionList.Length);
			for (var i = 0; i < remainingRoomDirs.Length; i += Settings.GetGameWidth()) {
				if (remainingRoomDirs.Length - i < Settings.GetGameWidth()) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(), 
						Settings.FormatDefaultBackground(), 
						remainingRoomDirs.Substring(i, remainingRoomDirs.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(), 
					Settings.FormatDefaultBackground(), 
					remainingRoomDirs.Substring(i, Settings.GetGameWidth()));
			}
		}
		public void LookRoom() {
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				Settings.FormatTextBorder());
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatRoomOutputText(), 
				Settings.FormatDefaultBackground(), 
				this.Name);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				Settings.FormatTextBorder());
			for (var i = 0; i < this.Desc.Length; i += Settings.GetGameWidth()) {
				if (this.Desc.Length - i < Settings.GetGameWidth()) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(), 
						Settings.FormatDefaultBackground(), 
						this.Desc.Substring(i, this.Desc.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(), 
					Settings.FormatDefaultBackground(), 
					this.Desc.Substring(i, Settings.GetGameWidth()));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(), 
				Settings.FormatTextBorder());
			var sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "};
			if (this.RoomObjects.Count > 0 && this.RoomObjects[0] != null) {
				var objCount = this.RoomObjects.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var item in this.RoomObjects) {
					var sb = new StringBuilder();
					var itemTitle = item.Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (this.RoomObjects[objCount - 1] != item) {
						sb.Append(", ");
					}
					sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			else {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add("There is nothing in the room.");
			}
			OutputHandler.Display.StoreUserOutput(sameLineOutput);
			this.ShowDirections();
		}
		public void LootCorpse(Player player, string[] input) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this.Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || this.Monster.Name.Contains(inputName)) {
				if (this.Monster.HitPoints <= 0 && this.Monster.WasLooted == false) {
					var goldLooted = this.Monster.Gold;
					player.Gold += this.Monster.Gold;
					try {
						this.Monster.Gold = 0;
						var lootGoldString = "You looted " + goldLooted + " gold coins from the " + this.Monster.Name + "!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							lootGoldString);
						for (var i = 0; i < this.Monster.MonsterItems.Count; i++) {
							var itemType = this.Monster.MonsterItems[i].GetType().FullName;
							var playerWeight = PlayerHandler.GetInventoryWeight(player);
							var itemWeight = this.Monster.MonsterItems[i].Weight;
							if (playerWeight + itemWeight > player.MaxCarryWeight) {
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't carry that much!");
								return;
							}
							if (itemType == "DungeonGame.Consumable") {
								player.Consumables.Add((Consumable)this.Monster.MonsterItems[i]);
							}
							else {
								player.Inventory.Add(this.Monster.MonsterItems[i]);
							}
							var lootItemString = "You looted " + this.Monster.MonsterItems[i].Name + " from the " +
							                     this.Monster.Name + "!";
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatSuccessOutputText(),
								Settings.FormatDefaultBackground(),
								lootItemString);
							this.Monster.MonsterItems.RemoveAt(i);
						}
						this.Monster.MonsterItems.Clear();
						this.Monster.WasLooted = true;
						var monsterIndex = this.RoomObjects.FindIndex(
							f => f.Name == this.Monster.Name);
						if (monsterIndex != -1) this.RoomObjects.RemoveAt(monsterIndex);
					}
					catch (InvalidOperationException) {
					}
				}
				else if (this.Monster.WasLooted) {
					var alreadyLootString = "You already looted " + this.Monster.Name + "!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						alreadyLootString);
				}
				else {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							"You cannot loot something that isn't dead!");
				}
			}
			else {
				var noLootString = "There is no " + inputName + " in the room!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noLootString);
			}
		}
		private string CalculateNpcLevelDiff(Player player) {
			if (this.Monster == null) return null;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var monsterName = textInfo.ToTitleCase(this.Monster.Name);
			var levelDiff = player.Level - this.Monster.Level;
			var difficultyStringBuilder = new StringBuilder();
			difficultyStringBuilder.Append("Difficulty: ");
			if (levelDiff >= 3) {
				difficultyStringBuilder.Append("You will crush " + monsterName + " like a bug.");
			}
			else switch (levelDiff) {
				case 2:
					difficultyStringBuilder.Append("You have an edge over " + monsterName + ".");
					break;
				case 1:
					difficultyStringBuilder.Append("You have a slight edge over " + monsterName + ".");
					break;
				case 0:
					difficultyStringBuilder.Append("You're about even with " + monsterName + ".");
					break;
				default: {
					switch (levelDiff) {
						case -1:
							difficultyStringBuilder.Append(monsterName + " has a slight edge over you.");
							break;
						case -2:
							difficultyStringBuilder.Append(monsterName + " has an edge over you.");
							break;
						default: {
							if (levelDiff <= -3) {
								difficultyStringBuilder.Append(monsterName + " will crush you like a bug.");
							}
							break;
						}
					}
					break;
				}
			}
			return difficultyStringBuilder.ToString();
		}
		public void LookNpc(string[] input, Player player) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this.Monster.Name.Split(' ');
			if (monsterName.Last() == inputName || this.Monster.Name.Contains(inputName)) {
				for (var i = 0; i < this.Monster.Desc.Length; i += Settings.GetGameWidth()) {
					if (this.Monster.Desc.Length - i < Settings.GetGameWidth()) {
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(), 
							Settings.FormatDefaultBackground(), 
							this.Monster.Desc.Substring(i, this.Monster.Desc.Length - i));
						continue;
					}
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(), 
						Settings.FormatDefaultBackground(), 
						this.Monster.Desc.Substring(i, Settings.GetGameWidth()));
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(), 
					Settings.FormatDefaultBackground(), 
					this.CalculateNpcLevelDiff(player));
				var sameLineOutput = new List<string> {
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					"It is carrying: "};
				OutputHandler.Display.StoreUserOutput(sameLineOutput);
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				if (this.Monster.MonsterItems == null) return;
				foreach (var item in this.Monster.MonsterItems) {
					var sameLineOutputItem = new List<string>();
					var sb = new StringBuilder();
					var itemTitle = item.Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sameLineOutputItem.Add(Settings.FormatRoomOutputText());
					sameLineOutputItem.Add(Settings.FormatDefaultBackground());
					sameLineOutputItem.Add(sb.ToString());
					OutputHandler.Display.StoreUserOutput(sameLineOutputItem);
				}
			}
			else {
				var noNpcString = "There is no " + inputName + " in the room!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noNpcString);
			}
		}
	}
}