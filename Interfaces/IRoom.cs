using System.Collections.Generic;
using System.Threading;

namespace DungeonGame {
	public interface IRoom : IRoomInteraction {
		bool IsDiscovered { get; set; }
		IRoom North { get; set; }
		IRoom South { get; set; }
		IRoom East { get; set; }
		IRoom West { get; set; }
		IRoom NorthWest { get; set; }
		IRoom SouthWest { get; set; }
		IRoom NorthEast { get; set; }
		IRoom SouthEast { get; set; }
		IRoom Up { get; set; }
		IRoom Down { get; set; }
		string Desc { get; set; }
		List<IRoomInteraction> RoomObjects { get; set; }
		Monster Monster { get; set; }

		void AttackOpponent(Player player, string[] input, Timer globalTimer);
		void LootCorpse(Player player, string[] input);
		void ShowDirections();
		void ShowCommands();
		void LookRoom();
		void LookNpc(string[] input, Player player);
	}
}