using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class ArcAbilityUnitTests
	{
		[Test]
		public void DistanceShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			RoomHandler.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(0, 0, 0), new DungeonRoom(1, 1)},
				{new Coordinate(0, 1, 0), new DungeonRoom(1, 1)},
				{new Coordinate(1, 0, 0), new DungeonRoom(1, 1)}
			};
			var roomOneCoord = new Coordinate(0, 1, 0);
			var roomTwoCoord = new Coordinate(1, 0, 0);
			RoomHandler.Rooms[roomTwoCoord]._Monster = null;
			RoomHandler.Rooms[roomOneCoord]._Monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			var monster = RoomHandler.Rooms[roomOneCoord]._Monster;
			MonsterBuilder.BuildMonster(monster);
			RoomHandler.SetPlayerLocation(player, 0, 0, 0);
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] { "ability", "distance" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Distance Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("50% chance to hit monster in attack direction.",
				OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Usage example if monster is in room to north. 'use distance north'",
				OutputHandler.Display.Output[5][2]);
			var input = new[] { "use", "distance", "south" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("distance south", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player._PlayerQuiver.Quantity);
			Assert.AreEqual("You can't attack in that direction!", OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			input = new[] { "use", "distance", "east" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player._PlayerQuiver.Quantity);
			Assert.AreEqual("There is no monster in that direction to attack!",
				OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			Assert.AreEqual(player._MaxComboPoints, player._ComboPoints);
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Distance);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			player._Abilities[abilityIndex].Offensive.ChanceToSucceed = 0;
			input = new[] { "use", "distance", "north" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var missString = "You tried to shoot " + monster._Name + " from afar but failed!";
			Assert.AreEqual(missString, OutputHandler.Display.Output[8][2]);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			player._Abilities[abilityIndex].Offensive.ChanceToSucceed = 100;
			player._ComboPoints = player._MaxComboPoints;
			arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var abilityDmg = player._Abilities[abilityIndex].Offensive.Amount;
			var shootString = "You successfully shot " + monster._Name + " from afar for " + abilityDmg + " damage!";
			Assert.AreEqual(shootString, OutputHandler.Display.Output[9][2]);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			Assert.AreEqual(monster._MaxHitPoints - abilityDmg, monster._HitPoints);
		}
		[Test]
		public void GutShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Gut);
			var inputInfo = new[] { "ability", "gut" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Gut Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " +
									  player._Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new[] { "use", "gut" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("gut", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			var abilityDamage = player._Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = player._Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = player._Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = player._Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var abilitySuccessString = "Your " + player._Abilities[abilityIndex].Name + " hit the " + monster._Name + " for " +
									   abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[6][2]);
			var bleedString = "The " + monster._Name + " is bleeding!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				monster._Effects[0].BleedingRound(monster);
				var bleedAmount = monster._Effects[0]._EffectAmountOverTime;
				var bleedRoundString = "The " + monster._Name + " bleeds for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				monster._HitPoints);
		}
		[Test]
		public void PreciseShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var inputInfo = new[] { "ability", "precise" };
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Precise);
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Precise Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			var input = new[] { "use", "precise" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("precise", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			var abilityDamage = player._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - abilityDamage);
			var abilitySuccessString = "Your " + player._Abilities[abilityIndex].Name + " hit the " + monster._Name + " for " +
									   abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void StunShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Stun);
			var inputInfo = new[] { "ability", "stun" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Stun Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			var abilityInfoString = "Stuns opponent for " +
									player._Abilities[abilityIndex].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "stun" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			var abilityDamage = player._Abilities[abilityIndex].Stun.DamageAmount;
			var abilityCurRounds = player._Abilities[abilityIndex].Stun.StunCurRounds;
			var abilityMaxRounds = player._Abilities[abilityIndex].Stun.StunMaxRounds;
			var attackSuccessString = "You " + player._Abilities[abilityIndex].Name + " the " + monster._Name + " for " +
									  abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[5][2]);
			var stunString = "The " + monster._Name + " is stunned!";
			Assert.AreEqual(stunString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				monster._Effects[0].StunnedRound(monster);
				var stunnedString = "The " + monster._Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
		}
		[Test]
		public void WoundShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Wound);
			var inputInfo = new[] { "ability", "wound" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Wound Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 5", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 10", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " +
									  player._Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new[] { "use", "wound" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("wound", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			var abilityDamage = player._Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = player._Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = player._Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = player._Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var abilitySuccessString = "Your " + player._Abilities[abilityIndex].Name + " hit the " + monster._Name + " for " +
									   abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[6][2]);
			var bleedString = "The " + monster._Name + " is bleeding!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage, monster._HitPoints);
			Assert.AreEqual(abilityCurRounds, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster._Effects[0]._EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 7; i++)
			{
				monster._Effects[0].BleedingRound(monster);
				var bleedAmount = monster._Effects[0]._EffectAmountOverTime;
				var bleedRoundString = "The " + monster._Name + " bleeds for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(monster._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				monster._HitPoints);
		}
		[Test]
		public void DoubleShotAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Double);
			var inputInfo = new[] { "ability", "double" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Double Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual(
				"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "double" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("double", abilityName);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 2, player._PlayerQuiver.Quantity);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			var hitAmount = player._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster._MaxHitPoints - 2 * hitAmount, monster._HitPoints);
			Assert.AreEqual(player._MaxComboPoints - 2 * comboCost, player._ComboPoints);
			var attackString = "Your double shot hit the " + monster._Name + " for 25" + " physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			player._MaxComboPoints = 25;
			player._ComboPoints = player._MaxComboPoints;
			monster._MaxHitPoints = 100;
			monster._HitPoints = monster._MaxHitPoints;
			arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[7][2]);
			const string outOfComboString = "You didn't have enough combo points for the second shot!";
			Assert.AreEqual(outOfComboString, OutputHandler.Display.Output[8][2]);
		}
		[Test]
		public void BandageAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			player._Abilities.Add(new PlayerAbility(
				"bandage", 25, 1, PlayerAbility.ArcherAbility.Bandage, 2));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Bandage);
			var inputInfo = new[] { "ability", "bandage" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputHandler.Display.Output[4][2]);
			var healInfoStringCombat = "Heal over time will restore health for " +
									   player._Abilities[abilityIndex].Healing.HealMaxRounds + " rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputHandler.Display.Output[5][2]);
			var healInfoStringNonCombat = "Heal over time will restore health " +
										  player._Abilities[abilityIndex].Healing.HealMaxRounds + " times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputHandler.Display.Output[6][2]);
			var input = new[] { "use", "bandage" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			var baseHitPoints = player._HitPoints;
			player.UseAbility(input);
			var comboCost = player._Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player._MaxComboPoints - comboCost, player._ComboPoints);
			var healAmount = player._Abilities[abilityIndex].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player._Effects[0]._EffectGroup);
			Assert.AreEqual(baseHitPoints + healAmount, player._HitPoints);
			baseHitPoints = player._HitPoints;
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].HealingRound(player);
				var healOverTimeAmt = player._Effects[0]._EffectAmountOverTime;
				var healAmtString = "You have been healed for " + healOverTimeAmt + " health.";
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + (i - 1) * healOverTimeAmt, player._HitPoints);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void SwiftAuraAbilityUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			player._Abilities.Add(new PlayerAbility(
				"swift aura", 150, 1, PlayerAbility.ArcherAbility.SwiftAura, 6));
			var input = new[] { "use", "swift", "aura" };
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Swift Aura", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Swift Aura Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("_Dexterity is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			var baseDex = player._Dexterity;
			var baseCombo = player._ComboPoints;
			var baseMaxCombo = player._MaxComboPoints;
			var abilityIndex = player._Abilities.FindIndex(f => f.Name == InputHandler.ParseInput(input));
			player.UseAbility(input);
			Assert.AreEqual(player._Dexterity, baseDex + player._Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseCombo - player._Abilities[abilityIndex].ComboCost, player._ComboPoints);
			Assert.AreEqual(
				player._MaxComboPoints, baseMaxCombo + player._Abilities[abilityIndex].ChangeAmount.Amount * 10);
			Assert.AreEqual("You generate a Swift Aura around yourself.", OutputHandler.Display.Output[5][2]);
			for (var i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseDex, player._Dexterity);
			Assert.AreEqual(baseMaxCombo, player._MaxComboPoints);
			Assert.AreEqual(baseCombo - player._Abilities[abilityIndex].ComboCost, player._ComboPoints);
		}
		[Test]
		public void ImmolatingArrowAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10,
				_InCombat = true
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _InCombat = true, _FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player._PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player._Abilities.Add(new PlayerAbility(
				"immolating arrow", 35, 1, PlayerAbility.ArcherAbility.ImmolatingArrow, 8));
			var input = new[] { "use", "immolating", "arrow" };
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Immolating Arrow", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 35", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time for 3 rounds.", OutputHandler.Display.Output[5][2]);
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			var index = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.ImmolatingArrow);
			Assert.AreEqual(player._ComboPoints, player._MaxComboPoints - player._Abilities[index].ComboCost);
			var attackString = "Your immolating arrow hit the " + monster._Name + " for 25 physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster._HitPoints,
				monster._MaxHitPoints - player._Abilities[index].Offensive.Amount);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(Effect.EffectType.OnFire, monster._Effects[0]._EffectGroup);
			Assert.AreEqual(3, monster._Effects[0]._EffectMaxRound);
			for (var i = 0; i < 3; i++)
			{
				var baseHitPoints = monster._HitPoints;
				monster._Effects[0].OnFireRound(monster);
				Assert.AreEqual(i + 2, monster._Effects[0]._EffectCurRound);
				Assert.AreEqual(monster._HitPoints, baseHitPoints - monster._Effects[0]._EffectAmountOverTime);
				var burnString = "The " + monster._Name + " burns for " +
								 monster._Effects[0]._EffectAmountOverTime + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i][2]);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
		}
		[Test]
		public void AmbushAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxComboPoints = 100,
				_ComboPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			player._Abilities.Add(new PlayerAbility(
				"ambush", 75, 1, PlayerAbility.ArcherAbility.Ambush, 4));
			var inputInfo = new[] { "ability", "ambush" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Ambush", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 75", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("A surprise attack is launched, which initiates combat.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "ambush", monster._Name };
			player._InCombat = true;
			var arrowCount = player._PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual("You can't ambush " + monster._Name + ", you're already in combat!",
				OutputHandler.Display.Output[5][2]);
			player._InCombat = false;
			player.UseAbility(monster, input);
			var index = player._Abilities.FindIndex(
				f => f.ArcAbilityCategory == PlayerAbility.ArcherAbility.Ambush);
			var abilityDamage = player._Abilities[index].Offensive.Amount;
			var attackString = "Your ambush hit the " + monster._Name + " for " + abilityDamage + " physical damage.";
			Assert.AreEqual(arrowCount - 1, player._PlayerQuiver.Quantity);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - abilityDamage);
		}
	}
}