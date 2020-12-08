using System;
using System.Collections.Generic;

namespace DungeonGame
{
	public class PlayerAbility
	{
		public enum DamageType
		{
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum WarriorAbility
		{
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
		public enum ArcherAbility
		{
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
		public PlayerAbility(string name, int rageCost, int rank, WarriorAbility warAbility, int minLevel)
		{
			_Name = name;
			_RageCost = rageCost;
			_Rank = rank;
			_WarAbilityCategory = warAbility;
			_MinLevel = minLevel;
			switch (_WarAbilityCategory)
			{
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
		public PlayerAbility(string name, int comboCost, int rank, ArcherAbility archerAbility, int minLevel)
		{
			_Name = name;
			_ComboCost = comboCost;
			_Rank = rank;
			_ArcAbilityCategory = archerAbility;
			_MinLevel = minLevel;
			switch (_ArcAbilityCategory)
			{
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

		private static void DeductAbilityCost(Player player, int index)
		{
			if (player._PlayerClass == Player.PlayerClassType.Warrior)
			{
				player._RagePoints -= player._Abilities[index]._RageCost;
			}
			else
			{
				player._ComboPoints -= player._Abilities[index]._ComboCost;
			}
		}
		public static void PowerAuraAbilityInfo(Player player, int index)
		{
			string powerAuraString = $"Power Aura Amount: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);
			string powerAuraInfoString = $"Strength is increased by {player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				powerAuraInfoString);
		}
		public static void UsePowerAura(Player player, int index)
		{
			DeductAbilityCost(player, index);
			const string powerAuraString = "You generate a Power Aura around yourself.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				powerAuraString);
			int powerAuraIndex = player._Effects.FindIndex(e => e._Name == player._Abilities[index]._Name);
			if (powerAuraIndex != -1)
			{
				player._Effects[powerAuraIndex]._IsEffectExpired = true;
			}
			player._Strength += player._Abilities[index]._ChangeAmount._Amount;
			PlayerHandler.CalculatePlayerStats(player);
			player._Effects.Add(new Effect(player._Abilities[index]._Name, Effect.EffectType.ChangeStat,
				player._Abilities[index]._ChangeAmount._Amount, player._Abilities[index]._ChangeAmount._ChangeCurRound,
				player._Abilities[index]._ChangeAmount._ChangeMaxRound, 1, 1, false,
				ChangeStat.StatType.Strength));
		}
		public static void SwiftAuraAbilityInfo(Player player, int index)
		{
			string swiftAuraString = $"Swift Aura Amount: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			string swiftAuraInfoString = $"Dexterity is increased by {player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				swiftAuraInfoString);
		}
		public static void UseSwiftAura(Player player, int index)
		{
			DeductAbilityCost(player, index);
			const string swiftAuraString = "You generate a Swift Aura around yourself.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				swiftAuraString);
			int swiftAuraIndex = player._Effects.FindIndex(e => e._Name == player._Abilities[index]._Name);
			if (swiftAuraIndex != -1)
			{
				player._Effects[swiftAuraIndex]._IsEffectExpired = true;
			}
			player._Dexterity += player._Abilities[index]._ChangeAmount._Amount;
			PlayerHandler.CalculatePlayerStats(player);
			player._Effects.Add(new Effect(player._Abilities[index]._Name,
				Effect.EffectType.ChangeStat, player._Abilities[index]._ChangeAmount._Amount,
				player._Abilities[index]._ChangeAmount._ChangeCurRound, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				1, 1, false, ChangeStat.StatType.Dexterity));
		}
		public static void WarCryAbilityInfo(Player player, int index)
		{
			string warCryString = $"War Cry Amount: {-1 * player._Abilities[index]._ChangeAmount._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				warCryString);
			string warCryInfoString = $"Opponent's attacks are decreased by {-1 * player._Abilities[index]._ChangeAmount._Amount} for " +
				$"{player._Abilities[index]._ChangeAmount._ChangeMaxRound} rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				warCryInfoString);
		}
		public static void UseWarCry(Player player, int index)
		{
			DeductAbilityCost(player, index);
			const string warCryString = "You shout a War Cry, intimidating your opponent, and decreasing incoming damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				warCryString);
			player._Effects.Add(new Effect(player._Abilities[index]._Name,
				Effect.EffectType.ChangeOpponentDamage, player._Abilities[index]._ChangeAmount._Amount,
				player._Abilities[index]._ChangeAmount._ChangeCurRound, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				1, 1, false));
		}
		public static void StunAbilityInfo(Player player, int index)
		{
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Stun._DamageAmount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"Stuns opponent for {player._Abilities[index]._Stun._StunMaxRounds} rounds, preventing their attacks.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
		}
		public static void UseStunAbility(Monster opponent, Player player, int index)
		{
			if (player._PlayerClass == Player.PlayerClassType.Archer && PlayerHandler.OutOfArrows(player))
			{
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player._PlayerClass == Player.PlayerClassType.Archer)
			{
				player._PlayerQuiver.UseArrow();
			}
			int abilityDamage = player._Abilities[index]._Stun._DamageAmount;
			string attackSuccessString = $"You {player._Abilities[index]._Name} the {opponent._Name} for {abilityDamage} physical damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			opponent._HitPoints -= abilityDamage;
			string stunString = $"The {opponent._Name} is stunned!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunString);
			opponent._Effects.Add(new Effect(player._Abilities[index]._Name, Effect.EffectType.Stunned,
				player._Abilities[index]._Stun._StunCurRounds, player._Abilities[index]._Stun._StunMaxRounds,
				1, 1, true));
		}
		public static void BerserkAbilityInfo(Player player, int index)
		{
			string dmgIncreaseString = $"Damage Increase: {player._Abilities[index]._Offensive._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgIncreaseString);
			string armDecreaseString = $"Armor Decrease: {player._Abilities[index]._ChangeAmount._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				armDecreaseString);
			string dmgInfoString = $"Damage increased at cost of armor decrease for {player._Abilities[index]._ChangeAmount._ChangeMaxRound} rounds";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgInfoString);
		}
		public static void UseBerserkAbility(Player player, int index)
		{
			DeductAbilityCost(player, index);
			const string berserkString = "You go into a berserk rage!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				berserkString);
			player._Effects.Add(new Effect($"{player._Abilities[index]._Name} Damage Increase",
				Effect.EffectType.ChangePlayerDamage, player._Abilities[index]._Offensive._Amount,
				player._Abilities[index]._ChangeAmount._ChangeCurRound, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				1, 1, false));
			player._Effects.Add(new Effect($"{player._Abilities[index]._Name} Armor Decrease",
				Effect.EffectType.ChangeArmor, player._Abilities[index]._ChangeAmount._Amount,
				player._Abilities[index]._ChangeAmount._ChangeCurRound, player._Abilities[index]._ChangeAmount._ChangeMaxRound,
				1, 1, true));
		}
		public static void DistanceAbilityInfo(Player player, int index)
		{
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Offensive._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			string abilityInfoString = $"{player._Abilities[index]._Offensive._ChanceToSucceed}% chance to hit monster in attack direction.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityInfoString);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Usage example if monster is in room to north. 'use distance north'");
		}
		public static void UseDistanceAbility(Player player, int index, string direction)
		{
			int targetX = player._PlayerLocation._X;
			int targetY = player._PlayerLocation._Y;
			int targetZ = player._PlayerLocation._Z;
			switch (direction)
			{
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
			try
			{
				room = RoomHandler.Rooms[findCoord];
			}
			catch (KeyNotFoundException)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't attack in that direction!");
				return;
			}
			DungeonRoom opponentRoom = RoomHandler.Rooms[findCoord] as DungeonRoom;
			Monster opponent = opponentRoom?._Monster;
			if (opponent == null)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"There is no monster in that direction to attack!");
				return;
			}
			DeductAbilityCost(player, index);
			if (player._PlayerClass == Player.PlayerClassType.Archer)
			{
				player._PlayerQuiver.UseArrow();
			}
			int successChance = GameHandler.GetRandomNumber(1, 100);
			if (successChance > player._Abilities[index]._Offensive._ChanceToSucceed)
			{
				string attackString = $"You tried to shoot {opponent._Name} from afar but failed!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					attackString);
			}
			else
			{
				Settings.FormatAttackSuccessText();
				opponent._HitPoints -= player._Abilities[index]._Offensive._Amount;
				string shootString = $"You successfully shot {opponent._Name} from afar for {player._Abilities[index]._Offensive._Amount} damage!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					shootString);
				if (opponent._HitPoints <= 0)
				{
					opponent.MonsterDeath(player);
				}
			}
		}
		public static void DefenseAbilityInfo(Player player, int index)
		{
			string abilityDmgString = $"Block Damage: {player._Abilities[index]._Defensive._BlockDamage}";
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
		public static void UseDefenseAbility(Player player, int index)
		{
			DeductAbilityCost(player, index);
			int blockAmount = player._Abilities[index]._Defensive._BlockDamage;
			string blockString = $"You start blocking your opponent's attacks! You will block {blockAmount} damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);
			player._Effects.Add(new Effect(player._Abilities[index]._Name,
				Effect.EffectType.BlockDamage, blockAmount, 10));
		}
		public static void BandageAbilityInfo(Player player, int index)
		{
			string healAmountString = $"Heal Amount: {player._Abilities[index]._Healing._HealAmount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player._Abilities[index]._Healing._HealOverTime <= 0)
			{
				return;
			}

			string healOverTimeString = $"Heal Over Time: {player._Abilities[index]._Healing._HealOverTime}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			string healInfoStringCombat = $"Heal over time will restore health for {player._Abilities[index]._Healing._HealMaxRounds} rounds in combat.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringCombat);
			string healInfoStringNonCombat = $"Heal over time will restore health {player._Abilities[index]._Healing._HealMaxRounds} times every 10 seconds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoStringNonCombat);
		}
		public static void UseBandageAbility(Player player, int index)
		{
			DeductAbilityCost(player, index);
			int healAmount = player._Abilities[index]._Healing._HealAmount;
			string healString = $"You heal yourself for {healAmount} health.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healString);
			player._HitPoints += healAmount;
			if (player._HitPoints > player._MaxHitPoints)
			{
				player._HitPoints = player._MaxHitPoints;
			}
			if (player._Abilities[index]._Healing._HealOverTime <= 0)
			{
				return;
			}

			player._Effects.Add(new Effect(player._Abilities[index]._Name,
				Effect.EffectType.Healing, player._Abilities[index]._Healing._HealOverTime,
				player._Abilities[index]._Healing._HealCurRounds, player._Abilities[index]._Healing._HealMaxRounds,
				1, 10, false));
		}
		public static void DisarmAbilityInfo(Player player, int index)
		{
			string abilityString = $"{player._Abilities[index]._Offensive._Amount}% chance to disarm opponent's weapon.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityString);
		}
		public static void UseDisarmAbility(Monster opponent, Player player, int index)
		{
			DeductAbilityCost(player, index);
			int successChance = GameHandler.GetRandomNumber(1, 100);
			if (successChance > player._Abilities[index]._Offensive._Amount)
			{
				string disarmFailString = $"You tried to disarm {opponent._Name} but failed!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					disarmFailString);
			}
			else
			{
				opponent._MonsterWeapon._Equipped = false;
				string disarmSuccessString = $"You successfully disarmed {opponent._Name}!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					disarmSuccessString);
			}
		}
		public static void OffenseDamageAbilityInfo(Player player, int index)
		{
			string abilityDmgString = $"Instant Damage: {player._Abilities[index]._Offensive._Amount}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				abilityDmgString);
			if (player._Abilities[index]._ArcAbilityCategory == ArcherAbility.Double)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.");
			}
			if (player._Abilities[index]._WarAbilityCategory == WarriorAbility.Onslaught)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"Two attacks are launched which each cause instant damage. Cost and damage are per attack.");
			}
			if (player._Abilities[index]._ArcAbilityCategory == ArcherAbility.Ambush)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"A surprise attack is launched, which initiates combat.");
			}
			if (player._Abilities[index]._Offensive._AmountOverTime <= 0)
			{
				return;
			}

