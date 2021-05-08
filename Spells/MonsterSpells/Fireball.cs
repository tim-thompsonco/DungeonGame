using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Spells.MonsterSpells {
	public class Fireball : IMonsterOffensiveSpell, IMonsterOffensiveOverTimeSpell {
		public string Name { get; set; }
		public int _ManaCost { get; }
		public int _DamageAmount { get; }
		public int _DamageOverTimeAmount { get; }
		public int _MaxDamageRounds { get; }

		public Fireball(int monsterLevel) {
			Name = GetType().Name.ToLower();
			_ManaCost = 50;
			_DamageAmount = GetDamageAmount(monsterLevel);
			_DamageOverTimeAmount = GetDamageOverTimeAmount(monsterLevel);
			_MaxDamageRounds = 3;
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

			int spellDamage = CombatController.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, _DamageAmount);

			if (spellDamage > 0) {
				HitPlayerWithFireball(monster, player, spellDamage);
			}
		}

		private void DeductManaCost(Monster monster) {
			monster.EnergyPoints -= _ManaCost;
		}

		private void DisplaySpellAttackMessage(Monster monster) {
			string attackString;
			if (monster.MonsterCategory == Monster.MonsterType.Dragon) {
				attackString = $"The {monster.Name} breathes a pillar of fire at you!";
			} else {
				attackString = $"The {monster.Name} casts a fireball and launches it at you!";
			}

			OutputController.Display.StoreUserOutput(
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

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}

		public void AddDamageOverTimeEffect(Player player) {
			player.Effects.Add(new BurningEffect(Name, _MaxDamageRounds, _DamageOverTimeAmount));
		}

		public void DisplayDamageOverTimeMessage() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				"You burst into flame!");
		}
	}
}
