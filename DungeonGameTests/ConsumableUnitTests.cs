using System.Collections.Generic;
using System.Linq.Expressions;
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
			Assert.AreEqual("You don't have any health potions!", OutputHandler.Display.Output[1][2]);
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
			Assert.AreEqual("You don't have any mana potions!", OutputHandler.Display.Output[1][2]);
		}
	}
}