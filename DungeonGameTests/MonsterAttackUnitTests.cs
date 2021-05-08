using DungeonGame;
using DungeonGame.AttackOptions;
using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace DungeonGameTests {
	public class MonsterAttackUnitTests {
		[Test]
		public void DetermineAttackUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 100, MaxHitPoints = 100 };
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			OutputController.Display.ClearUserOutput();
			AttackOption attackChoice = monster.DetermineAttack(player, false);
			Assert.AreEqual(AttackType.Spell, attackChoice.AttackCategory);
			monster.EnergyPoints = 0;
			attackChoice = monster.DetermineAttack(player, false);
			Assert.AreEqual(AttackType.Physical, attackChoice.AttackCategory);
		}
		[Test]
		public void FireballSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 100, MaxHitPoints = 100, FireResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon);
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			int spellIndex = monster.Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Fireball);
			monster.Spellbook[spellIndex].CastFireOffense(monster, player, spellIndex);
			int spellCost = monster.Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			int spellDamage = monster.Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			int spellMaxRounds = monster.Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].MaxRound);
			string attackString = $"The {monster.Name} casts a fireball and launches it at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} hits you for {spellDamage} fire damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(true, player.Effects[0] is BurningEffect);
			int burnDamage = monster.Spellbook[spellIndex]._Offensive._AmountOverTime;
			BurningEffect burnEffect = player.Effects[0] as BurningEffect;
			for (int i = 2; i < 5; i++) {
				burnEffect.ProcessBurningRound(player);
				string burnString = $"You burn for {burnDamage} fire damage.";
				Assert.AreEqual(burnString, OutputController.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				GameController.RemovedExpiredEffectsAsync(player);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - spellDamage - (burnDamage * 3), player.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 200, MaxHitPoints = 200, FrostResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(1, Monster.MonsterType.Skeleton);
			while (monster.SkeletonCategory != Monster.SkeletonType.Mage) {
				monster = new Monster(1, Monster.MonsterType.Skeleton);
			}
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			int spellIndex = monster.Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Frostbolt);
			foreach (IItem item in player.Inventory.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			monster.Spellbook[spellIndex].CastFrostOffense(monster, player, spellIndex);
			int spellCost = monster.Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			int spellDamage = monster.Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			int spellMaxRounds = monster.Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].MaxRound);
			string attackString = $"The {monster.Name} conjures up a frostbolt and launches it at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} hits you for {spellDamage} frost damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be increased by 50%!";
			Assert.AreEqual(frozenString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(true, player.Effects[0] is FrozenEffect);
			FrozenEffect frozenEffect = player.Effects[0] as FrozenEffect;
			// Remove all spells after casting to make monster decide to use physical attack for unit test
			monster.Spellbook = null;
			for (int i = 2; i < 4; i++) {
				int playerHitPointsBefore = player.HitPoints;
				double multiplier = frozenEffect.EffectMultiplier;
				int baseDamage = monster.MonsterWeapon._RegDamage;
				int frozenDamage = (int)(monster.MonsterWeapon._RegDamage * multiplier);
				Assert.AreEqual(frozenDamage, baseDamage * multiplier, 1);
				monster.MonsterWeapon._Durability = 100;
				monster.Attack(player);
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				Assert.AreEqual(frozenString, OutputController.Display.Output[i][2]);
				Assert.AreEqual(playerHitPointsBefore - frozenDamage, player.HitPoints, 7);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void LightningSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 200, MaxHitPoints = 200, ArcaneResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Elemental);
			while (monster.ElementalCategory != Monster.ElementalType.Air) {
				monster = new Monster(3, Monster.MonsterType.Elemental);
			}
			MonsterBuilder.BuildMonster(monster);
			int spellIndex = monster.Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Lightning);
			foreach (IItem item in player.Inventory.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			monster.Spellbook[spellIndex].CastArcaneOffense(monster, player, spellIndex);
			int spellCost = monster.Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			int spellDamage = monster.Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			string attackString = $"The {monster.Name} casts a bolt of lightning at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} hits you for {spellDamage} arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void BloodLeechAbilityUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 100, MaxHitPoints = 100 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Vampire) { HitPoints = 10, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster.Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.BloodLeech);
			foreach (IItem item in player.Inventory.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int monsterHealthBase = monster.HitPoints;
			monster.Abilities[abilityIndex].UseBloodLeechAbility(monster, player, abilityIndex);
			int abilityCost = monster.Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			int leechAmount = monster.Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - leechAmount, player.HitPoints);
			Assert.AreEqual(monsterHealthBase + leechAmount, monster.HitPoints);
			string attackString = $"The {monster.Name} tries to sink its fangs into you and suck your blood!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} leeches {leechAmount} life from you.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void PoisonBiteAbilityUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 100, MaxHitPoints = 100 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Spider) { HitPoints = 50, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster.Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.PoisonBite);
			foreach (IItem item in player.Inventory.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			monster.Abilities[abilityIndex].UseOffenseDamageAbility(monster, player, abilityIndex);
			int abilityCost = monster.Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			int abilityDamage = monster.Abilities[abilityIndex]._Offensive._Amount;
			int abilityDamageOverTime = monster.Abilities[abilityIndex]._Offensive._AmountOverTime;
			int abilityCurRounds = monster.Abilities[abilityIndex]._Offensive._AmountCurRounds;
			int abilityMaxRounds = monster.Abilities[abilityIndex]._Offensive._AmountMaxRounds;
			string attackString = $"The {monster.Name} tries to bite you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} bites you for {abilityDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			string bleedString = $"You are bleeding from {monster.Name}'s attack!";
			Assert.AreEqual(bleedString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(true, player.Effects[0] is BleedingEffect);
			Assert.AreEqual(player.MaxHitPoints - abilityDamage, player.HitPoints);
			Assert.AreEqual(abilityCurRounds, player.Effects[0].CurrentRound);
			Assert.AreEqual(abilityMaxRounds, player.Effects[0].MaxRound);
			OutputController.Display.ClearUserOutput();
			BleedingEffect bleedEffect = player.Effects[0] as BleedingEffect;
			for (int i = 2; i < 5; i++) {
				bleedEffect.ProcessBleedingRound(player);
				int bleedAmount = bleedEffect.BleedDamageOverTime;
				string bleedRoundString = $"You bleed for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputController.Display.Output[i - 2][2]);
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				GameController.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - abilityDamage - (abilityDamageOverTime * abilityMaxRounds),
				player.HitPoints);
		}
		[Test]
		public void TailWhipAbilityUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 200, MaxHitPoints = 200 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = monster.Abilities.FindIndex(
				f => f._AbilityCategory == MonsterAbility.Ability.TailWhip);
			foreach (IItem item in player.Inventory.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			monster.Abilities[abilityIndex].UseOffenseDamageAbility(monster, player, abilityIndex);
			int abilityCost = monster.Abilities[abilityIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - abilityCost, monster.EnergyPoints);
			int attackDamage = monster.Abilities[abilityIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - attackDamage, player.HitPoints);
			string attackString = $"The {monster.Name} swings its tail at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} strikes you with its tail for {attackDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
		}
		[Test]
		public void FirebreathSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { HitPoints = 200, MaxHitPoints = 200, FireResistance = 0 };
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Dragon);
			MonsterBuilder.BuildMonster(monster);
			int spellIndex = monster.Spellbook.FindIndex(
				f => f._SpellCategory == MonsterSpell.SpellType.Fireball);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			monster.MonsterWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			monster.Spellbook[spellIndex].CastFireOffense(monster, player, spellIndex);
			int spellCost = monster.Spellbook[spellIndex]._EnergyCost;
			Assert.AreEqual(monster.MaxEnergyPoints - spellCost, monster.EnergyPoints);
			int spellDamage = monster.Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(player.MaxHitPoints - spellDamage, player.HitPoints);
			int spellMaxRounds = monster.Spellbook[spellIndex]._Offensive._AmountMaxRounds;
			Assert.AreEqual(spellMaxRounds, player.Effects[0].MaxRound);
			string attackString = $"The {monster.Name} breathes a pillar of fire at you!";
			Assert.AreEqual(attackString, OutputController.Display.Output[0][2]);
			string attackSuccessString = $"The {monster.Name} hits you for {spellDamage} fire damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display.Output[1][2]);
			const string onFireString = "You burst into flame!";
			Assert.AreEqual(onFireString, OutputController.Display.Output[2][2]);
			Assert.AreEqual(true, player.Effects[0] is BurningEffect);
			int burnDamage = monster.Spellbook[spellIndex]._Offensive._AmountOverTime;
			BurningEffect burnEffect = player.Effects[0] as BurningEffect;
			for (int i = 2; i < 5; i++) {
				burnEffect.ProcessBurningRound(player);
				string burnString = $"You burn for {burnDamage} fire damage.";
				Assert.AreEqual(burnString, OutputController.Display.Output[i + 1][2]);
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				GameController.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(player.MaxHitPoints - spellDamage - (burnDamage * 3), player.HitPoints);
		}
	}
}