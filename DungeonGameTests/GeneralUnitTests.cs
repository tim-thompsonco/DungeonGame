using DungeonGame;
using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Equipment;
using DungeonGame.Items.WeaponObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGameTests {
	public class GeneralUnitTests {
		[SetUp]
		public void Setup() {
		}
		[Test]
		public void ValueUnitTests() {
			// Test RoundNumber method in Helper class
			Assert.AreEqual(110, GameHelper.RoundNumber(107));
			Assert.AreEqual(110, GameHelper.RoundNumber(105));
			Assert.AreEqual(100, GameHelper.RoundNumber(104));
			/* Test _Monster constructor HP and exp smoothing
			if values smoothed correctly, % should be 0 */
			Monster monster = new Monster(1, MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(monster);
			Assert.AreEqual(0, monster.MaxHitPoints % 10);
			Assert.AreEqual(0, monster.ExperienceProvided % 10);
		}
		[Test]
		public void ArmorUnitTests() {
			// Test armor creation values
			// Test case 1, level 1 head cloth armor, armor rating should be 4 to 6
			int[] testArrClothHead = new[] { 4, 5, 6 };
			Armor testArmorClothHead = new Armor(
				1, ArmorType.Cloth, ArmorSlot.Head);
			CollectionAssert.Contains(testArrClothHead, testArmorClothHead.ArmorRating);
			// Test case 2, level 3 chest leather armor, armor rating should be 15 to 17
			int[] testArrLeatherChest = new[] { 15, 16, 17 };
			Armor testArmorLeatherChest = new Armor(
				3, ArmorType.Leather, ArmorSlot.Chest);
			CollectionAssert.Contains(testArrLeatherChest, testArmorLeatherChest.ArmorRating);
			// Test case 3, level 2 legs plate armor, armor rating should be 12 to 14
			int[] testArrPlateLegs = new[] { 12, 13, 14 };
			Armor testArmorPlateLegs = new Armor(
				2, ArmorType.Plate, ArmorSlot.Legs);
			CollectionAssert.Contains(testArrPlateLegs, testArmorPlateLegs.ArmorRating);
		}
		[Test]
		public void WeaponUnitTests() {
			// Test weapon creation values
			// Test case 1, weapon on level 1 skeleton based on possible weapon types
			// _Name check
			Monster skeletonLevelOne = new Monster(1, MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(skeletonLevelOne);
			Weapon skeletonWeapon = skeletonLevelOne.MonsterWeapon;
			switch (skeletonWeapon.Quality) {
				case 1:
					switch (skeletonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("chipped dagger", skeletonWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sword (1H)", skeletonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sword (2H)", skeletonWeapon.Name);
							break;
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (skeletonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("chipped sturdy dagger", skeletonWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sturdy sword (1H)", skeletonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sturdy sword (2H)", skeletonWeapon.Name);
							break;
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (skeletonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("chipped fine dagger", skeletonWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("chipped fine sword (1H)", skeletonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped fine sword (2H)", skeletonWeapon.Name);
							break;
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 2, weapon on level 1 spider based on possible weapon types
			// _Name check
			Monster spiderLevelOne = new Monster(1, MonsterType.Spider);
			MonsterBuilder.BuildMonster(spiderLevelOne);
			Weapon spiderWeapon = spiderLevelOne.MonsterWeapon;
			switch (spiderWeapon.Quality) {
				case 1:
					switch (spiderWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("venomous fang", spiderWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (spiderWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("sturdy venomous fang", spiderWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (spiderWeapon.WeaponGroup) {
						case WeaponType.Dagger:
							Assert.AreEqual("fine venomous fang", spiderWeapon.Name);
							break;
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Axe:
						case WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// _Name check
			Monster zombieLevelThree = new Monster(3, MonsterType.Zombie);
			MonsterBuilder.BuildMonster(zombieLevelThree);
			Weapon zombieWeapon = zombieLevelThree.MonsterWeapon;
			switch (zombieWeapon.Quality) {
				case 1:
					switch (zombieWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Bow:
							break;
						case WeaponType.Axe:
							Assert.AreEqual("worn axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (zombieWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Bow:
							break;
						case WeaponType.Axe:
							Assert.AreEqual("worn sturdy axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (zombieWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.OneHandedSword:
						case WeaponType.TwoHandedSword:
						case WeaponType.Bow:
							break;
						case WeaponType.Axe:
							Assert.AreEqual("worn fine axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// _Name check
			Monster demonLevelTwo = new Monster(2, MonsterType.Demon);
			MonsterBuilder.BuildMonster(demonLevelTwo);
			Weapon demonWeapon = demonLevelTwo.MonsterWeapon;
			switch (demonWeapon.Quality) {
				case 1:
					switch (demonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.Bow:
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("dull sword (1H)", demonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sword (2H)", demonWeapon.Name);
							break;
						case WeaponType.Axe:
							Assert.AreEqual("dull axe", demonWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (demonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.Bow:
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("dull sturdy sword (1H)", demonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sturdy sword (2H)", demonWeapon.Name);
							break;
						case WeaponType.Axe:
							Assert.AreEqual("dull sturdy axe", demonWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (demonWeapon.WeaponGroup) {
						case WeaponType.Dagger:
						case WeaponType.Bow:
							break;
						case WeaponType.OneHandedSword:
							Assert.AreEqual("dull fine sword (1H)", demonWeapon.Name);
							break;
						case WeaponType.TwoHandedSword:
							Assert.AreEqual("dull fine sword (2H)", demonWeapon.Name);
							break;
						case WeaponType.Axe:
							Assert.AreEqual("dull fine axe", demonWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
		}
		[Test]
		public void PlayerMaxLevelUnitTest() {
			/* Player should not be able to go beyond level 10 */
			Player player = new Player("placeholder", PlayerClassType.Mage) { Level = 10 };
			player.Experience = player.ExperienceToLevel - 1;
			player.Experience++;
			PlayerHelper.LevelUpCheck(player);
			Assert.AreEqual(10, player.Level);
		}
		[Test]
		public void CheckStatusUnitTest() {
			Player player = new Player("placeholder", PlayerClassType.Mage);
			RoomHelper.Rooms = new RoomBuilder(
				100, 5, 0, 4, 0).RetrieveSpawnRooms();
			GameHelper.CheckStatus(player);
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, SpellType.Reflect, 1));
			player.CastSpell("reflect");
			for (int i = 0; i <= 30; i++) {
				GameHelper.CheckStatus(player);
			}
		}
		[Test]
		public void EffectUserOutputUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage);
			RoomHelper.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, SpellType.Reflect, 1));
			UserOutput defaultEffectOutput = OutputHelper.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
			player.CastSpell("reflect");
			OutputHelper.Display.ClearUserOutput();
			defaultEffectOutput = OutputHelper.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(30 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			for (int i = 0; i < 10; i++) {
				GameHelper.CheckStatus(player);
			}
			player.Effects.Add(new BurningEffect("burning", 3, 5));
			Assert.AreEqual("Your spell reflect is slowly fading away.", OutputHelper.Display.Output[0][2]);
			player.CastSpell("rejuvenate");
			defaultEffectOutput = OutputHelper.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(20 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[2][0]);
			Assert.AreEqual("(30 seconds) Rejuvenate", defaultEffectOutput.Output[2][2]);
			Assert.AreEqual(Settings.FormatAttackFailText(), defaultEffectOutput.Output[3][0]);
			Assert.AreEqual("(30 seconds) Burning", defaultEffectOutput.Output[3][2]);
		}
		[Test]
		public void SaveLoadGameUnitTest() {
			Player player = new Player("placeholder", PlayerClassType.Mage);
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			RoomHelper.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player.CanSave = true;
			GameHelper.SaveGame(player);
			Assert.AreEqual("Your game has been saved.", OutputHelper.Display.Output[0][2]);
			OutputHelper.Display.ClearUserOutput();
			RoomHelper.Rooms = null;
			GameHelper.LoadGame();
			player = GameHelper.LoadPlayer();
			Assert.AreEqual("placeholder", player.Name);
			Assert.NotNull(RoomHelper.Rooms);
			Assert.AreEqual("Reloading your saved game.", OutputHelper.Display.Output[1][2]);
		}
		[Test]
		public void MonsterResistanceUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			Assert.AreEqual(monster.Level * 5, monster.FireResistance);
			Assert.AreEqual(monster.Level * 5, monster.FrostResistance);
			Assert.AreEqual(monster.Level * 5, monster.ArcaneResistance);
			int arcaneResistance = monster.ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			string[] input = new[] { "cast", "lightning" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Lightning);
			string spellName = InputHelper.ParseInput(input);
			player.CastSpell(monster, spellName);
			int reducedDamage = (int)(player.Spellbook[spellIndex].Offensive.Amount * resistanceMod);
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - reducedDamage);
		}
		[Test]
		public void PlayerResistanceUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Elemental);
			while (monster.ElementalCategory != ElementalType.Air) {
				monster = new Monster(3, MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			Assert.AreEqual(player.Level * 5, player.FireResistance);
			Assert.AreEqual(player.Level * 5, player.FrostResistance);
			Assert.AreEqual(player.Level * 5, player.ArcaneResistance);
			int arcaneResistance = player.ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			int spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Lightning);
			monster.Spellbook[spellIndex].CastArcaneOffense(monster, player, spellIndex);
			int reducedDamage = (int)(monster.Spellbook[spellIndex].Offensive.Amount * resistanceMod);
			Assert.AreEqual(player.HitPoints, player.MaxHitPoints - reducedDamage);
		}
		[Test]
		public void MonsterResistanceLevelUnitTest() {
			Monster monster = new Monster(1, MonsterType.Vampire);
			Assert.AreEqual(monster.Level * 5, monster.FireResistance);
			Assert.AreEqual(monster.Level * 5, monster.FrostResistance);
			Assert.AreEqual(monster.Level * 5, monster.ArcaneResistance);
			monster = new Monster(3, MonsterType.Elemental);
			Assert.AreEqual(monster.Level * 5, monster.FireResistance);
			Assert.AreEqual(monster.Level * 5, monster.FrostResistance);
			Assert.AreEqual(monster.Level * 5, monster.ArcaneResistance);
			monster = new Monster(10, MonsterType.Dragon);
			Assert.AreEqual(monster.Level * 5, monster.FireResistance);
			Assert.AreEqual(monster.Level * 5, monster.FrostResistance);
			Assert.AreEqual(monster.Level * 5, monster.ArcaneResistance);
		}
	}
}