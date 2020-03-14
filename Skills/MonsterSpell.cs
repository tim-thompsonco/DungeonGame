using System;
using System.Linq;

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

		// Default constructor for JSON serialization
		public MonsterSpell() { }
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
			var fireSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects.ToList()) {
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
						var changeDamageAmount = effect.EffectAmountOverTime < fireSpellDamage ? 
							effect.EffectAmountOverTime : fireSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						fireSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < fireSpellDamage ?
							effect.EffectAmount : fireSpellDamage;
						effect.BlockDamageRound(blockAmount);
						fireSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < fireSpellDamage ? 
							effect.EffectAmountOverTime : fireSpellDamage;
						monster.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (fireSpellDamage > 0) continue;
				var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + monster.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			player.HitPoints -= fireSpellDamage;
			var attackSuccessString = "The " + monster.Name + " hits you for " + fireSpellDamage + " fire damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
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
			var attackString = "The " + monster.Name + " conjures up a frostbolt and launches it at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var frostSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects.ToList()) {
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
						var changeDamageAmount = effect.EffectAmountOverTime < frostSpellDamage ? 
							effect.EffectAmountOverTime : frostSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						frostSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < frostSpellDamage ?
							effect.EffectAmount : frostSpellDamage;
						effect.BlockDamageRound(blockAmount);
						frostSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < frostSpellDamage ? 
							effect.EffectAmountOverTime : frostSpellDamage;
						monster.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (frostSpellDamage > 0) continue;
				var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + monster.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			player.HitPoints -= frostSpellDamage;
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
			player.Effects.Add(new Effect(monster.Spellbook[index].Name,Effect.EffectType.Frozen, 
				monster.Spellbook[index].Offensive.AmountCurRounds, monster.Spellbook[index].Offensive.AmountMaxRounds, 
				1.5, 1, true));
		}
		public static void CastArcaneOffense(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Spellbook[index].EnergyCost;
			var attackString = "The " + monster.Name + " casts a bolt of lightning at you!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var arcaneSpellDamage = monster.Spellbook[index].Offensive.Amount;
			foreach (var effect in player.Effects.ToList()) {
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
						var changeDamageAmount = effect.EffectAmountOverTime < arcaneSpellDamage ? 
							effect.EffectAmountOverTime : arcaneSpellDamage;
						effect.ChangeOpponentDamageRound(player);
						arcaneSpellDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < arcaneSpellDamage ?
							effect.EffectAmount : arcaneSpellDamage;
						effect.BlockDamageRound(blockAmount);
						arcaneSpellDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < arcaneSpellDamage ? 
							effect.EffectAmountOverTime : arcaneSpellDamage;
						monster.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (arcaneSpellDamage > 0) continue;
				var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + monster.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			player.HitPoints -= arcaneSpellDamage;
			var attackSuccessString = "The " + monster.Name + " hits you for " + arcaneSpellDamage + " arcane damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
	}
}