using System.Collections.Generic;

namespace DungeonGame
{
	public static class MonsterHandler
	{
		public static void DisplayStats(Monster monster)
		{
			var opponentHealthString = "Opponent HP: " + monster._HitPoints + " / " + monster._MaxHitPoints;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				opponentHealthString);
			var healLineOutput = new List<string>();
			var hitPointMaxUnits = monster._MaxHitPoints / 10;
			var hitPointUnits = monster._HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++)
			{
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
		public static int CheckArmorRating(Monster monster)
		{
			var totalArmorRating = 0;
			if (monster._MonsterChestArmor != null && monster._MonsterChestArmor._Equipped)
			{
				totalArmorRating += monster._MonsterChestArmor._ArmorRating;
			}
			if (monster._MonsterHeadArmor != null && monster._MonsterHeadArmor._Equipped)
			{
				totalArmorRating += monster._MonsterHeadArmor._ArmorRating;
			}
			if (monster._MonsterLegArmor != null && monster._MonsterLegArmor._Equipped)
			{
				totalArmorRating += monster._MonsterLegArmor._ArmorRating;
			}
			return totalArmorRating;
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index)
		{
			if (opponent._Spellbook[index]._DamageGroup == MonsterSpell.DamageType.Physical)
			{
				return opponent._Spellbook[index]._Offensive._Amount;
			}
			var damageReductionPercentage = opponent._Spellbook[index]._DamageGroup switch
			{
				MonsterSpell.DamageType.Fire => (player._FireResistance / 100.0),
				MonsterSpell.DamageType.Frost => (player._FrostResistance / 100.0),
				MonsterSpell.DamageType.Arcane => (player._ArcaneResistance / 100.0),
				_ => 0.0
			};
			return (int)(opponent._Spellbook[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
	}
}