using DungeonGame.Controllers;
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

			Assert.AreEqual(1, stunnedEffect._TickDuration);
			Assert.AreEqual(true, stunnedEffect._IsHarmful);
			Assert.AreEqual(effectName, stunnedEffect._Name);
			Assert.AreEqual(1, stunnedEffect._CurrentRound);
			Assert.AreEqual(maxRound, stunnedEffect._MaxRound);
		}

		[Test]
		public void MonsterHasStunnedEffectUnitTest() {
			monster._Effects.Add(new StunnedEffect(effectName, maxRound));

			Assert.AreEqual(1, monster._Effects.Count);
			Assert.AreEqual(true, monster._Effects[0] is StunnedEffect);
		}

		[Test]
		public void ProcessStunnedEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster._Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster._Effects.Find(effect => effect is StunnedEffect);
			string stunnedMessage = $"The {monster._Name} is stunned and cannot attack.";

			stunnedEffect.ProcessStunnedRound(monster);

			Assert.AreEqual(stunnedMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(2, stunnedEffect._CurrentRound);
			Assert.AreEqual(false, stunnedEffect._IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			monster._Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster._Effects.Find(effect => effect is StunnedEffect);

			for (int i = 0; i < maxRound - 1; i++) {
				stunnedEffect.ProcessStunnedRound(monster);
			}

			Assert.AreEqual(3, stunnedEffect._CurrentRound);
			Assert.AreEqual(false, stunnedEffect._IsEffectExpired);
		}

		[Test]
		public void MonsterStunnedEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			monster._Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster._Effects.Find(effect => effect is StunnedEffect);

			for (int i = 0; i < maxRound; i++) {
				stunnedEffect.ProcessStunnedRound(monster);
			}

			Assert.AreEqual(4, stunnedEffect._CurrentRound);
			Assert.AreEqual(true, stunnedEffect._IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster._Effects.Add(new StunnedEffect(effectName, maxRound));
			StunnedEffect stunnedEffect = (StunnedEffect)monster._Effects.Find(effect => effect is StunnedEffect);
			stunnedEffect._IsEffectExpired = true;

			stunnedEffect.ProcessStunnedRound(monster);

			Assert.AreEqual(0, OutputController.Display._Output.Count);
			Assert.AreEqual(1, stunnedEffect._CurrentRound);
		}
	}
}
