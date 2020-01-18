using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class TownRoom : IRoom {
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
		// List of objects in room (including Vendors)
		public List<IRoomInteraction> RoomObjects { get; set; }
		public IVendor Vendor;
		public IMonster Monster;

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public TownRoom() {}
		public TownRoom(
			string name,
			string desc,
			int x,
			int y,
			int z,
			bool goNorth,
			bool goSouth,
			bool goEast,
			bool goWest,
			bool goNorthWest,
			bool goSouthWest,
			bool goNorthEast,
			bool goSouthEast,
			bool goUp,
			bool goDown
			) {
			this.RoomObjects = new List<IRoomInteraction>();
			this.Name = name;
			this.Desc = desc;
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
		}
		public TownRoom(
			string name,
			string desc,
			int x,
			int y,
			int z,
			bool goNorth,
			bool goSouth,
			bool goEast,
			bool goWest,
			bool goNorthWest,
			bool goSouthWest,
			bool goNorthEast,
			bool goSouthEast,
			bool goUp,
			bool goDown,
			IVendor vendor
			)
			: this(name, 
				desc, 
				x, 
				y, 
				z,
				goNorth,
				goSouth, 
				goEast,
				goWest,
				goNorthWest, 
				goSouthWest,
				goNorthEast, 
				goSouthEast, 
				goUp, 
				goDown) {
			this.Vendor = vendor;
			this.RoomObjects.Add(this.Vendor);
		}

		public IMonster GetMonster() {
			return this.Monster;
		}
		public bool AttackOpponent(
			Player player, string[] input, UserOutput output, UserOutput mapOutput, List<IRoom> roomList) {
			return true;
		}
		public void LootCorpse(Player player, string[] input, UserOutput output) { }
		public void RebuildRoomObjects() {
			this.RoomObjects?.Clear();
			this.RoomObjects.Add((IRoomInteraction) this.Vendor);
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
			const string directionList = "Available Directions: ";
			var sameLineOutput = new List<string> {
				Helper.FormatRoomOutputText(), 
				Helper.FormatDefaultBackground(),
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
			if (directionList.Length + roomDirs.ToString().Length > Helper.GetGameWidth()) {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString().Substring(
					0, Helper.GetGameWidth() - directionList.Length));
				output.StoreUserOutput(sameLineOutput);
			}
			else {
				sameLineOutput.Add(Helper.FormatInfoText());
				sameLineOutput.Add(Helper.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString());
				output.StoreUserOutput(sameLineOutput);
				return;
			}
			var remainingRoomDirs = roomDirs.ToString().Substring(Helper.GetGameWidth() - directionList.Length);
			for (var i = 0; i < remainingRoomDirs.Length; i += Helper.GetGameWidth()) {
				if (remainingRoomDirs.Length - i < Helper.GetGameWidth()) {
					output.StoreUserOutput(
						Helper.FormatInfoText(), 
						Helper.FormatDefaultBackground(), 
						remainingRoomDirs.Substring(i, remainingRoomDirs.Length - i));
					continue;
				}
				output.StoreUserOutput(
					Helper.FormatInfoText(), 
					Helper.FormatDefaultBackground(), 
					remainingRoomDirs.Substring(i, Helper.GetGameWidth()));
			}
		}
		public void LookRoom(UserOutput output) {
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				Helper.FormatTextBorder());
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(), 
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
				Helper.FormatFailureOutputText(), Helper.FormatDefaultBackground(), "Room Contents: "
			};
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
				sameLineOutput.Add("There is nothing in the room");
			}
			output.StoreUserOutput(sameLineOutput); 
			this.ShowDirections(output);
		}
		public void LookNpc(string[] input, UserOutput output) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var vendorName = this.Vendor.GetName().Split(' ');
			if (vendorName.Last() == inputName || this.Vendor.GetName() == inputName) {
				for (var i = 0; i < this.Vendor.Desc.Length; i += Helper.GetGameWidth()) {
					if (this.Vendor.Desc.Length - i < Helper.GetGameWidth()) {
						output.StoreUserOutput(
							Helper.FormatRoomOutputText(), 
							Helper.FormatDefaultBackground(), 
							this.Vendor.Desc.Substring(i, this.Vendor.Desc.Length - i));
						continue;
					}
					output.StoreUserOutput(
						Helper.FormatRoomOutputText(), 
						Helper.FormatDefaultBackground(), 
						this.Vendor.Desc.Substring(i, Helper.GetGameWidth()));
				}
				var sameLineOutput = new List<string>() {
					Helper.FormatRoomOutputText(),
					Helper.FormatDefaultBackground(),
					"He is carrying: "};
				output.StoreUserOutput(sameLineOutput);
				var objCount = this.Vendor.VendorItems.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var itemForSale in this.Vendor.VendorItems) {
					var sameLineOutputItem = new List<string>();
					var sb = new StringBuilder();
					var itemTitle = itemForSale.GetName();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sameLineOutputItem.Add(Helper.FormatRoomOutputText());
					sameLineOutputItem.Add(Helper.FormatDefaultBackground());
					sameLineOutputItem.Add(sb.ToString());
					output.StoreUserOutput(sameLineOutputItem);
				}
			}
			else {
				var noVendorString = "There is no " + inputName + " in the room!";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					noVendorString);
			}
		}
	}
}