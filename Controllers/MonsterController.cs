using DungeonGame.Monsters;
using DungeonGame.Players;
using System.Collections.Generic;

namespace DungeonGame.Controllers {
	public static class MonsterController {
		public static void DisplayStats(Monster monster) {
			string opponentHealthString = $"Opponent HP: {monster._HitPoints} / {monster._MaxHitPoints}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				opponentHealthString);
			List<string> healLineOutput = new List<string>();
			int hitPointMaxUnits = monster._MaxHitPoints / 10;
			int hitPointUnits = monster._HitPoints / hitPointMaxUnits;
			for (int i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputController.Display.StoreUserOutput(healLineOutput);
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"==================================================");
		}
		public static int CheckArmorRating(Monster monster) {
			int totalArmorRating = 0;
			if (monster._MonsterChestArmor != null && monster._MonsterChestArmor._Equipped) {
				totalArmorRating += monster._MonsterChestArmor._ArmorRating;
			}
			if (monster._MonsterHeadArmor != null && monster._MonsterHeadArmor._Equipped) {
				totalArmorRating += monster._MonsterHeadArmor._ArmorRating;
			}
			if (monster._MonsterLegArmor != null && monster._MonsterLegArmor._Equipped) {
				totalArmorRating += monster._MonsterLegArmor._ArmorRating;
			}
			return totalArmorRating;
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index) {
			if (opponent._Spellbook[index]._DamageGroup == MonsterSpell.DamageType.Physical) {
				return opponent._Spellbook[index]._Offensive._Amount;
			}
			double damageReductionPercentage = opponent._Spellbook[index]._DamageGroup switch {
				MonsterSpell.DamageType.Fire => player._FireResistance / 100.0,
				MonsterSpell.DamageType.Frost => player._FrostResistance / 100.0,
				MonsterSpell.DamageType.Arcane => player._ArcaneResistance / 100.0,
				_ => 0.0
			};
			return (int)(opponent._Spellbook[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
	}
}