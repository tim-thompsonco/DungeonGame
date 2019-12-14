using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame {
  public class DungeonRoom : IRoom {
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
      "[Q]uit"};
    // List of objects in room (including monsters)
    private readonly List<IRoomInteraction> RoomObjects = new List<IRoomInteraction>();
    public IMonster Monster;

    //Constructor without monster
    public DungeonRoom (
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
		//Constructor with monster
		public DungeonRoom(
			string name,
			string desc,
			int x,
			int y,
			int z,
			IMonster monster,
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
			this.Monster = monster;
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

		public void AttackMonster(Player player, string[] input) {
			var inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
				}
			var inputName = inputString.ToString().Trim();
			var monsterName = Monster.GetName().Split(' ');
			if (monsterName.Last() == inputName || Monster.GetName() == inputName) {
				if (this.Monster.HitPoints > 0) {
					var fightEvent = new CombatHelper();
					var outcome = fightEvent.SingleCombat(Monster, player);
					if (outcome == false) {
						Helper.PlayerDeath();
					}
				}
				else {
					Console.WriteLine("The {0} is already dead.", this.Monster.Name);
				}
			}
			else {
				Console.WriteLine("There is no {0} to attack.", inputName);
			}
		}
    public void RebuildRoomObjects() {
			try {
				RoomObjects.Clear();
				if(!this.Monster.WasLooted) {
					RoomObjects.Add((DungeonGame.IRoomInteraction)Monster);
				}
			}
			catch(NullReferenceException) {
			}
    }
    public void ShowCommands() {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.Write("Available Commands: ");
      Console.WriteLine(String.Join(", ", this.Commands));
    }
    public void ShowDirections() {
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write("Available Directions: ");
      Console.ForegroundColor = ConsoleColor.White;
      if (this.GoNorth) {
        Console.Write("[N]orth ");
      }
      if (this.GoSouth) {
        Console.Write("[S]outh ");
      }
			if (this.GoEast) {
				Console.Write("[E]ast ");
			}
			if (this.GoWest) {
				Console.Write("[W]est ");
			}
			if (this.GoNorthWest) {
				Console.Write("[N]orth[W]est ");
			}
			if (this.GoSouthWest) {
				Console.Write("[S]outh[W]est ");
			}
			if (this.GoNorthEast) {
				Console.Write("[N]orth[E]ast ");
			}
			if (this.GoSouthEast) {
				Console.Write("[S]outh[E]ast ");
			}
			if (this.GoUp) {
				Console.Write("[U]p ");
			}
			if (this.GoDown) {
				Console.Write("[D]own ");
			}
			Console.WriteLine();
    }
    public void LookRoom() {
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.Name);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(this.Desc);
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine("==================================================");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("Room Contents: ");
			Console.ForegroundColor = ConsoleColor.White;
			this.RebuildRoomObjects();
			if(RoomObjects.Count > 0) {
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				foreach (IRoomInteraction item in RoomObjects) {
					var itemTitle = item.GetName().ToString();
					itemTitle = textInfo.ToTitleCase(itemTitle);
					Console.Write(string.Join(", ", itemTitle));
				}
			}
			else {
				Console.Write("There is nothing in the room");
			}
			Console.WriteLine("."); // Add period at end of list of objects in room
      this.ShowDirections();
    }
		public void LootCorpse(Player player, string[] input) {
			var inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
				}
			var inputName = inputString.ToString().Trim();
			var monsterName = Monster.GetName().Split(' ');
			if (monsterName.Last() == inputName || Monster.GetName() == inputName) {
				if (Monster.HitPoints <= 0 && Monster.WasLooted == false) {
					var goldLooted = Monster.Gold;
					player.Gold += Monster.Gold;
					Console.ForegroundColor = ConsoleColor.Green;
					try {
						foreach (var loot in Monster.MonsterItems) {
							player.Inventory.Add(loot);
							Console.WriteLine("You looted {0} from the {1}!", loot.GetName(), this.Monster.Name);
						}
					}
					catch (InvalidOperationException) {
					}
					Monster.MonsterItems.Clear();
					Monster.Gold = 0;
					Monster.WasLooted = true;
					Console.WriteLine("You looted {0} gold coins from the {1}!", goldLooted, this.Monster.Name);
				}
				else if (Monster.WasLooted) {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("You already looted {0}!", this.Monster.Name);
				}
				else {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("You cannot loot something that isn't dead!");
				}
			}
			else {
				Console.WriteLine("There is no {0} in the room!", inputName);
			}
		}
		public void LookMonster(string[] input) {
			var inputString = new StringBuilder();
			for(int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var monsterName = Monster.GetName().Split(' ');
			if(monsterName.Last() == inputName || Monster.GetName() == inputName) {
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.WriteLine(Monster.Desc);
				Console.Write("\nHe is carrying: ");
				foreach (var loot in Monster.MonsterItems) {
					Console.WriteLine(string.Join(", ", loot.GetName()));
				}
			}
			else {
				Console.WriteLine("There is no {0} in the room!", inputName);
			}
		}
	}
}