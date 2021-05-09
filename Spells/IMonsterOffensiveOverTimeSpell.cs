using DungeonGame.Players;

namespace DungeonGame.Spells {
	public interface IMonsterOffensiveOverTimeSpell {
		int DamageOverTimeAmount { get; }
		int MaxDamageRounds { get; }

		void DisplayDamageOverTimeMessage();
		void AddDamageOverTimeEffect(Player player);
	}
}
