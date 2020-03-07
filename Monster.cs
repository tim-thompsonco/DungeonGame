using System.Collections.Generic;

namespace DungeonGame {
	public class Monster : IRoomInteraction {
		public enum MonsterType {
			Skeleton,
			Zombie,
			Spider,
			Demon
		}
		public string Name { get; set; }
		public string Desc { get; set; }
		public int Level { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitPoints { get; set; }
		public int ExperienceProvided { get; set; }
		public int Gold { get; set; }
		public bool WasLooted { get; set; }
		public bool InCombat { get; set; }
		public bool IsStunned { get; set; }
		public int StatReplenishInterval { get; set; }
		public MonsterType MonsterCategory { get; set; }
		public Weapon MonsterWeapon { get; set; }
		public Armor MonsterHeadArmor { get; set; }
		public Armor MonsterBackArmor { get; set; }
		public Armor MonsterChestArmor { get; set; }
		public Armor MonsterWristArmor { get; set; }
		public Armor MonsterHandsArmor { get; set; }
		public Armor MonsterWaistArmor { get; set; }
		public Armor MonsterLegArmor { get; set; }
		public List<IEquipment> MonsterItems { get; set; }
		public List<Effect> Effects { get; set; }

		public Monster(int level, MonsterType monsterType) {
			this.MonsterItems = new List<IEquipment>();
			this.Effects = new List<Effect>();
			this.StatReplenishInterval = 3;
			this.Level = level;
			this.MonsterCategory = monsterType;
			this.Name = "placeholder";
			this.Desc = "placeholder";
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
		}

		public void BuildMonster() {
			MonsterBuilder.BuildMonsterNameDesc(this);
			MonsterBuilder.BuildMonsterGear(this);
		}
		public void TakeDamage(int weaponDamage) {
			this.HitPoints -= weaponDamage;
		}
		public void DisplayStats() {
			var opponentHealthString = "Opponent HP: " + this.HitPoints + " / " + this.MaxHitPoints;
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				opponentHealthString);
			var healLineOutput = new List<string>();
			var hitPointMaxUnits = this.MaxHitPoints / 10;
			var hitPointUnits = this.HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++) {
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputHandler.Display.StoreUserOutput(healLineOutput);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"==================================================");
		}
		public int Attack(Player player) {
			var attackDamage = this.MonsterWeapon.Attack();
			var randomChanceToHit = GameHandler.GetRandomNumber(1, 100);
			var chanceToDodge = player.DodgeChance;
			if (chanceToDodge > 50) chanceToDodge = 50;
			return randomChanceToHit <= chanceToDodge ? 0 : attackDamage;
		}
		private int CheckArmorRating() {
			var totalArmorRating = 0;
			if (this.MonsterChestArmor != null && this.MonsterChestArmor.Equipped) {
				totalArmorRating += this.MonsterChestArmor.ArmorRating;
			}
			if (this.MonsterHeadArmor != null && this.MonsterHeadArmor.Equipped) {
				totalArmorRating += this.MonsterHeadArmor.ArmorRating;
			}
			if (this.MonsterLegArmor != null && this.MonsterLegArmor.Equipped) {
				totalArmorRating += this.MonsterLegArmor.ArmorRating;
			}
			return totalArmorRating;
		}
		public int ArmorRating(Player player) {
			var totalArmorRating = this.CheckArmorRating();
			var levelDiff = player.Level - this.Level;
			var armorMultiplier = 1.00 + -(double)levelDiff / 5;
			var adjArmorRating = totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public bool IsMonsterDead(Player player) {
			if (this.HitPoints <= 0) this.MonsterDeath(player);
			return this.HitPoints <= 0;
		}
		private void MonsterDeath(Player player) {
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
			player.GainExperience(this.ExperienceProvided);
			PlayerHandler.LevelUpCheck(player);
		}
	}
}