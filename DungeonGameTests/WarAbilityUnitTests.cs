using DungeonGame;
using DungeonGame.Effects;
using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace DungeonGameTests {
	public class WarAbilityUnitTests {
		[Test]
		public void SlashAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) { MaxRagePoints = 100, RagePoints = 100 };
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			string[] inputInfo = new[] { "ability", "slash" };
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Slash);
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Slash", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHelper.Display.Output[3][2]);
			string[] input = new[] { "use", "slash" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("slash", abilityName);
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - player.Abilities[abilityIndex].RageCost,
				player.RagePoints);
			int abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			string abilitySuccessString = $"Your {player.Abilities[abilityIndex].Name} hit the {monster.Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHelper.Display.Output[4][2]);
		}
		[Test]
		public void RendAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) { MaxRagePoints = 100, RagePoints = 100 };
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Rend);
			string[] inputInfo = new[] { "ability", "rend" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Rend", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHelper.Display.Output[4][2]);
			string bleedOverTimeString = $"Bleeding damage over time for {player.Abilities[abilityIndex].Offensive.AmountMaxRounds} rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHelper.Display.Output[5][2]);
			string[] input = new[] { "use", "rend" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			int abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			int abilityDamageOverTime = player.Abilities[abilityIndex].Offensive.AmountOverTime;
			int abilityCurRounds = player.Abilities[abilityIndex].Offensive.AmountCurRounds;
			int abilityMaxRounds = player.Abilities[abilityIndex].Offensive.AmountMaxRounds;
			string abilitySuccessString = $"Your {player.Abilities[abilityIndex].Name} hit the {monster.Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHelper.Display.Output[6][2]);
			string bleedString = $"The {monster.Name} is bleeding!";
			Assert.AreEqual(bleedString, OutputHelper.Display.Output[7][2]);
			Assert.AreEqual(true, monster.Effects[0] is BleedingEffect);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage, monster.HitPoints);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].CurrentRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].MaxRound);
			BleedingEffect bleedEffect = monster.Effects[0] as BleedingEffect;
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				bleedEffect.ProcessRound();
				int bleedAmount = bleedEffect.BleedDamageOverTime;
				string bleedRoundString = $"The {monster.Name} bleeds for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHelper.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster.Effects[0].CurrentRound);
				GameHelper.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage - (abilityDamageOverTime * abilityMaxRounds),
				monster.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				InCombat = true
			};
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Charge);
			string[] inputInfo = new[] { "ability", "charge" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Charge", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			string abilityInfoString = $"Stuns opponent for {player.Abilities[abilityIndex].Stun.StunMaxRounds} rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "charge" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			int abilityDamage = player.Abilities[abilityIndex].Stun.DamageAmount;
			int abilityCurRounds = player.Abilities[abilityIndex].Stun.StunCurRounds;
			int abilityMaxRounds = player.Abilities[abilityIndex].Stun.StunMaxRounds;
			string attackSuccessString = $"You {player.Abilities[abilityIndex].Name} the {monster.Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHelper.Display.Output[5][2]);
			string stunString = $"The {monster.Name} is stunned!";
			Assert.AreEqual(stunString, OutputHelper.Display.Output[6][2]);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage, monster.HitPoints);
			Assert.AreEqual(true, monster.Effects[0] is StunnedEffect);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].CurrentRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].MaxRound);
			StunnedEffect stunnedEffect = monster.Effects[0] as StunnedEffect;
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 4; i++) {
				stunnedEffect.ProcessRound();
				string stunnedString = $"The {monster.Name} is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHelper.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster.Effects[0].CurrentRound);
				GameHelper.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void BlockAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				InCombat = true,
				MaxHitPoints = 100,
				HitPoints = 100,
				DodgeChance = 0,
				Level = 3
			};
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Zombie) { HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon.CritMultiplier = 1;
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Block);
			string[] inputInfo = new[] { "ability", "block" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Block", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Block Damage: 50", OutputHelper.Display.Output[3][2]);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			Assert.AreEqual(blockInfoString, OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "block" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("block", abilityName);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			int blockAmount = player.Abilities[abilityIndex].Defensive.BlockDamage;
			string blockString = $"You start blocking your opponent's attacks! You will block {blockAmount} damage.";
			Assert.AreEqual(blockString, OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(true, player.Effects[0] is BlockDamageEffect);
			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
			BlockDamageEffect blockDmgEffect = player.Effects[0] as BlockDamageEffect;
			int blockAmountRemaining = blockDmgEffect.BlockAmount;
			Assert.AreEqual(blockAmount, blockAmountRemaining);
			int i = 0;
			while (blockAmountRemaining > 0) {
				monster.MonsterWeapon.Durability = 100;
				int blockAmountBefore = blockAmountRemaining;
				monster.Attack(player);
				blockAmountRemaining = player.Effects.Any() ? blockDmgEffect.BlockAmount : 0;
				if (blockAmountRemaining > 0) {
					Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
					string blockRoundString = $"Your defensive move blocked {(blockAmountBefore - blockAmountRemaining)} damage!";
					Assert.AreEqual(blockRoundString, OutputHelper.Display.Output[i + (i * 1)][2]);
				} else {
					GameHelper.RemovedExpiredEffectsAsync(player);
					int attackAmount = monster.MonsterWeapon.Attack() - blockAmountBefore;
					Assert.AreEqual(player.MaxHitPoints - attackAmount, player.HitPoints);
					const string blockEndString = "You are no longer blocking damage!";
					Assert.AreEqual(blockEndString, OutputHelper.Display.Output[i + 3][2]);
					Thread.Sleep(1000);
					Assert.AreEqual(false, player.Effects.Any());
					string hitString = $"The {monster.Name} hits you for {attackAmount} physical damage.";
					Assert.AreEqual(hitString, OutputHelper.Display.Output[i + 4][2]);
				}
				i++;
			}
		}
		[Test]
		public void BerserkAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				InCombat = true,
				MaxHitPoints = 100,
				HitPoints = 100,
				DodgeChance = 0
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Berserk);
			string[] inputInfo = new[] { "ability", "berserk" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Berserk", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHelper.Display.Output[2][2]);
			string dmgIncreaseString = $"Damage Increase: {player.Abilities[abilityIndex].Offensive.Amount}";
			Assert.AreEqual(dmgIncreaseString, OutputHelper.Display.Output[3][2]);
			string armDecreaseString = $"Armor Decrease: {player.Abilities[abilityIndex].ChangeAmount.Amount}";
			Assert.AreEqual(armDecreaseString, OutputHelper.Display.Output[4][2]);
			string dmgInfoString = $"Damage increased at cost of armor decrease for {player.Abilities[abilityIndex].ChangeAmount.ChangeMaxRound} rounds";
			Assert.AreEqual(dmgInfoString, OutputHelper.Display.Output[5][2]);
			string[] input = new[] { "use", "berserk" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("berserk", abilityName);
			int baseArmorRating = GearHelper.CheckArmorRating(player);
			int baseDamage = player.PhysicalAttack(monster);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			Assert.AreEqual(2, player.Effects.Count);
			Assert.AreEqual(true, player.Effects[0] is ChangePlayerDamageEffect);
			Assert.AreEqual(true, player.Effects[1] is ChangeArmorEffect);
			ChangePlayerDamageEffect changePlayerDmgEffect = player.Effects[0] as ChangePlayerDamageEffect;
			ChangeArmorEffect changeArmorEffect = player.Effects[1] as ChangeArmorEffect;
			const string berserkString = "You go into a berserk rage!";
			Assert.AreEqual(berserkString, OutputHelper.Display.Output[6][2]);
			for (int i = 2; i < 6; i++) {
				OutputHelper.Display.ClearUserOutput();
				int berserkArmorAmount = player.Abilities[abilityIndex].ChangeAmount.Amount;
				int berserkDamageAmount = player.Abilities[abilityIndex].Offensive.Amount;
				int berserkArmorRating = GearHelper.CheckArmorRating(player);
				int berserkDamage = player.PhysicalAttack(monster);
				Assert.AreEqual(berserkArmorRating, baseArmorRating + berserkArmorAmount);
				Assert.AreEqual(berserkDamage, baseDamage + berserkDamageAmount, 5);
				changePlayerDmgEffect.ProcessChangePlayerDamageRound(player);
				string changeDmgString = $"Your damage is increased by {berserkDamageAmount}.";
				Assert.AreEqual(changeDmgString, OutputHelper.Display.Output[0][2]);
				changeArmorEffect.ProcessChangeArmorRound();
				string changeArmorString = $"Your armor is decreased by {berserkArmorAmount * -1}.";
				Assert.AreEqual(changeArmorString, OutputHelper.Display.Output[1][2]);
				GameHelper.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void DisarmAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				MaxHitPoints = 100,
				HitPoints = 100
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Disarm);
			string[] inputInfo = new[] { "ability", "disarm" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Disarm", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			string abilityString = $"{player.Abilities[abilityIndex].Offensive.Amount}% chance to disarm opponent's weapon.";
			Assert.AreEqual(abilityString, OutputHelper.Display.Output[3][2]);
			player.Abilities[abilityIndex].Offensive.Amount = 0; // Set disarm success chance to 0% for test
			string[] input = new[] { "use", "disarm" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("disarm", abilityName);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			Assert.AreEqual(true, monster.MonsterWeapon.Equipped);
			string disarmFailString = $"You tried to disarm {monster.Name} but failed!";
			Assert.AreEqual(disarmFailString, OutputHelper.Display.Output[4][2]);
			player.Abilities[abilityIndex].Offensive.Amount = 100; // Set disarm success chance to 100% for test
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - (rageCost * 2), player.RagePoints);
			Assert.AreEqual(false, monster.MonsterWeapon.Equipped);
			string disarmSuccessString = $"You successfully disarmed {monster.Name}!";
			Assert.AreEqual(disarmSuccessString, OutputHelper.Display.Output[5][2]);
		}
		[Test]
		public void BandageAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			player.Abilities.Add(new PlayerAbility(
				"bandage", 25, 1, WarriorAbility.Bandage, 2));
			OutputHelper.Display.ClearUserOutput();
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Bandage);
			string[] inputInfo = new[] { "ability", "bandage" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputHelper.Display.Output[4][2]);
			string healInfoStringCombat = $"Heal over time will restore health for {player.Abilities[abilityIndex].Healing.HealMaxRounds} rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputHelper.Display.Output[5][2]);
			string healInfoStringNonCombat = $"Heal over time will restore health {player.Abilities[abilityIndex].Healing.HealMaxRounds} times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputHelper.Display.Output[6][2]);
			string[] input = new[] { "use", "bandage" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			int baseHitPoints = player.HitPoints;
			player.UseAbility(input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			int healAmount = player.Abilities[abilityIndex].Healing.HealAmount;
			string healString = $"You heal yourself for {healAmount} health.";
			Assert.AreEqual(healString, OutputHelper.Display.Output[7][2]);
			Assert.AreEqual(true, player.Effects[0] is HealingEffect);
			Assert.AreEqual(baseHitPoints + healAmount, player.HitPoints);
			baseHitPoints = player.HitPoints;
			HealingEffect healEffect = player.Effects[0] as HealingEffect;
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				healEffect.ProcessHealingRound(player);
				int healOverTimeAmt = healEffect.HealOverTimeAmount;
				string healAmtString = $"You have been healed for {healOverTimeAmt} health.";
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				Assert.AreEqual(healAmtString, OutputHelper.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + ((i - 1) * healOverTimeAmt), player.HitPoints);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void PowerAuraAbilityUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) { MaxRagePoints = 150, RagePoints = 150 };
			player.Abilities.Add(new PlayerAbility(
				"power aura", 150, 1, WarriorAbility.PowerAura, 6));
			OutputHelper.Display.ClearUserOutput();
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.PowerAura);
			string[] inputInfo = new[] { "ability", "power", "aura" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Power Aura", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 150", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Power Aura Amount: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Strength is increased by 15 for 10 minutes.", OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "power", "aura" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("power aura", abilityName);
			int baseStr = player.Strength;
			int? baseRage = player.RagePoints;
			int? baseMaxRage = player.MaxRagePoints;
			player.UseAbility(input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(baseMaxRage - rageCost, player.RagePoints);
			Assert.AreEqual("You generate a Power Aura around yourself.", OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(
				baseRage - player.Abilities[abilityIndex].RageCost, player.RagePoints);
			Assert.AreEqual(player.Strength, baseStr + player.Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				player.MaxRagePoints, baseMaxRage + player.Abilities[abilityIndex].ChangeAmount.Amount * 10);
			Assert.AreEqual(true, player.Effects[0] is ChangeStatEffect);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 0; i < 600; i++) {
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseStr, player.Strength);
			Assert.AreEqual(baseMaxRage, player.MaxRagePoints);
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
		}
		[Test]
		public void WarCryAbilityUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				InCombat = true
			};
			player.Abilities.Add(new PlayerAbility(
				"war cry", 50, 1, WarriorAbility.WarCry, 4));
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.WarCry);
			string[] inputInfo = new[] { "ability", "war", "cry" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("War Cry", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 50", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("War Cry Amount: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Opponent's attacks are decreased by 25 for 3 rounds.",
				OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "war", "cry" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("war cry", abilityName);
			player.UseAbility(input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			Assert.AreEqual("You shout a War Cry, intimidating your opponent, and decreasing incoming damage.",
				OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(true, player.Effects[0] is ChangeMonsterDamageEffect);
			ChangeMonsterDamageEffect changeMonsterDmgEffect = player.Effects[0] as ChangeMonsterDamageEffect;
			for (int i = 2; i < 5; i++) {
				int baseAttackDamageM = monster.UnarmedAttackDamage;
				int attackDamageM = baseAttackDamageM;
				int changeDamageAmount = changeMonsterDmgEffect.ChangeAmount < attackDamageM ?
					changeMonsterDmgEffect.ChangeAmount : attackDamageM;
				attackDamageM -= changeDamageAmount;
				Assert.AreEqual(baseAttackDamageM - changeDamageAmount, attackDamageM);
				changeMonsterDmgEffect.ProcessChangeMonsterDamageRound(player);
				string changeDmgString = $"Incoming damage is decreased by {-1 * changeMonsterDmgEffect.ChangeAmount}.";
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				Assert.AreEqual(changeDmgString, OutputHelper.Display.Output[i - 2][2]);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void OnslaughtAbilityUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Warrior) {
				MaxRagePoints = 100,
				RagePoints = 100,
				InCombat = true
			};
			player.Abilities.Add(new PlayerAbility(
				"onslaught", 25, 1, WarriorAbility.Onslaught, 8));
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = player.Abilities.FindIndex(
				f => f.WarAbilityCategory == WarriorAbility.Onslaught);
			string[] inputInfo = new[] { "ability", "onslaught" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Onslaught", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual(
				"Two attacks are launched which each cause instant damage. Cost and damage are per attack.",
				OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "onslaught" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("onslaught", abilityName);
			player.UseAbility(monster, input);
			int? rageCost = player.Abilities[abilityIndex].RageCost;
			int hitAmount = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.MaxHitPoints - (2 * hitAmount), monster.HitPoints);
			Assert.AreEqual(player.MaxRagePoints - (2 * rageCost), player.RagePoints);
			string attackString = $"Your onslaught hit the {monster.Name} for 25 physical damage.";
			Assert.AreEqual(attackString, OutputHelper.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHelper.Display.Output[6][2]);
			player.MaxRagePoints = 25;
			player.RagePoints = player.MaxRagePoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.UseAbility(monster, input);
			Assert.AreEqual(player.MaxRagePoints - rageCost, player.RagePoints);
			Assert.AreEqual(attackString, OutputHelper.Display.Output[7][2]);
			const string outOfRageString = "You didn't have enough rage points for the second attack!";
			Assert.AreEqual(outOfRageString, OutputHelper.Display.Output[8][2]);
		}
	}
}