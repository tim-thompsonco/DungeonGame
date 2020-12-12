using DungeonGame.Items;
using NUnit.Framework;

namespace DungeonGameTests.Items
{
	class QuiverUnitTests
	{
		string quiverName;
		Quiver quiver;

		[SetUp]
		public void Setup()
		{
			quiverName = "Test quiver";
			quiver = new Quiver(quiverName, 30, 25);
		}

		[Test]
		public void QuiverCreationTest()
		{
			Assert.AreEqual(quiverName, quiver._Name);
			Assert.AreEqual($"A {quiverName} that can hold {quiver._MaxQuantity} arrows.", quiver._Desc);
			Assert.AreEqual(1, quiver._Weight);
			Assert.AreEqual(30, quiver._Quantity);
			Assert.AreEqual(30, quiver._MaxQuantity);
			Assert.AreEqual(25, quiver._ItemValue);
		}

		[Test]
		public void QuiverHaveArrowsTrueTest()
		{
			bool quiverHasArrows = quiver.HaveArrows();

			Assert.AreEqual(quiverHasArrows, true);
		}

		[Test]
		public void QuiverUseArrowTest()
		{
			quiver.UseArrow();

			Assert.AreEqual(29, quiver._Quantity);
		}

		[Test]
		public void QuiverHaveArrowsFalseTest()
		{
			quiver._Quantity = 0;
			bool quiverHasArrows = quiver.HaveArrows();

			Assert.AreEqual(quiverHasArrows, false);
		}
	}
}
