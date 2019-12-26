using System;

namespace DungeonGame {
	public class Spell {
		public enum SpellType {
			FireOffense,
			FrostOffense,
			ArcaneOffense,
			Healing,
			Defense
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
		public bool OverTime { get; set; }

		public Spell(string name, int manaCost, int rank, SpellType spellType, bool overTime) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
			this.OverTime = overTime;
			switch(this.SpellCategory) {
				case SpellType.FireOffense:
					this.FireOffense = new FireOffense(25, 5, 1, 3);
					break;
				case SpellType.Healing when this.OverTime:
					this.Healing = new Healing(20, 10, 1, 3);
					break;
				case SpellType.Healing when this.OverTime == false:
					this.Healing = new Healing(50);
					break;
				case SpellType.Defense:
					this.Defense = new Defense(25, 1, 3);
					break;
				case SpellType.FrostOffense:
					this.FrostOffense = new FrostOffense(15, 1, 2);
					break;
				case SpellType.ArcaneOffense:
					this.ArcaneOffense = new ArcaneOffense(35);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string GetName() {
			return this.Name.ToString();
		}
		public void DefenseSpellInfo(Player player, int index) {
			Console.WriteLine("Augment Armor Amount: {0}", player.Spellbook[index].Defense.AugmentAmount);
			Console.WriteLine("Armor will be augmented for {0} rounds.", player.Spellbook[index].Defense.AugmentMaxRounds);
		}
		public void CastDefense(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var augmentArmorAmount = player.Spellbook[index].Defense.AugmentAmount;
			Helper.FormatAttackSuccessText();
			Console.WriteLine("You augmented your armor by {0} with {1}.", augmentArmorAmount, player.Spellbook[index].Name);
			player.SetAugmentArmor(true, augmentArmorAmount, 1, 3);
		}
		public void FrostOffenseSpellInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Spellbook[index].FrostOffense.FrostDamage);
			Console.WriteLine("Frost damage will freeze opponent for {0} rounds, stunning them.",
				player.Spellbook[index].FrostOffense.FrozenMaxRounds);
		}
		public void CastFrostOffense(IMonster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var frostSpellDamage = player.Spellbook[index].FrostOffense.FrostDamage;
			if (frostSpellDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You missed!");
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You hit the {0} for {1} frost damage.", opponent.Name, frostSpellDamage);
				opponent.TakeDamage(frostSpellDamage);
				opponent.StartStunned(
					true,
					player.Spellbook[index].FrostOffense.FrozenCurRounds,
					player.Spellbook[index].FrostOffense.FrozenMaxRounds
					);
			}
		}
		public void FireOffenseSpellInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Spellbook[index].FireOffense.BlastDamage);
			if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
			Console.WriteLine("Damage Over Time: {0}", player.Spellbook[index].FireOffense.BurnDamage);
			Console.WriteLine("Fire damage over time will burn for {0} rounds.", player.Spellbook[index].FireOffense.BurnMaxRounds);
		}
		public void CastFireOffense(IMonster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var fireSpellDamage = player.Spellbook[index].FireOffense.BlastDamage;
			if (fireSpellDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You missed!");
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You hit the {0} for {1} fire damage.", opponent.Name, fireSpellDamage);
				opponent.TakeDamage(fireSpellDamage);
				if (player.Spellbook[index].FireOffense.BurnDamage <= 0) return;
				Helper.FormatOnFireText();
				Console.WriteLine("The {0} bursts into flame!", opponent.Name);
				opponent.SetOnFire(
					true,
					player.Spellbook[index].FireOffense.BurnDamage,
					player.Spellbook[index].FireOffense.BurnCurRounds,
					player.Spellbook[index].FireOffense.BurnMaxRounds
				);
			}
		}
		public void ArcaneOffenseSpellInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Spellbook[index].ArcaneOffense.ArcaneDamage);
		}
		public void CastArcaneOffense(IMonster opponent, Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var arcaneSpellDamage = player.Spellbook[index].ArcaneOffense.ArcaneDamage;
			if (arcaneSpellDamage == 0) {
				Helper.FormatAttackFailText();
				Console.WriteLine("You missed!");
			}
			else {
				Helper.FormatAttackSuccessText();
				Console.WriteLine("You hit the {0} for {1} arcane damage.", opponent.Name, arcaneSpellDamage);
				opponent.TakeDamage(arcaneSpellDamage);
			}
		}
		public void HealingSpellInfo(Player player, int index) {
			Console.WriteLine("Heal Amount: {0}", player.Spellbook[index].Healing.HealAmount);
			if (player.Spellbook[index].Healing.HealOverTime <= 0) return;
			Console.WriteLine("Heal Over Time: {0}", player.Spellbook[index].Healing.HealOverTime);
			Console.WriteLine("Heal over time will restore health for {0} rounds.", player.Spellbook[index].Healing.HealMaxRounds);
		}
		public void CastHealing(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var healAmount = player.Spellbook[index].Healing.HealAmount;
			Helper.FormatAttackSuccessText();
			Console.WriteLine("You heal yourself for {0} health.", healAmount);
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