using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class ArcAbilityUnitTests {
		[Test]
		public void DistanceShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1),
				new DungeonRoom(0, 1, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1),
				new DungeonRoom(1, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)};
			RoomHandler.Rooms[2].Monster = null;
			RoomHandler.Rooms[1].Monster = new Monster(3, Monster.MonsterType.Demon)
				{HitPoints = 100, MaxHitPoints = 100};
			var monster = RoomHandler.Rooms[1].Monster;
			MonsterBuilder.BuildMonster(monster);
			RoomHandler.SetPlayerLocation(player, 0, 0, 0);
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] {"ability", "distance"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Distance Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("50% chance to hit monster in attack direction.", 
				OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Usage example if monster is in room to north. 'use distance north'", 
				OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "distance", "south"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("distance south", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player.PlayerQuiver.Quantity);
			Assert.AreEqual("You can't attack in that direction!", OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(player.MaxComboPoints,player.ComboPoints);
			input = new [] {"use", "distance", "east"};
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player.PlayerQuiver.Quantity);
			Assert.AreEqual("There is no monster in that direction to attack!", 
				OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(player.MaxComboPoints,player.ComboPoints);
			Assert.AreEqual(player.MaxComboPoints,player.ComboPoints);
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Distance);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			player.Abilities[abilityIndex].Offensive.ChanceToSucceed = 0;
			input = new [] {"use", "distance", "north"};
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var missString = "You tried to shoot " + monster.Name + " from afar but failed!";
			Assert.AreEqual(missString,OutputHandler.Display.Output[8][2]);
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
			player.Abilities[abilityIndex].Offensive.ChanceToSucceed = 100;
			player.ComboPoints = player.MaxComboPoints;
			arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var abilityDmg = player.Abilities[abilityIndex].Offensive.Amount;
			var shootString = "You successfully shot " + monster.Name + " from afar for " + abilityDmg + " damage!"; 
			Assert.AreEqual(shootString,OutputHandler.Display.Output[9][2]);
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
			Assert.AreEqual(monster.MaxHitPoints - abilityDmg, monster.HitPoints);
		}
		[Test]
		public void GutShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Gut);
			var inputInfo = new[] {"ability", "gut"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Gut Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "gut"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("gut", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
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
		public void PreciseShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var inputInfo = new[] {"ability", "precise"};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Precise);
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Precise Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			var input = new [] {"use", "precise"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("precise", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
			var abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			var abilitySuccessString = "Your " + player.Abilities[abilityIndex].Name + " hit the " + monster.Name + " for " +
			                           abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void StunShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Stun);
			var inputInfo = new[] {"ability", "stun"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Stun Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			var abilityInfoString = "Stuns opponent for " + 
			                        player.Abilities[abilityIndex].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "stun"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
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
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].StunnedRound(monster);
				var stunnedString = "The " + monster.Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void WoundShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Wound);
			var inputInfo = new[] {"ability", "wound"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Wound Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 5", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 10", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "wound"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("wound", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
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
			for (var i = 2; i < 7; i++) {
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
		public void DoubleShotAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Double);
			var inputInfo = new[] {"ability", "double"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Double Shot", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual(
				"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] {"use", "double"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("double", abilityName);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 2, player.PlayerQuiver.Quantity);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			var hitAmount = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.MaxHitPoints - 2 * hitAmount,monster.HitPoints);
			Assert.AreEqual(player.MaxComboPoints - 2 * comboCost, player.ComboPoints);
			var attackString = "Your double shot hit the " + monster.Name + " for 25" + " physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			player.MaxComboPoints = 25;
			player.ComboPoints = player.MaxComboPoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[7][2]);
			const string outOfComboString = "You didn't have enough combo points for the second shot!";
			Assert.AreEqual(outOfComboString, OutputHandler.Display.Output[8][2]);
		}
		[Test]
		public void BandageAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			player.Abilities.Add(new Ability(
				"bandage", 25, 1, Ability.ArcherAbility.Bandage, 2));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Bandage);
			var inputInfo = new[] {"ability", "bandage"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputHandler.Display.Output[4][2]);
			var healInfoStringCombat = "Heal over time will restore health for " + 
			                           player.Abilities[abilityIndex].Healing.HealMaxRounds + " rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputHandler.Display.Output[5][2]);
			var healInfoStringNonCombat = "Heal over time will restore health " + 
			                              player.Abilities[abilityIndex].Healing.HealMaxRounds + " times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputHandler.Display.Output[6][2]);
			var input = new [] {"use", "bandage"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			var baseHitPoints = player.HitPoints;
			player.UseAbility(input);
			var comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost,player.ComboPoints);
			var healAmount = player.Abilities[abilityIndex].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player.Effects[0].EffectGroup);
			Assert.AreEqual(baseHitPoints + healAmount, player.HitPoints);
			baseHitPoints = player.HitPoints;
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				player.Effects[0].HealingRound(player);
				var healOverTimeAmt = player.Effects[0].EffectAmountOverTime;
				var healAmtString = "You have been healed for " + healOverTimeAmt + " health.";
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + (i - 1) * healOverTimeAmt, player.HitPoints);
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
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
			Assert.AreEqual("You generate a Swift Aura around yourself.", OutputHandler.Display.Output[5][2]);
			for (var i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseDex, player.Dexterity);
			Assert.AreEqual(baseMaxCombo, player.MaxComboPoints);
			Assert.AreEqual(baseCombo - player.Abilities[abilityIndex].ComboCost, player.ComboPoints);
		}
		[Test]
		public void ImmolatingArrowAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10, InCombat = true};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			MonsterBuilder.BuildMonster(monster);
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player.Abilities.Add(new Ability(
				"immolating arrow", 35, 1, Ability.ArcherAbility.ImmolatingArrow, 8));
			var input = new[] {"use", "immolating", "arrow"};
			PlayerHandler.AbilityInfo(player, input);
			Assert.AreEqual("Immolating Arrow", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 35", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5",OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time for 3 rounds.", OutputHandler.Display.Output[5][2]);
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
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
			var player = new Player("test", Player.PlayerClassType.Archer) {MaxComboPoints = 100, ComboPoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			player.Abilities.Add(new Ability(
				"ambush", 75, 1, Ability.ArcherAbility.Ambush, 4));
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
			var arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual("You can't ambush " + monster.Name + ", you're already in combat!", 
				OutputHandler.Display.Output[5][2]);
			player.InCombat = false;
			player.UseAbility(monster, input);
			var index = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == Ability.ArcherAbility.Ambush);
			var abilityDamage = player.Abilities[index].Offensive.Amount;
			var attackString = "Your ambush hit the " + monster.Name + " for " + abilityDamage + " physical damage.";
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			Assert.AreEqual(attackString,OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
		}
	}
}