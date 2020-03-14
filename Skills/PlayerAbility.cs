using System;

namespace DungeonGame {
	public class PlayerAbility {
		public enum WarriorAbility {
			Slash,
			Rend,
			Charge,
			Block,
			Berserk,
			Disarm,
			Bandage,
			PowerAura,
			WarCry,
			Onslaught
		}
		public enum ArcherAbility {
			Distance,
			Gut,
			Precise,
			Stun,
			Double,
			Wound,
			Bandage,
			SwiftAura,
			ImmolatingArrow,
			Ambush
		}
		public string Name { get; set; }
		public ArcherAbility? ArcAbilityCategory { get; set; }
		public WarriorAbility? WarAbilityCategory { get; set; }
		public Healing Healing { get; set; }
		public Defensive Defensive { get; set; }
		public Offensive Offensive { get; set; }
		public ChangeAmount ChangeAmount { get; set; }
		public Stun Stun { get; set; }
		public int MinLevel { get; set; }
		public int? RageCost { get; set; }
		public int? ComboCost { get; set; }
		public int Rank { get; set; }
		
		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public PlayerAbility() {}
		public PlayerAbility(string name, int rageCost, int rank, WarriorAbility warAbility, int minLevel) {
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
					this.Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case WarriorAbility.Charge:
					this.Stun = new Stun(15, 1, 2);
					break;
				case WarriorAbility.Block:
					this.Defensive = new Defensive(50);
					break;
				case WarriorAbility.Berserk:
					this.Offensive = new Offensive(20, 1, 4);
					this.ChangeAmount = new ChangeAmount(-15, 1, 4);
					break;
				case WarriorAbility.Disarm:
					this.Offensive = new Offensive(35);
					break;
				case WarriorAbility.Bandage:
					this.Healing = new Healing(25, 5, 1, 3);
					break;
				case WarriorAbility.PowerAura:
					this.ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case WarriorAbility.WarCry:
					this.ChangeAmount = new ChangeAmount(-25, 1, 3);
					break;
				case WarriorAbility.Onslaught:
					this.Offensive = new Offensive(25);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public PlayerAbility(string name, int comboCost, int rank, ArcherAbility archerAbility, int minLevel) {
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
					this.Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Precise:
					this.Offensive = new Offensive(50);
					break;
				case ArcherAbility.Stun:
					this.Stun = new Stun(15, 1, 3);
					break;
				case ArcherAbility.Wound:
					this.Offensive = new Offensive(
						5, 10, 1, 5, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Double:
					this.Offensive = new Offensive(25);
					break;
				case ArcherAbility.Bandage:
					this.Healing = new Healing(25, 5, 1, 3);
					break;
				case ArcherAbility.SwiftAura:
					this.ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case ArcherAbility.ImmolatingArrow:
					this.Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case ArcherAbility.Ambush:
					this.Offensive = new Offensive(50);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private static void DeductAbilityCost(Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Warrior) {
				player.RagePoints -= player.Abilities[index].RageCost;
			}
			else {
				player.ComboPoints -= player.Abilities[index].ComboCost;
			}
		}
		public static void PowerAuraAbilityInfo(Player player, int index) {
			var powerAuraString = "Power Aura Amount: " + player.Abilities[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				powerAuraString);
			var powerAuraInfoString = "Strength is increased by " + player.Abilities[index].ChangeAmount.Amount + 
			                          " for " + player.Abilities[index].ChangeAmount.ChangeMaxRound / 60 + " minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				powerAuraInfoString);
		}
		public static void UsePowerAura(Player player, int index) {
			DeductAbilityCost(player, index);
			const string powerAuraString = "You generate a Power Aura around yourself.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);
			var powerAuraIndex = player.Effects.FindIndex(e => e.Name == player.Abilities[index].Name);
			if (powerAuraIndex != -1) {
				player.Effects[powerAuraIndex].IsEffectExpired = true;
				GameHandler.RemovedExpiredEffects(player);
			}
			player.Strength += player.Abilities[index].ChangeAmount.Amount;
			PlayerHandler.CalculatePlayerStats(player);
			player.Effects.Add(new Effect(player.Abilities[index].Name, Effect.EffectType.ChangeStat,
				player.Abilities[index].ChangeAmount.Amount, player.Abilities[index].ChangeAmount.ChangeCurRound,
				player.Abilities[index].ChangeAmount.ChangeMaxRound, 1, 1, false,
				ChangeStat.StatType.Strength));
		}
		public static void SwiftAuraAbilityInfo(Player player, int index) {
			var swiftAuraString = "Swift Aura Amount: " + player.Abilities[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			var swiftAuraInfoString = "Dexterity is increased by " + player.Abilities[index].ChangeAmount.Amount + 
			                          " for " + player.Abilities[index].ChangeAmount.ChangeMaxRound / 60 + " minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				swiftAuraInfoString);
		}
		public static void UseSwiftAura(Player player, int index) {
			DeductAbilityCost(player, index);
			const string swiftAuraString = "You generate a Swift Aura around yourself.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			var swiftAuraIndex = player.Effects.FindIndex(e => e.Name == player.Abilities[index].Name);
			if (swiftAuraIndex != -1) {
				player.Effects[swiftAuraIndex].IsEffectExpired = true;
				GameHandler.RemovedExpiredEffects(player);
			}
			player.Dexterity += player.Abilities[index].ChangeAmount.Amount;
			PlayerHandler.CalculatePlayerStats(player);
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.ChangeStat, player.Abilities[index].ChangeAmount.Amount,
				player.Abilities[index].ChangeAmount.ChangeCurRound, player.Abilities[index].ChangeAmount.ChangeMaxRound,
				1, 1, false, ChangeStat.StatType.Dexterity));
		}
		public static void WarCryAbilityInfo(Player player, int index) {
			var warCryString = "War Cry Amount: " + -1 * player.Abilities[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				warCryString);
			var warCryInfoString = "Opponent's attacks are decreased by " + -1 * player.Abilities[index].ChangeAmount.Amount 
			                        + " for " + player.Abilities[index].ChangeAmount.ChangeMaxRound + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				warCryInfoString);
		}
		public static void UseWarCry(Player player, int index) {
			DeductAbilityCost(player, index);
			const string warCryString = "You shout a War Cry, intimidating your opponent, and decreasing incoming damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				warCryString);
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.ChangeOpponentDamage, player.Abilities[index].ChangeAmount.Amount,
				player.Abilities[index].ChangeAmount.ChangeCurRound, player.Abilities[index].ChangeAmount.ChangeMaxRound, 
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
			if (player.PlayerClass == Player.PlayerClassType.Archer && PlayerHandler.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			var abilityDamage = player.Abilities[index].Stun.DamageAmount;
			var attackSuccessString = "You " +  player.Abilities[index].Name  + " the " + opponent.Name + " for " +
			                          abilityDamage + " physical damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			opponent.HitPoints -= abilityDamage;
			var stunString = "The " + opponent.Name + " is stunned!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunString);
			opponent.Effects.Add(new Effect(player.Abilities[index].Name, Effect.EffectType.Stunned, 
				player.Abilities[index].Stun.StunCurRounds, player.Abilities[index].Stun.StunMaxRounds, 
				1, 1, true));
		}
		public static void BerserkAbilityInfo(Player player, int index) {
			var dmgIncreaseString = "Damage Increase: " + player.Abilities[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgIncreaseString);
			var armDecreaseString = "Armor Decrease: " + player.Abilities[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				armDecreaseString);
			var dmgInfoString = "Damage increased at cost of armor decrease for " +
			                    player.Abilities[index].ChangeAmount.ChangeMaxRound + " rounds";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgInfoString);
		}
		public static void UseBerserkAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			const string berserkString = "You go into a berserk rage!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				berserkString);
			player.Effects.Add(new Effect(player.Abilities[index].Name + " Damage Increase",
				Effect.EffectType.ChangePlayerDamage, player.Abilities[index].Offensive.Amount,
				player.Abilities[index].ChangeAmount.ChangeCurRound, player.Abilities[index].ChangeAmount.ChangeMaxRound, 
				1, 1, false));
			player.Effects.Add(new Effect(player.Abilities[index].Name + " Armor Decrease",
				Effect.EffectType.ChangeArmor, player.Abilities[index].ChangeAmount.Amount,
				player.Abilities[index].ChangeAmount.ChangeCurRound, player.Abilities[index].ChangeAmount.ChangeMaxRound, 
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
				if (opponent.HitPoints <= 0) opponent.MonsterDeath(player);
			}
		}
		public static void DefenseAbilityInfo(Player player, int index) {
			var abilityDmgString = "Block Damage: " + player.Abilities[index].Defensive.BlockDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				blockInfoString);
		}
		public static void UseDefenseAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			var blockAmount = player.Abilities[index].Defensive.BlockDamage;
			var blockString = "You start blocking your opponent's attacks! You will block " + blockAmount + " damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.BlockDamage, blockAmount, 10));
		}
		public static void BandageAbilityInfo(Player player, int index) {
			var healAmountString = "Heal Amount: " + player.Abilities[index].Healing.HealAmount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player.Abilities[index].Healing.HealOverTime <= 0) return;
			var healOverTimeString = "Heal Over Time: " + player.Abilities[index].Healing.HealOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			var healInfoStringCombat = "Heal over time will restore health for " + 
			                     player.Abilities[index].Healing.HealMaxRounds + " rounds in combat.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringCombat);
			var healInfoStringNonCombat = "Heal over time will restore health " + 
			                           player.Abilities[index].Healing.HealMaxRounds + " times every 10 seconds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringNonCombat);
		}
		public static void UseBandageAbility(Player player, int index) {
			DeductAbilityCost(player, index);
			var healAmount = player.Abilities[index].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healString);
			player.HitPoints += healAmount;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
			if (player.Abilities[index].Healing.HealOverTime <= 0) return;
			player.Effects.Add(new Effect(player.Abilities[index].Name,
				Effect.EffectType.Healing, player.Abilities[index].Healing.HealOverTime,
				player.Abilities[index].Healing.HealCurRounds, player.Abilities[index].Healing.HealMaxRounds,
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
			if (player.Abilities[index].WarAbilityCategory == WarriorAbility.Onslaught) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two attacks are launched which each cause instant damage. Cost and damage are per attack.");
			}
			if (player.Abilities[index].ArcAbilityCategory == ArcherAbility.Ambush) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"A surprise attack is launched, which initiates combat.");
			}
			if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
			var dmgOverTimeString = "Damage Over Time: " + player.Abilities[index].Offensive.AmountOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgOverTimeString);
			switch (player.Abilities[index].Offensive.OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					var bleedOverTimeString = "Bleeding damage over time for " + 
					                          player.Abilities[index].Offensive.AmountMaxRounds + " rounds.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						bleedOverTimeString);
					break;
				case Offensive.OffensiveType.Fire:
					var onFireString = "Fire damage over time for " + 
					                   player.Abilities[index].Offensive.AmountMaxRounds + " rounds.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						onFireString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static void UseOffenseDamageAbility(Monster opponent, Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && PlayerHandler.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			var abilityDamage = player.Abilities[index].Offensive.Amount;
			opponent.HitPoints -= abilityDamage;
			var abilitySuccessString = "Your " + player.Abilities[index].Name + " hit the " + opponent.Name + " for " +
			                           abilityDamage + " physical damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				abilitySuccessString);
			if (player.Abilities[index].Offensive.AmountOverTime <= 0) return;
			switch (player.Abilities[index].Offensive.OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					var bleedString = "The " + opponent.Name + " is bleeding!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);
					opponent.Effects.Add(new Effect(player.Abilities[index].Name,
						Effect.EffectType.Bleeding, player.Abilities[index].Offensive.AmountOverTime, 
						player.Abilities[index].Offensive.AmountCurRounds, player.Abilities[index].Offensive.AmountMaxRounds, 
						1, 1, true));
					break;
				case Offensive.OffensiveType.Fire:
					var onFireString = "The " + opponent.Name + " bursts into flame!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatOnFireText(),
						Settings.FormatDefaultBackground(),
						onFireString);
					opponent.Effects.Add(new Effect(player.Abilities[index].Name,
						Effect.EffectType.OnFire, player.Abilities[index].Offensive.AmountOverTime, 
						player.Abilities[index].Offensive.AmountCurRounds, player.Abilities[index].Offensive.AmountMaxRounds, 
						1, 1, true));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}