using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Spells {
	public interface IMonsterOffensiveSpell : IName {
		int DamageAmount { get; }
		int ManaCost { get; }

		void CastSpell(Monster monster, Player player);
		void DisplaySuccessfulAttackMessage(Monster monster, int spellDamage);
	}
}
