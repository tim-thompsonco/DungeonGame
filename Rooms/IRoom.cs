using DungeonGame.Monsters;
using DungeonGame.Players;
using System.Collections.Generic;
using System.Threading;

namespace DungeonGame.Rooms
{
	public interface IRoom : IName
	{
		bool _IsDiscovered { get; set; }
		IRoom _North { get; set; }
		IRoom _South { get; set; }
		IRoom _East { get; set; }
		IRoom _West { get; set; }
		IRoom _NorthWest { get; set; }
		IRoom _SouthWest { get; set; }
		IRoom _NorthEast { get; set; }
		IRoom _SouthEast { get; set; }
		IRoom _Up { get; set; }
		IRoom _Down { get; set; }
		string _Desc { get; set; }
		List<IName> _RoomObjects { get; set; }
		Monster _Monster { get; set; }

		void AttackOpponent(Player player, string[] input, Timer globalTimer);
		void LootCorpse(Player player, string[] input);
		void ShowDirections();
		void ShowCommands();
		void LookRoom();
		void LookNpc(string[] input, Player player);
	}
}