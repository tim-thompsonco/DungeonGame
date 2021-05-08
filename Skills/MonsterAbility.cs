using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;
using System.Linq;

namespace DungeonGame {
	public class MonsterAbility {
		public enum DamageType {
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum Ability {
			PoisonBite,
			BloodLeech,
			TailWhip
		}
		public string _Name { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public Ability _AbilityCategory { get; set; }
		public Offensive _Offensive { get; set; }
		public int _EnergyCost { get; set; }

		public MonsterAbility(string name, int energyCost, Ability abilityCategory, int monsterLevel) {
			_Name = name;
			_EnergyCost = energyCost;
			_DamageGroup = DamageType.Physical;
			_AbilityCategory = abilityCategory;
			_Offensive = _AbilityCategory switch {
				Ability.PoisonBite => new Offensive(15 + ((monsterLevel - 1) * 5),
					5 + ((monsterLevel - 1) * 2), 1, 3,
					Offensive.OffensiveType.Bleed),
				Ability.BloodLeech => new Offensive(10 + ((monsterLevel - 1) * 5)),
				Ability.TailWhip => new Offensive(10 + ((monsterLevel - 1) * 5)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public void UseBloodLeechAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index]._EnergyCost;

			string attackString = $"The {monster.Name} tries to sink its fangs into you and suck your blood!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int leechAmount = monster.Abilities[index]._Offensive._Amount;
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
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}

		private int AdjustAbilityDamageFromPlayerEffects(Player player, Monster monster, int abilityDamage) {
			foreach (IEffect effect in player.Effects.ToList()) {
				if (effect is FrozenEffect frozenEffect) {
					frozenEffect.ProcessFrozenRound();
				}

				if (effect is ChangeMonsterDamageEffect changeMonsterDmgEffect) {
					abilityDamage = changeMonsterDmgEffect.GetUpdatedDamageFromChange(abilityDamage);

					changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
				}

				if (effect is BlockDamageEffect blockDmgEffect) {
					int baseAbilityDamage = abilityDamage;

					abilityDamage = blockDmgEffect.GetDecreasedDamageFromBlock(abilityDamage);

					blockDmgEffect.ProcessBlockDamageRound(baseAbilityDamage);
				}

				if (effect is ReflectDamageEffect reflectDmgEffect) {
					int baseSpellDamage = abilityDamage;

					abilityDamage = reflectDmgEffect.GetReflectedDamageAmount(abilityDamage);

					reflectDmgEffect.ProcessReflectDamageRound(baseSpellDamage);
				}

				if (abilityDamage <= 0) {
					string effectAbsorbString = $"Your {effect.Name} absorbed all of {monster.Name}'s attack!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);

					return 0;
				}
			}

			return abilityDamage;
		}

		public void UseOffenseDamageAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index]._EnergyCost;

			string attackString;
			if (monster.MonsterCategory == Monster.MonsterType.Spider) {
				attackString = $"The {monster.Name} tries to bite you!";
			} else {
				attackString = $"The {monster.Name} swings its tail at you!";
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int attackDamage = monster.Abilities[index]._Offensive._Amount;
			attackDamage = AdjustAbilityDamageFromPlayerEffects(player, monster, attackDamage);

			string attackSuccessString;
			if (monster.MonsterCategory == Monster.MonsterType.Spider) {
				attackSuccessString = $"The {monster.Name} bites you for {attackDamage} physical damage.";
			} else {
				attackSuccessString = $"The {monster.Name} strikes you with its tail for {attackDamage} physical damage.";
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			player.HitPoints -= attackDamage;

			if (monster.Abilities[index]._Offensive._AmountOverTime <= 0) {
				return;
			}

			if (monster.Abilities[index]._Offensive._OffensiveGroup is Offensive.OffensiveType.Bleed) {
				string bleedString = $"You are bleeding from {monster.Name}'s attack!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					bleedString);

				player.Effects.Add(new BleedingEffect(monster.Abilities[index]._Name,
					monster.Abilities[index]._Offensive._AmountMaxRounds, monster.Abilities[index]._Offensive._AmountOverTime));
			}
		}
	}
}