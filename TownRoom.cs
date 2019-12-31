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
		public List<string> Commands { get; set; } = new List<string>() {
			"[I]nventory",
			"Save",
			"[Q]uit"};
		// List of objects in room (including monsters)
		private readonly List<IRoomInteraction> _roomObjects = new List<IRoomInteraction>();
		public IVendor Vendor;
		public IMonster Monster;

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
		public bool AttackOpponent(Player player, string[] input, UserOutput output) {
			return true;
		}
		public void LootCorpse(Player player, string[] input) { }
		public void RebuildRoomObjects() {
			this._roomObjects?.Clear();
			this._roomObjects.Add((IRoomInteraction) this.Vendor);
		}
		public void ShowCommands() {
			Helper.FormatGeneralInfoText();
			Console.Write("Available Commands: ");
			Console.WriteLine(string.Join(", ", this.Commands));
		}
		public void ShowDirections(UserOutput output) {
			var sameLineOutput = new List<string> {"darkcyan", "black", "Available Directions: "};
			Helper.FormatRoomInfoText(); // white
			var sb = new StringBuilder();
			if (this.GoNorth) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[N]orth ");
			}
			if (this.GoSouth) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[S]outh ");
			}
			if (this.GoEast) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[E]ast ");
			}
			if (this.GoWest) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[W]est ");
			}
			if (this.GoNorthWest) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[N]orth[W]est ");
			}
			if (this.GoSouthWest) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[S]outh[W]est ");
			}
			if (this.GoNorthEast) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[N]orth[E]ast ");
			}
			if (this.GoSouthEast) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[S]outh[E]ast ");
			}
			if (this.GoUp) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[U]p ");
			}
			if (this.GoDown) {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("[D]own");
			}
			output.StoreUserOutput(sameLineOutput);
		}
		public void LookRoom(UserOutput output) {
			output.StoreUserOutput(
				"darkgreen", 
				"black", 
				"==================================================");
			output.StoreUserOutput(
				"darkcyan", 
				"black", 
				this.Name);
			output.StoreUserOutput(
				"darkgreen", 
				"black", 
				"==================================================");
			output.StoreUserOutput(
				"darkcyan", 
				"black", 
				this.Desc);
			output.StoreUserOutput(
				"darkgreen", 
				"black", 
				"==================================================");
			var sameLineOutput = new List<string> {"darkcyan", "black", "Room Contents: "};
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
					sameLineOutput.Add("white");
					sameLineOutput.Add("black");
					sameLineOutput.Add(sb.ToString());
				}
			}
			else {
				sameLineOutput.Add("white");
				sameLineOutput.Add("black");
				sameLineOutput.Add("There is nothing in the room");
			}
			output.StoreUserOutput(sameLineOutput);
			// this.ShowDirections();
		}
		public void LookNpc(string[] input) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var vendorName = this.Vendor.GetName().Split(' ');
			if (vendorName.Last() == inputName || this.Vendor.GetName() == inputName) {
				Helper.FormatGeneralInfoText();
				Console.WriteLine(this.Vendor.Desc);
				Console.Write("\nHe is carrying:\n");
				foreach (var itemForSale in this.Vendor.VendorItems) {
					Console.WriteLine(string.Join(", ", itemForSale.GetName()));
				}
			}
			else {
				Helper.FormatFailureOutputText();
				Console.WriteLine("There is no {0} in the room!", inputName);
			}
		}
	}
}