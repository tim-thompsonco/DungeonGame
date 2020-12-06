using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class ConsumableUnitTests
	{
		[Test]
		public void HealthPotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Archer)
			{
				_MaxHitPoints = 100,
				_HitPoints = 10,
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Health) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Health);
			var input = new[] { "drink", "health" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("health", potionName);
			var baseHealth = player._HitPoints;
			var healAmount = player._Consumables[potionIndex].RestoreHealth.RestoreHealthAmt;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankHealthString = "You drank a potion and replenished " + healAmount + " health.";
			Assert.AreEqual(drankHealthString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseHealth + healAmount, player._HitPoints);
			Assert.IsEmpty(player._Consumables);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ManaPotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_MaxManaPoints = 100,
				_ManaPoints = 10,
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Mana) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Mana);
			var input = new[] { "drink", "mana" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("mana", potionName);
			var baseMana = player._ManaPoints;
			var manaAmount = player._Consumables[potionIndex].RestoreMana.RestoreManaAmt;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankManaString = "You drank a potion and replenished " + manaAmount + " mana.";
			Assert.AreEqual(drankManaString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseMana + manaAmount, player._ManaPoints);
			Assert.IsEmpty(player._Consumables);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ConstitutionPotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Constitution) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Constitution);
			var input = new[] { "drink", "constitution" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("constitution", potionName);
			var statAmount = player._Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player._Consumables[potionIndex].ChangeStat.StatCategory;
			var baseConst = player._Constitution;
			var baseMaxHitPoints = player._MaxHitPoints;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseConst + statAmount, player._Constitution);
			Assert.AreEqual(baseMaxHitPoints + statAmount * 10, player._MaxHitPoints);
			Assert.IsEmpty(player._Consumables);
			Assert.AreEqual(player._Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseConst, player._Constitution);
			Assert.AreEqual(baseMaxHitPoints, player._MaxHitPoints);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void IntelligencePotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Intelligence) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Intelligence);
			var input = new[] { "drink", "intelligence" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("intelligence", potionName);
			var statAmount = player._Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player._Consumables[potionIndex].ChangeStat.StatCategory;
			var baseInt = player._Intelligence;
			var baseMaxManaPoints = player._MaxManaPoints;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseInt + statAmount, player._Intelligence);
			Assert.AreEqual(baseMaxManaPoints + statAmount * 10, player._MaxManaPoints);
			Assert.IsEmpty(player._Consumables);
			Assert.AreEqual(player._Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseInt, player._Intelligence);
			Assert.AreEqual(baseMaxManaPoints, player._MaxManaPoints);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void StrengthPotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Strength) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Strength);
			var input = new[] { "drink", "strength" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("strength", potionName);
			var statAmount = player._Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player._Consumables[potionIndex].ChangeStat.StatCategory;
			var baseStr = player._Strength;
			var baseMaxCarryWeight = player._MaxCarryWeight;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseStr + statAmount, player._Strength);
			Assert.AreEqual(baseMaxCarryWeight + statAmount * 2.5, player._MaxCarryWeight, 1);
			Assert.IsEmpty(player._Consumables);
			Assert.AreEqual(player._Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseStr, player._Strength);
			Assert.AreEqual(baseMaxCarryWeight, player._MaxCarryWeight);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void DexterityPotionUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> { new Consumable(1, Consumable.PotionType.Dexterity) }
			};
			var potionIndex = player._Consumables.FindIndex(
				f => f.PotionCategory == Consumable.PotionType.Dexterity);
			var input = new[] { "drink", "dexterity" };
			var potionName = InputHandler.ParseInput(input);
			Assert.AreEqual("dexterity", potionName);
			var statAmount = player._Consumables[potionIndex].ChangeStat.ChangeAmount;
			var statType = player._Consumables[potionIndex].ChangeStat.StatCategory;
			var baseDex = player._Dexterity;
			var baseDodgeChance = player._DodgeChance;
			player.DrinkPotion(InputHandler.ParseInput(input));
			var drankStatString = "You drank a potion and increased " + statType + " by " + statAmount + ".";
			Assert.AreEqual(drankStatString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(baseDex + statAmount, player._Dexterity);
			Assert.AreEqual(baseDodgeChance + statAmount * 1.5, player._DodgeChance);
			Assert.IsEmpty(player._Consumables);
			Assert.AreEqual(player._Effects[0].EffectGroup, Effect.EffectType.ChangeStat);
			for (var i = 1; i < 601; i++)
			{
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseDex, player._Dexterity);
			Assert.AreEqual(baseDodgeChance, player._DodgeChance);
			player.DrinkPotion(InputHandler.ParseInput(input));
			Assert.AreEqual("What potion did you want to drink?", OutputHandler.Display.Output[1][2]);
		}
		[Test]
		public void ArmorUpgradeKitUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> {new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Armor,
					ChangeArmor.KitType.Cloth)}
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var armorIndex = player._Inventory.FindIndex(f => f._Name.Contains("cloth"));
			var armor = player._Inventory[armorIndex] as Armor;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var armorName = textInfo.ToTitleCase(player._Inventory[armorIndex]._Name);
			var kitAmount = player._Consumables[0].ChangeArmor.ChangeAmount;
			var kitName = player._Consumables[0]._Name;
			var input = new[] { "enhance", "doesn't exist", kitName };
			var armorAmount = armor.ArmorRating;
			GearHandler.UseArmorKit(player, input);
			const string upgradeFail = "What armor did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputHandler.Display.Output[0][2]);
			Assert.IsNotEmpty(player._Consumables);
			input = new[] { "enhance", armorName, "doesn't exist" };
			GearHandler.UseArmorKit(player, input);
			const string kitFail = "What armor kit did you want to use?";
			Assert.AreEqual(kitFail, OutputHandler.Display.Output[1][2]);
			Assert.IsNotEmpty(player._Consumables);
			input = new[] { "enhance", armorName, kitName };
			GearHandler.UseArmorKit(player, input);
			var upgradeSuccess = "You upgraded " + armorName + " with an armor kit.";
			Assert.AreEqual(upgradeSuccess, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(armorAmount + kitAmount, armor.ArmorRating);
			Assert.IsEmpty(player._Consumables);
			player._Consumables.Add(new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Armor,
				ChangeArmor.KitType.Leather));
			kitName = player._Consumables[0]._Name;
			input = new[] { "enhance", armorName, kitName };
			GearHandler.UseArmorKit(player, input);
			var enhanceFail = "You can't upgrade " + armorName + " with that!";
			Assert.IsNotEmpty(player._Consumables);
			Assert.AreEqual(enhanceFail
			, OutputHandler.Display.Output[3][2]);
			player._Consumables[0] = new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Weapon,
				ChangeArmor.KitType.Cloth);
			kitName = player._Consumables[0]._Name;
			input = new[] { "enhance", armorName, kitName };
			GearHandler.UseArmorKit(player, input);
			Assert.IsNotEmpty(player._Consumables);
			Assert.AreEqual(enhanceFail
				, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void WeaponUpgradeKitUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_Consumables = new List<Consumable> {new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Weapon,
					ChangeWeapon.KitType.Grindstone)}
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var weaponIndex = player._Inventory.FindIndex(f => f._Name.Contains("dagger"));
			var weapon = player._Inventory[weaponIndex] as Weapon;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var weaponName = textInfo.ToTitleCase(player._Inventory[weaponIndex]._Name);
			var kitAmount = player._Consumables[0].ChangeWeapon.ChangeAmount;
			var kitName = player._Consumables[0]._Name;
			var input = new[] { "enhance", "doesn't exist", kitName };
			var weaponAmount = weapon.RegDamage;
			GearHandler.UseWeaponKit(player, input);
			const string upgradeFail = "What weapon did you want to upgrade?";
			Assert.AreEqual(upgradeFail, OutputHandler.Display.Output[0][2]);
			Assert.IsNotEmpty(player._Consumables);
			input = new[] { "enhance", weaponName, "doesn't exist" };
			GearHandler.UseWeaponKit(player, input);
			const string kitFail = "What weapon kit did you want to use?";
			Assert.AreEqual(kitFail, OutputHandler.Display.Output[1][2]);
			Assert.IsNotEmpty(player._Consumables);
			input = new[] { "enhance", weaponName, kitName };
			GearHandler.UseWeaponKit(player, input);
			var upgradeSuccess = "You upgraded " + weaponName + " with a weapon kit.";
			Assert.AreEqual(upgradeSuccess, OutputHandler.Display.Output[2][2]);
			Assert.AreEqual(weaponAmount + kitAmount, weapon.RegDamage);
			Assert.IsEmpty(player._Consumables);
			player._Consumables.Add(new Consumable(Consumable.KitLevel.Light, Consumable.KitType.Weapon,
				ChangeWeapon.KitType.Bowstring));
			kitName = player._Consumables[0]._Name;
			input = new[] { "enhance", weaponName, kitName };
			GearHandler.UseWeaponKit(player, input);
			var enhanceFail = "You can't upgrade " + weaponName + " with that!";
			Assert.IsNotEmpty(player._Consumables);
			Assert.AreEqual(enhanceFail
			, OutputHandler.Display.Output[3][2]);
			player._Inventory.Add(new Weapon(1, Weapon.WeaponType.Bow));
			weaponIndex = player._Inventory.FindIndex(f => f._Name.Contains("bow"));
			weapon = player._Inventory[weaponIndex] as Weapon;
			weapon.Equipped = true;
			weaponName = textInfo.ToTitleCase(player._Inventory[weaponIndex]._Name);
			input = new[] { "enhance", weaponName, kitName };
			GearHandler.UseWeaponKit(player, input);
			upgradeSuccess = "You upgraded " + weaponName + " with a weapon kit.";
			Assert.IsEmpty(player._Consumables);
			Assert.AreEqual(upgradeSuccess, OutputHandler.Display.Output[4][2]);
		}
	}
}