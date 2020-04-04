using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests {
	public class SpellUnitTests {
		[Test]
		public void FireballSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) 
			{MaxManaPoints = 100, ManaPoints = 100, InCombat = true};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 50, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var inputInfo = new[] {"spell", "fireball"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Fireball);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Fireball", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 35", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time will burn for 3 rounds.",
				OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			var input = new [] {"cast", "fireball"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(25, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].EffectMaxRound);
			Assert.AreEqual("You hit the " + monster.Name + " for " + 
			                player.Spellbook[spellIndex].Offensive.Amount + " fire damage.", 
				OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("The " + monster.Name + " bursts into flame!", 
				OutputHandler.Display.Output[1][2]);
			for (var i = 2; i < 5; i++) {
				monster.Effects[0].OnFireRound(monster);
				Assert.AreEqual(
					"The " + monster.Name + " burns for " + monster.Effects[0].EffectAmountOverTime + " fire damage.",
					OutputHandler.Display.Output[i][2]);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(10, monster.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var inputInfo = new[] {"spell", "frostbolt"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Frostbolt);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Frostbolt", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Frost damage will freeze opponent for 2 rounds.",
				OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			var input = new [] {"cast", "frostbolt"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player.PlayerWeapon.Durability = 100;
			var baseDamage = (double) player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].EffectCurRound);
			Assert.AreEqual(2, monster.Effects[0].EffectMaxRound);
			var attackString = "You hit the " + monster.Name + " for " + player.Spellbook[spellIndex].Offensive.Amount +
			                   " frost damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var frozenString = "The " + monster.Name +
			                   " is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[1][2]);
			var monsterHitPointsBefore = monster.HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster.Effects[0].EffectMultiplier;
			for (var i = 2; i < 4; i++) {
				monster.Effects[0].FrozenRound(monster);
				Assert.AreEqual(i, monster.Effects[0].EffectCurRound);
				Assert.AreEqual(frozenString, OutputHandler.Display.Output[i][2]);
				player.PlayerWeapon.Durability = 100;
				var frozenDamage = (double) player.PhysicalAttack(monster);
				monster.HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHandler.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster.Effects.Any());
			var finalBaseDamageWithMod = (int) (totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int) totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int) totalFrozenDamage);
		}
		[Test]
		public void LightningSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var inputInfo = new[] {"spell", "lightning"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Lightning);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Lightning", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 35", OutputHandler.Display.Output[3][2]);
			var input = new [] {"cast", "lightning"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("lightning", spellName);
			player.PlayerWeapon.Durability = 100;
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			var arcaneSpellDamage = player.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - arcaneSpellDamage);
			var attackSuccessString = "You hit the " + monster.Name + " for " + arcaneSpellDamage + " arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void HealSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) 
				{MaxManaPoints = 100, ManaPoints = 100, MaxHitPoints = 100, HitPoints = 50};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] {"spell", "heal"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Heal);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Heal", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 50", OutputHandler.Display.Output[3][2]);
			var input = new [] {"cast", "heal"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("heal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			var healString = "You heal yourself for " + player.Spellbook[spellIndex].Healing.HealAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[4][2]);
			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
		}
		[Test]
		public void RejuvenateSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {
				MaxManaPoints = 100, ManaPoints = 100, MaxHitPoints = 100, HitPoints = 50};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] {"spell", "rejuvenate"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Rejuvenate);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Rejuvenate", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 20", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 10", OutputHandler.Display.Output[4][2]);
			var healInfoString = "Heal over time will restore health for " + 
			                     player.Spellbook[spellIndex].Healing.HealMaxRounds + " rounds.";
			Assert.AreEqual(healInfoString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			var input = new [] {"cast", "rejuvenate"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("rejuvenate", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(70, player.HitPoints);
			var healString = "You heal yourself for " + player.Spellbook[spellIndex].Healing.HealAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player.Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++) {
				player.Effects[0].HealingRound(player);
				var healAmtString = "You have been healed for " + player.Effects[0].EffectAmountOverTime + " health."; 
				Assert.AreEqual(i, player.Effects[0].EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 1][2]);
				Assert.AreEqual(70 + (i - 1) * player.Effects[0].EffectAmountOverTime, player.HitPoints);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void DiamondskinSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] {"spell", "diamondskin"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Diamondskin);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Diamondskin", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Augment Armor Amount: 25", OutputHandler.Display.Output[3][2]);
			var augmentInfoString = "Armor will be augmented for " + 
			                        player.Spellbook[spellIndex].ChangeAmount.ChangeMaxRound + " rounds.";
			Assert.AreEqual(augmentInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"cast", "diamondskin"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("diamondskin", spellName);
			var baseArmor = GearHandler.CheckArmorRating(player);
			player.InCombat = true;
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			var augmentString = "You augmented your armor by " + player.Spellbook[spellIndex].ChangeAmount.Amount + " with " 
			                    + player.Spellbook[spellIndex].Name + ".";
			Assert.AreEqual(augmentString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(Effect.EffectType.ChangeArmor, player.Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++) {
				var augmentedArmor = GearHandler.CheckArmorRating(player);
				Assert.AreEqual(baseArmor + 25, augmentedArmor);
				player.Effects[0].ChangeArmorRound();
				var augmentRoundString = "Your armor is increased by " + player.Effects[0].EffectAmountOverTime + ".";
				Assert.AreEqual(augmentRoundString, OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void TownPortalSpellUnitTest() {
			/* Town Portal should change location of player to where portal is set to, which is 0, 7, 0, town entrance */
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			RoomHandler.Rooms = new RoomBuilder(100, 5, 0, 4, 0).RetrieveSpawnRooms();
			player.Spellbook.Add(new PlayerSpell(
				"town portal", 100, 1, PlayerSpell.SpellType.TownPortal, 2));
			var inputInfo = new[] {"spell", "town", "portal"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.TownPortal);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Town Portal", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHandler.Display.Output[2][2]);
			const string portalString = "This spell will create a portal and return you to town.";
			Assert.AreEqual(portalString, OutputHandler.Display.Output[3][2]);
			var newCoord = new Coordinate(-2, 6, 0);
			player.PlayerLocation = newCoord;
			Assert.AreEqual(-2, player.PlayerLocation.X);
			Assert.AreEqual(6, player.PlayerLocation.Y);
			Assert.AreEqual(0, player.PlayerLocation.Z);
			var input = new [] {"cast", "town", "portal"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("town portal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(0, player.PlayerLocation.X);
			Assert.AreEqual(7, player.PlayerLocation.Y);
			Assert.AreEqual(0, player.PlayerLocation.Z);
			Assert.AreEqual("You open a portal and step through it.", OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void ReflectDamageSpellUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			var monster = new Monster(3, Monster.MonsterType.Zombie) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 4));
			var inputInfo = new[] {"spell", "reflect"};
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Reflect);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Reflect", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Reflect Damage Amount: 25", OutputHandler.Display.Output[3][2]);
			var reflectInfoString = "Damage up to " + player.Spellbook[spellIndex].ChangeAmount.Amount + 
			                        " will be reflected for " +
			                        player.Spellbook[spellIndex].ChangeAmount.ChangeMaxRound + " rounds.";
			Assert.AreEqual(reflectInfoString, OutputHandler.Display.Output[4][2]);
			var input = new [] {"cast", "reflect"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("reflect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual("You create a shield around you that will reflect damage.", 
				OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(Effect.EffectType.ReflectDamage, player.Effects[0].EffectGroup);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++) {
				var attackDamageM = monster.MonsterWeapon.Attack();
				var index = player.Effects.FindIndex(
					f => f.EffectGroup == Effect.EffectType.ReflectDamage);
				var reflectAmount = player.Effects[index].EffectAmountOverTime < attackDamageM ? 
					player.Effects[index].EffectAmountOverTime : attackDamageM;
				Assert.AreEqual(true, reflectAmount <= player.Effects[index].EffectAmountOverTime);
				monster.HitPoints -= reflectAmount;
				Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - reflectAmount * (i - 1));
				player.Effects[index].ReflectDamageRound(reflectAmount);
				Assert.AreEqual(
					"You reflected " + reflectAmount + " damage back at your opponent!", 
					OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void ArcaneIntellectSpellUnitTest() {
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 150, ManaPoints = 150};
			player.Spellbook.Add(new PlayerSpell(
				"arcane intellect", 150, 1, PlayerSpell.SpellType.ArcaneIntellect, 6));
			var infoInput = new [] {"spell", "arcane", "intellect"};
			PlayerHandler.SpellInfo(player, infoInput);
			Assert.AreEqual("Arcane Intellect", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Arcane Intellect Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Intelligence is increased by 15 for 10 minutes.", 
				OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			var baseInt = player.Intelligence;
			var baseMana = player.ManaPoints;
			var baseMaxMana = player.MaxManaPoints;
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.ArcaneIntellect);
			var input = new [] {"cast", "arcane", "intellect"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("arcane intellect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(baseMaxMana- player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(player.Intelligence, baseInt + player.Spellbook[spellIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseMana - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(
				baseMaxMana + player.Spellbook[spellIndex].ChangeAmount.Amount * 10, player.MaxManaPoints);
			Assert.AreEqual("You cast Arcane Intellect on yourself.", OutputHandler.Display.Output[0][2]);
			for (var i = 0; i < 10; i++) {
				player.Effects[0].ChangeStatRound();
			}
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Arcane Intellect", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 590; i++) {
				player.Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseInt, player.Intelligence);
			Assert.AreEqual(0, player.ManaPoints);
			Assert.AreEqual(baseMaxMana, player.MaxManaPoints);
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
		}
		[Test]
		public void FrostNovaSpellUnitTest() {
			var player = new Player("test", Player.PlayerClassType.Mage) {MaxManaPoints = 100, ManaPoints = 100};
			player.Spellbook.Add(new PlayerSpell(
				"frost nova", 40, 1, PlayerSpell.SpellType.FrostNova, 8));
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon) 
				{HitPoints = 100, MaxHitPoints = 100};
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster.MonsterItems.Where(item => item.Equipped)) {
				item.Equipped = false;
			}
			var spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.FrostNova);
			var infoInput = new[] {"spell", "frost", "nova"};
			PlayerHandler.SpellInfo(player, infoInput);
			Assert.AreEqual("Frost Nova", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Frost damage will freeze opponent for " +
			                player.Spellbook[spellIndex].Offensive.AmountMaxRounds + " rounds.",
				OutputHandler.Display.Output[4][2]);
			Assert.AreEqual( "Frozen opponents take 1.5x physical, arcane and frost damage.", 
				OutputHandler.Display.Output[5][2]);
			Assert.AreEqual("Opponent will be stunned for " + 
			                player.Spellbook[spellIndex].Offensive.AmountMaxRounds + " rounds.", 
				OutputHandler.Display.Output[6][2]);
			var input = new [] {"cast", "frost", "nova"};
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("frost nova", spellName);
			player.PlayerWeapon.Durability = 100;
			var baseDamage = (double) player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			var attackSuccessString = "You hit the " + monster.Name + " for " + 
			                          player.Spellbook[spellIndex].Offensive.Amount + " frost damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[7][2]);
			var frozenString = "The " + monster.Name +
			                   " is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[8][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(player.ManaPoints, player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost);
			var frostIndex = monster.Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.Frozen);
			var stunIndex = monster.Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[frostIndex].EffectCurRound);
			Assert.AreEqual(1, monster.Effects[stunIndex].EffectCurRound);
			Assert.AreEqual(2, monster.Effects[frostIndex].EffectMaxRound);
			Assert.AreEqual(2, monster.Effects[stunIndex].EffectMaxRound);
			var monsterHitPointsBefore = monster.HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster.Effects[frostIndex].EffectMultiplier;
			for (var i = 2; i < 4; i++) {
				OutputHandler.Display.ClearUserOutput();
				monster.Effects[stunIndex].StunnedRound(monster);
				var stunnedRoundString = "The " + monster.Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedRoundString, OutputHandler.Display.Output[0][2]);
				Assert.AreEqual(true, monster.IsStunned);
				Assert.AreEqual(i, monster.Effects[stunIndex].EffectCurRound);
				player.PlayerWeapon.Durability = 100;
				var frozenDamage = (double) player.PhysicalAttack(monster);
				Assert.AreEqual(i, monster.Effects[frostIndex].EffectCurRound);
				var frozenRoundString = "The " + monster.Name +
				                        " is frozen. Physical, frost and arcane damage to it will be double!";
				Assert.AreEqual(frozenRoundString, OutputHandler.Display.Output[1][2]);
				monster.HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHandler.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(false, monster.IsStunned);
			var finalBaseDamageWithMod = (int) (totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int) totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int) totalFrozenDamage);
		}
	}
}