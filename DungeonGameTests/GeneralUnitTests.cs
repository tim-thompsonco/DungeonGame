using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DungeonGame;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DungeonGameTests {
	public class GeneralUnitTests {
		[SetUp]
		public void Setup() {
		}
		[Test]
		public void ValueUnitTests() {
			// Test RoundNumber method in Helper class
			Assert.AreEqual(110, GameHandler.RoundNumber(107));
			Assert.AreEqual(110, GameHandler.RoundNumber(105));
			Assert.AreEqual(100, GameHandler.RoundNumber(104));
			/* Test Monster constructor HP and exp smoothing
			if values smoothed correctly, % should be 0 */
			var monster = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(monster);
			Assert.AreEqual(0, monster.MaxHitPoints % 10);
			Assert.AreEqual(0, monster.ExperienceProvided % 10);
			// Test consumable potion creation
			var potion = new Consumable(3, Consumable.PotionType.Health);
			Assert.AreEqual(25, potion.ItemValue);
			Assert.AreEqual("minor health potion", potion.Name);
			Assert.AreEqual(50, potion.RestoreHealth.RestoreHealthAmt);
			var potionTwo = new Consumable(4, Consumable.PotionType.Health);
			Assert.AreEqual(50, potionTwo.ItemValue);
			Assert.AreEqual("health potion", potionTwo.Name);
			Assert.AreEqual(100, potionTwo.RestoreHealth.RestoreHealthAmt);
			var potionThree = new Consumable(6, Consumable.PotionType.Health);
			Assert.AreEqual(50, potionThree.ItemValue);
			Assert.AreEqual("health potion", potionThree.Name);
			Assert.AreEqual(100, potionThree.RestoreHealth.RestoreHealthAmt);
			var potionFour = new Consumable(7, Consumable.PotionType.Health);
			Assert.AreEqual(75, potionFour.ItemValue);
			Assert.AreEqual("greater health potion", potionFour.Name);
			Assert.AreEqual(150, potionFour.RestoreHealth.RestoreHealthAmt);
			// Test consumable gem creation
			var gem = new Loot(1, Loot.GemType.Amethyst);
			Assert.AreEqual(20, gem.ItemValue);
			Assert.AreEqual("chipped amethyst", gem.Name);
			var gemTwo = new Loot(3, Loot.GemType.Amethyst);
			Assert.AreEqual(60, gemTwo.ItemValue);
			Assert.AreEqual("chipped amethyst", gemTwo.Name);
			var gemThree = new Loot(4, Loot.GemType.Amethyst);
			Assert.AreEqual(80, gemThree.ItemValue);
			Assert.AreEqual("dull amethyst", gemThree.Name);
			var gemFour = new Loot(6, Loot.GemType.Amethyst);
			Assert.AreEqual(120, gemFour.ItemValue);
			Assert.AreEqual("dull amethyst", gemFour.Name);
			var gemFive = new Loot(7, Loot.GemType.Amethyst);
			Assert.AreEqual(140, gemFive.ItemValue);
			Assert.AreEqual("amethyst", gemFive.Name);
		}
		[Test]
		public void ArmorUnitTests() {
			// Test armor creation values
			// Test case 1, level 1 head cloth armor, armor rating should be 4 to 6
			var testArrClothHead = new [] {4, 5, 6};
			var testArmorClothHead = new Armor(
				1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head);
			CollectionAssert.Contains(testArrClothHead, testArmorClothHead.ArmorRating);
			// Test case 2, level 3 chest leather armor, armor rating should be 15 to 17
			var testArrLeatherChest = new [] {15, 16, 17};
			var testArmorLeatherChest = new Armor(
				3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			CollectionAssert.Contains(testArrLeatherChest, testArmorLeatherChest.ArmorRating);
			// Test case 3, level 2 legs plate armor, armor rating should be 12 to 14
			var testArrPlateLegs = new [] {12, 13, 14};
			var testArmorPlateLegs = new Armor(
				2, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs);
			CollectionAssert.Contains(testArrPlateLegs, testArmorPlateLegs.ArmorRating);
		}
		[Test]
		public void WeaponUnitTests() {
		// Test weapon creation values
			// Test case 1, weapon on level 1 skeleton based on possible weapon types
			// Name check
			var skeletonLevelOne = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(skeletonLevelOne);
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
			MonsterBuilder.BuildMonster(spiderLevelOne);
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
			MonsterBuilder.BuildMonster(zombieLevelThree);
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
			MonsterBuilder.BuildMonster(demonLevelTwo);
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
		public void PlayerMaxLevelUnitTest() {
			/* Player should not be able to go beyond level 10 */
			var player = new Player("placeholder", Player.PlayerClassType.Mage) {Level = 10};
			player.Experience = player.ExperienceToLevel - 1;
			player.Experience++;
			PlayerHandler.LevelUpCheck(player);
			Assert.AreEqual(10, player.Level);
		}
		[Test]
		public void CheckStatusUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new RoomBuilder(
				100, 5, 0, 4, 0).RetrieveSpawnRooms();
			GameHandler.CheckStatus(player);
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			player.CastSpell("reflect");
			for (var i = 0; i <= 30; i++) {
				GameHandler.CheckStatus(player);
			}
		}
		[Test]
		public void EffectUserOutputUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
			player.CastSpell("reflect");
			OutputHandler.Display.ClearUserOutput();
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(30 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 10; i++) {
				GameHandler.CheckStatus(player);
			}
			player.Effects.Add(new Effect("burning", Effect.EffectType.OnFire, 5, 
				1, 3, 1, 10, true));
			Assert.AreEqual("Your spell reflect is slowly fading away.", OutputHandler.Display.Output[0][2]);
			player.CastSpell("rejuvenate");
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(20 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[2][0]);
			Assert.AreEqual("(30 seconds) Rejuvenate", defaultEffectOutput.Output[2][2]);
			Assert.AreEqual(Settings.FormatAttackFailText(), defaultEffectOutput.Output[3][0]);
			Assert.AreEqual("(30 seconds) Burning", defaultEffectOutput.Output[3][2]);
		}
		[Test]
		public void SaveLoadGameUnitTest() {
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			RoomHandler.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player.CanSave = true;
			GameHandler.SaveGame(player);
			Assert.AreEqual( "Your game has been saved.", OutputHandler.Display.Output[0][2]);
			OutputHandler.Display.ClearUserOutput();
			RoomHandler.Rooms = null;
			GameHandler.LoadGame();
			player = GameHandler.LoadPlayer();
			Assert.AreEqual("placeholder", player.Name);
			Assert.NotNull(RoomHandler.Rooms);
			Assert.AreEqual("Reloading your saved game.", OutputHandler.Display.Output[1][2]);
		}
	}
}