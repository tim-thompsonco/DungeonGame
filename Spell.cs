using System;

namespace DungeonGame {
	public class Spell {
		public enum SpellType {
			Fireball,
			Frostbolt,
			Lightning,
			Heal,
			Rejuvenate,
			Diamondskin
		}
		public string Name { get; set; }
		public SpellType SpellCategory { get; set; }
		public Defense Defense { get; set; }
		public FireOffense FireOffense { get; set; }
		public FrostOffense FrostOffense { get; set; }
		public ArcaneOffense ArcaneOffense { get; set; }
		public Healing Healing { get; set; }
		public int ManaCost { get; set; }
		public int Rank { get; set; }

		public Spell(string name, int manaCost, int rank, SpellType spellType) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
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
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string GetName() {
			return this.Name;
		}
		public static void DefenseSpellInfo(Player player, int index, UserOutput output) {
			var augmentAmountString = "Augment Armor Amount: " + player.Spellbook[index].Defense.AugmentAmount;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(),
				augmentAmountString);
			var augmentInfoString = "Armor will be augmented for " + 
			                        player.Spellbook[index].Defense.AugmentMaxRounds + " rounds.";
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(),
				augmentInfoString);
		}
		public static void CastDefense(Player player, int index, UserOutput output) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var changeArmorAmount = player.Spellbook[index].Defense.AugmentAmount;
			var augmentString = "You augmented your armor by " + 
			                    changeArmorAmount + " with " + player.Spellbook[index].Name + ".";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				augmentString);
			player.SetChangeArmor(true, changeArmorAmount, 1, 3);
		}
		public static void FrostOffenseSpellInfo(Player player, int index, UserOutput output) {
			var frostAmountString = "Instant Damage: " + player.Spellbook[index].FrostOffense.FrostDamage;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				frostAmountString);
			var frostInfoString = "Frost damage will freeze opponent for " + 
			                      player.Spellbook[index].FrostOffense.FrozenMaxRounds + " rounds, stunning them.";
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				frostInfoString);
		}
		public static void CastFrostOffense(IMonster opponent, Player player, int index, UserOutput output) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var frostSpellDamage = player.Spellbook[index].FrostOffense.FrostDamage;
			if (frostSpellDamage == 0) {
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + frostSpellDamage + " frost damage.";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(frostSpellDamage);
				opponent.StartStunned(
					true,
					player.Spellbook[index].FrostOffense.FrozenCurRounds,
					player.Spellbook[index].FrostOffense.FrozenMaxRounds
					);
			}
		}
		public static void FireOffenseSpellInfo(Player player, int index, UserOutput output) {
			var fireAmountString = "Instant Damage: " + player.Spellbook[index].FireOffense.BlastDamage;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				fireAmountString);
			if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
			var fireOverTimeString = "Damage Over Time: " + player.Spellbook[index].FireOffense.BurnDamage;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				fireOverTimeString);
			var fireInfoString = "Fire damage over time will burn for " +
			                     player.Spellbook[index].FireOffense.BurnMaxRounds + " rounds.";
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				fireInfoString);
		}
		public static void CastFireOffense(IMonster opponent, Player player, int index, UserOutput output) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var fireSpellDamage = player.Spellbook[index].FireOffense.BlastDamage;
			if (fireSpellDamage == 0) {
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + fireSpellDamage + " fire damage.";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(fireSpellDamage);
				if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
				var onFireString = "The " + opponent.Name + " bursts into flame!";
				output.StoreUserOutput(
					Helper.FormatOnFireText(),
					Helper.FormatDefaultBackground(),
					onFireString);
				opponent.SetOnFire(
					true,
					player.Spellbook[index].FireOffense.BurnDamage,
					player.Spellbook[index].FireOffense.BurnCurRounds,
					player.Spellbook[index].FireOffense.BurnMaxRounds
				);
			}
		}
		public static void ArcaneOffenseSpellInfo(Player player, int index, UserOutput output) {
			var arcaneAmountString = "Instant Damage: " + player.Spellbook[index].ArcaneOffense.ArcaneDamage;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				arcaneAmountString);
		}
		public static void CastArcaneOffense(IMonster opponent, Player player, int index, UserOutput output) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var arcaneSpellDamage = player.Spellbook[index].ArcaneOffense.ArcaneDamage;
			if (arcaneSpellDamage == 0) {
				output.StoreUserOutput(
					Helper.FormatAttackFailText(),
					Helper.FormatDefaultBackground(),
					"You missed!");
			}
			else {
				var attackSuccessString = "You hit the " + opponent.Name + " for " + arcaneSpellDamage + " arcane damage.";
				output.StoreUserOutput(
					Helper.FormatAttackSuccessText(),
					Helper.FormatDefaultBackground(),
					attackSuccessString);
				opponent.TakeDamage(arcaneSpellDamage);
			}
		}
		public static void HealingSpellInfo(Player player, int index, UserOutput output) {
			var healAmountString = "Heal Amount: " + player.Spellbook[index].Healing.HealAmount;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				healAmountString);
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			var healOverTimeString = "Heal Over Time: " + player.Spellbook[index].Healing.HealOverTime;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				healOverTimeString);
			var healInfoString = "Heal over time will restore health for " + 
			                     player.Spellbook[index].Healing.HealMaxRounds + " rounds.";
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				healInfoString);
		}
		public static void CastHealing(Player player, int index, UserOutput output) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var healAmount = player.Spellbook[index].Healing.HealAmount;
			var healString = "You heal yourself for " + healAmount + " health.";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				healString);
			player.HitPoints += healAmount;
			if (player.HitPoints > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			}
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			player.SetHealing(
				true,
				player.Spellbook[index].Healing.HealOverTime,
				player.Spellbook[index].Healing.HealCurRounds,
				player.Spellbook[index].Healing.HealMaxRounds);
		}
	}
}