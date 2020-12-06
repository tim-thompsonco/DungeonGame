using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame
{
	public class DungeonRoom : IRoom
	{
		public enum RoomType
		{
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
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int DungeonLevel { get; set; }
		public List<string> Commands { get; set; }
		public List<string> CombatCommands { get; set; }
		public List<IRoomInteraction> _RoomObjects { get; set; }
		public Monster _Monster { get; set; }

		// Default constructor for JSON deserialization
		public DungeonRoom() { }
		public DungeonRoom(int levelRangeLow, int levelRangeHigh)
		{
			this._RoomObjects = new List<IRoomInteraction>();
			this.Commands = new List<string> { "[I]nventory", "Save", "[Q]uit" };
			this.CombatCommands = new List<string> { "[F]ight", "[I]nventory", "Flee" };
			this.DungeonLevel = GameHandler.GetRandomNumber(levelRangeLow, levelRangeHigh);
			var randomNum = GameHandler.GetRandomNumber(1, 100);
			// Reserving numbers 80-100 for chance of room not having a monster
			if (randomNum <= 16)
			{
				// 20% chance of spawning based on cumulative 0.2 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Zombie);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 32)
			{
				// 20% chance of spawning based on cumulative 0.4 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Skeleton);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 40)
			{
				// 10% chance of spawning based on cumulative 0.5 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Elemental);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 48)
			{
				// 10% chance of spawning based on cumulative 0.6 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Vampire);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 60)
			{
				// 15% chance of spawning based on cumulative 0.75 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Troll);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 68)
			{
				// 10% chance of spawning based on cumulative 0.85 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Demon);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 76)
			{
				// 10% chance of spawning based on cumulative 0.95 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Spider);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
			else if (randomNum <= 80)
			{
				// 5% chance of spawning based on cumulative 1 * 80
				this._Monster = new Monster(this.DungeonLevel, Monster.MonsterType.Dragon);
				MonsterBuilder.BuildMonster(this._Monster);
				this._RoomObjects.Add(this._Monster);
			}
		}

		public void AttackOpponent(Player player, string[] input, Timer globalTimer)
		{
			globalTimer.Change(Timeout.Infinite, Timeout.Infinite);
			try
			{
				var inputString = new StringBuilder();
				for (var i = 1; i < input.Length; i++)
				{
					inputString.Append(input[i]);
					inputString.Append(' ');
				}
				var inputName = inputString.ToString().Trim();
				var monsterName = this._Monster._Name.Split(' ');
				if (monsterName.Last() == inputName || this._Monster._Name == inputName ||
					this._Monster._Name.Contains(input.Last()) || this._Monster != null)
				{
					if (this._Monster._HitPoints > 0)
					{
						var fightEvent = new CombatHandler(this._Monster, player);
						fightEvent.StartCombat();
						if (player._HitPoints <= 0)
						{
							Messages.PlayerDeath();
						}
					}
					else
					{
						var monsterDeadString = "The " + this._Monster._Name + " is already dead.";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatFailureOutputText(),
							Settings.FormatDefaultBackground(),
							monsterDeadString);
					}
				}
				else
				{
					var noMonsterString = "There is no " + inputName + " to attack.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noMonsterString);
				}
			}
			catch (IndexOutOfRangeException)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack that.");
			}
			globalTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}
		public void ShowCommands()
		{
			var sameLineOutput = new List<string> {
			Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available _Commands: "};
			if (this._Monster != null && this._Monster._InCombat && this.CombatCommands != null)
			{
				var objCombatCount = this.CombatCommands.Count;
				foreach (var command in this.CombatCommands)
				{
					var sb = new StringBuilder();
					sb.Append(command);
					if (this.CombatCommands[objCombatCount - 1] != command)
					{
						sb.Append(", ");
					}
					if (this.CombatCommands[objCombatCount - 1] == command) sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			else if (this.Commands != null)
			{
				var objCount = this.Commands.Count;
				foreach (var command in this.Commands)
				{
					var sb = new StringBuilder();
					sb.Append(command);
					if (this.Commands[objCount - 1] != command)
					{
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
		public void ShowDirections()
		{
			const string directionList = "Available Directions: ";
			var sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(),
				Settings.FormatDefaultBackground(),
				directionList};
			var roomDirs = new StringBuilder();
			if (this._North != null)
			{
				roomDirs.Append("[N]orth ");
			}
			if (this._South != null)
			{
				roomDirs.Append("[S]outh ");
			}
			if (this._East != null)
			{
				roomDirs.Append("[E]ast ");
			}
			if (this._West != null)
			{
				roomDirs.Append("[W]est ");
			}
			if (this._NorthWest != null)
			{
				roomDirs.Append("[N]orth[W]est ");
			}
			if (this._SouthWest != null)
			{
				roomDirs.Append("[S]outh[W]est ");
			}
			if (this._NorthEast != null)
			{
				roomDirs.Append("[N]orth[E]ast ");
			}
			if (this._SouthEast != null)
			{
				roomDirs.Append("[S]outh[E]ast ");
			}
			if (this._Up != null)
			{
				roomDirs.Append("[U]p ");
			}
			if (this._Down != null)
			{
				roomDirs.Append("[D]own");
			}
			if (directionList.Length + roomDirs.ToString().Length > Settings.GetGameWidth())
			{
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString().Substring(
					0, Settings.GetGameWidth() - directionList.Length));
				OutputHandler.Display.StoreUserOutput(sameLineOutput);
			}
			else
			{
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString());
				OutputHandler.Display.StoreUserOutput(sameLineOutput);
				return;
			}
			var remainingRoomDirs = roomDirs.ToString().Substring(Settings.GetGameWidth() - directionList.Length);
			for (var i = 0; i < remainingRoomDirs.Length; i += Settings.GetGameWidth())
			{
				if (remainingRoomDirs.Length - i < Settings.GetGameWidth())
				{
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
		public void LookRoom()
		{
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatRoomOutputText(),
				Settings.FormatDefaultBackground(),
				this._Name);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			for (var i = 0; i < this._Desc.Length; i += Settings.GetGameWidth())
			{
				if (this._Desc.Length - i < Settings.GetGameWidth())
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						this._Desc.Substring(i, this._Desc.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					this._Desc.Substring(i, Settings.GetGameWidth()));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			var sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "};
			if (this._RoomObjects.Count > 0 && this._RoomObjects[0] != null)
			{
				var objCount = this._RoomObjects.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var item in this._RoomObjects)
				{
					var sb = new StringBuilder();
					var itemTitle = item._Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (this._RoomObjects[objCount - 1] != item)
					{
						sb.Append(", ");
					}
					sb.Append(".");
					sameLineOutput.Add(Settings.FormatInfoText());
					sameLineOutput.Add(Settings.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			else
			{
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add("There is nothing in the room.");
			}
			OutputHandler.Display.StoreUserOutput(sameLineOutput);
			this.ShowDirections();
		}
		public void LootCorpse(Player player, string[] input)
		{
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++)
			{
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this._Monster._Name.Split(' ');
			if (monsterName.Last() == inputName || this._Monster._Name.Contains(inputName))
			{
				if (this._Monster._HitPoints <= 0 && this._Monster._WasLooted == false)
				{
					var goldLooted = this._Monster._Gold;
					player._Gold += this._Monster._Gold;
					try
					{
						this._Monster._Gold = 0;
						var lootGoldString = "You looted " + goldLooted + " gold coins from the " + this._Monster._Name + "!";
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							lootGoldString);
						while (this._Monster._MonsterItems.Count > 0)
						{
							var playerWeight = PlayerHandler.GetInventoryWeight(player);
							var itemWeight = this._Monster._MonsterItems[0].Weight;
							if (playerWeight + itemWeight > player._MaxCarryWeight)
							{
								OutputHandler.Display.StoreUserOutput(
									Settings.FormatFailureOutputText(),
									Settings.FormatDefaultBackground(),
									"You can't carry that much!");
								return;
							}
							if (this._Monster._MonsterItems[0] is Consumable)
							{
								player._Consumables.Add((Consumable)this._Monster._MonsterItems[0]);
							}
							else
							{
								player._Inventory.Add(this._Monster._MonsterItems[0]);
							}
							var lootItemString = "You looted " + this._Monster._MonsterItems[0]._Name + " from the " +
												 this._Monster._Name + "!";
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatSuccessOutputText(),
								Settings.FormatDefaultBackground(),
								lootItemString);
							this._Monster._MonsterItems.RemoveAt(0);
						}
						this._Monster._WasLooted = true;
						var monsterIndex = this._RoomObjects.FindIndex(
							f => f._Name == this._Monster._Name);
						if (monsterIndex != -1) this._RoomObjects.RemoveAt(monsterIndex);
					}
					catch (InvalidOperationException)
					{
					}
				}
				else if (this._Monster._WasLooted)
				{
					var alreadyLootString = "You already looted " + this._Monster._Name + "!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						alreadyLootString);
				}
				else
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot loot something that isn't dead!");
				}
			}
			else
			{
				var noLootString = "There is no " + inputName + " in the room!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noLootString);
			}
		}
		private string CalculateNpcLevelDiff(Player player)
		{
			if (this._Monster == null) return null;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var monsterName = textInfo.ToTitleCase(this._Monster._Name);
			var levelDiff = player._Level - this._Monster._Level;
			var difficultyStringBuilder = new StringBuilder();
			difficultyStringBuilder.Append("Difficulty: ");
			if (levelDiff >= 3)
			{
				difficultyStringBuilder.Append("You will crush " + monsterName + " like a bug.");
			}
			else switch (levelDiff)
				{
					case 2:
						difficultyStringBuilder.Append("You have an edge over " + monsterName + ".");
						break;
					case 1:
						difficultyStringBuilder.Append("You have a slight edge over " + monsterName + ".");
						break;
					case 0:
						difficultyStringBuilder.Append("You're about even with " + monsterName + ".");
						break;
					default:
						{
							switch (levelDiff)
							{
								case -1:
									difficultyStringBuilder.Append(monsterName + " has a slight edge over you.");
									break;
								case -2:
									difficultyStringBuilder.Append(monsterName + " has an edge over you.");
									break;
								default:
									{
										if (levelDiff <= -3)
										{
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
		public void LookNpc(string[] input, Player player)
		{
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++)
			{
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this._Monster._Name.Split(' ');
			if (monsterName.Last() == inputName || this._Monster._Name.Contains(inputName))
			{
				for (var i = 0; i < this._Monster._Desc.Length; i += Settings.GetGameWidth())
				{
					if (this._Monster._Desc.Length - i < Settings.GetGameWidth())
					{
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							this._Monster._Desc.Substring(i, this._Monster._Desc.Length - i));
						continue;
					}
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						this._Monster._Desc.Substring(i, Settings.GetGameWidth()));
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
				if (this._Monster._MonsterItems == null) return;
				foreach (var item in this._Monster._MonsterItems)
				{
					var sameLineOutputItem = new List<string>();
					var sb = new StringBuilder();
					var itemTitle = item._Name;
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sameLineOutputItem.Add(Settings.FormatRoomOutputText());
					sameLineOutputItem.Add(Settings.FormatDefaultBackground());
					sameLineOutputItem.Add(sb.ToString());
					OutputHandler.Display.StoreUserOutput(sameLineOutputItem);
				}
			}
			else
			{
				var noNpcString = "There is no " + inputName + " in the room!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noNpcString);
			}
		}
	}
}