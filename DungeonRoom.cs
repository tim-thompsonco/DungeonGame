using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
		// List of objects in room (including monsters)
		public List<IRoomInteraction> RoomObjects { get; set; }
		public IMonster Monster;

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
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
			this.Commands = new List<string>() {
				"[I]nventory",
				"Save",
				"[Q]uit"};
			var randomNum = Helper.GetRandomNumber(1, 100);
			var randomNumLevel = Helper.GetRandomNumber(levelRangeLow, levelRangeHigh);
			// Reserving numbers 80-100 for chance of room not having a monster
			if (randomNum < 25) {
				this.Monster = new Monster(randomNumLevel, DungeonGame.Monster.MonsterType.Zombie);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum < 50) {
				this.Monster = new Monster(randomNumLevel, DungeonGame.Monster.MonsterType.Skeleton);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum < 70) {
				this.Monster = new Monster(randomNumLevel, DungeonGame.Monster.MonsterType.Spider);
				this.RoomObjects.Add(this.Monster);
			}
			else if (randomNum < 80) {
				this.Monster = new Monster(randomNumLevel, DungeonGame.Monster.MonsterType.Demon);
				this.RoomObjects.Add(this.Monster);
			}
		}
		
		public IMonster GetMonster() {
			return this.Monster;
		}
		public bool AttackOpponent(
			Player player, string[] input, UserOutput output, UserOutput mapOutput, List<IRoom> roomList) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this.Monster.GetName().Split(' ');
			if (monsterName.Last() == inputName || this.Monster.GetName() == inputName) {
				if (this.Monster.HitPoints > 0) {
					var fightEvent = new CombatHelper();
					var outcome = fightEvent.SingleCombat(this.Monster, player, output, mapOutput, roomList);
					switch (outcome) {
						case false when player.HitPoints <= 0:
							Helper.PlayerDeath(output);
							return false;
						case false:
							return false;
					}
				}
				else {
					var monsterDeadString = "The " + this.Monster.Name + " is already dead."; 
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						monsterDeadString);
				}
			}
			else {
				var noMonsterString = "There is no " + inputName + " to attack.";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					noMonsterString);
			}
			return true;
		}
		public void RebuildRoomObjects() {
			this.RoomObjects.Clear();
			if (this.Monster != null && !this.Monster.WasLooted) {
				this.RoomObjects.Add(this.Monster);
			}
		}
		public void ShowCommands(UserOutput output) {
			var sameLineOutput = new List<string> {
			Helper.FormatGeneralInfoText(), Helper.FormatDefaultBackground(), "Available Commands: "};
			var objCount = this.Commands.Count;
			foreach (var command in this.Commands) {
				var sb = new StringBuilder();
				sb.Append(command);
				if (this.Commands[objCount - 1] != command) {
					sb.Append(", ");
				}
				if (this.Commands[objCount - 1] == command) sb.Append(".");
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add(sb.ToString());
			}
			output.StoreUserOutput(sameLineOutput);
		}
		public void ShowDirections(UserOutput output) {
			var sameLineOutput = new List<string> {
				Helper.FormatRoomOutputText(), 
				Helper.FormatDefaultBackground(),
				"Available Directions: "};
			var sb = new StringBuilder();
			if (this.GoNorth) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[N]orth ");
			}
			if (this.GoSouth) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[S]outh ");
			}
			if (this.GoEast) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[E]ast ");
			}
			if (this.GoWest) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[W]est ");
			}
			if (this.GoNorthWest) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[N]orth[W]est ");
			}
			if (this.GoSouthWest) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[S]outh[W]est ");
			}
			if (this.GoNorthEast) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[N]orth[E]ast ");
			}
			if (this.GoSouthEast) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[S]outh[E]ast ");
			}
			if (this.GoUp) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[U]p ");
			}
			if (this.GoDown) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("[D]own");
			}
			output.StoreUserOutput(sameLineOutput);
		}
		public void LookRoom(UserOutput output) {
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				Helper.FormatTextBorder());
			output.StoreUserOutput(
				Helper.FormatRoomOutputText(), 
				Helper.FormatDefaultBackground(), 
				this.Name);
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				Helper.FormatTextBorder());
			for (var i = 0; i < this.Desc.Length; i += Helper.GetGameWidth()) {
				if (this.Desc.Length - i < Helper.GetGameWidth()) {
					output.StoreUserOutput(
						Helper.FormatRoomOutputText(), 
						Helper.FormatDefaultBackground(), 
						this.Desc.Substring(i, this.Desc.Length - i));
					continue;
				}
				output.StoreUserOutput(
					Helper.FormatRoomOutputText(), 
					Helper.FormatDefaultBackground(), 
					this.Desc.Substring(i, Helper.GetGameWidth()));
			}
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				Helper.FormatTextBorder());
			var sameLineOutput = new List<string> {
				Helper.FormatRoomOutputText(),
				Helper.FormatDefaultBackground(), 
				"Room Contents: "};
			if (this.RoomObjects.Count > 0 && this.RoomObjects[0] != null) {
				var objCount = this.RoomObjects.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var item in this.RoomObjects) {
					var sb = new StringBuilder();
					var itemTitle = item.GetName();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (this.RoomObjects[objCount - 1] != item) {
						sb.Append(", ");
					}
					sb.Append(".");
					sameLineOutput.Add(Helper.FormatInfoText());
					sameLineOutput.Add(Helper.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
			}
			else {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add("There is nothing in the room.");
			}
			output.StoreUserOutput(sameLineOutput);
			this.ShowDirections(output);
		}
		public void LootCorpse(Player player, string[] input, UserOutput output) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this.Monster.GetName().Split(' ');
			if (monsterName.Last() == inputName || this.Monster.GetName() == inputName) {
				if (this.Monster.HitPoints <= 0 && this.Monster.WasLooted == false) {
					var goldLooted = this.Monster.Gold;
					player.Gold += this.Monster.Gold;
					try {
						this.Monster.Gold = 0;
						var lootGoldString = "You looted " + goldLooted + " gold coins from the " + this.Monster.Name + "!";
						output.StoreUserOutput(
							Helper.FormatSuccessOutputText(),
							Helper.FormatDefaultBackground(),
							lootGoldString);
						for (var i = 0; i < this.Monster.MonsterItems.Count; i++) {
							var itemType = this.Monster.MonsterItems[i].GetType().FullName;
							var playerWeight = PlayerHelper.GetInventoryWeight(player);
							var itemWeight = this.Monster.MonsterItems[i].Weight;
							if (playerWeight + itemWeight > player.MaxCarryWeight) {
								output.StoreUserOutput(
									Helper.FormatFailureOutputText(),
									Helper.FormatDefaultBackground(),
									"You can't carry that much!");
								return;
							}
							if (itemType == "DungeonGame.Consumable") {
								player.Consumables.Add((Consumable)this.Monster.MonsterItems[i]);
							}
							else {
								player.Inventory.Add(this.Monster.MonsterItems[i]);
							}
							var lootItemString = "You looted " + this.Monster.MonsterItems[i].GetName() + " from the " +
							                     this.Monster.Name + "!";
							output.StoreUserOutput(
								Helper.FormatSuccessOutputText(),
								Helper.FormatDefaultBackground(),
								lootItemString);
							this.Monster.MonsterItems.RemoveAt(i);
						}
						this.Monster.MonsterItems.Clear();
						this.Monster.WasLooted = true;
						var monsterIndex = this.RoomObjects.FindIndex(
							f => f.GetName() == this.Monster.Name);
						if (monsterIndex != -1) this.RoomObjects.RemoveAt(monsterIndex);
					}
					catch (InvalidOperationException) {
					}
				}
				else if (this.Monster.WasLooted) {
					var alreadyLootString = "You already looted " + this.Monster.Name + "!";
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						alreadyLootString);
				}
				else {
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You cannot loot something that isn't dead!");
				}
			}
			else {
				var noLootString = "There is no " + inputName + " in the room!"; 
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					noLootString);
			}
		}
		public void LookNpc(string[] input, UserOutput output) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = this.Monster.GetName().Split(' ');
			if (monsterName.Last() == inputName || this.Monster.GetName() == inputName) {
				for (var i = 0; i < this.Monster.Desc.Length; i += Helper.GetGameWidth()) {
					if (this.Monster.Desc.Length - i < Helper.GetGameWidth()) {
						output.StoreUserOutput(
							Helper.FormatRoomOutputText(), 
							Helper.FormatDefaultBackground(), 
							this.Monster.Desc.Substring(i, this.Monster.Desc.Length - i));
						continue;
					}
					output.StoreUserOutput(
						Helper.FormatRoomOutputText(), 
						Helper.FormatDefaultBackground(), 
						this.Monster.Desc.Substring(i, Helper.GetGameWidth()));
				}
				var sameLineOutput = new List<string>() {
					Helper.FormatRoomOutputText(),
					Helper.FormatDefaultBackground(),
					"It is carrying: "};
				output.StoreUserOutput(sameLineOutput);
				var objCount = this.Monster.MonsterItems.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var item in this.Monster.MonsterItems) {
					var sameLineOutputItem = new List<string>();
					var sb = new StringBuilder();
					var itemTitle = item.GetName();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sameLineOutputItem.Add(Helper.FormatRoomOutputText());
					sameLineOutputItem.Add(Helper.FormatDefaultBackground());
					sameLineOutputItem.Add(sb.ToString());
					output.StoreUserOutput(sameLineOutputItem);
				}
			}
			else {
				var noNpcString = "There is no " + inputName + " in the room!";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					noNpcString);
			}
		}
	}
}