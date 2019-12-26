using System;

namespace DungeonGame {
	public class Ability {
		public enum AbilityType {
			DirectDamage,
			DamageOverTime,
			Stun,
			Block
		}
		public string Name { get; set; }
		public AbilityType AbilityCategory { get; set; }
		public Defensive Defensive { get; set; }
		public Offensive Offensive { get; set; }
		public Stun Stun { get; set; }
		public int RageCost { get; set; }
		public bool CanStun { get; set; }
		public int Rank { get; set; }

		public Ability(string name, int rageCost, int rank, AbilityType abilityType) {
			this.Name = name;
			this.RageCost = rageCost;
			this.Rank = rank;
			this.AbilityCategory = abilityType;
			switch (AbilityCategory) {
				case AbilityType.DirectDamage:
					this.Offensive = new Offensive(50);
					break;
				case AbilityType.DamageOverTime:
					this.Offensive = new Offensive(10, 5, 1, 3);
					break;
				case AbilityType.Stun:
					this.Stun = new Stun(15, 1, 2);
					break;
				case AbilityType.Block:
					this.Defensive = new Defensive(50);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public string GetName() {
			return this.Name.ToString();
		}
		public void StunAbilityInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Abilities[index].Stun.DamageAmount);
			Console.WriteLine("Stuns opponent for {0} rounds, preventing their attacks.", player.Abilities[index].Stun.StunMaxRounds);
		}
		public void UseStunAbility(IMonster opponent, Player player, int index) {
			player.RagePoints -= player.Abilities[index].RageCost;
			var abilityDamage = player.Abilities[index].Stun.DamageAmount;
			if (abilityDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You missed!");
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You hit the {0} for {1} physical damage.", opponent.Name, abilityDamage);
				opponent.TakeDamage(abilityDamage);
				opponent.IsStunned = true;
				Console.WriteLine("The {0} is stunned!", opponent.Name);
				opponent.StartStunned(
					true,
					player.Abilities[index].Stun.StunCurRounds,
					player.Abilities[index].Stun.StunMaxRounds
				);
			}
		}
		public void DefenseAbilityInfo(Player player, int index) {
			Console.WriteLine("Absorb Damage: {0}", player.Abilities[index].Defensive.AbsorbDamage);
		}
		public int UseDefenseAbility(Player player, int index) {
			Helper.FormatAttackSuccessText();
			player.RagePoints -= player.Abilities[index].RageCost;
			return player.Abilities[index].Defensive.AbsorbDamage;
		}
		public void OffenseDamageAbilityInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Abilities[index].Offensive.DamageAmount);
			if (player.Abilities[index].Offensive.DamageOverTime <= 0) return;
			Console.WriteLine("Damage Over Time: {0}", player.Abilities[index].Offensive.DamageOverTime);
			Console.WriteLine("Bleeding damage over time for {0} rounds.", player.Abilities[index].Offensive.DamageMaxRounds);
		}
		public void UseOffenseDamageAbility(IMonster opponent, Player player, int index) {
			player.RagePoints -= player.Abilities[index].RageCost;
			var abilityDamage = player.Abilities[index].Offensive.DamageAmount;
			if (abilityDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("Your {0} missed!", player.Abilities[index].Name);
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You {0} the {1} for {2} physical damage.",
					player.Abilities[index].Name,
					opponent.Name,
					abilityDamage);
				opponent.TakeDamage(abilityDamage);
				if (player.Abilities[index].Offensive.DamageOverTime <= 0) return;
				Console.WriteLine("The {0} is bleeding!", opponent.Name);
				opponent.StartBleeding(
					true,
					player.Abilities[index].Offensive.DamageOverTime,
					player.Abilities[index].Offensive.DamageCurRounds,
					player.Abilities[index].Offensive.DamageMaxRounds
					);
			}
		}
	}
}