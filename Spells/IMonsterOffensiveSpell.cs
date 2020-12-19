using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Spells {
	public interface IMonsterOffensiveSpell : IName {
		int _DamageAmount { get; }
		int _ManaCost { get; }

		void CastSpell(Monster monster, Player player);
		int CalculateSpellDamage(Monster monster, Player player);
		int DecreaseSpellDamageFromPlayerResistance(Player player, int spellDamage);
		void DisplayEffectAbsorbedAllDamageMessage(Monster monster, Effect effect);
		void DisplaySuccessfulAttackMessage(Monster monster, int spellDamage);
	}
}
