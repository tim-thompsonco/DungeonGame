using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions {
	internal class HealthPotionUnitTests {
		private Player player;
		private HealthPotion potion;

		[SetUp]
		public void Setup() {
			potion = new HealthPotion(PotionStrength.Minor);
			player = new Player("test", PlayerClassType.Archer) {
				Inventory = new List<IItem>()
			};
		}

		[Test]
		public void PotionCreationTest() {
			Assert.AreEqual(1, potion.Weight);
		}

		[Test]
		public void MinorPotionCreationTest() {
			potion = new HealthPotion(PotionStrength.Minor);

			Assert.AreEqual("minor health potion", potion.Name);
			Assert.AreEqual("A minor health potion that restores 50 health.", potion.Desc);
			Assert.AreEqual(50, potion.HealthAmount);
			Assert.AreEqual(25, potion.ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest() {
			potion = new HealthPotion(PotionStrength.Normal);

			Assert.AreEqual("health potion", potion.Name);
			Assert.AreEqual("A health potion that restores 100 health.", potion.Desc);
			Assert.AreEqual(100, potion.HealthAmount);
			Assert.AreEqual(50, potion.ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest() {
			potion = new HealthPotion(PotionStrength.Greater);

			Assert.AreEqual("greater health potion", potion.Name);
			Assert.AreEqual("A greater health potion that restores 150 health.", potion.Desc);
			Assert.AreEqual(150, potion.HealthAmount);
			Assert.AreEqual(75, potion.ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionFullHealthTest() {
			potion = new HealthPotion(PotionStrength.Greater);  // Greater health potion restores 150 health
			player.Inventory.Add(potion);
			player.MaxHitPoints = 200;
			player.HitPoints = 25;
			int oldPlayerHP = player.HitPoints;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerHP + potion.HealthAmount, player.HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionPartialHealthTest() {
			potion = new HealthPotion(PotionStrength.Greater);  // Greater health potion restores 150 health
			player.Inventory.Add(potion);
			player.MaxHitPoints = 200;
			player.HitPoints = 100;

			potion.DrinkPotion(player);

			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest() {
			OutputHelper.Display.ClearUserOutput();
			player.Inventory.Add(potion);
			string displayMessage = $"You drank a potion and replenished {potion.HealthAmount} health.";

			potion.DrinkPotion(player);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
		}
	}
}
