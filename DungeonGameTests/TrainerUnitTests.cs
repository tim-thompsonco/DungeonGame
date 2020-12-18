using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class TrainerUnitTests
	{
		[Test]
		public void UpgradeSpellTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Mage);
			Trainer trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			player._PlayerClass = Player.PlayerClassType.Archer;
			const string spellName = "fireball";
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual("You can't upgrade spells. You're not a mage!",
					OutputController.Display.Output[0][2]);
			player._PlayerClass = Player.PlayerClassType.Mage;
			int spellIndex = player._Spellbook.FindIndex(
				f => f._Name == spellName);
			Assert.AreEqual(25, player._Spellbook[spellIndex]._Offensive._Amount);
			Assert.AreEqual(5, player._Spellbook[spellIndex]._Offensive._AmountOverTime);
			player._Gold = 0;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual(25, player._Spellbook[spellIndex]._Offensive._Amount);
			Assert.AreEqual(5, player._Spellbook[spellIndex]._Offensive._AmountOverTime);
			Assert.AreEqual("You are not ready to upgrade that spell. You need to level up first!",
				OutputController.Display.Output[1][2]);
			trainer.UpgradeSpell(player, "not a spell");
			Assert.AreEqual("You don't have that spell to train!", OutputController.Display.Output[2][2]);
			player._Intelligence = 20;
			player._Level = 2;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual("You can't afford that!", OutputController.Display.Output[3][2]);
			player._Gold = 100;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual(2, player._Spellbook[spellIndex]._Rank);
			Assert.AreEqual(35, player._Spellbook[spellIndex]._Offensive._Amount);
			Assert.AreEqual(10, player._Spellbook[spellIndex]._Offensive._AmountOverTime);
			Assert.AreEqual(60, player._Gold);
			Assert.AreEqual("You upgraded Fireball to Rank 2 for 40 gold.",
				OutputController.Display.Output[4][2]);
		}
		[Test]
		public void UpgradeAbilityTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Archer);
			Trainer trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Archer);
			player._PlayerClass = Player.PlayerClassType.Mage;
			const string abilityName = "distance";
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual("You can't upgrade abilities. You're not a warrior or archer!",
				OutputController.Display.Output[0][2]);
			player._PlayerClass = Player.PlayerClassType.Archer;
			int abilityIndex = player._Abilities.FindIndex(
				f => f._Name == abilityName || f._Name.Contains(abilityName));
			Assert.AreEqual(25, player._Abilities[abilityIndex]._Offensive._Amount);
			Assert.AreEqual(50, player._Abilities[abilityIndex]._Offensive._ChanceToSucceed);
			player._Gold = 0;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual(25, player._Abilities[abilityIndex]._Offensive._Amount);
			Assert.AreEqual(50, player._Abilities[abilityIndex]._Offensive._ChanceToSucceed);
			Assert.AreEqual("You are not ready to upgrade that ability. You need to level up first!",
				OutputController.Display.Output[1][2]);
			trainer.UpgradeAbility(player, "not an ability");
			Assert.AreEqual("You don't have that ability to train!", OutputController.Display.Output[2][2]);
			player._Intelligence = 20;
			player._Level = 2;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual("You can't afford that!", OutputController.Display.Output[3][2]);
			player._Gold = 100;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual(2, player._Abilities[abilityIndex]._Rank);
			Assert.AreEqual(35, player._Abilities[abilityIndex]._Offensive._Amount);
			Assert.AreEqual(55, player._Abilities[abilityIndex]._Offensive._ChanceToSucceed);
			Assert.AreEqual(60, player._Gold);
			Assert.AreEqual("You upgraded Distance Shot to Rank 2 for 40 gold.",
				OutputController.Display.Output[4][2]);
		}
		[Test]
		public void TrainAbilityTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Warrior);
			Trainer trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Warrior);
			player._PlayerClass = Player.PlayerClassType.Mage;
			const string abilityName = "bandage";
			trainer.TrainAbility(player, abilityName);
			Assert.AreEqual("You can't train abilities. You're not a warrior or archer!",
				OutputController.Display.Output[0][2]);
			player._PlayerClass = Player.PlayerClassType.Warrior;
			int abilityIndex = player._Abilities.FindIndex(
				f => f._Name == abilityName || f._Name.Contains(abilityName));
			Assert.AreEqual(-1, abilityIndex);
			player._Gold = 0;
			trainer.TrainAbility(player, abilityName);
			abilityIndex = player._Abilities.FindIndex(
				f => f._Name == abilityName || f._Name.Contains(abilityName));
			Assert.AreEqual(-1, abilityIndex);
			Assert.AreEqual("You are not ready to train that ability. You need to level up first!",
				OutputController.Display.Output[1][2]);
			player._Intelligence = 20;
			player._Level = 2;
			trainer.TrainAbility(player, abilityName);
			Assert.AreEqual("You can't afford that!", OutputController.Display.Output[2][2]);
			player._Gold = 100;
			trainer.TrainAbility(player, abilityName);
			abilityIndex = player._Abilities.FindIndex(
				f => f._Name == abilityName || f._Name.Contains(abilityName));
			Assert.AreNotEqual(-1, abilityIndex);
			Assert.AreEqual(60, player._Gold);
			Assert.AreEqual("You purchased Bandage (Rank 1) for 40 gold.",
				OutputController.Display.Output[3][2]);
		}
		[Test]
		public void TrainSpellTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("placeholder", Player.PlayerClassType.Warrior);
			Trainer trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			const string spellName = "frost nova";
			trainer.TrainSpell(player, spellName);
			Assert.AreEqual("You can't train spells. You're not a mage!", OutputController.Display.Output[0][2]);
			player = new Player("placeholder", Player.PlayerClassType.Mage);
			int spellIndex = player._Spellbook.FindIndex(
				f => f._Name == spellName || f._Name.Contains(spellName));
			Assert.AreEqual(-1, spellIndex);
			player._Gold = 0;
			trainer.TrainSpell(player, spellName);
			spellIndex = player._Spellbook.FindIndex(
				f => f._Name == spellName || f._Name.Contains(spellName));
			Assert.AreEqual(-1, spellIndex);
			Assert.AreEqual("You are not ready to train that spell. You need to level up first!",
				OutputController.Display.Output[1][2]);
			player._Intelligence = 20;
			player._Level = 8;
			trainer.TrainSpell(player, spellName);
			Assert.AreEqual("You can't afford that!", OutputController.Display.Output[2][2]);
			player._Gold = 200;
			trainer.TrainSpell(player, spellName);
			spellIndex = player._Spellbook.FindIndex(
				f => f._Name == spellName || f._Name.Contains(spellName));
			Assert.AreNotEqual(-1, spellIndex);
			Assert.AreEqual(40, player._Gold);
			Assert.AreEqual("You purchased Frost Nova (Rank 1) for 160 gold.",
				OutputController.Display.Output[3][2]);
		}
	}
}