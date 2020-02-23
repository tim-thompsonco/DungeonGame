using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class ArcAbilityUnitTests {
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
			player.UseAbility(monster, input);
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
			player.UseAbility(monster, inputTwo);
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
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Abilities.Add(new Ability(
				"swift aura", 150, 1, Ability.ArcherAbility.SwiftAura, 6));
			var input = new [] {"use", "swift", "aura"};
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
			var abilityIndex = player.Abilities.FindIndex(f => f.Name == InputHandler.ParseInput(input));
			player.UseAbility(input);
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
		public void ImmolatingArrowAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			GearHandler.EquipInitialGear(player);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player.Abilities.Add(new Ability(
				"immolating arrow", 35, 1, Ability.ArcherAbility.ImmolatingArrow, 8));
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.MaxComboPoints = 100;
			player.ComboPoints = player.MaxComboPoints;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.InCombat = true;
			monster.InCombat = true;
			var input = new[] {"use", "immolating", "arrow"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Immolating Arrow", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 35", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5",OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time for 3 rounds.", OutputHandler.Display.Output[5][2]);
			player.UseAbility(monster, input);
			var index = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.ImmolatingArrow);
			Assert.AreEqual(player.ComboPoints,player.MaxComboPoints - player.Abilities[index].ComboCost);
			var attackString = "Your immolating arrow hit the " + monster.Name + " for 25 physical damage.";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.HitPoints, 
				monster.MaxHitPoints - player.Abilities[index].Offensive.Amount);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(Effect.EffectType.OnFire, monster.Effects[0].EffectGroup);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 0; i < 3; i++) {
				var baseHitPoints = monster.HitPoints;
				monster.Effects[0].OnFireRound(monster);
				Assert.AreEqual(i + 2, monster.Effects[0].EffectCurRound);
				Assert.AreEqual(monster.HitPoints, baseHitPoints - monster.Effects[0].EffectAmountOverTime);
				var burnString = "The " + monster.Name + " burns for " + 
				                 monster.Effects[0].EffectAmountOverTime + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i][2]);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void AmbushAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			GearHandler.EquipInitialGear(player);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			player.Abilities.Add(new Ability(
				"ambush", 75, 1, Ability.ArcherAbility.Ambush, 4));
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.MaxComboPoints = 100;
			player.ComboPoints = player.MaxComboPoints;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			var inputInfo = new[] {"ability", "ambush"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Ambush", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 75", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("A surprise attack is launched, which initiates combat.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] {"use", "ambush", monster.Name};
			player.InCombat = true;
			player.UseAbility(monster, input);
			Assert.AreEqual("You can't ambush " + monster.Name + ", you're already in combat!", 
				OutputHandler.Display.Output[5][2]);
			player.InCombat = false;
			player.UseAbility(monster, input);
			var index = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Ambush);
			var abilityDamage = player.Abilities[index].Offensive.Amount;
			var attackString = "Your ambush hit the " + monster.Name + " for " + abilityDamage + " physical damage.";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
		}
	}
}