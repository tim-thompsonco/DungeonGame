using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class WarAbilityUnitTests {
		[Test]
		public void SlashAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var inputInfo = new[] {"ability", "slash"};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Slash);
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Slash", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			var input = new [] {"use", "slash"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("slash", abilityName);
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - player.Abilities[abilityIndex].RageCost, 
				player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			var abilitySuccessString = "Your " + player.Abilities[abilityIndex].Name + " hit the " + monster.Name + " for " +
			                           abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void RendAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Rend);
			var inputInfo = new[] {"ability", "rend"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Rend", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "rend"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = player.Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = player.Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = player.Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var abilitySuccessString = "Your " + player.Abilities[abilityIndex].Name + " hit the " + monster.Name + " for " +
			                           abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[6][2]);
			var bleedString = "The " + monster.Name + " is bleeding!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage,monster.HitPoints);
			Assert.AreEqual(abilityCurRounds,monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds,monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster);
				var bleedAmount = monster.Effects[0].EffectAmountOverTime;
				var bleedRoundString = "The " + monster.Name + " bleeds for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds, 
				monster.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Charge);
			var inputInfo = new[] {"ability", "charge"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Charge", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			var abilityInfoString = "Stuns opponent for " + 
			                        player.Abilities[abilityIndex].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "charge"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Stun.DamageAmount;
			var abilityCurRounds = player.Abilities[abilityIndex].Stun.StunCurRounds;
			var abilityMaxRounds = player.Abilities[abilityIndex].Stun.StunMaxRounds;
			var attackSuccessString = "You " +  player.Abilities[abilityIndex].Name  + " the " + monster.Name + " for " +
			                          abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[5][2]);
			var stunString = "The " + monster.Name + " is stunned!";
			Assert.AreEqual(stunString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage,monster.HitPoints);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].StunnedRound(monster);
				var stunnedString = "The " + monster.Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void BlockAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true, MaxHitPoints = 100, HitPoints = 100, DodgeChance = 0};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Block);
			var inputInfo = new[] {"ability", "block"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Block", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Block Damage: 50", OutputHandler.Display.Output[3][2]);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			Assert.AreEqual(blockInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "block"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("block", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var blockAmount = player.Abilities[abilityIndex].Defensive.BlockDamage;
			var blockString = "You start blocking your opponent's attacks! You will block " + blockAmount + " damage.";
			Assert.AreEqual(blockString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(
				true, player.Effects[0].EffectGroup == Effect.EffectType.BlockDamage);
			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			var blockAmountRemaining = player.Effects[0].EffectAmount;
			Assert.AreEqual(blockAmount, blockAmountRemaining);
			var combatSim = new CombatHandler(monster, player);
			var i = 0;
			while (blockAmountRemaining > 0) {
				var blockAmountBefore = blockAmountRemaining;
				combatSim.ProcessMonsterAttack();
				blockAmountRemaining = player.Effects.Any() ? player.Effects[0].EffectAmount : 0;
				Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
				var blockRoundString = "Your defensive move blocked " + (blockAmountBefore - blockAmountRemaining) + " damage!";
				Assert.AreEqual(blockRoundString, OutputHandler.Display.Output[i][2]);
				i++;
				GameHandler.RemovedExpiredEffects(player);
			}
			const string blockEndString = "You are no longer blocking damage!";
			Assert.AreEqual(blockEndString, OutputHandler.Display.Output[i][2]);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void BerserkAbilityUnitTest() {
			// Berserk should create a change damage and change armor effect, which should expire when combat ends
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			GearHandler.EquipInitialGear(player);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			var input = new [] {"use", "berserk"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("berserk", abilityName);
			player.UseAbility(monster, input);
			Assert.AreEqual(2, player.Effects.Count);
			player.InCombat = false;
			GameHandler.CheckStatus(player);
			Assert.AreEqual(0, player.Effects.Count);
		}
		[Test]
		public void BandageAbilityUnitTests() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			GearHandler.EquipInitialGear(player);
			player.Abilities.Add(
				new Ability("use bandage", 25, 1, Ability.ArcherAbility.Bandage, 1));
			player.HitPoints = 10;
			/* Bandage should heal 25 immediately, 5 over time, cur round 1, max round 3
			Make sure stacked healing effects only tick for 3 rounds in combat */
			player.InCombat = true;
			var input = new [] {"use", "bandage"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			player.UseAbility(input);
			player.UseAbility(input);
			Assert.AreEqual(60, player.HitPoints);
			for (var i = 0; i < 5; i++) {
				player.Effects[0].HealingRound(player);
				player.Effects[1].HealingRound(player);
				if (i <= 2) Assert.AreEqual(60 + (i + 1) * 10, player.HitPoints);
			}
			Assert.AreEqual(90, player.HitPoints);
			player.InCombat = false;
			// Make sure stacked healing effects tick properly outside of combat		
			player.HitPoints = 10;
			var inputTwo = new [] {"use", "bandage"};
			var abilityNameTwo = InputHandler.ParseInput(inputTwo);
			Assert.AreEqual("bandage", abilityName);
			player.UseAbility(inputTwo);
			player.UseAbility(inputTwo);
			Assert.AreEqual(60, player.HitPoints);
			foreach (var effect in player.Effects.Where(effect => effect.EffectGroup == Effect.EffectType.Healing)) {
				while (!effect.IsEffectExpired) {
					effect.HealingRound(player);
				}
			}
			Assert.AreEqual(90, player.HitPoints);
		}
		[Test]
		public void PowerAuraAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Abilities.Add(new Ability(
				"power aura", 150, 1, Ability.WarriorAbility.PowerAura, 6));
			var input = new [] {"use", "power", "aura"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Power Aura", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Power Aura Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Strength is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			var baseStr = player.Strength;
			var baseRage = player.RagePoints;
			var baseMaxRage = player.MaxRagePoints;
			var abilityIndex = player.Abilities.FindIndex(f => f.Name == InputHandler.ParseInput(input));
			player.UseAbility(input);
			Assert.AreEqual(player.Strength, baseStr + player.Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseRage - player.Abilities[abilityIndex].RageCost, player.RagePoints);
			Assert.AreEqual(
				player.MaxRagePoints, baseMaxRage + player.Abilities[abilityIndex].ChangeAmount.Amount * 10);
			var expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual("You generate a Power Aura around yourself.", expectedOutput);
			for (var i = 0; i < 10; i++) {
				GameHandler.CheckStatus(player);
			}
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Power Aura", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 590; i++) {
				GameHandler.CheckStatus(player);
			}
			Assert.AreEqual(baseStr, player.Strength);
			Assert.AreEqual(0, player.RagePoints);
			Assert.AreEqual(baseMaxRage, player.MaxRagePoints);
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
		}
		[Test]
		public void WarCryAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Abilities.Add(new Ability(
				"war cry", 50, 1, Ability.WarriorAbility.WarCry, 4));
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.InCombat = true;
			monster.InCombat = true;
			var input = new[] {"use", "war", "cry"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("War Cry", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 50", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("War Cry Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Opponent's attacks are decreased by 25 for 3 rounds.", 
				OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			player.UseAbility(input);
			Assert.AreEqual("You shout a War Cry, intimidating your opponent, and decreasing incoming damage.", 
				OutputHandler.Display.Output[0][2]);
			var baseAttackDamageM = monster.Attack(player);
			var attackDamageM = baseAttackDamageM;
			var indexDamageChange = player.Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.ChangeOpponentDamage);
			if (indexDamageChange != -1) {
				var changeDamageAmount = player.Effects[indexDamageChange].EffectAmountOverTime < attackDamageM ? 
					player.Effects[indexDamageChange].EffectAmountOverTime : attackDamageM;
				attackDamageM += changeDamageAmount;
				if (attackDamageM < 0) attackDamageM = 0;
			}
			Assert.AreEqual(attackDamageM, baseAttackDamageM - 25 >= 0 ? baseAttackDamageM - 25 : 0);
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(3 rounds) War Cry", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 3; i++) {
				GameHandler.CheckStatus(player);
			}
			OutputHandler.Display.ClearUserOutput();
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
			baseAttackDamageM = monster.Attack(player);
			indexDamageChange = player.Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.ChangeOpponentDamage);
			attackDamageM = baseAttackDamageM;
			if (indexDamageChange != -1) {
				var changeDamageAmount = player.Effects[indexDamageChange].EffectAmountOverTime < attackDamageM ? 
					player.Effects[indexDamageChange].EffectAmountOverTime : attackDamageM;
				attackDamageM -= changeDamageAmount;
			}
			Assert.AreEqual(attackDamageM, baseAttackDamageM);
		}
		[Test]
		public void OnslaughtAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Abilities.Add(new Ability(
				"onslaught", 25, 1, Ability.WarriorAbility.Onslaught, 8));
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.MaxRagePoints = 100;
			player.RagePoints = player.MaxRagePoints;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.InCombat = true;
			monster.InCombat = true;
			var input = new[] {"use", "onslaught"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Onslaught", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Two attacks are launched which each cause instant damage. Cost and damage are per attack.", 
				OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			player.UseAbility(monster, input);
			var index = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Onslaught);
			Assert.AreEqual(monster.HitPoints, 
				monster.MaxHitPoints - 2 * player.Abilities[index].Offensive.Amount);
			Assert.AreEqual(player.RagePoints, player.MaxRagePoints - 2 * player.Abilities[index].RageCost);
			var attackString = "Your onslaught hit the " + monster.Name + " for 25" + " physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[1][2]);
			player.MaxRagePoints = 25;
			player.RagePoints = player.MaxRagePoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			OutputHandler.Display.ClearUserOutput();
			player.UseAbility(monster, input);
			const string outOfRageString = "You didn't have enough rage points for the second attack!";
			Assert.AreEqual(monster.HitPoints, 
				monster.MaxHitPoints - player.Abilities[index].Offensive.Amount);
			Assert.AreEqual(player.RagePoints, player.MaxRagePoints - player.Abilities[index].RageCost);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(outOfRageString, OutputHandler.Display.Output[1][2]);
		}
	}
}