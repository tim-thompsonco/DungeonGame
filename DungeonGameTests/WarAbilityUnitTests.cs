using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class WarAbilityUnitTests
	{
		[Test]
		public void SlashAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior) { _MaxRagePoints = 100, _RagePoints = 100 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var inputInfo = new[] { "ability", "slash" };
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Slash);
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Slash", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHandler.Display.Output[3][2]);
			var input = new[] { "use", "slash" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("slash", abilityName);
			player.UseAbility(monster, input);
			Assert.AreEqual(player._MaxRagePoints - player._Abilities[abilityIndex].RageCost,
				player._RagePoints);
			var abilityDamage = player._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			var abilitySuccessString = "Your " + player._Abilities[abilityIndex].Name + " hit the " + monster._Name + " for " +
									   abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void RendAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior) { _MaxRagePoints = 100, _RagePoints = 100 };
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Rend);
			var inputInfo = new[] { "ability", "rend" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Rend", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			var bleedOverTimeString = "Bleeding damage over time for " +
									  player._Abilities[abilityIndex].Offensive.AmountMaxRounds + " rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHandler.Display.Output[5][2]);
			var input = new[] { "use", "rend" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("rend", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			var abilityDamage = player._Abilities[abilityIndex].Offensive.Amount;
			var abilityDamageOverTime = player._Abilities[abilityIndex].Offensive.AmountOverTime;
			var abilityCurRounds = player._Abilities[abilityIndex].Offensive.AmountCurRounds;
			var abilityMaxRounds = player._Abilities[abilityIndex].Offensive.AmountMaxRounds;
			var abilitySuccessString = "Your " + player._Abilities[abilityIndex].Name + " hit the " + monster._Name + " for " +
									   abilityDamage + " physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHandler.Display.Output[6][2]);
			var bleedString = "The " + monster._Name + " is bleeding!";
			Assert.AreEqual(bleedString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Bleeding);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage, monster.HitPoints);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				monster.Effects[0].BleedingRound(monster);
				var bleedAmount = monster.Effects[0].EffectAmountOverTime;
				var bleedRoundString = "The " + monster._Name + " bleeds for " + bleedAmount + " physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage - abilityDamageOverTime * abilityMaxRounds,
				monster.HitPoints);
		}
		[Test]
		public void ChargeAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_InCombat = true
			};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Charge);
			var inputInfo = new[] { "ability", "charge" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Charge", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			var abilityInfoString = "Stuns opponent for " +
									player._Abilities[abilityIndex].Stun.StunMaxRounds + " rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "charge" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("charge", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			var abilityDamage = player._Abilities[abilityIndex].Stun.DamageAmount;
			var abilityCurRounds = player._Abilities[abilityIndex].Stun.StunCurRounds;
			var abilityMaxRounds = player._Abilities[abilityIndex].Stun.StunMaxRounds;
			var attackSuccessString = "You " + player._Abilities[abilityIndex].Name + " the " + monster._Name + " for " +
									  abilityDamage + " physical damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[5][2]);
			var stunString = "The " + monster._Name + " is stunned!";
			Assert.AreEqual(stunString, OutputHandler.Display.Output[6][2]);
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage, monster.HitPoints);
			Assert.AreEqual(
				true, monster.Effects[0].EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(abilityCurRounds, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(abilityMaxRounds, monster.Effects[0].EffectMaxRound);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 4; i++)
			{
				monster.Effects[0].StunnedRound(monster);
				var stunnedString = "The " + monster._Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void BlockAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_InCombat = true,
				_MaxHitPoints = 100,
				_HitPoints = 100,
				_DodgeChance = 0,
				_Level = 3
			};
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Zombie)
			{ HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			monster.MonsterWeapon.CritMultiplier = 1;
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Block);
			var inputInfo = new[] { "ability", "block" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Block", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Block Damage: 50", OutputHandler.Display.Output[3][2]);
			const string blockInfoString =
				"Block damage will prevent incoming damage from opponent until block damage is used up.";
			Assert.AreEqual(blockInfoString, OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "block" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("block", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			var blockAmount = player._Abilities[abilityIndex].Defensive.BlockDamage;
			var blockString = "You start blocking your opponent's attacks! You will block " + blockAmount + " damage.";
			Assert.AreEqual(blockString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(
				true, player._Effects[0].EffectGroup == Effect.EffectType.BlockDamage);
			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
			var blockAmountRemaining = player._Effects[0].EffectAmount;
			Assert.AreEqual(blockAmount, blockAmountRemaining);
			var i = 0;
			while (blockAmountRemaining > 0)
			{
				monster.MonsterWeapon.Durability = 100;
				var blockAmountBefore = blockAmountRemaining;
				monster.Attack(player);
				blockAmountRemaining = player._Effects.Any() ? player._Effects[0].EffectAmount : 0;
				if (blockAmountRemaining > 0)
				{
					Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
					var blockRoundString = "Your defensive move blocked " + (blockAmountBefore - blockAmountRemaining) + " damage!";
					Assert.AreEqual(blockRoundString, OutputHandler.Display.Output[i + (i * 1)][2]);
				}
				else
				{
					GameHandler.RemovedExpiredEffectsAsync(player);
					var attackAmount = monster.MonsterWeapon.Attack() - blockAmountBefore;
					Assert.AreEqual(player._MaxHitPoints - attackAmount, player._HitPoints);
					const string blockEndString = "You are no longer blocking damage!";
					Assert.AreEqual(blockEndString, OutputHandler.Display.Output[i + 3][2]);
					Thread.Sleep(1000);
					Assert.AreEqual(false, player._Effects.Any());
					var hitString = "The " + monster._Name + " hits you for " + attackAmount + " physical damage.";
					Assert.AreEqual(hitString, OutputHandler.Display.Output[i + 4][2]);
				}
				i++;
			}
		}
		[Test]
		public void BerserkAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_InCombat = true,
				_MaxHitPoints = 100,
				_HitPoints = 100,
				_DodgeChance = 0
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Berserk);
			var inputInfo = new[] { "ability", "berserk" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Berserk", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 40", OutputHandler.Display.Output[2][2]);
			var dmgIncreaseString = "Damage Increase: " + player._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(dmgIncreaseString, OutputHandler.Display.Output[3][2]);
			var armDecreaseString = "Armor Decrease: " + player._Abilities[abilityIndex].ChangeAmount.Amount;
			Assert.AreEqual(armDecreaseString, OutputHandler.Display.Output[4][2]);
			var dmgInfoString = "Damage increased at cost of armor decrease for " +
								player._Abilities[abilityIndex].ChangeAmount.ChangeMaxRound + " rounds";
			Assert.AreEqual(dmgInfoString, OutputHandler.Display.Output[5][2]);
			var input = new[] { "use", "berserk" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("berserk", abilityName);
			var baseArmorRating = GearHandler.CheckArmorRating(player);
			var baseDamage = player.PhysicalAttack(monster);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			Assert.AreEqual(2, player._Effects.Count);
			Assert.AreEqual(Effect.EffectType.ChangePlayerDamage, player._Effects[0].EffectGroup);
			Assert.AreEqual(Effect.EffectType.ChangeArmor, player._Effects[1].EffectGroup);
			const string berserkString = "You go into a berserk rage!";
			Assert.AreEqual(berserkString, OutputHandler.Display.Output[6][2]);
			for (var i = 2; i < 6; i++)
			{
				OutputHandler.Display.ClearUserOutput();
				var berserkArmorAmount = player._Abilities[abilityIndex].ChangeAmount.Amount;
				var berserkDamageAmount = player._Abilities[abilityIndex].Offensive.Amount;
				var berserkArmorRating = GearHandler.CheckArmorRating(player);
				var berserkDamage = player.PhysicalAttack(monster);
				Assert.AreEqual(berserkArmorRating, baseArmorRating + berserkArmorAmount);
				Assert.AreEqual(berserkDamage, baseDamage + berserkDamageAmount, 5);
				player._Effects[0].ChangePlayerDamageRound(player);
				var changeDmgString = "Your damage is increased by " + berserkDamageAmount + ".";
				Assert.AreEqual(changeDmgString, OutputHandler.Display.Output[0][2]);
				player._Effects[1].ChangeArmorRound();
				var changeArmorString = "Your armor is decreased by " + berserkArmorAmount * -1 + ".";
				Assert.AreEqual(changeArmorString, OutputHandler.Display.Output[1][2]);
				GameHandler.RemovedExpiredEffectsAsync(player);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void DisarmAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 100
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Disarm);
			var inputInfo = new[] { "ability", "disarm" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Disarm", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			var abilityString = player._Abilities[abilityIndex].Offensive.Amount + "% chance to disarm opponent's weapon.";
			Assert.AreEqual(abilityString, OutputHandler.Display.Output[3][2]);
			player._Abilities[abilityIndex].Offensive.Amount = 0; // Set disarm success chance to 0% for test
			var input = new[] { "use", "disarm" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("disarm", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			Assert.AreEqual(true, monster.MonsterWeapon.Equipped);
			var disarmFailString = "You tried to disarm " + monster._Name + " but failed!";
			Assert.AreEqual(disarmFailString, OutputHandler.Display.Output[4][2]);
			player._Abilities[abilityIndex].Offensive.Amount = 100; // Set disarm success chance to 100% for test
			player.UseAbility(monster, input);
			Assert.AreEqual(player._MaxRagePoints - rageCost * 2, player._RagePoints);
			Assert.AreEqual(false, monster.MonsterWeapon.Equipped);
			var disarmSuccessString = "You successfully disarmed " + monster._Name + "!";
			Assert.AreEqual(disarmSuccessString, OutputHandler.Display.Output[5][2]);
		}
		[Test]
		public void BandageAbilityUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 10
			};
			GearHandler.EquipInitialGear(player);
			player._Abilities.Add(new PlayerAbility(
				"bandage", 25, 1, PlayerAbility.WarriorAbility.Bandage, 2));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Bandage);
			var inputInfo = new[] { "ability", "bandage" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 5", OutputHandler.Display.Output[4][2]);
			var healInfoStringCombat = "Heal over time will restore health for " +
									   player._Abilities[abilityIndex].Healing.HealMaxRounds + " rounds in combat.";
			Assert.AreEqual(healInfoStringCombat, OutputHandler.Display.Output[5][2]);
			var healInfoStringNonCombat = "Heal over time will restore health " +
										  player._Abilities[abilityIndex].Healing.HealMaxRounds + " times every 10 seconds.";
			Assert.AreEqual(healInfoStringNonCombat, OutputHandler.Display.Output[6][2]);
			var input = new[] { "use", "bandage" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("bandage", abilityName);
			var baseHitPoints = player._HitPoints;
			player.UseAbility(input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			var healAmount = player._Abilities[abilityIndex].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[7][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player._Effects[0].EffectGroup);
			Assert.AreEqual(baseHitPoints + healAmount, player._HitPoints);
			baseHitPoints = player._HitPoints;
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].HealingRound(player);
				var healOverTimeAmt = player._Effects[0].EffectAmountOverTime;
				var healAmtString = "You have been healed for " + healOverTimeAmt + " health.";
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + (i - 1) * healOverTimeAmt, player._HitPoints);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void PowerAuraAbilityUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior) { _MaxRagePoints = 150, _RagePoints = 150 };
			player._Abilities.Add(new PlayerAbility(
				"power aura", 150, 1, PlayerAbility.WarriorAbility.PowerAura, 6));
			OutputHandler.Display.ClearUserOutput();
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.PowerAura);
			var inputInfo = new[] { "ability", "power", "aura" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Power Aura", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Power Aura Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("_Strength is increased by 15 for 10 minutes.", OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "power", "aura" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("power aura", abilityName);
			var baseStr = player._Strength;
			var baseRage = player._RagePoints;
			var baseMaxRage = player._MaxRagePoints;
			player.UseAbility(input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(baseMaxRage - rageCost, player._RagePoints);
			Assert.AreEqual("You generate a Power Aura around yourself.", OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(
				baseRage - player._Abilities[abilityIndex].RageCost, player._RagePoints);
			Assert.AreEqual(player._Strength, baseStr + player._Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				player._MaxRagePoints, baseMaxRage + player._Abilities[abilityIndex].ChangeAmount.Amount * 10);
			Assert.AreEqual(Effect.EffectType.ChangeStat, player._Effects[0].EffectGroup);
			for (var i = 0; i < 600; i++)
			{
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseStr, player._Strength);
			Assert.AreEqual(baseMaxRage, player._MaxRagePoints);
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
		}
		[Test]
		public void WarCryAbilityUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_InCombat = true
			};
			player._Abilities.Add(new PlayerAbility(
				"war cry", 50, 1, PlayerAbility.WarriorAbility.WarCry, 4));
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.WarCry);
			var inputInfo = new[] { "ability", "war", "cry" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("War Cry", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 50", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("War Cry Amount: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Opponent's attacks are decreased by 25 for 3 rounds.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "war", "cry" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("war cry", abilityName);
			player.UseAbility(input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			Assert.AreEqual("You shout a War Cry, intimidating your opponent, and decreasing incoming damage.",
				OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(Effect.EffectType.ChangeOpponentDamage, player._Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++)
			{
				var baseAttackDamageM = monster.UnarmedAttackDamage;
				var attackDamageM = baseAttackDamageM;
				var changeDamageAmount = player._Effects[0].EffectAmountOverTime < attackDamageM ?
					player._Effects[0].EffectAmountOverTime : attackDamageM;
				attackDamageM -= changeDamageAmount;
				Assert.AreEqual(baseAttackDamageM - changeDamageAmount, attackDamageM);
				player._Effects[0].ChangeOpponentDamageRound(player);
				var changeDmgString = "Incoming damage is decreased by " + -1 * player._Effects[0].EffectAmountOverTime + ".";
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				Assert.AreEqual(changeDmgString, OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void OnslaughtAbilityUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Warrior)
			{
				_MaxRagePoints = 100,
				_RagePoints = 100,
				_InCombat = true
			};
			player._Abilities.Add(new PlayerAbility(
				"onslaught", 25, 1, PlayerAbility.WarriorAbility.Onslaught, 8));
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			var abilityIndex = player._Abilities.FindIndex(
				f => f.WarAbilityCategory == PlayerAbility.WarriorAbility.Onslaught);
			var inputInfo = new[] { "ability", "onslaught" };
			PlayerHandler.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Onslaught", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Rage Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual(
				"Two attacks are launched which each cause instant damage. Cost and damage are per attack.",
				OutputHandler.Display.Output[4][2]);
			var input = new[] { "use", "onslaught" };
			var abilityName = InputHandler.ParseInput(input);
			Assert.AreEqual("onslaught", abilityName);
			player.UseAbility(monster, input);
			var rageCost = player._Abilities[abilityIndex].RageCost;
			var hitAmount = player._Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.MaxHitPoints - 2 * hitAmount, monster.HitPoints);
			Assert.AreEqual(player._MaxRagePoints - 2 * rageCost, player._RagePoints);
			var attackString = "Your onslaught hit the " + monster._Name + " for 25" + " physical damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[6][2]);
			player._MaxRagePoints = 25;
			player._RagePoints = player._MaxRagePoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			player.UseAbility(monster, input);
			Assert.AreEqual(player._MaxRagePoints - rageCost, player._RagePoints);
			Assert.AreEqual(attackString, OutputHandler.Display.Output[7][2]);
			const string outOfRageString = "You didn't have enough rage points for the second attack!";
			Assert.AreEqual(outOfRageString, OutputHandler.Display.Output[8][2]);
		}
	}
}