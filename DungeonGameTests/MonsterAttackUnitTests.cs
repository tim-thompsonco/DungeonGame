using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace DungeonGameTests
{
	public class MonsterAttackUnitTests
	{
		[Test]
		public void DetermineAttackUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			OutputController.Display.ClearUserOutput();
			AttackOption attackChoice = monster.DetermineAttack(player, false);
			Assert.AreEqual(AttackOption.AttackType.Spell, attackChoice._AttackCategory);
			monster._EnergyPoints = 0;
			attackChoice = monster.DetermineAttack(player, false);
			Assert.AreEqual(AttackOption.AttackType.Physical, attackChoice._AttackCategory);
		}
		[Test]
		public void FireballSpellUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 100, _MaxHitPoints = 100, _FireResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon);
			MonsterBuilder.BuildMonster(monster);
			monster._MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Fireball);
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			int spellCost = monster._Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			int spellDamage = monster._Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			int spellMaxRounds = monster._Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			string attackString = $"The {monster._Name} casts a fireball and launches it at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} hits you for {spellDamage} fire damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.OnFire);
			int burnDamage = monster._Spellbook[spellIndex]._Offensive._AmountOverTime;
			for (int i = 2; i < 5; i++)
			{
				player._Effects[0].OnFireRound(player);
				string burnString = $"You burn for {burnDamage} fire damage.";
				Assert.AreEqual(burnString, OutputController.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(player);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - spellDamage - (burnDamage * 3), player._HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _FrostResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(1, Monster.MonsterType.Skeleton);
			while (monster._SkeletonCategory != Monster.SkeletonType.Mage)
			{
				monster = new Monster(1, Monster.MonsterType.Skeleton);
			}
			MonsterBuilder.BuildMonster(monster);
			monster._MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Frostbolt);
			foreach (IItem item in player._Inventory.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			MonsterSpell.CastFrostOffense(monster, player, spellIndex);
			int spellCost = monster._Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			int spellDamage = monster._Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			int spellMaxRounds = monster._Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			string attackString = $"The {monster._Name} conjures up a frostbolt and launches it at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} hits you for {spellDamage} frost damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
			Assert.AreEqual(frozenString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.Frozen);
			// Remove all spells after casting to make monster decide to use physical attack for unit test
			monster._Spellbook = null;
			for (int i = 2; i < 4; i++)
			{
				int playerHitPointsBefore = player._HitPoints;
				double multiplier = player._Effects[0]._EffectMultiplier;
				int baseDamage = monster._MonsterWeapon._RegDamage;
				int frozenDamage = (int)(monster._MonsterWeapon._RegDamage * multiplier);
				Assert.AreEqual(frozenDamage, baseDamage * multiplier, 1);
				player._Effects[0].FrozenRound(player);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				Assert.AreEqual(frozenString, OutputController.Display.Output[i + 1][2]);
				monster._MonsterWeapon._Durability = 100;
				monster.Attack(player);
				Assert.AreEqual(playerHitPointsBefore - frozenDamage, player._HitPoints, 7);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void LightningSpellUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _ArcaneResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster._ElementalCategory != Monster.ElementalType.Air)
			{
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Lightning);
			foreach (IItem item in player._Inventory.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			MonsterSpell.CastArcaneOffense(monster, player, spellIndex);
			int spellCost = monster._Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			int spellDamage = monster._Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			string attackString = $"The {monster._Name} casts a bolt of lightning at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} hits you for {spellDamage} arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void BloodLeechAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Vampire) { _HitPoints = 10, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster._Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.BloodLeech);
			foreach (IItem item in player._Inventory.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			int monsterHealthBase = monster._HitPoints;
			MonsterAbility.UseBloodLeechAbility(monster, player, abilityIndex);
			int abilityCost = monster._Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			int leechAmount = monster._Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - leechAmount, player._HitPoints);
			Assert.AreEqual(monsterHealthBase + leechAmount, monster._HitPoints);
			string attackString = $"The {monster._Name} tries to sink its fangs into you and suck your blood!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} leeches {leechAmount} life from you.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void PoisonBiteAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 100, _MaxHitPoints = 100 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Spider) { _HitPoints = 50, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster._Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.PoisonBite);
			foreach (IItem item in player._Inventory.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			int abilityCost = monster._Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			int abilityDamage = monster._Abilities[abilityIndex]._Offensive._Amount;
			int abilityDamageOverTime = monster._Abilities[abilityIndex]._Offensive._AmountOverTime;
			int abilityCurRounds = monster._Abilities[abilityIndex]._Offensive._AmountCurRounds;
			int abilityMaxRounds = monster._Abilities[abilityIndex]._Offensive._AmountMaxRounds;
			string attackString = $"The {monster._Name} tries to bite you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} bites you for {abilityDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			string bleedString = $"You are bleeding from {monster._Name}'s attack!";
			Assert.AreEqual(bleedString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(
				true, player._Effects[0]._EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(player._MaxHitPoints - abilityDamage, player._HitPoints);
			Assert.AreEqual(abilityCurRounds, player._Effects[0]._EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, player._Effects[0]._EffectMaxRound);
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++)
			{
				player._Effects[0].BleedingRound(player);
				int bleedAmount = player._Effects[0]._EffectAmountOverTime;
				string bleedRoundString = $"You bleed for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				player._HitPoints);
		}
		[Test]
		public void TailWhipAbilityUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage) { _HitPoints = 200, _MaxHitPoints = 200 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster._Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.TailWhip);
			foreach (IItem item in player._Inventory.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			int abilityCost = monster._Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - abilityCost, monster._EnergyPoints);
			int attackDamage = monster._Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - attackDamage, player._HitPoints);
			string attackString = $"The {monster._Name} swings its tail at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} strikes you with its tail for {attackDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void FirebreathSpellUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{ _HitPoints = 200, _MaxHitPoints = 200, _FireResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			int spellIndex = monster._Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Fireball);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem._Equipped))
			{
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			monster._MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			int spellCost = monster._Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster._MaxEnergyPoints - spellCost, monster._EnergyPoints);
			int spellDamage = monster._Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player._MaxHitPoints - spellDamage, player._HitPoints);
			int spellMaxRounds = monster._Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player._Effects[0]._EffectMaxRound);
			string attackString = $"The {monster._Name} breathes a pillar of fire at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster._Name} hits you for {spellDamage} fire damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.OnFire);
			int burnDamage = monster._Spellbook[spellIndex]._Offensive._AmountOverTime;
			for (int i = 2; i < 5; i++)
			{
				player._Effects[0].OnFireRound(player);
				string burnString = $"You burn for {burnDamage} fire damage.";
				Assert.AreEqual(burnString, OutputController.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(player._MaxHitPoints - spellDamage - burnDamage * 3, player._HitPoints);
		}
	}
}