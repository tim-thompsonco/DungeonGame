using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class VendorUnitTests {
		[Test]
		public void BuyItemUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			player.Gold = 100;
			var room = new TownRoom("test", "test", 
				new Vendor("test", "test", Vendor.VendorType.Armorer));
			var input = new string[] {"buy", "helmet"};
			var inputName = InputHandler.ParseInput(input);
			int quantity;
			bool quantityProvided = int.TryParse(input.Last(), out quantity);
			if (!quantityProvided) {
				quantity = 1;
			}
			else {
				input[input.Length - 1] = "";
			}
			var baseGold = player.Gold;
			var index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			var buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[0][2]);
			var buyItemIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem.ItemValue, player.Gold);
		}
		[Test]
		public void BuySinglePotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			player.Gold = 100;
			var room = new TownRoom("test", "test", 
				new Vendor("test", "test", Vendor.VendorType.Healer));
			var input = new string[] {"buy", "health", "potion"};
			var inputName = InputHandler.ParseInput(input);
			int quantity;
			bool quantityProvided = int.TryParse(input.Last(), out quantity);
			if (!quantityProvided) {
				quantity = 1;
			}
			else {
				input[input.Length - 1] = "";
			}
			var baseGold = player.Gold;
			var index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			var buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[0][2]);
			var buyItemIndex = player.Consumables.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem.ItemValue, player.Gold);
		}
		[Test]
		public void BuyMultiplePotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			player.Gold = 1000;
			player.Consumables = null;
			var room = new TownRoom("test", "test", 
				new Vendor("test", "test", Vendor.VendorType.Healer));
			var input = new string[] {"buy", "health", "potion", "5"};
			int quantity;
			bool quantityProvided = int.TryParse(input.Last(), out quantity);
			if (!quantityProvided) {
				quantity = 1;
			}
			else {
				input[input.Length - 1] = "";
			}
			var inputName = InputHandler.ParseInput(input);
			Assert.AreEqual(5, quantity);
			var baseGold = player.Gold;
			var index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			var buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			var purchaseString = "You purchased " + buyItem.Name + " from the vendor for " + buyItem.ItemValue + " gold.";
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[1][2]);
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[3][2]);
			Assert.AreEqual(purchaseString, OutputHandler.Display.Output[4][2]);
			var buyItemIndex = player.Consumables.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			var potionCount = player.Consumables.OfType<Consumable.PotionType>().Count();
			Assert.AreEqual(baseGold - buyItem.ItemValue * 5, player.Gold);
		}
	}
}