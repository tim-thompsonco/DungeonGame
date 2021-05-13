using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class BurningEffectUnitTests {
		[Test]
		public void PlayerHasBurningEffectUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			player.Effects.Add(burningEffect);

			Assert.AreEqual(1, player.Effects.Count);
			Assert.AreEqual(effectOverTimeSettings.Name, burningEffect.Name);
			Assert.AreEqual(true, player.Effects[0] is BurningEffect);
		}

		[Test]
		public void MonsterHasBurningEffectUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "burning test"
			};
			monster.Effects.Add(new BurningEffect(effectOverTimeSettings));

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is BurningEffect);
		}

		[Test]
		public void ProcessBurningEffectRoundPlayerUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			player.Effects.Add(burningEffect);
			string burningMessage = $"You burn for {effectOverTimeSettings.AmountOverTime} fire damage.";

			burningEffect.ProcessRound();

			Assert.AreEqual(player.MaxHitPoints - effectOverTimeSettings.AmountOverTime, player.HitPoints);
			Assert.AreEqual(burningMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, burningEffect.CurrentRound);
			Assert.AreEqual(false, burningEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBurningEffectRoundMonsterUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			monster.Effects.Add(burningEffect);
			string burningMessage = $"The {monster.Name} burns for {effectOverTimeSettings.AmountOverTime} fire damage.";

			burningEffect.ProcessRound();

			Assert.AreEqual(monster.MaxHitPoints - effectOverTimeSettings.AmountOverTime, monster.HitPoints);
			Assert.AreEqual(burningMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, burningEffect.CurrentRound);
			Assert.AreEqual(false, burningEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBurningEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			player.Effects.Add(burningEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound - 1; i++) {
				burningEffect.ProcessRound();
			}

			Assert.AreEqual(3, burningEffect.CurrentRound);
			Assert.AreEqual(false, burningEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBurningEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			monster.Effects.Add(burningEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound - 1; i++) {
				burningEffect.ProcessRound();
			}

			Assert.AreEqual(3, burningEffect.CurrentRound);
			Assert.AreEqual(false, burningEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBurningEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = player,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			player.Effects.Add(burningEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound; i++) {
				burningEffect.ProcessRound();
			}

			Assert.AreEqual(4, burningEffect.CurrentRound);
			Assert.AreEqual(true, burningEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBurningEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Monster monster = new Monster(5, MonsterType.Skeleton);
			EffectOverTimeSettings effectOverTimeSettings = new EffectOverTimeSettings {
				AmountOverTime = 20,
				EffectHolder = monster,
				MaxRound = 3,
				Name = "burning test"
			};
			BurningEffect burningEffect = new BurningEffect(effectOverTimeSettings);
			monster.Effects.Add(burningEffect);

			for (int i = 0; i < effectOverTimeSettings.MaxRound; i++) {
				burningEffect.ProcessRound();
			}

			Assert.AreEqual(4, burningEffect.CurrentRound);
			Assert.AreEqual(true, burningEffect.IsEffectExpired);
		}
	}
}
