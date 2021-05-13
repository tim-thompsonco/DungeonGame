using DungeonGame;
using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DungeonGameTests {
	public class ArcAbilityUnitTests {
		[Test]
		public void DistanceShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			RoomHelper.Rooms = new Dictionary<Coordinate, IRoom> {
				{new Coordinate(0, 0, 0), new DungeonRoom(1, 1)},
				{new Coordinate(0, 1, 0), new DungeonRoom(1, 1)},
				{new Coordinate(1, 0, 0), new DungeonRoom(1, 1)}
			};
			Coordinate roomOneCoord = new Coordinate(0, 1, 0);
			Coordinate roomTwoCoord = new Coordinate(1, 0, 0);
			RoomHelper.Rooms[roomTwoCoord].Monster = null;
			RoomHelper.Rooms[roomOneCoord].Monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			Monster monster = RoomHelper.Rooms[roomOneCoord].Monster;
			MonsterBuilder.BuildMonster(monster);
			RoomHelper.SetPlayerLocation(player, 0, 0, 0);
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			string[] inputInfo = new[] { "ability", "distance" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Distance Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("50% chance to hit monster in attack direction.",
				OutputHelper.Display.Output[4][2]);
			Assert.AreEqual("Usage example if monster is in room to north. 'use distance north'",
				OutputHelper.Display.Output[5][2]);
			string[] input = new[] { "use", "distance", "south" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("distance south", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player.PlayerQuiver.Quantity);
			Assert.AreEqual("You can't attack in that direction!", OutputHelper.Display.Output[6][2]);
			Assert.AreEqual(player.MaxComboPoints, player.ComboPoints);
			input = new[] { "use", "distance", "east" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount, player.PlayerQuiver.Quantity);
			Assert.AreEqual("There is no monster in that direction to attack!",
				OutputHelper.Display.Output[7][2]);
			Assert.AreEqual(player.MaxComboPoints, player.ComboPoints);
			Assert.AreEqual(player.MaxComboPoints, player.ComboPoints);
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Distance);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			player.Abilities[abilityIndex].Offensive.ChanceToSucceed = 0;
			input = new[] { "use", "distance", "north" };
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			string missString = $"You tried to shoot {monster.Name} from afar but failed!";
			Assert.AreEqual(missString, OutputHelper.Display.Output[8][2]);
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			player.Abilities[abilityIndex].Offensive.ChanceToSucceed = 100;
			player.ComboPoints = player.MaxComboPoints;
			arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int abilityDmg = player.Abilities[abilityIndex].Offensive.Amount;
			string shootString = $"You successfully shot {monster.Name} from afar for {abilityDmg} damage!";
			Assert.AreEqual(shootString, OutputHelper.Display.Output[9][2]);
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			Assert.AreEqual(monster.MaxHitPoints - abilityDmg, monster.HitPoints);
		}
		[Test]
		public void GutShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Gut);
			string[] inputInfo = new[] { "ability", "gut" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Gut Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHelper.Display.Output[4][2]);
			string bleedOverTimeString = $"Bleeding damage over time for {player.Abilities[abilityIndex].Offensive.AmountMaxRounds} rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHelper.Display.Output[5][2]);
			string[] input = new[] { "use", "gut" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("gut", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
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
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				BleedingEffect bleedEffect = monster.Effects[0] as BleedingEffect;
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
		public void PreciseShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			string[] inputInfo = new[] { "ability", "precise" };
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Precise);
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Precise Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHelper.Display.Output[3][2]);
			string[] input = new[] { "use", "precise" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("precise", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			int abilityDamage = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
			string abilitySuccessString = $"Your {player.Abilities[abilityIndex].Name} hit the {monster.Name} for {abilityDamage} physical damage.";
			Assert.AreEqual(abilitySuccessString, OutputHelper.Display.Output[4][2]);
		}
		[Test]
		public void StunShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
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
				f => f.ArcAbilityCategory == ArcherAbility.Stun);
			string[] inputInfo = new[] { "ability", "stun" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Stun Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			string abilityInfoString = $"Stuns opponent for {player.Abilities[abilityIndex].Stun.StunMaxRounds} rounds, preventing their attacks.";
			Assert.AreEqual(abilityInfoString, OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "stun" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("stun", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
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
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				StunnedEffect stunnedEffect = monster.Effects[0] as StunnedEffect;
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
		public void WoundShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Wound);
			string[] inputInfo = new[] { "ability", "wound" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Wound Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 40", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 5", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 10", OutputHelper.Display.Output[4][2]);
			string bleedOverTimeString = $"Bleeding damage over time for {player.Abilities[abilityIndex].Offensive.AmountMaxRounds} rounds.";
			Assert.AreEqual(bleedOverTimeString, OutputHelper.Display.Output[5][2]);
			string[] input = new[] { "use", "wound" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("wound", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
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
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 7; i++) {
				BleedingEffect bleedEffect = monster.Effects[0] as BleedingEffect;
				bleedEffect.ProcessRound();
				int bleedAmount = bleedEffect.BleedDamageOverTime;
				string bleedRoundString = $"The {monster.Name} bleeds for {bleedAmount} physical damage.";
				Assert.AreEqual(bleedRoundString, OutputHelper.Display.Output[i - 2][2]);
				Assert.AreEqual(i, monster.Effects[0].CurrentRound);
				GameHelper.RemovedExpiredEffectsAsync(monster);
			}
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(monster.MaxHitPoints - abilityDamage - (abilityDamageOverTime * abilityMaxRounds),
				monster.HitPoints);
		}
		[Test]
		public void DoubleShotAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, InCombat = true };
			MonsterBuilder.BuildMonster(monster);
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Double);
			string[] inputInfo = new[] { "ability", "double" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Double Shot", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual(
				"Two arrows are fired which each cause instant damage. Cost and damage are per arrow.",
				OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "double" };
			string abilityName = InputHelper.ParseInput(input);
			Assert.AreEqual("double", abilityName);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 2, player.PlayerQuiver.Quantity);
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			int hitAmount = player.Abilities[abilityIndex].Offensive.Amount;
			Assert.AreEqual(monster.MaxHitPoints - (2 * hitAmount), monster.HitPoints);
			Assert.AreEqual(player.MaxComboPoints - (2 * comboCost), player.ComboPoints);
			string attackString = $"Your double shot hit the {monster.Name} for 25 physical damage.";
			Assert.AreEqual(attackString, OutputHelper.Display.Output[5][2]);
			Assert.AreEqual(attackString, OutputHelper.Display.Output[6][2]);
			player.MaxComboPoints = 25;
			player.ComboPoints = player.MaxComboPoints;
			monster.MaxHitPoints = 100;
			monster.HitPoints = monster.MaxHitPoints;
			arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			Assert.AreEqual(attackString, OutputHelper.Display.Output[7][2]);
			const string outOfComboString = "You didn't have enough combo points for the second shot!";
			Assert.AreEqual(outOfComboString, OutputHelper.Display.Output[8][2]);
		}
		[Test]
		public void BandageAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			player.Abilities.Add(new PlayerAbility(
				"bandage", 25, 1, ArcherAbility.Bandage, 2));
			OutputHelper.Display.ClearUserOutput();
			int abilityIndex = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Bandage);
			string[] inputInfo = new[] { "ability", "bandage" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Bandage", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 25", OutputHelper.Display.Output[2][2]);
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
			int? comboCost = player.Abilities[abilityIndex].ComboCost;
			Assert.AreEqual(player.MaxComboPoints - comboCost, player.ComboPoints);
			int healAmount = player.Abilities[abilityIndex].Healing.HealAmount;
			string healString = $"You heal yourself for {healAmount} health.";
			Assert.AreEqual(healString, OutputHelper.Display.Output[7][2]);
			Assert.AreEqual(true, player.Effects[0] is HealingEffect);
			Assert.AreEqual(baseHitPoints + healAmount, player.HitPoints);
			baseHitPoints = player.HitPoints;
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				HealingEffect healEffect = player.Effects[0] as HealingEffect;
				healEffect.ProcessRound();
				int healOverTimeAmt = healEffect.HealOverTimeAmount;
				string healAmtString = $"You have been healed for {healOverTimeAmt} health.";
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				Assert.AreEqual(healAmtString, OutputHelper.Display.Output[i - 2][2]);
				Assert.AreEqual(baseHitPoints + ((i - 1) * healOverTimeAmt), player.HitPoints);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void SwiftAuraAbilityUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("placeholder", PlayerClassType.Archer);
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			player.Abilities.Add(new PlayerAbility(
				"swift aura", 150, 1, ArcherAbility.SwiftAura, 6));
			string[] input = new[] { "use", "swift", "aura" };
			PlayerHelper.AbilityInfo(player, input);
			Assert.AreEqual("Swift Aura", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 150", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Swift Aura Amount: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Dexterity is increased by 15 for 10 minutes.", OutputHelper.Display.Output[4][2]);
			int baseDex = player.Dexterity;
			int? baseCombo = player.ComboPoints;
			int? baseMaxCombo = player.MaxComboPoints;
			int abilityIndex = player.Abilities.FindIndex(f => f.Name == InputHelper.ParseInput(input));
			player.UseAbility(input);
			Assert.AreEqual(player.Dexterity, baseDex + player.Abilities[abilityIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseCombo - player.Abilities[abilityIndex].ComboCost, player.ComboPoints);
			Assert.AreEqual(
				player.MaxComboPoints, baseMaxCombo + (player.Abilities[abilityIndex].ChangeAmount.Amount * 10));
			Assert.AreEqual("You generate a Swift Aura around yourself.", OutputHelper.Display.Output[5][2]);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseDex, player.Dexterity);
			Assert.AreEqual(baseMaxCombo, player.MaxComboPoints);
			Assert.AreEqual(baseCombo - player.Abilities[abilityIndex].ComboCost, player.ComboPoints);
		}
		[Test]
		public void ImmolatingArrowAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10,
				InCombat = true
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, InCombat = true, FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			player.Abilities.Add(new PlayerAbility(
				"immolating arrow", 35, 1, ArcherAbility.ImmolatingArrow, 8));
			string[] input = new[] { "use", "immolating", "arrow" };
			PlayerHelper.AbilityInfo(player, input);
			Assert.AreEqual("Immolating Arrow", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 35", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHelper.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time for 3 rounds.", OutputHelper.Display.Output[5][2]);
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			int index = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.ImmolatingArrow);
			Assert.AreEqual(player.ComboPoints, player.MaxComboPoints - player.Abilities[index].ComboCost);
			string attackString = $"Your immolating arrow hit the {monster.Name} for 25 physical damage.";
			Assert.AreEqual(attackString, OutputHelper.Display.Output[6][2]);
			Assert.AreEqual(monster.HitPoints,
				monster.MaxHitPoints - player.Abilities[index].Offensive.Amount);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(true, monster.Effects[0] is BurningEffect);
			Assert.AreEqual(3, monster.Effects[0].MaxRound);
			BurningEffect burnEffect = monster.Effects[0] as BurningEffect;
			for (int i = 0; i < 3; i++) {
				int baseHitPoints = monster.HitPoints;
				burnEffect.ProcessRound();
				Assert.AreEqual(i + 2, monster.Effects[0].CurrentRound);
				Assert.AreEqual(monster.HitPoints, baseHitPoints - burnEffect.FireDamageOverTime);
				string burnString = $"The {monster.Name} burns for {burnEffect.FireDamageOverTime} fire damage.";
				Assert.AreEqual(burnString, OutputHelper.Display.Output[i][2]);
				GameHelper.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
		}
		[Test]
		public void AmbushAbilityUnitTest() {
			Player player = new Player("test", PlayerClassType.Archer) {
				MaxComboPoints = 100,
				ComboPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 10
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			player.Abilities.Add(new PlayerAbility(
				"ambush", 75, 1, ArcherAbility.Ambush, 4));
			string[] inputInfo = new[] { "ability", "ambush" };
			PlayerHelper.AbilityInfo(player, inputInfo);
			Assert.AreEqual("Ambush", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Combo Cost: 75", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 50", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("A surprise attack is launched, which initiates combat.",
				OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "use", "ambush", monster.Name };
			player.InCombat = true;
			int arrowCount = player.PlayerQuiver.Quantity;
			player.UseAbility(monster, input);
			Assert.AreEqual($"You can't ambush {monster.Name}, you're already in combat!",
				OutputHelper.Display.Output[5][2]);
			player.InCombat = false;
			player.UseAbility(monster, input);
			int index = player.Abilities.FindIndex(
				f => f.ArcAbilityCategory == ArcherAbility.Ambush);
			int abilityDamage = player.Abilities[index].Offensive.Amount;
			string attackString = "Your ambush hit the " + monster.Name + " for " + abilityDamage + " physical damage.";
			Assert.AreEqual(arrowCount - 1, player.PlayerQuiver.Quantity);
			Assert.AreEqual(attackString, OutputHelper.Display.Output[6][2]);
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - abilityDamage);
		}
	}
}