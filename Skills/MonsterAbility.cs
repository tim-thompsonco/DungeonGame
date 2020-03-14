using System;
using System.Linq;

namespace DungeonGame {
	public class MonsterAbility {
		public enum Ability {
			PoisonBite,
			BloodLeech,
			TailWhip
		}
		public string Name { get; set; }
		public Ability AbilityCategory { get; set; }
		public Offensive Offensive { get; set; }
		public int EnergyCost { get; set; }
				
		// Default constructor for JSON serialization
		public MonsterAbility() { }
		public MonsterAbility(string name, int energyCost, Ability abilityCategory, int monsterLevel) {
			this.Name = name;
			this.EnergyCost = energyCost;
			this.AbilityCategory = abilityCategory;
			switch (this.AbilityCategory) {
				case Ability.PoisonBite:
					this.Offensive = new Offensive(
						15 + (monsterLevel - 1) * 10, 5 + (monsterLevel - 1) * 5, 
						1, 3, Offensive.OffensiveType.Bleed);
					break;
				case Ability.BloodLeech:
					this.Offensive = new Offensive(10 + (monsterLevel - 1) * 10);
					break;
				case Ability.TailWhip:
					this.Offensive = new Offensive(10 + (monsterLevel - 1) * 10);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public static void UseBloodLeechAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index].EnergyCost;
			var attackString = "The " + monster.Name + " tries to sink its fangs into you and suck your blood!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var leechAmount = monster.Abilities[index].Offensive.Amount;
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
						var frozenAttackAmount = leechAmount * effect.EffectMultiplier;
						leechAmount = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						var changeDamageAmount = effect.EffectAmountOverTime < leechAmount ? 
							effect.EffectAmountOverTime : leechAmount;
						effect.ChangeOpponentDamageRound(player);
						leechAmount += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < leechAmount ?
							effect.EffectAmount : leechAmount;
						effect.BlockDamageRound(blockAmount);
						leechAmount -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < leechAmount ? 
							effect.EffectAmountOverTime : leechAmount;
						monster.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (leechAmount > 0) continue;
				var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + monster.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			player.HitPoints -= leechAmount;
			monster.HitPoints += leechAmount;
			if (monster.HitPoints > monster.MaxHitPoints) monster.HitPoints = monster.MaxHitPoints;
			var attackSuccessString = "The " + monster.Name + " leeches " + leechAmount + " life from you.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
		}
		public static void UseOffenseDamageAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index].EnergyCost;
			var attackString = string.Empty;
			if (monster.MonsterCategory == Monster.MonsterType.Spider) {
				attackString = "The " + monster.Name + " tries to bite you!";
			}
			else {
				attackString = "The " + monster.Name + " swings its tail at you!";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var attackDamage = monster.Abilities[index].Offensive.Amount;
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
						var frozenAttackAmount = attackDamage * effect.EffectMultiplier;
						attackDamage = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						var changeDamageAmount = effect.EffectAmountOverTime < attackDamage ? 
							effect.EffectAmountOverTime : attackDamage;
						effect.ChangeOpponentDamageRound(player);
						attackDamage += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < attackDamage ?
							effect.EffectAmount : attackDamage;
						effect.BlockDamageRound(blockAmount);
						attackDamage -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < attackDamage ? 
							effect.EffectAmountOverTime : attackDamage;
						monster.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (attackDamage > 0) continue;
				var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + monster.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					effectAbsorbString);
				return;
			}
			var attackSuccessString = string.Empty;
			if (monster.MonsterCategory == Monster.MonsterType.Spider) {
				attackSuccessString = "The " + monster.Name + " bites you for " + attackDamage + " physical damage.";
			}
			else {
				attackSuccessString = "The " + monster.Name + " strikes you with its tail for " + 
				                      attackDamage + " physical damage.";
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			player.HitPoints -= attackDamage;
			if (monster.Abilities[index].Offensive.AmountOverTime <= 0) return;
			switch (monster.Abilities[index].Offensive.OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					var bleedString = "You are bleeding from " + monster.Name + "'s attack!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);
					player.Effects.Add(new Effect(monster.Abilities[index].Name,
						Effect.EffectType.Bleeding, monster.Abilities[index].Offensive.AmountOverTime, 
						monster.Abilities[index].Offensive.AmountCurRounds, monster.Abilities[index].Offensive.AmountMaxRounds, 
						1, 1, true));
					break;
				case Offensive.OffensiveType.Fire:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}