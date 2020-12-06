using System.Linq;
using System.Threading;
using DungeonGame;
using NUnit.Framework;

namespace DungeonGameTests
{
	public class SpellUnitTests
	{
		[Test]
		public void FireballSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _MaxManaPoints = 100, _ManaPoints = 100, _InCombat = true };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 50, _MaxHitPoints = 100, _FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player._PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			var inputInfo = new[] { "spell", "fireball" };
			var spellIndex = player._Spellbook.FindIndex(
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
			var input = new[] { "cast", "fireball" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(25, monster._HitPoints);
			Assert.AreEqual(3, monster._Effects[0].EffectMaxRound);
			Assert.AreEqual("You hit the " + monster._Name + " for " +
							player._Spellbook[spellIndex].Offensive.Amount + " fire damage.",
				OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("The " + monster._Name + " bursts into flame!",
				OutputHandler.Display.Output[1][2]);
			for (var i = 2; i < 5; i++)
			{
				monster._Effects[0].OnFireRound(monster);
				Assert.AreEqual(
					"The " + monster._Name + " burns for " + monster._Effects[0].EffectAmountOverTime + " fire damage.",
					OutputHandler.Display.Output[i][2]);
				Assert.AreEqual(i, monster._Effects[0].EffectCurRound);
				GameHandler.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(10, monster._HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var inputInfo = new[] { "spell", "frostbolt" };
			var spellIndex = player._Spellbook.FindIndex(
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
			var input = new[] { "cast", "frostbolt" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player._PlayerWeapon.Durability = 100;
			var baseDamage = (double)player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(85, monster._HitPoints);
			Assert.AreEqual(1, monster._Effects[0].EffectCurRound);
			Assert.AreEqual(2, monster._Effects[0].EffectMaxRound);
			var attackString = "You hit the " + monster._Name + " for " + player._Spellbook[spellIndex].Offensive.Amount +
							   " frost damage.";
			Assert.AreEqual(attackString, OutputHandler.Display.Output[0][2]);
			var frozenString = "The " + monster._Name +
							   " is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[1][2]);
			var monsterHitPointsBefore = monster._HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster._Effects[0].EffectMultiplier;
			for (var i = 2; i < 4; i++)
			{
				monster._Effects[0].FrozenRound(monster);
				Assert.AreEqual(i, monster._Effects[0].EffectCurRound);
				Assert.AreEqual(frozenString, OutputHandler.Display.Output[i][2]);
				player._PlayerWeapon.Durability = 100;
				var frozenDamage = (double)player.PhysicalAttack(monster);
				monster._HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHandler.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			var finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster._HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
		[Test]
		public void LightningSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _ArcaneResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var inputInfo = new[] { "spell", "lightning" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Lightning);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Lightning", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 35", OutputHandler.Display.Output[3][2]);
			var input = new[] { "cast", "lightning" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("lightning", spellName);
			player._PlayerWeapon.Durability = 100;
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			var arcaneSpellDamage = player._Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - arcaneSpellDamage);
			var attackSuccessString = "You hit the " + monster._Name + " for " + arcaneSpellDamage + " arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void HealSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{ _MaxManaPoints = 100, _ManaPoints = 100, _MaxHitPoints = 100, _HitPoints = 50 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] { "spell", "heal" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Heal);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Heal", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 50", OutputHandler.Display.Output[3][2]);
			var input = new[] { "cast", "heal" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("heal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			var healString = "You heal yourself for " + player._Spellbook[spellIndex].Healing.HealAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[4][2]);
			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
		}
		[Test]
		public void RejuvenateSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage)
			{
				_MaxManaPoints = 100,
				_ManaPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 50
			};
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] { "spell", "rejuvenate" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Rejuvenate);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Rejuvenate", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 20", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 10", OutputHandler.Display.Output[4][2]);
			var healInfoString = "Heal over time will restore health for " +
								 player._Spellbook[spellIndex].Healing.HealMaxRounds + " rounds.";
			Assert.AreEqual(healInfoString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			var input = new[] { "cast", "rejuvenate" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("rejuvenate", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(70, player._HitPoints);
			var healString = "You heal yourself for " + player._Spellbook[spellIndex].Healing.HealAmount + " health.";
			Assert.AreEqual(healString, OutputHandler.Display.Output[0][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player._Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++)
			{
				player._Effects[0].HealingRound(player);
				var healAmtString = "You have been healed for " + player._Effects[0].EffectAmountOverTime + " health.";
				Assert.AreEqual(i, player._Effects[0].EffectCurRound);
				Assert.AreEqual(healAmtString, OutputHandler.Display.Output[i - 1][2]);
				Assert.AreEqual(70 + (i - 1) * player._Effects[0].EffectAmountOverTime, player._HitPoints);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void DiamondskinSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var inputInfo = new[] { "spell", "diamondskin" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Diamondskin);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Diamondskin", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Augment Armor Amount: 25", OutputHandler.Display.Output[3][2]);
			var augmentInfoString = "Armor will be augmented for " +
									player._Spellbook[spellIndex].ChangeAmount.ChangeMaxRound + " rounds.";
			Assert.AreEqual(augmentInfoString, OutputHandler.Display.Output[4][2]);
			var input = new[] { "cast", "diamondskin" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("diamondskin", spellName);
			var baseArmor = GearHandler.CheckArmorRating(player);
			player._InCombat = true;
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			var augmentString = "You augmented your armor by " + player._Spellbook[spellIndex].ChangeAmount.Amount + " with "
								+ player._Spellbook[spellIndex].Name + ".";
			Assert.AreEqual(augmentString, OutputHandler.Display.Output[5][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(true, player._Effects.Any());
			Assert.AreEqual(Effect.EffectType.ChangeArmor, player._Effects[0].EffectGroup);
			for (var i = 2; i < 5; i++)
			{
				var augmentedArmor = GearHandler.CheckArmorRating(player);
				Assert.AreEqual(baseArmor + 25, augmentedArmor);
				player._Effects[0].ChangeArmorRound();
				var augmentRoundString = "Your armor is increased by " + player._Effects[0].EffectAmountOverTime + ".";
				Assert.AreEqual(augmentRoundString, OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void TownPortalSpellUnitTest()
		{
			/* Town Portal should change location of player to where portal is set to, which is 0, 7, 0, town entrance */
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			RoomHandler.Rooms = new RoomBuilder(100, 5, 0, 4, 0).RetrieveSpawnRooms();
			player._Spellbook.Add(new PlayerSpell(
				"town portal", 100, 1, PlayerSpell.SpellType.TownPortal, 2));
			var inputInfo = new[] { "spell", "town", "portal" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.TownPortal);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Town Portal", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHandler.Display.Output[2][2]);
			const string portalString = "This spell will create a portal and return you to town.";
			Assert.AreEqual(portalString, OutputHandler.Display.Output[3][2]);
			var newCoord = new Coordinate(-2, 6, 0);
			player._PlayerLocation = newCoord;
			Assert.AreEqual(-2, player._PlayerLocation.X);
			Assert.AreEqual(6, player._PlayerLocation.Y);
			Assert.AreEqual(0, player._PlayerLocation.Z);
			var input = new[] { "cast", "town", "portal" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("town portal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(0, player._PlayerLocation.X);
			Assert.AreEqual(7, player._PlayerLocation.Y);
			Assert.AreEqual(0, player._PlayerLocation.Z);
			Assert.AreEqual("You open a portal and step through it.", OutputHandler.Display.Output[4][2]);
		}
		[Test]
		public void ReflectDamageSpellUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			var monster = new Monster(3, Monster.MonsterType.Zombie)
			{ _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 4));
			var inputInfo = new[] { "spell", "reflect" };
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.Reflect);
			PlayerHandler.SpellInfo(player, inputInfo);
			Assert.AreEqual("Reflect", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Reflect Damage Amount: 25", OutputHandler.Display.Output[3][2]);
			var reflectInfoString = "Damage up to " + player._Spellbook[spellIndex].ChangeAmount.Amount +
									" will be reflected for " +
									player._Spellbook[spellIndex].ChangeAmount.ChangeMaxRound + " rounds.";
			Assert.AreEqual(reflectInfoString, OutputHandler.Display.Output[4][2]);
			var input = new[] { "cast", "reflect" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("reflect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual("You create a shield around you that will reflect damage.",
				OutputHandler.Display.Output[5][2]);
			Assert.AreEqual(true, player._Effects.Any());
			Assert.AreEqual(Effect.EffectType.ReflectDamage, player._Effects[0].EffectGroup);
			OutputHandler.Display.ClearUserOutput();
			for (var i = 2; i < 5; i++)
			{
				var attackDamageM = monster._MonsterWeapon.Attack();
				var index = player._Effects.FindIndex(
					f => f.EffectGroup == Effect.EffectType.ReflectDamage);
				var reflectAmount = player._Effects[index].EffectAmountOverTime < attackDamageM ?
					player._Effects[index].EffectAmountOverTime : attackDamageM;
				Assert.AreEqual(true, reflectAmount <= player._Effects[index].EffectAmountOverTime);
				monster._HitPoints -= reflectAmount;
				Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - reflectAmount * (i - 1));
				player._Effects[index].ReflectDamageRound(reflectAmount);
				Assert.AreEqual(
					"You reflected " + reflectAmount + " damage back at your opponent!",
					OutputHandler.Display.Output[i - 2][2]);
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void ArcaneIntellectSpellUnitTest()
		{
			OutputHandler.Display.ClearUserOutput();
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 150, _ManaPoints = 150 };
			player._Spellbook.Add(new PlayerSpell(
				"arcane intellect", 150, 1, PlayerSpell.SpellType.ArcaneIntellect, 6));
			var infoInput = new[] { "spell", "arcane", "intellect" };
			PlayerHandler.SpellInfo(player, infoInput);
			Assert.AreEqual("Arcane Intellect", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 150", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Arcane Intellect Amount: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("_Intelligence is increased by 15 for 10 minutes.",
				OutputHandler.Display.Output[4][2]);
			OutputHandler.Display.ClearUserOutput();
			var baseInt = player._Intelligence;
			var baseMana = player._ManaPoints;
			var baseMaxMana = player._MaxManaPoints;
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.ArcaneIntellect);
			var input = new[] { "cast", "arcane", "intellect" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("arcane intellect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(baseMaxMana - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(player._Intelligence, baseInt + player._Spellbook[spellIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseMana - player._Spellbook[spellIndex].ManaCost, player._ManaPoints);
			Assert.AreEqual(
				baseMaxMana + player._Spellbook[spellIndex].ChangeAmount.Amount * 10, player._MaxManaPoints);
			Assert.AreEqual("You cast Arcane Intellect on yourself.", OutputHandler.Display.Output[0][2]);
			for (var i = 0; i < 10; i++)
			{
				player._Effects[0].ChangeStatRound();
			}
			var defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Arcane Intellect", defaultEffectOutput.Output[1][2]);
			for (var i = 0; i < 590; i++)
			{
				player._Effects[0].ChangeStatRound();
			}
			GameHandler.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseInt, player._Intelligence);
			Assert.AreEqual(0, player._ManaPoints);
			Assert.AreEqual(baseMaxMana, player._MaxManaPoints);
			defaultEffectOutput = OutputHandler.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
		}
		[Test]
		public void FrostNovaSpellUnitTest()
		{
			var player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			player._Spellbook.Add(new PlayerSpell(
				"frost nova", 40, 1, PlayerSpell.SpellType.FrostNova, 8));
			GearHandler.EquipInitialGear(player);
			OutputHandler.Display.ClearUserOutput();
			var monster = new Monster(3, Monster.MonsterType.Demon)
			{ _HitPoints = 100, _MaxHitPoints = 100, _FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (var item in monster._MonsterItems.Where(item => item.Equipped))
			{
				item.Equipped = false;
			}
			var spellIndex = player._Spellbook.FindIndex(
				f => f.SpellCategory == PlayerSpell.SpellType.FrostNova);
			var infoInput = new[] { "spell", "frost", "nova" };
			PlayerHandler.SpellInfo(player, infoInput);
			Assert.AreEqual("Frost Nova", OutputHandler.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHandler.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 40", OutputHandler.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHandler.Display.Output[3][2]);
			Assert.AreEqual("Frost damage will freeze opponent for " +
							player._Spellbook[spellIndex].Offensive.AmountMaxRounds + " rounds.",
				OutputHandler.Display.Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputHandler.Display.Output[5][2]);
			Assert.AreEqual("Opponent will be stunned for " +
							player._Spellbook[spellIndex].Offensive.AmountMaxRounds + " rounds.",
				OutputHandler.Display.Output[6][2]);
			var input = new[] { "cast", "frost", "nova" };
			var spellName = InputHandler.ParseInput(input);
			Assert.AreEqual("frost nova", spellName);
			player._PlayerWeapon.Durability = 100;
			var baseDamage = (double)player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			var attackSuccessString = "You hit the " + monster._Name + " for " +
									  player._Spellbook[spellIndex].Offensive.Amount + " frost damage.";
			Assert.AreEqual(attackSuccessString, OutputHandler.Display.Output[7][2]);
			var frozenString = "The " + monster._Name +
							   " is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputHandler.Display.Output[8][2]);
			OutputHandler.Display.ClearUserOutput();
			Assert.AreEqual(player._ManaPoints, player._MaxManaPoints - player._Spellbook[spellIndex].ManaCost);
			var frostIndex = monster._Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.Frozen);
			var stunIndex = monster._Effects.FindIndex(
				f => f.EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(85, monster._HitPoints);
			Assert.AreEqual(1, monster._Effects[frostIndex].EffectCurRound);
			Assert.AreEqual(1, monster._Effects[stunIndex].EffectCurRound);
			Assert.AreEqual(2, monster._Effects[frostIndex].EffectMaxRound);
			Assert.AreEqual(2, monster._Effects[stunIndex].EffectMaxRound);
			var monsterHitPointsBefore = monster._HitPoints;
			var totalBaseDamage = 0.0;
			var totalFrozenDamage = 0.0;
			var multiplier = monster._Effects[frostIndex].EffectMultiplier;
			for (var i = 2; i < 4; i++)
			{
				OutputHandler.Display.ClearUserOutput();
				monster._Effects[stunIndex].StunnedRound(monster);
				var stunnedRoundString = "The " + monster._Name + " is stunned and cannot attack.";
				Assert.AreEqual(stunnedRoundString, OutputHandler.Display.Output[0][2]);
				Assert.AreEqual(true, monster._IsStunned);
				Assert.AreEqual(i, monster._Effects[stunIndex].EffectCurRound);
				player._PlayerWeapon.Durability = 100;
				var frozenDamage = (double)player.PhysicalAttack(monster);
				Assert.AreEqual(i, monster._Effects[frostIndex].EffectCurRound);
				var frozenRoundString = "The " + monster._Name +
										" is frozen. Physical, frost and arcane damage to it will be double!";
				Assert.AreEqual(frozenRoundString, OutputHandler.Display.Output[1][2]);
				monster._HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHandler.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(false, monster._IsStunned);
			var finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			var finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster._HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
	}
}