using System;
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
			RoomHandler.Rooms = new List<IRoom> {new DungeonRoom( 1, 1, 1, 1, 1)};
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
			RoomHandler.Rooms = new List<IRoom> {new DungeonRoom(1, 1, 1, 1, 1)};
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

		[Test]
		public void GenerateTownJSON() {
			var town = new List<TownRoom>();
			var name = string.Empty;
			var desc = string.Empty;
			name = "Outside Dungeon Entrance";
			desc =
				"You are outside a rocky outcropping with a wooden door laid into the rock. It has a simple metal handle with no lock. It almost seems like it locks from the inside though. There is a bloody handprint on the door. Around you is a grassy meadow with a cobblestone path leading away from the rocky outcropping towards what looks like a town. Smoke rises from a few chimneys in the distance.";
			town.Add(new TownRoom(0, 4, 0, name, desc));
			name = "Cobblestone Path";
			desc =
				"You are walking on a cobblestone path north towards a town in the distance. Smoke rises from a few chimneys. Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			town.Add(new TownRoom(0, 5, 0, name, desc));
			town[0].North = town[1];
			town[1].South = town[0];
			desc =
				"You are walking on a cobblestone path north towards a nearby town. Smoke rises from a few chimneys. Around you is a grassy meadow and behind you is a rocky outcropping with a wooden door set into the rock.";
			town.Add(new TownRoom(0, 6, 0, name, desc));
			town[1].North = town[2];
			town[2].South = town[1];
			name = "Town Entrance";
			desc =
				"You are at the entrance to a small town. To the northeast you hear the clanking of metal on metal from what sounds like a blacksmith or armorer. There is a large fountain in the middle of the courtyard and off to the northwest are a few buildings with signs outside that you can't read from this distance.";
			town.Add(new TownRoom(0, 7, 0, name, desc));
			town[2].North = town[3];
			town[3].South = town[2];
			name = "Town - East";
			desc =
				"You are in the east part of the town. In front of you is a small building with a forge and furnace outside and a large man pounding away at a chestplate with a hammer. One building over you can see another large man running a sword against a grindstone to sharpen it.";
			town.Add(new TownRoom(1, 8, 0, name, desc, new Vendor("armorer", "A large man covered in sweat beating away at a chestplate with a hammer. He wipes his brow as you approach and wonders whether you're going to make him a little bit richer or not. You can: buy <item>, sell <item>, or <show forsale> to see what he has for sale.", Vendor.VendorType.Armorer)));
			town[3].NorthEast = town[4];
			town[4].SouthWest = town[3];
			desc =
				"You are in the east part of the town. A large man is in front of a building sharpening a sword against a grindstone. To the south, you can see a small building with a forge and furnace outside. There is another large man in front of it pounding away at a chestplate with a hammer.";
			town.Add(new TownRoom(1, 9, 0, name, desc, new Vendor("weaponsmith", "A large man covered in sweat sharpening a sword against a grindstone. He wipes his brow as you approach and wonders whether you're going to make him a little bit richer or not. You can: buy <item>, sell <item>, or <show forsale> to see what he has for sale.", Vendor.VendorType.Weaponsmith)));
			town[4].North = town[5];
			town[5].South = town[4];
			name = "Town - Center";
			desc =
				"You are in the central part of the town. There is a wrinkled old man standing in front of a small hut, his hands clasped in the arms of his robes, as he gazes around the town calmly.";
			town.Add(new TownRoom(0, 10, 0, name, desc, new Vendor("healer", "An old man covered in robes looks you up and raises an eyebrow questioningly. He can rid you of all your pain every so often. In fact, he may even provide you with some help that will be invaluable in your travels. You can buy <item>, sell <item>, or <show forsale> to see what he has for sale. You can also try to ask him to <heal> you.", Vendor.VendorType.Healer)));
			town[5].NorthWest = town[6];
			town[6].SouthEast = town[5];
			name = "Town - West";
			desc =
				"You are in the west part of the town. A woman stands in front of a building with displays of various items in front of it. It looks like she buys and sells a little bit of everything.";
			town.Add(new TownRoom(-1, 9, 0, name, desc, new Vendor("shopkeeper", "A woman in casual work clothes looks at you and asks if you want to buy anything. She raises an item to show an example of what she has for sale. You can buy <item>, sell <item>, or <show forsale> to see what she has for sale.", Vendor.VendorType.Shopkeeper)));
			town[6].SouthWest = town[7];
			town[7].NorthEast = town[6];
			desc =
				"You are in the west part of the town. There is a large, wooden building southwest of you with a sign out front that reads 'Training'. Depending on what class you are, it appears that this place might have some people who can help you learn more.";
			town.Add(new TownRoom(-1, 8, 0, name, desc));
			town[7].South = town[8];
			town[8].North = town[7];
			town[8].SouthEast = town[3];
			town[3].NorthWest = town[8];
			name = "Training Hall - Entrance";
			desc =
				"You are in the entrance of the training hall. To your west is a large room with training dummies and several people hitting them with various swords, axes and other melee weapons. To your east is another large room with training dummies. There are numerous arrows sticking out of the dummies and several people shooting the dummies with bows. To your south is one more large room with dummies. The dummies are charred because there is someone in a robe torching them with a fire spell.";
			town.Add(new TownRoom(-2, 7, 0, name, desc));
			town[8].SouthWest = town[9];
			town[9].NorthEast = town[8];
			name = "Training Hall - Warrior Guild";
			desc =
				"You are in a large room with training dummies and several people hitting them with various swords, axes and other melee weapons. A grizzled old man watches the practice, his arms folded across his chest,  sometimes nodding his head while other times cringing in disbelief. He looks like he could teach you a few things if you have the money for lessons.";
			town.Add(new TownRoom(-3, 7, 0, name, desc, new Trainer("warrior grandmaster", "A grizzled old man in a leather vest and plate gauntlets looks you up and down and wonders if you have what it takes to be a warrior. If you're ready, he can let you train <abilityname> to learn something new or upgrade <abilityname> to increase the rank on an ability that you already have. You can <show upgrades> to see the full list of options.", Trainer.TrainerCategory.Warrior)));
			town[9].West = town[10];
			town[10].East = town[9];
			name = "Training Hall - Mage Guild";
			desc =
				"You are in a large room with training dummies. The dummies are being charred by a person in a robe casting a fire spell. A middle-aged woman in an expensive-looking robe watches quietly, holding a staff upright in one hand, while she points with her other hand at the dummy and provides corrections to the trainee's incantation. She looks like she could teach you a few things if you have the money for lessons.";
			town.Add(new TownRoom(-2, 6, 0, name, desc, new Trainer("mage grandmaster", "A middle-aged woman in an expensive-looking robe, holding a staff upright, looks you up and down and wonders if you have the intelligence to be a mage. If you're ready, she can let you train <spellname> to learn a new spell or upgrade <spellname> to increase the rank on a spell that you already have. You can <show upgrades> to see the full list of options.", Trainer.TrainerCategory.Mage)));
			town[9].South = town[11];
			town[11].North = town[9];
			name = "Training Hall - Archer Guild";
			desc =
				"You are in a large room with training dummies. There are numerous arrows sticking out of the dummies and several people shooting the dummies with bows. A young woman in leather armor looks on, voicing encouragement to the trainees, and scolding them when she spots a bad habit. She looks like she could teach you a few things if you have the money for lessons.";
			town.Add(new TownRoom(-1, 7, 0, name, desc, new Trainer("archer grandmaster", "A young woman in leather armor glances at you while keeping a keen eye on her students. She looks like she has extremely fast reflexes but that glance suggested that she thought you did not. She can let you train <abilityname> to learn something new or upgrade <abilityname> to increase the rank on an ability that you already have. You can <show upgrades> to see the full list of options.", Trainer.TrainerCategory.Archer)));
			town[9].East = town[12];
			town[12].West = town[9];
			var serializerRooms = new JsonSerializer();
			serializerRooms.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
			serializerRooms.NullValueHandling = NullValueHandling.Ignore;
			serializerRooms.TypeNameHandling = TypeNameHandling.Auto;
			serializerRooms.Formatting = Formatting.Indented;
			serializerRooms.PreserveReferencesHandling = PreserveReferencesHandling.All;
			using (var sw = new StreamWriter("townrooms.json"))
			using (var writer = new JsonTextWriter(sw)) {
				serializerRooms.Serialize(writer, town, typeof(List<IRoom>));
			}
		}
	}
}