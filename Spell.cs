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
		public Healing Healing { get; set; }
		public int ManaCost { get; set; }
		public int Rank { get; set; }

		public Spell(string name, int manaCost, int rank, SpellType spellType) {
			this.Name = name;
			this.ManaCost = manaCost;
			this.Rank = rank;
			this.SpellCategory = spellType;
			switch(this.SpellCategory) {
				case SpellType.FireOffense:
					this.FireOffense = new FireOffense(
					25, // Blast damage
					5, // Burn damage
					1, // Burn current rounds
					3 // Burn max rounds
					);
					break;
				case SpellType.Healing:
					this.Healing = new Healing(
					25 // Heal amount
					);
					break;
				case SpellType.Defense:
					this.Defense = new Defense(
						25, // Augment armor amount
						1, // Augment current rounds
						3 // Augment max rounds
						);
					break;
				default:
					break;
			}
		}

		public string GetName() {
			return this.Name.ToString();
		}
		public void DefenseSpellInfo(Player player, int index) {
			Console.WriteLine("Augment Armor Amount: {0}", player.Spellbook[index].Defense.AugmentArmor);
			Console.WriteLine("Armor will be augmented for {0} rounds.", player.Spellbook[index].Defense.AugmentMaxRounds);
		}
		public void CastDefense(Player player, int index) {
			player.ManaPoints -= player.Spellbook[index].ManaCost;
			var augmentArmorAmount = player.Spellbook[index].Defense.AugmentArmor;
			Helper.FormatAttackSuccessText();
			Console.WriteLine("You augmented your armor by {0} with {1}.", augmentArmorAmount, player.Spellbook[index].Name);
			player.SetAugmentArmor(
				true, // Is player armor augmented
				augmentArmorAmount, // Augment armor amount
				1, // Augment current round
				3 // Augment max round
				);
		}
		public void FireOffenseSpellInfo(Player player, int index) {
			Console.WriteLine("Instant Damage: {0}", player.Spellbook[index].FireOffense.BlastDamage);
			if (player.Spellbook[index].FireOffense.BurnDamage > 0) {
				Console.WriteLine("Damage Over Time: {0}", player.Spellbook[index].FireOffense.BurnDamage);
				Console.WriteLine("Fire damage over time will burn for {0} rounds.", player.Spellbook[index].FireOffense.BurnMaxRounds);
			}
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
				Helper.FormatOnFireText();
				Console.WriteLine("The {0} bursts into flame!", opponent.Name);
				opponent.SetOnFire(
					true, // Is monster on fire
					player.Spellbook[index].FireOffense.BurnDamage, // Burn damage
					player.Spellbook[index].FireOffense.BurnCurRounds, // Burn current round
					player.Spellbook[index].FireOffense.BurnMaxRounds // Burn max round
				);
			}
		}
		public void HealingSpellInfo(Player player, int index) {
			Console.WriteLine("Heal Amount: {0}", player.Spellbook[index].Healing.HealAmount);
			if (player.Spellbook[index].Healing.HealOverTime > 0) {
				Console.WriteLine("Heal Over Time: {0}", player.Spellbook[index].Healing.HealOverTime);
				Console.WriteLine("Heal over time will restore health for {0} rounds.", player.Spellbook[index].Healing.HealMaxRounds);
			}
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
		}
	}
}