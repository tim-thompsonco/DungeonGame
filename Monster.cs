using System;
using System.Collections.Generic;

namespace DungeonGame {
	public class Monster : IMonster {
		private static readonly Random RndGenerate = new Random();
		public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitPoints { get; set; }
		public int ExperienceProvided { get; set; }
		public int Gold { get; set; }
		public bool IsStunned { get; set; }
		public int StunnedCurRound { get; set; }
		public int StunnedMaxRound { get; set; }
		public bool IsBleeding { get; set; }
		public int BleedDamage { get; set; }
		public int BleedCurRound { get; set; }
		public int BleedMaxRound { get; set; }
		public bool IsFrozen { get; set; }
		public int FrozenCurRound { get; set; }
		public int FrozenMaxRound { get; set; }
		public bool OnFire { get; set; }
		public int OnFireDamage { get; set; }
		public int OnFireCurRound { get; set; }
		public int OnFireMaxRound { get; set; }
		public bool WasLooted { get; set; }
		public List<IEquipment> MonsterItems { get; set; }
		public Loot Item { get; set; }
		public Consumable Consumable { get; set; }
		public Weapon MonsterWeapon { get; set; }
		public Armor MonsterChestArmor { get; set; }
		public Armor MonsterHeadArmor { get; set; }
		public Armor MonsterLegArmor { get; set; }

		public Monster(string name, string desc, int level, int goldCoins, int maxHp, int expProvided, Weapon weapon) {
			this.MonsterItems = new List<IEquipment>();
			this.Name = name;
			this.Desc = desc;
			this.Level = level;
			this.Gold = goldCoins;
			this.MaxHitPoints = maxHp;
			this.HitPoints = maxHp;
			this.ExperienceProvided = expProvided;
			this.MonsterWeapon = weapon;
			this.MonsterItems.Add((IEquipment)this.MonsterWeapon);
		}
		public Monster(string name, string desc, int level, int goldCoins, int maxHp, int expProvided, Weapon weapon, Loot item)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.Item = item;
			this.MonsterItems.Add((IEquipment)this.Item);
		}
		public Monster(string name, string desc, int level, int goldCoins, int maxHp, int expProvided, Weapon weapon, Armor armor)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.MonsterChestArmor = armor;
			this.MonsterItems.Add((IEquipment)this.MonsterChestArmor);
		}
		public Monster(string name, string desc, int level, int goldCoins, int maxHp, int expProvided, Weapon weapon, Armor armor, Consumable consumable)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.MonsterChestArmor = armor;
			this.Consumable = consumable;
			this.MonsterItems.Add((IEquipment)this.MonsterChestArmor);
			this.MonsterItems.Add((IEquipment)this.Consumable);
		}
		
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public void DisplayStats() {
			Helper.FormatGeneralInfoText();
			Console.WriteLine("Opponent HP: {0} / {1}", this.HitPoints, this.MaxHitPoints);
			Console.WriteLine("==================================================");
		}
		public int Attack() {
			return this.MonsterWeapon.Attack();
		}
		public int CheckArmorRating() {
			var totalArmorRating = 0;
			if (this.MonsterChestArmor != null && this.MonsterChestArmor.IsEquipped()) {
				totalArmorRating += this.MonsterChestArmor.ArmorRating;
			}
			if (this.MonsterHeadArmor != null && this.MonsterHeadArmor.IsEquipped()) {
				totalArmorRating += this.MonsterHeadArmor.ArmorRating;
			}
			if (this.MonsterLegArmor != null && this.MonsterLegArmor.IsEquipped()) {
				totalArmorRating += this.MonsterLegArmor.ArmorRating;
			}
			return totalArmorRating;
		}
		public int ArmorRating(Player player) {
			var totalArmorRating = this.CheckArmorRating();
			var levelDiff = player.Level - this.Level;
			var armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public string GetName() {
			return this.Name.ToString();
		}
		public bool IsEquipped() {
			return false;
		}
		public void SetOnFire(bool onFire, int onFireDamage, int onFireCurRound, int onFireMaxRound) {
			this.OnFire = onFire;
			this.OnFireDamage = onFireDamage;
			this.OnFireCurRound = onFireCurRound;
			this.OnFireMaxRound = onFireMaxRound;
		}
		public void BurnOnFire() {
			this.HitPoints -= this.OnFireDamage;
			Helper.FormatOnFireText();
			Console.WriteLine("The {0} burns for {1} fire damage.", this.Name, this.OnFireDamage);
			this.OnFireCurRound += 1;
			if (this.OnFireCurRound <= this.OnFireMaxRound) return;
			this.OnFire = false;
			this.OnFireCurRound = 1;
		}
		public void StartBleeding(bool bleeding, int bleedDamage, int bleedCurRound, int bleedMaxRound) {
			this.IsBleeding = bleeding;
			this.BleedDamage = bleedDamage;
			this.BleedCurRound = bleedCurRound;
			this.BleedMaxRound = bleedMaxRound;
		}
		public void Bleeding() {
			Helper.FormatAttackSuccessText();
			this.HitPoints -= this.BleedDamage;
			Console.WriteLine("The {0} bleeds for {1} physical damage.", this.Name, this.BleedDamage);
			this.BleedCurRound += 1;
			if (this.BleedCurRound <= this.BleedMaxRound) return;
			this.IsBleeding = false;
			this.BleedCurRound = 1;
		}
		public void StartStunned(bool stunned, int stunCurRound, int stunMaxRound) {
			this.IsStunned = stunned;
			this.StunnedCurRound = stunCurRound;
			this.StunnedMaxRound = stunMaxRound;
		}
		public void Stunned() {
			this.StunnedCurRound += 1;
			Console.WriteLine("The {0} is stunned and cannot attack.", this.Name);
			if (this.StunnedCurRound <= this.StunnedMaxRound) return;
			this.IsStunned = false;
			this.StunnedCurRound = 1;
		}
		public void StartFrozen(bool frozen, int frozenCurRound, int frozenMaxRound) {
			this.IsFrozen = frozen;
			this.FrozenCurRound = frozenCurRound;
			this.FrozenMaxRound = frozenMaxRound;
		}
		public void Frozen() {
			this.FrozenCurRound += 1;
			Console.WriteLine("The {0} is frozen and cannot attack.", this.Name);
			if (this.FrozenCurRound <= this.FrozenMaxRound) return;
			this.IsFrozen = false;
			this.FrozenCurRound = 1;
		}
	}
}