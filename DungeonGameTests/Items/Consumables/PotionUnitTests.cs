using DungeonGame;
using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables
{
	class PotionUnitTests
	{
		Player player;
		Potion potion;

		[SetUp]
		public void Setup()
		{
			potion = new Potion(Potion.PotionStrength.Minor);
			player = new Player("test", Player.PlayerClassType.Archer)
			{
				_Consumables = new List<Consumable>()
			};
		}

		[Test]
		public void DrinkPotionThrowsErrorTest()
		{
			string errorMessage = string.Empty;

			try
			{
				potion.DrinkPotion(player);
			}
			catch (NotImplementedException ex)
			{
				errorMessage = ex.Message;
			}

			Assert.AreEqual("Drink potion has not been implemented.", errorMessage);
		}
	}
}
