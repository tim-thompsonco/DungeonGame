using DungeonGame;
using DungeonGame.Coordinates;
using DungeonGame.Effects;
using DungeonGame.Helpers;
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
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100, InCombat = true };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 50, MaxHitPoints = 100, FireResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			player.PlayerWeapon.CritMultiplier = 1; // Remove crit chance to remove "noise" in test
			string[] inputInfo = new[] { "spell", "fireball" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Fireball);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Fireball", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 35", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 25", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Damage Over Time: 5", OutputHelper.Display.Output[4][2]);
			Assert.AreEqual("Fire damage over time will burn for 3 rounds.",
				OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			string[] input = new[] { "cast", "fireball" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("fireball", spellName);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(25, monster.HitPoints);
			Assert.AreEqual(3, monster.Effects[0].MaxRound);
			Assert.AreEqual($"You hit the {monster.Name} for {player.Spellbook[spellIndex].Offensive.Amount} fire damage.",
				OutputHelper.Display.Output[0][2]);
			Assert.AreEqual($"The {monster.Name} bursts into flame!",
				OutputHelper.Display.Output[1][2]);
			BurningEffect burnEffect = monster.Effects[0] as BurningEffect;
			for (int i = 2; i < 5; i++) {
				burnEffect.ProcessRound();
				Assert.AreEqual(
					$"The {monster.Name} burns for {burnEffect.FireDamageOverTime} fire damage.",
					OutputHelper.Display.Output[i][2]);
				Assert.AreEqual(i, monster.Effects[0].CurrentRound);
				GameHelper.RemovedExpiredEffectsAsync(monster);
				Thread.Sleep(1000);
			}
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(10, monster.HitPoints);
		}
		[Test]
		public void FrostboltSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			string[] inputInfo = new[] { "spell", "frostbolt" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Frostbolt);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Frostbolt", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Frost damage will freeze opponent for 2 rounds.",
				OutputHelper.Display.Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			string[] input = new[] { "cast", "frostbolt" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("frostbolt", spellName);
			player.PlayerWeapon.Durability = 100;
			double baseDamage = player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[0].CurrentRound);
			Assert.AreEqual(2, monster.Effects[0].MaxRound);
			string attackString = $"You hit the {monster.Name} for {player.Spellbook[spellIndex].Offensive.Amount} frost damage.";
			Assert.AreEqual(attackString, OutputHelper.Display.Output[0][2]);
			string frozenString = $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by 50%!";
			Assert.AreEqual(frozenString, OutputHelper.Display.Output[1][2]);
			FrozenEffect frozenEffect = monster.Effects[0] as FrozenEffect;
			int monsterHitPointsBefore = monster.HitPoints;
			double totalBaseDamage = 0.0;
			double totalFrozenDamage = 0.0;
			double multiplier = frozenEffect.EffectMultiplier;
			for (int i = 2; i < 4; i++) {
				frozenEffect.ProcessRound();
				Assert.AreEqual(i, monster.Effects[0].CurrentRound);
				Assert.AreEqual(frozenString, OutputHelper.Display.Output[i][2]);
				player.PlayerWeapon.Durability = 100;
				double frozenDamage = player.PhysicalAttack(monster);
				monster.HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHelper.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster.Effects.Any());
			int finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			int finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 14);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
		[Test]
		public void LightningSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, ArcaneResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			string[] inputInfo = new[] { "spell", "lightning" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Lightning);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Lightning", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 35", OutputHelper.Display.Output[3][2]);
			string[] input = new[] { "cast", "lightning" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("lightning", spellName);
			player.PlayerWeapon.Durability = 100;
			player.CastSpell(monster, spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			int arcaneSpellDamage = player.Spellbook[spellIndex].Offensive.Amount;
			Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - arcaneSpellDamage);
			string attackSuccessString = $"You hit the {monster.Name} for {arcaneSpellDamage} arcane damage.";
			Assert.AreEqual(attackSuccessString, OutputHelper.Display.Output[4][2]);
		}
		[Test]
		public void HealSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100, MaxHitPoints = 100, HitPoints = 50 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "heal" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Heal);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Heal", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 50", OutputHelper.Display.Output[3][2]);
			string[] input = new[] { "cast", "heal" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("heal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			string healString = $"You heal yourself for {player.Spellbook[spellIndex].Healing.HealAmount} health.";
			Assert.AreEqual(healString, OutputHelper.Display.Output[4][2]);
			Assert.AreEqual(player.MaxHitPoints, player.HitPoints);
		}
		[Test]
		public void RejuvenateSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) {
				MaxManaPoints = 100,
				ManaPoints = 100,
				MaxHitPoints = 100,
				HitPoints = 50
			};
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "rejuvenate" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Rejuvenate);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Rejuvenate", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Heal Amount: 20", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("Heal Over Time: 10", OutputHelper.Display.Output[4][2]);
			string healInfoString = $"Heal over time will restore health for {player.Spellbook[spellIndex].Healing.HealMaxRounds} rounds.";
			Assert.AreEqual(healInfoString, OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			string[] input = new[] { "cast", "rejuvenate" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("rejuvenate", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(70, player.HitPoints);
			string healString = $"You heal yourself for {player.Spellbook[spellIndex].Healing.HealAmount} health.";
			Assert.AreEqual(healString, OutputHelper.Display.Output[0][2]);
			Assert.AreEqual(true, player.Effects[0] is HealingEffect);
			HealingEffect healEffect = player.Effects[0] as HealingEffect;
			for (int i = 2; i < 5; i++) {
				healEffect.ProcessRound();
				string healAmtString = $"You have been healed for {healEffect.HealOverTimeAmount} health.";
				Assert.AreEqual(i, player.Effects[0].CurrentRound);
				Assert.AreEqual(healAmtString, OutputHelper.Display.Output[i - 1][2]);
				Assert.AreEqual(70 + ((i - 1) * healEffect.HealOverTimeAmount), player.HitPoints);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void DiamondskinSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			string[] inputInfo = new[] { "spell", "diamondskin" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Diamondskin);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Diamondskin", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 25", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Augment Armor Amount: 25", OutputHelper.Display.Output[3][2]);
			string augmentInfoString = $"Armor will be augmented for {player.Spellbook[spellIndex].ChangeAmount.ChangeMaxRound} rounds.";
			Assert.AreEqual(augmentInfoString, OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "cast", "diamondskin" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("diamondskin", spellName);
			int baseArmor = GearHelper.CheckArmorRating(player);
			player.InCombat = true;
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			string augmentString = $"You augmented your armor by {player.Spellbook[spellIndex].ChangeAmount.Amount} with {player.Spellbook[spellIndex].Name}.";
			Assert.AreEqual(augmentString, OutputHelper.Display.Output[5][2]);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(true, player.Effects[0] is ChangeArmorEffect);
			ChangeArmorEffect changeArmorEffect = player.Effects[0] as ChangeArmorEffect;
			for (int i = 2; i < 5; i++) {
				int augmentedArmor = GearHelper.CheckArmorRating(player);
				Assert.AreEqual(baseArmor + 25, augmentedArmor);
				changeArmorEffect.ProcessChangeArmorRound();
				string augmentRoundString = $"Your armor is increased by {changeArmorEffect.ChangeArmorAmount}.";
				Assert.AreEqual(augmentRoundString, OutputHelper.Display.Output[i - 2][2]);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void TownPortalSpellUnitTest() {
			/* Town Portal should change location of player to where portal is set to, which is 0, 7, 0, town entrance */
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			RoomHelper.Rooms = new RoomBuilder(100, 5, 0, 4, 0).RetrieveSpawnRooms();
			player.Spellbook.Add(new PlayerSpell(
				"town portal", 100, 1, SpellType.TownPortal, 2));
			string[] inputInfo = new[] { "spell", "town", "portal" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.TownPortal);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Town Portal", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHelper.Display.Output[2][2]);
			const string portalString = "This spell will create a portal and return you to town.";
			Assert.AreEqual(portalString, OutputHelper.Display.Output[3][2]);
			Coordinate newCoord = new Coordinate(-2, 6, 0);
			player.PlayerLocation = newCoord;
			Assert.AreEqual(-2, player.PlayerLocation.X);
			Assert.AreEqual(6, player.PlayerLocation.Y);
			Assert.AreEqual(0, player.PlayerLocation.Z);
			string[] input = new[] { "cast", "town", "portal" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("town portal", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(0, player.PlayerLocation.X);
			Assert.AreEqual(7, player.PlayerLocation.Y);
			Assert.AreEqual(0, player.PlayerLocation.Z);
			Assert.AreEqual("You open a portal and step through it.", OutputHelper.Display.Output[4][2]);
		}
		[Test]
		public void ReflectDamageSpellUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			Monster monster = new Monster(3, MonsterType.Zombie) { HitPoints = 100, MaxHitPoints = 100 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			player.Spellbook.Add(new PlayerSpell(
				"reflect", 100, 1, SpellType.Reflect, 4));
			string[] inputInfo = new[] { "spell", "reflect" };
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.Reflect);
			PlayerHelper.SpellInfo(player, inputInfo);
			Assert.AreEqual("Reflect", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 100", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Reflect Damage Amount: 25", OutputHelper.Display.Output[3][2]);
			string reflectInfoString = $"Damage up to {player.Spellbook[spellIndex].ChangeAmount.Amount} will be reflected for " +
				$"{player.Spellbook[spellIndex].ChangeAmount.ChangeMaxRound} rounds.";
			Assert.AreEqual(reflectInfoString, OutputHelper.Display.Output[4][2]);
			string[] input = new[] { "cast", "reflect" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("reflect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual("You create a shield around you that will reflect damage.",
				OutputHelper.Display.Output[5][2]);
			Assert.AreEqual(true, player.Effects.Any());
			Assert.AreEqual(true, player.Effects[0] is ReflectDamageEffect);
			ReflectDamageEffect reflectDmgEffect = player.Effects[0] as ReflectDamageEffect;
			OutputHelper.Display.ClearUserOutput();
			for (int i = 2; i < 5; i++) {
				int attackDamageM = monster.MonsterWeapon.Attack();
				int reflectAmount = reflectDmgEffect.ReflectDamageAmount < attackDamageM ?
					reflectDmgEffect.ReflectDamageAmount : attackDamageM;
				Assert.AreEqual(true, reflectAmount <= reflectDmgEffect.ReflectDamageAmount);
				monster.HitPoints -= reflectAmount;
				Assert.AreEqual(monster.HitPoints, monster.MaxHitPoints - (reflectAmount * (i - 1)));
				reflectDmgEffect.ProcessChangeDamageRound(reflectAmount);
				Assert.AreEqual(
					$"You reflected {reflectAmount} damage back at your opponent!",
					OutputHelper.Display.Output[i - 2][2]);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
		}
		[Test]
		public void ArcaneIntellectSpellUnitTest() {
			OutputHelper.Display.ClearUserOutput();
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 150, ManaPoints = 150 };
			player.Spellbook.Add(new PlayerSpell(
				"arcane intellect", 150, 1, SpellType.ArcaneIntellect, 6));
			string[] infoInput = new[] { "spell", "arcane", "intellect" };
			PlayerHelper.SpellInfo(player, infoInput);
			Assert.AreEqual("Arcane Intellect", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 150", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Arcane Intellect Amount: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual("_Intelligence is increased by 15 for 10 minutes.",
				OutputHelper.Display.Output[4][2]);
			OutputHelper.Display.ClearUserOutput();
			int baseInt = player.Intelligence;
			int? baseMana = player.ManaPoints;
			int? baseMaxMana = player.MaxManaPoints;
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.ArcaneIntellect);
			string[] input = new[] { "cast", "arcane", "intellect" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("arcane intellect", spellName);
			player.CastSpell(spellName);
			Assert.AreEqual(baseMaxMana - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(player.Intelligence, baseInt + player.Spellbook[spellIndex].ChangeAmount.Amount);
			Assert.AreEqual(
				baseMana - player.Spellbook[spellIndex].ManaCost, player.ManaPoints);
			Assert.AreEqual(
				baseMaxMana + (player.Spellbook[spellIndex].ChangeAmount.Amount * 10), player.MaxManaPoints);
			Assert.AreEqual("You cast Arcane Intellect on yourself.", OutputHelper.Display.Output[0][2]);
			ChangeStatEffect changeStatEffect = player.Effects[0] as ChangeStatEffect;
			for (int i = 0; i < 10; i++) {
				changeStatEffect.ProcessChangeStatRound(player);
			}
			UserOutput defaultEffectOutput = OutputHelper.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatGeneralInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("(590 seconds) Arcane Intellect", defaultEffectOutput.Output[1][2]);
			for (int i = 0; i < 590; i++) {
				changeStatEffect.ProcessChangeStatRound(player);
			}
			GameHelper.RemovedExpiredEffectsAsync(player);
			Thread.Sleep(1000);
			Assert.AreEqual(false, player.Effects.Any());
			Assert.AreEqual(baseInt, player.Intelligence);
			Assert.AreEqual(0, player.ManaPoints);
			Assert.AreEqual(baseMaxMana, player.MaxManaPoints);
			defaultEffectOutput = OutputHelper.ShowEffects(player);
			Assert.AreEqual("Player _Effects:", defaultEffectOutput.Output[0][2]);
			Assert.AreEqual(Settings.FormatInfoText(), defaultEffectOutput.Output[1][0]);
			Assert.AreEqual("None.", defaultEffectOutput.Output[1][2]);
		}
		[Test]
		public void FrostNovaSpellUnitTest() {
			Player player = new Player("test", PlayerClassType.Mage) { MaxManaPoints = 100, ManaPoints = 100 };
			player.Spellbook.Add(new PlayerSpell(
				"frost nova", 40, 1, SpellType.FrostNova, 8));
			GearHelper.EquipInitialGear(player);
			OutputHelper.Display.ClearUserOutput();
			Monster monster = new Monster(3, MonsterType.Demon) { HitPoints = 100, MaxHitPoints = 100, FrostResistance = 0 };
			MonsterBuilder.BuildMonster(monster);
			foreach (IItem item in monster.MonsterItems.Where(item => item is IEquipment eItem && eItem.Equipped)) {
				IEquipment eItem = item as IEquipment;
				eItem.Equipped = false;
			}
			int spellIndex = player.Spellbook.FindIndex(
				f => f.SpellCategory == SpellType.FrostNova);
			string[] infoInput = new[] { "spell", "frost", "nova" };
			PlayerHelper.SpellInfo(player, infoInput);
			Assert.AreEqual("Frost Nova", OutputHelper.Display.Output[0][2]);
			Assert.AreEqual("Rank: 1", OutputHelper.Display.Output[1][2]);
			Assert.AreEqual("Mana Cost: 40", OutputHelper.Display.Output[2][2]);
			Assert.AreEqual("Instant Damage: 15", OutputHelper.Display.Output[3][2]);
			Assert.AreEqual($"Frost damage will freeze opponent for {player.Spellbook[spellIndex].Offensive.AmountMaxRounds} rounds.",
				OutputHelper.Display.Output[4][2]);
			Assert.AreEqual("Frozen opponents take 1.5x physical, arcane and frost damage.",
				OutputHelper.Display.Output[5][2]);
			Assert.AreEqual($"Opponent will be stunned for {player.Spellbook[spellIndex].Offensive.AmountMaxRounds} rounds.",
				OutputHelper.Display.Output[6][2]);
			string[] input = new[] { "cast", "frost", "nova" };
			string spellName = InputHelper.ParseInput(input);
			Assert.AreEqual("frost nova", spellName);
			player.PlayerWeapon.Durability = 100;
			double baseDamage = player.PhysicalAttack(monster);
			player.CastSpell(monster, spellName);
			string attackSuccessString = $"You hit the {monster.Name} for {player.Spellbook[spellIndex].Offensive.Amount} frost damage.";
			Assert.AreEqual(attackSuccessString, OutputHelper.Display.Output[7][2]);
			string frozenString = $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by 50%!";
			Assert.AreEqual(frozenString, OutputHelper.Display.Output[8][2]);
			OutputHelper.Display.ClearUserOutput();
			Assert.AreEqual(player.ManaPoints, player.MaxManaPoints - player.Spellbook[spellIndex].ManaCost);
			int frostIndex = monster.Effects.FindIndex(f => f is FrozenEffect);
			int stunIndex = monster.Effects.FindIndex(f => f is StunnedEffect);
			Assert.AreEqual(85, monster.HitPoints);
			Assert.AreEqual(1, monster.Effects[frostIndex].CurrentRound);
			Assert.AreEqual(1, monster.Effects[stunIndex].CurrentRound);
			Assert.AreEqual(2, monster.Effects[frostIndex].MaxRound);
			Assert.AreEqual(2, monster.Effects[stunIndex].MaxRound);
			FrozenEffect frozenEffect = monster.Effects[frostIndex] as FrozenEffect;
			StunnedEffect stunnedEffect = monster.Effects[stunIndex] as StunnedEffect;
			int monsterHitPointsBefore = monster.HitPoints;
			double totalBaseDamage = 0.0;
			double totalFrozenDamage = 0.0;
			double multiplier = frozenEffect.EffectMultiplier;
			for (int i = 2; i < 4; i++) {
				OutputHelper.Display.ClearUserOutput();
				stunnedEffect.ProcessRound();
				frozenEffect.ProcessRound();
				string stunnedRoundString = $"The {monster.Name} is stunned and cannot attack.";
				Assert.AreEqual(stunnedRoundString, OutputHelper.Display.Output[0][2]);
				Assert.AreEqual(true, monster.IsStunned);
				Assert.AreEqual(i, monster.Effects[stunIndex].CurrentRound);
				player.PlayerWeapon.Durability = 100;
				double frozenDamage = player.PhysicalAttack(monster);
				Assert.AreEqual(i, monster.Effects[frostIndex].CurrentRound);
				string frozenRoundString = $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by 50%!";
				Assert.AreEqual(frozenRoundString, OutputHelper.Display.Output[1][2]);
				monster.HitPoints -= (int)frozenDamage;
				totalBaseDamage += baseDamage;
				totalFrozenDamage += frozenDamage;
			}
			GameHelper.RemovedExpiredEffectsAsync(monster);
			Thread.Sleep(1000);
			Assert.AreEqual(false, monster.Effects.Any());
			Assert.AreEqual(false, monster.IsStunned);
			int finalBaseDamageWithMod = (int)(totalBaseDamage * multiplier);
			int finalTotalFrozenDamage = (int)totalFrozenDamage;
			Assert.AreEqual(finalTotalFrozenDamage, finalBaseDamageWithMod, 14);
			Assert.AreEqual(monster.HitPoints, monsterHitPointsBefore - (int)totalFrozenDamage);
		}
	}
}