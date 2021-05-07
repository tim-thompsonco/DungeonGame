using DungeonGame.Controllers;
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

			Assert.AreEqual(1, bleedEffect.TickDuration);
			Assert.AreEqual(true, bleedEffect.IsHarmful);
			Assert.AreEqual(effectName, bleedEffect.Name);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
			Assert.AreEqual(maxRound, bleedEffect.MaxRound);
			Assert.AreEqual(bleedDamageOverTime, bleedEffect.BleedDamageOverTime);
		}

		[Test]
		public void PlayerHasBleedingEffectUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));

			Assert.AreEqual(1, player._Effects.Count);
			Assert.AreEqual(true, player._Effects[0] is BleedingEffect);
		}

		[Test]
		public void MonsterHasBleedingEffectUnitTest() {
			monster.Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));

			Assert.AreEqual(1, monster.Effects.Count);
			Assert.AreEqual(true, monster.Effects[0] is BleedingEffect);
		}

		[Test]
		public void ProcessBleedingEffectRoundPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"You bleed for {bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(player);

			Assert.AreEqual(player._MaxHitPoints - bleedDamageOverTime, player._HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBleedingEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster.Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"The {monster.Name} bleeds for {bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(monster);

			Assert.AreEqual(monster.MaxHitPoints - bleedDamageOverTime, monster.HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(player);
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			monster.Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(monster);
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound; i++) {
				bleedEffect.ProcessBleedingRound(player);
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			monster.Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < maxRound; i++) {
				bleedEffect.ProcessBleedingRound(monster);
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			player._Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = player._Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(player);

			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			monster.Effects.Add(new BleedingEffect(effectName, maxRound, bleedDamageOverTime));
			BleedingEffect bleedEffect = monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(monster);

			Assert.AreEqual(monster.MaxHitPoints, monster.HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}
	}
}
