using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class WarAbilityUnitTests {
		[Test]
		public void SlashAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var inputInfo = new[] {"ability", "slash"};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Slash);
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Slash", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			var input = new [] {"use", "slash"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("slash", abilityName);
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - player.Abilities[abilityIndex].RageCost, 
				player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			var abilitySuccessString = "Your " + player.Abilities[abilityIndex].Name + " hit the " + monster.Name + " for " +
			                           abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void RendAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Rend);
			var inputInfo = new[] {"ability", "rend"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Rend", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " + 
			                          player.Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "rend"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = player.Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = player.Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = player.Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var abilitySuccessString = "Your " + player.Abilities[abilityIndex].Name + " hit the " + monster.Name + " for " +
			                           abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[6][2]);
			var bleedString = "The " + monster.Name + " is bleeding!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage,monster.HitPoints);
			Assert.AreEqual(abilityCurRounds,monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds,monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].BleedingRound(monster);
				var bleedAmount = monster.Effects[0].EffectAmountOverTime;
				var bleedRoundString = "The " + monster.Name + " bleeds for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds, 
				monster.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Charge);
			var inputInfo = new[] {"ability", "charge"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Charge", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			var abilityInfoString = "Stuns opponent for " + 
			                        player.Abilities[abilityIndex].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "charge"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var abilityDamage = player.Abilities[abilityIndex].Stun.DamageAmount;
			var abilityCurRounds = player.Abilities[abilityIndex].Stun.StunCurRounds;
			var abilityMaxRounds = player.Abilities[abilityIndex].Stun.StunMaxRounds;
			var attackSuccessString = "You " +  player.Abilities[abilityIndex].Name  + " the " + monster.Name + " for " +
			                          abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[5][2]);
			var stunString = "The " + monster.Name + " is stunned!";
			Assert.AreEqual(stunString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage,monster.HitPoints);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].StunnedRound(monster);
				var stunnedString = "The " + monster.Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHandler.Display.Output[i-2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffects(monster);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void BlockAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true, MaxHitPoints = 100, HitPoints = 100, DodgeChance = 0};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Block);
			var inputInfo = new[] {"ability", "block"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Block", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Block Damage: 50", OutputHandler.Display.Output[3][2]);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			Assert.AreEqual(blockInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "block"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("block", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var blockAmount = player.Abilities[abilityIndex].Defensive.BlockDamage;
			var blockString = "You start blocking your opponent's attacks! You will block " + blockAmount + " damage.";
			Assert.AreEqual(blockString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(
				true, player.Effects[0].EffectGroup == Effect.EffectType.BlockDamage);
			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			var blockAmountRemaining = player.Effects[0].EffectAmount;
			Assert.AreEqual(blockAmount, blockAmountRemaining);
			var combatSim = new CombatHandler(monster, player);
			var i = 0;
			while (blockAmountRemaining > 0) {
				var blockAmountBefore = blockAmountRemaining;
				combatSim.ProcessMonsterAttack();
				blockAmountRemaining = player.Effects.Any() ? player.Effects[0].EffectAmount : 0;
				Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
				var blockRoundString = "Your defensive move blocked " + (blockAmountBefore - blockAmountRemaining) + " damage!";
				Assert.AreEqual(blockRoundString, OutputHandler.Display.Output[i][2]);
				i++;
				GameHandler.RemovedExpiredEffects(player);
			}
			const string blockEndString = "You are no longer blocking damage!";
			Assert.AreEqual(blockEndString, OutputHandler.Display.Output[i][2]);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void BerserkAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true, MaxHitPoints = 100, HitPoints = 100, DodgeChance = 0};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Berserk);
			var inputInfo = new[] {"ability", "berserk"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Berserk", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHandler.Display.Output[2][2]);
			var dmgIncreaseString = "Damage Increase: " + player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(dmgIncreaseString, OutputHandler.Display.Output[3][2]);
			var armDecreaseString = "Armor Decrease: " + player.Abilities[abilityIndex].ChangeAmount.Amount;
			Assert.AreEqual(armDecreaseString, OutputHandler.Display.Output[4][2]);
			var dmgInfoString = "Damage increased at cost of armor decrease for " +
			                    player.Abilities[abilityIndex].ChangeAmount.ChangeMaxRound + " rounds";
			Assert.AreEqual(dmgInfoString, OutputHandler.Display.Output[5][2]);
			var input = new [] {"use", "berserk"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("berserk", abilityName);
			var baseArmorRating = GearHandler.CheckArmorRating(player);
			var baseDamage = player.Attack(monster);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			Assert.AreEqual(2, player.Effects.Count);
			Assert.AreEqual(Effect.EffectType.ChangePlayerDamage, player.Effects[0].EffectGroup);
			Assert.AreEqual(Effect.EffectType.ChangeArmor, player.Effects[1].EffectGroup);
			const string berserkString = "You go into a berserk rage!";
			Assert.AreEqual(berserkString, OutputHandler.Display.Output[6][2]);
			for (var i = 2; i < 6; i++) {
				OutputHandler.Display.ClearUserOutput();
				var berserkArmorAmount = player.Abilities[abilityIndex].ChangeAmount.Amount;
				var berserkDamageAmount = player.Abilities[abilityIndex].Offensive.Amount;
				var berserkArmorRating = GearHandler.CheckArmorRating(player);
				var berserkDamage = player.Attack(monster);
				Assert.AreEqual(berserkArmorRating, baseArmorRating + berserkArmorAmount);
				Assert.AreEqual(berserkDamage, baseDamage + berserkDamageAmount, 5);
				player.Effects[0].ChangePlayerDamageRound(player);
				var changeDmgString = "Your damage is increased by " + berserkDamageAmount + ".";
				Assert.AreEqual(changeDmgString, OutputHandler.Display.Output[0][2]);
				player.Effects[1].ChangeArmorRound();
				var changeArmorString = "Your armor is decreased by " + berserkArmorAmount + ".";
				Assert.AreEqual(changeArmorString, OutputHandler.Display.Output[1][2]);
				GameHandler.RemovedExpiredEffects(player);
			}
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void DisarmAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				MaxHitPoints = 100, HitPoints = 100};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Disarm);
			var inputInfo = new[] {"ability", "disarm"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Disarm", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			var abilityString = player.Abilities[abilityIndex].Offensive.Amount + "% chance to disarm opponent's weapon.";
			Assert.AreEqual(abilityString, OutputHandler.Display.Output[3][2]);
			player.Abilities[abilityIndex].Offensive.Amount = 0; // Set disarm success chance to 0% for test
			var input = new [] {"use", "disarm"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("disarm", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			Assert.AreEqual(true, monster.MonsterWeapon.Equipped);
			var disarmFailString = "You tried to disarm " + monster.Name + " but failed!";
			Assert.AreEqual(disarmFailString, OutputHandler.Display.Output[4][2]);
			player.Abilities[abilityIndex].Offensive.Amount = 100; // Set disarm success chance to 100% for test
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - rageCost * 2,player.RagePoints);
			Assert.AreEqual(false, monster.MonsterWeapon.Equipped);
			var disarmSuccessString = "You successfully disarmed " + monster.Name + "!";
			Assert.AreEqual(disarmSuccessString, OutputHandler.Display.Output[5][2]);
		}
		[Test]
		public void BandageAbilityUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				MaxHitPoints = 100, HitPoints = 10};
			GearHandler.EquipInitialGear(player);
			player.Abilities.Add(new Ability(
				"bandage", 25, 1, Ability.WarriorAbility.Bandage, 2));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Bandage);
			var inputInfo = new[] {"ability", "bandage"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputHandler.Display.Output[4][2]);
			var healInfoStringCombat = "Heal over time will restore health for " + 
			                           player.Abilities[abilityIndex].Healing.HealMaxRounds + " rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputHandler.Display.Output[5][2]);
			var healInfoStringNonCombat = "Heal over time will restore health " + 
			                              player.Abilities[abilityIndex].Healing.HealMaxRounds + " times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputHandler.Display.Output[6][2]);
			var input = new [] {"use", "bandage"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			var baseHitPoints = player.HitPoints;
			player.UseAbility(input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			var healAmount = player.Abilities[abilityIndex].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player.Effects[0].EffectGroup);
			Assert.AreEqual(baseHitPoints + healAmount, player.HitPoints);
			baseHitPoints = player.HitPoints;
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				player.Effects[0].HealingRound(player);
				var healOverTimeAmt = player.Effects[0].EffectAmountOverTime;
				var healAmtString = "You have been healed for " + healOverTimeAmt + " health.";
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + (i - 1) * healOverTimeAmt, player.HitPoints);
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void PowerAuraAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 150, RagePoints = 150};
			player.Abilities.Add(new Ability(
				"power aura", 150, 1, Ability.WarriorAbility.PowerAura, 6));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.PowerAura);
			var inputInfo = new[] {"ability", "power", "aura"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Power Aura", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Power Aura Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Strength is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			var input = new [] {"use", "power", "aura"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("power aura", abilityName);
			var baseStr = player.Strength;
			var baseRage = player.RagePoints;
			var baseMaxRage = player.MaxRagePoints;
			player.UseAbility(input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(baseMaxRage - rageCost,player.RagePoints);
			Assert.AreEqual("You generate a Power Aura around yourself.", OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(
				baseRage - player.Abilities[abilityIndex].RageCost, player.RagePoints);
			Assert.AreEqual(player.Strength, baseStr + player.Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				player.MaxRagePoints, baseMaxRage + player.Abilities[abilityIndex].ChangeAmount.Amount * 10);
			Assert.AreEqual(Effect.EffectType.ChangeStat, player.Effects[0].EffectGroup);
			for (var i = 0; i < 600; i++) {
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseStr, player.Strength);
			Assert.AreEqual(baseMaxRage, player.MaxRagePoints);
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
		}
		[Test]
		public void WarCryAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true};
			player.Abilities.Add(new Ability(
				"war cry", 50, 1, Ability.WarriorAbility.WarCry, 4));
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.WarCry);
			var inputInfo = new[] {"ability", "war", "cry"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("War Cry", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 50", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("War Cry Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Opponent's attacks are decreased by 25 for 3 rounds.", 
				OutputHandler.Display.Output[4][2]);
			var input = new[] {"use", "war", "cry"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("war cry", abilityName);
			player.UseAbility(input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost,player.RagePoints);
			Assert.AreEqual("You shout a War Cry, intimidating your opponent, and decreasing incoming damage.", 
				OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(Effect.EffectType.ChangeOpponentDamage, player.Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++) {
				var baseAttackDamageM = monster.Attack(player);
				var attackDamageM = baseAttackDamageM;
				var changeDamageAmount = player.Effects[0].EffectAmountOverTime < attackDamageM ?
					player.Effects[0].EffectAmountOverTime : attackDamageM;
				attackDamageM -= changeDamageAmount;
				Assert.AreEqual(baseAttackDamageM - changeDamageAmount, attackDamageM);
				player.Effects[0].ChangeOpponentDamageRound(player);
				var changeDmgString = "Incoming damage is decreased by " + player.Effects[0].EffectAmountOverTime + ".";
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				Assert.AreEqual(changeDmgString, OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void OnslaughtAbilityUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior) {MaxRagePoints = 100, RagePoints = 100,
				InCombat = true};
			player.Abilities.Add(new Ability(
				"onslaught", 25, 1, Ability.WarriorAbility.Onslaught, 8));
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100, InCombat = true};
			var abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == Ability.WarriorAbility.Onslaught);
			var inputInfo = new[] {"ability", "onslaught"};
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Onslaught", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual(
				"Two attacks are launched which each cause instant damage. Cost and damage are per attack.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] {"use", "onslaught"};
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("onslaught", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player.Abilities[abilityIndex].RageCost;
			var hitAmount = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.MaxHitPoints - 2 * hitAmount,monster.HitPoints);
			Assert.AreEqual(player.MaxRagePoints - 2 * rageCost, player.RagePoints);
			var attackString = "Your onslaught hit the " + monster.Name + " for 25" + " physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			player.MaxRagePoints = 25;
			player.RagePoints = player.MaxRagePoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[7][2]);
			const string outOfRageString = "You didn't have enough rage points for the second attack!";
			Assert.AreEqual(outOfRageString, OutputHandler.Display.Output[8][2]);
		}
	}
}