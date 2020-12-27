﻿using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	class BleedingEffectUnitTests {
		Player player;
		Monster monster;
		string effectName;
		int maxRound;
		int bleedDamageOverTime;

		[SetUp]
		public void Setup() {
			player = new Player("test", Player.PlayerClassType.Mage);
			monster = new Monster(5, Monster.MonsterType.Skeleton);
			effectName = "bleed test";
			maxRound = 3;
			bleedDamageOverTime = 20;
		}

		[Test]
		public void CreateBleedingEffectUnitTest() {
			BleedingEffect bleedEffect = new BleedingEffect(effectName, maxRound, bleedDamageOverTime);

			Assert.AreEqual(1, bleedEffect._TickDuration);
			Assert.AreEqual(true, bleedEffect._IsHarmful);
			Assert.AreEqual(effectName, bleedEffect._Name);
			Assert.AreEqual(1, bleedEffect._CurrentRound);
			Assert.AreEqual(maxRound, bleedEffect._MaxRound);
			Assert.AreEqual(bleedDamageOverTime, bleedEffect._BleedDamageOverTime);
		}

		[Test]
		public void PlayerHasBleedingEffectUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));

			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(true, player._Effects[0] is BleedingEffect);
		}

		[Test]
		public void MonsterHasBleedingEffectUnitTest() {
			monster._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));

			Assert.AreEqual(1, monster._Effects.Count);
			Assert.AreEqual(true, monster._Effects[0] is BleedingEffect);
		}

		[Test]
		public void ProcessBleedingEffectRoundPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"You bleed for {bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(player);

			Assert.AreEqual(player._MaxHitPoints - bleedDamageOverTime, player._HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(2, bleedEffect._CurrentRound);
			Assert.AreEqual(false, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void ProcessBleedingEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"The {monster._Name} bleeds for {bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(monster);

			Assert.AreEqual(monster._MaxHitPoints - bleedDamageOverTime, monster._HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display._Output[0][2]);
			Assert.AreEqual(2, bleedEffect._CurrentRound);
			Assert.AreEqual(false, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(player);
			}

			Assert.AreEqual(3, bleedEffect._CurrentRound);
			Assert.AreEqual(false, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			monster._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(monster);
			}

			Assert.AreEqual(3, bleedEffect._CurrentRound);
			Assert.AreEqual(false, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound; i++) {
				bleedEffect.ProcessBleedingRound(player);
			}

			Assert.AreEqual(4, bleedEffect._CurrentRound);
			Assert.AreEqual(true, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			monster._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound; i++) {
				bleedEffect.ProcessBleedingRound(monster);
			}

			Assert.AreEqual(4, bleedEffect._CurrentRound);
			Assert.AreEqual(true, bleedEffect._IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect._IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(player);

			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
			Assert.AreEqual(0, OutputController.Display._Output.Count);
			Assert.AreEqual(1, bleedEffect._CurrentRound);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect._IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(monster);

			Assert.AreEqual(monster._MaxHitPoints, monster._HitPoints);
			Assert.AreEqual(0, OutputController.Display._Output.Count);
			Assert.AreEqual(1, bleedEffect._CurrentRound);
		}
	}
}
