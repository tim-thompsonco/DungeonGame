using System.Collections.Generic;
using System.Linq;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class ConsumableUnitTests {
		[Test]
		public void HealthPotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Archer) {
				MaxHitPoints = 100,
				HitPoints = 10,
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Health)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Health);
			var input = new [] {"drink", "health"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("health", potionName);
			var baseHealth = player.HitPoints;
			var healAmount = player.Consumables[potionIndex].RestoreHealth.RestoreHealthAmt;
			player.DrinkPotion(input);
			var drankHealthString = "You drank a potion and replenished " + healAmount + " health.";
			Assert.AreEqual(drankHealthString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseHealth + healAmount, player.HitPoints);
			Assert.IsEmpty(player.Consumables);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ManaPotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {
				MaxManaPoints = 100,
				ManaPoints = 10,
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Mana)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Mana);
			var input = new [] {"drink", "mana"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("mana", potionName);
			var baseMana = player.ManaPoints;
			var manaAmount = player.Consumables[potionIndex].RestoreMana.RestoreManaAmt;
			player.DrinkPotion(input);
			var drankManaString = "You drank a potion and replenished " + manaAmount + " mana.";
			Assert.AreEqual(drankManaString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseMana + manaAmount, player.ManaPoints);
			Assert.IsEmpty(player.Consumables);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ConstitutionPotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Constitution)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Constitution);
			var input = new [] {"drink", "constitution"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("constitution", potionName);
			var statAmount = player.Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player.Consumables[potionIndex].ChangeStat.StatCategory;
			var baseConst = player.Constitution;
			var baseMaxHitPoints = player.MaxHitPoints;
			player.DrinkPotion(input);
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseConst + statAmount, player.Constitution);
			Assert.AreEqual(baseMaxHitPoints + statAmount * 10, player.MaxHitPoints);
			Assert.IsEmpty(player.Consumables);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseConst, player.Constitution);
			Assert.AreEqual(baseMaxHitPoints, player.MaxHitPoints);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void IntelligencePotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Intelligence)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Intelligence);
			var input = new [] {"drink", "intelligence"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("intelligence", potionName);
			var statAmount = player.Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player.Consumables[potionIndex].ChangeStat.StatCategory;
			var baseInt = player.Intelligence;
			var baseMaxManaPoints = player.MaxManaPoints;
			player.DrinkPotion(input);
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseInt + statAmount, player.Intelligence);
			Assert.AreEqual(baseMaxManaPoints + statAmount * 10, player.MaxManaPoints);
			Assert.IsEmpty(player.Consumables);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseInt, player.Intelligence);
			Assert.AreEqual(baseMaxManaPoints, player.MaxManaPoints);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void StrengthPotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Strength)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Strength);
			var input = new [] {"drink", "strength"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("strength", potionName);
			var statAmount = player.Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player.Consumables[potionIndex].ChangeStat.StatCategory;
			var baseStr = player.Strength;
			var baseMaxCarryWeight = player.MaxCarryWeight;
			player.DrinkPotion(input);
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseStr + statAmount, player.Strength);
			Assert.AreEqual(baseMaxCarryWeight + statAmount * 2, player.MaxCarryWeight);
			Assert.IsEmpty(player.Consumables);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseStr, player.Strength);
			Assert.AreEqual(baseMaxCarryWeight, player.MaxCarryWeight);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void DexterityPotionUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {
				Consumables = new List<Consumable> {new Consumable(1, Consumable.PotionType.Dexterity)}
			};
			var potionIndex = player.Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Dexterity);
			var input = new [] {"drink", "dexterity"};
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("dexterity", potionName);
			var statAmount = player.Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player.Consumables[potionIndex].ChangeStat.StatCategory;
			var baseDex = player.Dexterity;
			var baseDodgeChance = player.DodgeChance;
			player.DrinkPotion(input);
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseDex + statAmount, player.Dexterity);
			Assert.AreEqual(baseDodgeChance + statAmount * 1.5, player.DodgeChance);
			Assert.IsEmpty(player.Consumables);
			Assert.AreEqual(player.Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffects(player);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseDex, player.Dexterity);
			Assert.AreEqual(baseDodgeChance, player.DodgeChance);
			player.DrinkPotion(input);
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ArmorUpgradeKitUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {
				Consumables = new List<Consumable> {new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Armor, 
					ChangeArmor.KitType.Cloth)}
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var armorIndex = player.Inventory.FindIndex(f => f.Name.Contains("cloth"));
			var armor = player.Inventory[armorIndex] as Armor;
			var armorName = player.Inventory[armorIndex].Name;
			var kitAmount = player.Consumables[0].ChangeArmor.ChangeAmount;
			var kitName = player.Consumables[0].Name;
			var input = new [] {"enhance", "doesn't exist", kitName};
			var armorAmount = armor.ArmorRating;
			GearHandler.UseArmorKit(player, input);
			const string upgradeFail = "What armor did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputHandler.Display.Output[0][2]);
			Assert.IsNotEmpty(player.Consumables);
			input = new [] {"enhance", armorName, "doesn't exist"};
			GearHandler.UseArmorKit(player, input);
			const string kitFail = "What armor kit did you want to use?";
			Assert.AreEqual(kitFail, OutputHandler.Display.Output[1][2]);
			Assert.IsNotEmpty(player.Consumables);
			input = new [] {"enhance", armorName, kitName};
			GearHandler.UseArmorKit(player, input);
			var upgradeSuccess = "You upgraded " + armor.Name + " with an armor kit.";
			Assert.AreEqual(upgradeSuccess, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(armorAmount + kitAmount, armor.ArmorRating);
			Assert.IsEmpty(player.Consumables);
			player.Consumables.Add(new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Armor, 
				ChangeArmor.KitType.Leather));
			kitName = player.Consumables[0].Name;
			input = new [] {"enhance", armorName, kitName};
			GearHandler.UseArmorKit(player, input);
			var enhanceFail = "You can't upgrade " + armor.Name + " with that!";
			Assert.IsNotEmpty(player.Consumables);
			Assert.AreEqual(enhanceFail
			, OutputHandler.Display.Output[3][2]);
			player.Consumables[0] = new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Weapon,
				ChangeArmor.KitType.Cloth);
			kitName = player.Consumables[0].Name;
			input = new [] {"enhance", armorName, kitName};
			GearHandler.UseArmorKit(player, input);
			Assert.IsNotEmpty(player.Consumables);
			Assert.AreEqual(enhanceFail
				, OutputHandler.Display.Output[4][2]);
		}
	}
}