using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Players;
using NUnit.Framework;
using System;

namespace DungeonGameTests.Effects.SettingsObjects {
	internal class EffectSettingsUnitTests {
		[Test]
		public void EffectSettingsMissingEffectHolderThrowsException() {
			EffectSettings effectSettings = new EffectSettings {
				MaxRound = 3,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectSettings: EffectHolder has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectSettingsMissingMaxRoundThrowsException() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = player,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectSettings: MaxRound has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectSettingsMissingNameThrowsException() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = player,
				MaxRound = 3
			};
			const string expectedErrorMessage = "EffectSettings: Name has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectSettingsWithAllSettingsDoesNotThrowException() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = player,
				MaxRound = 3,
				Name = "test"
			};

			Assert.DoesNotThrow(() => effectSettings.ValidateSettings());
		}
	}
}
