using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using DungeonGame.Rooms;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGameTests
{
	public class VendorUnitTests
	{
		[Test]
		public void BuyItemUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage)
			{
				_Gold = 100
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", Vendor.VendorType.Armorer));
			string[] input = new string[] { "buy", "helmet" };
			string inputName = InputController.ParseInput(input);
			int quantity;
			bool quantityProvided = int.TryParse(input.Last(), out quantity);
			if (!quantityProvided)
			{
				quantity = 1;
			}
			else
			{
				input = input.Take(input.Count() - 1).ToArray();
			}
			int baseGold = player._Gold;
			int index = room._Vendor._VendorItems.FindIndex(
				f => f._Name == inputName || f._Name.Contains(input.Last()));
			IItem buyItem = room._Vendor._VendorItems[index];
			room._Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem._Name} from the vendor for {buyItem._ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputController.Display.Output[0][2]);
			int buyItemIndex = player._Inventory.FindIndex(
				f => f._Name == inputName || f._Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem._ItemValue, player._Gold);
		}
		[Test]
		public void BuySinglePotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage)
			{
				_Gold = 100
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", Vendor.VendorType.Healer));
			string[] input = new string[] { "buy", "health", "potion" };
			string inputName = InputController.ParseInput(input);
			int quantity;
			bool quantityProvided = int.TryParse(input.Last(), out quantity);
			if (!quantityProvided)
			{
				quantity = 1;
			}
			else
			{
				input = input.Take(input.Count() - 1).ToArray();
			}
			int baseGold = player._Gold;
			int index = room._Vendor._VendorItems.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			IItem buyItem = room._Vendor._VendorItems[index];
			room._Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem._Name} from the vendor for {buyItem._ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputController.Display.Output[0][2]);
			int buyItemIndex = player._Inventory.FindIndex(
				f => f._Name == inputName || f._Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			Assert.AreEqual(baseGold - buyItem._ItemValue, player._Gold);
		}
		[Test]
		public void BuyMultiplePotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage)
			{
				_Gold = 1000,
				_Inventory = new List<IItem>()
			};
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", Vendor.VendorType.Healer));
			string[] input = new string[] { "buy", "health", "potion", "5" };
			bool quantityProvided = int.TryParse(input.Last(), out int quantity);
			if (!quantityProvided)
			{
				quantity = 1;
			}
			else
			{
				input = input.Take(input.Count() - 1).ToArray();
			}
			string inputName = InputController.ParseInput(input);
			Assert.AreEqual(5, quantity);
			int baseGold = player._Gold;
			int index = room._Vendor._VendorItems.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			IItem buyItem = room._Vendor._VendorItems[index];
			room._Vendor.BuyItem(player, input, quantity);
			string purchaseString = $"You purchased {buyItem._Name} from the vendor for {buyItem._ItemValue} gold.";
			Assert.AreEqual(purchaseString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(purchaseString, OutputController.Display.Output[1][2]);
			Assert.AreEqual(purchaseString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(purchaseString, OutputController.Display.Output[3][2]);
			Assert.AreEqual(purchaseString, OutputController.Display.Output[4][2]);
			int buyItemIndex = player._Inventory.FindIndex(
				f => f._Name == inputName || f._Name.Contains(input.Last()));
			Assert.AreNotEqual(-1, buyItemIndex);
			int potionCount = player._Inventory.OfType<IPotion>().Count();
			Assert.AreEqual(baseGold - buyItem._ItemValue * quantity, player._Gold);
		}
		[Test]
		public void SellItemUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage) { _Gold = 100 };
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", Vendor.VendorType.Armorer));
			string[] input = new[] { "sell", "cap" };
			string inputName = InputController.ParseInput(input);
			int index = player._Inventory.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			IItem sellItem = player._Inventory[index];
			int baseGold = player._Gold;
			room._Vendor.SellItem(player, input);
			string soldString = $"You sold {sellItem._Name} to the vendor for {sellItem._ItemValue} gold.";
			Assert.AreEqual(soldString, OutputController.Display.Output[0][2]);
			int sellItemIndex = player._Inventory.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			Assert.AreEqual(-1, sellItemIndex);
			Assert.AreEqual(baseGold + sellItem._ItemValue, player._Gold);
		}
		[Test]
		public void SellMultipleItemsWithSameName()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem> {
					new Armor(1, Armor.ArmorSlot.Back),
					new Armor(1, Armor.ArmorSlot.Back)
				}
			};
			player._Inventory[0]._Name = "cape";
			player._Inventory[0]._ItemValue = 10;
			player._Inventory[1]._Name = "cape";
			player._Inventory[1]._ItemValue = 5;
			TownRoom room = new TownRoom("test", "test",
				new Vendor("test", "test", Vendor.VendorType.Armorer));
			string[] input = new[] { "sell", "cape", "2" };
			room._Vendor.SellItem(player, input);
			Assert.AreEqual(1, player._Inventory.Count);
			Assert.AreEqual(10, player._Inventory[0]._ItemValue);
		}
	}
}