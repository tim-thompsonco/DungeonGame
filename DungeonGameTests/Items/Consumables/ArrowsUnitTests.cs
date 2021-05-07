using DungeonGame.Controllers;
using DungeonGame.Items.Consumables;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Items {
	class ArrowsUnitTests {
		string arrowsName;
		int arrowsMaxQuantity;
		Arrows arrows;
		Player player;

		[SetUp]
		public void Setup() {
			arrowsName = "arrows";
			arrowsMaxQuantity = 50;
			arrows = new Arrows(arrowsName, 15, Arrows.ArrowType.Standard);
			player = new Player("test", Player.PlayerClassType.Archer);
		}

		[Test]
		public void ArrowsCreationTest() {
			Assert.AreEqual(arrowsName, arrows.Name);
			Assert.AreEqual($"A bundle of {arrows._Quantity} arrows.", arrows._Desc);
			Assert.AreEqual(1, arrows._Weight);
			Assert.AreEqual(arrowsMaxQuantity, arrows._Quantity);
			Assert.AreEqual(15, arrows._ItemValue);
		}

		[Test]
		public void PlayerHasNoQuiverUnitTest() {
			OutputController.Display.ClearUserOutput();

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual("You don't have a quiver to reload!", OutputController.Display._Output[0][2]);
		}

		[Test]
		public void PlayerQuiverIsFullUnitTest() {
			GearController.EquipInitialGear(player);

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player._PlayerQuiver._Quantity, player._PlayerQuiver._MaxQuantity);
			Assert.AreEqual(arrowsMaxQuantity, arrows._Quantity);
		}

		[Test]
		public void PlayerQuiverIsEmptyUnitTest() {
			GearController.EquipInitialGear(player);
			player._PlayerQuiver._MaxQuantity = arrowsMaxQuantity;
			player._PlayerQuiver._Quantity = 0;

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player._PlayerQuiver._Quantity, player._PlayerQuiver._MaxQuantity);
			Assert.AreEqual(0, arrows._Quantity);
		}

		[Test]
		public void PlayerQuiverIsPartiallyEmptyUnitTest() {
			GearController.EquipInitialGear(player);
			player._PlayerQuiver._MaxQuantity = arrowsMaxQuantity;
			player._PlayerQuiver._Quantity = 35;
			int arrowsToLoad = player._PlayerQuiver._MaxQuantity - player._PlayerQuiver._Quantity;

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player._PlayerQuiver._Quantity, player._PlayerQuiver._MaxQuantity);
			Assert.AreEqual(arrowsMaxQuantity - arrowsToLoad, arrows._Quantity);
		}
	}
}
