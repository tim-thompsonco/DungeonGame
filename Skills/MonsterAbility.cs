using System;

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
			var leechAmount = monster.Abilities[index].Offensive.Amount;
			var attackString = "The " + monster.Name + " sinks its fangs into you and drains your blood!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackString);
			var attackSuccessString = "The " + monster.Name + " leeches " + leechAmount + " life from you.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			player.HitPoints -= leechAmount;
			monster.HitPoints += leechAmount;
			if (monster.HitPoints > monster.MaxHitPoints) monster.HitPoints = monster.MaxHitPoints;
		}
		public static void UseOffenseDamageAbility(Monster monster, Player player, int index) {
			monster.EnergyPoints -= monster.Abilities[index].EnergyCost;
			var attackDamage = monster.Abilities[index].Offensive.Amount;
			player.TakeDamage(attackDamage);
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