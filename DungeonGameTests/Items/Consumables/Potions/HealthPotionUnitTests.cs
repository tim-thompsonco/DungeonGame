using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions
{
	class HealthPotionUnitTests
	{
		Player player;
		HealthPotion potion;

		[SetUp]
		public void Setup()
		{
			potion = new HealthPotion(PotionStrength.Minor);
			player = new Player("test", Player.PlayerClassType.Archer)
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
			potion = new HealthPotion(PotionStrength.Minor);

			Assert.AreEqual("minor health potion", potion._Name);
			Assert.AreEqual("A minor health potion that restores 50 health.", potion._Desc);
			Assert.AreEqual(50, potion._HealthAmount);
			Assert.AreEqual(25, potion._ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest()
		{
			potion = new HealthPotion(PotionStrength.Normal);

			Assert.AreEqual("health potion", potion._Name);
			Assert.AreEqual("A health potion that restores 100 health.", potion._Desc);
			Assert.AreEqual(100, potion._HealthAmount);
			Assert.AreEqual(50, potion._ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest()
		{
			potion = new HealthPotion(PotionStrength.Greater);

			Assert.AreEqual("greater health potion", potion._Name);
			Assert.AreEqual("A greater health potion that restores 150 health.", potion._Desc);
			Assert.AreEqual(150, potion._HealthAmount);
			Assert.AreEqual(75, potion._ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionFullHealthTest()
		{
			potion = new HealthPotion(PotionStrength.Greater);  // Greater health potion restores 150 health
			player._Inventory.Add(potion);
			player._MaxHitPoints = 200;
			player._HitPoints = 25;
			int oldPlayerHP = player._HitPoints;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerHP + potion._HealthAmount, player._HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionPartialHealthTest()
		{
			potion = new HealthPotion(PotionStrength.Greater);  // Greater health potion restores 150 health
			player._Inventory.Add(potion);
			player._MaxHitPoints = 200;
			player._HitPoints = 100;

			potion.DrinkPotion(player);

			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest()
		{
			OutputController.Display.ClearUserOutput();
			player._Inventory.Add(potion);
			string displayMessage = $"You drank a potion and replenished {potion._HealthAmount} health.";

			potion.DrinkPotion(player);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
		}
	}
}
