using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Trainers;
using DungeonGame.Vendors;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame.Rooms {
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
		public List<IName> RoomObjects { get; set; }
		public Vendor Vendor;
		public Trainer Trainer;
		public Monster Monster { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public TownRoom() { }
		public TownRoom(string name, string desc) {
			RoomObjects = new List<IName>();
			Name = name;
			Desc = desc;
			Commands = new List<string> {
				"[I]nventory",
				"Save",
				"[Q]uit"};
		}
		public TownRoom(string name, string desc, Vendor vendor) : this(name, desc) {
			Vendor = vendor;
			RoomObjects.Add(Vendor);
		}
		public TownRoom(string name, string desc, Trainer trainer) : this(name, desc) {
			Trainer = trainer;
			RoomObjects.Add(Trainer);
		}

		public void AttackOpponent(Player player, string[] input, Timer globalTimer) { }
		public void LootCorpse(Player player, string[] input) { }
		public void ShowCommands() {
			List<string> sameLineOutput = new List<string> {
				Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available _Commands: "};
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
				Settings.FormatFailureOutputText(),
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
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "
			};
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

		public void LookNpc(string[] input, Player player) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			string inputName = inputString.ToString().Trim();
			int nameIndex = RoomObjects.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			if (RoomObjects[nameIndex].GetType() == typeof(Vendor)) {
				string[] vendorName = Vendor.Name.Split(' ');
				if (vendorName.Last() == inputName || Vendor.Name == inputName) {
					for (int i = 0; i < Vendor.Desc.Length; i += Settings.GetGameWidth()) {
						if (Vendor.Desc.Length - i < Settings.GetGameWidth()) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(),
								Settings.FormatDefaultBackground(),
								Vendor.Desc.Substring(i, Vendor.Desc.Length - i));
							continue;
						}
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							Vendor.Desc.Substring(i, Settings.GetGameWidth()));
					}
					List<string> sameLineOutput = new List<string> {
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						"The vendor is carrying: "};
					OutputHelper.Display.StoreUserOutput(sameLineOutput);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					foreach (IItem itemForSale in Vendor.VendorItems) {
						List<string> sameLineOutputItem = new List<string>();
						StringBuilder sb = new StringBuilder();
						string itemTitle = itemForSale.Name;
						itemTitle = textInfo.ToTitleCase(itemTitle);
						sb.Append(itemTitle);
						sameLineOutputItem.Add(Settings.FormatRoomOutputText());
						sameLineOutputItem.Add(Settings.FormatDefaultBackground());
						sameLineOutputItem.Add(sb.ToString());
						OutputHelper.Display.StoreUserOutput(sameLineOutputItem);
					}
				} else {
					string noVendorString = "There is no " + inputName + " in the room!";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noVendorString);
				}
			} else {
				string[] trainerName = Trainer.Name.Split(' ');
				if (trainerName.Last() == inputName || Trainer.Name == inputName) {
					for (int i = 0; i < Trainer.Desc.Length; i += Settings.GetGameWidth()) {
						if (Trainer.Desc.Length - i < Settings.GetGameWidth()) {
							OutputHelper.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(),
								Settings.FormatDefaultBackground(),
								Trainer.Desc.Substring(i, Trainer.Desc.Length - i));
							continue;
						}
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							Trainer.Desc.Substring(i, Settings.GetGameWidth()));
					}
				} else {
					string noTrainerString = "There is no " + inputName + " in the room!";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noTrainerString);
				}
			}
		}
	}
}