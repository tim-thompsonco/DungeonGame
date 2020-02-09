using System;
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
		public Loot Item { get; set; }
		public Consumable Consumable { get; set; }
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
			this.BuildMonsterNameDesc();
			var randomNumHitPoint = GameHandler.GetRandomNumber(20, 40);
			var maxHitPoints = 80 + ((this.Level - 1) * randomNumHitPoint);
			this.MaxHitPoints = GameHandler.RoundNumber(maxHitPoints);
			this.HitPoints = this.MaxHitPoints;
			if (this.MonsterCategory == MonsterType.Spider) {
				this.Gold = 0;
			}
			else {
				var randomNumGold = GameHandler.GetRandomNumber(5, 10);
				this.Gold = 10 + ((this.Level - 1) * randomNumGold);
			}
			var randomNumExp = GameHandler.GetRandomNumber(20, 40);
			var expProvided = this.MaxHitPoints + randomNumExp;
			this.ExperienceProvided = GameHandler.RoundNumber(expProvided);
			this.BuildMonsterGear();
		}

		private void BuildMonsterGear() {
			var randomGearNum = GameHandler.GetRandomNumber(1, 10);
			switch (this.MonsterCategory) {
				case MonsterType.Skeleton:
					this.MonsterWeapon = randomGearNum switch {
						1 => 
						this.MonsterWeapon = new Weapon(this.Level, Weapon.WeaponType.Axe, this.MonsterCategory),
						2 => 
						this.MonsterWeapon = new Weapon(
							this.Level, Weapon.WeaponType.TwoHandedSword, this.MonsterCategory),
						_ => 
						this.MonsterWeapon = new Weapon(
							this.Level, Weapon.WeaponType.OneHandedSword, this.MonsterCategory)
					};
					this.BuildMonsterArmor();
					if (randomGearNum <= 4) {
						var randomPotionNum = GameHandler.GetRandomNumber(1, 10);
						this.MonsterItems.Add(randomPotionNum <= 5
							? new Consumable(this.Level, Consumable.PotionType.Health)
							: new Consumable(this.Level, Consumable.PotionType.Mana));
					}
					break;
				case MonsterType.Zombie:
					this.MonsterWeapon = new Weapon(this.Level, Weapon.WeaponType.Axe, this.MonsterCategory);
					this.BuildMonsterArmor();
					break;
				case MonsterType.Spider:
					this.MonsterWeapon = new Weapon(this.Level, Weapon.WeaponType.Dagger, this.MonsterCategory);
					if (randomGearNum <= 5) this.MonsterItems.Add(new Loot("large venom sac", this.Level, 1));
					break;
				case MonsterType.Demon:
					this.MonsterWeapon = randomGearNum switch {
						1 => 
						this.MonsterWeapon = new Weapon(
							this.Level, Weapon.WeaponType.OneHandedSword, this.MonsterCategory),
						2 => 
						this.MonsterWeapon = new Weapon(
							this.Level, Weapon.WeaponType.TwoHandedSword, this.MonsterCategory),
						_ => 
						this.MonsterWeapon = new Weapon(this.Level, Weapon.WeaponType.Axe, this.MonsterCategory) 
					};
					if (randomGearNum <= 3) {
						this.BuildMonsterGem();
					}
					this.BuildMonsterArmor();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.MonsterWeapon.Equipped = true;
			this.MonsterItems.Add(this.MonsterWeapon);
		}
		private void BuildMonsterGem() {
			var randomGemNum = GameHandler.GetRandomNumber(1, 6);
			switch (randomGemNum) {
				case 1:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Amethyst));
					break;
				case 2:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Diamond));
					break;
				case 3:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Emerald));
					break;
				case 4:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Ruby));
					break;
				case 5:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Sapphire));
					break;
				case 6:
					this.MonsterItems.Add(new Consumable(this.Level, Consumable.GemType.Topaz));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void BuildMonsterArmor() {
			var randomCatNum = GameHandler.GetRandomNumber(1, 7);
			switch(randomCatNum){
				case 1:
					this.MonsterBackArmor = new Armor(this.Level, Armor.ArmorSlot.Back);
					this.MonsterBackArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterBackArmor);
					break;
				case 2:
					this.MonsterChestArmor = new Armor(this.Level, Armor.ArmorSlot.Chest);
					this.MonsterChestArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterChestArmor);
					break;
				case 3:
					this.MonsterHeadArmor = new Armor(this.Level, Armor.ArmorSlot.Head);
					this.MonsterHeadArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterHeadArmor);
					break;
				case 4:
					this.MonsterLegArmor = new Armor(this.Level, Armor.ArmorSlot.Legs);
					this.MonsterLegArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterLegArmor);
					break;
				case 5:
					this.MonsterWaistArmor = new Armor(this.Level, Armor.ArmorSlot.Waist);
					this.MonsterWaistArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterWaistArmor);
					break;
				case 6:
					this.MonsterWristArmor = new Armor(this.Level, Armor.ArmorSlot.Wrist);
					this.MonsterWristArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterWristArmor);
					break;
				case 7:
					this.MonsterHandsArmor = new Armor(this.Level, Armor.ArmorSlot.Hands);
					this.MonsterHandsArmor.Equipped = true;
					this.MonsterItems.Add(this.MonsterHandsArmor);
					break;
			}
		}
		private void BuildMonsterNameDesc() {
			switch (this.MonsterCategory) {
				case MonsterType.Skeleton:
					this.Name = this.Level switch {
						1 => "skeleton",
						2 => "skeleton fighter",
						3 => "skeleton warrior",
						4 => "skeleton guardian",
						5 => "skeleton defender",
						6 => "skeleton conqueror",
						7 => "skeleton zealot",
						8 => "skeleton gladiator",
						9 => "skeleton knight",
						10 => "skeleton champion",
						_ => "skeleton placeholder"
					};
					this.Desc =
						"A " + this.Name + " stands in front of you. Its bones look worn and damaged from years of fighting. A " +
						"ghastly yellow glow surrounds it, which is the only indication of the magic that must exist to " +
						"reanimate it.";
					break;
				case MonsterType.Zombie:
					this.Name = this.Level switch {
						1 => "zombie",
						2 => "rotting zombie",
						3 => "vicious zombie",
						4 => "rabid zombie",
						5 => "crazed zombie",
						6 => "frenzied zombie",
						7 => "virulent zombie",
						8 => "delirious zombie",
						9 => "furious zombie",
						10 => "fanatical zombie",
						_ => "zombie placeholder"
					};
					this.Desc =
						"A " + this.Name +
						" stares at you, it's face frozen in a look of indifference to the fact a bug is crawling" +
						" out of it's empty eye sockets. In one hand, it drags a weapon against the ground, as it stares at you " +
						"menacingly. Bones, muscle and tendons are visible through many gashes and tears in it's rotting skin.";
					break;
				case MonsterType.Spider:
					this.Name = this.Level switch {
						1 => "spider",
						2 => "black spider",
						3 => "huge spider",
						4 => "ghoulish spider",
						5 => "menacing spider",
						6 => "sinister spider",
						7 => "macabre spider",
						8 => "gruesome spider",
						9 => "hideous spider",
						10 => "abominable spider",
						_ => "spider placeholder"
					};
					this.Desc =
						"A " + this.Name + " about the size of a large bear skitters down the corridor towards you. " +
						"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
						"you, legs ending in sharp claws carrying it closer as it hisses hungrily.";
					break;
				case MonsterType.Demon:
					this.Name = this.Level switch {
						1 => "lesser demon",
						2 => "demon",
						3 => "horned demon",
						4 => "greater demon",
						5 => "hulking demon",
						6 => "immense demon",
						7 => "massive demon",
						8 => "towering demon",
						9 => "titanic demon",
						10 => "colossal demon",
						_ => "demon placeholder"
					};
					this.Desc =
						"A " + this.Name + " stands before you with two horns sticking out of it's head. It's eyes glint " +
						"yellow and a look of pure hatred adorns its face. Leathery wings spread out on either side of its " +
						"back as it rises up to its full height and growls at you.";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
		public int CheckArmorRating() {
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
			var armorMultiplier = 1.00 + (-(double)levelDiff / 5);
			var adjArmorRating = (double)totalArmorRating * armorMultiplier;
			return (int)adjArmorRating;
		}
		public bool IsMonsterDead(Player player) {
			if (this.HitPoints <= 0) this.MonsterDeath(player);
			return this.HitPoints <= 0;
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
			player.GainExperience(this.ExperienceProvided);
			PlayerHandler.LevelUpCheck(player);
		}
	}
}