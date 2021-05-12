using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class HealingEffectUnitTests {
		[Test]
		public void PlayerHasHealingEffectUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(true, player.Effects[0] is HealingEffect);
			Assert.AreEqual(1, player.Effects[0].CurrentRound);
			Assert.AreEqual(effectOverTimeSettings.MaxRound, player.Effects[0].MaxRound);
			Assert.AreEqual(false, player.Effects[0].IsHarmful);
			Assert.AreEqual(false, healEffect.IsEffectExpired);
			Assert.AreEqual(healEffect.EffectHolder, player);
		}

		[Test]
		public void PlayerIsHealedWhenEffectAmountLessThanMaxPlayerHealthUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) { MaxHitPoints = 100 };
			const int initialPlayerHealth = 50;
			player.HitPoints = initialPlayerHealth;
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);
			string healMessage = $"You have been healed for {effectOverTimeSettings.AmountOverTime} health.";

			healEffect.ProcessRound();

			Assert.AreEqual(initialPlayerHealth + effectOverTimeSettings.AmountOverTime, player.HitPoints);
			Assert.AreEqual(3, healEffect.MaxRound);
			Assert.AreEqual(healMessage, OutputHelper.Display.Output[0][2]);
		}

		[Test]
		public void PlayerIsHealedWhenEffectAmountEqualToMaxPlayerHealthUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) { MaxHitPoints = 100 };
			const int initialPlayerHealth = 75;
			player.HitPoints = initialPlayerHealth;
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);

			healEffect.ProcessRound();

			Assert.AreEqual(initialPlayerHealth + effectOverTimeSettings.AmountOverTime, player.HitPoints);
		}

		[Test]
		public void PlayerIsHealedWhenEffectAmountMoreThanMaxPlayerHealthUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) { MaxHitPoints = 100 };
			const int initialPlayerHealth = 80;
			int maxAmountPlayerCanBeHealed = player.MaxHitPoints - initialPlayerHealth;
			player.HitPoints = initialPlayerHealth;
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);
			string healMessage = $"You have been healed for {maxAmountPlayerCanBeHealed} health.";

			healEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			Assert.AreEqual(healMessage, OutputHelper.Display.Output[0][2]);
		}

		[Test]
		public void HealingEffectExpiresWhenCurrentRoundExceedsMaxRoundUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) { MaxHitPoints = 100 };
			const int initialPlayerHealth = 50;
			player.HitPoints = initialPlayerHealth;
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);

			healEffect.ProcessRound();
			healEffect.ProcessRound();
			healEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			Assert.AreEqual(4, healEffect.CurrentRound);
			Assert.AreEqual(true, healEffect.IsEffectExpired);
		}

		[Test]
		public void HealingEffectCanBeSetAsExpired() {
			Player player = new Player("test", PlayerClassType.Warrior);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 25,
				EffectHolder = player,
				MaxRound = 3,
				Name = "healing test"
			};
			IEffect healEffect = new HealingEffect(effectOverTimeSettings);
			player.Effects.Add(healEffect);

			healEffect.SetEffectAsExpired();

			Assert.AreEqual(true, healEffect.IsEffectExpired);
		}
	}
}
