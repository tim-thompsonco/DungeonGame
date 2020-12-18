using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DungeonGame.Rooms {
	public class TownRoom : IRoom {
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
		public List<string> _Commands { get; set; }
		// List of objects in room (including Vendors)
		public List<IName> _RoomObjects { get; set; }
		public Vendor _Vendor;
		public Trainer _Trainer;
		public Monster _Monster { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public TownRoom() { }
		public TownRoom(string name, string desc) {
			_RoomObjects = new List<IName>();
			_Name = name;
			_Desc = desc;
			_Commands = new List<string> {
				"[I]nventory",
				"Save",
				"[Q]uit"};
		}
		public TownRoom(string name, string desc, Vendor vendor) : this(name, desc) {
			_Vendor = vendor;
			_RoomObjects.Add(_Vendor);
		}
		public TownRoom(string name, string desc, Trainer trainer) : this(name, desc) {
			_Trainer = trainer;
			_RoomObjects.Add(_Trainer);
		}

		public void AttackOpponent(Player player, string[] input, Timer globalTimer) { }
		public void LootCorpse(Player player, string[] input) { }
		public void ShowCommands() {
			List<string> sameLineOutput = new List<string> {
				Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), "Available _Commands: "};
			int objCount = _Commands.Count;
			foreach (string command in _Commands) {
				StringBuilder sb = new StringBuilder();
				sb.Append(command);
				if (_Commands[objCount - 1] != command) {
					sb.Append(", ");
				}
				if (_Commands[objCount - 1] == command) {
					sb.Append(".");
				}

				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(sb.ToString());
			}
			OutputController.Display.StoreUserOutput(sameLineOutput);
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
				OutputController.Display.StoreUserOutput(sameLineOutput);
			} else {
				sameLineOutput.Add(Settings.FormatInfoText());
				sameLineOutput.Add(Settings.FormatDefaultBackground());
				sameLineOutput.Add(roomDirs.ToString());
				OutputController.Display.StoreUserOutput(sameLineOutput);
				return;
			}
			string remainingRoomDirs = roomDirs.ToString().Substring(Settings.GetGameWidth() - directionList.Length);
			for (int i = 0; i < remainingRoomDirs.Length; i += Settings.GetGameWidth()) {
				if (remainingRoomDirs.Length - i < Settings.GetGameWidth()) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						remainingRoomDirs.Substring(i, remainingRoomDirs.Length - i));
					continue;
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					remainingRoomDirs.Substring(i, Settings.GetGameWidth()));
			}
		}
		public void LookRoom() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				_Name);
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			for (int i = 0; i < _Desc.Length; i += Settings.GetGameWidth()) {
				if (_Desc.Length - i < Settings.GetGameWidth()) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						_Desc.Substring(i, _Desc.Length - i));
					continue;
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(),
					Settings.FormatDefaultBackground(),
					_Desc.Substring(i, Settings.GetGameWidth()));
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			List<string> sameLineOutput = new List<string> {
				Settings.FormatFailureOutputText(), Settings.FormatDefaultBackground(), "Room Contents: "
			};
			if (_RoomObjects.Count > 0 && _RoomObjects[0] != null) {
				int objCount = _RoomObjects.Count;
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (IName item in _RoomObjects) {
					StringBuilder sb = new StringBuilder();
					string itemTitle = item._Name;
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
			OutputController.Display.StoreUserOutput(sameLineOutput);
			ShowDirections();
		}
		private string CalculateNpcLevelDiff(Player player) {
			return null; // Not using this method for vendors, no reason to attack them
		}
		public void LookNpc(string[] input, Player player) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			string inputName = inputString.ToString().Trim();
			int nameIndex = _RoomObjects.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			if (_RoomObjects[nameIndex].GetType() == typeof(Vendor)) {
				string[] vendorName = _Vendor._Name.Split(' ');
				if (vendorName.Last() == inputName || _Vendor._Name == inputName) {
					for (int i = 0; i < _Vendor._Desc.Length; i += Settings.GetGameWidth()) {
						if (_Vendor._Desc.Length - i < Settings.GetGameWidth()) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(),
								Settings.FormatDefaultBackground(),
								_Vendor._Desc.Substring(i, _Vendor._Desc.Length - i));
							continue;
						}
						OutputController.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							_Vendor._Desc.Substring(i, Settings.GetGameWidth()));
					}
					List<string> sameLineOutput = new List<string> {
						Settings.FormatRoomOutputText(),
						Settings.FormatDefaultBackground(),
						"The vendor is carrying: "};
					OutputController.Display.StoreUserOutput(sameLineOutput);
					TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
					foreach (IItem itemForSale in _Vendor._VendorItems) {
						List<string> sameLineOutputItem = new List<string>();
						StringBuilder sb = new StringBuilder();
						string itemTitle = itemForSale._Name;
						itemTitle = textInfo.ToTitleCase(itemTitle);
						sb.Append(itemTitle);
						sameLineOutputItem.Add(Settings.FormatRoomOutputText());
						sameLineOutputItem.Add(Settings.FormatDefaultBackground());
						sameLineOutputItem.Add(sb.ToString());
						OutputController.Display.StoreUserOutput(sameLineOutputItem);
					}
				} else {
					string noVendorString = "There is no " + inputName + " in the room!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noVendorString);
				}
			} else {
				string[] trainerName = _Trainer._Name.Split(' ');
				if (trainerName.Last() == inputName || _Trainer._Name == inputName) {
					for (int i = 0; i < _Trainer._Desc.Length; i += Settings.GetGameWidth()) {
						if (_Trainer._Desc.Length - i < Settings.GetGameWidth()) {
							OutputController.Display.StoreUserOutput(
								Settings.FormatRoomOutputText(),
								Settings.FormatDefaultBackground(),
								_Trainer._Desc.Substring(i, _Trainer._Desc.Length - i));
							continue;
						}
						OutputController.Display.StoreUserOutput(
							Settings.FormatRoomOutputText(),
							Settings.FormatDefaultBackground(),
							_Trainer._Desc.Substring(i, Settings.GetGameWidth()));
					}
				} else {
					string noTrainerString = "There is no " + inputName + " in the room!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						noTrainerString);
				}
			}
		}
	}
}