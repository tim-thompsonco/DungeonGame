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
			potion = new HealthPotion(1);
			player = new Player("test", Player.PlayerClassType.Archer)
			{
				_Consumables = new List<Consumable>()
			};
		}

		[Test]
		public void PotionCreationTest()
		{
			Assert.AreEqual(1, potion._Weight);
		}

		[Test]
		public void MinorPotionCreationLevelOneTest()
		{
			potion = new HealthPotion(1);

			Assert.AreEqual("minor health potion", potion._Name);
			Assert.AreEqual("A minor health potion that restores 50 health.", potion._Desc);
			Assert.AreEqual(50, potion._HealthAmount);
			Assert.AreEqual(25, potion._ItemValue);
		}

		[Test]
		public void MinorPotionCreationLevelThreeTest()
		{
			potion = new HealthPotion(3);

			Assert.AreEqual("minor health potion", potion._Name);
			Assert.AreEqual("A minor health potion that restores 50 health.", potion._Desc);
			Assert.AreEqual(50, potion._HealthAmount);
			Assert.AreEqual(25, potion._ItemValue);
		}

		[Test]
		public void NormalPotionCreationLevelFourTest()
		{
			potion = new HealthPotion(4);

			Assert.AreEqual("health potion", potion._Name);
			Assert.AreEqual("A health potion that restores 100 health.", potion._Desc);
			Assert.AreEqual(100, potion._HealthAmount);
			Assert.AreEqual(50, potion._ItemValue);
		}

		[Test]
		public void NormalPotionCreationLevelSixTest()
		{
			potion = new HealthPotion(6);

			Assert.AreEqual("health potion", potion._Name);
			Assert.AreEqual("A health potion that restores 100 health.", potion._Desc);
			Assert.AreEqual(100, potion._HealthAmount);
			Assert.AreEqual(50, potion._ItemValue);
		}

		[Test]
		public void GreaterPotionCreationLevelSevenTest()
		{
			potion = new HealthPotion(7);

			Assert.AreEqual("greater health potion", potion._Name);
			Assert.AreEqual("A greater health potion that restores 150 health.", potion._Desc);
			Assert.AreEqual(150, potion._HealthAmount);
			Assert.AreEqual(75, potion._ItemValue);
		}

		[Test]
		public void GreaterPotionCreationLevelTenTest()
		{
			potion = new HealthPotion(10);

			Assert.AreEqual("greater health potion", potion._Name);
			Assert.AreEqual("A greater health potion that restores 150 health.", potion._Desc);
			Assert.AreEqual(150, potion._HealthAmount);
			Assert.AreEqual(75, potion._ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionFullHealthTest()
		{
			potion = new HealthPotion(10);  // lvl 10 potion restores 150 health
			player._Consumables.Add(potion);
			player._MaxHitPoints = 200;
			player._HitPoints = 25;
			int oldPlayerHP = player._HitPoints;

			player.AttemptDrinkPotion("health potion");

			Assert.AreEqual(oldPlayerHP + potion._HealthAmount, player._HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionPartialHealthTest()
		{
			potion = new HealthPotion(10);  // lvl 10 potion restores 150 health
			player._Consumables.Add(potion);
			player._MaxHitPoints = 200;
			player._HitPoints = 100;

			player.AttemptDrinkPotion("health potion");

			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest()
		{
			OutputController.Display.ClearUserOutput();
			player._Consumables.Add(potion);
			string displayMessage = $"You drank a potion and replenished {potion._HealthAmount} health.";

			player.AttemptDrinkPotion("health potion");

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
		}
	}
}
