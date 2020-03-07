using System;

namespace DungeonGame {
	public static class MonsterBuilder {
		public static void BuildMonsterGear(Monster monster) {
			var randomGearNum = GameHandler.GetRandomNumber(1, 10);
			switch (monster.MonsterCategory) {
				case Monster.MonsterType.Skeleton:
					monster.MonsterWeapon = randomGearNum switch {
						1 => 
						monster.MonsterWeapon = new Weapon(monster.Level, Weapon.WeaponType.Axe, monster.MonsterCategory),
						2 => 
						monster.MonsterWeapon = new Weapon(
							monster.Level, Weapon.WeaponType.TwoHandedSword, monster.MonsterCategory),
						_ => 
						monster.MonsterWeapon = new Weapon(
							monster.Level, Weapon.WeaponType.OneHandedSword, monster.MonsterCategory)
					};
					BuildMonsterArmor(monster);
					if (randomGearNum <= 4) {
						var randomPotionNum = GameHandler.GetRandomNumber(1, 6);
						monster.MonsterItems.Add(randomPotionNum switch {
							1 => new Consumable(monster.Level, Consumable.PotionType.Health),
							2 => new Consumable(monster.Level, Consumable.PotionType.Mana),
							3 => new Consumable(monster.Level, Consumable.PotionType.Constitution),
							4 => new Consumable(monster.Level, Consumable.PotionType.Dexterity),
							5 => new Consumable(monster.Level, Consumable.PotionType.Intelligence),
							6 => new Consumable(monster.Level, Consumable.PotionType.Strength),
							_ => throw new ArgumentOutOfRangeException()
						});
					}
					break;
				case Monster.MonsterType.Zombie:
					monster.MonsterWeapon = new Weapon(monster.Level, Weapon.WeaponType.Axe, monster.MonsterCategory);
					BuildMonsterArmor(monster);
					break;
				case Monster.MonsterType.Spider:
					monster.MonsterWeapon = new Weapon(monster.Level, Weapon.WeaponType.Dagger, monster.MonsterCategory);
					if (randomGearNum <= 5) monster.MonsterItems.Add(new Loot("large venom sac", monster.Level, 1));
					break;
				case Monster.MonsterType.Demon:
					monster.MonsterWeapon = randomGearNum switch {
						1 => 
						monster.MonsterWeapon = new Weapon(
							monster.Level, Weapon.WeaponType.OneHandedSword, monster.MonsterCategory),
						2 => 
						monster.MonsterWeapon = new Weapon(
							monster.Level, Weapon.WeaponType.TwoHandedSword, monster.MonsterCategory),
						_ => 
						monster.MonsterWeapon = new Weapon(monster.Level, Weapon.WeaponType.Axe, monster.MonsterCategory) 
					};
					if (randomGearNum <= 2) {
						BuildMonsterKit(monster);
					}
					if (randomGearNum <= 3) {
						BuildMonsterGem(monster);
					}
					BuildMonsterArmor(monster);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			monster.MonsterWeapon.Equipped = true;
			monster.MonsterItems.Add(monster.MonsterWeapon);
		}
		public static void BuildMonsterGem(Monster monster) {
			var randomGemNum = GameHandler.GetRandomNumber(1, 6);
			switch (randomGemNum) {
				case 1:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Amethyst));
					break;
				case 2:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Diamond));
					break;
				case 3:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Emerald));
					break;
				case 4:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Ruby));
					break;
				case 5:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Sapphire));
					break;
				case 6:
					monster.MonsterItems.Add(new Loot(monster.Level, Loot.GemType.Topaz));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static void BuildMonsterKit(Monster monster) {
			var kitRandomNum = GameHandler.GetRandomNumber(1, 2);
			var kitCategory = kitRandomNum switch {
				1 => Consumable.KitType.Armor,
				2 => Consumable.KitType.Weapon,
				_ => throw new ArgumentOutOfRangeException()};
			var kitTypeRandomNum = GameHandler.GetRandomNumber(1, 3);
			var kitType = kitTypeRandomNum switch {
				1 => ChangeArmor.KitType.Cloth,
				2 => ChangeArmor.KitType.Leather,
				3 => ChangeArmor.KitType.Plate,
				_ => throw new ArgumentOutOfRangeException()};
			var kitLevelRandomNum = GameHandler.GetRandomNumber(1, 3);
			var kitLevel = kitLevelRandomNum switch {
				1 => Consumable.KitLevel.Light,
				2 => Consumable.KitLevel.Medium,
				3 => Consumable.KitLevel.Heavy,
				_ => throw new ArgumentOutOfRangeException()};
			monster.MonsterItems.Add(new Consumable(kitLevel, kitCategory, kitType));
		}
		public static void BuildMonsterArmor(Monster monster) {
			var randomCatNum = GameHandler.GetRandomNumber(1, 7);
			switch(randomCatNum){
				case 1:
					monster.MonsterBackArmor = new Armor(monster.Level, Armor.ArmorSlot.Back);
					monster.MonsterBackArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterBackArmor);
					break;
				case 2:
					monster.MonsterChestArmor = new Armor(monster.Level, Armor.ArmorSlot.Chest);
					monster.MonsterChestArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterChestArmor);
					break;
				case 3:
					monster.MonsterHeadArmor = new Armor(monster.Level, Armor.ArmorSlot.Head);
					monster.MonsterHeadArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterHeadArmor);
					break;
				case 4:
					monster.MonsterLegArmor = new Armor(monster.Level, Armor.ArmorSlot.Legs);
					monster.MonsterLegArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterLegArmor);
					break;
				case 5:
					monster.MonsterWaistArmor = new Armor(monster.Level, Armor.ArmorSlot.Waist);
					monster.MonsterWaistArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterWaistArmor);
					break;
				case 6:
					monster.MonsterWristArmor = new Armor(monster.Level, Armor.ArmorSlot.Wrist);
					monster.MonsterWristArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterWristArmor);
					break;
				case 7:
					monster.MonsterHandsArmor = new Armor(monster.Level, Armor.ArmorSlot.Hands);
					monster.MonsterHandsArmor.Equipped = true;
					monster.MonsterItems.Add(monster.MonsterHandsArmor);
					break;
			}
		}
		public static void BuildMonsterNameDesc(Monster monster) {
			switch (monster.MonsterCategory) {
				case Monster.MonsterType.Skeleton:
					monster.Name = monster.Level switch {
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
					monster.Desc =
						"A " + monster.Name + " stands in front of you. Its bones look worn and damaged from years of fighting. A " +
						"ghastly yellow glow surrounds it, which is the only indication of the magic that must exist to " +
						"reanimate it.";
					break;
				case Monster.MonsterType.Zombie:
					monster.Name = monster.Level switch {
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
					monster.Desc =
						"A " + monster.Name +
						" stares at you, it's face frozen in a look of indifference to the fact a bug is crawling" +
						" out of it's empty eye sockets. In one hand, it drags a weapon against the ground, as it stares at you " +
						"menacingly. Bones, muscle and tendons are visible through many gashes and tears in it's rotting skin.";
					break;
				case Monster.MonsterType.Spider:
					monster.Name = monster.Level switch {
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
					monster.Desc =
						"A " + monster.Name + " about the size of a large bear skitters down the corridor towards you. " +
						"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
						"you, legs ending in sharp claws carrying it closer as it hisses hungrily.";
					break;
				case Monster.MonsterType.Demon:
					monster.Name = monster.Level switch {
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
					monster.Desc =
						"A " + monster.Name + " stands before you with two horns sticking out of it's head. It's eyes glint " +
						"yellow and a look of pure hatred adorns its face. Leathery wings spread out on either side of its " +
						"back as it rises up to its full height and growls at you.";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}