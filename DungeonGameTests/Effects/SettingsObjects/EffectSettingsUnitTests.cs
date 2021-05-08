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
			string actualErrorMessage = string.Empty;

			try {
				effectSettings.ValidateSettings();
			} catch (Exception ex) {
				actualErrorMessage = ex.Message;
			}

			Assert.AreEqual(expectedErrorMessage, actualErrorMessage);
		}

		[Test]
		public void EffectSettingsMissingMaxRoundThrowsException() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = player,
				Name = "test"
			};
			const string expectedErrorMessage = "EffectSettings: MaxRound has not been set.";
			string actualErrorMessage = string.Empty;

			try {
				effectSettings.ValidateSettings();
			} catch (Exception ex) {
				actualErrorMessage = ex.Message;
			}

			Assert.AreEqual(expectedErrorMessage, actualErrorMessage);
		}

		[Test]
		public void EffectSettingsMissingNameThrowsException() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = player,
				MaxRound = 3
			};
			const string expectedErrorMessage = "EffectSettings: Name has not been set.";
			string actualErrorMessage = string.Empty;

			try {
				effectSettings.ValidateSettings();
			} catch (Exception ex) {
				actualErrorMessage = ex.Message;
			}

			Assert.AreEqual(expectedErrorMessage, actualErrorMessage);
		}
	}
}
