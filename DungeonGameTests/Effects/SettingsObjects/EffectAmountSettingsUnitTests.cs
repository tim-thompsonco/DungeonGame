using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Players;
using NUnit.Framework;
using System;

namespace DungeonGameTests.Effects.SettingsObjects {
	internal class EffectAmountSettingsUnitTests {
		[Test]
		public void EffectAmountSettingsMissingAmountThrowsException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectSettings = new EffectAmountSettings {
				EffectHolder = player,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectAmountSettings: Amount has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectAmountSettingsMissingEffectHolderThrowsException() {
			EffectAmountSettings effectSettings = new EffectAmountSettings {
				Amount = 50,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectAmountSettings: EffectHolder has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectAmountSettingsMissingNameThrowsException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
			};
			const string expectedErrorMessage = "EffectAmountSettings: Name has not been set.";

			Exception exception = Assert.Throws<Exception>(() => effectSettings.ValidateSettings());

			Assert.AreEqual(expectedErrorMessage, exception.Message);
		}

		[Test]
		public void EffectAmountSettingsWithAllSettingsDoesNotThrowException() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectAmountSettings effectSettings = new EffectAmountSettings {
				Amount = 50,
				EffectHolder = player,
				Name = "test"
			};

			Assert.DoesNotThrow(() => effectSettings.ValidateSettings());
		}
	}
}
