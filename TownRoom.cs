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
		private readonly List<IRoomInteraction> _roomObjects = new List<IRoomInteraction>();
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
			this._roomObjects?.Clear();
			this._roomObjects.Add((IRoomInteraction) this.Vendor);
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
				Helper.FormatFailureOutputText(), 
				Helper.FormatDefaultBackground(), "Available Directions: "};
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
						"darkcyan", 
						Helper.FormatDefaultBackground(), 
						this.Desc.Substring(i, this.Desc.Length - i));
					continue;
				}
				output.StoreUserOutput(
					"darkcyan", 
					Helper.FormatDefaultBackground(), 
					this.Desc.Substring(i, Helper.GetGameWidth()));
			}
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				Helper.FormatTextBorder());
			var sameLineOutput = new List<string> {Helper.FormatFailureOutputText(), Helper.FormatDefaultBackground(), "Room Contents: "};
			this.RebuildRoomObjects();
			if (this._roomObjects.Count > 0 && this._roomObjects[0] != null) {
				var objCount = this._roomObjects.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var item in this._roomObjects) {
					var sb = new StringBuilder();
					var itemTitle = item.GetName();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					if (this._roomObjects[objCount - 1] != item) {
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
				var objCount = this.Vendor.VendorItems.Count;
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (var itemForSale in this.Vendor.VendorItems) {
					var sb = new StringBuilder();
					var itemTitle = itemForSale.GetName();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					sb.Append(itemTitle);
					sb.Append(this.Vendor.VendorItems[objCount - 1] != itemForSale ? ", " : ".");
					sameLineOutput.Add(Helper.FormatRoomOutputText());
					sameLineOutput.Add(Helper.FormatDefaultBackground());
					sameLineOutput.Add(sb.ToString());
				}
				output.StoreUserOutput(sameLineOutput);
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