			string dmgOverTimeString = $"Damage Over Time: {player._Abilities[index]._Offensive._AmountOverTime}";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				dmgOverTimeString);
			switch (player._Abilities[index]._Offensive._OffensiveGroup)
			{
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedOverTimeString = $"Bleeding damage over time for {player._Abilities[index]._Offensive._AmountMaxRounds} rounds.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						bleedOverTimeString);
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"Fire damage over time for {player._Abilities[index]._Offensive._AmountMaxRounds} rounds.";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						onFireString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static void UseOffenseDamageAbility(Monster opponent, Player player, int index)
		{
			if (player._PlayerClass == Player.PlayerClassType.Archer && PlayerHandler.OutOfArrows(player))
			{
				/* If quiver is empty, player can only do a normal attack, and attack() also checks for
				 arrow count and informs player that they are out of arrows */
				player.PhysicalAttack(opponent);
				return;
			}
			DeductAbilityCost(player, index);
			if (player._PlayerClass == Player.PlayerClassType.Archer)
			{
				player._PlayerQuiver.UseArrow();
			}
			int abilityDamage = PlayerHandler.CalculateAbilityDamage(player, opponent, index);
			opponent._HitPoints -= abilityDamage;
			string abilitySuccessString = $"Your {player._Abilities[index]._Name} hit the {opponent._Name} for {abilityDamage} physical damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				abilitySuccessString);
			if (player._Abilities[index]._Offensive._AmountOverTime <= 0)
			{
				return;
			}

			switch (player._Abilities[index]._Offensive._OffensiveGroup)
			{
				case Offensive.OffensiveType.Normal:
					break;
				case Offensive.OffensiveType.Bleed:
					string bleedString = $"The {opponent._Name} is bleeding!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						bleedString);
					opponent._Effects.Add(new Effect(player._Abilities[index]._Name,
						Effect.EffectType.Bleeding, player._Abilities[index]._Offensive._AmountOverTime,
						player._Abilities[index]._Offensive._AmountCurRounds, player._Abilities[index]._Offensive._AmountMaxRounds,
						1, 1, true));
					break;
				case Offensive.OffensiveType.Fire:
					string onFireString = $"The {opponent._Name} bursts into flame!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatOnFireText(),
						Settings.FormatDefaultBackground(),
						onFireString);
					opponent._Effects.Add(new Effect(player._Abilities[index]._Name,
						Effect.EffectType.OnFire, player._Abilities[index]._Offensive._AmountOverTime,
						player._Abilities[index]._Offensive._AmountCurRounds, player._Abilities[index]._Offensive._AmountMaxRounds,
						1, 1, true));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}