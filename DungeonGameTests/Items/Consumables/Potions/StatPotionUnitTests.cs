using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions {
	class StatPotionUnitTests {
		Player player;
		StatPotion potion;

		[SetUp]
		public void Setup() {
			potion = new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution);
			player = new Player("test", Player.PlayerClassType.Archer) {
				Inventory = new List<IItem>()
			};
		}

		[Test]
		public void PotionCreationTest() {
			Assert.AreEqual(1, potion.Weight);
		}

		[Test]
		public void MinorPotionCreationTest() {
			potion = new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution);

			Assert.AreEqual("minor constitution potion", potion.Name);
			Assert.AreEqual("A minor constitution potion that increases constitution by 5.", potion.Desc);
			Assert.AreEqual(5, potion.StatAmount);
			Assert.AreEqual(25, potion.ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest() {
			potion = new StatPotion(PotionStrength.Normal, StatPotion.StatType.Constitution);

			Assert.AreEqual("constitution potion", potion.Name);
			Assert.AreEqual("A constitution potion that increases constitution by 10.", potion.Desc);
			Assert.AreEqual(10, potion.StatAmount);
			Assert.AreEqual(50, potion.ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest() {
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Constitution);

			Assert.AreEqual("greater constitution potion", potion.Name);
			Assert.AreEqual("A greater constitution potion that increases constitution by 15.", potion.Desc);
			Assert.AreEqual(15, potion.StatAmount);
			Assert.AreEqual(75, potion.ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionConstitutionTest() {
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Constitution);
			player.Inventory.Add(potion);
			player.Constitution = 20;
			int oldPlayerConst = player.Constitution;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerConst + potion.StatAmount, player.Constitution);
			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(600, player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDexterityTest() {
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Dexterity);
			player.Inventory.Add(potion);
			player.Dexterity = 20;
			int oldPlayerDex = player.Dexterity;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerDex + potion.StatAmount, player.Dexterity);
			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(600, player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionIntelligenceTest() {
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Intelligence);
			player.Inventory.Add(potion);
			player.Intelligence = 20;
			int oldPlayerInt = player.Intelligence;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerInt + potion.StatAmount, player.Intelligence);
			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(600, player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionStrengthTest() {
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Strength);
			player.Inventory.Add(potion);
			player.Strength = 20;
			int oldPlayerStr = player.Strength;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerStr + potion.StatAmount, player.Strength);
			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(600, player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest() {
			OutputController.Display.ClearUserOutput();
			player.Inventory.Add(potion);
			string displayMessage = $"You drank a potion and increased Constitution by {potion.StatAmount}.";

			potion.DrinkPotion(player);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
		}
	}
}