//using System.Linq;
//using DungeonGame.Controllers;
//using DungeonGame.Effects;
//using DungeonGame.Monsters;
//using DungeonGame.Players;

//namespace DungeonGame.Spells.MonsterSpells {
//	public class Fireball : IMonsterOffensiveSpell, IMonsterOffensiveOverTimeSpell {
//		public string _Name { get; set; }
//		public int _ManaCost { get; }
//		public int _DamageAmount { get; }
//		public int _DamageOverTimeAmount { get; }
//		public int _MaxDamageRounds { get; }

//		public Fireball(int monsterLevel) {
//			_Name = GetType().Name.ToLower();
//			_ManaCost = 50;
//			_DamageAmount = GetDamageAmount(monsterLevel);
//			_DamageOverTimeAmount = GetDamageOverTimeAmount(monsterLevel);
//			_MaxDamageRounds = 3;
//		}

//		private int GetDamageAmount(int monsterLevel) {
//			int damageAmt = 25;

//			damageAmt += (monsterLevel - 1) * 5;

//			return damageAmt;
//		}

//		private int GetDamageOverTimeAmount(int monsterLevel) {
//			int damageOverTimeAmt = 5;

//			damageOverTimeAmt += (monsterLevel - 1) * 2;

//			return damageOverTimeAmt;
//		}

//		public void CastSpell(Monster monster, Player player) {
//			DeductManaCost(monster);

//			DisplaySpellAttackMessage(monster);

//			int spellDamage = CalculateSpellDamage(monster, player);

//			if (spellDamage > 0) {
//				HitPlayerWithFireball(monster, player, spellDamage);
//			}
//		}

//		private void DeductManaCost(Monster monster) {
//			monster._EnergyPoints -= _ManaCost;
//		}

//		private void DisplaySpellAttackMessage(Monster monster) {
//			string attackString;
//			if (monster._MonsterCategory == Monster.MonsterType.Dragon) {
//				attackString = $"The {monster._Name} breathes a pillar of fire at you!";
//			} else {
//				attackString = $"The {monster._Name} casts a fireball and launches it at you!";
//			}

//			OutputController.Display.StoreUserOutput(
//				Settings.FormatAttackSuccessText(),
//				Settings.FormatDefaultBackground(),
//				attackString);
//		}

//		public int CalculateSpellDamage(Monster monster, Player player) {
//			int baseSpellDamage = _DamageAmount;

//			int spellDamage = DecreaseSpellDamageFromPlayerResistance(player, baseSpellDamage);

//			foreach (IEffect effect in player._Effects.ToList()) {
//				if (effect is FrozenEffect frozenEffect) {
//					spellDamage = frozenEffect.GetIncreasedDamageFromFrozen(spellDamage);
//					frozenEffect.FrozenRound();
//				}

//				if (effect._EffectGroup == Effect.EffectType.ChangeOpponentDamage) {
//					spellDamage = IncreaseSpellDamageFromChangeEffect(player, effect, spellDamage);
//				}

//				if (effect._EffectGroup == Effect.EffectType.BlockDamage) {
//					spellDamage = DecreaseSpellDamageFromBlockEffect(effect, spellDamage);
//				}

//				if (effect._EffectGroup == Effect.EffectType.ReflectDamage) {
//					spellDamage = DecreaseSpellDamageFromReflectEffect(monster, effect, spellDamage);
//				}

//				if (spellDamage <= 0) {
//					DisplayEffectAbsorbedAllDamageMessage(monster, effect);

//					return 0;
//				} 
//			}

//			return spellDamage;
//		}

//		public int DecreaseSpellDamageFromPlayerResistance(Player player, int spellDamage) {
//			double damageReductionPercentage = player._FireResistance / 100.0;

//			int reducedSpellDamage = (int)(spellDamage * (1 - damageReductionPercentage));

//			return reducedSpellDamage;
//		}

//		public int IncreaseSpellDamageFromChangeEffect(Player player, Effect effect, int spellDamage) {
//			int changeDamageAmount = effect._EffectAmountOverTime < spellDamage ?
//				effect._EffectAmountOverTime : spellDamage;

//			effect.ChangeOpponentDamageRound(player);

//			spellDamage += changeDamageAmount;

//			return spellDamage;
//		}

//		public int DecreaseSpellDamageFromBlockEffect(Effect effect, int spellDamage) {
//			int blockAmount = effect._EffectAmount < spellDamage ?
//							effect._EffectAmount : spellDamage;

//			effect.BlockDamageRound(blockAmount);

//			spellDamage -= blockAmount;

//			return spellDamage;
//		}

//		public int DecreaseSpellDamageFromReflectEffect(Monster monster, Effect effect, int spellDamage) {
//			int reflectAmount = effect._EffectAmountOverTime < spellDamage ?
//							effect._EffectAmountOverTime : spellDamage;

//			monster._HitPoints -= reflectAmount;

//			effect.ReflectDamageRound(reflectAmount);

//			spellDamage -= reflectAmount;

//			return spellDamage;
//		}

//		public void DisplayEffectAbsorbedAllDamageMessage(Monster monster, Effect effect) {
//			string effectAbsorbString = $"Your {effect._Name} absorbed all of {monster._Name}'s attack!";

//			OutputController.Display.StoreUserOutput(
//				Settings.FormatAttackFailText(),
//				Settings.FormatDefaultBackground(),
//				effectAbsorbString);
//		}

//		private void HitPlayerWithFireball(Monster monster, Player player, int spellDamage) {
//			DeductSpellDamageFromPlayerHealth(player, spellDamage);

//			DisplaySuccessfulAttackMessage(monster, spellDamage);


//		}

//		private void DeductSpellDamageFromPlayerHealth(Player player, int spellDamage) {
//			player._HitPoints -= spellDamage;
//		}

//		public void DisplaySuccessfulAttackMessage(Monster monster, int spellDamage) {
//			string attackSuccessString = $"The {monster._Name} hits you for {spellDamage} fire damage.";

//			OutputController.Display.StoreUserOutput(
//				Settings.FormatAttackSuccessText(),
//				Settings.FormatDefaultBackground(),
//				attackSuccessString);
//		}

//		public void DisplayDamageOverTimeMessage() {
//			OutputController.Display.StoreUserOutput(
//				Settings.FormatOnFireText(),
//				Settings.FormatDefaultBackground(),
//				"You burst into flame!");
//		}

//		public void AddDamageOverTimeEffect(Player player) {
//			player._Effects.Add(
//				new Effect(_Name, Effect.EffectType.OnFire, _DamageOverTimeAmount, 1, _MaxDamageRounds,	1, 1, true));
//		}
//	}
//}
