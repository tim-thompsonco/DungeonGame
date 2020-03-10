using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class MonsterAttackUnitTests {
		
		[Test]
		public void DetermineAttackUnitTest() {
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			OutputHandler.Display.ClearUserOutput();
			var attackChoice = monster.DetermineAttack();
			Assert.AreEqual(AttackOption.AttackType.Spell, attackChoice.AttackCategory);
			monster.EnergyPoints = 0;
			attackChoice = monster.DetermineAttack();
			Assert.AreEqual(AttackOption.AttackType.Physical, attackChoice.AttackCategory);
		}
		[Test]
		public void FireballSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 100, MaxHitPoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon);
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Fireball);
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			var spellCost = monster.Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			var spellDamage = monster.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			var spellMaxRounds = monster.Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].EffectMaxRound);
			var attackString = "The " + monster.Name + " casts a fireball and launches it at you!";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " hits you for " + spellDamage + " fire damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.OnFire);
			var burnDamage = monster.Spellbook[spellIndex].Offensive.AmountOverTime;
			for (var i = 2; i < 5; i++) {
				player.Effects[0].OnFireRound(player);
				var burnString = "You burn for " + burnDamage + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(player);
			}
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - spellDamage - burnDamage * 3, player.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 200, MaxHitPoints = 200};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(1, Monster.MonsterType.Skeleton);
			while (monster.SkeletonCategory != Monster.SkeletonType.Mage) {
				monster = new Monster(1, Monster.MonsterType.Skeleton);
			}
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Frostbolt);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			MonsterSpell.CastFrostOffense(monster, player, spellIndex);
			var spellCost = monster.Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			var spellDamage = monster.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			var spellMaxRounds = monster.Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].EffectMaxRound);
			var attackString = "The " + monster.Name + " conjures up a frostbolt and launches it at you!";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " hits you for " + spellDamage + " frost damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.Frozen);
			// Remove all spells after casting to make monster decide to use physical attack for unit test
			monster.Spellbook = null;
			for (var i = 2; i < 4; i++) {
				var playerHitPointsBefore = player.HitPoints;
				var multiplier = player.Effects[0].EffectMultiplier;
				var baseDamage = monster.MonsterWeapon.RegDamage;
				var frozenDamage = (int)(monster.MonsterWeapon.RegDamage * multiplier);
				Assert.AreEqual(frozenDamage, baseDamage * multiplier, 1);
				player.Effects[0].FrozenRound(player);
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				Assert.AreEqual(frozenString, OutputHandler.Display.Output[i + 1][2]);
				monster.MonsterWeapon.Durability = 100;
				monster.Attack(player);
				Assert.AreEqual(playerHitPointsBefore - frozenDamage, player.HitPoints, 7);
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void LightningSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 200, MaxHitPoints = 200};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster.ElementalCategory != Monster.ElementalType.Air) {
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			var spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Lightning);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			MonsterSpell.CastArcaneOffense(monster, player, spellIndex);
			var spellCost = monster.Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			var spellDamage = monster.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			var attackString = "The " + monster.Name + " casts a bolt of lightning at you!";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " hits you for " + spellDamage + " arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void BloodLeechAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 100, MaxHitPoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Vampire) {HitPoints = 10, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster.Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.BloodLeech);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var monsterHealthBase = monster.HitPoints;
			MonsterAbility.UseBloodLeechAbility(monster, player, abilityIndex);
			var abilityCost = monster.Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			var leechAmount = monster.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - leechAmount, player.HitPoints);
			Assert.AreEqual(monsterHealthBase + leechAmount, monster.HitPoints);
			var attackString = "The " + monster.Name + " tries to sink its fangs into you and suck your blood!";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " leeches " + leechAmount + " life from you.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void PoisonBiteAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 100, MaxHitPoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Spider) {HitPoints = 50, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster.Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.PoisonBite);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			var abilityCost = monster.Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			var abilityDamage = monster.Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = monster.Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = monster.Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = monster.Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var attackString = "The " + monster.Name + " tries to bite you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " bites you for " + abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			var bleedString = "You are bleeding from " + monster.Name + "'s attack!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(
				true, player.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(player.MaxHitPoints - abilityDamage,player.HitPoints);
			Assert.AreEqual(abilityCurRounds,player.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds,player.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				player.Effects[0].BleedingRound(player);
				var bleedAmount = player.Effects[0].EffectAmountOverTime;
				var bleedRoundString = "You bleed for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(player);
			}
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds, 
				player.HitPoints);
		}
		[Test]
		public void TailWhipAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 200, MaxHitPoints = 200};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = monster.Abilities.FindIndex(
				f => f.AbilityCategory == MonsterAbility.Ability.TailWhip);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			MonsterAbility.UseOffenseDamageAbility(monster, player, abilityIndex);
			var abilityCost = monster.Abilities[abilityIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			var attackDamage = monster.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - attackDamage, player.HitPoints);
			var attackString = "The " + monster.Name + " swings its tail at you!";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " strikes you with its tail for " + 
			                          attackDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void FirebreathSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {HitPoints = 200, MaxHitPoints = 200};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			var spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Fireball);
			foreach (var item in player.Inventory.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			monster.MonsterWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			MonsterSpell.CastFireOffense(monster, player, spellIndex);
			var spellCost = monster.Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			var spellDamage = monster.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			var spellMaxRounds = monster.Spellbook[spellIndex].Offensive.AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].EffectMaxRound);
			var attackString = "The " + monster.Name + " breathes a pillar of fire at you!";
			Assert.AreEqual(attackString,OutputHandler.Display.Output[0][2]);
			var attackSuccessString = "The " + monster.Name + " hits you for " + spellDamage + " fire damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.OnFire);
			var burnDamage = monster.Spellbook[spellIndex].Offensive.AmountOverTime;
			for (var i = 2; i < 5; i++) {
				player.Effects[0].OnFireRound(player);
				var burnString = "You burn for " + burnDamage + " fire damage.";
				Assert.AreEqual(burnString, OutputHandler.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(player);
			}
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - spellDamage - burnDamage * 3, player.HitPoints);
		}
		[Test]
		public void HealSpellUnitTest() {
			var monster = new Monster(3, Monster.MonsterType.Troll) {HitPoints = 10, MaxHitPoints = 100};
			while (monster.TrollCategory != Monster.TrollType.Shaman) {
				monster = new Monster(3, Monster.MonsterType.Troll) {HitPoints = 10, MaxHitPoints = 100};
			}
			MonsterBuilder.BuildMonster(monster);
			OutputHandler.Display.ClearUserOutput();
			var spellIndex = monster.Spellbook.FindIndex(
				f => f.SpellCategory == MonsterSpell.SpellType.Heal);
			var monsterHealthBase = monster.HitPoints;
			MonsterSpell.CastHealing(monster, spellIndex);
			var spellCost = monster.Spellbook[spellIndex].EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			var healAmount = monster.Spellbook[spellIndex].Healing.HealAmount;
			Assert.AreEqual(monsterHealthBase + healAmount, monster.HitPoints);
			var healString = "The " + monster.Name + " heals itself for " + healAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[0][2]);
		}
	}
}