﻿using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	class StunnedEffectUnitTests {
		Monster monster;
		string effectName;
		int maxRound;

		[SetUp]
		public void Setup() {
			monster = new Monster(5, Monster.MonsterType.Skeleton);
			effectName = "stunned test";
			maxRound = 3;
		}

		[Test]
		public void CreateStunnedEffectUnitTest() {
			StunnedEffect stunnedEffect = new StunnedEffect(effectName, maxRound);

			Assert.AreEqual(1, stunnedEffect.TickDuration);
			Assert.AreEqual(true, stunnedEffect.IsHarmful);
			Assert.AreEqual(effectName, stunnedEffect.Name);
			Assert.AreEqual(1, stunnedEffect.CurrentRound);
			Assert.AreEqual(maxRound, stunnedEffect.MaxRound);
		}

		[Test]
		public void MonsterHasStunnedEffectUnitTest() {
			monster.Effects.Add(new StunnedEffect(effectName, maxRound));

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is StunnedEffect);
		}

		[Test]
		public void ProcessStunnedEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster.Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster.Effects.Find(effect => effect is StunnedEffect);
			string stunnedMessage = $"The {monster.Name} is stunned and cannot attack.";

			stunnedEffect.ProcessStunnedRound(monster);

			Assert.AreEqual(stunnedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, stunnedEffect.CurrentRound);
			Assert.AreEqual(false, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			monster.Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster.Effects.Find(effect => effect is StunnedEffect);

			for (int i = 0; i < maxRound - 1; i++) {
				stunnedEffect.ProcessStunnedRound(monster);
			}

			Assert.AreEqual(3, stunnedEffect.CurrentRound);
			Assert.AreEqual(false, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			monster.Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster.Effects.Find(effect => effect is StunnedEffect);

			for (int i = 0; i < maxRound; i++) {
				stunnedEffect.ProcessStunnedRound(monster);
			}

			Assert.AreEqual(4, stunnedEffect.CurrentRound);
			Assert.AreEqual(true, stunnedEffect.IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster.Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster.Effects.Find(effect => effect is StunnedEffect);
			stunnedEffect.IsEffectExpired = true;

			stunnedEffect.ProcessStunnedRound(monster);

			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, stunnedEffect.CurrentRound);
		}
	}
}
