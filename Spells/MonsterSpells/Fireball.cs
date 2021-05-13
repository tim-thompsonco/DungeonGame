using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Spells.MonsterSpells {
	public class Fireball : IMonsterOffensiveSpell, IMonsterOffensiveOverTimeSpell {
		public string Name { get; set; }
		public int ManaCost { get; }
		public int DamageAmount { get; }
		public int DamageOverTimeAmount { get; }
		public int MaxDamageRounds { get; }

		public Fireball(int monsterLevel) {
			Name = GetType().Name.ToLower();
			ManaCost = 50;
			DamageAmount = GetDamageAmount(monsterLevel);
			DamageOverTimeAmount = GetDamageOverTimeAmount(monsterLevel);
			MaxDamageRounds = 3;
		}

		private int GetDamageAmount(int monsterLevel) {
			int damageAmt = 25;

			damageAmt += (monsterLevel - 1) * 5;

			return damageAmt;
		}

		private int GetDamageOverTimeAmount(int monsterLevel) {
			int damageOverTimeAmt = 5;

			damageOverTimeAmt += (monsterLevel - 1) * 2;

			return damageOverTimeAmt;
		}

		public void CastSpell(Monster monster, Player player) {
			DeductManaCost(monster);

			DisplaySpellAttackMessage(monster);

			int spellDamage = CombatHelper.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, DamageAmount);

			if (spellDamage > 0) {
				HitPlayerWithFireball(monster, player, spellDamage);
			}
		}

		private void DeductManaCost(Monster monster) {
			monster.EnergyPoints -= ManaCost;
		}

		private void DisplaySpellAttackMessage(Monster monster) {
			string attackString = monster.MonsterCategory == MonsterType.Dragon
				? $"The {monster.Name} breathes a pillar of fire at you!"
				: $"The {monster.Name} casts a fireball and launches it at you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
		}

		private void HitPlayerWithFireball(Monster monster, Player player, int spellDamage) {
			DeductSpellDamageFromPlayerHealth(player, spellDamage);

			DisplaySuccessfulAttackMessage(monster, spellDamage);

			AddDamageOverTimeEffect(player);

			DisplayDamageOverTimeMessage();
		}

		private void DeductSpellDamageFromPlayerHealth(Player player, int spellDamage) {
			player.HitPoints -= spellDamage;
		}

		public void DisplaySuccessfulAttackMessage(Monster monster, int spellDamage) {
			string attackSuccessString = $"The {monster.Name} hits you for {spellDamage} fire damage.";

			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}

		public void AddDamageOverTimeEffect(Player player) {
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = DamageOverTimeAmount,
				EffectHolder = player,
				MaxRound = MaxDamageRounds,
				Name = Name
			};
			player.Effects.Add(new BurningEffect(effectOverTimeSettings));
		}

		public void DisplayDamageOverTimeMessage() {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				"You burst into flame!");
		}
	}
}
