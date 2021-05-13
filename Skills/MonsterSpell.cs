using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;

namespace DungeonGame {
	public class MonsterSpell {
		public string Name { get; set; }
		public DamageType? DamageGroup { get; set; }
		public SpellType SpellCategory { get; set; }
		public Offensive Offensive { get; set; }
		public int EnergyCost { get; set; }

		public MonsterSpell(string name, int energyCost, SpellType spellType, int monsterLevel) {
			Name = name;
			EnergyCost = energyCost;
			SpellCategory = spellType;
			switch (SpellCategory) {
				case SpellType.Fireball:
					DamageGroup = DamageType.Fire;
					Offensive = new Offensive(25 + ((monsterLevel - 1) * 5),
						5 + ((monsterLevel - 1) * 2), 1, 3, OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					DamageGroup = DamageType.Frost;
					Offensive = new Offensive(15 + ((monsterLevel - 1) * 5),
						1, 2);
					break;
				case SpellType.Lightning:
					DamageGroup = DamageType.Arcane;
					Offensive = new Offensive(35 + ((monsterLevel - 1) * 5));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void CastFireOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;

			string attackString = monster.MonsterCategory == MonsterType.Dragon
				? $"The {monster.Name} breathes a pillar of fire at you!"
				: $"The {monster.Name} casts a fireball and launches it at you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int fireSpellDamage = MonsterHelper.CalculateSpellDamage(player, monster, index);
			fireSpellDamage = CombatHelper.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, fireSpellDamage);

			if (fireSpellDamage == 0) {
				return;
			}

			player.HitPoints -= fireSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {fireSpellDamage} fire damage.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			if (monster.Spellbook[index].Offensive.AmountOverTime > 0) {
				const string onFireString = "You burst into flame!";
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatOnFireText(),
					Settings.FormatDefaultBackground(),
					onFireString);

				EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
					AmountOverTime = monster.Spellbook[index].Offensive.AmountOverTime,
					EffectHolder = player,
					MaxRound = monster.Spellbook[index].Offensive.AmountMaxRounds,
					Name = monster.Spellbook[index].Name
				};
				player.Effects.Add(new BurningEffect(effectOverTimeSettings));
			}
		}

		public void CastFrostOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;

			string attackString = $"The {monster.Name} conjures up a frostbolt and launches it at you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int frostSpellDamage = MonsterHelper.CalculateSpellDamage(player, monster, index);
			frostSpellDamage = CombatHelper.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, frostSpellDamage);

			if (frostSpellDamage == 0) {
				return;
			}

			player.HitPoints -= frostSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {frostSpellDamage} frost damage.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			player.Effects.Add(new FrozenEffect(monster.Spellbook[index].Name, monster.Spellbook[index].Offensive.AmountMaxRounds));

			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be increased by 50%!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);
		}

		public void CastArcaneOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;

			string attackString = $"The {monster.Name} casts a bolt of lightning at you!";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);

			int arcaneSpellDamage = MonsterHelper.CalculateSpellDamage(player, monster, index);
			arcaneSpellDamage = CombatHelper.GetMonsterAttackDamageUpdatedFromPlayerEffects(player, monster, arcaneSpellDamage);

			if (arcaneSpellDamage == 0) {
				return;
			}

			player.HitPoints -= arcaneSpellDamage;

			string attackSuccessString = $"The {monster.Name} hits you for {arcaneSpellDamage} arcane damage.";
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
	}
}