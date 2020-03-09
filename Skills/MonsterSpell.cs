using System;

namespace DungeonGame {
	public class MonsterSpell {
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning,
			Heal
		}
		public string Name { get; set; }
		public SpellType SpellCategory { get; set; }
		public Offensive Offensive { get; set; }
		public Healing Healing { get; set; }
		public int EnergyCost { get; set; }

		public MonsterSpell(string name, int energyCost, SpellType spellType, int monsterLevel) {
			this.Name = name;
			this.EnergyCost = energyCost;
			this.SpellCategory = spellType;
			switch(this.SpellCategory) {
				case SpellType.Fireball:
					this.Offensive = new Offensive(
						25 + (monsterLevel - 1) * 10, 5 + (monsterLevel - 1) * 5, 
						1, 3, Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					this.Offensive = new Offensive(15 + (monsterLevel - 1) * 10, 1, 2);
					break;
				case SpellType.Lightning:
					this.Offensive = new Offensive(35 + (monsterLevel - 1) * 10);
					break;
				case SpellType.Heal:
					this.Healing = new Healing(50 + (monsterLevel - 1) * 10);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public static void CastFireOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;
			var fireSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = fireSpellDamage * effect.EffectMultiplier;
						fireSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var attackString = string.Empty;
			if (monster.MonsterCategory == Monster.MonsterType.Dragon) {
				attackString = "The " + monster.Name + " breathes a pillar of fire at you!";
			}
			else {
				attackString = "The " + monster.Name + " casts a fireball and launches it at you!";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var attackSuccessString = "The " + monster.Name + " hits you for " + fireSpellDamage + " fire damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			player.HitPoints -= fireSpellDamage;
			if (monster.Spellbook[index].Offensive.AmountOverTime <= 0) return;
			const string onFireString = "You burst into flame!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				onFireString);
			player.Effects.Add(new Effect(monster.Spellbook[index].Name,
				Effect.EffectType.OnFire, monster.Spellbook[index].Offensive.AmountOverTime,
				monster.Spellbook[index].Offensive.AmountCurRounds, monster.Spellbook[index].Offensive.AmountMaxRounds, 
				1, 1, true));
		}
		public static void CastFrostOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;
			var frostSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = frostSpellDamage * effect.EffectMultiplier;
						frostSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var attackString = "The " + monster.Name + " casts a frostbolt and launches it at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var attackSuccessString = "The " + monster.Name + " hits you for " + frostSpellDamage + " frost damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			var frozenEffectIndex = player.Effects.FindIndex(
				e => e.EffectGroup == Effect.EffectType.Frozen);
			if (frozenEffectIndex == -1) {
				const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					frozenString);
			}
			player.HitPoints -= frostSpellDamage;
			player.Effects.Add(new Effect(monster.Spellbook[index].Name,Effect.EffectType.Frozen, 
				monster.Spellbook[index].Offensive.AmountCurRounds, monster.Spellbook[index].Offensive.AmountMaxRounds, 
				1.5, 1, true));
		}
		public static void CastArcaneOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;
			var arcaneSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = arcaneSpellDamage * effect.EffectMultiplier;
						arcaneSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var attackString = "The " + monster.Name + " casts a bolt of lightning at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var attackSuccessString = "The " + monster.Name + " hits you for " + arcaneSpellDamage + " arcane damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			player.HitPoints -= arcaneSpellDamage;
		}
		public static void CastHealing(Monster monster, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;
			var healAmount = monster.Spellbook[index].Healing.HealAmount;
			var healString = "The " + monster.Name + " heals itself for " + healAmount + " health.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				healString);
			monster.HitPoints += healAmount;
			if (monster.HitPoints > monster.MaxHitPoints) monster.HitPoints = monster.MaxHitPoints;
			if (monster.Spellbook[index].Healing.HealOverTime <= 0) return;
			monster.Effects.Add(new Effect(monster.Spellbook[index].Name,
				Effect.EffectType.Healing, monster.Spellbook[index].Healing.HealOverTime,
				monster.Spellbook[index].Healing.HealCurRounds, monster.Spellbook[index].Healing.HealMaxRounds,
				1, 10, false));
		}
	}
}