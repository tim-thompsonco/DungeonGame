using DungeonGame.Helpers;
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
			Assert.AreEqual($"A bundle of {arrows.Quantity} arrows.", arrows.Desc);
			Assert.AreEqual(1, arrows.Weight);
			Assert.AreEqual(arrowsMaxQuantity, arrows.Quantity);
			Assert.AreEqual(15, arrows.ItemValue);
		}

		[Test]
		public void PlayerHasNoQuiverUnitTest() {
			OutputHelper.Display.ClearUserOutput();

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual("You don't have a quiver to reload!", OutputHelper.Display.Output[0][2]);
		}

		[Test]
		public void PlayerQuiverIsFullUnitTest() {
			GearHelper.EquipInitialGear(player);

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player.PlayerQuiver.Quantity, player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(arrowsMaxQuantity, arrows.Quantity);
		}

		[Test]
		public void PlayerQuiverIsEmptyUnitTest() {
			GearHelper.EquipInitialGear(player);
			player.PlayerQuiver.MaxQuantity = arrowsMaxQuantity;
			player.PlayerQuiver.Quantity = 0;

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player.PlayerQuiver.Quantity, player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(0, arrows.Quantity);
		}

		[Test]
		public void PlayerQuiverIsPartiallyEmptyUnitTest() {
			GearHelper.EquipInitialGear(player);
			player.PlayerQuiver.MaxQuantity = arrowsMaxQuantity;
			player.PlayerQuiver.Quantity = 35;
			int arrowsToLoad = player.PlayerQuiver.MaxQuantity - player.PlayerQuiver.Quantity;

			arrows.LoadPlayerQuiverWithArrows(player);

			Assert.AreEqual(player.PlayerQuiver.Quantity, player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(arrowsMaxQuantity - arrowsToLoad, arrows.Quantity);
		}
	}
}
