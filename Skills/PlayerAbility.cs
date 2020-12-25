using DungeonGame.Controllers;
using DungeonGame.Effects;
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
		public string _Name { get; set; }
		public ArcherAbility? _ArcAbilityCategory { get; set; }
		public WarriorAbility? _WarAbilityCategory { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public Healing _Healing { get; set; }
		public Defensive _Defensive { get; set; }
		public Offensive _Offensive { get; set; }
		public ChangeAmount _ChangeAmount { get; set; }
		public Stun _Stun { get; set; }
		public int _MinLevel { get; set; }
		public int? _RageCost { get; set; }
		public int? _ComboCost { get; set; }
		public int _Rank { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public PlayerAbility() { }
		public PlayerAbility(string name, int rageCost, int rank, WarriorAbility warAbility, int minLevel) {
			_Name = name;
			_RageCost = rageCost;
			_Rank = rank;
			_WarAbilityCategory = warAbility;
			_MinLevel = minLevel;
			switch (_WarAbilityCategory) {
				case WarriorAbility.Slash:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(50);
					break;
				case WarriorAbility.Rend:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case WarriorAbility.Charge:
					_DamageGroup = DamageType.Physical;
					_Stun = new Stun(15, 1, 2);
					break;
				case WarriorAbility.Block:
					_Defensive = new Defensive(50);
					break;
				case WarriorAbility.Berserk:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(20, 1, 4);
					_ChangeAmount = new ChangeAmount(-15, 1, 4);
					break;
				case WarriorAbility.Disarm:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(35);
					break;
				case WarriorAbility.Bandage:
					_Healing = new Healing(25, 5, 1, 3);
					break;
				case WarriorAbility.PowerAura:
					_ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case WarriorAbility.WarCry:
					_ChangeAmount = new ChangeAmount(-25, 1, 3);
					break;
				case WarriorAbility.Onslaught:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(25);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public PlayerAbility(string name, int comboCost, int rank, ArcherAbility archerAbility, int minLevel) {
			_Name = name;
			_ComboCost = comboCost;
			_Rank = rank;
			_ArcAbilityCategory = archerAbility;
			_MinLevel = minLevel;
			switch (_ArcAbilityCategory) {
				case ArcherAbility.Distance:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(25, 50);
					break;
				case ArcherAbility.Gut:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(
						15, 5, 1, 3, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Precise:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(50);
					break;
				case ArcherAbility.Stun:
					_DamageGroup = DamageType.Physical;
					_Stun = new Stun(15, 1, 3);
					break;
				case ArcherAbility.Wound:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(
						5, 10, 1, 5, Offensive.OffensiveType.Bleed);
					break;
				case ArcherAbility.Double:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(25);
					break;
				case ArcherAbility.Bandage:
					_Healing = new Healing(25, 5, 1, 3);
					break;
				case ArcherAbility.SwiftAura:
					_ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case ArcherAbility.ImmolatingArrow:
					_DamageGroup = DamageType.Fire;
					_Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case ArcherAbility.Ambush:
					_DamageGroup = DamageType.Physical;
					_Offensive = new Offensive(50);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void DeductAbilityCost(Player player, int index) {
			if (player._PlayerClass == Player.PlayerClassType.Warrior) {
				player._RagePoints -= player._Abilities[index]._RageCost;
			} else {
				player._ComboPoints -= player._Abilities[index]._ComboCost;
			}
		}
		public static void PowerAuraAbilityInfo(Player player, int index) {
			string powerAuraString = $"Power Aura Amount: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);
			string powerAuraInfoString = $"Strength is increased by {player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound / 60} minutes.";
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

			int powerAuraIndex = player._Effects.FindIndex(e => e._Name == player._Abilities[index]._Name);
			if (powerAuraIndex != -1) {
				player._Effects[powerAuraIndex]._IsEffectExpired = true;
			}

			player._Strength += player._Abilities[index]._ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player._Effects.Add(new ChangeStatEffect(player._Abilities[index]._Name, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				ChangeStatEffect.StatType.Strength, player._Abilities[index]._ChangeAmount._Amount));
		}

		public static void SwiftAuraAbilityInfo(Player player, int index) {
			string swiftAuraString = $"Swift Aura Amount: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			string swiftAuraInfoString = $"Dexterity is increased by {player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound / 60} minutes.";
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

			int swiftAuraIndex = player._Effects.FindIndex(e => e._Name == player._Abilities[index]._Name);
			if (swiftAuraIndex != -1) {
				player._Effects[swiftAuraIndex]._IsEffectExpired = true;
			}

			player._Dexterity += player._Abilities[index]._ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player._Effects.Add(new ChangeStatEffect(player._Abilities[index]._Name, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				ChangeStatEffect.StatType.Dexterity, player._Abilities[index]._ChangeAmount._Amount));
		}

		public static void WarCryAbilityInfo(Player player, int index) {
			string warCryString = $"War Cry Amount: {-1 * player._Abilities[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				warCryString);
			string warCryInfoString = $"Opponent's attacks are decreased by {-1 * player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound} rounds.";
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

			player._Effects.Add(new ChangeMonsterDamageEffect(player._Abilities[index]._Name, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				player._Abilities[index]._ChangeAmount._Amount));
		}

		public static void StunAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Stun._DamageAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"Stuns opponent for {player._Abilities[index]._Stun._StunMaxRounds} rounds, preventing their attacks.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
		}

		public static void UseStunAbility(Monster monster, Player player, int index) {
			if (player._PlayerClass == Player.PlayerClassType.Archer && PlayerController.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(monster);
				return;
			}

			DeductAbilityCost(player, index);

			if (player._PlayerClass == Player.PlayerClassType.Archer) {
				player._PlayerQuiver.UseArrow();
			}

			int abilityDamage = player._Abilities[index]._Stun._DamageAmount;

			string attackSuccessString = $"You {player._Abilities[index]._Name} the {monster._Name} for {abilityDamage} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster._HitPoints -= abilityDamage;

			string stunString = $"The {monster._Name} is stunned!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunString);

			monster._Effects.Add(new StunnedEffect(player._Abilities[index]._Name, player._Abilities[index]._Stun._StunMaxRounds));
		}

		public static void BerserkAbilityInfo(Player player, int index) {
			string dmgIncreaseString = $"Damage Increase: {player._Abilities[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgIncreaseString);
			string armDecreaseString = $"Armor Decrease: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				armDecreaseString);
			string dmgInfoString = $"Damage increased at cost of armor decrease for {player._Abilities[index]._ChangeAmount._ChangeMaxRound} rounds";
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

			player._Effects.Add(new ChangePlayerDamageEffect($"{player._Abilities[index]._Name} Damage Increase",
				player._Abilities[index]._ChangeAmount._ChangeMaxRound, player._Abilities[index]._Offensive._Amount));
			player._Effects.Add(new ChangeArmorEffect($"{player._Abilities[index]._Name} Armor Decrease",
				player._Abilities[index]._ChangeAmount._ChangeMaxRound, player._Abilities[index]._ChangeAmount._Amount));
		}

		public static void DistanceAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"{player._Abilities[index]._Offensive._ChanceToSucceed}% chance to hit monster in attack direction.";
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
			int targetX = player._PlayerLocation._X;
			int targetY = player._PlayerLocation._Y;
			int targetZ = player._PlayerLocation._Z;
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
			if (player._PlayerClass == Player.PlayerClassType.Archer) {
				player._PlayerQuiver.UseArrow();
			}
			int successChance = GameController.GetRandomNumber(1, 100);
			if (successChance > player._Abilities[index]._Offensive._ChanceToSucceed) {
				string attackString = $"You tried to shoot {opponent._Name} from afar but failed!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					attackString);
			} else {
				Settings.FormatAttackSuccessText();
				opponent._HitPoints -= player._Abilities[index]._Offensive._Amount;
				string shootString = $"You successfully shot {opponent._Name} from afar for {player._Abilities[index]._Offensive._Amount} damage!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					shootString);
				if (opponent._HitPoints <= 0) {
					opponent.MonsterDeath(player);
				}
			}
		}
		public static void DefenseAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Block Damage: {player._Abilities[index]._Defensive._BlockDamage}";
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

			int blockAmount = player._Abilities[index]._Defensive._BlockDamage;

			string blockString = $"You start blocking your opponent's attacks! You will block {blockAmount} damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);

			player._Effects.Add(new BlockDamageEffect(player._Abilities[index]._Name, blockAmount));
		}

		public static void BandageAbilityInfo(Player player, int index) {
			string healAmountString = $"Heal Amount: {player._Abilities[index]._Healing._HealAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player._Abilities[index]._Healing._HealOverTime <= 0) {
				return;
			}

			string healOverTimeString = $"Heal Over Time: {player._Abilities[index]._Healing._HealOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			string healInfoStringCombat = $"Heal over time will restore health for {player._Abilities[index]._Healing._HealMaxRounds} rounds in combat.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringCombat);
			string healInfoStringNonCombat = $"Heal over time will restore health {player._Abilities[index]._Healing._HealMaxRounds} times every 10 seconds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringNonCombat);
		}

		public static void UseBandageAbility(Player player, int index) {
			DeductAbilityCost(player, index);

			int healAmount = player._Abilities[index]._Healing._HealAmount;

			string healString = $"You heal yourself for {healAmount} health.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healString);

			if (player._HitPoints + healAmount > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			} else {
				player._HitPoints += healAmount;
			}

			if (player._Abilities[index]._Healing._HealOverTime <= 0) {
				return;
			}

			player._Effects.Add(new HealingEffect(player._Abilities[index]._Name, player._Abilities[index]._Healing._HealMaxRounds, 
				player._Abilities[index]._Healing._HealOverTime));
		}

		public static void DisarmAbilityInfo(Player player, int index) {
			string abilityString = $"{player._Abilities[index]._Offensive._Amount}% chance to disarm opponent's weapon.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityString);
		}
		public static void UseDisarmAbility(Monster opponent, Player player, int index) {
			DeductAbilityCost(player, index);
			int successChance = GameController.GetRandomNumber(1, 100);
			if (successChance > player._Abilities[index]._Offensive._Amount) {
				string disarmFailString = $"You tried to disarm {opponent._Name} but failed!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					disarmFailString);
			} else {
				opponent._MonsterWeapon._Equipped = false;
				string disarmSuccessString = $"You successfully disarmed {opponent._Name}!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					disarmSuccessString);
			}
		}
		public static void OffenseDamageAbilityInfo(Player player, int index) {
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			if (player._Abilities[index]._ArcAbilityCategory == ArcherAbility.Double) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.");
			}
			if (player._Abilities[index]._WarAbilityCategory == WarriorAbility.Onslaught) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two attacks are launched which each cause instant damage. Cost and damage are per attack.");
			}
			if (player._Abilities[index]._ArcAbilityCategory == ArcherAbility.Ambush) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"A surprise attack is launched, which initiates combat.");
			}
			if (player._Abilities[index]._Offensive._AmountOverTime <= 0) {
				return;
			}

			string dmgOverTimeString = $"Damage Over Time: {player._Abilities[index]._Offensive._AmountOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgOverTimeString);
			switch (player._Abilities[index]._Offensive._OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedOverTimeString = $"Bleeding damage over time for {player._Abilities[index]._Offensive._AmountMaxRounds} rounds.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						bleedOverTimeString);
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"Fire damage over time for {player._Abilities[index]._Offensive._AmountMaxRounds} rounds.";
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
			if (player._PlayerClass == Player.PlayerClassType.Archer && PlayerController.OutOfArrows(player)) {
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(monster);
				return;
			}

			DeductAbilityCost(player, index);

			if (player._PlayerClass == Player.PlayerClassType.Archer) {
				player._PlayerQuiver.UseArrow();
			}

			int abilityDamage = PlayerController.CalculateAbilityDamage(player, monster, index);
			monster._HitPoints -= abilityDamage;

			string abilitySuccessString = $"Your {player._Abilities[index]._Name} hit the {monster._Name} for {abilityDamage} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				abilitySuccessString);

			if (player._Abilities[index]._Offensive._AmountOverTime <= 0) {
				return;
			}

			switch (player._Abilities[index]._Offensive._OffensiveGroup) {
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedString = $"The {monster._Name} is bleeding!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);

					monster._Effects.Add(new BleedingEffect(player._Abilities[index]._Name, player._Abilities[index]._Offensive._AmountMaxRounds,
						player._Abilities[index]._Offensive._AmountOverTime));
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"The {monster._Name} bursts into flame!";
					OutputController.Display.StoreUserOutput(
						Settings.FormatOnFireText(),
						Settings.FormatDefaultBackground(),
						onFireString);

					monster._Effects.Add(new BurningEffect(player._Abilities[index]._Name, player._Abilities[index]._Offensive._AmountMaxRounds,
						player._Abilities[index]._Offensive._AmountOverTime));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}