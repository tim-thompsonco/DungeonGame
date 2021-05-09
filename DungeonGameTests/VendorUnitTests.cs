using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.ArmorObjects;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using DungeonGame.Rooms;
using DungeonGame.Vendors;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGameTests {
	public class VendorUnitTests {
		[Test]
		public void BuyItemUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage) {
				Gold = 100
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", VendorType.Armorer));
			string[] input = new string[] { "buy", "helmet" };
			string inputName = InputHelper.ParseInput(input);
			bool quantityProvided = int.TryParse(input.Last(), out int quantity);
			if (!quantityProvided) {
				quantity = 1;
			} else {
				input = input.Take(input.Count() - 1).ToArray();
			}
			int baseGold = player.Gold;
			int index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			IItem buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[0][2]);
			int buyItemIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem.ItemValue, player.Gold);
		}
		[Test]
		public void BuySinglePotionUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage) {
				Gold = 100
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", VendorType.Healer));
			string[] input = new string[] { "buy", "health", "potion" };
			string inputName = InputHelper.ParseInput(input);
			bool quantityProvided = int.TryParse(input.Last(), out int quantity);
			if (!quantityProvided) {
				quantity = 1;
			} else {
				input = input.Take(input.Count() - 1).ToArray();
			}
			int baseGold = player.Gold;
			int index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			IItem buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[0][2]);
			int buyItemIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem.ItemValue, player.Gold);
		}
		[Test]
		public void BuyMultiplePotionUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage) {
				Gold = 1000,
				Inventory = new List<IItem>()
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", VendorType.Healer));
			string[] input = new string[] { "buy", "health", "potion", "5" };
			bool quantityProvided = int.TryParse(input.Last(), out int quantity);
			if (!quantityProvided) {
				quantity = 1;
			} else {
				input = input.Take(input.Count() - 1).ToArray();
			}
			string inputName = InputHelper.ParseInput(input);
			Assert.AreEqual(5, quantity);
			int baseGold = player.Gold;
			int index = room.Vendor.VendorItems.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			IItem buyItem = room.Vendor.VendorItems[index];
			room.Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem.Name} from the vendor for {buyItem.ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[1][2]);
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[2][2]);
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[3][2]);
			Assert.AreEqual(purchaseString, OutputHelper.Display.Output[4][2]);
			int buyItemIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			int potionCount = player.Inventory.OfType<IPotion>().Count();
			Assert.AreEqual(baseGold - (buyItem.ItemValue * quantity), player.Gold);
		}
		[Test]
		public void SellItemUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage) { Gold = 100 };
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", VendorType.Armorer));
			string[] input = new[] { "sell", "cap" };
			string inputName = InputHelper.ParseInput(input);
			int index = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			IItem sellItem = player.Inventory[index];
			int baseGold = player.Gold;
			room.Vendor.SellItem(player, input);
			string soldString = $"You sold {sellItem.Name} to the vendor for {sellItem.ItemValue} gold.";
			Assert.AreEqual(soldString, OutputHelper.Display.Output[0][2]);
			int sellItemIndex = player.Inventory.FindIndex(
				f => f.Name == inputName || f.Name.Contains(inputName));
			Assert.AreEqual(-1, sellItemIndex);
			Assert.AreEqual(baseGold + sellItem.ItemValue, player.Gold);
		}
		[Test]
		public void SellMultipleItemsWithSameName() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Mage) {
				Inventory = new List<IItem> {
					new Armor(1, ArmorSlot.Back),
					new Armor(1, ArmorSlot.Back)
				}
			};
			player.Inventory[0].Name = "cape";
			player.Inventory[0].ItemValue = 10;
			player.Inventory[1].Name = "cape";
			player.Inventory[1].ItemValue = 5;
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", VendorType.Armorer));
			string[] input = new[] { "sell", "cape", "2" };
			room.Vendor.SellItem(player, input);
			Assert.AreEqual(1, player.Inventory.Count);
			Assert.AreEqual(10, player.Inventory[0].ItemValue);
		}
	}
}