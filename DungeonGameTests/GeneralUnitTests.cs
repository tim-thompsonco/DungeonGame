using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
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
			Assert.AreEqual(110, GameController.RoundNumber(107));
			Assert.AreEqual(110, GameController.RoundNumber(105));
			Assert.AreEqual(100, GameController.RoundNumber(104));
			/* Test _Monster constructor HP and exp smoothing
			if values smoothed correctly, % should be 0 */
			Monster monster = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(monster);
			Assert.AreEqual(0, monster._MaxHitPoints % 10);
			Assert.AreEqual(0, monster._ExperienceProvided % 10);
		}
		[Test]
		public void ArmorUnitTests() {
			// Test armor creation values
			// Test case 1, level 1 head cloth armor, armor rating should be 4 to 6
			int[] testArrClothHead = new[] { 4, 5, 6 };
			Armor testArmorClothHead = new Armor(
				1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head);
			CollectionAssert.Contains(testArrClothHead, testArmorClothHead.ArmorRating);
			// Test case 2, level 3 chest leather armor, armor rating should be 15 to 17
			int[] testArrLeatherChest = new[] { 15, 16, 17 };
			Armor testArmorLeatherChest = new Armor(
				3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			CollectionAssert.Contains(testArrLeatherChest, testArmorLeatherChest.ArmorRating);
			// Test case 3, level 2 legs plate armor, armor rating should be 12 to 14
			int[] testArrPlateLegs = new[] { 12, 13, 14 };
			Armor testArmorPlateLegs = new Armor(
				2, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs);
			CollectionAssert.Contains(testArrPlateLegs, testArmorPlateLegs.ArmorRating);
		}
		[Test]
		public void WeaponUnitTests() {
			// Test weapon creation values
			// Test case 1, weapon on level 1 skeleton based on possible weapon types
			// _Name check
			Monster skeletonLevelOne = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(skeletonLevelOne);
			Weapon skeletonWeapon = skeletonLevelOne._MonsterWeapon;
			switch (skeletonWeapon._Quality) {
				case 1:
					switch (skeletonWeapon._WeaponGroup) {
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
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (skeletonWeapon._WeaponGroup) {
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
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (skeletonWeapon._WeaponGroup) {
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
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 2, weapon on level 1 spider based on possible weapon types
			// _Name check
			Monster spiderLevelOne = new Monster(1, Monster.MonsterType.Spider);
			MonsterBuilder.BuildMonster(spiderLevelOne);
			Weapon spiderWeapon = spiderLevelOne._MonsterWeapon;
			switch (spiderWeapon._Quality) {
				case 1:
					switch (spiderWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Axe:
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (spiderWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("sturdy venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Axe:
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (spiderWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("fine venomous fang", spiderWeapon.Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Axe:
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// _Name check
			Monster zombieLevelThree = new Monster(3, Monster.MonsterType.Zombie);
			MonsterBuilder.BuildMonster(zombieLevelThree);
			Weapon zombieWeapon = zombieLevelThree._MonsterWeapon;
			switch (zombieWeapon._Quality) {
				case 1:
					switch (zombieWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (zombieWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn sturdy axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (zombieWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn fine axe", zombieWeapon.Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
			// Test case 3, weapon on level 3 zombie based on possible weapon types
			// _Name check
			Monster demonLevelTwo = new Monster(2, Monster.MonsterType.Demon);
			MonsterBuilder.BuildMonster(demonLevelTwo);
			Weapon demonWeapon = demonLevelTwo._MonsterWeapon;
			switch (demonWeapon._Quality) {
				case 1:
					switch (demonWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
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
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (demonWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
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
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (demonWeapon._WeaponGroup) {
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
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
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
		}
		[Test]
		public void PlayerMaxLevelUnitTest() {
			/* Player should not be able to go beyond level 10 */
			Player player = new Player("placeholder", Player.PlayerClassType.Mage) { _Level = 10 };
			player._Experience = player._ExperienceToLevel - 1;
			player._Experience++;
			PlayerController.LevelUpCheck(player);
			Assert.AreEqual(10, player._Level);
		}
		[Test]
		public void CheckStatusUnitTest() {
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomController._Rooms = new RoomBuilder(
				100, 5, 0, 4, 0).RetrieveSpawnRooms();
			GameController.CheckStatus(player);
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			player.CastSpell("reflect");
			for (int i = 0; i <= 30; i++) {
				GameController.CheckStatus(player);
			}
		}
		[Test]
		public void EffectUserOutputUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomController._Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			UserOutput defaultEffectOutput = OutputController.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
			player.CastSpell("reflect");
			OutputController.Display.ClearUserOutput();
			defaultEffectOutput = OutputController.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(30 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			for (int i = 0; i < 10; i++) {
				GameController.CheckStatus(player);
			}
			player._Effects.Add(new BurningEffect("burning", 3, 5));
			Assert.AreEqual("Your spell reflect is slowly fading away.", OutputController.Display.Output[0][2]);
			player.CastSpell("rejuvenate");
			defaultEffectOutput = OutputController.ShowEffects(player);
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
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			RoomController._Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player._CanSave = true;
			GameController.SaveGame(player);
			Assert.AreEqual("Your game has been saved.", OutputController.Display.Output[0][2]);
			OutputController.Display.ClearUserOutput();
			RoomController._Rooms = null;
			GameController.LoadGame();
			player = GameController.LoadPlayer();
			Assert.AreEqual("placeholder", player._Name);
			Assert.NotNull(RoomController._Rooms);
			Assert.AreEqual("Reloading your saved game.", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void MonsterResistanceUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon) { _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			Assert.AreEqual(monster._Level * 5, monster._FireResistance);
			Assert.AreEqual(monster._Level * 5, monster._FrostResistance);
			Assert.AreEqual(monster._Level * 5, monster._ArcaneResistance);
			int arcaneResistance = monster._ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			string[] input = new[] { "cast", "lightning" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Lightning);
			string spellName = InputController.ParseInput(input);
			player.CastSpell(monster, spellName);
			int reducedDamage = (int)(player._Spellbook[spellIndex]._Offensive._Amount * resistanceMod);
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - reducedDamage);
		}
		[Test]
		public void PlayerResistanceUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster._ElementalCategory != Monster.ElementalType.Air) {
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			Assert.AreEqual(player._Level * 5, player._FireResistance);
			Assert.AreEqual(player._Level * 5, player._FrostResistance);
			Assert.AreEqual(player._Level * 5, player._ArcaneResistance);
			int arcaneResistance = player._ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Lightning);
			monster._Spellbook[spellIndex].CastArcaneOffense(monster, player, spellIndex);
			int reducedDamage = (int)(monster._Spellbook[spellIndex]._Offensive._Amount * resistanceMod);
			Assert.AreEqual(player._HitPoints, player._MaxHitPoints - reducedDamage);
		}
		[Test]
		public void MonsterResistanceLevelUnitTest() {
			Monster monster = new Monster(1, Monster.MonsterType.Vampire);
			Assert.AreEqual(monster._Level * 5, monster._FireResistance);
			Assert.AreEqual(monster._Level * 5, monster._FrostResistance);
			Assert.AreEqual(monster._Level * 5, monster._ArcaneResistance);
			monster = new Monster(3, Monster.MonsterType.Elemental);
			Assert.AreEqual(monster._Level * 5, monster._FireResistance);
			Assert.AreEqual(monster._Level * 5, monster._FrostResistance);
			Assert.AreEqual(monster._Level * 5, monster._ArcaneResistance);
			monster = new Monster(10, Monster.MonsterType.Dragon);
			Assert.AreEqual(monster._Level * 5, monster._FireResistance);
			Assert.AreEqual(monster._Level * 5, monster._FrostResistance);
			Assert.AreEqual(monster._Level * 5, monster._ArcaneResistance);
		}
	}
}