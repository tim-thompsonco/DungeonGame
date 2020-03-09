using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class Monster : IRoomInteraction {
		public enum MonsterType {
			Skeleton,
			Zombie,
			Spider,
			Demon,
			Elemental,
			Vampire,
			Troll,
			Dragon
		}
		public enum ElementalType {
			Fire,
			Air,
			Water
		}
		public enum SkeletonType {
			Warrior,
			Archer,
			Mage
		}
		public enum TrollType {
			Warrior,
			Shaman
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitPoints { get; set; }
		public int MaxEnergyPoints { get; set; }
		public int EnergyPoints { get; set; }
		public int ExperienceProvided { get; set; }
		public int Gold { get; set; }
		public bool WasLooted { get; set; }
		public bool InCombat { get; set; }
		public bool IsStunned { get; set; }
		public int StatReplenishInterval { get; set; }
		public MonsterType MonsterCategory { get; set; }
		public ElementalType? ElementalCategory { get; set; }
		public SkeletonType? SkeletonCategory { get; set; }
		public TrollType? TrollCategory { get; set; }
		public Weapon MonsterWeapon { get; set; }
		public Quiver MonsterQuiver { get; set; }
		public Armor MonsterHeadArmor { get; set; }
		public Armor MonsterBackArmor { get; set; }
		public Armor MonsterChestArmor { get; set; }
		public Armor MonsterWristArmor { get; set; }
		public Armor MonsterHandsArmor { get; set; }
		public Armor MonsterWaistArmor { get; set; }
		public Armor MonsterLegArmor { get; set; }
		public List<IEquipment> MonsterItems { get; set; }
		public List<Effect> Effects { get; set; }
		public List<MonsterSpell> Spellbook { get; set; }
		public List<MonsterAbility> Abilities { get; set; }

		public Monster(int level, MonsterType monsterType) {
			this.MonsterItems = new List<IEquipment>();
			this.Effects = new List<Effect>();
			this.StatReplenishInterval = 3;
			this.Level = level;
			this.MonsterCategory = monsterType;
			var randomNumHitPoint = GameHandler.GetRandomNumber(20, 40);
			var maxHitPoints = 80 + (this.Level - 1) * randomNumHitPoint;
			this.MaxHitPoints = GameHandler.RoundNumber(maxHitPoints);
			this.HitPoints = this.MaxHitPoints;
			if (this.MonsterCategory == MonsterType.Spider) {
				this.Gold = 0;
			}
			else {
				var randomNumGold = GameHandler.GetRandomNumber(5, 10);
				this.Gold = 10 + (this.Level - 1) * randomNumGold;
			}
			var randomNumExp = GameHandler.GetRandomNumber(20, 40);
			var expProvided = this.MaxHitPoints + randomNumExp;
			this.ExperienceProvided = GameHandler.RoundNumber(expProvided);
			this.MaxEnergyPoints = 100 + this.Level * 10;
			this.EnergyPoints = this.MaxEnergyPoints;
			switch (this.MonsterCategory) {
				case MonsterType.Skeleton:
					var randomSkeletonType = GameHandler.GetRandomNumber(1, 3);
					this.SkeletonCategory = randomSkeletonType switch {
						1 => SkeletonType.Archer,
						2 => SkeletonType.Warrior,
						3 => SkeletonType.Mage,
						_ => throw new ArgumentOutOfRangeException()
					};
					switch (this.SkeletonCategory) {
						case SkeletonType.Warrior:
							break;
						case SkeletonType.Archer:
							break;
						case SkeletonType.Mage:
							this.Spellbook = new List<MonsterSpell> {
								new MonsterSpell("frostbolt", 50, MonsterSpell.SpellType.Frostbolt, this.Level), 
								new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, this.Level)};
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case MonsterType.Zombie:
					break;
				case MonsterType.Spider:
					this.Abilities = new List<MonsterAbility> {
						new MonsterAbility("poison bite", 50, MonsterAbility.Ability.PoisonBite, this.Level)};
					break;
				case MonsterType.Demon:
					this.Spellbook = new List<MonsterSpell> {
						new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, this.Level)};
					break;
				case MonsterType.Elemental:
					var randomElementalType = GameHandler.GetRandomNumber(1, 3);
					this.ElementalCategory = randomElementalType switch {
						1 => ElementalType.Air,
						2 => ElementalType.Fire,
						3 => ElementalType.Water,
						_ => throw new ArgumentOutOfRangeException()
					};
					switch (this.ElementalCategory) {
						case ElementalType.Fire:
							this.Spellbook = new List<MonsterSpell> {
								new MonsterSpell("fireball", 50, MonsterSpell.SpellType.Fireball, this.Level)};
							break;
						case ElementalType.Air:
							this.Spellbook = new List<MonsterSpell> {
								new MonsterSpell("lightning", 50, MonsterSpell.SpellType.Lightning, this.Level)};
							break;
						case ElementalType.Water:
							this.Spellbook = new List<MonsterSpell> {
								new MonsterSpell("frostbolt", 50, MonsterSpell.SpellType.Frostbolt, this.Level)};
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case MonsterType.Vampire:
					this.Abilities = new List<MonsterAbility> {
						new MonsterAbility("blood leech", 50, MonsterAbility.Ability.BloodLeech, this.Level)};
					break;
				case MonsterType.Troll:
					var randomTrollType = GameHandler.GetRandomNumber(1, 2);
					this.TrollCategory = randomTrollType switch {
						1 => TrollType.Shaman,
						2 => TrollType.Warrior,
						_ => throw new ArgumentOutOfRangeException()
					};
					switch (this.TrollCategory) {
						case TrollType.Warrior:
							break;
						case TrollType.Shaman:
							this.Spellbook = new List<MonsterSpell> {
								new MonsterSpell("heal", 50, MonsterSpell.SpellType.Heal, this.Level)};
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case MonsterType.Dragon:
					this.Abilities = new List<MonsterAbility> {
						new MonsterAbility("tail whip", 50, MonsterAbility.Ability.TailWhip, this.Level)};
					this.Spellbook = new List<MonsterSpell> {
						new MonsterSpell("fire breath", 50, MonsterSpell.SpellType.Fireball, this.Level)};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		public int Attack(Player player) {
			var attackAmount = 0;
			try {
				if (this.MonsterWeapon.Equipped && this.MonsterWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
					attackAmount = this.MonsterWeapon.Attack();
				}
				if (this.MonsterWeapon.Equipped &&
				    this.MonsterWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    this.MonsterQuiver.HaveArrows()) {
					this.MonsterQuiver.UseArrow();
					attackAmount = this.MonsterWeapon.Attack();
				}
				if (this.MonsterWeapon.Equipped &&
				    this.MonsterWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    !this.MonsterQuiver.HaveArrows()) {
					attackAmount = 5;
				}
			}
			catch (NullReferenceException) {
				var monsterDisarmed = "The " + this.Name + " is disarmed! They are going hand to hand!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					monsterDisarmed);
				attackAmount = 5;
			}
			var randomChanceToHit = GameHandler.GetRandomNumber(1, 100);
			var chanceToDodge = player.DodgeChance;
			if (chanceToDodge > 50) chanceToDodge = 50;
			foreach (var effect in player.Effects) {
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
						var frozenAttackAmount = attackAmount * effect.EffectMultiplier;
						attackAmount = (int)frozenAttackAmount;
						effect.FrozenRound(player);
						break;
					case Effect.EffectType.ReflectDamage:
						break;
					case Effect.EffectType.ChangeStat:
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						break;
					case Effect.EffectType.BlockDamage:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				GameHandler.RemovedExpiredEffects(this);
			}
			return randomChanceToHit <= chanceToDodge ? 0 : attackAmount;
		}
		public int ArmorRating(Player player) {
			var totalArmorRating = MonsterHandler.CheckArmorRating(this);
			var levelDiff = player.Level - this.Level;
			var armorMultiplier = 1.00 + -(double)levelDiff / 5;
			var adjArmorRating = totalArmorRating * armorMultiplier;
			if (this.ElementalCategory != null) adjArmorRating += 20;
			return (int)adjArmorRating;
		}
		public void MonsterDeath(Player player) {
			player.InCombat = false;
			this.InCombat = false;
			this.Effects.Clear();
			var defeatString = "You have defeated the " + this.Name + "!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				defeatString);
			var expGainString = "You have gained " + this.ExperienceProvided + " experience!";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				expGainString);
			foreach (var loot in this.MonsterItems) {
				loot.Equipped = false;
			}
			this.Name = "Dead " + this.Name;
			this.Desc = "A corpse of a monster you killed.";
			player.Experience += this.ExperienceProvided;
			PlayerHandler.LevelUpCheck(player);
		}
	}
}