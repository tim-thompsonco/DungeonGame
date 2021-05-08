using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using DungeonGame.Items.Consumables.Potions;
using DungeonGame.Players;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace DungeonGameTests {
	public class IItemUnitTests {
		[Test]
		public void HealthPotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Archer) {
				MaxHitPoints = 100,
				HitPoints = 10,
				Inventory = new List<IItem> { new HealthPotion(PotionStrength.Minor) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(HealthPotion));
			string[] input = new[] { "drink", "health" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("health", potionName);
			int baseHealth = player.HitPoints;
			HealthPotion potion = player.Inventory[potionIndex] as HealthPotion;
			int healAmount = potion.HealthAmount;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankHealthString = $"You drank a potion and replenished {healAmount} health.";
			Assert.AreEqual(drankHealthString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseHealth + healAmount, player.HitPoints);
			Assert.IsEmpty(player.Inventory);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ManaPotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				MaxManaPoints = 100,
				ManaPoints = 10,
				Inventory = new List<IItem> { new ManaPotion(PotionStrength.Minor) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(ManaPotion));
			string[] input = new[] { "drink", "mana" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("mana", potionName);
			int? baseMana = player.ManaPoints;
			ManaPotion potion = player.Inventory[potionIndex] as ManaPotion;
			int manaAmount = potion.ManaAmount;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankManaString = $"You drank a potion and replenished {manaAmount} mana.";
			Assert.AreEqual(drankManaString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseMana + manaAmount, player.ManaPoints);
			Assert.IsEmpty(player.Inventory);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ConstitutionPotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "constitution" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("constitution", potionName);
			StatPotion potion = player.Inventory[potionIndex] as StatPotion;
			int statAmount = potion.StatAmount;
			StatPotion.StatType statType = potion.StatPotionType;
			int baseConst = player.Constitution;
			int baseMaxHitPoints = player.MaxHitPoints;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseConst + statAmount, player.Constitution);
			Assert.AreEqual(baseMaxHitPoints + (statAmount * 10), player.MaxHitPoints);
			Assert.IsEmpty(player.Inventory);
			Assert.AreEqual(true, player.Effects[0] is ChangeStatEffect);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseConst, player.Constitution);
			Assert.AreEqual(baseMaxHitPoints, player.MaxHitPoints);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void IntelligencePotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Intelligence) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "intelligence" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("intelligence", potionName);
			StatPotion potion = player.Inventory[potionIndex] as StatPotion;
			int statAmount = potion.StatAmount;
			StatPotion.StatType statType = potion.StatPotionType;
			int baseInt = player.Intelligence;
			int? baseMaxManaPoints = player.MaxManaPoints;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseInt + statAmount, player.Intelligence);
			Assert.AreEqual(baseMaxManaPoints + (statAmount * 10), player.MaxManaPoints);
			Assert.IsEmpty(player.Inventory);
			Assert.AreEqual(true, player.Effects[0] is ChangeStatEffect);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseInt, player.Intelligence);
			Assert.AreEqual(baseMaxManaPoints, player.MaxManaPoints);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void StrengthPotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Strength) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "strength" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("strength", potionName);
			StatPotion potion = player.Inventory[potionIndex] as StatPotion;
			int statAmount = potion.StatAmount;
			StatPotion.StatType statType = potion.StatPotionType;
			int baseStr = player.Strength;
			int baseMaxCarryWeight = player.MaxCarryWeight;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseStr + statAmount, player.Strength);
			Assert.AreEqual(baseMaxCarryWeight + (statAmount * 2.5), player.MaxCarryWeight, 1);
			Assert.IsEmpty(player.Inventory);
			Assert.AreEqual(true, player.Effects[0] is ChangeStatEffect);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseStr, player.Strength);
			Assert.AreEqual(baseMaxCarryWeight, player.MaxCarryWeight);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void DexterityPotionUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Dexterity) }
			};
			int potionIndex = player.Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "dexterity" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("dexterity", potionName);
			StatPotion potion = player.Inventory[potionIndex] as StatPotion;
			int statAmount = potion.StatAmount;
			StatPotion.StatType statType = potion.StatPotionType;
			int baseDex = player.Dexterity;
			double baseDodgeChance = player.DodgeChance;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseDex + statAmount, player.Dexterity);
			Assert.AreEqual(baseDodgeChance + (statAmount * 1.5), player.DodgeChance);
			Assert.IsEmpty(player.Inventory);
			Assert.AreEqual(true, player.Effects[0] is ChangeStatEffect);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 1; i < 601; i++) {
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseDex, player.Dexterity);
			Assert.AreEqual(baseDodgeChance, player.DodgeChance);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ArmorUpgradeKitUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			player.Inventory.Add(new ArmorKit(KitLevel.Light, ArmorKit.KitType.Cloth));
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			int armorIndex = player.Inventory.FindIndex(f => f.Name.Contains("cloth"));
			Armor armor = player.Inventory[armorIndex] as Armor;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string armorName = textInfo.ToTitleCase(player.Inventory[armorIndex].Name);
			ArmorKit armorKit = player.Inventory.Find(item => item is ArmorKit) as ArmorKit;
			int kitAmount = armorKit.KitAugmentAmount;
			string kitName = armorKit.Name;
			string[] input = new[] { "enhance", "doesn't exist", kitName };
			int armorAmount = armor.ArmorRating;
			GearController.UseArmorKit(player, input);
			const string upgradeFail = "What armor did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputController.Display.Output[0][2]);
			Assert.IsNotEmpty(player.Inventory);
			input = new[] { "enhance", armorName, "doesn't exist" };
			GearController.UseArmorKit(player, input);
			const string kitFail = "What armor kit did you want to use?";
			Assert.AreEqual(kitFail, OutputController.Display.Output[1][2]);
			Assert.IsNotEmpty(player.Inventory);
			input = new[] { "enhance", armorName, kitName };
			GearController.UseArmorKit(player, input);
			string upgradeSuccess = $"You upgraded {armorName} with an armor kit.";
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[2][2]);
			Assert.AreEqual(armorAmount + kitAmount, armor.ArmorRating);
			Assert.AreEqual(0, player.Inventory.FindAll(item => item is IKit).Count);
			player.Inventory.Add(new ArmorKit(KitLevel.Light, ArmorKit.KitType.Leather));
			kitName = player.Inventory.Find(item => item is ArmorKit).Name;
			input = new[] { "enhance", armorName, kitName };
			GearController.UseArmorKit(player, input);
			string enhanceFail = $"You can't upgrade {armorName} with that!";
			Assert.IsNotEmpty(player.Inventory);
			Assert.AreEqual(enhanceFail, OutputController.Display.Output[3][2]);
		}
		[Test]
		public void WeaponUpgradeKitUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage);
			player.Inventory.Add(new WeaponKit(KitLevel.Light, WeaponKit.KitType.Grindstone));
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			int weaponIndex = player.Inventory.FindIndex(f => f.Name.Contains("dagger"));
			Weapon weapon = player.Inventory[weaponIndex] as Weapon;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string weaponName = textInfo.ToTitleCase(player.Inventory[weaponIndex].Name);
			WeaponKit weaponKit = player.Inventory.Find(item => item is WeaponKit) as WeaponKit;
			int kitAmount = weaponKit.KitAugmentAmount;
			string kitName = weaponKit.Name;
			string[] input = new[] { "enhance", "doesn't exist", kitName };
			int weaponAmount = weapon._RegDamage;
			GearController.UseWeaponKit(player, input);
			const string upgradeFail = "What weapon did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputController.Display.Output[0][2]);
			Assert.IsNotEmpty(player.Inventory);
			input = new[] { "enhance", weaponName, "doesn't exist" };
			GearController.UseWeaponKit(player, input);
			const string kitFail = "What weapon kit did you want to use?";
			Assert.AreEqual(kitFail, OutputController.Display.Output[1][2]);
			Assert.IsNotEmpty(player.Inventory);
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			string upgradeSuccess = $"You upgraded {weaponName} with a weapon kit.";
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[2][2]);
			Assert.AreEqual(weaponAmount + kitAmount, weapon._RegDamage);
			Assert.AreEqual(0, player.Inventory.FindAll(item => item is IKit).Count);
			player.Inventory.Add(new WeaponKit(KitLevel.Light, WeaponKit.KitType.Bowstring));
			weaponKit = player.Inventory.Find(item => item is WeaponKit) as WeaponKit;
			kitName = weaponKit.Name;
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			string enhanceFail = $"You can't upgrade {weaponName} with that!";
			Assert.IsNotEmpty(player.Inventory);
			Assert.AreEqual(enhanceFail, OutputController.Display.Output[3][2]);
			player.Inventory.Add(new Weapon(1, Weapon.WeaponType.Bow));
			weapon = player.Inventory.Find(item => item is Weapon && item.Name.ToLower().Contains("bow")) as Weapon;
			weapon.Equipped = true;
			weaponName = textInfo.ToTitleCase(weapon.Name);
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			upgradeSuccess = $"You upgraded {weaponName} with a weapon kit.";
			Assert.AreEqual(0, player.Inventory.FindAll(item => item is IKit).Count);
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[4][2]);
		}
	}
}