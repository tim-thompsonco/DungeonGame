using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using DungeonGame.Items.Consumables.Potions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace DungeonGameTests
{
	public class IItemUnitTests
	{
		[Test]
		public void HealthPotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxHitPoints = 100,
				_HitPoints = 10,
				_Inventory = new List<IItem> { new HealthPotion(PotionStrength.Minor) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(HealthPotion));
			string[] input = new[] { "drink", "health" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("health", potionName);
			int baseHealth = player._HitPoints;
			HealthPotion potion = player._Inventory[potionIndex] as HealthPotion;
			int healAmount = potion._HealthAmount;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankHealthString = $"You drank a potion and replenished {healAmount} health.";
			Assert.AreEqual(drankHealthString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseHealth + healAmount, player._HitPoints);
			Assert.IsEmpty(player._Inventory);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ManaPotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{
				_MaxManaPoints = 100,
				_ManaPoints = 10,
				_Inventory = new List<IItem> { new ManaPotion(PotionStrength.Minor) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(ManaPotion));
			string[] input = new[] { "drink", "mana" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("mana", potionName);
			int? baseMana = player._ManaPoints;
			ManaPotion potion = player._Inventory[potionIndex] as ManaPotion;
			int manaAmount = potion._ManaAmount;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankManaString = $"You drank a potion and replenished {manaAmount} mana.";
			Assert.AreEqual(drankManaString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseMana + manaAmount, player._ManaPoints);
			Assert.IsEmpty(player._Inventory);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ConstitutionPotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Constitution) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "constitution" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("constitution", potionName);
			StatPotion potion = player._Inventory[potionIndex] as StatPotion;
			int statAmount = potion._StatAmount;
			StatPotion.StatType statType = potion._StatCategory;
			int baseConst = player._Constitution;
			int baseMaxHitPoints = player._MaxHitPoints;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseConst + statAmount, player._Constitution);
			Assert.AreEqual(baseMaxHitPoints + (statAmount * 10), player._MaxHitPoints);
			Assert.IsEmpty(player._Inventory);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.ChangeStat);
			for (int i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseConst, player._Constitution);
			Assert.AreEqual(baseMaxHitPoints, player._MaxHitPoints);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void IntelligencePotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Intelligence) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "intelligence" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("intelligence", potionName);
			StatPotion potion = player._Inventory[potionIndex] as StatPotion;
			int statAmount = potion._StatAmount;
			StatPotion.StatType statType = potion._StatCategory;
			int baseInt = player._Intelligence;
			int? baseMaxManaPoints = player._MaxManaPoints;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseInt + statAmount, player._Intelligence);
			Assert.AreEqual(baseMaxManaPoints + (statAmount * 10), player._MaxManaPoints);
			Assert.IsEmpty(player._Inventory);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.ChangeStat);
			for (int i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseInt, player._Intelligence);
			Assert.AreEqual(baseMaxManaPoints, player._MaxManaPoints);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void StrengthPotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Strength) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "strength" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("strength", potionName);
			StatPotion potion = player._Inventory[potionIndex] as StatPotion;
			int statAmount = potion._StatAmount;
			StatPotion.StatType statType = potion._StatCategory;
			int baseStr = player._Strength;
			int baseMaxCarryWeight = player._MaxCarryWeight;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseStr + statAmount, player._Strength);
			Assert.AreEqual(baseMaxCarryWeight + (statAmount * 2.5), player._MaxCarryWeight, 1);
			Assert.IsEmpty(player._Inventory);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.ChangeStat);
			for (int i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseStr, player._Strength);
			Assert.AreEqual(baseMaxCarryWeight, player._MaxCarryWeight);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void DexterityPotionUnitTest()
		{
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Inventory = new List<IItem> { new StatPotion(PotionStrength.Minor, StatPotion.StatType.Dexterity) }
			};
			int potionIndex = player._Inventory.FindIndex(f => f.GetType() == typeof(StatPotion));
			string[] input = new[] { "drink", "dexterity" };
			string potionName = InputController.ParseInput(input);
			Assert.AreEqual("dexterity", potionName);
			StatPotion potion = player._Inventory[potionIndex] as StatPotion;
			int statAmount = potion._StatAmount;
			StatPotion.StatType statType = potion._StatCategory;
			int baseDex = player._Dexterity;
			double baseDodgeChance = player._DodgeChance;
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			string drankStatString = $"You drank a potion and increased {statType} by {statAmount}.";
			Assert.AreEqual(drankStatString, OutputController.Display.Output[0][2]);
			Assert.AreEqual(baseDex + statAmount, player._Dexterity);
			Assert.AreEqual(baseDodgeChance + (statAmount * 1.5), player._DodgeChance);
			Assert.IsEmpty(player._Inventory);
			Assert.AreEqual(player._Effects[0]._EffectGroup, Effect.EffectType.ChangeStat);
			for (int i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseDex, player._Dexterity);
			Assert.AreEqual(baseDodgeChance, player._DodgeChance);
			player.AttemptDrinkPotion(InputController.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputController.Display.Output[1][2]);
		}
		[Test]
		public void ArmorUpgradeKitUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage);
			player._Inventory.Add(new ArmorKit(KitLevel.Light, ArmorKit.KitType.Cloth));
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			int armorIndex = player._Inventory.FindIndex(f => f._Name.Contains("cloth"));
			Armor armor = player._Inventory[armorIndex] as Armor;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string armorName = textInfo.ToTitleCase(player._Inventory[armorIndex]._Name);
			ArmorKit armorKit = player._Inventory.Find(item => item is ArmorKit) as ArmorKit;
			int kitAmount = armorKit._KitAugmentAmount;
			string kitName = armorKit._Name;
			string[] input = new[] { "enhance", "doesn't exist", kitName };
			int armorAmount = armor._ArmorRating;
			GearController.UseArmorKit(player, input);
			const string upgradeFail = "What armor did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputController.Display.Output[0][2]);
			Assert.IsNotEmpty(player._Inventory);
			input = new[] { "enhance", armorName, "doesn't exist" };
			GearController.UseArmorKit(player, input);
			const string kitFail = "What armor kit did you want to use?";
			Assert.AreEqual(kitFail, OutputController.Display.Output[1][2]);
			Assert.IsNotEmpty(player._Inventory);
			input = new[] { "enhance", armorName, kitName };
			GearController.UseArmorKit(player, input);
			string upgradeSuccess = $"You upgraded {armorName} with an armor kit.";
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[2][2]);
			Assert.AreEqual(armorAmount + kitAmount, armor._ArmorRating);
			Assert.AreEqual(0, player._Inventory.FindAll(item => item is IKit).Count);
			player._Inventory.Add(new ArmorKit(KitLevel.Light, ArmorKit.KitType.Leather));
			kitName = player._Inventory.Find(item => item is ArmorKit)._Name;
			input = new[] { "enhance", armorName, kitName };
			GearController.UseArmorKit(player, input);
			string enhanceFail = $"You can't upgrade {armorName} with that!";
			Assert.IsNotEmpty(player._Inventory);
			Assert.AreEqual(enhanceFail, OutputController.Display.Output[3][2]);
		}
		[Test]
		public void WeaponUpgradeKitUnitTest()
		{
			Player player = new Player("test", Player.PlayerClassType.Mage);
			player._Inventory.Add(new WeaponKit(KitLevel.Light, WeaponKit.KitType.Grindstone));
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			int weaponIndex = player._Inventory.FindIndex(f => f._Name.Contains("dagger"));
			Weapon weapon = player._Inventory[weaponIndex] as Weapon;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string weaponName = textInfo.ToTitleCase(player._Inventory[weaponIndex]._Name);
			WeaponKit weaponKit = player._Inventory.Find(item => item is WeaponKit) as WeaponKit;
			int kitAmount = weaponKit._KitAugmentAmount;
			string kitName = weaponKit._Name;
			string[] input = new[] { "enhance", "doesn't exist", kitName };
			int weaponAmount = weapon._RegDamage;
			GearController.UseWeaponKit(player, input);
			const string upgradeFail = "What weapon did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputController.Display.Output[0][2]);
			Assert.IsNotEmpty(player._Inventory);
			input = new[] { "enhance", weaponName, "doesn't exist" };
			GearController.UseWeaponKit(player, input);
			const string kitFail = "What weapon kit did you want to use?";
			Assert.AreEqual(kitFail, OutputController.Display.Output[1][2]);
			Assert.IsNotEmpty(player._Inventory);
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			string upgradeSuccess = $"You upgraded {weaponName} with a weapon kit.";
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[2][2]);
			Assert.AreEqual(weaponAmount + kitAmount, weapon._RegDamage);
			Assert.AreEqual(0, player._Inventory.FindAll(item => item is IKit).Count);
			player._Inventory.Add(new WeaponKit(KitLevel.Light, WeaponKit.KitType.Bowstring));
			weaponKit = player._Inventory.Find(item => item is WeaponKit) as WeaponKit;
			kitName = weaponKit._Name;
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			string enhanceFail = $"You can't upgrade {weaponName} with that!";
			Assert.IsNotEmpty(player._Inventory);
			Assert.AreEqual(enhanceFail, OutputController.Display.Output[3][2]);
			player._Inventory.Add(new Weapon(1, Weapon.WeaponType.Bow));
			weapon = player._Inventory.Find(item => item is Weapon && item._Name.ToLower().Contains("bow")) as Weapon;
			weapon._Equipped = true;
			weaponName = textInfo.ToTitleCase(weapon._Name);
			input = new[] { "enhance", weaponName, kitName };
			GearController.UseWeaponKit(player, input);
			upgradeSuccess = $"You upgraded {weaponName} with a weapon kit.";
			Assert.AreEqual(0, player._Inventory.FindAll(item => item is IKit).Count);
			Assert.AreEqual(upgradeSuccess, OutputController.Display.Output[4][2]);
		}
	}
}