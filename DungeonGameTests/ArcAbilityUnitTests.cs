using DungeonGame;
using DungeonGame.Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DungeonGameTests
{
	public class ArcAbilityUnitTests
	{
		[Test]
		public void DistanceShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			RoomController._Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(0, 0, 0), new DungeonRoom(1, 1)},
				{new Coordinate(0, 1, 0), new DungeonRoom(1, 1)},
				{new Coordinate(1, 0, 0), new DungeonRoom(1, 1)}
			};
			Coordinate roomOneCoord = new Coordinate(0, 1, 0);
			Coordinate roomTwoCoord = new Coordinate(1, 0, 0);
			RoomController._Rooms[roomTwoCoord]._Monster = null;
			RoomController._Rooms[roomOneCoord]._Monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			Monster monster = RoomController._Rooms[roomOneCoord]._Monster;
			MonsterBuilder.BuildMonster(monster);
			RoomController.SetPlayerLocation(player, 0, 0, 0);
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			string[] inputInfo = new[] { "ability", "distance" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Distance Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputController.Display.Output[3][2]);
			Assert.AreEqual("50% chance to hit monster in attack direction.",
				OutputController.Display.Output[4][2]);
			Assert.AreEqual("Usage example if monster is in room to north. 'use distance north'",
				OutputController.Display.Output[5][2]);
			string[] input = new[] { "use", "distance", "south" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("distance south", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player._PlayerQuiver._Quantity);
			Assert.AreEqual("You can't attack in that direction!", OutputController.Display.Output[6][2]);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			input = new[] { "use", "distance", "east" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player._PlayerQuiver._Quantity);
			Assert.AreEqual("There is no monster in that direction to attack!",
				OutputController.Display.Output[7][2]);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Distance);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			player._Abilities[abilityIndex]._Offensive._ChanceToSucceed = 0;
			input = new[] { "use", "distance", "north" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			string missString = $"You tried to shoot {monster._Name} from afar but failed!";
			Assert.AreEqual(missString, OutputController.Display.Output[8][2]);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			player._Abilities[abilityIndex]._Offensive._ChanceToSucceed = 100;
			player._ComboPoints = player._MaxComboPoints;
			arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int abilityDmg = player._Abilities[abilityIndex]._Offensive._Amount;
			string shootString = $"You successfully shot {monster._Name} from afar for {abilityDmg} damage!";
			Assert.AreEqual(shootString, OutputController.Display.Output[9][2]);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			Assert.AreEqual(monster._MaxHitPoints - abilityDmg, monster._HitPoints);
		}
		[Test]
		public void GutShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in monster._MonsterItems.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Gut);
			string[] inputInfo = new[] { "ability", "gut" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Gut Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputController.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputController.Display.Output[4][2]);
			string bleedOverTimeString = $"Bleeding damage over time for {player._Abilities[abilityIndex]._Offensive._AmountMaxRounds} rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputController.Display.Output[5][2]);
			string[] input = new[] { "use", "gut" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("gut", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			int abilityDamage = player._Abilities[abilityIndex]._Offensive._Amount;
			int abilityDamageOverTime = player._Abilities[abilityIndex]._Offensive._AmountOverTime;
			int abilityCurRounds = player._Abilities[abilityIndex]._Offensive._AmountCurRounds;
			int abilityMaxRounds = player._Abilities[abilityIndex]._Offensive._AmountMaxRounds;
			string abilitySuccessString = $"Your {player._Abilities[abilityIndex]._Name} hit the {monster._Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputController.Display.Output[6][2]);
			string bleedString = $"The {monster._Name} is bleeding!";
			Assert.AreEqual(bleedString, OutputController.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++)
			{
				monster._Effects[0].BleedingRound(monster);
				int bleedAmount = monster._Effects[0]._EffectAmountOverTime;
				string bleedRoundString = $"The {monster._Name} bleeds for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				monster._HitPoints);
		}
		[Test]
		public void PreciseShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in monster._MonsterItems.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			string[] inputInfo = new[] { "ability", "precise" };
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Precise);
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Precise Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputController.Display.Output[3][2]);
			string[] input = new[] { "use", "precise" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("precise", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			int abilityDamage = player._Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - abilityDamage);
			string abilitySuccessString = $"Your {player._Abilities[abilityIndex]._Name} hit the {monster._Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputController.Display.Output[4][2]);
		}
		[Test]
		public void StunShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in monster._MonsterItems.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Stun);
			string[] inputInfo = new[] { "ability", "stun" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Stun Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputController.Display.Output[3][2]);
			string abilityInfoString = $"Stuns opponent for {player._Abilities[abilityIndex]._Stun._StunMaxRounds} rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputController.Display.Output[4][2]);
			string[] input = new[] { "use", "stun" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			int abilityDamage = player._Abilities[abilityIndex]._Stun._DamageAmount;
			int abilityCurRounds = player._Abilities[abilityIndex]._Stun._StunCurRounds;
			int abilityMaxRounds = player._Abilities[abilityIndex]._Stun._StunMaxRounds;
			string attackSuccessString = $"You {player._Abilities[abilityIndex]._Name} the {monster._Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[5][2]);
			string stunString = $"The {monster._Name} is stunned!";
			Assert.AreEqual(stunString, OutputController.Display.Output[6][2]);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++)
			{
				monster._Effects[0].StunnedRound(monster);
				string stunnedString = $"The {monster._Name} is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
		}
		[Test]
		public void WoundShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in monster._MonsterItems.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Wound);
			string[] inputInfo = new[] { "ability", "wound" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Wound Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 5", OutputController.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 10", OutputController.Display.Output[4][2]);
			string bleedOverTimeString = $"Bleeding damage over time for {player._Abilities[abilityIndex]._Offensive._AmountMaxRounds} rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputController.Display.Output[5][2]);
			string[] input = new[] { "use", "wound" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("wound", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			int abilityDamage = player._Abilities[abilityIndex]._Offensive._Amount;
			int abilityDamageOverTime = player._Abilities[abilityIndex]._Offensive._AmountOverTime;
			int abilityCurRounds = player._Abilities[abilityIndex]._Offensive._AmountCurRounds;
			int abilityMaxRounds = player._Abilities[abilityIndex]._Offensive._AmountMaxRounds;
			string abilitySuccessString = $"Your {player._Abilities[abilityIndex]._Name} hit the {monster._Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputController.Display.Output[6][2]);
			string bleedString = $"The {monster._Name} is bleeding!";
			Assert.AreEqual(bleedString, OutputController.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 7; i++)
			{
				monster._Effects[0].BleedingRound(monster);
				int bleedAmount = monster._Effects[0]._EffectAmountOverTime;
				string bleedRoundString = $"The {monster._Name} bleeds for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(monster);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				monster._HitPoints);
		}
		[Test]
		public void DoubleShotAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Double);
			string[] inputInfo = new[] { "ability", "double" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Double Shot", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputController.Display.Output[3][2]);
			Assert.AreEqual(
				"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.",
				OutputController.Display.Output[4][2]);
			string[] input = new[] { "use", "double" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("double", abilityName);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 2, player._PlayerQuiver._Quantity);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			int hitAmount = player._Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(monster._MaxHitPoints - (2 * hitAmount), monster._HitPoints);
			Assert.AreEqual(player._MaxComboPoints - (2 * comboCost), player._ComboPoints);
			string attackString = $"Your double shot hit the {monster._Name} for 25 physical damage.";
			Assert.AreEqual(attackString, OutputController.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputController.Display.Output[6][2]);
			player._MaxComboPoints = 25;
			player._ComboPoints = player._MaxComboPoints;
			monster._MaxHitPoints = 100;
			monster._HitPoints = monster._MaxHitPoints;
			arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			Assert.AreEqual(attackString, OutputController.Display.Output[7][2]);
			const string outOfComboString = "You didn't have enough combo points for the second shot!";
			Assert.AreEqual(outOfComboString, OutputController.Display.Output[8][2]);
		}
		[Test]
		public void BandageAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			player._Abilities.Add(new PlayerAbility(
				"bandage", 25, 1, PlayerAbility.ArcherAbility.Bandage, 2));
			OutputController.Display.ClearUserOutput();
			int abilityIndex = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Bandage);
			string[] inputInfo = new[] { "ability", "bandage" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputController.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputController.Display.Output[4][2]);
			string healInfoStringCombat = $"Heal over time will restore health for {player._Abilities[abilityIndex]._Healing._HealMaxRounds} rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputController.Display.Output[5][2]);
			string healInfoStringNonCombat = $"Heal over time will restore health {player._Abilities[abilityIndex]._Healing._HealMaxRounds} times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputController.Display.Output[6][2]);
			string[] input = new[] { "use", "bandage" };
			string abilityName = InputController.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			int baseHitPoints = player._HitPoints;
			player.UseAbility(input);
			int? comboCost = player._Abilities[abilityIndex]._ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			int healAmount = player._Abilities[abilityIndex]._Healing._HealAmount;
			string healString = $"You heal yourself for {healAmount} health.";
			Assert.AreEqual(healString, OutputController.Display.Output[7][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player._Effects[0]._EffectGroup);
			Assert.AreEqual(baseHitPoints + healAmount, player._HitPoints);
			baseHitPoints = player._HitPoints;
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++)
			{
				player._Effects[0].HealingRound(player);
				int healOverTimeAmt = player._Effects[0]._EffectAmountOverTime;
				string healAmtString = $"You have been healed for {healOverTimeAmt} health.";
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				Assert.AreEqual(healAmtString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + ((i - 1) * healOverTimeAmt), player._HitPoints);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void SwiftAuraAbilityUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Archer);
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			player._Abilities.Add(new PlayerAbility(
				"swift aura", 150, 1, PlayerAbility.ArcherAbility.SwiftAura, 6));
			string[] input = new[] { "use", "swift", "aura" };
			PlayerController.AbilityInfo(player, input);
			Assert.AreEqual("Swift Aura", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 150", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Swift Aura Amount: 15", OutputController.Display.Output[3][2]);
			Assert.AreEqual("Dexterity is increased by 15 for 10 minutes.", OutputController.Display.Output[4][2]);
			int baseDex = player._Dexterity;
			int? baseCombo = player._ComboPoints;
			int? baseMaxCombo = player._MaxComboPoints;
			int abilityIndex = player._Abilities.FindIndex(f => f._Name == InputController.ParseInput(input));
			player.UseAbility(input);
			Assert.AreEqual(player._Dexterity, baseDex + player._Abilities[abilityIndex]._ChangeAmount._Amount);
			Assert.AreEqual(
				baseCombo - player._Abilities[abilityIndex]._ComboCost, player._ComboPoints);
			Assert.AreEqual(
				player._MaxComboPoints, baseMaxCombo + (player._Abilities[abilityIndex]._ChangeAmount._Amount * 10));
			Assert.AreEqual("You generate a Swift Aura around yourself.", OutputController.Display.Output[5][2]);
			for (int i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseDex, player._Dexterity);
			Assert.AreEqual(baseMaxCombo, player._MaxComboPoints);
			Assert.AreEqual(baseCombo - player._Abilities[abilityIndex]._ComboCost, player._ComboPoints);
		}
		[Test]
		public void ImmolatingArrowAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10,
				_InCombat = true
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true, _FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player._PlayerWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player._Abilities.Add(new PlayerAbility(
				"immolating arrow", 35, 1, PlayerAbility.ArcherAbility.ImmolatingArrow, 8));
			string[] input = new[] { "use", "immolating", "arrow" };
			PlayerController.AbilityInfo(player, input);
			Assert.AreEqual("Immolating Arrow", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 35", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputController.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputController.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time for 3 rounds.", OutputController.Display.Output[5][2]);
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			int index = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.ImmolatingArrow);
			Assert.AreEqual(player._ComboPoints, player._MaxComboPoints - player._Abilities[index]._ComboCost);
			string attackString = $"Your immolating arrow hit the {monster._Name} for 25 physical damage.";
			Assert.AreEqual(attackString, OutputController.Display.Output[6][2]);
			Assert.AreEqual(monster._HitPoints,
				monster._MaxHitPoints - player._Abilities[index]._Offensive._Amount);
			OutputController.Display.ClearUserOutput();
			Assert.AreEqual(Effect.EffectType.OnFire, monster._Effects[0]._EffectGroup);
			Assert.AreEqual(3, monster._Effects[0]._EffectMaxRound);
			for (int i = 0; i < 3; i++)
			{
				int baseHitPoints = monster._HitPoints;
				monster._Effects[0].OnFireRound(monster);
				Assert.AreEqual(i + 2, monster._Effects[0]._EffectCurRound);
				Assert.AreEqual(monster._HitPoints, baseHitPoints - monster._Effects[0]._EffectAmountOverTime);
				string burnString = $"The {monster._Name} burns for {monster._Effects[0]._EffectAmountOverTime} fire damage.";
				Assert.AreEqual(burnString, OutputController.Display.Output[i][2]);
				GameController.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
		}
		[Test]
		public void AmbushAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			player._Abilities.Add(new PlayerAbility(
				"ambush", 75, 1, PlayerAbility.ArcherAbility.Ambush, 4));
			string[] inputInfo = new[] { "ability", "ambush" };
			PlayerController.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Ambush", OutputController.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 75", OutputController.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputController.Display.Output[3][2]);
			Assert.AreEqual("A surprise attack is launched, which initiates combat.",
				OutputController.Display.Output[4][2]);
			string[] input = new[] { "use", "ambush", monster._Name };
			player._InCombat = true;
			int arrowCount = player._PlayerQuiver._Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual($"You can't ambush {monster._Name}, you're already in combat!",
				OutputController.Display.Output[5][2]);
			player._InCombat = false;
			player.UseAbility(monster, input);
			int index = player._Abilities.FindIndex(
				f => f._ArcAbilityCategory == PlayerAbility.ArcherAbility.Ambush);
			int abilityDamage = player._Abilities[index]._Offensive._Amount;
			string attackString = "Your ambush hit the " + monster._Name + " for " + abilityDamage + " physical damage.";
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver._Quantity);
			Assert.AreEqual(attackString, OutputController.Display.Output[6][2]);
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - abilityDamage);
		}
	}
}