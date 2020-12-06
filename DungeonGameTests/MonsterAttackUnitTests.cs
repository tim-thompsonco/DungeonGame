using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class MonsterAttackUnitTests
	{
		[Test]
		public void DetermineAttackUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			OutputHandler.Display.ClearUserOutput();
			var attackChoice = monster.DetermineAttack(player);
			Assert.AreEqual(AttackOption.AttackType.Spell, attackChoice._AttackCategory);
			monster._EnergyPoints = 0;
			attackChoice = monster.DetermineAttack(player);
			Assert.AreEqual(AttackOption.AttackType.Physical, attackChoice._AttackCategory);
		}
		[Test]
		public void FireballSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 100, _MaxHitPoints = 100, _FireResistance = 0 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon);
			MonsterBuilder.BuildMonster(monster);
			monster._MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var spellIndex = monster._Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Fireball);
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			var spellCost = monster._Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			var spellDamage = monster._Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			var spellMaxRounds = monster._Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			var attackString = "The " + monster._Name + " casts a fireball and launches it at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " hits you for " + spellDamage + " fire damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.OnFire);
			var burnDamage = monster._Spellbook[spellIndex].Offensive.AmountOverTime;
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].OnFireRound(player);
				var burnString = "You burn for " + burnDamage + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(player);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - spellDamage - burnDamage * 3, player._HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _FrostResistance = 0 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(1, Monster.MonsterType.Skeleton);
			while (monster._SkeletonCategory != Monster.SkeletonType.Mage)
			{
				monster = new Monster(1, Monster.MonsterType.Skeleton);
			}
			MonsterBuilder.BuildMonster(monster);
			monster._MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var spellIndex = monster._Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Frostbolt);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			MonsterSpell.CastFrostOffense(monster, player, spellIndex);
			var spellCost = monster._Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			var spellDamage = monster._Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			var spellMaxRounds = monster._Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			var attackString = "The " + monster._Name + " conjures up a frostbolt and launches it at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " hits you for " + spellDamage + " frost damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.Frozen);
			// Remove all spells after casting to make monster decide to use physical attack for unit test
			monster._Spellbook = null;
			for (var i = 2; i < 4; i++)
			{
				var playerHitPointsBefore = player._HitPoints;
				var multiplier = player._Effects[0]._EffectMultiplier;
				var baseDamage = monster._MonsterWeapon.RegDamage;
				var frozenDamage = (int)(monster._MonsterWeapon.RegDamage * multiplier);
				Assert.AreEqual(frozenDamage, baseDamage * multiplier, 1);
				player._Effects[0].FrozenRound(player);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				Assert.AreEqual(frozenString, OutputHandler.Display.Output[i + 1][2]);
				monster._MonsterWeapon.Durability = 100;
				monster.Attack(player);
				Assert.AreEqual(playerHitPointsBefore - frozenDamage, player._HitPoints, 7);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void LightningSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _ArcaneResistance = 0 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster._ElementalCategory != Monster.ElementalType.Air)
			{
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			var spellIndex = monster._Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Lightning);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			MonsterSpell.CastArcaneOffense(monster, player, spellIndex);
			var spellCost = monster._Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			var spellDamage = monster._Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			var attackString = "The " + monster._Name + " casts a bolt of lightning at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " hits you for " + spellDamage + " arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void BloodLeechAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Vampire) { _HitPoints = 10, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster._Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.BloodLeech);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var monsterHealthBase = monster._HitPoints;
			MonsterAbility.UseBloodLeechAbility(monster, player, abilityIndex);
			var abilityCost = monster._Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			var leechAmount = monster._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - leechAmount, player._HitPoints);
			Assert.AreEqual(monsterHealthBase + leechAmount, monster._HitPoints);
			var attackString = "The " + monster._Name + " tries to sink its fangs into you and suck your blood!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " leeches " + leechAmount + " life from you.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void PoisonBiteAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Spider) { _HitPoints = 50, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster._Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.PoisonBite);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			var abilityCost = monster._Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			var abilityDamage = monster._Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = monster._Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = monster._Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = monster._Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var attackString = "The " + monster._Name + " tries to bite you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " bites you for " + abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			var bleedString = "You are bleeding from " + monster._Name + "'s attack!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(
				true, player._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(player._MaxHitPoints - abilityDamage, player._HitPoints);
			Assert.AreEqual(abilityCurRounds, player._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, player._Effects[0]._EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].BleedingRound(player);
				var bleedAmount = player._Effects[0]._EffectAmountOverTime;
				var bleedRoundString = "You bleed for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				player._HitPoints);
		}
		[Test]
		public void TailWhipAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 200, _MaxHitPoints = 200 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster._Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.TailWhip);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			var abilityCost = monster._Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			var attackDamage = monster._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - attackDamage, player._HitPoints);
			var attackString = "The " + monster._Name + " swings its tail at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " strikes you with its tail for " +
									  attackDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void FirebreathSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _FireResistance = 0 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			var spellIndex = monster._Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Fireball);
			foreach (var item in player._Inventory.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			monster._MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			var spellCost = monster._Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			var spellDamage = monster._Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			var spellMaxRounds = monster._Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			var attackString = "The " + monster._Name + " breathes a pillar of fire at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster._Name + " hits you for " + spellDamage + " fire damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.OnFire);
			var burnDamage = monster._Spellbook[spellIndex].Offensive.AmountOverTime;
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].OnFireRound(player);
				var burnString = "You burn for " + burnDamage + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - spellDamage - burnDamage * 3, player._HitPoints);
		}
	}
}