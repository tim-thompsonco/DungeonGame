using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame {
	public class TownRoom : IRoom {
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
		public List<string> Commands { get; set; }
		// List of objects in room (including Vendors)
		public List<IRoomInteraction> RoomObjects { get; set; }
		public Vendor Vendor;
		public Trainer Trainer;
		public Monster Monster { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public TownRoom() {}
		public TownRoom(string name, string desc) {
			this.RoomObjects = new List<IRoomInteraction>();
			this.Name = name;
			this.Desc = desc;
			this.Commands = new List<string> {
				"[I]nventory",
				"Save",
				"[Q]uit"};
		}
		public TownRoom(string name, string desc, Vendor vendor) : this(name, desc) {
			this.Vendor = vendor;
			this.RoomObjects.Add(this.Vendor);
		}
		public TownRoom(string name, string desc, Trainer trainer) : this(name, desc) {
			this.Trainer = trainer;
			this.RoomObjects.Add(this.Trainer);
		}

		public void AttackOpponent(Player player, string[] input, Timer globalTimer) {}
		public void LootCorpse(Player player, string[] input) { }
		public void ShowCommands() {
			var sameLineOutput = new List<string> {
				Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available Commands: "};
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
			OutputHandler.Display.StoreUserOutput(sameLineOutput);
		}
		public void ShowDirections() {
			const string directionList = "Available Directions: ";
			var sameLineOutput = new List<string> {
				Settings.FormatRoomOutputText(), 
				Settings.FormatDefaultBackground(),
				directionList};
			var roomDirs = new StringBuilder();
			if (this.North != null) {
				roomDirs.Append("[N]orth ");
			}
			if (this.South != null) {
				roomDirs.Append("[S]outh ");
			}
			if (this.East != null) {
				roomDirs.Append("[E]ast ");
			}
			if (this.West != null) {
				roomDirs.Append("[W]est ");
			}
			if (this.NorthWest != null) {
				roomDirs.Append("[N]orth[W]est ");
			}
			if (this.SouthWest != null) {
				roomDirs.Append("[S]outh[W]est ");
			}
			if (this.NorthEast != null) {
				roomDirs.Append("[N]orth[E]ast ");
			}
			if (this.SouthEast != null) {
				roomDirs.Append("[S]outh[E]ast ");
			}
			if (this.Up != null) {
				roomDirs.Append("[U]p ");
			}
			if (this.Down != null) {
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
				Settings.FormatFailureOutputText(), 
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
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "
			};
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
		private string CalculateNpcLevelDiff(Player player) {
			return null; // Not using this method for vendors, no reason to attack them
		}
		public void LookNpc(string[] input, Player player) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var nameIndex = this.RoomObjects.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (this.RoomObjects[nameIndex].GetType() == typeof(Vendor)) {
				var vendorName = this.Vendor.Name.Split(' ');
				if (vendorName.Last() == inputName || this.Vendor.Name == inputName) {
					for (var i = 0; i < this.Vendor.Desc.Length; i += Settings.GetGameWidth()) {
						if (this.Vendor.Desc.Length - i < Settings.GetGameWidth()) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(), 
								Settings.FormatDefaultBackground(), 
								this.Vendor.Desc.Substring(i, this.Vendor.Desc.Length - i));
							continue;
						}
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(), 
							Settings.FormatDefaultBackground(), 
							this.Vendor.Desc.Substring(i, Settings.GetGameWidth()));
					}
					var sameLineOutput = new List<string> {
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						"The vendor is carrying: "};
					OutputHandler.Display.StoreUserOutput(sameLineOutput);
					var textInfo = new CultureInfo("en-US", false).TextInfo;
					foreach (var itemForSale in this.Vendor.VendorItems) {
						var sameLineOutputItem = new List<string>();
						var sb = new StringBuilder();
						var itemTitle = itemForSale.Name;
						itemTitle = textInfo.ToTitleCase(itemTitle);
						sb.Append(itemTitle);
						sameLineOutputItem.Add(Settings.FormatRoomOutputText());
						sameLineOutputItem.Add(Settings.FormatDefaultBackground());
						sameLineOutputItem.Add(sb.ToString());
						OutputHandler.Display.StoreUserOutput(sameLineOutputItem);
					}
				}
				else {
					var noVendorString = "There is no " + inputName + " in the room!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noVendorString);
				}
			}
			else {
				var trainerName = this.Trainer.Name.Split(' ');
				if (trainerName.Last() == inputName || this.Trainer.Name == inputName) {
					for (var i = 0; i < this.Trainer.Desc.Length; i += Settings.GetGameWidth()) {
						if (this.Trainer.Desc.Length - i < Settings.GetGameWidth()) {
							OutputHandler.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(), 
								Settings.FormatDefaultBackground(), 
								this.Trainer.Desc.Substring(i, this.Trainer.Desc.Length - i));
							continue;
						}
						OutputHandler.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(), 
							Settings.FormatDefaultBackground(), 
							this.Trainer.Desc.Substring(i, Settings.GetGameWidth()));
					}
				}
				else {
					var noTrainerString = "There is no " + inputName + " in the room!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noTrainerString);
				}
			}
		}
	}
}