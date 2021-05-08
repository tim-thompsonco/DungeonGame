using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;

namespace DungeonGameTests.Effects {
	internal class BleedingEffectUnitTests {
		private Player _player;
		private Monster _monster;
		private string _effectName;
		private int _maxRound;
		private int _bleedDamageOverTime;

		[SetUp]
		public void Setup() {
			_player = new Player("test", Player.PlayerClassType.Mage);
			_monster = new Monster(5, Monster.MonsterType.Skeleton);
			_effectName = "bleed test";
			_maxRound = 3;
			_bleedDamageOverTime = 20;
		}

		[Test]
		public void CreateBleedingEffectUnitTest() {
			BleedingEffect bleedEffect = new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime);

			Assert.AreEqual(1, bleedEffect.TickDuration);
			Assert.AreEqual(true, bleedEffect.IsHarmful);
			Assert.AreEqual(_effectName, bleedEffect.Name);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
			Assert.AreEqual(_maxRound, bleedEffect.MaxRound);
			Assert.AreEqual(_bleedDamageOverTime, bleedEffect.BleedDamageOverTime);
		}

		[Test]
		public void PlayerHasBleedingEffectUnitTest() {
			_player.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));

			Assert.AreEqual(1, _player.Effects.Count);
			Assert.AreEqual(true, _player.Effects[0] is BleedingEffect);
		}

		[Test]
		public void MonsterHasBleedingEffectUnitTest() {
			_monster.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));

			Assert.AreEqual(1, _monster.Effects.Count);
			Assert.AreEqual(true, _monster.Effects[0] is BleedingEffect);
		}

		[Test]
		public void ProcessBleedingEffectRoundPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			_player.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _player.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"You bleed for {_bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(_player);

			Assert.AreEqual(_player.MaxHitPoints - _bleedDamageOverTime, _player.HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ProcessBleedingEffectRoundMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			_monster.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			string bleedMessage = $"The {_monster.Name} bleeds for {_bleedDamageOverTime} physical damage.";

			bleedEffect.ProcessBleedingRound(_monster);

			Assert.AreEqual(_monster.MaxHitPoints - _bleedDamageOverTime, _monster.HitPoints);
			Assert.AreEqual(bleedMessage, OutputController.Display.Output[0][2]);
			Assert.AreEqual(2, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			_player.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _player.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < _maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(_player);
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectDoesNotExpireWhenCurrentRoundEqualsMaxRoundUnitTest() {
			_monster.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < _maxRound - 1; i++) {
				bleedEffect.ProcessBleedingRound(_monster);
			}

			Assert.AreEqual(3, bleedEffect.CurrentRound);
			Assert.AreEqual(false, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void PlayerBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			_player.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _player.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < _maxRound; i++) {
				bleedEffect.ProcessBleedingRound(_player);
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void MonsterBleedingEffectExpiresWhenCurrentRoundGreaterThanMaxRoundUnitTest() {
			_monster.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;

			for (int i = 0; i < _maxRound; i++) {
				bleedEffect.ProcessBleedingRound(_monster);
			}

			Assert.AreEqual(4, bleedEffect.CurrentRound);
			Assert.AreEqual(true, bleedEffect.IsEffectExpired);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectPlayerUnitTest() {
			OutputController.Display.ClearUserOutput();
			_player.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _player.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(_player);

			Assert.AreEqual(_player.MaxHitPoints, _player.HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}

		[Test]
		public void ExpiredBleedingEffectDoesNotAffectMonsterUnitTest() {
			OutputController.Display.ClearUserOutput();
			_monster.Effects.Add(new BleedingEffect(_effectName, _maxRound, _bleedDamageOverTime));
			BleedingEffect bleedEffect = _monster.Effects.Find(effect => effect is BleedingEffect) as BleedingEffect;
			bleedEffect.IsEffectExpired = true;

			bleedEffect.ProcessBleedingRound(_monster);

			Assert.AreEqual(_monster.MaxHitPoints, _monster.HitPoints);
			Assert.AreEqual(0, OutputController.Display.Output.Count);
			Assert.AreEqual(1, bleedEffect.CurrentRound);
		}
	}
}
