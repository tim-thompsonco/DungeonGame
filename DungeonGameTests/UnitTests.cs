using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class Tests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void UnitTests() {
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
	}
}