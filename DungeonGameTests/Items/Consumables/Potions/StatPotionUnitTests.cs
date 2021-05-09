using DungeonGame;
using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace DungeonGameTests.Items.Consumables.Potions {
	internal class StatPotionUnitTests {
		private Player _player;
		private StatPotion _potion;

		[SetUp]
		public void Setup() {
			_potion = new StatPotion(PotionStrength.Minor, StatType.Constitution);
			_player = new Player("test", PlayerClassType.Archer) {
				Inventory = new List<IItem>()
			};
		}

		[Test]
		public void PotionCreationTest() {
			Assert.AreEqual(1, _potion.Weight);
		}

		[Test]
		public void MinorPotionCreationTest() {
			_potion = new StatPotion(PotionStrength.Minor, StatType.Constitution);

			Assert.AreEqual("minor constitution potion", _potion.Name);
			Assert.AreEqual("A minor constitution potion that increases constitution by 5.", _potion.Desc);
			Assert.AreEqual(5, _potion.StatAmount);
			Assert.AreEqual(25, _potion.ItemValue);
		}

		[Test]
		public void NormalPotionCreationTest() {
			_potion = new StatPotion(PotionStrength.Normal, StatType.Constitution);

			Assert.AreEqual("constitution potion", _potion.Name);
			Assert.AreEqual("A constitution potion that increases constitution by 10.", _potion.Desc);
			Assert.AreEqual(10, _potion.StatAmount);
			Assert.AreEqual(50, _potion.ItemValue);
		}

		[Test]
		public void GreaterPotionCreationTest() {
			_potion = new StatPotion(PotionStrength.Greater, StatType.Constitution);

			Assert.AreEqual("greater constitution potion", _potion.Name);
			Assert.AreEqual("A greater constitution potion that increases constitution by 15.", _potion.Desc);
			Assert.AreEqual(15, _potion.StatAmount);
			Assert.AreEqual(75, _potion.ItemValue);
		}

		[Test]
		public void PlayerDrinkPotionConstitutionTest() {
			_potion = new StatPotion(PotionStrength.Greater, StatType.Constitution);
			_player.Inventory.Add(_potion);
			_player.Constitution = 20;
			int oldPlayerConst = _player.Constitution;

			_potion.DrinkPotion(_player);

			Assert.AreEqual(oldPlayerConst + _potion.StatAmount, _player.Constitution);
			Assert.AreEqual(1, _player.Effects.Count);
			Assert.AreEqual(600, _player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDexterityTest() {
			_potion = new StatPotion(PotionStrength.Greater, StatType.Dexterity);
			_player.Inventory.Add(_potion);
			_player.Dexterity = 20;
			int oldPlayerDex = _player.Dexterity;

			_potion.DrinkPotion(_player);

			Assert.AreEqual(oldPlayerDex + _potion.StatAmount, _player.Dexterity);
			Assert.AreEqual(1, _player.Effects.Count);
			Assert.AreEqual(600, _player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionIntelligenceTest() {
			_potion = new StatPotion(PotionStrength.Greater, StatType.Intelligence);
			_player.Inventory.Add(_potion);
			_player.Intelligence = 20;
			int oldPlayerInt = _player.Intelligence;

			_potion.DrinkPotion(_player);

			Assert.AreEqual(oldPlayerInt + _potion.StatAmount, _player.Intelligence);
			Assert.AreEqual(1, _player.Effects.Count);
			Assert.AreEqual(600, _player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionStrengthTest() {
			_potion = new StatPotion(PotionStrength.Greater, StatType.Strength);
			_player.Inventory.Add(_potion);
			_player.Strength = 20;
			int oldPlayerStr = _player.Strength;

			_potion.DrinkPotion(_player);

			Assert.AreEqual(oldPlayerStr + _potion.StatAmount, _player.Strength);
			Assert.AreEqual(1, _player.Effects.Count);
			Assert.AreEqual(600, _player.Effects[0].MaxRound);
		}

		[Test]
		public void PlayerDrinkPotionDisplayMessageTest() {
			OutputHelper.Display.ClearUserOutput();
			_player.Inventory.Add(_potion);
			string displayMessage = $"You drank a potion and increased Constitution by {_potion.StatAmount}.";

			_potion.DrinkPotion(_player);

			Assert.AreEqual(displayMessage, OutputHelper.Display.Output[0][2]);
		}
	}
}