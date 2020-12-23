using DungeonGame;
using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Items.Equipment;
using DungeonGame.Monsters;
using DungeonGame.Players;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace DungeonGameTests {
	public class SpellUnitTests {
		[Test]
		public void FireballSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100, _InCombat = true };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon) { _HitPoints = 50, _MaxHitPoints = 100, _FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player._PlayerWeapon._CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			string[] inputInfo = new[] { "spell", "fireball" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Fireball);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Fireball", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 35", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputController.Display._Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputController.Display._Output[4][2]);
			Assert.AreEqual("Fire damage over time will burn for 3 rounds.",
				OutputController.Display._Output[5][2]);
			OutputController.Display.ClearUserOutput();
			string[] input = new[] { "cast", "fireball" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(25, monster._HitPoints);
			Assert.AreEqual(3, monster._Effects[0]._EffectMaxRound);
			Assert.AreEqual($"You hit the {monster._Name} for {player._Spellbook[spellIndex]._Offensive._Amount} fire damage.",
				OutputController.Display._Output[0][2]);
			Assert.AreEqual($"The {monster._Name} bursts into flame!",
				OutputController.Display._Output[1][2]);
			for (int i = 2; i < 5; i++) {
				monster._Effects[0].OnFireRound(monster);
				Assert.AreEqual(
					$"The {monster._Name} burns for {monster._Effects[0]._EffectAmountOverTime} fire damage.",
					OutputController.Display._Output[i][2]);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				GameController.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(10, monster._HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon) { _HitPoints = 100, _MaxHitPoints = 100, _FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem._Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			string[] inputInfo = new[] { "spell", "frostbolt" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Frostbolt);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Frostbolt", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputController.Display._Output[3][2]);
			Assert.AreEqual("Frost damage will freeze opponent for 2 rounds.",
				OutputController.Display._Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputController.Display._Output[5][2]);
			OutputController.Display.ClearUserOutput();
			string[] input = new[] { "cast", "frostbolt" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player._PlayerWeapon._Durability = 100;
			double baseDamage = player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(85, monster._HitPoints);
			Assert.AreEqual(1, monster._Effects[0]._EffectCurRound);
			Assert.AreEqual(2, monster._Effects[0]._EffectMaxRound);
			string attackString = $"You hit the {monster._Name} for {player._Spellbook[spellIndex]._Offensive._Amount} frost damage.";
			Assert.AreEqual(attackString, OutputController.Display._Output[0][2]);
			string frozenString = $"The {monster._Name} is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputController.Display._Output[1][2]);
			int monsterHitPointsBefore = monster._HitPoints;
			double totalBaseDamage = 0.0;
			double totalFrozenDamage = 0.0;
			double multiplier = monster._Effects[0]._EffectMultiplier;
			for (int i = 2; i < 4; i++) {
				monster._Effects[0].FrozenRound(monster);
				Assert.AreEqual(i, monster._Effects[0]._EffectCurRound);
				Assert.AreEqual(frozenString, OutputController.Display._Output[i][2]);
				player._PlayerWeapon._Durability = 100;
				double frozenDamage = player.PhysicalAttack(monster);
				monster._HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameController.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			int finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			int finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster._HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
		[Test]
		public void LightningSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon) { _HitPoints = 100, _MaxHitPoints = 100, _ArcaneResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem._Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			string[] inputInfo = new[] { "spell", "lightning" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Lightning);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Lightning", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Instant Damage: 35", OutputController.Display._Output[3][2]);
			string[] input = new[] { "cast", "lightning" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("lightning", spellName);
			player._PlayerWeapon._Durability = 100;
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			int arcaneSpellDamage = player._Spellbook[spellIndex]._Offensive._Amount;
			Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - arcaneSpellDamage);
			string attackSuccessString = $"You hit the {monster._Name} for {arcaneSpellDamage} arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display._Output[4][2]);
		}
		[Test]
		public void HealSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100, _MaxHitPoints = 100, _HitPoints = 50 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "heal" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Heal);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Heal", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Heal Amount: 50", OutputController.Display._Output[3][2]);
			string[] input = new[] { "cast", "heal" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("heal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			string healString = $"You heal yourself for {player._Spellbook[spellIndex]._Healing._HealAmount} health.";
			Assert.AreEqual(healString, OutputController.Display._Output[4][2]);
			Assert.AreEqual(player._MaxHitPoints, player._HitPoints);
		}
		[Test]
		public void RejuvenateSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) {
				_MaxManaPoints = 100,
				_ManaPoints = 100,
				_MaxHitPoints = 100,
				_HitPoints = 50
			};
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "rejuvenate" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Rejuvenate);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Rejuvenate", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Heal Amount: 20", OutputController.Display._Output[3][2]);
			Assert.AreEqual("Heal Over Time: 10", OutputController.Display._Output[4][2]);
			string healInfoString = $"Heal over time will restore health for {player._Spellbook[spellIndex]._Healing._HealMaxRounds} rounds.";
			Assert.AreEqual(healInfoString, OutputController.Display._Output[5][2]);
			OutputController.Display.ClearUserOutput();
			string[] input = new[] { "cast", "rejuvenate" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("rejuvenate", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(70, player._HitPoints);
			string healString = $"You heal yourself for {player._Spellbook[spellIndex]._Healing._HealAmount} health.";
			Assert.AreEqual(healString, OutputController.Display._Output[0][2]);
			Assert.AreEqual(Effect.EffectType.Healing, player._Effects[0]._EffectGroup);
			for (int i = 2; i < 5; i++) {
				player._Effects[0].HealingRound(player);
				string healAmtString = $"You have been healed for {player._Effects[0]._EffectAmountOverTime} health.";
				Assert.AreEqual(i, player._Effects[0]._EffectCurRound);
				Assert.AreEqual(healAmtString, OutputController.Display._Output[i - 1][2]);
				Assert.AreEqual(70 + ((i - 1) * player._Effects[0]._EffectAmountOverTime), player._HitPoints);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void DiamondskinSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "diamondskin" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Diamondskin);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Diamondskin", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Augment Armor Amount: 25", OutputController.Display._Output[3][2]);
			string augmentInfoString = $"Armor will be augmented for {player._Spellbook[spellIndex]._ChangeAmount._ChangeMaxRound} rounds.";
			Assert.AreEqual(augmentInfoString, OutputController.Display._Output[4][2]);
			string[] input = new[] { "cast", "diamondskin" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("diamondskin", spellName);
			int baseArmor = GearController.CheckArmorRating(player);
			player._InCombat = true;
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			string augmentString = $"You augmented your armor by {player._Spellbook[spellIndex]._ChangeAmount._Amount} with {player._Spellbook[spellIndex]._Name}.";
			Assert.AreEqual(augmentString, OutputController.Display._Output[5][2]);
			OutputController.Display.ClearUserOutput();
			Assert.AreEqual(true, player._Effects.Any());
			Assert.AreEqual(Effect.EffectType.ChangeArmor, player._Effects[0]._EffectGroup);
			for (int i = 2; i < 5; i++) {
				int augmentedArmor = GearController.CheckArmorRating(player);
				Assert.AreEqual(baseArmor + 25, augmentedArmor);
				player._Effects[0].ChangeArmorRound();
				string augmentRoundString = $"Your armor is increased by {player._Effects[0]._EffectAmountOverTime}.";
				Assert.AreEqual(augmentRoundString, OutputController.Display._Output[i - 2][2]);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void TownPortalSpellUnitTest() {
			/* Town Portal should change location of player to where portal is set to, which is 0, 7, 0, town entrance */
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			RoomController._Rooms = new RoomBuilder(100, 5, 0, 4, 0).RetrieveSpawnRooms();
			player._Spellbook.Add(new PlayerSpell(
				"town portal", 100, 1, PlayerSpell.SpellType.TownPortal, 2));
			string[] inputInfo = new[] { "spell", "town", "portal" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.TownPortal);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Town Portal", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputController.Display._Output[2][2]);
			const string portalString = "This spell will create a portal and return you to town.";
			Assert.AreEqual(portalString, OutputController.Display._Output[3][2]);
			Coordinate newCoord = new Coordinate(-2, 6, 0);
			player._PlayerLocation = newCoord;
			Assert.AreEqual(-2, player._PlayerLocation._X);
			Assert.AreEqual(6, player._PlayerLocation._Y);
			Assert.AreEqual(0, player._PlayerLocation._Z);
			string[] input = new[] { "cast", "town", "portal" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("town portal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(0, player._PlayerLocation._X);
			Assert.AreEqual(7, player._PlayerLocation._Y);
			Assert.AreEqual(0, player._PlayerLocation._Z);
			Assert.AreEqual("You open a portal and step through it.", OutputController.Display._Output[4][2]);
		}
		[Test]
		public void ReflectDamageSpellUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			Monster monster = new Monster(3, Monster.MonsterType.Zombie) { _HitPoints = 100, _MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem._Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			player._Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, PlayerSpell.SpellType.Reflect, 4));
			string[] inputInfo = new[] { "spell", "reflect" };
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.Reflect);
			PlayerController.SpellInfo(player, inputInfo);
			Assert.AreEqual("Reflect", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Reflect Damage Amount: 25", OutputController.Display._Output[3][2]);
			string reflectInfoString = $"Damage up to {player._Spellbook[spellIndex]._ChangeAmount._Amount} will be reflected for " +
				$"{player._Spellbook[spellIndex]._ChangeAmount._ChangeMaxRound} rounds.";
			Assert.AreEqual(reflectInfoString, OutputController.Display._Output[4][2]);
			string[] input = new[] { "cast", "reflect" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("reflect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual("You create a shield around you that will reflect damage.",
				OutputController.Display._Output[5][2]);
			Assert.AreEqual(true, player._Effects.Any());
			Assert.AreEqual(Effect.EffectType.ReflectDamage, player._Effects[0]._EffectGroup);
			OutputController.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				int attackDamageM = monster._MonsterWeapon.Attack();
				int index = player._Effects.FindIndex(
					f => f._EffectGroup == Effect.EffectType.ReflectDamage);
				int reflectAmount = player._Effects[index]._EffectAmountOverTime < attackDamageM ?
					player._Effects[index]._EffectAmountOverTime : attackDamageM;
				Assert.AreEqual(true, reflectAmount <= player._Effects[index]._EffectAmountOverTime);
				monster._HitPoints -= reflectAmount;
				Assert.AreEqual(monster._HitPoints, monster._MaxHitPoints - reflectAmount * (i - 1));
				player._Effects[index].ReflectDamageRound(reflectAmount);
				Assert.AreEqual(
					$"You reflected {reflectAmount} damage back at your opponent!",
					OutputController.Display._Output[i - 2][2]);
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
		}
		[Test]
		public void ArcaneIntellectSpellUnitTest() {
			OutputController.Display.ClearUserOutput();
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 150, _ManaPoints = 150 };
			player._Spellbook.Add(new PlayerSpell(
				"arcane intellect", 150, 1, PlayerSpell.SpellType.ArcaneIntellect, 6));
			string[] infoInput = new[] { "spell", "arcane", "intellect" };
			PlayerController.SpellInfo(player, infoInput);
			Assert.AreEqual("Arcane Intellect", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 150", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Arcane Intellect Amount: 15", OutputController.Display._Output[3][2]);
			Assert.AreEqual("_Intelligence is increased by 15 for 10 minutes.",
				OutputController.Display._Output[4][2]);
			OutputController.Display.ClearUserOutput();
			int baseInt = player._Intelligence;
			int? baseMana = player._ManaPoints;
			int? baseMaxMana = player._MaxManaPoints;
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.ArcaneIntellect);
			string[] input = new[] { "cast", "arcane", "intellect" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("arcane intellect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(baseMaxMana - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(player._Intelligence, baseInt + player._Spellbook[spellIndex]._ChangeAmount._Amount);
			Assert.AreEqual(
				baseMana - player._Spellbook[spellIndex]._ManaCost, player._ManaPoints);
			Assert.AreEqual(
				baseMaxMana + player._Spellbook[spellIndex]._ChangeAmount._Amount * 10, player._MaxManaPoints);
			Assert.AreEqual("You cast Arcane Intellect on yourself.", OutputController.Display._Output[0][2]);
			for (int i = 0; i < 10; i++) {
				player._Effects[0].ChangeStatRound();
			}
			UserOutput defaultEffectOutput = OutputController.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput._Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput._Output[1][0]);
			Assert.AreEqual("(590 seconds) Arcane Intellect", defaultEffectOutput._Output[1][2]);
			for (int i = 0; i < 590; i++) {
				player._Effects[0].ChangeStatRound();
			}
			GameController.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player._Effects.Any());
			Assert.AreEqual(baseInt, player._Intelligence);
			Assert.AreEqual(0, player._ManaPoints);
			Assert.AreEqual(baseMaxMana, player._MaxManaPoints);
			defaultEffectOutput = OutputController.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput._Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput._Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput._Output[1][2]);
		}
		[Test]
		public void FrostNovaSpellUnitTest() {
			Player player = new Player("test", Player.PlayerClassType.Mage) { _MaxManaPoints = 100, _ManaPoints = 100 };
			player._Spellbook.Add(new PlayerSpell(
				"frost nova", 40, 1, PlayerSpell.SpellType.FrostNova, 8));
			GearController.EquipInitialGear(player);
			OutputController.Display.ClearUserOutput();
			Monster monster = new Monster(3, Monster.MonsterType.Demon) { _HitPoints = 100, _MaxHitPoints = 100, _FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster._MonsterItems.Where(item => item is IEquipment eItem && eItem._Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem._Equipped = false;
			}
			int spellIndex = player._Spellbook.FindIndex(
				f => f._SpellCategory == PlayerSpell.SpellType.FrostNova);
			string[] infoInput = new[] { "spell", "frost", "nova" };
			PlayerController.SpellInfo(player, infoInput);
			Assert.AreEqual("Frost Nova", OutputController.Display._Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputController.Display._Output[1][2]);
			Assert.AreEqual("Mana Cost: 40", OutputController.Display._Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputController.Display._Output[3][2]);
			Assert.AreEqual($"Frost damage will freeze opponent for {player._Spellbook[spellIndex]._Offensive._AmountMaxRounds} rounds.",
				OutputController.Display._Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputController.Display._Output[5][2]);
			Assert.AreEqual($"Opponent will be stunned for {player._Spellbook[spellIndex]._Offensive._AmountMaxRounds} rounds.",
				OutputController.Display._Output[6][2]);
			string[] input = new[] { "cast", "frost", "nova" };
			string spellName = InputController.ParseInput(input);
			Assert.AreEqual("frost nova", spellName);
			player._PlayerWeapon._Durability = 100;
			double baseDamage = player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			string attackSuccessString = $"You hit the {monster._Name} for {player._Spellbook[spellIndex]._Offensive._Amount} frost damage.";
			Assert.AreEqual(attackSuccessString, OutputController.Display._Output[7][2]);
			string frozenString = $"The {monster._Name} is frozen. Physical, frost and arcane damage to it will be double!";
			Assert.AreEqual(frozenString, OutputController.Display._Output[8][2]);
			OutputController.Display.ClearUserOutput();
			Assert.AreEqual(player._ManaPoints, player._MaxManaPoints - player._Spellbook[spellIndex]._ManaCost);
			int frostIndex = monster._Effects.FindIndex(
				f => f._EffectGroup == Effect.EffectType.Frozen);
			int stunIndex = monster._Effects.FindIndex(
				f => f._EffectGroup == Effect.EffectType.Stunned);
			Assert.AreEqual(85, monster._HitPoints);
			Assert.AreEqual(1, monster._Effects[frostIndex]._EffectCurRound);
			Assert.AreEqual(1, monster._Effects[stunIndex]._EffectCurRound);
			Assert.AreEqual(2, monster._Effects[frostIndex]._EffectMaxRound);
			Assert.AreEqual(2, monster._Effects[stunIndex]._EffectMaxRound);
			int monsterHitPointsBefore = monster._HitPoints;
			double totalBaseDamage = 0.0;
			double totalFrozenDamage = 0.0;
			double multiplier = monster._Effects[frostIndex]._EffectMultiplier;
			for (int i = 2; i < 4; i++) {
				OutputController.Display.ClearUserOutput();
				monster._Effects[stunIndex].StunnedRound(monster);
				string stunnedRoundString = $"The {monster._Name} is stunned and cannot attack.";
				Assert.AreEqual(stunnedRoundString, OutputController.Display._Output[0][2]);
				Assert.AreEqual(true, monster._IsStunned);
				Assert.AreEqual(i, monster._Effects[stunIndex]._EffectCurRound);
				player._PlayerWeapon._Durability = 100;
				double frozenDamage = player.PhysicalAttack(monster);
				Assert.AreEqual(i, monster._Effects[frostIndex]._EffectCurRound);
				string frozenRoundString = $"The {monster._Name} is frozen. Physical, frost and arcane damage to it will be double!";
				Assert.AreEqual(frozenRoundString, OutputController.Display._Output[1][2]);
				monster._HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameController.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster._Effects.Any());
			Assert.AreEqual(false, monster._IsStunned);
			int finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			int finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 7);
			Assert.AreEqual(monster._HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
	}
}