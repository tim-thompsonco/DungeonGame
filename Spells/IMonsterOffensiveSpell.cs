using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Spells {
	public interface IMonsterOffensiveSpell : IName {
		int _DamageAmount { get; }
		int _ManaCost { get; }

		void CastSpell(Monster monster, Player player);
		void DisplaySuccessfulAttackMessage(Monster monster, int spellDamage);
	}
}
