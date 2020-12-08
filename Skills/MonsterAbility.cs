using System;
using System.Linq;

namespace DungeonGame
{
	public class MonsterAbility
	{
		public enum DamageType
		{
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum Ability
		{
			PoisonBite,
			BloodLeech,
			TailWhip
		}
		public string _Name { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public Ability _AbilityCategory { get; set; }
		public Offensive _Offensive { get; set; }
		public int _EnergyCost { get; set; }

		// Default constructor for JSON serialization
		public MonsterAbility() { }
		public MonsterAbility(string name, int energyCost, Ability abilityCategory, int monsterLevel)
		{
			_Name = name;
			_EnergyCost = energyCost;
			_DamageGroup = DamageType.Physical;
			_AbilityCategory = abilityCategory;
			_Offensive = _AbilityCategory switch
			{
				Ability.PoisonBite => new Offensive(15 + ((monsterLevel - 1) * 5),
					5 + ((monsterLevel - 1) * 2), 1, 3,
					Offensive.OffensiveType.Bleed),
				Ability.BloodLeech => new Offensive(10 + ((monsterLevel - 1) * 5)),
				Ability.TailWhip => new Offensive(10 + ((monsterLevel - 1) * 5)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public static void UseBloodLeechAbility(Monster monster, Player player, int index)
		{
			monster._EnergyPoints -= monster._Abilities[index]._EnergyCost;
			string attackString = $"The {monster._Name} tries to sink its fangs into you and suck your blood!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			int leechAmount = monster._Abilities[index]._Offensive._Amount;
			foreach (Effect effect in player._Effects.ToList())
			{
				switch (effect._EffectGroup)
				{
					case Effect.EffectType.Healing:
					case Effect.EffectType.ChangePlayerDamage:
					case Effect.EffectType.ChangeArmor:
					case Effect.EffectType.OnFire:
					case Effect.EffectType.Bleeding:
					case Effect.EffectType.Stunned:
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.Frozen:
						double frozenAttackAmount = leechAmount * effect._EffectMultiplier;
						leechAmount = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect._IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						int changeDamageAmount = effect._EffectAmountOverTime < leechAmount ?
							effect._EffectAmountOverTime : leechAmount;
						effect.ChangeOpponentDamageRound(player);
						leechAmount += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						int blockAmount = effect._EffectAmount < leechAmount ?
							effect._EffectAmount : leechAmount;
						effect.BlockDamageRound(blockAmount);
						leechAmount -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						int reflectAmount = effect._EffectAmountOverTime < leechAmount ?
							effect._EffectAmountOverTime : leechAmount;
						monster._HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						leechAmount -= reflectAmount;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (leechAmount > 0)
				{
					continue;
				}

				string effectAbsorbString = $"Your {effect._Name} absorbed all of {monster._Name}'s attack!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			player._HitPoints -= leechAmount;
			monster._HitPoints += leechAmount;
			if (monster._HitPoints > monster._MaxHitPoints)
			{
				monster._HitPoints = monster._MaxHitPoints;
			}

			string attackSuccessString = $"The {monster._Name} leeches {leechAmount} life from you.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
		public static void UseOffenseDamageAbility(Monster monster, Player player, int index)
		{
			monster._EnergyPoints -= monster._Abilities[index]._EnergyCost;
			string attackString;
			if (monster._MonsterCategory == Monster.MonsterType.Spider)
			{
				attackString = $"The {monster._Name} tries to bite you!";
			}
			else
			{
				attackString = $"The {monster._Name} swings its tail at you!";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			int attackDamage = monster._Abilities[index]._Offensive._Amount;
			foreach (Effect effect in player._Effects.ToList())
			{
				switch (effect._EffectGroup)
				{
					case Effect.EffectType.Healing:
					case Effect.EffectType.ChangePlayerDamage:
					case Effect.EffectType.ChangeArmor:
					case Effect.EffectType.OnFire:
					case Effect.EffectType.Bleeding:
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						double frozenAttackAmount = attackDamage * effect._EffectMultiplier;
						attackDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect._IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						int changeDamageAmount = effect._EffectAmountOverTime < attackDamage ?
							effect._EffectAmountOverTime : attackDamage;
						effect.ChangeOpponentDamageRound(player);
						attackDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						int blockAmount = effect._EffectAmount < attackDamage ?
							effect._EffectAmount : attackDamage;
						effect.BlockDamageRound(blockAmount);
						attackDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						int reflectAmount = effect._EffectAmountOverTime < attackDamage ?
							effect._EffectAmountOverTime : attackDamage;
						monster._HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						attackDamage -= reflectAmount;
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (attackDamage > 0)
				{
					continue;
				}

				string effectAbsorbString = $"Your {effect._Name} absorbed all of {monster._Name}'s attack!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			string attackSuccessString;
			if (monster._MonsterCategory == Monster.MonsterType.Spider)
			{
				attackSuccessString = $"The {monster._Name} bites you for {attackDamage} physical damage.";
			}
			else
			{
				attackSuccessString = $"The {monster._Name} strikes you with its tail for {attackDamage} physical damage.";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			player._HitPoints -= attackDamage;
			if (monster._Abilities[index]._Offensive._AmountOverTime <= 0)
			{
				return;
			}

			switch (monster._Abilities[index]._Offensive._OffensiveGroup)
			{
				case Offensive.OffensiveType.Normal:
				case Offensive.OffensiveType.Fire:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedString = $"You are bleeding from {monster._Name}'s attack!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);
					player._Effects.Add(new Effect(monster._Abilities[index]._Name,
						Effect.EffectType.Bleeding, monster._Abilities[index]._Offensive._AmountOverTime,
						monster._Abilities[index]._Offensive._AmountCurRounds, monster._Abilities[index]._Offensive._AmountMaxRounds,
						1, 1, true));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}