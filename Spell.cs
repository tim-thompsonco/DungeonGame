using System;

namespace DungeonGame {
	public class Spell {
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning,
			Heal,
			Rejuvenate,
			Diamondskin,
			TownPortal
		}
		public string Name { get; set; }
		public SpellType SpellCategory { get; set; }
		public Defense Defense { get; set; }
		public FireOffense FireOffense { get; set; }
		public FrostOffense FrostOffense { get; set; }
		public ArcaneOffense ArcaneOffense { get; set; }
		public Healing Healing { get; set; }
		public Portal Portal { get; set; }
		public int MinLevel { get; set; }
		public int ManaCost { get; set; }
		public int Rank { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Spell() {}
		public Spell(string name, int manaCost, int rank, SpellType spellType, int minLevel) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
			this.MinLevel = minLevel;
			switch(this.SpellCategory) {
				case SpellType.Fireball:
					this.FireOffense = new FireOffense(25, 5, 1, 3);
					break;
				case SpellType.Frostbolt:
					this.FrostOffense = new FrostOffense(15, 1, 2);
					break;
				case SpellType.Lightning:
					this.ArcaneOffense = new ArcaneOffense(35);
					break;
				case SpellType.Heal:
					this.Healing = new Healing(50);
					break;
				case SpellType.Rejuvenate:
					this.Healing = new Healing(20, 10, 1, 3);
					break;
				case SpellType.Diamondskin:
					this.Defense = new Defense(25, 1, 3);
					break;
				case SpellType.TownPortal:
					this.Portal = new Portal();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string GetName() {
			return this.Name;
		}
		public static void DefenseSpellInfo(Player player, int index) {
			var augmentAmountString = "Augment Armor Amount: " + player.Spellbook[index].Defense.AugmentAmount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				augmentAmountString);
			var augmentInfoString = "Armor will be augmented for " + 
			                        player.Spellbook[index].Defense.AugmentMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				augmentInfoString);
		}
		public static void CastDefense(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var changeArmorAmount = player.Spellbook[index].Defense.AugmentAmount;
			var augmentString = "You augmented your armor by " + 
			                    changeArmorAmount + " with " + player.Spellbook[index].Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				augmentString);
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.ChangeArmor, player.Spellbook[index].Defense.AugmentAmount,
				player.Spellbook[index].Defense.AugmentCurRounds, player.Spellbook[index].Defense.AugmentMaxRounds, 
				1, 10));
		}
		public static void FrostOffenseSpellInfo(Player player, int index) {
			var frostAmountString = "Instant Damage: " + player.Spellbook[index].FrostOffense.FrostDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostAmountString);
			var frostInfoString = "Frost damage will freeze opponent for " + 
			                      player.Spellbook[index].FrostOffense.FrozenMaxRounds + " rounds, increasing subsequent + " +
			                      "physical, arcane and frost damage by 1.5x.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostInfoString);
		}
		public static void CastFrostOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var frostSpellDamage = player.Spellbook[index].FrostOffense.FrostDamage;
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangeDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.AbsorbDamage:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = frostSpellDamage * effect.EffectMultiplier;
						frostSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(opponent);
						effect.IsEffectExpired = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (frostSpellDamage == 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + frostSpellDamage + " frost damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					attackSuccessString);
				var frozenEffectIndex = opponent.Effects.FindIndex(
					e => e.EffectGroup == Effect.EffectType.Frozen);
				if (frozenEffectIndex == -1) {
					var frozenString = "The " + opponent.Name +
					                   " is frozen. Physical, frost and arcane damage to it will be double!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackSuccessText(),
						Settings.FormatDefaultBackground(),
						frozenString);
				} 
				opponent.TakeDamage(frostSpellDamage);
				opponent.Effects.Add(new Effect(player.Spellbook[index].Name,Effect.EffectType.Frozen, 
					player.Spellbook[index].FrostOffense.FrozenCurRounds, player.Spellbook[index].FrostOffense.FrozenMaxRounds, 
					1.5, 1));
			}
		}
		public static void FireOffenseSpellInfo(Player player, int index) {
			var fireAmountString = "Instant Damage: " + player.Spellbook[index].FireOffense.BlastDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireAmountString);
			if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
			var fireOverTimeString = "Damage Over Time: " + player.Spellbook[index].FireOffense.BurnDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireOverTimeString);
			var fireInfoString = "Fire damage over time will burn for " +
			                     player.Spellbook[index].FireOffense.BurnMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireInfoString);
		}
		public static void CastFireOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var fireSpellDamage = player.Spellbook[index].FireOffense.BlastDamage;
			if (fireSpellDamage == 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + fireSpellDamage + " fire damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(fireSpellDamage);
				if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
				var onFireString = "The " + opponent.Name + " bursts into flame!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatOnFireText(),
					Settings.FormatDefaultBackground(),
					onFireString);
				opponent.Effects.Add(new Effect(player.Spellbook[index].Name,
					Effect.EffectType.OnFire, player.Spellbook[index].FireOffense.BurnDamage,
					player.Spellbook[index].FireOffense.BurnCurRounds, player.Spellbook[index].FireOffense.BurnMaxRounds, 
					1, 1));
			}
		}
		public static void ArcaneOffenseSpellInfo(Player player, int index) {
			var arcaneAmountString = "Instant Damage: " + player.Spellbook[index].ArcaneOffense.ArcaneDamage;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneAmountString);
		}
		public static void CastArcaneOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var arcaneSpellDamage = player.Spellbook[index].ArcaneOffense.ArcaneDamage;
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangeDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.AbsorbDamage:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = arcaneSpellDamage * effect.EffectMultiplier;
						arcaneSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(opponent);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (arcaneSpellDamage == 0) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + arcaneSpellDamage + " arcane damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(arcaneSpellDamage);
			}
		}
		public static void HealingSpellInfo(Player player, int index) {
			var healAmountString = "Heal Amount: " + player.Spellbook[index].Healing.HealAmount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healAmountString);
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			var healOverTimeString = "Heal Over Time: " + player.Spellbook[index].Healing.HealOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healOverTimeString);
			var healInfoString = "Heal over time will restore health for " + 
			                     player.Spellbook[index].Healing.HealMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				healInfoString);
		}
		public static void CastHealing(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var healAmount = player.Spellbook[index].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				healString);
			player.HitPoints += healAmount;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.Healing, player.Spellbook[index].Healing.HealOverTime,
				player.Spellbook[index].Healing.HealCurRounds, player.Spellbook[index].Healing.HealMaxRounds,
				1, 10));
		}
		public static void PortalSpellInfo() {
			const string portalString = "This spell will create a portal and return you to town.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				portalString);
		}
		public static void CastTownPortal(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			RoomHandler.SetPlayerLocation(player, player.Spellbook[index].Portal.CoordX, 
				player.Spellbook[index].Portal.CoordY, player.Spellbook[index].Portal.CoordZ);
			const string portalString = "You open a portal and step through it.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				portalString);
			RoomHandler.Rooms[RoomHandler.RoomIndex].LookRoom();
		}
	}
}