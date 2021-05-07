using DungeonGame.Controllers;
using DungeonGame.Effects;
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
		public string _Name { get; set; }
		public DamageType? _DamageGroup { get; set; }
		public SpellType _SpellCategory { get; set; }
		public ChangeAmount _ChangeAmount { get; set; }
		public Offensive _Offensive { get; set; }
		public Healing _Healing { get; set; }
		public Portal _Portal { get; set; }
		public int _MinLevel { get; set; }
		public int _ManaCost { get; set; }
		public int _Rank { get; set; }

		// Default constructor for JSON serialization
		public PlayerSpell() { }
		public PlayerSpell(string name, int manaCost, int rank, SpellType spellType, int minLevel) {
			_Name = name;
			_ManaCost = manaCost;
			_Rank = rank;
			_SpellCategory = spellType;
			_MinLevel = minLevel;
			switch (_SpellCategory) {
				case SpellType.Fireball:
					_DamageGroup = DamageType.Fire;
					_Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					_DamageGroup = DamageType.Frost;
					_Offensive = new Offensive(15, 1, 2);
					break;
				case SpellType.Lightning:
					_DamageGroup = DamageType.Arcane;
					_Offensive = new Offensive(35);
					break;
				case SpellType.Heal:
					_Healing = new Healing(50);
					break;
				case SpellType.Rejuvenate:
					_Healing = new Healing(20, 10, 1, 3);
					break;
				case SpellType.Diamondskin:
					_ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.TownPortal:
					_Portal = new Portal();
					break;
				case SpellType.Reflect:
					_ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.ArcaneIntellect:
					_ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case SpellType.FrostNova:
					_DamageGroup = DamageType.Frost;
					_Offensive = new Offensive(15, 1, 2);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void AugmentArmorSpellInfo(Player player, int index) {
			string augmentAmountString = $"Augment Armor Amount: {player._Spellbook[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				augmentAmountString);
			string augmentInfoString = $"Armor will be augmented for {player._Spellbook[index]._ChangeAmount._ChangeMaxRound} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				augmentInfoString);
		}

		public static void CastAugmentArmor(Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			int changeArmorAmount = player._Spellbook[index]._ChangeAmount._Amount;

			string augmentString = $"You augmented your armor by {changeArmorAmount} with {player._Spellbook[index]._Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				augmentString);

			player._Effects.Add(
				new ChangeArmorEffect(player._Spellbook[index]._Name, player._Spellbook[index]._ChangeAmount._ChangeMaxRound,
					player._Spellbook[index]._ChangeAmount._Amount));
		}

		public static void ReflectDamageSpellInfo(Player player, int index) {
			string reflectDamageString = $"Reflect Damage Amount: {player._Spellbook[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				reflectDamageString);
			string reflectInfoString = $"Damage up to {player._Spellbook[index]._ChangeAmount._Amount} will be reflected for " +
				$"{player._Spellbook[index]._ChangeAmount._ChangeMaxRound} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				reflectInfoString);
		}

		public static void CastArcaneIntellect(Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			const string intellectString = "You cast Arcane Intellect on yourself.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				intellectString);

			int arcaneIntIndex = player._Effects.FindIndex(e => e.Name == player._Spellbook[index]._Name);
			if (arcaneIntIndex != -1) {
				player._Effects[arcaneIntIndex].IsEffectExpired = true;
			}

			player._Intelligence += player._Spellbook[index]._ChangeAmount._Amount;

			PlayerController.CalculatePlayerStats(player);

			player._Effects.Add(
				new ChangeStatEffect(player._Spellbook[index]._Name, player._Spellbook[index]._ChangeAmount._ChangeMaxRound,
					ChangeStatEffect.StatType.Intelligence, player._Spellbook[index]._ChangeAmount._Amount));
		}

		public static void ArcaneIntellectSpellInfo(Player player, int index) {
			string arcaneIntString = $"Arcane Intellect Amount: {player._Spellbook[index]._ChangeAmount._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneIntString);
			string arcaneIntInfoString = $"_Intelligence is increased by {player._Spellbook[index]._ChangeAmount._Amount} for " +
				$"{player._Spellbook[index]._ChangeAmount._ChangeMaxRound / 60} minutes.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneIntInfoString);
		}

		public static void CastReflectDamage(Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			const string reflectString = "You create a shield around you that will reflect damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				reflectString);

			player._Effects.Add(
				new ReflectDamageEffect(player._Spellbook[index]._Name, player._Spellbook[index]._ChangeAmount._ChangeMaxRound,
					player._Spellbook[index]._ChangeAmount._Amount));
		}

		public static void FrostOffenseSpellInfo(Player player, int index) {
			string frostAmountString = $"Instant Damage: {player._Spellbook[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostAmountString);
			string frostInfoString = $"Frost damage will freeze opponent for {player._Spellbook[index]._Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostInfoString);
			const string frostDmgInfoString = "Frozen opponents take 1.5x physical, arcane and frost damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostDmgInfoString);
			if (player._Spellbook[index]._SpellCategory != SpellType.FrostNova) {
				return;
			}

			string frostNovaInfoString = $"Opponent will be stunned for {player._Spellbook[index]._Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostNovaInfoString);
		}

		public static void CastFrostOffense(Monster monster, Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			int frostSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster._Effects) {
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {frostSpellDamage} frost damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster._HitPoints -= frostSpellDamage;

			monster._Effects.Add(new FrozenEffect(player._Spellbook[index]._Name, player._Spellbook[index]._Offensive._AmountMaxRounds));

			string frozenString = $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by 50%!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);

			if (player._Spellbook[index]._SpellCategory != SpellType.FrostNova) {
				return;
			}

			monster._Effects.Add(new StunnedEffect(player._Spellbook[index]._Name, player._Spellbook[index]._Offensive._AmountMaxRounds));
		}

		public static void FireOffenseSpellInfo(Player player, int index) {
			string fireAmountString = $"Instant Damage: {player._Spellbook[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireAmountString);
			if (player._Spellbook[index]._Offensive._AmountOverTime <= 0) {
				return;
			}

			string fireOverTimeString = $"Damage Over Time: {player._Spellbook[index]._Offensive._AmountOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireOverTimeString);
			string fireInfoString = $"Fire damage over time will burn for {player._Spellbook[index]._Offensive._AmountMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireInfoString);
		}

		public static void CastFireOffense(Monster monster, Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			int fireSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster._Effects) {
				fireSpellDamage = effect.GetIncreasedDamageFromFrozen(fireSpellDamage);
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {fireSpellDamage} fire damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster._HitPoints -= fireSpellDamage;

			if (player._Spellbook[index]._Offensive._AmountOverTime <= 0) {
				return;
			}

			string onFireString = $"The {monster.Name} bursts into flame!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				onFireString);

			monster._Effects.Add(new BurningEffect(player._Spellbook[index]._Name, player._Spellbook[index]._Offensive._AmountMaxRounds,
				player._Spellbook[index]._Offensive._AmountOverTime));
		}

		public static void ArcaneOffenseSpellInfo(Player player, int index) {
			string arcaneAmountString = $"Instant Damage: {player._Spellbook[index]._Offensive._Amount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneAmountString);
		}

		public static void CastArcaneOffense(Monster monster, Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			int arcaneSpellDamage = PlayerController.CalculateSpellDamage(player, monster, index);

			foreach (FrozenEffect effect in monster._Effects) {
				arcaneSpellDamage = effect.GetIncreasedDamageFromFrozen(arcaneSpellDamage);
				effect.ProcessFrozenRound(monster);
				effect.IsEffectExpired = true;
			}

			string attackSuccessString = $"You hit the {monster.Name} for {arcaneSpellDamage} arcane damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);

			monster._HitPoints -= arcaneSpellDamage;
		}

		public static void HealingSpellInfo(Player player, int index) {
			string healAmountString = $"Heal Amount: {player._Spellbook[index]._Healing._HealAmount}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player._Spellbook[index]._Healing._HealOverTime <= 0) {
				return;
			}

			string healOverTimeString = $"Heal Over Time: {player._Spellbook[index]._Healing._HealOverTime}";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			string healInfoString = $"Heal over time will restore health for {player._Spellbook[index]._Healing._HealMaxRounds} rounds.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoString);
		}

		public static void CastHealing(Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;

			int healAmount = player._Spellbook[index]._Healing._HealAmount;

			string healString = $"You heal yourself for {healAmount} health.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				healString);

			if (player._HitPoints + healAmount > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			} else {
				player._HitPoints += healAmount;
			}

			if (player._Spellbook[index]._Healing._HealOverTime <= 0) {
				return;
			}

			player._Effects.Add(new HealingEffect(player._Spellbook[index]._Name, player._Spellbook[index]._Healing._HealMaxRounds,
				player._Spellbook[index]._Healing._HealOverTime));
		}

		public static void PortalSpellInfo() {
			const string portalString = "This spell will create a portal and return you to town.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				portalString);
		}
		public static void CastTownPortal(Player player, int index) {
			player._ManaPoints -= player._Spellbook[index]._ManaCost;
			RoomController.SetPlayerLocation(player, player._Spellbook[index]._Portal._CoordX,
				player._Spellbook[index]._Portal._CoordY, player._Spellbook[index]._Portal._CoordZ);
			const string portalString = "You open a portal and step through it.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				portalString);
			IRoom playerRoom = RoomController._Rooms[player._PlayerLocation];
			playerRoom.LookRoom();
		}
	}
}