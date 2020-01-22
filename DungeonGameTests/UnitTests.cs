using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using DungeonGame;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using NUnit.Framework;

namespace DungeonGameTests {
	public class Tests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void ValueUnitTests() {
			// Test RoundNumber method in Helper class
			Assert.AreEqual(110, Helper.RoundNumber(107));
			Assert.AreEqual(110, Helper.RoundNumber(105));
			Assert.AreEqual(100, Helper.RoundNumber(104));
			/* Test Monster constructor HP and exp smoothing
			if values smoothed correctly, % should be 0 */
			var monster = new Monster(1, Monster.MonsterType.Skeleton);
			Assert.AreEqual(0, monster.MaxHitPoints % 10);
			Assert.AreEqual(0, monster.ExperienceProvided % 10);
			// Test consumable potion creation
			var potion = new Consumable(3, Consumable.PotionType.Health);
			Assert.AreEqual(9, potion.ItemValue);
			Assert.AreEqual("minor health potion", potion.Name);
			Assert.AreEqual(50, potion.RestoreHealth.RestoreHealthAmt);
			var potionTwo = new Consumable(4, Consumable.PotionType.Health);
			Assert.AreEqual(12, potionTwo.ItemValue);
			Assert.AreEqual("health potion", potionTwo.Name);
			Assert.AreEqual(100, potionTwo.RestoreHealth.RestoreHealthAmt);
			var potionThree = new Consumable(6, Consumable.PotionType.Health);
			Assert.AreEqual(18, potionThree.ItemValue);
			Assert.AreEqual("health potion", potionThree.Name);
			Assert.AreEqual(100, potionThree.RestoreHealth.RestoreHealthAmt);
			var potionFour = new Consumable(7, Consumable.PotionType.Health);
			Assert.AreEqual(21, potionFour.ItemValue);
			Assert.AreEqual("greater health potion", potionFour.Name);
			Assert.AreEqual(150, potionFour.RestoreHealth.RestoreHealthAmt);
			// Test consumable gem creation
			var gem = new Consumable(1, Consumable.GemType.Amethyst);
			Assert.AreEqual(20, gem.ItemValue);
			Assert.AreEqual("chipped amethyst", gem.Name);
			var gemTwo = new Consumable(3, Consumable.GemType.Amethyst);
			Assert.AreEqual(60, gemTwo.ItemValue);
			Assert.AreEqual("chipped amethyst", gemTwo.Name);
			var gemThree = new Consumable(4, Consumable.GemType.Amethyst);
			Assert.AreEqual(80, gemThree.ItemValue);
			Assert.AreEqual("dull amethyst", gemThree.Name);
			var gemFour = new Consumable(6, Consumable.GemType.Amethyst);
			Assert.AreEqual(120, gemFour.ItemValue);
			Assert.AreEqual("dull amethyst", gemFour.Name);
			var gemFive = new Consumable(7, Consumable.GemType.Amethyst);
			Assert.AreEqual(140, gemFive.ItemValue);
			Assert.AreEqual("amethyst", gemFive.Name);
		}
		[Test]
		public void ArmorUnitTests() {
			// Test armor creation values
			// Test case 1, level 1 head cloth armor, armor rating should be 4 to 6
			var testArrClothHead = new int[3] {4, 5, 6};
			var testArmorClothHead = new Armor(
				1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head);
			CollectionAssert.Contains(testArrClothHead, testArmorClothHead.ArmorRating);
			// Test case 2, level 3 chest leather armor, armor rating should be 15 to 17
			var testArrLeatherChest = new int[3] {15, 16, 17};
			var testArmorLeatherChest = new Armor(
				3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			CollectionAssert.Contains(testArrLeatherChest, testArmorLeatherChest.ArmorRating);
			// Test case 3, level 2 legs plate armor, armor rating should be 12 to 14
			var testArrPlateLegs = new int[3] {12, 13, 14};
			var testArmorPlateLegs = new Armor(
				2, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs);
			CollectionAssert.Contains(testArrPlateLegs, testArmorPlateLegs.ArmorRating);
			// Test armor name creation values
			Assert.AreEqual("ripped cloth cap", testArmorClothHead.Name);
			Assert.AreEqual("worn leather vest", testArmorLeatherChest.Name);
			Assert.AreEqual("dented plate leggings", testArmorPlateLegs.Name);
		}
		[Test]
		public void WeaponUnitTests() {
		// Test weapon creation values
			// Test case 1, weapon on level 1 skeleton based on possible weapon types
			// Name check
			var skeletonLevelOne = new Monster(1, Monster.MonsterType.Skeleton);
			var skeletonWeapon = skeletonLevelOne.MonsterWeapon;
			switch (skeletonWeapon.Quality) {
				case 1:
					switch (skeletonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped dagger", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sword (1H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sword (2H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (skeletonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped sturdy dagger", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sturdy sword (1H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sturdy sword (2H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (skeletonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped fine dagger", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped fine sword (1H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped fine sword (2H)", skeletonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 2, weapon on level 1 spider based on possible weapon types
			// Name check
			var spiderLevelOne = new Monster(1, Monster.MonsterType.Spider);
			var spiderWeapon = spiderLevelOne.MonsterWeapon;
			switch (spiderWeapon.Quality) {
				case 1:
					switch (spiderWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (spiderWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("sturdy venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (spiderWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("fine venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// Name check
			var zombieLevelThree = new Monster(3, Monster.MonsterType.Zombie);
			var zombieWeapon = zombieLevelThree.MonsterWeapon;
			switch (zombieWeapon.Quality) {
				case 1:
					switch (zombieWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn axe", zombieWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (zombieWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn sturdy axe", zombieWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (zombieWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							break;
						case Weapon.WeaponType.TwoHandedSword:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn fine axe", zombieWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// Name check
			var demonLevelTwo = new Monster(2, Monster.MonsterType.Demon);
			var demonWeapon = demonLevelTwo.MonsterWeapon;
			switch (demonWeapon.Quality) {
				case 1:
					switch (demonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull sword (1H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sword (2H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull axe", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (demonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull sturdy sword (1H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sturdy sword (2H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull sturdy axe", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (demonWeapon.WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull fine sword (1H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull fine sword (2H)", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull fine axe", demonWeapon.Name);
							break;
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
		}
		[Test]
		public void BandageAbilityUnitTests() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			player.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			var output = new UserOutput();
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			var globalTimer = new Timer(
				e => Helper.CheckStatus(player, spawnedRooms, output), 
				null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			player.Abilities.Add(
				new Ability("use bandage", 25, 1, Ability.ArcherAbility.Bandage));
			player.HitPoints = 10;
			/* Bandage should heal 25 immediately, 5 over time, cur round 1, max round 3
			Make sure stacked healing effects only tick for 3 rounds in combat */
			player.InCombat = true;
			var input = new string[2] {"use", "bandage"};
			var abilityName = Helper.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			player.UseAbility(abilityName, output);
			player.UseAbility(abilityName, output);
			Assert.AreEqual(60, player.HitPoints);
			for (var i = 0; i < 5; i++) {
				player.Effects[0].HealingRound(player, output);
				player.Effects[1].HealingRound(player, output);
				if (i <= 2) Assert.AreEqual(60 + ((i + 1) * 10), player.HitPoints);
			}
			Assert.AreEqual(90, player.HitPoints);
			player.InCombat = false;
			// Make sure stacked healing effects tick properly outside of combat		
			player.HitPoints = 10;
			var inputTwo = new string[2] {"use", "bandage"};
			var abilityNameTwo = Helper.ParseInput(inputTwo);
			Assert.AreEqual("bandage", abilityName);
			player.UseAbility(abilityNameTwo, output);
			player.UseAbility(abilityNameTwo, output);
			Assert.AreEqual(60, player.HitPoints);
			Thread.Sleep(TimeSpan.FromSeconds(30)); // Should finish ticking after 30 seconds
			Assert.AreEqual(90, player.HitPoints);
			// Make sure additional erroneous ticks don't happen
			Thread.Sleep(TimeSpan.FromSeconds(15)); // Check additional 15 seconds to make sure no more ticks
			Assert.AreEqual(90, player.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var output = new UserOutput();
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].GetMonster() == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			player.InCombat = true;
			monster.InCombat = true;
			var input = new string[2] {"use", "charge"};
			var abilityName = Helper.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, abilityName, output);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(2, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].StunnedRound(monster, output);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				Helper.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void RendAbilityUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var output = new UserOutput();
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].GetMonster() == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			var input = new string[2] {"use", "rend"};
			var abilityName = Helper.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, abilityName, output);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(70, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster, output);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				Helper.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(55, monster.HitPoints);
		}
		[Test]
		public void BerserkAbilityUnitTest() {
		// Berserk should create a change damage and change armor effect, which should expire when combat ends
		var player = new Player("placeholder", Player.PlayerClassType.Warrior);
		var output = new UserOutput();
		GearHelper.EquipInitialGear(player, output);
		var spawnedRooms = new List<IRoom> {
			new DungeonRoom(0, 0, 0, false, false, false,
				false, false, false, false, false, false,
				false, 1, 1)
		};
		if (spawnedRooms[0].GetMonster() == null) {
			spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
		}
		var globalTimer = new Timer(
			e => Helper.CheckStatus(player, spawnedRooms, output), 
			null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		var monster = spawnedRooms[0].Monster;
		var input = new string[2] {"use", "berserk"};
		var abilityName = Helper.ParseInput(input);
		Assert.AreEqual("berserk", abilityName);
		player.UseAbility(monster, abilityName, output);
		Assert.AreEqual(2, player.Effects.Count);
		player.InCombat = false;
		Thread.Sleep(TimeSpan.FromSeconds(3));
		Assert.AreEqual(0, player.Effects.Count);
		}
		[Test]
		public void ArcherAbilityUnitTests() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			player.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			var output = new UserOutput();
			GearHelper.EquipInitialGear(player, output);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].GetMonster() == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.InCombat = true;
			monster.InCombat = true;
			monster.HitPoints = 50;
			monster.MaxHitPoints = 100;
			var input = new string[2] {"use", "stun"};
			var abilityName = Helper.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			player.UseAbility(monster, abilityName, output);
			Assert.AreEqual(35, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].StunnedRound(monster, output);
				Assert.AreEqual(i , monster.Effects[0].EffectCurRound);
				Helper.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			monster.HitPoints = 80;
			var inputTwo = new string[2] {"use", "gut"};
			var abilityNameTwo = Helper.ParseInput(inputTwo);
			Assert.AreEqual("gut", abilityNameTwo);
			player.UseAbility(monster, abilityNameTwo, output);
			Assert.AreEqual(65, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster, output);
				Assert.AreEqual(i , monster.Effects[0].EffectCurRound);
				Helper.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(50, monster.HitPoints);
		}

		[Test]
		public void FireballSpellUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			player.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			var output = new UserOutput();
			GearHelper.EquipInitialGear(player, output);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].GetMonster() == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			monster.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player.InCombat = true;
			monster.InCombat = true;
			monster.HitPoints = 50;
			monster.MaxHitPoints = 100;
			var input = new string[2] {"cast", "fireball"};
			var spellName = Helper.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName, output);
			Assert.AreEqual(25, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].OnFireRound(monster, output);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				Helper.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(10, monster.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			player.StatReplenishInterval = 9999999; // Disable stat replenish over time method
			var output = new UserOutput();
			GearHelper.EquipInitialGear(player, output);
			var spawnedRooms = new List<IRoom> {
				new DungeonRoom(0, 0, 0, false, false, false,
					false, false, false, false, false, false,
					false, 1, 1)
			};
			if (spawnedRooms[0].GetMonster() == null) {
				spawnedRooms[0].Monster = new Monster(3, Monster.MonsterType.Demon);
			}
			var monster = spawnedRooms[0].Monster;
			foreach (var item in monster.MonsterItems.Where(item => item.IsEquipped())) {
				item.Equipped = false;
			}
			monster.HitPoints = 100;
			var input = new string[2] {"cast", "frostbolt"};
			var spellName = Helper.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player.PlayerWeapon.Durability = 100;
			var baseDamage = (double) player.Attack(monster, output);
			player.CastSpell(monster, spellName, output);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(2, monster.Effects[0].EffectMaxRound);
			var monsterHitPointsBefore = monster.HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster.Effects[0].EffectMultiplier;
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].FrozenRound(monster, output);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				player.PlayerWeapon.Durability = 100;
				var frozenDamage = (double) player.Attack(monster, output);
				Assert.AreEqual(frozenDamage, baseDamage * multiplier, 1);
				monster.TakeDamage((int) frozenDamage);
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			Helper.RemovedExpiredEffects(monster);
			Assert.AreEqual(false, monster.Effects.Any());
			var finalBaseDamageWithMod = (int) (totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int) totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 1);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int) totalFrozenDamage);
		}
		[Test]
		public void DiamondskinSpellUnitTest() {
			/* Diamondskin should augment armor by 25 points, 1 cur round, 3 max round */
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			var output = new UserOutput();
			GearHelper.EquipInitialGear(player, output);
			player.InCombat = true;
			var inputThree = new string[2] {"cast", "diamondskin"};
			var spellName = Helper.ParseInput(inputThree);
			Assert.AreEqual("diamondskin", spellName);
			var baseArmor = GearHelper.CheckArmorRating(player, output);
			player.CastSpell(spellName, output);
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(
				true, player.Effects[0].EffectGroup == Effect.EffectType.ChangeArmor);
			var augmentedArmor = GearHelper.CheckArmorRating(player, output);
			Assert.AreEqual(baseArmor + 25, augmentedArmor);
			// Check for 6 rounds, should only augment armor for first 3 rounds then expire
			for (var i = 1; i < 6; i++) {
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Assert.AreEqual(
					GearHelper.CheckArmorRating(player, output), i <= 2 ? augmentedArmor : baseArmor);
			}
			Helper.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void UpgradeSpellTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			var output = new UserOutput();
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			player.PlayerClass = Player.PlayerClassType.Archer;
			trainer.UpgradeSpell(player, "fireball", output);
			var expectedOutput =output.Output[0][2]; 
			Assert.AreEqual("You can't upgrade spells. You're not a mage!",expectedOutput);
			player.PlayerClass = Player.PlayerClassType.Mage;
			var spellIndex = player.Spellbook.FindIndex(
				f => f.GetName() == "fireball");
			Assert.AreEqual(25, player.Spellbook[spellIndex].FireOffense.BlastDamage);
			Assert.AreEqual(5, player.Spellbook[spellIndex].FireOffense.BurnDamage);
			player.Gold = 0;
			trainer.UpgradeSpell(player, "fireball", output);
			Assert.AreEqual(25, player.Spellbook[spellIndex].FireOffense.BlastDamage);
			Assert.AreEqual(5, player.Spellbook[spellIndex].FireOffense.BurnDamage);
			var expectedOutputTwo = output.Output[1][2];
			Assert.AreEqual("You are not ready to upgrade that spell. You need to level up first!", 
				expectedOutputTwo);
			trainer.UpgradeSpell(player, "not a spell", output);
			var expectedOutputThree = output.Output[2][2];
			Assert.AreEqual("You don't have that spell to train!", expectedOutputThree);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeSpell(player, "fireball", output);
			var expectedOutputFour = output.Output[3][2];
			Assert.AreEqual("You can't afford that!",expectedOutputFour);
			player.Gold = 100;
			trainer.UpgradeSpell(player, "fireball", output);
			Assert.AreEqual(2, player.Spellbook[spellIndex].Rank);
			Assert.AreEqual(35, player.Spellbook[spellIndex].FireOffense.BlastDamage);
			Assert.AreEqual(10, player.Spellbook[spellIndex].FireOffense.BurnDamage);
			Assert.AreEqual(60, player.Gold);
			var expectedOutputFive = output.Output[4][2];
			Assert.AreEqual("You upgraded fireball to level 2 for 40 gold.", expectedOutputFive);
		}
		[Test]
		public void UpgradeAbilityTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			var output = new UserOutput();
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Archer);
			player.PlayerClass = Player.PlayerClassType.Mage;
			trainer.UpgradeAbility(player, "distance", output);
			var expectedOutput =output.Output[0][2]; 
			Assert.AreEqual("You can't upgrade abilities. You're not a warrior or archer!",expectedOutput);
			player.PlayerClass = Player.PlayerClassType.Archer;
			var abilityIndex = player.Abilities.FindIndex(
				f => f.GetName() == "distance" || f.GetName().Contains("distance"));
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			player.Gold = 0;
			trainer.UpgradeAbility(player, "distance", output);
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			var expectedOutputTwo = output.Output[1][2];
			Assert.AreEqual("You are not ready to upgrade that ability. You need to level up first!", 
				expectedOutputTwo);
			trainer.UpgradeAbility(player, "not an ability", output);
			var expectedOutputThree = output.Output[2][2];
			Assert.AreEqual("You don't have that ability to train!", expectedOutputThree);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeAbility(player, "distance", output);
			var expectedOutputFour = output.Output[3][2];
			Assert.AreEqual("You can't afford that!",expectedOutputFour);
			player.Gold = 100;
			trainer.UpgradeAbility(player, "distance", output);
			Assert.AreEqual(2, player.Abilities[abilityIndex].Rank);
			Assert.AreEqual(35, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(55, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			Assert.AreEqual(60, player.Gold);
			var expectedOutputFive = output.Output[4][2];
			Assert.AreEqual("You upgraded distance shot to level 2 for 40 gold.", expectedOutputFive);
		}
	}
}