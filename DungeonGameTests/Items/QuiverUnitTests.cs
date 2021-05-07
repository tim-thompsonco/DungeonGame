using DungeonGame.Items;
using NUnit.Framework;

namespace DungeonGameTests.Items {
	class QuiverUnitTests {
		string quiverName;
		Quiver quiver;

		[SetUp]
		public void Setup() {
			quiverName = "Test quiver";
			quiver = new Quiver(quiverName, 30, 25);
		}

		[Test]
		public void QuiverCreationTest() {
			Assert.AreEqual(quiverName, quiver.Name);
			Assert.AreEqual($"A {quiverName} that can hold {quiver.MaxQuantity} arrows.", quiver.Desc);
			Assert.AreEqual(1, quiver.Weight);
			Assert.AreEqual(30, quiver.Quantity);
			Assert.AreEqual(30, quiver.MaxQuantity);
			Assert.AreEqual(25, quiver.ItemValue);
		}

		[Test]
		public void QuiverHaveArrowsTrueTest() {
			bool quiverHasArrows = quiver.HaveArrows();

			Assert.AreEqual(quiverHasArrows, true);
		}

		[Test]
		public void QuiverUseArrowTest() {
			quiver.UseArrow();

			Assert.AreEqual(29, quiver.Quantity);
		}

		[Test]
		public void QuiverHaveArrowsFalseTest() {
			quiver.Quantity = 0;
			bool quiverHasArrows = quiver.HaveArrows();

			Assert.AreEqual(quiverHasArrows, false);
		}
	}
}
