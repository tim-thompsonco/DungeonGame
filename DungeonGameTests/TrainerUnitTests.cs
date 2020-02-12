using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class TrainerUnitTests {
		[Test]
		public void UpgradeSpellTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Mage);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Mage);
			player.PlayerClass = Player.PlayerClassType.Archer;
			trainer.UpgradeSpell(player, "fireball");
			var expectedOutput =OutputHandler.Display.Output[0][2]; 
			Assert.AreEqual("You can't upgrade spells. You're not a mage!",expectedOutput);
			player.PlayerClass = Player.PlayerClassType.Mage;
			var spellIndex = player.Spellbook.FindIndex(
				f => f.Name == "fireball");
			Assert.AreEqual(25, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(5, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			player.Gold = 0;
			trainer.UpgradeSpell(player, "fireball");
			Assert.AreEqual(25, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(5, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			var expectedOutputTwo = OutputHandler.Display.Output[1][2];
			Assert.AreEqual("You are not ready to upgrade that spell. You need to level up first!", 
				expectedOutputTwo);
			trainer.UpgradeSpell(player, "not a spell");
			var expectedOutputThree = OutputHandler.Display.Output[2][2];
			Assert.AreEqual("You don't have that spell to train!", expectedOutputThree);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeSpell(player, "fireball");
			var expectedOutputFour = OutputHandler.Display.Output[3][2];
			Assert.AreEqual("You can't afford that!",expectedOutputFour);
			player.Gold = 100;
			trainer.UpgradeSpell(player, "fireball");
			Assert.AreEqual(2, player.Spellbook[spellIndex].Rank);
			Assert.AreEqual(35, player.Spellbook[spellIndex].Offensive.Amount);
			Assert.AreEqual(10, player.Spellbook[spellIndex].Offensive.AmountOverTime);
			Assert.AreEqual(60, player.Gold);
			var expectedOutputFive = OutputHandler.Display.Output[4][2];
			Assert.AreEqual("You upgraded Fireball to rank 2 for 40 gold.", expectedOutputFive);
		}
		[Test]
		public void UpgradeAbilityTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Archer);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Archer);
			player.PlayerClass = Player.PlayerClassType.Mage;
			trainer.UpgradeAbility(player, "distance");
			var expectedOutput =OutputHandler.Display.Output[0][2]; 
			Assert.AreEqual("You can't upgrade abilities. You're not a warrior or archer!",expectedOutput);
			player.PlayerClass = Player.PlayerClassType.Archer;
			var abilityIndex = player.Abilities.FindIndex(
				f => f.Name == "distance" || f.Name.Contains("distance"));
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			player.Gold = 0;
			trainer.UpgradeAbility(player, "distance");
			Assert.AreEqual(25, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(50, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			var expectedOutputTwo = OutputHandler.Display.Output[1][2];
			Assert.AreEqual("You are not ready to upgrade that ability. You need to level up first!", 
				expectedOutputTwo);
			trainer.UpgradeAbility(player, "not an ability");
			var expectedOutputThree = OutputHandler.Display.Output[2][2];
			Assert.AreEqual("You don't have that ability to train!", expectedOutputThree);
			player.Intelligence = 20;
			player.Level = 2;
			trainer.UpgradeAbility(player, "distance");
			var expectedOutputFour = OutputHandler.Display.Output[3][2];
			Assert.AreEqual("You can't afford that!",expectedOutputFour);
			player.Gold = 100;
			trainer.UpgradeAbility(player, "distance");
			Assert.AreEqual(2, player.Abilities[abilityIndex].Rank);
			Assert.AreEqual(35, player.Abilities[abilityIndex].Offensive.Amount);
			Assert.AreEqual(55, player.Abilities[abilityIndex].Offensive.ChanceToSucceed);
			Assert.AreEqual(60, player.Gold);
			var expectedOutputFive = OutputHandler.Display.Output[4][2];
			Assert.AreEqual("You upgraded Distance Shot to rank 2 for 40 gold.", expectedOutputFive);
		}
		[Test]
		public void TrainAbilityTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("placeholder", Player.PlayerClassType.Warrior);
			var trainer = new Trainer("some name", "some desc", Trainer.TrainerCategory.Warrior);
			player.PlayerClass = Player.PlayerClassType.Mage;
			trainer.TrainAbility(player, "bandage");
			var expectedOutput = OutputHandler.Display.Output[0][2]; 
			Assert.AreEqual("You can't train abilities. You're not a warrior or archer!",expectedOutput);
			player.PlayerClass = Player.PlayerClassType.Warrior;
			var abilityIndex = player.Abilities.FindIndex(
				f => f.Name == "bandage" || f.Name.Contains("bandage"));
			Assert.AreEqual(-1, abilityIndex);
			player.Gold = 0;
			OutputHandler.Display.ClearUserOutput();
			trainer.TrainAbility(player, "bandage");
			abilityIndex = player.Abilities.FindIndex(
				f => f.Name == "bandage" || f.Name.Contains("bandage"));
			Assert.AreEqual(-1, abilityIndex);
			expectedOutput = OutputHandler.Display.Output[0][2]; 
			Assert.AreEqual("You are not ready to train that ability. You need to level up first!", 
				expectedOutput);
			player.Intelligence = 20;
			player.Level = 2;
			OutputHandler.Display.ClearUserOutput();
			trainer.TrainAbility(player, "bandage");
			expectedOutput = OutputHandler.Display.Output[0][2]; 
			Assert.AreEqual("You can't afford that!",expectedOutput);
			player.Gold = 100;
			OutputHandler.Display.ClearUserOutput();
			trainer.TrainAbility(player, "bandage");
			expectedOutput = OutputHandler.Display.Output[0][2]; 
			abilityIndex = player.Abilities.FindIndex(
				f => f.Name == "bandage" || f.Name.Contains("bandage"));
			Assert.AreNotEqual(-1, abilityIndex);
			Assert.AreEqual(60, player.Gold);
			Assert.AreEqual("You purchased Bandage (Rank 1) for 40 gold.", expectedOutput);
		}
	}
}