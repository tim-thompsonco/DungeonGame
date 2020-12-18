using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions
{
	class StatPotionUnitTests
	{
		Player player;
		StatPotion potion;

		[SetUp]
		public void Setup()
		{
			potion = new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution);
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
			potion = new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution);

			Assert.AreEqual("minor constitution potion", potion._Name);
			Assert.AreEqual("A minor constitution potion that increases constitution by 5.", potion._Desc);
			Assert.AreEqual(5, potion._StatAmount);
			Assert.AreEqual(25, potion._ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest()
		{
			potion = new StatPotion(PotionStrength.Normal, StatPotion.StatType.Constitution);

			Assert.AreEqual("constitution potion", potion._Name);
			Assert.AreEqual("A constitution potion that increases constitution by 10.", potion._Desc);
			Assert.AreEqual(10, potion._StatAmount);
			Assert.AreEqual(50, potion._ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest()
		{
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Constitution);

			Assert.AreEqual("greater constitution potion", potion._Name);
			Assert.AreEqual("A greater constitution potion that increases constitution by 15.", potion._Desc);
			Assert.AreEqual(15, potion._StatAmount);
			Assert.AreEqual(75, potion._ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionConstitutionTest()
		{
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Constitution);
			player._Inventory.Add(potion);
			player._Constitution = 20;
			int oldPlayerConst = player._Constitution;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerConst + potion._StatAmount, player._Constitution);
			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(600, player._Effects[0]._EffectMaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDexterityTest()
		{
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Dexterity);
			player._Inventory.Add(potion);
			player._Dexterity = 20;
			int oldPlayerDex = player._Dexterity;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerDex + potion._StatAmount, player._Dexterity);
			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(600, player._Effects[0]._EffectMaxRound);
		}

		[Test]
		public void PlayerDrinkPotionIntelligenceTest()
		{
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Intelligence);
			player._Inventory.Add(potion);
			player._Intelligence = 20;
			int oldPlayerInt = player._Intelligence;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerInt + potion._StatAmount, player._Intelligence);
			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(600, player._Effects[0]._EffectMaxRound);
		}

		[Test]
		public void PlayerDrinkPotionStrengthTest()
		{
			potion = new StatPotion(PotionStrength.Greater, StatPotion.StatType.Strength);
			player._Inventory.Add(potion);
			player._Strength = 20;
			int oldPlayerStr = player._Strength;

			potion.DrinkPotion(player);

			Assert.AreEqual(oldPlayerStr + potion._StatAmount, player._Strength);
			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(600, player._Effects[0]._EffectMaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest()
		{
			OutputController.Display.ClearUserOutput();
			player._Inventory.Add(potion);
			string displayMessage = $"You drank a potion and increased Constitution by {potion._StatAmount}.";

			potion.DrinkPotion(player);

			Assert.AreEqual(displayMessage, OutputController.Display.Output[0][2]);
		}
	}
}