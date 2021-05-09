using DungeonGame.Controllers;
using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class PlayerAbility {
		public enum DamageType {
			Physical,
			Fire,
			Frost,
			Arcane
		}
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
		public DamageType? DamageGroup { get; set; }
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
		public PlayerAbility() { }
		public PlayerAbility(string name, int rageCost, int rank, WarriorAbility warAbility, int minLevel) {
			Name = name;
			RageCost = rageCost;
			Rank = rank;
			WarAbilityCategory = warAbility;
			MinLevel = minLevel;
			switch (WarAbilityCategory) {
				case WarriorAbility.Slash:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(50);
					break;
				case WarriorAbility.Rend:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case WarriorAbility.Charge:
					DamageGroup = DamageType.Physical;
					Stun = new Stun(15, 1, 2);
					break;
				case WarriorAbility.Block:
					Defensive = new Defensive(50);
					break;
				case WarriorAbility.Berserk:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(20, 1, 4);
					ChangeAmount = new ChangeAmount(-15, 1, 4);
					break;
				case WarriorAbility.Disarm:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(35);
					break;
				case WarriorAbility.Bandage:
					Healing = new Healing(25, 5, 1, 3);
					break;
				case WarriorAbility.PowerAura:
					ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case WarriorAbility.WarCry:
					ChangeAmount = new ChangeAmount(-25, 1, 3);
					break;
				case WarriorAbility.Onslaught:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(25);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public PlayerAbility(string name, int comboCost, int rank, ArcherAbility archerAbility, int minLevel) {
			Name = name;
			ComboCost = comboCost;
			Rank = rank;
			ArcAbilityCategory = archerAbility;
			MinLevel = minLevel;
			switch (ArcAbilityCategory) {
				case ArcherAbility.Distance:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(25, 50);
					break;
				case ArcherAbility.Gut:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Precise:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(50);
					break;
				case ArcherAbility.Stun:
					DamageGroup = DamageType.Physical;
					Stun = new Stun(15, 1, 3);
					break;
				case ArcherAbility.Wound:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(
						5, 10, 1, 5, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Double:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(25);
					break;
				case ArcherAbility.Bandage:
					Healing = new Healing(25, 5, 1, 3);
					break;
				case ArcherAbility.SwiftAura:
					ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case ArcherAbility.ImmolatingArrow:
					DamageGroup = DamageType.Fire;
					Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case ArcherAbility.Ambush:
					DamageGroup = DamageType.Physical;
					Offensive = new Offensive(50);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void DeductAbilityCost(Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Warrior) {
				player.RagePoints -= player.Abilities[index].RageCost;
			} else {
				player.ComboPoints -= player.Abilities[index].ComboCost;
			}
		}
		public static void PowerAuraAbilityInfo(Player player, int index) {
			string powerAuraString = $"Power Aura Amount: {player.Abilities[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);
			string powerAuraInfoString = $"Strength is increased by {player.Abilities[index].ChangeAmount._Amount} for " +
				$"{player.Abilities[index].ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				powerAuraInfoString);
		}

		public static void UsePowerAura(Player player, int index) {
			DeductAbilityCost(player, index);

			const string powerAuraString = "You generate a Power Aura around yourself.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);

			int powerAuraIndex = player.Effects.FindIndex(e => e.Name == player.Abilities[index].Name);
			if (powerAuraIndex != -1) {
				player.Effects[powerAuraIndex].IsEffectExpired = true;
			}

			player.Strength += player.Abilities[index].ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player.Effects.Add(new ChangeStatEffect(player.Abilities[index].Name, player.Abilities[index].ChangeAmount._ChangeMaxRound,
				ChangeStatEffect.StatType.Strength, player.Abilities[index].ChangeAmount._Amount));
		}

		public static void SwiftAuraAbilityInfo(Player player, int index) {
			string swiftAuraString = $"Swift Aura Amount: {player.Abilities[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			string swiftAuraInfoString = $"Dexterity is increased by {player.Abilities[index].ChangeAmount._Amount} for " +
				$"{player.Abilities[index].ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				swiftAuraInfoString);
		}

		public static void UseSwiftAura(Player player, int index) {
			DeductAbilityCost(player, index);

			const string swiftAuraString = "You generate a Swift Aura around yourself.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);

			int swiftAuraIndex = player.Effects.FindIndex(e => e.Name == player.Abilities[index].Name);
			if (swiftAuraIndex != -1) {
				player.Effects[swiftAuraIndex].IsEffectExpired = true;
			}

			player.Dexterity += player.Abilities[index].ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player.Effects.Add(new ChangeStatEffect(player.Abilities[index].Name, player.Abilities[index].ChangeAmount._ChangeMaxRound,
				ChangeStatEffect.StatType.Dexterity, player.Abilities[index].ChangeAmount._Amount));
		}

		public static void WarCryAbilityInfo(Player player, int index) {
			string warCryString = $"War Cry Amount: {-1 * player.Abilities[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				warCryString);
			string warCryInfoString = $"Opponent's attacks are decreased by {-1 * player.Abilities[index].ChangeAmount._Amount} for " +
				$"{player.Abilities[index].ChangeAmount._ChangeMaxRound} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				warCryInfoString);
		}

		public static void UseWarCry(Player player, int index) {
			DeductAbilityCost(player, index);

			const string warCryString = "You shout a War Cry, intimidating your opponent, and decreasing incoming damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				warCryString);

			player.Effects.Add(new ChangeMonsterDamageEffect(player.Abilities[index].Name, player.Abilities[index].ChangeAmount._ChangeMaxRound,
				player.Abilities[index].ChangeAmount._Amount));
		}

		public static void StunAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player.Abilities[index].Stun._DamageAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"Stuns opponent for {player.Abilities[index].Stun._StunMaxRounds} rounds, preventing their attacks.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
		}

		public static void UseStunAbility(Monster monster, Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && PlayerController.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(monster);
				return;
			}

			DeductAbilityCost(player, index);

			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}

			int abilityDamage = player.Abilities[index].Stun._DamageAmount;

			string attackSuccessString = $"You {player.Abilities[index].Name} the {monster.Name} for {abilityDamage} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster.HitPoints -= abilityDamage;

			string stunString = $"The {monster.Name} is stunned!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunString);

			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				MaxRound = player.Abilities[index].Stun._StunMaxRounds,
				Name = player.Abilities[index].Name
			};
			monster.Effects.Add(new StunnedEffect(effectSettings));
		}

		public static void BerserkAbilityInfo(Player player, int index) {
			string dmgIncreaseString = $"Damage Increase: {player.Abilities[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgIncreaseString);
			string armDecreaseString = $"Armor Decrease: {player.Abilities[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				armDecreaseString);
			string dmgInfoString = $"Damage increased at cost of armor decrease for {player.Abilities[index].ChangeAmount._ChangeMaxRound} rounds";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgInfoString);
		}

		public static void UseBerserkAbility(Player player, int index) {
			DeductAbilityCost(player, index);

			const string berserkString = "You go into a berserk rage!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				berserkString);

			player.Effects.Add(new ChangePlayerDamageEffect($"{player.Abilities[index].Name} Damage Increase",
				player.Abilities[index].ChangeAmount._ChangeMaxRound, player.Abilities[index].Offensive._Amount));
			player.Effects.Add(new ChangeArmorEffect($"{player.Abilities[index].Name} Armor Decrease",
				player.Abilities[index].ChangeAmount._ChangeMaxRound, player.Abilities[index].ChangeAmount._Amount));
		}

		public static void DistanceAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player.Abilities[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"{player.Abilities[index].Offensive._ChanceToSucceed}% chance to hit monster in attack direction.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Usage example if monster is in room to north. 'use distance north'");
		}
		public static void UseDistanceAbility(Player player, int index, string direction) {
			int targetX = player.PlayerLocation.X;
			int targetY = player.PlayerLocation.Y;
			int targetZ = player.PlayerLocation.Z;
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
			Coordinate findCoord = new Coordinate(targetX, targetY, targetZ);
			IRoom room;
			try {
				room = RoomController._Rooms[findCoord];
			} catch (KeyNotFoundException) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack in that direction!");
				return;
			}
			DungeonRoom opponentRoom = RoomController._Rooms[findCoord] as DungeonRoom;
			Monster opponent = opponentRoom?._Monster;
			if (opponent == null) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"There is no monster in that direction to attack!");
				return;
			}
			DeductAbilityCost(player, index);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}
			int successChance = GameController.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive._ChanceToSucceed) {
				string attackString = $"You tried to shoot {opponent.Name} from afar but failed!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					attackString);
			} else {
				Settings.FormatAttackSuccessText();
				opponent.HitPoints -= player.Abilities[index].Offensive._Amount;
				string shootString = $"You successfully shot {opponent.Name} from afar for {player.Abilities[index].Offensive._Amount} damage!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					shootString);
				if (opponent.HitPoints <= 0) {
					opponent.MonsterDeath(player);
				}
			}
		}
		public static void DefenseAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Block Damage: {player.Abilities[index].Defensive._BlockDamage}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				blockInfoString);
		}

		public static void UseDefenseAbility(Player player, int index) {
			DeductAbilityCost(player, index);

			int blockAmount = player.Abilities[index].Defensive._BlockDamage;

			string blockString = $"You start blocking your opponent's attacks! You will block {blockAmount} damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);

			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = blockAmount,
				EffectHolder = player,
				Name = player.Abilities[index].Name
			};
			player.Effects.Add(new BlockDamageEffect(effectAmountSettings));
		}

		public static void BandageAbilityInfo(Player player, int index) {
			string healAmountString = $"Heal Amount: {player.Abilities[index].Healing._HealAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player.Abilities[index].Healing._HealOverTime <= 0) {
				return;
			}

			string healOverTimeString = $"Heal Over Time: {player.Abilities[index].Healing._HealOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			string healInfoStringCombat = $"Heal over time will restore health for {player.Abilities[index].Healing._HealMaxRounds} rounds in combat.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringCombat);
			string healInfoStringNonCombat = $"Heal over time will restore health {player.Abilities[index].Healing._HealMaxRounds} times every 10 seconds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringNonCombat);
		}

		public static void UseBandageAbility(Player player, int index) {
			DeductAbilityCost(player, index);

			int healAmount = player.Abilities[index].Healing._HealAmount;

			string healString = $"You heal yourself for {healAmount} health.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healString);

			if (player.HitPoints + healAmount > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			} else {
				player.HitPoints += healAmount;
			}

			if (player.Abilities[index].Healing._HealOverTime <= 0) {
				return;
			}

			player.Effects.Add(new HealingEffect(player.Abilities[index].Name, player.Abilities[index].Healing._HealMaxRounds,
				player.Abilities[index].Healing._HealOverTime));
		}

		public static void DisarmAbilityInfo(Player player, int index) {
			string abilityString = $"{player.Abilities[index].Offensive._Amount}% chance to disarm opponent's weapon.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityString);
		}
		public static void UseDisarmAbility(Monster opponent, Player player, int index) {
			DeductAbilityCost(player, index);
			int successChance = GameController.GetRandomNumber(1, 100);
			if (successChance > player.Abilities[index].Offensive._Amount) {
				string disarmFailString = $"You tried to disarm {opponent.Name} but failed!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					disarmFailString);
			} else {
				opponent.MonsterWeapon.Equipped = false;
				string disarmSuccessString = $"You successfully disarmed {opponent.Name}!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					disarmSuccessString);
			}
		}
		public static void OffenseDamageAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player.Abilities[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			if (player.Abilities[index].ArcAbilityCategory == ArcherAbility.Double) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.");
			}
			if (player.Abilities[index].WarAbilityCategory == WarriorAbility.Onslaught) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two attacks are launched which each cause instant damage. Cost and damage are per attack.");
			}
			if (player.Abilities[index].ArcAbilityCategory == ArcherAbility.Ambush) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"A surprise attack is launched, which initiates combat.");
			}
			if (player.Abilities[index].Offensive._AmountOverTime <= 0) {
				return;
			}

			string dmgOverTimeString = $"Damage Over Time: {player.Abilities[index].Offensive._AmountOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgOverTimeString);
			switch (player.Abilities[index].Offensive._OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedOverTimeString = $"Bleeding damage over time for {player.Abilities[index].Offensive._AmountMaxRounds} rounds.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						bleedOverTimeString);
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"Fire damage over time for {player.Abilities[index].Offensive._AmountMaxRounds} rounds.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						onFireString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void UseOffenseDamageAbility(Monster monster, Player player, int index) {
			if (player.PlayerClass == Player.PlayerClassType.Archer && PlayerController.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(monster);
				return;
			}

			DeductAbilityCost(player, index);

			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				player.PlayerQuiver.UseArrow();
			}

			int abilityDamage = PlayerController.CalculateAbilityDamage(player, monster, index);
			monster.HitPoints -= abilityDamage;

			string abilitySuccessString = $"Your {player.Abilities[index].Name} hit the {monster.Name} for {abilityDamage} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				abilitySuccessString);

			if (player.Abilities[index].Offensive._AmountOverTime <= 0) {
				return;
			}

			switch (player.Abilities[index].Offensive._OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedString = $"The {monster.Name} is bleeding!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);

					EffectOverTimeSettings effectSettings = new EffectOverTimeSettings {
						AmountOverTime = player.Abilities[index].Offensive._AmountOverTime,
						EffectHolder = monster,
						MaxRound = player.Abilities[index].Offensive._AmountMaxRounds,
						Name = player.Abilities[index].Name
					};
					monster.Effects.Add(new BleedingEffect(effectSettings));
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"The {monster.Name} bursts into flame!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatOnFireText(),
						Settings.FormatDefaultBackground(),
						onFireString);

					monster.Effects.Add(new BurningEffect(player.Abilities[index].Name, player.Abilities[index].Offensive._AmountMaxRounds,
						player.Abilities[index].Offensive._AmountOverTime));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}