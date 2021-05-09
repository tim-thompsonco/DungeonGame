using DungeonGame.Helpers;
using DungeonGame.Items.Consumables.Arrow;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Items {
	internal class ArrowsUnitTests {
		private string _arrowsName;
		private int _arrowsMaxQuantity;
		private Arrows _arrows;
		private Player _player;

		[SetUp]
		public void Setup() {
			_arrowsName = "arrows";
			_arrowsMaxQuantity = 50;
			_arrows = new Arrows(_arrowsName, 15, ArrowType.Standard);
			_player = new Player("test", PlayerClassType.Archer);
		}

		[Test]
		public void ArrowsCreationTest() {
			Assert.AreEqual(_arrowsName, _arrows.Name);
			Assert.AreEqual($"A bundle of {_arrows.Quantity} arrows.", _arrows.Desc);
			Assert.AreEqual(1, _arrows.Weight);
			Assert.AreEqual(_arrowsMaxQuantity, _arrows.Quantity);
			Assert.AreEqual(15, _arrows.ItemValue);
		}

		[Test]
		public void PlayerHasNoQuiverUnitTest() {
			OutputHelper.Display.ClearUserOutput();

			_arrows.LoadPlayerQuiverWithArrows(_player);

			Assert.AreEqual("You don't have a quiver to reload!", OutputHelper.Display.Output[0][2]);
		}

		[Test]
		public void PlayerQuiverIsFullUnitTest() {
			GearHelper.EquipInitialGear(_player);

			_arrows.LoadPlayerQuiverWithArrows(_player);

			Assert.AreEqual(_player.PlayerQuiver.Quantity, _player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(_arrowsMaxQuantity, _arrows.Quantity);
		}

		[Test]
		public void PlayerQuiverIsEmptyUnitTest() {
			GearHelper.EquipInitialGear(_player);
			_player.PlayerQuiver.MaxQuantity = _arrowsMaxQuantity;
			_player.PlayerQuiver.Quantity = 0;

			_arrows.LoadPlayerQuiverWithArrows(_player);

			Assert.AreEqual(_player.PlayerQuiver.Quantity, _player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(0, _arrows.Quantity);
		}

		[Test]
		public void PlayerQuiverIsPartiallyEmptyUnitTest() {
			GearHelper.EquipInitialGear(_player);
			_player.PlayerQuiver.MaxQuantity = _arrowsMaxQuantity;
			_player.PlayerQuiver.Quantity = 35;
			int arrowsToLoad = _player.PlayerQuiver.MaxQuantity - _player.PlayerQuiver.Quantity;

			_arrows.LoadPlayerQuiverWithArrows(_player);

			Assert.AreEqual(_player.PlayerQuiver.Quantity, _player.PlayerQuiver.MaxQuantity);
			Assert.AreEqual(_arrowsMaxQuantity - arrowsToLoad, _arrows.Quantity);
		}
	}
}
