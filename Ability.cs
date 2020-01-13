using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
			Wound
		}
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
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Ability() {}

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
					this.Offensive = new Offensive(25, 50);
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
				case ShotType.Wound:
					this.Offensive = new Offensive(5, 10, 1, 5);
					break;
				case ShotType.Double:
					this.Offensive = new Offensive(25);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public string GetName() {
			return this.Name;
		}
		public static void DeductAbilityCost(Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Warrior) {
				player.RagePoints -= player.Abilities[index].RageCost;
			}
			else {
				player.ComboPoints -= player.Abilities[index].ComboCost;
				
				player.PlayerQuiver.UseArrow();
			}
		}
		public static bool OutOfArrows(Player player) {
			return !player.PlayerQuiver.HaveArrows();
		}
		public static void StunAbilityInfo(Player player, int index, UserOutput output) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Stun.DamageAmount;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityDmgString);
			var abilityInfoString = "Stuns opponent for " + 
			                        player.Abilities[index].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityInfoString);
		}
		public static void UseStunAbility(IMonster opponent, Player player, int index, UserOutput output) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.Attack(output);
				return;
			}
			DeductAbilityCost(player, index);
			var abilityDamage = player.Abilities[index].Stun.DamageAmount;
			if (abilityDamage == 0) {
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You " +  player.Abilities[index].Name  + " the " + opponent.Name + " for " +
				                          abilityDamage + " physical damage.";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(abilityDamage);
				opponent.IsStunned = true;
				var stunString = "The " + opponent.Name + " is stunned!";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					stunString);
				opponent.StartStunned(
					true,
					player.Abilities[index].Stun.StunCurRounds,
					player.Abilities[index].Stun.StunMaxRounds
				);
			}
		}
		public static void BerserkAbilityInfo(Player player, int index, UserOutput output) {
			var dmgIncreaseString = "Damage Increase: " + player.Abilities[index].Offensive.Amount;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				dmgIncreaseString);
			var armIncreaseString = "Armor Decrease: " + player.Abilities[index].ChangeArmor.ChangeArmorAmount;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				armIncreaseString);
			var dmgInfoString = "Damage increased at cost of armor decrease for " +
			                    player.Abilities[index].ChangeArmor.ChangeMaxRound + " rounds";
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				dmgInfoString);
		}
		public static int[] UseBerserkAbility(IMonster opponent, Player player, int index) {
			DeductAbilityCost(player, index);
			var returnValues = new int[4];
			returnValues[0] = player.Abilities[index].Offensive.Amount; // Damage increase amount
			returnValues[1] = player.Abilities[index].ChangeArmor.ChangeArmorAmount; // Armor decrease amount
			returnValues[2] = player.Abilities[index].ChangeArmor.ChangeCurRound; // Armor decrease current round
			returnValues[3] = player.Abilities[index].ChangeArmor.ChangeMaxRound; // Armor decrease max round
			return returnValues;
		}
		public static void DistanceAbilityInfo(Player player, int index, UserOutput output) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Offensive.Amount;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityDmgString);
			var abilityInfoString = player.Abilities[index].Offensive.ChanceToSucceed +
			                        "% chance to hit monster in attack direction.";
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityInfoString);
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				"Usage example if monster is in room to north. 'use distance north'");
		}
		public static void UseDistanceAbility(
			List<IRoom> spawnedRooms,
			Player player,
			int index,
			string direction, 
			UserOutput output) {
			var targetX = player.X;
			var targetY = player.Y;
			var targetZ = player.Z;
			switch (direction) {
				case "n":
				case "north":
					targetY += 1;
					break;
				case "s":
				case "south":
					targetY -= 1;
					break;
				case "e":
				case "east":
					targetX += 1;
					break;
				case "w":
				case "west":
					targetX -= 1;
					break;
				case "ne":
				case "northeast":
					targetX += 1;
					targetY += 1;
					break;
				case "nw":
				case "northwest":
					targetX -= 1;
					targetY += 1;
					break;
				case "se":
				case "southeast":
					targetX += 1;
					targetY -= 1;
					break;
				case "sw":
				case "southwest":
					targetX -= 1;
					targetY -= 1;
					break;
				case "u":
				case "up":
					targetZ += 1;
					break;
				case "d":
				case "down":
					targetZ -= 1;
					break;
			}
			var roomName = spawnedRooms.Find(f => f.X == targetX && f.Y == targetY && f.Z == targetZ);
			if (roomName == null) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't attack in that direction!");
				return;
			}
			var roomIndex = spawnedRooms.IndexOf(roomName);
			var opponent = spawnedRooms[roomIndex].GetMonster();
			if (opponent == null) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"There is no monster in that direction to attack!");
				return;
			}
			DeductAbilityCost(player, index);
			var successChance = Helper.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive.ChanceToSucceed) {
				var attackString = "You tried to shoot " + opponent.Name + " from afar but failed!";
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					attackString);
			}
			else {
				Helper.FormatAttackSuccessText();
				opponent.HitPoints -= player.Abilities[index].Offensive.Amount;
				var shootString = "You successfully shot " + opponent.Name + " from afar for " + 
				                  player.Abilities[index].Offensive.Amount + " damage!"; 
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					shootString);
				opponent.IsMonsterDead(player, output);
			}
		}
		public static void DefenseAbilityInfo(Player player, int index, UserOutput output) {
			var abilityDmgString = "Absorb Damage: " + player.Abilities[index].Defensive.AbsorbDamage;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityDmgString);
		}
		public static int UseDefenseAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			return player.Abilities[index].Defensive.AbsorbDamage;
		}
		public static void DisarmAbilityInfo(Player player, int index, UserOutput output) {
			var abilityString = player.Abilities[index].Offensive.Amount + "% chance to disarm opponent's weapon.";
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityString);
		}
		public static void UseDisarmAbility(IMonster opponent, Player player, int index, UserOutput output) {
			DeductAbilityCost(player, index);
			var successChance = Helper.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive.Amount) {
				var disarmFailString = "You tried to disarm " + opponent.Name + " but failed!";
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					disarmFailString);
			}
			else {
				opponent.MonsterWeapon.Equipped = false;
				var disarmSuccessString = "You successfully disarmed " + opponent.Name + "!";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					disarmSuccessString);
			}
		}
		public static void OffenseDamageAbilityInfo(Player player, int index, UserOutput output) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Offensive.Amount;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				abilityDmgString);
			if (player.Abilities[index].ShotCategory == ShotType.Double) {
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					"Two arrows are fired which each cause instant damage.");
			}
			if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
			var dmgOverTimeString = "Damage Over Time: " + player.Abilities[index].Offensive.AmountOverTime;
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				dmgOverTimeString);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[index].Offensive.AmountMaxRounds + " rounds.";
			output.StoreUserOutput(
				Helper.FormatInfoText(),
				Helper.FormatDefaultBackground(),
				bleedOverTimeString);
		}
		public static void UseOffenseDamageAbility(IMonster opponent, Player player, int index, UserOutput output) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.Attack(output);
				return;
			}
			DeductAbilityCost(player, index);
			var abilityDamage = player.Abilities[index].Offensive.Amount;
			if (abilityDamage == 0) {
				var abilityFailString = "Your " + player.Abilities[index].Name + " missed!";
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					abilityFailString);
			}
			else {
				;
				var abilitySuccessString = "You " + player.Abilities[index].Name + " the " + opponent.Name + " for " +
				                           abilityDamage + " physical damage.";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					abilitySuccessString);
				opponent.TakeDamage(abilityDamage);
				if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
				var bleedString = "The " + opponent.Name + " is bleeding!";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					bleedString);
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