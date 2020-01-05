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
		}
	}
}