using DungeonGame.Monsters;
using DungeonGame.Players;
using System.Collections.Generic;

namespace DungeonGame.Helpers {
	public static class MonsterHelper {
		public static void DisplayStats(Monster monster) {
			string opponentHealthString = $"Opponent HP: {monster.HitPoints} / {monster.MaxHitPoints}";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				opponentHealthString);
			List<string> healLineOutput = new List<string>();
			int hitPointMaxUnits = monster.MaxHitPoints / 10;
			int hitPointUnits = monster.HitPoints / hitPointMaxUnits;
			for (int i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputHelper.Display.StoreUserOutput(healLineOutput);
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"==================================================");
		}
		public static int CheckArmorRating(Monster monster) {
			int totalArmorRating = 0;
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
			if (opponent.Spellbook[index].DamageGroup == DamageType.Physical) {
				return opponent.Spellbook[index].Offensive.Amount;
			}
			double damageReductionPercentage = opponent.Spellbook[index].DamageGroup switch {
				DamageType.Fire => player.FireResistance / 100.0,
				DamageType.Frost => player.FrostResistance / 100.0,
				DamageType.Arcane => player.ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(opponent.Spellbook[index].Offensive.Amount * (1 - damageReductionPercentage));
		}
	}
}