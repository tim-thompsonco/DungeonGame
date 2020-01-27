using System.Collections.Generic;

namespace DungeonGame {
	public interface ITrainer : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		int BaseCost { get; set; }
		List<Ability> TrainableAbilities { get; set; }
		List<Spell> TrainableSpells { get; set; }

		void DisplayAvailableUpgrades(Player player);
		void TrainAbility(Player player, string inputName);
		void TrainSpell(Player player, string inputName);
		void UpgradeSpell(Player player, string inputName);
		void UpgradeAbility(Player player, string inputName);
	}
}