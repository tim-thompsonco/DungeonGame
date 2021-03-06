﻿using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class BleedingEffectUnitTests {
		[Test]
		public void PlayerHasBleedingEffectUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(effectOverTimeSettings.Name, bleedEffect.Name);
			Assert.AreEqual(true, player.Effects[0] is BleedingEffect);
		}

		[Test]
		public void MonsterHasBleedingEffectUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			monster.Effects.Add(new BleedingEffect(effectOverTimeSettings));

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is BleedingEffect);
		}

		[Test]
		public void ProcessBleedingEffectRoundPlayerUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			player.Effects.Add(bleedEffect);
			string bleedMessage = $"You bleed for {effectOverTimeSettings.AmountOverTime} physical damage.";

			bleedEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints - effectOverTimeSettings.AmountOverTime, player.HitPoints);
			Assert.AreEqual(bleedMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBleedingEffectRoundMonsterUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			monster.Effects.Add(bleedEffect);
			string bleedMessage = $"The {monster.Name} bleeds for {effectOverTimeSettings.AmountOverTime} physical damage.";

			bleedEffect.ProcessRound();

			Assert.AreEqual(monster.MaxHitPoints - effectOverTimeSettings.AmountOverTime, monster.HitPoints);
			Assert.AreEqual(bleedMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound - 1; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			monster.Effects.Add(bleedEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound - 1; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(effectOverTimeSettings);
			monster.Effects.Add(bleedEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}
	}
}
