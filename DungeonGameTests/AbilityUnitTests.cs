using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class AbilityUnitTests {
		[Test]
		public void BandageAbilityUnitTests() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			player.Abilities.Add(
				new Ability("use bandage", 25, 1, Ability.ArcherAbility.Bandage, 1));
			player.HitPoints = 10;
			/* Bandage should heal 25 immediately, 5 over time, cur round 1, max round 3
			Make sure stacked healing effects only tick for 3 rounds in combat */
			player.InCombat = true;
			var input = new [] {"use", "bandage"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			player.UseAbility(abilityName);
			player.UseAbility(abilityName);
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
			player.UseAbility(abilityNameTwo);
			player.UseAbility(abilityNameTwo);
			Assert.AreEqual(60, player.HitPoints);
			foreach (var effect in player.Effects.Where(effect => effect.EffectGroup == Effect.EffectType.Healing)) {
				while (!effect.IsEffectExpired) {
					effect.HealingRound(player);
				}
			}
			Assert.AreEqual(90, player.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].Monster == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			player.InCombat = true;
			monster.InCombat = true;
			var input = new [] {"use", "charge"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, abilityName);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(2, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].StunnedRound(monster);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void RendAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].Monster == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			monster.HitPoints = 100;
			var input = new [] {"use", "rend"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, abilityName);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(70, monster.HitPoints);
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
		player.UseAbility(monster, abilityName);
		Assert.AreEqual(2, player.Effects.Count);
		player.InCombat = false;
		GameHandler.CheckStatus(player);
		Assert.AreEqual(0, player.Effects.Count);
		}
		[Test]
		public void ArcherAbilityUnitTests() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer) {StatReplenishInterval = 9999999};
			// Disable stat replenish over time method
			GearHandler.EquipInitialGear(player);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].Monster == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.InCombat = true;
			monster.InCombat = true;
			monster.HitPoints = 50;
			monster.MaxHitPoints = 100;
			var input = new [] {"use", "stun"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			player.UseAbility(monster, abilityName);
			Assert.AreEqual(35, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].StunnedRound(monster);
				Assert.AreEqual(i , monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			monster.HitPoints = 80;
			var inputTwo = new [] {"use", "gut"};
			var abilityNameTwo = InputHandler.ParseInput(inputTwo);
			Assert.AreEqual("gut", abilityNameTwo);
			player.UseAbility(monster, abilityNameTwo);
			Assert.AreEqual(65, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster);
				Assert.AreEqual(i , monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(50, monster.HitPoints);
		}
		[Test]
		public void SwiftAuraAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Abilities.Add(new Ability(
				"swift aura", 150, 1, Ability.ArcherAbility.SwiftAura, 6));
			var input = new[] {"swift", "aura"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Swift Aura", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Swift Aura Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Dexterity is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			var baseDex = player.Dexterity;
			var baseCombo = player.ComboPoints;
			var baseMaxCombo = player.MaxComboPoints;
			const string inputName = "swift aura";
			var abilityIndex = player.Abilities.FindIndex(f => f.Name == inputName);
			player.UseAbility(inputName);
			Assert.AreEqual(player.Dexterity, baseDex + player.Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseCombo - player.Abilities[abilityIndex].ComboCost, player.ComboPoints);
			Assert.AreEqual(
				player.MaxComboPoints, baseMaxCombo + player.Abilities[abilityIndex].ChangeAmount.Amount * 10);
			var expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual("You generate a Swift Aura around yourself.", expectedOutput);
			for (var i = 0; i < 10; i++) {
				GameHandler.CheckStatus(player);
			}
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Swift Aura", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 590; i++) {
				GameHandler.CheckStatus(player);
			}
			Assert.AreEqual(baseDex, player.Dexterity);
			Assert.AreEqual(0, player.ComboPoints);
			Assert.AreEqual(baseMaxCombo, player.MaxComboPoints);
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
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
			var input = new[] {"power", "aura"};
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
			const string inputName = "power aura";
			var abilityIndex = player.Abilities.FindIndex(f => f.Name == inputName);
			player.UseAbility(inputName);
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
	}
}