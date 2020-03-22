using System;

namespace DungeonGame {
	public class PlayerSpell {
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
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
			this.MinLevel = minLevel;
			switch(this.SpellCategory) {
				case SpellType.Fireball:
					this.Offensive = new Offensive(
						25, 5, 1, 3, Offensive.OffensiveType.Fire);
					break;
				case SpellType.Frostbolt:
					this.Offensive = new Offensive(15, 1, 2);
					break;
				case SpellType.Lightning:
					this.Offensive = new Offensive(35);
					break;
				case SpellType.Heal:
					this.Healing = new Healing(50);
					break;
				case SpellType.Rejuvenate:
					this.Healing = new Healing(20, 10, 1, 3);
					break;
				case SpellType.Diamondskin:
					this.ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.TownPortal:
					this.Portal = new Portal();
					break;
				case SpellType.Reflect:
					this.ChangeAmount = new ChangeAmount(25, 1, 3);
					break;
				case SpellType.ArcaneIntellect:
					this.ChangeAmount = new ChangeAmount(15, 1, 600);
					break;
				case SpellType.FrostNova:
					this.Offensive = new Offensive(15, 1, 2);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public static void AugmentArmorSpellInfo(Player player, int index) {
			var augmentAmountString = "Augment Armor Amount: " + player.Spellbook[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				augmentAmountString);
			var augmentInfoString = "Armor will be augmented for " + 
			                        player.Spellbook[index].ChangeAmount.ChangeMaxRound + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				augmentInfoString);
		}
		public static void CastAugmentArmor(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var changeArmorAmount = player.Spellbook[index].ChangeAmount.Amount;
			var augmentString = "You augmented your armor by " + 
			                    changeArmorAmount + " with " + player.Spellbook[index].Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				augmentString);
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.ChangeArmor, player.Spellbook[index].ChangeAmount.Amount,
				player.Spellbook[index].ChangeAmount.ChangeCurRound, player.Spellbook[index].ChangeAmount.ChangeMaxRound, 
				1, 10, false));
		}
		public static void ReflectDamageSpellInfo(Player player, int index) {
			var reflectDamageString = "Reflect Damage Amount: " + player.Spellbook[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				reflectDamageString);
			var reflectInfoString = "Damage up to " + player.Spellbook[index].ChangeAmount.Amount + 
			                        " will be reflected for " +
			                        player.Spellbook[index].ChangeAmount.ChangeMaxRound + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				reflectInfoString);
		}
		public static void CastArcaneIntellect(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			const string intellectString = "You cast Arcane Intellect on yourself.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				intellectString);
			var arcaneIntIndex = player.Effects.FindIndex(e => e.Name == player.Spellbook[index].Name);
			if (arcaneIntIndex != -1) {
				player.Effects[arcaneIntIndex].IsEffectExpired = true;
				GameHandler.RemovedExpiredEffects(player);
			}
			player.Intelligence += player.Spellbook[index].ChangeAmount.Amount;
			PlayerHandler.CalculatePlayerStats(player);
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.ChangeStat, player.Spellbook[index].ChangeAmount.Amount,
				player.Spellbook[index].ChangeAmount.ChangeCurRound, player.Spellbook[index].ChangeAmount.ChangeMaxRound,
				1, 1, false, ChangeStat.StatType.Intelligence));
		}
		public static void ArcaneIntellectSpellInfo(Player player, int index) {
			var arcaneIntString = "Arcane Intellect Amount: " + player.Spellbook[index].ChangeAmount.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				arcaneIntString);
			var arcaneIntInfoString = "Intelligence is increased by " + player.Spellbook[index].ChangeAmount.Amount + 
			                        " for " + player.Spellbook[index].ChangeAmount.ChangeMaxRound / 60 + " minutes.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(), 
				Settings.FormatDefaultBackground(),
				arcaneIntInfoString);
		}
		public static void CastReflectDamage(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			const string reflectString = "You create a shield around you that will reflect damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				reflectString);
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.ReflectDamage, player.Spellbook[index].ChangeAmount.Amount,
				player.Spellbook[index].ChangeAmount.ChangeCurRound, player.Spellbook[index].ChangeAmount.ChangeMaxRound, 
				1, 10, false));
		}
		public static void FrostOffenseSpellInfo(Player player, int index) {
			var frostAmountString = "Instant Damage: " + player.Spellbook[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostAmountString);
			var frostInfoString = "Frost damage will freeze opponent for " +
			                      player.Spellbook[index].Offensive.AmountMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostInfoString);
			const string frostDmgInfoString = "Frozen opponents take 1.5x physical, arcane and frost damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostDmgInfoString);
			if (player.Spellbook[index].SpellCategory != SpellType.FrostNova) return;
			var frostNovaInfoString = "Opponent will be stunned for " + 
			                          player.Spellbook[index].Offensive.AmountMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				frostNovaInfoString);
		}
		public static void CastFrostOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var frostSpellDamage = player.Spellbook[index].Offensive.Amount;
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
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
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
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
			opponent.HitPoints -= frostSpellDamage;
			opponent.Effects.Add(new Effect(player.Spellbook[index].Name,Effect.EffectType.Frozen, 
				player.Spellbook[index].Offensive.AmountCurRounds, player.Spellbook[index].Offensive.AmountMaxRounds, 
				1.5, 1, true));
			if (player.Spellbook[index].SpellCategory != SpellType.FrostNova) return;
			opponent.Effects.Add(new Effect(player.Spellbook[index].Name,Effect.EffectType.Stunned, 
				player.Spellbook[index].Offensive.AmountCurRounds, player.Spellbook[index].Offensive.AmountMaxRounds, 
				1, 1, true));
		}
		public static void FireOffenseSpellInfo(Player player, int index) {
			var fireAmountString = "Instant Damage: " + player.Spellbook[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireAmountString);
			if (player.Spellbook[index].Offensive.AmountOverTime <= 0) return;
			var fireOverTimeString = "Damage Over Time: " + player.Spellbook[index].Offensive.AmountOverTime;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireOverTimeString);
			var fireInfoString = "Fire damage over time will burn for " +
			                     player.Spellbook[index].Offensive.AmountMaxRounds + " rounds.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				fireInfoString);
		}
		public static void CastFireOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var fireSpellDamage = player.Spellbook[index].Offensive.Amount;
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
						break;
					case Effect.EffectType.OnFire:
						break;
					case Effect.EffectType.Bleeding:
						break;
					case Effect.EffectType.Stunned:
						break;
					case Effect.EffectType.Frozen:
						var frozenAttackAmount = fireSpellDamage * effect.EffectMultiplier;
						fireSpellDamage = (int)frozenAttackAmount;
						effect.FrozenRound(opponent);
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var attackSuccessString = "You hit the " + opponent.Name + " for " + fireSpellDamage + " fire damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			opponent.HitPoints -= fireSpellDamage;
			if (player.Spellbook[index].Offensive.AmountOverTime <= 0) return;
			var onFireString = "The " + opponent.Name + " bursts into flame!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				onFireString);
			opponent.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.OnFire, player.Spellbook[index].Offensive.AmountOverTime,
				player.Spellbook[index].Offensive.AmountCurRounds, player.Spellbook[index].Offensive.AmountMaxRounds, 
				1, 1, true));
		}
		public static void ArcaneOffenseSpellInfo(Player player, int index) {
			var arcaneAmountString = "Instant Damage: " + player.Spellbook[index].Offensive.Amount;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				arcaneAmountString);
		}
		public static void CastArcaneOffense(Monster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var arcaneSpellDamage = player.Spellbook[index].Offensive.Amount;
			foreach (var effect in opponent.Effects) {
				switch (effect.EffectGroup) {
					case Effect.EffectType.Healing:
						break;
					case Effect.EffectType.ChangePlayerDamage:
						break;
					case Effect.EffectType.ChangeArmor:
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
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var attackSuccessString = "You hit the " + opponent.Name + " for " + arcaneSpellDamage + " arcane damage.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				attackSuccessString);
			opponent.HitPoints -= arcaneSpellDamage;
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
			if (player.HitPoints > player.MaxHitPoints) player.HitPoints = player.MaxHitPoints;
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			player.Effects.Add(new Effect(player.Spellbook[index].Name,
				Effect.EffectType.Healing, player.Spellbook[index].Healing.HealOverTime,
				player.Spellbook[index].Healing.HealCurRounds, player.Spellbook[index].Healing.HealMaxRounds,
				1, 10, false));
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
			var playerRoom = RoomHandler.Rooms[player.PlayerLocation];
			playerRoom.LookRoom();
		}
	}
}