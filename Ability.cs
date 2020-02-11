using System;

namespace DungeonGame {
	public class Ability {
		public enum WarriorAbility {
			Slash,
			Rend,
			Charge,
			Block,
			Berserk,
			Disarm,
			Bandage
		}
		public enum ArcherAbility {
			Distance,
			Gut,
			Precise,
			Stun,
			Double,
			Wound,
			Bandage,
			SwiftAura
		}
		public string Name { get; set; }
		public ArcherAbility ArcAbilityCategory { get; set; }
		public WarriorAbility WarAbilityCategory { get; set; }
		public Bandage Bandage { get; set; }
		public Defensive Defensive { get; set; }
		public Offensive Offensive { get; set; }
		public ChangeAbilityAmount ChangeAbilityAmount { get; set; }
		public Stun Stun { get; set; }
		public int MinLevel { get; set; }
		public int RageCost { get; set; }
		public int ComboCost { get; set; }
		public int Rank { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Ability() {}

		public Ability(string name, int rageCost, int rank, WarriorAbility warAbility, int minLevel) {
			this.Name = name;
			this.RageCost = rageCost;
			this.Rank = rank;
			this.WarAbilityCategory = warAbility;
			this.MinLevel = minLevel;
			switch (this.WarAbilityCategory) {
				case WarriorAbility.Slash:
					this.Offensive = new Offensive(50);
					break;
				case WarriorAbility.Rend:
					this.Offensive = new Offensive(15, 5, 1, 3);
					break;
				case WarriorAbility.Charge:
					this.Stun = new Stun(15, 1, 2);
					break;
				case WarriorAbility.Block:
					this.Defensive = new Defensive(50);
					break;
				case WarriorAbility.Berserk:
					this.Offensive = new Offensive(20);
					this.ChangeAbilityAmount = new ChangeAbilityAmount(15, 1, 4);
					break;
				case WarriorAbility.Disarm:
					this.Offensive = new Offensive(35);
					break;
				case WarriorAbility.Bandage:
					this.Bandage = new Bandage(25, 5, 1, 3);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public Ability(string name, int comboCost, int rank, ArcherAbility archerAbility, int minLevel) {
			this.Name = name;
			this.ComboCost = comboCost;
			this.Rank = rank;
			this.ArcAbilityCategory = archerAbility;
			this.MinLevel = minLevel;
			switch (this.ArcAbilityCategory) {
				case ArcherAbility.Distance:
					this.Offensive = new Offensive(25, 50);
					break;
				case ArcherAbility.Gut:
					this.Offensive = new Offensive(15, 5, 1, 3);
					break;
				case ArcherAbility.Precise:
					this.Offensive = new Offensive(50);
					break;
				case ArcherAbility.Stun:
					this.Stun = new Stun(15, 1, 3);
					break;
				case ArcherAbility.Wound:
					this.Offensive = new Offensive(5, 10, 1, 5);
					break;
				case ArcherAbility.Double:
					this.Offensive = new Offensive(25);
					break;
				case ArcherAbility.Bandage:
					this.Bandage = new Bandage(25, 5, 1, 3);
					break;
				case ArcherAbility.SwiftAura:
					this.ChangeAbilityAmount = new ChangeAbilityAmount(15, 1, 600);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public static void DeductAbilityCost(Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Warrior) {
				player.RagePoints -= player.Abilities[index].RageCost;
			}
			else {
				player.ComboPoints -= player.Abilities[index].ComboCost;
			}
		}
		public static bool OutOfArrows(Player player) {
			return !player.PlayerQuiver.HaveArrows();
		}
		public static void SwiftAuraAbilityInfo(Player player, int index) {
			var swiftAuraString = "Swift Aura Amount: " + player.Abilities[index].ChangeAbilityAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			var swiftAuraInfoString = "Dexterity is increased by " + player.Abilities[index].ChangeAbilityAmount.Amount + 
			                          " for " + (player.Abilities[index].ChangeAbilityAmount.ChangeMaxRound / 60) + " minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				swiftAuraInfoString);
		}
		public static void UseSwiftAura(Player player, int index) {
			player.ComboPoints -= player.Abilities[index].ComboCost;
			const string swiftAuraString = "You generate a swift aura around yourself.";
			player.Dexterity += player.Abilities[index].ChangeAbilityAmount.Amount;
			PlayerHandler.CalculatePlayerStats(player);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.ChangeStat, player.Abilities[index].ChangeAbilityAmount.Amount,
				player.Abilities[index].ChangeAbilityAmount.ChangeCurRound, player.Abilities[index].ChangeAbilityAmount.ChangeMaxRound, 
				1, 1, false));
		}
		public static void StunAbilityInfo(Player player, int index) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Stun.DamageAmount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			var abilityInfoString = "Stuns opponent for " + 
			                        player.Abilities[index].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
		}
		public static void UseStunAbility(Monster opponent, Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.Attack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			var abilityDamage = player.Abilities[index].Stun.DamageAmount;
			if (abilityDamage == 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You " +  player.Abilities[index].Name  + " the " + opponent.Name + " for " +
				                          abilityDamage + " physical damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(abilityDamage);
				var stunString = "The " + opponent.Name + " is stunned!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					stunString);
				opponent.Effects.Add(new Effect(player.Abilities[index].Name, Effect.EffectType.Stunned, 
					player.Abilities[index].Stun.StunCurRounds, player.Abilities[index].Stun.StunMaxRounds, 
					1, 1, true));
			}
		}
		public static void BerserkAbilityInfo(Player player, int index) {
			var dmgIncreaseString = "Damage Increase: " + player.Abilities[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgIncreaseString);
			var armIncreaseString = "Armor Decrease: " + player.Abilities[index].ChangeAbilityAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				armIncreaseString);
			var dmgInfoString = "Damage increased at cost of armor decrease for " +
			                    player.Abilities[index].ChangeAbilityAmount.ChangeMaxRound + " rounds";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgInfoString);
		}
		public static void UseBerserkAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			player.Effects.Add(new Effect(player.Abilities[index].Name + " Damage Increase",
				Effect.EffectType.ChangeDamage, player.Abilities[index].Offensive.Amount,
				player.Abilities[index].ChangeAbilityAmount.ChangeCurRound, player.Abilities[index].ChangeAbilityAmount.ChangeMaxRound, 
				1, 1, false));
			player.Effects.Add(new Effect(player.Abilities[index].Name + " Armor Decrease",
				Effect.EffectType.ChangeArmor, player.Abilities[index].ChangeAbilityAmount.Amount,
				player.Abilities[index].ChangeAbilityAmount.ChangeCurRound, player.Abilities[index].ChangeAbilityAmount.ChangeMaxRound, 
				1, 1, true));
		}
		public static void DistanceAbilityInfo(Player player, int index) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			var abilityInfoString = player.Abilities[index].Offensive.ChanceToSucceed +
			                        "% chance to hit monster in attack direction.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Usage example if monster is in room to north. 'use distance north'");
		}
		public static void UseDistanceAbility(Player player, int index, string direction) {
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
			var roomName = RoomHandler.Rooms.Find(f => f.X == targetX && f.Y == targetY && f.Z == targetZ);
			if (roomName == null) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack in that direction!");
				return;
			}
			var roomIndex = RoomHandler.Rooms.IndexOf(roomName);
			var opponentRoom = RoomHandler.Rooms[roomIndex] as DungeonRoom;
			var opponent = opponentRoom?.Monster;
			if (opponent == null) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"There is no monster in that direction to attack!");
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			var successChance = GameHandler.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive.ChanceToSucceed) {
				var attackString = "You tried to shoot " + opponent.Name + " from afar but failed!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					attackString);
			}
			else {
				Settings.FormatAttackSuccessText();
				opponent.HitPoints -= player.Abilities[index].Offensive.Amount;
				var shootString = "You successfully shot " + opponent.Name + " from afar for " + 
				                  player.Abilities[index].Offensive.Amount + " damage!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					shootString);
				opponent.IsMonsterDead(player);
			}
		}
		public static void DefenseAbilityInfo(Player player, int index) {
			var abilityDmgString = "Absorb Damage: " + player.Abilities[index].Defensive.AbsorbDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
		}
		public static int UseDefenseAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			return player.Abilities[index].Defensive.AbsorbDamage;
		}
		public static void BandageAbilityInfo(Player player, int index) {
			var healAmountString = "Heal Amount: " + player.Abilities[index].Bandage.HealAmount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player.Abilities[index].Bandage.HealOverTime <= 0) return;
			var healOverTimeString = "Heal Over Time: " + player.Abilities[index].Bandage.HealOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			var healInfoStringCombat = "Heal over time will restore health for " + 
			                     player.Abilities[index].Bandage.HealMaxRounds + " rounds in combat.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringCombat);
			var healInfoStringNonCombat = "Heal over time will restore health " + 
			                           player.Abilities[index].Bandage.HealMaxRounds + " times every 10 seconds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringNonCombat);
		}
		public static void UseBandageAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			var healAmount = player.Abilities[index].Bandage.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healString);
			player.HitPoints += healAmount;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
			if (player.Abilities[index].Bandage.HealOverTime <= 0) return;
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.Healing, player.Abilities[index].Bandage.HealOverTime,
				player.Abilities[index].Bandage.HealCurRounds, player.Abilities[index].Bandage.HealMaxRounds,
				1, 10, false));
		}
		public static void DisarmAbilityInfo(Player player, int index) {
			var abilityString = player.Abilities[index].Offensive.Amount + "% chance to disarm opponent's weapon.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityString);
		}
		public static void UseDisarmAbility(Monster opponent, Player player, int index) {
			DeductAbilityCost(player, index);
			var successChance = GameHandler.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive.Amount) {
				var disarmFailString = "You tried to disarm " + opponent.Name + " but failed!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					disarmFailString);
			}
			else {
				opponent.MonsterWeapon.Equipped = false;
				var disarmSuccessString = "You successfully disarmed " + opponent.Name + "!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					disarmSuccessString);
			}
		}
		public static void OffenseDamageAbilityInfo(Player player, int index) {
			var abilityDmgString = "Instant Damage: " + player.Abilities[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			if (player.Abilities[index].ArcAbilityCategory == ArcherAbility.Double) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.");
			}
			if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
			var dmgOverTimeString = "Damage Over Time: " + player.Abilities[index].Offensive.AmountOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgOverTimeString);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[index].Offensive.AmountMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				bleedOverTimeString);
		}
		public static void UseOffenseDamageAbility(Monster opponent, Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.Attack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			var abilityDamage = player.Abilities[index].Offensive.Amount;
			if (abilityDamage == 0) {
				var abilityFailString = "Your " + player.Abilities[index].Name + " missed!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					abilityFailString);
			}
			else {
				var abilitySuccessString = "You " + player.Abilities[index].Name + " the " + opponent.Name + " for " +
				                           abilityDamage + " physical damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					abilitySuccessString);
				opponent.TakeDamage(abilityDamage);
				if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
				var bleedString = "The " + opponent.Name + " is bleeding!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					bleedString);
				opponent.Effects.Add(new Effect(player.Abilities[index].Name,
					Effect.EffectType.Bleeding, player.Abilities[index].Offensive.AmountOverTime, 
					player.Abilities[index].Offensive.AmountCurRounds, player.Abilities[index].Offensive.AmountMaxRounds, 
					1, 1, true));
			}
		}
	}
}