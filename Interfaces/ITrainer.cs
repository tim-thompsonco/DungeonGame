using System.Collections.Generic;

namespace DungeonGame {
	public interface ITrainer : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		int BaseCost { get; set; }
		List<Ability> TrainableAbilities { get; set; }
		List<Spell> TrainableSpells { get; set; }

		void TrainAbility(Player player, string inputName, UserOutput output);
		void TrainSpell(Player player, string inputName, UserOutput output);
		void UpgradeSpell(Player player, string inputName, UserOutput output);
		void UpgradeAbility(Player player, string inputName, UserOutput output);
	}
}