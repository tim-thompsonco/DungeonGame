using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Players;
using NUnit.Framework;
using System;

namespace DungeonGameTests.Effects.SettingsObjects {
	internal class EffectOverTimeSettingsUnitTests {
		[Test]
		public void EffectOverTimeSettingsMissingEffectHolderThrowsException() {
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 5,
				MaxRound = 3,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectOverTimeSettings: EffectHolder has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectOverTimeSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectOverTimeSettingsMissingMaxRoundThrowsException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 5,
				EffectHolder = player,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectOverTimeSettings: MaxRound has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectOverTimeSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectOverTimeSettingsMissingNameThrowsException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 5,
				EffectHolder = player,
				MaxRound = 3
			};
			const string expectedErrorMessage = "EffectOverTimeSettings: Name has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectOverTimeSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectOverTimeSettingsMissingAmountOverTimeThrowsException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				EffectHolder = player,
				MaxRound = 3,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectOverTimeSettings: AmountOverTime has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectOverTimeSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectOverTimeSettingsWithAllSettingsDoesNotThrowException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 5,
				EffectHolder = player,
				MaxRound = 3,
				Name = "test"
			};

			Assert.DoesNotThrow(() => effectOverTimeSettings.ValidateSettings());
		}
	}
}
