using System;

namespace DungeonGame {
	public class Ability {
		public enum AbilityType {
			Slash,
			Rend,
			Charge,
			Block,
			Berserk,
			Disarm
		}
		public enum ShotType {
			Distance,
			Gut,
			Precise,
			Stun,
			Double,
			Poison
		}
		private static readonly Random RndGenerate = new Random();
		public string Name { get; set; }
		public ShotType ShotCategory { get; set; }
		public AbilityType AbilityCategory { get; set; }
		public Defensive Defensive { get; set; }
		public Offensive Offensive { get; set; }
		public ChangeArmor ChangeArmor { get; set; }
		public Stun Stun { get; set; }
		public int RageCost { get; set; }
		public int ComboCost { get; set; }
		public bool CanStun { get; set; }
		public int Rank { get; set; }

		public Ability(string name, int rageCost, int rank, AbilityType abilityType) {
			this.Name = name;
			this.RageCost = rageCost;
			this.Rank = rank;
			this.AbilityCategory = abilityType;
			switch (this.AbilityCategory) {
				case AbilityType.Slash:
					this.Offensive = new Offensive(50);
					break;
				case AbilityType.Rend:
					this.Offensive = new Offensive(10, 5, 1, 3);
					break;
				case AbilityType.Charge:
					this.Stun = new Stun(15, 1, 2);
					break;
				case AbilityType.Block:
					this.Defensive = new Defensive(50);
					break;
				case AbilityType.Berserk:
					this.Offensive = new Offensive(20);
					this.ChangeArmor = new ChangeArmor(15, 1, 4);
					break;
				case AbilityType.Disarm:
					this.Offensive = new Offensive(35);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public Ability(string name, int comboCost, int rank, ShotType shotType) {
			this.Name = name;
			this.ComboCost = comboCost;
			this.Rank = rank;
			this.ShotCategory = shotType;
			switch (this.ShotCategory) {
				case ShotType.Distance:
					this.Offensive = new Offensive(25);
					break;
				case ShotType.Gut:
					this.Offensive = new Offensive(15, 5, 1, 3);
					break;
				case ShotType.Precise:
					this.Offensive = new Offensive(50);
					break;
				case ShotType.Stun:
					this.Stun = new Stun(15, 1, 3);
					break;
				case ShotType.Poison:
					this.Offensive = new Offensive(0, 5, 1, 10);
					break;
				case ShotType.Double:
					this.Offensive = new Offensive(30);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public string GetName() {
			return this.Name.ToString();
		}

		public void DeductAbilityCost(Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Warrior) {
				player.RagePoints -= player.Abilities[index].RageCost;
			}
			else {
				player.ComboPoints -= player.Abilities[index].ComboCost;
			}
		}
		public void StunAbilityInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Abilities[index].Stun.DamageAmount);
			Console.WriteLine("Stuns opponent for {0} rounds, preventing their attacks.", player.Abilities[index].Stun.StunMaxRounds);
		}
		public void UseStunAbility(IMonster opponent, Player player, int index) {
			this.DeductAbilityCost(player, index);
			var abilityDamage = player.Abilities[index].Stun.DamageAmount;
			if (abilityDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You missed!");
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You {0} the {1} for {2} physical damage.",
					player.Abilities[index].Name,
					opponent.Name,
					abilityDamage);
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
		public void BerserkAbilityInfo(Player player, int index) {
			Console.WriteLine("Damage Increase: {0}", player.Abilities[index].Offensive.Amount);
			Console.WriteLine("Armor Decrease: {0}", player.Abilities[index].ChangeArmor.ChangeArmorAmount);
			Console.WriteLine("Damage increased at cost of armor decrease for {0} rounds", player.Abilities[index].ChangeArmor.ChangeMaxRound);
		}
		public int[] UseBerserkAbility(IMonster opponent, Player player, int index) {
			Helper.FormatAttackSuccessText();
			this.DeductAbilityCost(player, index);
			var returnValues = new int[4];
			returnValues[0] = player.Abilities[index].Offensive.Amount; // Damage increase amount
			returnValues[1] = player.Abilities[index].ChangeArmor.ChangeArmorAmount; // Armor decrease amount
			returnValues[2] = player.Abilities[index].ChangeArmor.ChangeCurRound; // Armor decrease current round
			returnValues[3] = player.Abilities[index].ChangeArmor.ChangeMaxRound; // Armor decrease max round
			return returnValues;
		}
		public void DefenseAbilityInfo(Player player, int index) {
			Console.WriteLine("Absorb Damage: {0}", player.Abilities[index].Defensive.AbsorbDamage);
		}
		public int UseDefenseAbility(Player player, int index) {
			Helper.FormatAttackSuccessText();
			this.DeductAbilityCost(player, index);
			return player.Abilities[index].Defensive.AbsorbDamage;
		}
		public void DisarmAbilityInfo(Player player, int index) {
			Console.WriteLine("{0}% chance to disarm opponent's weapon.", player.Abilities[index].Offensive.Amount);
		}
		public void UseDisarmAbility(IMonster opponent, Player player, int index) {
			this.DeductAbilityCost(player, index);
			var successChance = RndGenerate.Next(1, 100);
			if (successChance > player.Abilities[index].Offensive.Amount) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You tried to disarm {0} but failed!", opponent.Name);
			}
			else {
				Helper.FormatAttackSuccessText();
				opponent.MonsterWeapon.Equipped = false;
				Console.WriteLine("You successfully disarmed {0}!", opponent.Name);
			}
		}
		public void OffenseDamageAbilityInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Abilities[index].Offensive.Amount);
			if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
			Console.WriteLine("Damage Over Time: {0}", player.Abilities[index].Offensive.AmountOverTime);
			Console.WriteLine("Bleeding damage over time for {0} rounds.", player.Abilities[index].Offensive.AmountMaxRounds);
		}
		public void UseOffenseDamageAbility(IMonster opponent, Player player, int index) {
			this.DeductAbilityCost(player, index);
			var abilityDamage = player.Abilities[index].Offensive.Amount;
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
				if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
				Console.WriteLine("The {0} is bleeding!", opponent.Name);
				opponent.StartBleeding(
					true,
					player.Abilities[index].Offensive.AmountOverTime,
					player.Abilities[index].Offensive.AmountCurRounds,
					player.Abilities[index].Offensive.AmountMaxRounds
					);
			}
		}
	}
}