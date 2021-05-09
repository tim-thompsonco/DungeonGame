using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Monsters;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class StunnedEffectUnitTests {
		[Test]
		public void CreateStunnedEffectUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings);

			Assert.AreEqual(1, stunnedEffect.TickDuration);
			Assert.AreEqual(true, stunnedEffect.IsHarmful);
			Assert.AreEqual(effectSettings.Name, stunnedEffect.Name);
			Assert.AreEqual(1, stunnedEffect.CurrentRound);
			Assert.AreEqual(effectSettings.MaxRound, stunnedEffect.MaxRound);
		}

		[Test]
		public void MonsterHasStunnedEffectUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings);
			monster.Effects.Add(stunnedEffect);

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is StunnedEffect);
		}

		[Test]
		public void ProcessStunnedEffectRoundMonsterUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings);
			monster.Effects.Add(stunnedEffect);
			string stunnedMessage = $"The {monster.Name} is stunned and cannot attack.";

			stunnedEffect.ProcessRound();

			Assert.AreEqual(stunnedMessage, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(2, stunnedEffect.CurrentRound);
			Assert.AreEqual(false, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings);

			for (int i = 0; i < effectSettings.MaxRound - 1; i++) {
				stunnedEffect.ProcessRound();
			}

			Assert.AreEqual(3, stunnedEffect.CurrentRound);
			Assert.AreEqual(false, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings);

			for (int i = 0; i < stunnedEffect.MaxRound; i++) {
				stunnedEffect.ProcessRound();
			}

			Assert.AreEqual(4, stunnedEffect.CurrentRound);
			Assert.AreEqual(true, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(5, Monster.MonsterType.Skeleton);
			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				Name = "stunned test",
				MaxRound = 3
			};
			IEffect stunnedEffect = new StunnedEffect(effectSettings) { IsEffectExpired = true };

			stunnedEffect.ProcessRound();

			Assert.AreEqual(0, OutputHelper.Display.Output.Count);
			Assert.AreEqual(1, stunnedEffect.CurrentRound);
		}
	}
}
