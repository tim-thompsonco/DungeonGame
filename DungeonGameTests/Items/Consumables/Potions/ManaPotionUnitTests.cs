using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions
{
	class ManaPotionUnitTests
	{
		Player player;
		ManaPotion potion;

		[SetUp]
		public void Setup()
		{
			potion = new ManaPotion(PotionStrength.Minor);
			player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem>()
			};
		}

		[Test]
		public void PotionCreationTest()
		{
			Assert.AreEqual(1, potion._Weight);
		}

		[Test]
		public void MinorPotionCreationTest()
		{
			potion = new ManaPotion(PotionStrength.Minor);

			Assert.AreEqual("minor mana potion", potion._Name);
			Assert.AreEqual("A minor mana potion that restores 50 mana.", potion._Desc);
			Assert.AreEqual(50, potion._ManaAmount);
			Assert.AreEqual(25, potion._ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest()
		{
			potion = new ManaPotion(PotionStrength.Normal);

			Assert.AreEqual("mana potion", potion._Name);
			Assert.AreEqual("A mana potion that restores 100 mana.", potion._Desc);
			Assert.AreEqual(100, potion._ManaAmount);
			Assert.AreEqual(50, potion._ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest()
		{
			potion = new ManaPotion(PotionStrength.Greater);

			Assert.AreEqual("greater mana potion", potion._Name);
			Assert.AreEqual("A greater mana potion that restores 150 mana.", potion._Desc);
			Assert.AreEqual(150, potion._ManaAmount);
			Assert.AreEqual(75, potion._ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionFullManaTest()
		{
			potion = new ManaPotion(PotionStrength.Greater);  // Greater mana potion restores 150 mana
			player._Inventory.Add(potion);
			player._MaxManaPoints = 200;
			player._ManaPoints = 25;
			int? oldPlayerMana = player._ManaPoints;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerMana + potion._ManaAmount, player._ManaPoints);
		}

		[Test]
		public void PlayerDrinkPotionPartialManaTest()
		{
			potion = new ManaPotion(PotionStrength.Greater);  // Greater mana potion restores 150 mana
			player._Inventory.Add(potion);
			player._MaxManaPoints = 200;
			player._ManaPoints = 100;

			potion.DrinkPotion(player);

			Assert.AreEqual(player._MaxManaPoints, player._ManaPoints);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest()
		{
			OutputController.Display.ClearUserOutput();
			player._Inventory.Add(potion);
			string displayMessage = $"You drank a potion and replenished {potion._ManaAmount} mana.";

			potion.DrinkPotion(player);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
		}
	}
}
