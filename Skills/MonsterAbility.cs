using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;
using System.Linq;

namespace DungeonGame {
	public class MonsterAbility {
		public string Name { get; set; }
		public DamageType? DamageGroup { get; set; }
		public Ability AbilityCategory { get; set; }
		public Offensive Offensive { get; set; }
		public int EnergyCost { get; set; }

		public MonsterAbility(string name, int energyCost, Ability abilityCategory, int monsterLevel) {
			Name = name;
			EnergyCost = energyCost;
			DamageGroup = DamageType.Physical;
			AbilityCategory = abilityCategory;
			Offensive = AbilityCategory switch {
				Ability.PoisonBite => new Offensive(15 + ((monsterLevel - 1) * 5),
					5 + ((monsterLevel - 1) * 2), 1, 3,
					OffensiveType.Bleed),
				Ability.BloodLeech => new Offensive(10 + ((monsterLevel - 1) * 5)),
				Ability.TailWhip => new Offensive(10 + ((monsterLevel - 1) * 5)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public void UseBloodLeechAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index].EnergyCost;

			string attackString = $"The {monster.Name} tries to sink its fangs into you and suck your blood!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int leechAmount = monster.Abilities[index].Offensive.Amount;
			leechAmount = AdjustAbilityDamageFromPlayerEffects(player, monster, leechAmount);

			if (leechAmount == 0) {
				return;
			}

			player.HitPoints -= leechAmount;

			if (monster.HitPoints + leechAmount > monster.MaxHitPoints) {
				monster.HitPoints = monster.MaxHitPoints;
			} else {
				monster.HitPoints += leechAmount;
			}

			string attackSuccessString = $"The {monster.Name} leeches {leechAmount} life from you.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}

		private int AdjustAbilityDamageFromPlayerEffects(Player player, Monster monster, int abilityDamage) {
			foreach (IEffect effect in player.Effects.Where(effect => effect.IsEffectExpired is false).ToList()) {
				if (effect is FrozenEffect frozenEffect) {
					frozenEffect.ProcessRound();
				}

				if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
					abilityDamage = changeMonsterDmgEffect.GetUpdatedDamageFromChange(abilityDamage);

					changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
				}

				if (effect is IChangeDamageEffect changeDamageEffect) {
					int baseAbilityDamage = abilityDamage;

					abilityDamage = changeDamageEffect.GetChangedDamageFromEffect(abilityDamage);

					changeDamageEffect.ProcessChangeDamageRound(baseAbilityDamage);
				}

				if (abilityDamage <= 0) {
					string effectAbsorbString = $"Your {effect.Name} absorbed all of {monster.Name}'s attack!";
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);

					return 0;
				}
			}

			return abilityDamage;
		}

		public void UseOffenseDamageAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index].EnergyCost;

			string attackString = monster.MonsterCategory == MonsterType.Spider
				? $"The {monster.Name} tries to bite you!"
				: $"The {monster.Name} swings its tail at you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int attackDamage = monster.Abilities[index].Offensive.Amount;
			attackDamage = AdjustAbilityDamageFromPlayerEffects(player, monster, attackDamage);

			string attackSuccessString = monster.MonsterCategory == MonsterType.Spider
				? $"The {monster.Name} bites you for {attackDamage} physical damage."
				: $"The {monster.Name} strikes you with its tail for {attackDamage} physical damage.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			player.HitPoints -= attackDamage;

			if (monster.Abilities[index].Offensive.AmountOverTime <= 0) {
				return;
			}

			if (monster.Abilities[index].Offensive.OffensiveGroup is OffensiveType.Bleed) {
				string bleedString = $"You are bleeding from {monster.Name}'s attack!";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					bleedString);

				EffectOverTimeSettings effectSettings = new EffectOverTimeSettings {
					AmountOverTime = monster.Abilities[index].Offensive.AmountOverTime,
					EffectHolder = player,
					Name = monster.Abilities[index].Name,
					MaxRound = monster.Abilities[index].Offensive.AmountMaxRounds
				};
				player.Effects.Add(new BleedingEffect(effectSettings));
			}
		}
	}
}