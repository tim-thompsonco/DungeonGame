using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class ReflectDamageEffectUnitTests {
		[Test]
		public void PlayerHasReflectDamageEffectUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				MaxRound = 4,
				Name = "reflect damage test"
			};
			IEffect reflectEffect = new ReflectDamageEffect(effectAmountSettings);
			player.Effects.Add(reflectEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(true, player.Effects[0] is ReflectDamageEffect);
			Assert.AreEqual(1, player.Effects[0].CurrentRound);
			Assert.AreEqual(effectAmountSettings.MaxRound, player.Effects[0].MaxRound);
			Assert.AreEqual(false, player.Effects[0].IsHarmful);
			Assert.AreEqual(reflectEffect.EffectHolder, player);
		}

		[Test]
		public void ProcessReflectDamageEffectInCombatRoundUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				MaxRound = 4,
				Name = "reflect damage test"
			};
			ReflectDamageEffect reflectEffect = new ReflectDamageEffect(effectAmountSettings);
			player.Effects.Add(reflectEffect);
			const int incomingDamage = 30;
			string reflectMessage = $"You reflected {incomingDamage} damage back at your opponent!";

			reflectEffect.ProcessChangeDamageRound(incomingDamage);

			Assert.AreEqual(reflectMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(false, reflectEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessReflectDamageEffectOutOfCombatRoundUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectAmountSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				MaxRound = 4,
				Name = "reflect damage test"
			};
			ReflectDamageEffect reflectEffect = new ReflectDamageEffect(effectAmountSettings);
			player.Effects.Add(reflectEffect);
			const string reflectMessage = "Your spell reflect is slowly fading away.";

			reflectEffect.ProcessRound();

			Assert.AreEqual(reflectMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(effectAmountSettings.Amount, reflectEffect.ReflectDamageAmount);
			Assert.AreEqual(2, player.Effects[0].CurrentRound);
			Assert.AreEqual(false, reflectEffect.IsEffectExpired);
		}
	}
}
