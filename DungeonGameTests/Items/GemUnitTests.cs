using DungeonGame.Items;
using NUnit.Framework;

namespace DungeonGameTests.Items {
	public class GemUnitTests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void GemCreationUnitTestLevelOne() {
			Gem gem = new Gem(1, Gem.GemType.Amethyst);
			string gemDesc = $"A {gem.Name} that is worth some money to the right vendor.";

			Assert.AreEqual(1, gem.Weight);
			Assert.AreEqual(20, gem.ItemValue);
			Assert.AreEqual("chipped amethyst", gem.Name);
			Assert.AreEqual(gemDesc, gem.Desc);
		}

		[Test]
		public void GemCreationUnitTestLevelThree() {
			Gem gem = new Gem(3, Gem.GemType.Amethyst);
			string gemDesc = $"A {gem.Name} that is worth some money to the right vendor.";

			Assert.AreEqual(1, gem.Weight);
			Assert.AreEqual(60, gem.ItemValue);
			Assert.AreEqual("chipped amethyst", gem.Name);
			Assert.AreEqual(gemDesc, gem.Desc);
		}

		[Test]
		public void GemCreationUnitTestLevelFour() {
			Gem gem = new Gem(4, Gem.GemType.Amethyst);
			string gemDesc = $"A {gem.Name} that is worth some money to the right vendor.";

			Assert.AreEqual(1, gem.Weight);
			Assert.AreEqual(80, gem.ItemValue);
			Assert.AreEqual("dull amethyst", gem.Name);
			Assert.AreEqual(gemDesc, gem.Desc);
		}

		[Test]
		public void GemCreationUnitTestLevelSix() {
			Gem gem = new Gem(6, Gem.GemType.Amethyst);
			string gemDesc = $"A {gem.Name} that is worth some money to the right vendor.";

			Assert.AreEqual(1, gem.Weight);
			Assert.AreEqual(120, gem.ItemValue);
			Assert.AreEqual("dull amethyst", gem.Name);
			Assert.AreEqual(gemDesc, gem.Desc);
		}

		[Test]
		public void GemCreationUnitTestLevelSeven() {
			Gem gem = new Gem(7, Gem.GemType.Amethyst);
			string gemDesc = $"A {gem.Name} that is worth some money to the right vendor.";

			Assert.AreEqual(1, gem.Weight);
			Assert.AreEqual(140, gem.ItemValue);
			Assert.AreEqual("amethyst", gem.Name);
			Assert.AreEqual(gemDesc, gem.Desc);
		}
	}
}
