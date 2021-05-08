using DungeonGame.Controllers;
using DungeonGame.Effects;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Monsters;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;

namespace DungeonGame {
	public class PlayerSpell {
		public enum DamageType {
			Physical,
			Fire,
			Frost,
			Arcane
		}
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning,
			Heal,
			Rejuvenate,
			Diamondskin,
			TownPortal,
			Reflect,
			ArcaneIntellect,
			FrostNova
		}
		public string Name { get; set; }
		public DamageType? DamageGroup { get; set; }
		public SpellType SpellCategory { get; set; }
		public ChangeAmount ChangeAmount { get; set; }
		public Offensive Offensive { get; set; }
		public Healing Healing { get; set; }
		public Portal Portal { get; set; }
		public int MinLevel { get; set; }
		public int ManaCost { get; set; }
		public int Rank { get; set; }

		// Default constructor for JSON serialization
		public PlayerSpell() { }
		public PlayerSpell(string name, int manaCost, int rank, SpellType spellType, int minLevel) {
			Name = name;
			ManaCost = manaCost;
			Rank = rank;
			SpellCategory = spellType;
			MinLevel = minLevel;
			switch (SpellCategory) {
				case SpellType.Fireball:
					DamageGroup = DamageType.Fire;
					Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					DamageGroup = DamageType.Frost;
					Offensive = new Offensive(15, 1, 2);
					break;
				case SpellType.Lightning:
					DamageGroup = DamageType.Arcane;
					Offensive = new Offensive(35);
					break;
				case SpellType.Heal:
					Healing = new Healing(50);
					break;
				case SpellType.Rejuvenate:
					Healing = new Healing(20, 10, 1, 3);
					break;
				case SpellType.Diamondskin:
					ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.TownPortal:
					Portal = new Portal();
					break;
				case SpellType.Reflect:
					ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.ArcaneIntellect:
					ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case SpellType.FrostNova:
					DamageGroup = DamageType.Frost;
					Offensive = new Offensive(15, 1, 2);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void AugmentArmorSpellInfo(Player player, int index) {
			string augmentAmountString = $"Augment Armor Amount: {player.Spellbook[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				augmentAmountString);
			string augmentInfoString = $"Armor will be augmented for {player.Spellbook[index].ChangeAmount._ChangeMaxRound} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				augmentInfoString);
		}

		public static void CastAugmentArmor(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			int changeArmorAmount = player.Spellbook[index].ChangeAmount._Amount;

			string augmentString = $"You augmented your armor by {changeArmorAmount} with {player.Spellbook[index].Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				augmentString);

			player.Effects.Add(
				new ChangeArmorEffect(player.Spellbook[index].Name, player.Spellbook[index].ChangeAmount._ChangeMaxRound,
					player.Spellbook[index].ChangeAmount._Amount));
		}

		public static void ReflectDamageSpellInfo(Player player, int index) {
			string reflectDamageString = $"Reflect Damage Amount: {player.Spellbook[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				reflectDamageString);
			string reflectInfoString = $"Damage up to {player.Spellbook[index].ChangeAmount._Amount} will be reflected for " +
				$"{player.Spellbook[index].ChangeAmount._ChangeMaxRound} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				reflectInfoString);
		}

		public static void CastArcaneIntellect(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			const string intellectString = "You cast Arcane Intellect on yourself.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				intellectString);

			int arcaneIntIndex = player.Effects.FindIndex(e => e.Name == player.Spellbook[index].Name);
			if (arcaneIntIndex != -1) {
				player.Effects[arcaneIntIndex].IsEffectExpired = true;
			}

			player.Intelligence += player.Spellbook[index].ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player.Effects.Add(
				new ChangeStatEffect(player.Spellbook[index].Name, player.Spellbook[index].ChangeAmount._ChangeMaxRound,
					ChangeStatEffect.StatType.Intelligence, player.Spellbook[index].ChangeAmount._Amount));
		}

		public static void ArcaneIntellectSpellInfo(Player player, int index) {
			string arcaneIntString = $"Arcane Intellect Amount: {player.Spellbook[index].ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneIntString);
			string arcaneIntInfoString = $"_Intelligence is increased by {player.Spellbook[index].ChangeAmount._Amount} for " +
				$"{player.Spellbook[index].ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneIntInfoString);
		}

		public static void CastReflectDamage(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			const string reflectString = "You create a shield around you that will reflect damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				reflectString);

			player.Effects.Add(
				new ReflectDamageEffect(player.Spellbook[index].Name, player.Spellbook[index].ChangeAmount._ChangeMaxRound,
					player.Spellbook[index].ChangeAmount._Amount));
		}

		public static void FrostOffenseSpellInfo(Player player, int index) {
			string frostAmountString = $"Instant Damage: {player.Spellbook[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostAmountString);
			string frostInfoString = $"Frost damage will freeze opponent for {player.Spellbook[index].Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostInfoString);
			const string frostDmgInfoString = "Frozen opponents take 1.5x physical, arcane and frost damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostDmgInfoString);
			if (player.Spellbook[index].SpellCategory != SpellType.FrostNova) {
				return;
			}

			string frostNovaInfoString = $"Opponent will be stunned for {player.Spellbook[index].Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostNovaInfoString);
		}

		public static void CastFrostOffense(Monster monster, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			int frostSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster.Effects) {
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {frostSpellDamage} frost damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster.HitPoints -= frostSpellDamage;

			monster.Effects.Add(new FrozenEffect(player.Spellbook[index].Name, player.Spellbook[index].Offensive._AmountMaxRounds));

			string frozenString = $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by 50%!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);

			if (player.Spellbook[index].SpellCategory != SpellType.FrostNova) {
				return;
			}

			EffectSettings effectSettings = new EffectSettings {
				EffectHolder = monster,
				MaxRound = player.Spellbook[index].Offensive._AmountMaxRounds,
				Name = player.Spellbook[index].Name
			};
			monster.Effects.Add(new StunnedEffect(effectSettings));
		}

		public static void FireOffenseSpellInfo(Player player, int index) {
			string fireAmountString = $"Instant Damage: {player.Spellbook[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireAmountString);
			if (player.Spellbook[index].Offensive._AmountOverTime <= 0) {
				return;
			}

			string fireOverTimeString = $"Damage Over Time: {player.Spellbook[index].Offensive._AmountOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireOverTimeString);
			string fireInfoString = $"Fire damage over time will burn for {player.Spellbook[index].Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireInfoString);
		}

		public static void CastFireOffense(Monster monster, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			int fireSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster.Effects) {
				fireSpellDamage = effect.GetIncreasedDamageFromFrozen(fireSpellDamage);
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {fireSpellDamage} fire damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster.HitPoints -= fireSpellDamage;

			if (player.Spellbook[index].Offensive._AmountOverTime <= 0) {
				return;
			}

			string onFireString = $"The {monster.Name} bursts into flame!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				onFireString);

			monster.Effects.Add(new BurningEffect(player.Spellbook[index].Name, player.Spellbook[index].Offensive._AmountMaxRounds,
				player.Spellbook[index].Offensive._AmountOverTime));
		}

		public static void ArcaneOffenseSpellInfo(Player player, int index) {
			string arcaneAmountString = $"Instant Damage: {player.Spellbook[index].Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneAmountString);
		}

		public static void CastArcaneOffense(Monster monster, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			int arcaneSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster.Effects) {
				arcaneSpellDamage = effect.GetIncreasedDamageFromFrozen(arcaneSpellDamage);
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {arcaneSpellDamage} arcane damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster.HitPoints -= arcaneSpellDamage;
		}

		public static void HealingSpellInfo(Player player, int index) {
			string healAmountString = $"Heal Amount: {player.Spellbook[index].Healing._HealAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player.Spellbook[index].Healing._HealOverTime <= 0) {
				return;
			}

			string healOverTimeString = $"Heal Over Time: {player.Spellbook[index].Healing._HealOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			string healInfoString = $"Heal over time will restore health for {player.Spellbook[index].Healing._HealMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoString);
		}

		public static void CastHealing(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;

			int healAmount = player.Spellbook[index].Healing._HealAmount;

			string healString = $"You heal yourself for {healAmount} health.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				healString);

			if (player.HitPoints + healAmount > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			} else {
				player.HitPoints += healAmount;
			}

			if (player.Spellbook[index].Healing._HealOverTime <= 0) {
				return;
			}

			player.Effects.Add(new HealingEffect(player.Spellbook[index].Name, player.Spellbook[index].Healing._HealMaxRounds,
				player.Spellbook[index].Healing._HealOverTime));
		}

		public static void PortalSpellInfo() {
			const string portalString = "This spell will create a portal and return you to town.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				portalString);
		}
		public static void CastTownPortal(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			RoomController.SetPlayerLocation(player, player.Spellbook[index].Portal._CoordX,
				player.Spellbook[index].Portal._CoordY, player.Spellbook[index].Portal._CoordZ);
			const string portalString = "You open a portal and step through it.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				portalString);
			IRoom playerRoom = RoomController._Rooms[player.PlayerLocation];
			playerRoom.LookRoom();
		}
	}
}