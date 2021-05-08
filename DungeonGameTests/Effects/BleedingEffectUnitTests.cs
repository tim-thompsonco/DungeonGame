using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class BleedingEffectUnitTests {
		[Test]
		public void PlayerHasBleedingEffectUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(true, player.Effects[0] is BleedingEffect);
		}

		[Test]
		public void MonsterHasBleedingEffectUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			monster.Effects.Add(new BleedingEffect(EffectOverTimeSettings));

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is BleedingEffect);
		}

		[Test]
		public void ProcessBleedingEffectRoundPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			player.Effects.Add(bleedEffect);
			string bleedMessage = $"You bleed for {EffectOverTimeSettings.AmountOverTime} physical damage.";

			bleedEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints - EffectOverTimeSettings.AmountOverTime, player.HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBleedingEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			monster.Effects.Add(bleedEffect);
			string bleedMessage = $"The {monster.Name} bleeds for {EffectOverTimeSettings.AmountOverTime} physical damage.";

			bleedEffect.ProcessRound();

			Assert.AreEqual(monster.MaxHitPoints - EffectOverTimeSettings.AmountOverTime, monster.HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			for (int i = 0; i < EffectOverTimeSettings.MaxRound - 1; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			monster.Effects.Add(bleedEffect);

			for (int i = 0; i < EffectOverTimeSettings.MaxRound - 1; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			player.Effects.Add(bleedEffect);

			for (int i = 0; i < EffectOverTimeSettings.MaxRound; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			monster.Effects.Add(bleedEffect);

			for (int i = 0; i < EffectOverTimeSettings.MaxRound; i++) {
				bleedEffect.ProcessRound();
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			player.Effects.Add(bleedEffect);
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectOverTimeSettings EffectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "bleed test"
			};
			BleedingEffect bleedEffect = new BleedingEffect(EffectOverTimeSettings);
			monster.Effects.Add(bleedEffect);
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessRound();

			Assert.AreEqual(monster.MaxHitPoints, monster.HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}
	}
}
