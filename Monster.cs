using System;
using System.Collections.Generic;
using System.Linq;

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
		public int UnarmedAttackDamage { get; set; }
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

		// Default constructor for JSON serialization
		public Monster() {}
		public Monster(int level, MonsterType monsterType) {
			this.MonsterItems = new List<IEquipment>();
			this.Effects = new List<Effect>();
			this.StatReplenishInterval = 3;
			this.UnarmedAttackDamage = 5;
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
			var expProvided = this.MaxHitPoints;
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
					var randomNum = GameHandler.GetRandomNumber(1, 3);
					var randomPhysicalDmg = GameHandler.GetRandomNumber(20, 26);
					this.UnarmedAttackDamage = randomNum switch {
						1 => randomPhysicalDmg + (level - 1) * 1,
						2 => randomPhysicalDmg + (level - 1) * 2,
						3 => randomPhysicalDmg + (level - 1) * 3,
						_ => throw new ArgumentOutOfRangeException()
					};
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
		
		public AttackOption DetermineAttack(Player player) {
			var attackOptions = new List<AttackOption>();
			if (this.MonsterWeapon != null && this.MonsterWeapon.Equipped) {
				attackOptions.Add(new 
					AttackOption(AttackOption.AttackType.Physical, 
						this.MonsterWeapon.RegDamage - player.ArmorRating(this), -1));
			}
			else {
				attackOptions.Add(new 
					AttackOption(AttackOption.AttackType.Physical, this.UnarmedAttackDamage, -1));
			}
			if (this.Spellbook != null) {
				for (var i = 0; i < this.Spellbook.Count; i++) {
					if (this.EnergyPoints < this.Spellbook[i].EnergyCost) continue;
					switch (this.Spellbook[i].SpellCategory) {
						case MonsterSpell.SpellType.Fireball:
						case MonsterSpell.SpellType.Frostbolt:
						case MonsterSpell.SpellType.Lightning:
							var spellTotalDamage = 0;
							if (this.Spellbook[i].Offensive.AmountOverTime == 0) {
								spellTotalDamage = this.Spellbook[i].Offensive.Amount;
							}
							else {
								spellTotalDamage = this.Spellbook[i].Offensive.Amount + this.Spellbook[i].Offensive.AmountOverTime *
									this.Spellbook[i].Offensive.AmountMaxRounds;
							}
							attackOptions.Add(new 
								AttackOption(AttackOption.AttackType.Spell, spellTotalDamage, i));
							break;
						case MonsterSpell.SpellType.Heal:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			if (this.Abilities != null) {
				for (var i = 0; i < this.Abilities.Count; i++) {
					if (this.EnergyPoints < this.Abilities[i].EnergyCost) continue;
					switch (this.Abilities[i].AbilityCategory) {
						case MonsterAbility.Ability.PoisonBite:
						case MonsterAbility.Ability.BloodLeech:
						case MonsterAbility.Ability.TailWhip:
							var abilityTotalDamage = 0;
							if (this.Abilities[i].Offensive.AmountOverTime == 0) {
								abilityTotalDamage = this.Abilities[i].Offensive.Amount * 2;
							}
							else {
								abilityTotalDamage = this.Abilities[i].Offensive.Amount + this.Abilities[i].Offensive.AmountOverTime *
									this.Abilities[i].Offensive.AmountMaxRounds;
							}
							attackOptions.Add(new 
								AttackOption(AttackOption.AttackType.Ability, abilityTotalDamage, i));
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			attackOptions = attackOptions.OrderByDescending(attack => attack.DamageAmount).ToList();
			return attackOptions[0];
		}
		public void Attack(Player player) {
			var attackOption = this.DetermineAttack(player);
			switch (attackOption.AttackCategory) {
				case AttackOption.AttackType.Physical:
					this.PhysicalAttack(player);
					break;
				case AttackOption.AttackType.Spell:
					switch (this.Spellbook[attackOption.AttackIndex].SpellCategory) {
						case MonsterSpell.SpellType.Fireball:
							MonsterSpell.CastFireOffense(this, player, attackOption.AttackIndex);
							break;
						case MonsterSpell.SpellType.Frostbolt:
							MonsterSpell.CastFrostOffense(this, player, attackOption.AttackIndex);
							break;
						case MonsterSpell.SpellType.Lightning:
							MonsterSpell.CastArcaneOffense(this, player, attackOption.AttackIndex);
							break;
						case MonsterSpell.SpellType.Heal:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case AttackOption.AttackType.Ability:
					switch (this.Abilities[attackOption.AttackIndex].AbilityCategory) {
						case MonsterAbility.Ability.PoisonBite:
						case MonsterAbility.Ability.TailWhip:
							MonsterAbility.UseOffenseDamageAbility(this, player, attackOption.AttackIndex);
							break;
						case MonsterAbility.Ability.BloodLeech:
							MonsterAbility.UseBloodLeechAbility(this, player, attackOption.AttackIndex);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			GearHandler.DecreaseArmorDurability(player);
		}
		private void PhysicalAttack(Player player) {
			var attackAmount = 10;
			try {
				if (this.MonsterWeapon.Equipped && this.MonsterWeapon.WeaponGroup != Weapon.WeaponType.Bow) {
					attackAmount += this.MonsterWeapon.Attack();
				}
				if (this.MonsterWeapon.Equipped &&
				    this.MonsterWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    this.MonsterQuiver.HaveArrows()) {
					this.MonsterQuiver.UseArrow();
					attackAmount += this.MonsterWeapon.Attack();
				}
				if (this.MonsterWeapon.Equipped &&
				    this.MonsterWeapon.WeaponGroup == Weapon.WeaponType.Bow &&
				    !this.MonsterQuiver.HaveArrows()) {
					attackAmount += this.UnarmedAttackDamage;
				}
			}
			catch (NullReferenceException) {
				if (this.MonsterCategory == MonsterType.Elemental) {
					attackAmount += this.UnarmedAttackDamage;
				}
				else {
					var monsterDisarmed = "The " + this.Name + " is disarmed! They are going hand to hand!";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						monsterDisarmed);
					attackAmount += this.UnarmedAttackDamage;
				}
			}
			var randomChanceToHit = GameHandler.GetRandomNumber(1, 100);
			var chanceToDodge = player.DodgeChance;
			if (chanceToDodge > 50) chanceToDodge = 50;
			if (randomChanceToHit <= chanceToDodge) {
				var missString = "The " + this.Name + " missed you!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					missString);
				return;
			}
			var baseAttackAmount = attackAmount;
			foreach (var effect in player.Effects.ToList()) {
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
						effect.IsEffectExpired = true;
						break;
					case Effect.EffectType.ChangeOpponentDamage:
						var changeDamageAmount = effect.EffectAmountOverTime < attackAmount ? 
							effect.EffectAmountOverTime : attackAmount;
						effect.ChangeOpponentDamageRound(player);
						attackAmount += changeDamageAmount;
						break;
					case Effect.EffectType.BlockDamage:
						var blockAmount = effect.EffectAmount < attackAmount ?
							effect.EffectAmount : attackAmount;
						effect.BlockDamageRound(blockAmount);
						attackAmount -= blockAmount;
						break;
					case Effect.EffectType.ReflectDamage:
						var reflectAmount = effect.EffectAmountOverTime < attackAmount ? 
							effect.EffectAmountOverTime : attackAmount;
						this.HitPoints -= reflectAmount;
						effect.ReflectDamageRound(reflectAmount);
						attackAmount -= reflectAmount;
						break;
					case Effect.EffectType.ChangeStat:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (baseAttackAmount > attackAmount && attackAmount - player.ArmorRating(this) <= 0) {
					var effectAbsorbString = "Your " + effect.Name + " absorbed all of " + this.Name + "'s attack!"; 
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatAttackFailText(),
						Settings.FormatDefaultBackground(),
						effectAbsorbString);
					return;
				} 
				GameHandler.RemovedExpiredEffects(this);
			}
			if (attackAmount- player.ArmorRating(this) <= 0) {
				var armorAbsorbString = "Your armor absorbed all of " + this.Name + "'s attack!"; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackFailText(),
					Settings.FormatDefaultBackground(),
					armorAbsorbString);
			}
			else if (attackAmount - player.ArmorRating(this) > 0) {
				attackAmount -= player.ArmorRating(this);
				var hitString = "The " + this.Name + " hits you for " + attackAmount + " physical damage.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatAttackSuccessText(),
					Settings.FormatDefaultBackground(),
					hitString);
				player.HitPoints -= attackAmount;
			}
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