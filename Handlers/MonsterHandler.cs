using System.Collections.Generic;

namespace DungeonGame {
	public static class MonsterHandler {
		public static void DisplayStats(Monster monster) {
			var opponentHealthString = "Opponent HP: " + monster.HitPoints + " / " + monster.MaxHitPoints;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				opponentHealthString);
			var healLineOutput = new List<string>();
			var hitPointMaxUnits = monster.MaxHitPoints / 10;
			var hitPointUnits = monster.HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputHandler.Display.StoreUserOutput(healLineOutput);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"==================================================");
		}
		public static int CheckArmorRating(Monster monster) {
			var totalArmorRating = 0;
			if (monster.MonsterChestArmor != null && monster.MonsterChestArmor.Equipped) {
				totalArmorRating += monster.MonsterChestArmor.ArmorRating;
			}
			if (monster.MonsterHeadArmor != null && monster.MonsterHeadArmor.Equipped) {
				totalArmorRating += monster.MonsterHeadArmor.ArmorRating;
			}
			if (monster.MonsterLegArmor != null && monster.MonsterLegArmor.Equipped) {
				totalArmorRating += monster.MonsterLegArmor.ArmorRating;
			}
			return totalArmorRating;
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index) {
			if (opponent.Spellbook[index].DamageGroup == MonsterSpell.DamageType.Physical) {
				return opponent.Spellbook[index].Offensive.Amount;
			}
			var damageReductionPercentage = opponent.Spellbook[index].DamageGroup switch {
				MonsterSpell.DamageType.Fire => (player.FireResistance / 100.0),
				MonsterSpell.DamageType.Frost => (player.FrostResistance / 100.0),
				MonsterSpell.DamageType.Arcane => (player.ArcaneResistance / 100.0),
				_ => 0.0
			};
			return (int)(opponent.Spellbook[index].Offensive.Amount * (1 - damageReductionPercentage));
		}
	}
}