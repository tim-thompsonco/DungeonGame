using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class GeneralUnitTests
	{
		[SetUp]
		public void Setup()
		{
		}
		[Test]
		public void ValueUnitTests()
		{
			// Test RoundNumber method in Helper class
			Assert.AreEqual(110, GameHandler.RoundNumber(107));
			Assert.AreEqual(110, GameHandler.RoundNumber(105));
			Assert.AreEqual(100, GameHandler.RoundNumber(104));
			/* Test _Monster constructor HP and exp smoothing
			if values smoothed correctly, % should be 0 */
			Monster monster = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(monster);
			Assert.AreEqual(0, monster._MaxHitPoints % 10);
			Assert.AreEqual(0, monster._ExperienceProvided % 10);
			// Test consumable potion creation
			Consumable potion = new Consumable(3, Consumable.PotionType.Health);
			Assert.AreEqual(25, potion._ItemValue);
			Assert.AreEqual("minor health potion", potion._Name);
			Assert.AreEqual(50, potion._RestoreHealth._RestoreHealthAmt);
			Consumable potionTwo = new Consumable(4, Consumable.PotionType.Health);
			Assert.AreEqual(50, potionTwo._ItemValue);
			Assert.AreEqual("health potion", potionTwo._Name);
			Assert.AreEqual(100, potionTwo._RestoreHealth._RestoreHealthAmt);
			Consumable potionThree = new Consumable(6, Consumable.PotionType.Health);
			Assert.AreEqual(50, potionThree._ItemValue);
			Assert.AreEqual("health potion", potionThree._Name);
			Assert.AreEqual(100, potionThree._RestoreHealth._RestoreHealthAmt);
			Consumable potionFour = new Consumable(7, Consumable.PotionType.Health);
			Assert.AreEqual(75, potionFour._ItemValue);
			Assert.AreEqual("greater health potion", potionFour._Name);
			Assert.AreEqual(150, potionFour._RestoreHealth._RestoreHealthAmt);
			// Test consumable gem creation
			Loot gem = new Loot(1, Loot.GemType.Amethyst);
			Assert.AreEqual(20, gem._ItemValue);
			Assert.AreEqual("chipped amethyst", gem._Name);
			Loot gemTwo = new Loot(3, Loot.GemType.Amethyst);
			Assert.AreEqual(60, gemTwo._ItemValue);
			Assert.AreEqual("chipped amethyst", gemTwo._Name);
			Loot gemThree = new Loot(4, Loot.GemType.Amethyst);
			Assert.AreEqual(80, gemThree._ItemValue);
			Assert.AreEqual("dull amethyst", gemThree._Name);
			Loot gemFour = new Loot(6, Loot.GemType.Amethyst);
			Assert.AreEqual(120, gemFour._ItemValue);
			Assert.AreEqual("dull amethyst", gemFour._Name);
			Loot gemFive = new Loot(7, Loot.GemType.Amethyst);
			Assert.AreEqual(140, gemFive._ItemValue);
			Assert.AreEqual("amethyst", gemFive._Name);
		}
		[Test]
		public void ArmorUnitTests()
		{
			// Test armor creation values
			// Test case 1, level 1 head cloth armor, armor rating should be 4 to 6
			int[] testArrClothHead = new[] { 4, 5, 6 };
			Armor testArmorClothHead = new Armor(
				1, Armor.ArmorType.Cloth, Armor.ArmorSlot.Head);
			CollectionAssert.Contains(testArrClothHead, testArmorClothHead._ArmorRating);
			// Test case 2, level 3 chest leather armor, armor rating should be 15 to 17
			int[] testArrLeatherChest = new[] { 15, 16, 17 };
			Armor testArmorLeatherChest = new Armor(
				3, Armor.ArmorType.Leather, Armor.ArmorSlot.Chest);
			CollectionAssert.Contains(testArrLeatherChest, testArmorLeatherChest._ArmorRating);
			// Test case 3, level 2 legs plate armor, armor rating should be 12 to 14
			int[] testArrPlateLegs = new[] { 12, 13, 14 };
			Armor testArmorPlateLegs = new Armor(
				2, Armor.ArmorType.Plate, Armor.ArmorSlot.Legs);
			CollectionAssert.Contains(testArrPlateLegs, testArmorPlateLegs._ArmorRating);
		}
		[Test]
		public void WeaponUnitTests()
		{
			// Test weapon creation values
			// Test case 1, weapon on level 1 skeleton based on possible weapon types
			// _Name check
			Monster skeletonLevelOne = new Monster(1, Monster.MonsterType.Skeleton);
			MonsterBuilder.BuildMonster(skeletonLevelOne);
			Weapon skeletonWeapon = skeletonLevelOne._MonsterWeapon;
			switch (skeletonWeapon.Quality)
			{
				case 1:
					switch (skeletonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped dagger", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sword (1H)", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sword (2H)", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.Axe:
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (skeletonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped sturdy dagger", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped sturdy sword (1H)", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped sturdy sword (2H)", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.Axe:
						case Weapon.WeaponType.Bow:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (skeletonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("chipped fine dagger", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("chipped fine sword (1H)", skeletonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("chipped fine sword (2H)", skeletonWeapon._Name);
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
			switch (spiderWeapon.Quality)
			{
				case 1:
					switch (spiderWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("venomous fang", spiderWeapon._Name);
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
					switch (spiderWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("sturdy venomous fang", spiderWeapon._Name);
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
					switch (spiderWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
							Assert.AreEqual("fine venomous fang", spiderWeapon._Name);
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
			switch (zombieWeapon.Quality)
			{
				case 1:
					switch (zombieWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn axe", zombieWeapon._Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (zombieWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn sturdy axe", zombieWeapon._Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (zombieWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.OneHandedSword:
						case Weapon.WeaponType.TwoHandedSword:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("worn fine axe", zombieWeapon._Name);
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
			switch (demonWeapon.Quality)
			{
				case 1:
					switch (demonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull sword (1H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sword (2H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull axe", demonWeapon._Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 2:
					switch (demonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull sturdy sword (1H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull sturdy sword (2H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull sturdy axe", demonWeapon._Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case 3:
					switch (demonWeapon.WeaponGroup)
					{
						case Weapon.WeaponType.Dagger:
						case Weapon.WeaponType.Bow:
							break;
						case Weapon.WeaponType.OneHandedSword:
							Assert.AreEqual("dull fine sword (1H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.TwoHandedSword:
							Assert.AreEqual("dull fine sword (2H)", demonWeapon._Name);
							break;
						case Weapon.WeaponType.Axe:
							Assert.AreEqual("dull fine axe", demonWeapon._Name);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
			}
		}
		[Test]
		public void PlayerMaxLevelUnitTest()
		{
			/* Player should not be able to go beyond level 10 */
			Player player = new Player("placeholder", Player.PlayerClassType.Mage) { _Level = 10 };
			player._Experience = player._ExperienceToLevel - 1;
			player._Experience++;
			PlayerHandler.LevelUpCheck(player);
			Assert.AreEqual(10, player._Level);
		}
		[Test]
		public void CheckStatusUnitTest()
		{
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new RoomBuilder(
				100, 5, 0, 4, 0).RetrieveSpawnRooms();
			GameHandler.CheckStatus(player);
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			player.CastSpell("reflect");
			for (int i = 0; i <= 30; i++)
			{
				GameHandler.CheckStatus(player);
			}
		}
		[Test]
		public void EffectUserOutputUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			RoomHandler.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 1));
			UserOutput defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
			player.CastSpell("reflect");
			OutputHandler.Display.ClearUserOutput();
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(30 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			for (int i = 0; i < 10; i++)
			{
				GameHandler.CheckStatus(player);
			}
			player._Effects.Add(new Effect("burning", Effect.EffectType.OnFire, 5,
				1, 3, 1, 10, true));
			Assert.AreEqual("Your spell reflect is slowly fading away.", OutputHandler.Display.Output[0][2]);
			player.CastSpell("rejuvenate");
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(20 seconds) Reflect", defaultEffectOutput.Output[1][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[2][0]);
			Assert.AreEqual("(30 seconds) Rejuvenate", defaultEffectOutput.Output[2][2]);
			Assert.AreEqual(Settings.FormatAttackFailText(), defaultEffectOutput.Output[3][0]);
			Assert.AreEqual("(30 seconds) Burning", defaultEffectOutput.Output[3][2]);
		}
		[Test]
		public void SaveLoadGameUnitTest()
		{
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			RoomHandler.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(1, 1, 1), new DungeonRoom(1, 1)}
			};
			player._CanSave = true;
			GameHandler.SaveGame(player);
			Assert.AreEqual("Your game has been saved.", OutputHandler.Display.Output[0][2]);
			OutputHandler.Display.ClearUserOutput();
			RoomHandler.Rooms = null;
			GameHandler.LoadGame();
			player = GameHandler.LoadPlayer();
			Assert.AreEqual("placeholder", player._Name);
			Assert.NotNull(RoomHandler.Rooms);
			Assert.AreEqual("Reloading your saved game.", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void MonsterResistanceUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in monster._MonsterItems.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			Assert.AreEqual(monster._Level * 5, monster._FireResistance);
			Assert.AreEqual(monster._Level * 5, monster._FrostResistance);
			Assert.AreEqual(monster._Level * 5, monster._ArcaneResistance);
			int arcaneResistance = monster._ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			string[] input = new[] { "cast", "lightning" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Lightning);
			string spellName = InputHandler.ParseInput(input);
			player.CastSpell(monster, spellName);
			int reducedDamage = (int)(player._Spellbook[spellIndex]._Offensive._Amount * resistanceMod);
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - reducedDamage);
		}
		[Test]
		public void PlayerResistanceUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster._ElementalCategory != Monster.ElementalType.Air)
			{
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			foreach (IEquipment item in player._Inventory.Where(item => item._Equipped))
			{
				item._Equipped = false;
			}
			Assert.AreEqual(player._Level * 5, player._FireResistance);
			Assert.AreEqual(player._Level * 5, player._FrostResistance);
			Assert.AreEqual(player._Level * 5, player._ArcaneResistance);
			int arcaneResistance = player._ArcaneResistance;
			double resistanceMod = (100 - arcaneResistance) / 100.0;
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Lightning);
			MonsterSpell.CastArcaneOffense(monster, player, spellIndex);
			int reducedDamage = (int)(monster._Spellbook[spellIndex]._Offensive._Amount * resistanceMod);
			Assert.AreEqual(player._HitPoints, player._MaxHitPoints - reducedDamage);
		}
		[Test]
		public void MonsterResistanceLevelUnitTest()
		{
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