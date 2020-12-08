using System;
using System.Linq;

namespace DungeonGame
{
	public class MonsterSpell
	{
		public enum DamageType
		{
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum SpellType
		{
			Fireball,
			Frostbolt,
			Lightning
		}
		public string _Name { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public SpellType _SpellCategory { get; set; }
		public Offensive _Offensive { get; set; }
		public int _EnergyCost { get; set; }

		// Default constructor for JSON serialization
		public MonsterSpell() { }
		public MonsterSpell(string name, int energyCost, SpellType spellType, int monsterLevel)
		{
			_Name = name;
			_EnergyCost = energyCost;
			_SpellCategory = spellType;
			switch (_SpellCategory)
			{
				case SpellType.Fireball:
					_DamageGroup = DamageType.Fire;
					_Offensive = new Offensive(25 + ((monsterLevel - 1) * 5),
						5 + (monsterLevel - 1) * 2, 1, 3,
						Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					_DamageGroup = DamageType.Frost;
					_Offensive = new Offensive(15 + ((monsterLevel - 1) * 5),
						1, 2);
					break;
				case SpellType.Lightning:
					_DamageGroup = DamageType.Arcane;
					_Offensive = new Offensive(35 + ((monsterLevel - 1) * 5));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void CastFireOffense(Monster monster, Player player, int index)
		{
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;
			string attackString;
			if (monster._MonsterCategory == Monster.MonsterType.Dragon)
			{
				attackString = $"The {monster._Name} breathes a pillar of fire at you!";
			}
			else
			{
				attackString = $"The {monster._Name} casts a fireball and launches it at you!";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			int fireSpellDamage = MonsterHandler.CalculateSpellDamage(player, monster, index);
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
						double frozenAttackAmount = fireSpellDamage * effect._EffectMultiplier;
						fireSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect._IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						int changeDamageAmount = effect._EffectAmountOverTime < fireSpellDamage ?
							effect._EffectAmountOverTime : fireSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						fireSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						int blockAmount = effect._EffectAmount < fireSpellDamage ?
							effect._EffectAmount : fireSpellDamage;
						effect.BlockDamageRound(blockAmount);
						fireSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						int reflectAmount = effect._EffectAmountOverTime < fireSpellDamage ?
							effect._EffectAmountOverTime : fireSpellDamage;
						monster._HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						fireSpellDamage -= reflectAmount;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (fireSpellDamage > 0)
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
			player._HitPoints -= fireSpellDamage;
			string attackSuccessString = $"The {monster._Name} hits you for {fireSpellDamage} fire damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			if (monster._Spellbook[index]._Offensive._AmountOverTime <= 0)
			{
				return;
			}

			const string onFireString = "You burst into flame!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				onFireString);
			player._Effects.Add(new Effect(monster._Spellbook[index]._Name,
				Effect.EffectType.OnFire, monster._Spellbook[index]._Offensive._AmountOverTime,
				monster._Spellbook[index]._Offensive._AmountCurRounds, monster._Spellbook[index]._Offensive._AmountMaxRounds,
				1, 1, true));
		}
		public static void CastFrostOffense(Monster monster, Player player, int index)
		{
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;
			string attackString = $"The {monster._Name} conjures up a frostbolt and launches it at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			int frostSpellDamage = MonsterHandler.CalculateSpellDamage(player, monster, index);
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
						double frozenAttackAmount = frostSpellDamage * effect._EffectMultiplier;
						frostSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect._IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						int changeDamageAmount = effect._EffectAmountOverTime < frostSpellDamage ?
							effect._EffectAmountOverTime : frostSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						frostSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						int blockAmount = effect._EffectAmount < frostSpellDamage ?
							effect._EffectAmount : frostSpellDamage;
						effect.BlockDamageRound(blockAmount);
						frostSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						int reflectAmount = effect._EffectAmountOverTime < frostSpellDamage ?
							effect._EffectAmountOverTime : frostSpellDamage;
						monster._HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						frostSpellDamage -= reflectAmount;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (frostSpellDamage > 0)
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
			player._HitPoints -= frostSpellDamage;
			string attackSuccessString = $"The {monster._Name} hits you for {frostSpellDamage} frost damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			int frozenEffectIndex = player._Effects.FindIndex(
				e => e._EffectGroup == Effect.EffectType.Frozen);
			if (frozenEffectIndex == -1)
			{
				const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					frozenString);
			}
			player._Effects.Add(new Effect(monster._Spellbook[index]._Name, Effect.EffectType.Frozen,
				monster._Spellbook[index]._Offensive._AmountCurRounds, monster._Spellbook[index]._Offensive._AmountMaxRounds,
				1.5, 1, true));
		}
		public static void CastArcaneOffense(Monster monster, Player player, int index)
		{
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;
			string attackString = $"The {monster._Name} casts a bolt of lightning at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			int arcaneSpellDamage = MonsterHandler.CalculateSpellDamage(player, monster, index);
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
						double frozenAttackAmount = arcaneSpellDamage * effect._EffectMultiplier;
						arcaneSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect._IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						int changeDamageAmount = effect._EffectAmountOverTime < arcaneSpellDamage ?
							effect._EffectAmountOverTime : arcaneSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						arcaneSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						int blockAmount = effect._EffectAmount < arcaneSpellDamage ?
							effect._EffectAmount : arcaneSpellDamage;
						effect.BlockDamageRound(blockAmount);
						arcaneSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						int reflectAmount = effect._EffectAmountOverTime < arcaneSpellDamage ?
							effect._EffectAmountOverTime : arcaneSpellDamage;
						monster._HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						arcaneSpellDamage -= reflectAmount;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (arcaneSpellDamage > 0)
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
			player._HitPoints -= arcaneSpellDamage;
			string attackSuccessString = $"The {monster._Name} hits you for {arcaneSpellDamage} arcane damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
	}
}