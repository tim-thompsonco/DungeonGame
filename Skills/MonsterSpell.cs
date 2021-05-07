using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;

namespace DungeonGame {
	public class MonsterSpell {
		public enum DamageType {
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning
		}
		public string _Name { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public SpellType _SpellCategory { get; set; }
		public Offensive _Offensive { get; set; }
		public int _EnergyCost { get; set; }

		public MonsterSpell(string name, int energyCost, SpellType spellType, int monsterLevel) {
			_Name = name;
			_EnergyCost = energyCost;
			_SpellCategory = spellType;
			switch (_SpellCategory) {
				case SpellType.Fireball:
					_DamageGroup = DamageType.Fire;
					_Offensive = new Offensive(25 + ((monsterLevel - 1) * 5),
						5 + ((monsterLevel - 1) * 2), 1, 3,
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

		public void CastFireOffense(Monster monster, Player player, int index) {
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;

			string attackString;
			if (monster._MonsterCategory == Monster.MonsterType.Dragon) {
				attackString = $"The {monster.Name} breathes a pillar of fire at you!";
			} else {
				attackString = $"The {monster.Name} casts a fireball and launches it at you!";
			}

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int fireSpellDamage = MonsterController.CalculateSpellDamage(player, monster, index);
			fireSpellDamage = CombatController.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, fireSpellDamage);

			if (fireSpellDamage == 0) {
				return;
			}

			player._HitPoints -= fireSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {fireSpellDamage} fire damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			if (monster._Spellbook[index]._Offensive._AmountOverTime > 0) {
				const string onFireString = "You burst into flame!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatOnFireText(),
					Settings.FormatDefaultBackground(),
					onFireString);
				player._Effects.Add(
					new BurningEffect(monster._Spellbook[index]._Name, monster._Spellbook[index]._Offensive._AmountMaxRounds,
						monster._Spellbook[index]._Offensive._AmountOverTime));
			}
		}

		public void CastFrostOffense(Monster monster, Player player, int index) {
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;

			string attackString = $"The {monster.Name} conjures up a frostbolt and launches it at you!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int frostSpellDamage = MonsterController.CalculateSpellDamage(player, monster, index);
			frostSpellDamage = CombatController.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, frostSpellDamage);

			if (frostSpellDamage == 0) {
				return;
			}

			player._HitPoints -= frostSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {frostSpellDamage} frost damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			player._Effects.Add(new FrozenEffect(monster._Spellbook[index]._Name, monster._Spellbook[index]._Offensive._AmountMaxRounds));

			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be increased by 50%!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);
		}

		public void CastArcaneOffense(Monster monster, Player player, int index) {
			monster._EnergyPoints -= monster._Spellbook[index]._EnergyCost;

			string attackString = $"The {monster.Name} casts a bolt of lightning at you!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int arcaneSpellDamage = MonsterController.CalculateSpellDamage(player, monster, index);
			arcaneSpellDamage = CombatController.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, arcaneSpellDamage);

			if (arcaneSpellDamage == 0) {
				return;
			}

			player._HitPoints -= arcaneSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {arcaneSpellDamage} arcane damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
	}
}