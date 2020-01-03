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

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Monster() {}
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
		public Monster(
			string name, 
			string desc,
			int level,
			int goldCoins,
			int maxHp,
			int expProvided, 
			Weapon weapon, 
			Loot item)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.Item = item;
			this.MonsterItems.Add((IEquipment)this.Item);
		}
		public Monster(
			string name,
			string desc,
			int level,
			int goldCoins,
			int maxHp, 
			int expProvided,
			Weapon weapon, 
			Armor armor)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.MonsterChestArmor = armor;
			this.MonsterItems.Add((IEquipment)this.MonsterChestArmor);
		}
		public Monster(
			string name, 
			string desc, 
			int level,
			int goldCoins,
			int maxHp,
			int expProvided,
			Weapon weapon, 
			Armor armor, 
			Consumable consumable)
			: this(name, desc, level, goldCoins, maxHp, expProvided, weapon) {
			this.MonsterChestArmor = armor;
			this.Consumable = consumable;
			this.MonsterItems.Add((IEquipment)this.MonsterChestArmor);
			this.MonsterItems.Add((IEquipment)this.Consumable);
		}
		
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public void DisplayStats(UserOutput output) {
			var opponentStats = "Opponent HP: " + this.HitPoints + " / " + this.MaxHitPoints;
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				opponentStats);
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(),
				Helper.FormatDefaultBackground(),
				"==================================================");
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
			return this.Name;
		}
		public void SetOnFire(bool onFire, int onFireDamage, int onFireCurRound, int onFireMaxRound) {
			this.OnFire = onFire;
			this.OnFireDamage = onFireDamage;
			this.OnFireCurRound = onFireCurRound;
			this.OnFireMaxRound = onFireMaxRound;
		}
		public void StartBleeding(bool bleeding, int bleedDamage, int bleedCurRound, int bleedMaxRound) {
			this.IsBleeding = bleeding;
			this.BleedDamage = bleedDamage;
			this.BleedCurRound = bleedCurRound;
			this.BleedMaxRound = bleedMaxRound;
		}
		public void StartStunned(bool stunned, int stunCurRound, int stunMaxRound) {
			this.IsStunned = stunned;
			this.StunnedCurRound = stunCurRound;
			this.StunnedMaxRound = stunMaxRound;
		}
		public void Stunned(UserOutput output) {
			this.StunnedCurRound += 1;
			var stunnedString = "The " + this.Name + " is stunned and cannot attack.";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				stunnedString);
			if (this.StunnedCurRound <= this.StunnedMaxRound) return;
			this.IsStunned = false;
			this.StunnedCurRound = 1;
		}
		public bool IsMonsterDead(Player player, UserOutput output) {
			if (this.HitPoints <= 0) this.MonsterDeath(player, output);
			return this.HitPoints <= 0;
		}
		public void MonsterDeath(Player player, UserOutput output) {
			var defeatString = "You have defeated the " + this.Name + "!";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				defeatString);
			foreach (var loot in this.MonsterItems) {
				loot.Equipped = false;
			}
			this.Name = "Dead " + this.GetName();
			this.Desc = "A corpse of a monster you killed.";
			player.GainExperience(this.ExperienceProvided);
			PlayerHelper.LevelUpCheck(player, output);
		}
	}
}