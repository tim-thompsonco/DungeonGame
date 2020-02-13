using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class SpellUnitTests {
		[Test]
		public void FireballSpellUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage) {StatReplenishInterval = 9999999};
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
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player.InCombat = true;
			monster.InCombat = true;
			monster.HitPoints = 50;
			monster.MaxHitPoints = 100;
			var input = new [] {"cast", "fireball"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(25, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].OnFireRound(monster);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(10, monster.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
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
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			monster.HitPoints = 100;
			var input = new [] {"cast", "frostbolt"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player.PlayerWeapon.Durability = 100;
			var baseDamage = (double) player.Attack(monster);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(2, monster.Effects[0].EffectMaxRound);
			var monsterHitPointsBefore = monster.HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster.Effects[0].EffectMultiplier;
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].FrozenRound(monster);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				player.PlayerWeapon.Durability = 100;
				var frozenDamage = (double) player.Attack(monster);
				monster.TakeDamage((int) frozenDamage);
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHandler.RemovedExpiredEffects(monster);
			Assert.AreEqual(false, monster.Effects.Any());
			var finalBaseDamageWithMod = (int) (totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int) totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int) totalFrozenDamage);
		}
		[Test]
		public void DiamondskinSpellUnitTest() {
			/* Diamondskin should augment armor by 25 points, 1 cur round, 3 max round */
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			GearHandler.EquipInitialGear(player);
			player.InCombat = true;
			var inputThree = new [] {"cast", "diamondskin"};
			var spellName = InputHandler.ParseInput(inputThree);
			Assert.AreEqual("diamondskin", spellName);
			var baseArmor = GearHandler.CheckArmorRating(player);
			player.CastSpell(spellName);
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(
				true, player.Effects[0].EffectGroup == Effect.EffectType.ChangeArmor);
			var augmentedArmor = GearHandler.CheckArmorRating(player);
			Assert.AreEqual(baseArmor + 25, augmentedArmor);
			// Check for 6 rounds, should only augment armor for first 3 rounds then expire
			for (var i = 1; i < 6; i++) {
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Assert.AreEqual(
					GearHandler.CheckArmorRating(player), i <= 2 ? augmentedArmor : baseArmor);
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void TownPortalSpellUnitTest() {
			/* Town Portal should change location of player to where portal is set to */
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new RoomBuilder(
				100, 5, 0, 4, 0, RoomBuilder.StartDirection.Down).RetrieveSpawnRooms();
			player.Spellbook.Add(new Spell(
				"town portal", 50, 1, Spell.SpellType.TownPortal, 1));
			player.X = -2;
			player.Y = 6;
			player.Z = 0;
			Assert.AreEqual(-2, player.X);
			Assert.AreEqual(6, player.Y);
			Assert.AreEqual(0, player.Z);
			player.CastSpell("town portal");
			Assert.AreEqual(0, player.X);
			Assert.AreEqual(7, player.Y);
			Assert.AreEqual(0, player.Z);
			var expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual("You open a portal and step through it.", expectedOutput);
		}
		[Test]
		public void ReflectDamageSpellUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (RoomHandler.Rooms[0].Monster == null) {
				RoomHandler.Rooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = RoomHandler.Rooms[0].Monster;
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			monster.MaxHitPoints = 100;
			monster.HitPoints = 100;
			player.Spellbook.Add(new Spell(
				"reflect", 100, 1, Spell.SpellType.Reflect, 1));
			player.CastSpell("reflect");
			Assert.AreEqual(true, player.IsReflectingDamage);
			var expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual("You create a shield around you that will reflect damage.", expectedOutput);
			var attackDamageM = monster.Attack(player);
			var index = player.Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.ReflectDamage);
			var reflectAmount = player.Effects[index].EffectAmountOverTime < attackDamageM ? 
				player.Effects[index].EffectAmountOverTime : attackDamageM;
			Assert.AreEqual(true, reflectAmount <= player.Effects[index].EffectAmountOverTime);
			monster.HitPoints -= reflectAmount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - reflectAmount);
			OutputHandler.Display.ClearUserOutput();
			player.Effects[index].ReflectDamageRound(player, reflectAmount);
			expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual(
				"You reflected " + reflectAmount + " damage back at your opponent!", expectedOutput);
		}
		[Test]
		public void ArcaneIntellectSpellUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			player.Spellbook.Add(new Spell(
				"arcane intellect", 150, 1, Spell.SpellType.ArcaneIntellect, 1));
			PlayerHandler.SpellInfo(player, "arcane intellect");
			Assert.AreEqual("Arcane Intellect", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Arcane Intellect Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Intelligence is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			var baseInt = player.Intelligence;
			var baseMana = player.ManaPoints;
			var baseMaxMana = player.MaxManaPoints;
			const string inputName = "arcane intellect";
			var spellIndex = player.Spellbook.FindIndex(f => f.Name == inputName);
			player.CastSpell(inputName);
			Assert.AreEqual(player.Intelligence, baseInt + player.Spellbook[spellIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseMana - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(
				player.MaxManaPoints, baseMaxMana + player.Spellbook[spellIndex].ChangeAmount.Amount * 10);
			var expectedOutput = OutputHandler.Display.Output[0][2];
			Assert.AreEqual("You cast Arcane Intellect on yourself.", expectedOutput);
			for (var i = 0; i < 10; i++) {
				GameHandler.CheckStatus(player);
			}
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Arcane Intellect", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 590; i++) {
				GameHandler.CheckStatus(player);
			}
			Assert.AreEqual(baseInt, player.Intelligence);
			Assert.AreEqual(0, player.ManaPoints);
			Assert.AreEqual(baseMaxMana, player.MaxManaPoints);
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
		}
	}
}