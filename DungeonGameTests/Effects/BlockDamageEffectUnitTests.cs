using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class BlockDamageEffectUnitTests {
		[Test]
		public void PlayerHasBlockDamageEffectUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "block test"
			};
			IEffect blockEffect = new BlockDamageEffect(effectAmountSettings);
			player.Effects.Add(blockEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(true, player.Effects[0] is BlockDamageEffect);
			Assert.AreEqual(1, player.Effects[0].CurrentRound);
			Assert.AreEqual(3, player.Effects[0].MaxRound);
			Assert.AreEqual(false, player.Effects[0].IsHarmful);
			Assert.AreEqual(blockEffect.EffectHolder, player);
		}

		[Test]
		public void ProcessBlockDamageEffectInCombatRoundWhereDamageLessThanBlockAmountUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "block test"
			};
			BlockDamageEffect blockEffect = new BlockDamageEffect(effectAmountSettings);
			player.Effects.Add(blockEffect);
			const int incomingDamage = 30;
			string blockMessage = $"Your defensive move blocked {incomingDamage} damage!";

			blockEffect.ProcessChangeDamageRound(incomingDamage);

			Assert.AreEqual(blockMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(blockEffect.BlockAmount, 50 - incomingDamage);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(false, blockEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBlockDamageEffectInCombatRoundWhereDamageIsBlockAmountUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "block test"
			};
			BlockDamageEffect blockEffect = new BlockDamageEffect(effectAmountSettings);
			player.Effects.Add(blockEffect);
			const int incomingDamage = 50;
			string blockMessage = $"Your defensive move blocked {incomingDamage} damage!";
			const string blockExpiredMessage = "You are no longer blocking damage!";

			blockEffect.ProcessChangeDamageRound(incomingDamage);

			Assert.AreEqual(blockMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(blockExpiredMessage, OutputHelper.Display.Output[1][2]);
			Assert.AreEqual(blockEffect.BlockAmount, 0);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(true, blockEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBlockDamageEffectInCombatRoundWhereDamageIsMoreThanBlockAmountUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "block test"
			};
			BlockDamageEffect blockEffect = new BlockDamageEffect(effectAmountSettings);
			player.Effects.Add(blockEffect);
			const int incomingDamage = 80;
			string blockMessage = $"Your defensive move blocked {effectAmountSettings.Amount} damage!";
			const string blockExpiredMessage = "You are no longer blocking damage!";

			blockEffect.ProcessChangeDamageRound(incomingDamage);

			Assert.AreEqual(blockMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(blockExpiredMessage, OutputHelper.Display.Output[1][2]);
			Assert.AreEqual(blockEffect.BlockAmount, 0);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(true, blockEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBlockDamageEffectOutOfCombatRoundUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "block test"
			};
			BlockDamageEffect blockEffect = new BlockDamageEffect(effectAmountSettings);
			player.Effects.Add(blockEffect);
			const string blockFadeMessage = "Your block effect is fading away.";

			blockEffect.ProcessRound();

			Assert.AreEqual(blockFadeMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(effectAmountSettings.Amount, blockEffect.BlockAmount);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(false, blockEffect.IsEffectExpired);
		}
	}
}
