using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class TrainerUnitTests
	{
		[Test]
		public void UpgradeSpellTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			player.PlayerClass = Player.PlayerClassType.Archer;
			const string spellName = "fireball";
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual("You can't upgrade spells. You're not a mage!",
					OutputHandler.Display.Output[0][2]);
			player.PlayerClass = Player.PlayerClassType.Mage;
			var spellIndex = player.Spellbook.FindIndex(
				f => f.Name == spellName);
			Assert.AreEqual(25, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(5, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			player.Gold = 0;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual(25, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(5, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			Assert.AreEqual("You are not ready to upgrade that spell. You need to level up first!",
				OutputHandler.Display.Output[1][2]);
			trainer.UpgradeSpell(player, "not a spell");
			Assert.AreEqual("You don't have that spell to train!", OutputHandler.Display.Output[2][2]);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual("You can't afford that!", OutputHandler.Display.Output[3][2]);
			player.Gold = 100;
			trainer.UpgradeSpell(player, spellName);
			Assert.AreEqual(2, player.Spellbook[spellIndex].Rank);
			Assert.AreEqual(35, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(10, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			Assert.AreEqual(60, player.Gold);
			Assert.AreEqual("You upgraded Fireball to Rank 2 for 40 gold.",
				OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void UpgradeAbilityTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Archer);
			player.PlayerClass = Player.PlayerClassType.Mage;
			const string abilityName = "distance";
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual("You can't upgrade abilities. You're not a warrior or archer!",
				OutputHandler.Display.Output[0][2]);
			player.PlayerClass = Player.PlayerClassType.Archer;
			var abilityIndex = player.Abilities.FindIndex(
				f => f.Name == abilityName || f.Name.Contains(abilityName));
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			player.Gold = 0;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			Assert.AreEqual("You are not ready to upgrade that ability. You need to level up first!",
				OutputHandler.Display.Output[1][2]);
			trainer.UpgradeAbility(player, "not an ability");
			Assert.AreEqual("You don't have that ability to train!", OutputHandler.Display.Output[2][2]);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual("You can't afford that!", OutputHandler.Display.Output[3][2]);
			player.Gold = 100;
			trainer.UpgradeAbility(player, abilityName);
			Assert.AreEqual(2, player.Abilities[abilityIndex].Rank);
			Assert.AreEqual(35, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(55, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			Assert.AreEqual(60, player.Gold);
			Assert.AreEqual("You upgraded Distance Shot to Rank 2 for 40 gold.",
				OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void TrainAbilityTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Warrior);
			player.PlayerClass = Player.PlayerClassType.Mage;
			const string abilityName = "bandage";
			trainer.TrainAbility(player, abilityName);
			Assert.AreEqual("You can't train abilities. You're not a warrior or archer!",
				OutputHandler.Display.Output[0][2]);
			player.PlayerClass = Player.PlayerClassType.Warrior;
			var abilityIndex = player.Abilities.FindIndex(
				f => f.Name == abilityName || f.Name.Contains(abilityName));
			Assert.AreEqual(-1, abilityIndex);
			player.Gold = 0;
			trainer.TrainAbility(player, abilityName);
			abilityIndex = player.Abilities.FindIndex(
				f => f.Name == abilityName || f.Name.Contains(abilityName));
			Assert.AreEqual(-1, abilityIndex);
			Assert.AreEqual("You are not ready to train that ability. You need to level up first!",
				OutputHandler.Display.Output[1][2]);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.TrainAbility(player, abilityName);
			Assert.AreEqual("You can't afford that!", OutputHandler.Display.Output[2][2]);
			player.Gold = 100;
			trainer.TrainAbility(player, abilityName);
			abilityIndex = player.Abilities.FindIndex(
				f => f.Name == abilityName || f.Name.Contains(abilityName));
			Assert.AreNotEqual(-1, abilityIndex);
			Assert.AreEqual(60, player.Gold);
			Assert.AreEqual("You purchased Bandage (Rank 1) for 40 gold.",
				OutputHandler.Display.Output[3][2]);
		}
		[Test]
		public void TrainSpellTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			const string spellName = "frost nova";
			trainer.TrainSpell(player, spellName);
			Assert.AreEqual("You can't train spells. You're not a mage!", OutputHandler.Display.Output[0][2]);
			player = new Player("placeholder", Player.PlayerClassType.Mage);
			var spellIndex = player.Spellbook.FindIndex(
				f => f.Name == spellName || f.Name.Contains(spellName));
			Assert.AreEqual(-1, spellIndex);
			player.Gold = 0;
			trainer.TrainSpell(player, spellName);
			spellIndex = player.Spellbook.FindIndex(
				f => f.Name == spellName || f.Name.Contains(spellName));
			Assert.AreEqual(-1, spellIndex);
			Assert.AreEqual("You are not ready to train that spell. You need to level up first!",
				OutputHandler.Display.Output[1][2]);
			player.Intelligence = 20;
			player.Level = 8;
			trainer.TrainSpell(player, spellName);
			Assert.AreEqual("You can't afford that!", OutputHandler.Display.Output[2][2]);
			player.Gold = 200;
			trainer.TrainSpell(player, spellName);
			spellIndex = player.Spellbook.FindIndex(
				f => f.Name == spellName || f.Name.Contains(spellName));
			Assert.AreNotEqual(-1, spellIndex);
			Assert.AreEqual(40, player.Gold);
			Assert.AreEqual("You purchased Frost Nova (Rank 1) for 160 gold.",
				OutputHandler.Display.Output[3][2]);
		}
	}
}