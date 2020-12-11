using DungeonGame.Items;
using System;

namespace DungeonGame
{
	public static class MonsterBuilder
	{
		public static void BuildMonster(Monster monster)
		{
			BuildMonsterNameDesc(monster);
			BuildMonsterGear(monster);
		}
		private static void BuildMonsterGear(Monster monster)
		{
			var randomGearNum = GameHandler.GetRandomNumber(1, 10);
			switch (monster._MonsterCategory)
			{
				case Monster.MonsterType.Skeleton:
					switch (monster._SkeletonCategory)
					{
						case Monster.SkeletonType.Warrior:
							var randomWeaponNum = GameHandler.GetRandomNumber(1, 3);
							monster._MonsterWeapon = randomWeaponNum switch
							{
								1 => new Weapon(monster._Level, Weapon.WeaponType.Axe, monster._MonsterCategory),
								2 => new Weapon(monster._Level, Weapon.WeaponType.OneHandedSword, monster._MonsterCategory),
								3 => new Weapon(monster._Level, Weapon.WeaponType.TwoHandedSword, monster._MonsterCategory),
								_ => throw new ArgumentOutOfRangeException()
							};
							break;
						case Monster.SkeletonType.Archer:
							monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Bow, monster._MonsterCategory);
							monster._MonsterQuiver = new Quiver("basic quiver", 50, 50, 15);
							break;
						case Monster.SkeletonType.Mage:
							monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Dagger, monster._MonsterCategory);
							break;
						case null:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					BuildMonsterArmor(monster);
					break;
				case Monster.MonsterType.Zombie:
					if (randomGearNum < 4)
					{
						monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Axe, monster._MonsterCategory);
					}
					else if (randomGearNum < 7)
					{
						monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.OneHandedSword, monster._MonsterCategory);
					}
					else
					{
						monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.TwoHandedSword, monster._MonsterCategory);
					}
					BuildMonsterArmor(monster);
					break;
				case Monster.MonsterType.Spider:
					monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Dagger, monster._MonsterCategory);
					if (randomGearNum <= 5) monster._MonsterItems.Add(new Loot("large venom sac", monster._Level, 1));
					break;
				case Monster.MonsterType.Demon:
					monster._MonsterWeapon = randomGearNum switch
					{
						1 =>
						monster._MonsterWeapon = new Weapon(
							monster._Level, Weapon.WeaponType.OneHandedSword, monster._MonsterCategory),
						2 =>
						monster._MonsterWeapon = new Weapon(
							monster._Level, Weapon.WeaponType.TwoHandedSword, monster._MonsterCategory),
						_ =>
						monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Axe, monster._MonsterCategory)
					};
					if (randomGearNum <= 2)
					{
						BuildMonsterKit(monster);
					}
					if (randomGearNum <= 3)
					{
						BuildMonsterGem(monster);
					}
					BuildMonsterArmor(monster);
					break;
				case Monster.MonsterType.Elemental:
					switch (monster._ElementalCategory)
					{
						case Monster.ElementalType.Fire:
							monster._MonsterItems.Add(new Loot("essence of fire", monster._Level, 1));
							break;
						case Monster.ElementalType.Air:
							monster._MonsterItems.Add(new Loot("essence of air", monster._Level, 1));
							break;
						case Monster.ElementalType.Water:
							monster._MonsterItems.Add(new Loot("essence of water", monster._Level, 1));
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case Monster.MonsterType.Vampire:
					monster._MonsterWeapon = randomGearNum switch
					{
						1 =>
						monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Dagger, monster._MonsterCategory),
						2 =>
						monster._MonsterWeapon = new Weapon(
							monster._Level, Weapon.WeaponType.TwoHandedSword, monster._MonsterCategory),
						3 =>
						monster._MonsterWeapon = new Weapon(
							monster._Level, Weapon.WeaponType.Bow, monster._MonsterCategory),
						_ =>
						monster._MonsterWeapon = new Weapon(
							monster._Level, Weapon.WeaponType.OneHandedSword, monster._MonsterCategory)
					};
					if (monster._MonsterWeapon._WeaponGroup == Weapon.WeaponType.Bow)
					{
						monster._MonsterQuiver = new Quiver("basic quiver", 50, 50, 15);
					}
					BuildMonsterArmor(monster);
					if (randomGearNum <= 3)
					{
						BuildMonsterKit(monster);
					}
					if (randomGearNum <= 2)
					{
						BuildMonsterGem(monster);
					}
					break;
				case Monster.MonsterType.Troll:
					monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Axe, monster._MonsterCategory);
					BuildMonsterArmor(monster);
					var randomPotionNum = GameHandler.GetRandomNumber(1, 6);
					monster._MonsterItems.Add(randomPotionNum switch
					{
						1 => new Consumable(monster._Level, Consumable.PotionType.Health),
						2 => new Consumable(monster._Level, Consumable.PotionType.Mana),
						3 => new Consumable(monster._Level, Consumable.PotionType.Constitution),
						4 => new Consumable(monster._Level, Consumable.PotionType.Dexterity),
						5 => new Consumable(monster._Level, Consumable.PotionType.Intelligence),
						6 => new Consumable(monster._Level, Consumable.PotionType.Strength),
						_ => throw new ArgumentOutOfRangeException()
					});
					break;
				case Monster.MonsterType.Dragon:
					monster._MonsterWeapon = new Weapon(monster._Level, Weapon.WeaponType.Dagger, monster._MonsterCategory);
					if (randomGearNum <= 5) monster._MonsterItems.Add(new Loot("dragonscale", monster._Level, 1));
					if (randomGearNum <= 6)
					{
						BuildMonsterGem(monster);
					}
					if (randomGearNum <= 4)
					{
						BuildMonsterKit(monster);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			if (monster._MonsterCategory != Monster.MonsterType.Elemental) monster._MonsterWeapon._Equipped = true;
			if (monster._MonsterWeapon != null) monster._MonsterItems.Add(monster._MonsterWeapon);
			if (monster._MonsterQuiver == null) return;
			monster._MonsterQuiver._Equipped = true;
			monster._MonsterItems.Add(monster._MonsterQuiver);
		}
		private static void BuildMonsterGem(Monster monster)
		{
			int randomGemNum = GameHandler.GetRandomNumber(1, 6);
			switch (randomGemNum)
			{
				case 1:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Amethyst));
					break;
				case 2:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Diamond));
					break;
				case 3:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Emerald));
					break;
				case 4:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Ruby));
					break;
				case 5:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Sapphire));
					break;
				case 6:
					monster._MonsterItems.Add(new Gem(monster._Level, Gem.GemType.Topaz));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void BuildMonsterKit(Monster monster)
		{
			var kitRandomNum = GameHandler.GetRandomNumber(1, 2);
			var kitCategory = kitRandomNum switch
			{
				1 => Consumable.KitType.Armor,
				2 => Consumable.KitType.Weapon,
				_ => throw new ArgumentOutOfRangeException()
			};
			var kitLevelRandomNum = GameHandler.GetRandomNumber(1, 3);
			var kitLevel = kitLevelRandomNum switch
			{
				1 => Consumable.KitLevel.Light,
				2 => Consumable.KitLevel.Medium,
				3 => Consumable.KitLevel.Heavy,
				_ => throw new ArgumentOutOfRangeException()
			};
			if (kitCategory == Consumable.KitType.Armor)
			{
				var kitTypeRandomNum = GameHandler.GetRandomNumber(1, 3);
				var kitType = kitTypeRandomNum switch
				{
					1 => ChangeArmor.KitType.Cloth,
					2 => ChangeArmor.KitType.Leather,
					3 => ChangeArmor.KitType.Plate,
					_ => throw new ArgumentOutOfRangeException()
				};
				monster._MonsterItems.Add(new Consumable(kitLevel, kitCategory, kitType));
			}
			else
			{
				var kitTypeRandomNum = GameHandler.GetRandomNumber(1, 2);
				var kitType = kitTypeRandomNum switch
				{
					1 => ChangeWeapon.KitType.Bowstring,
					2 => ChangeWeapon.KitType.Grindstone,
					_ => throw new ArgumentOutOfRangeException()
				};
				monster._MonsterItems.Add(new Consumable(kitLevel, kitCategory, kitType));
			}
		}
		private static void BuildMonsterArmor(Monster monster)
		{
			var randomCatNum = GameHandler.GetRandomNumber(1, 7);
			switch (randomCatNum)
			{
				case 1:
					monster._MonsterBackArmor = new Armor(monster._Level, Armor.ArmorSlot.Back);
					monster._MonsterBackArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterBackArmor);
					break;
				case 2:
					monster._MonsterChestArmor = new Armor(monster._Level, Armor.ArmorSlot.Chest);
					monster._MonsterChestArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterChestArmor);
					break;
				case 3:
					monster._MonsterHeadArmor = new Armor(monster._Level, Armor.ArmorSlot.Head);
					monster._MonsterHeadArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterHeadArmor);
					break;
				case 4:
					monster._MonsterLegArmor = new Armor(monster._Level, Armor.ArmorSlot.Legs);
					monster._MonsterLegArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterLegArmor);
					break;
				case 5:
					monster._MonsterWaistArmor = new Armor(monster._Level, Armor.ArmorSlot.Waist);
					monster._MonsterWaistArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterWaistArmor);
					break;
				case 6:
					monster._MonsterWristArmor = new Armor(monster._Level, Armor.ArmorSlot.Wrist);
					monster._MonsterWristArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterWristArmor);
					break;
				case 7:
					monster._MonsterHandsArmor = new Armor(monster._Level, Armor.ArmorSlot.Hands);
					monster._MonsterHandsArmor._Equipped = true;
					monster._MonsterItems.Add(monster._MonsterHandsArmor);
					break;
			}
		}
		private static void BuildMonsterNameDesc(Monster monster)
		{
			switch (monster._MonsterCategory)
			{
				case Monster.MonsterType.Skeleton:
					switch (monster._SkeletonCategory)
					{
						case Monster.SkeletonType.Warrior:
							monster._Name = monster._Level switch
							{
								1 => "skeleton " + monster._SkeletonCategory.ToString().ToLowerInvariant(),
								2 => "skeleton fighter",
								3 => "skeleton warrior",
								4 => "skeleton guardian",
								5 => "skeleton defender",
								6 => "skeleton conqueror",
								7 => "skeleton zealot",
								8 => "skeleton gladiator",
								9 => "skeleton knight",
								10 => "skeleton champion",
								_ => throw new ArgumentOutOfRangeException()
							};
							break;
						case Monster.SkeletonType.Archer:
							monster._Name = "skeleton " + monster._SkeletonCategory.ToString().ToLowerInvariant();
							break;
						case Monster.SkeletonType.Mage:
							monster._Name = "skeleton " + monster._SkeletonCategory.ToString().ToLowerInvariant();
							break;
						case null:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					monster._Desc =
						"A " + monster._Name + " stands in front of you. Its bones look worn and damaged from years of fighting. A " +
						"ghastly yellow glow surrounds it, which is the only indication of the magic that must exist to " +
						"reanimate it.";
					break;
				case Monster.MonsterType.Zombie:
					monster._Name = monster._Level switch
					{
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
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name +
						" stares at you, it's face frozen in a look of indifference to the fact a bug is crawling" +
						" out of it's empty eye sockets. In one hand, it drags a weapon against the ground, as it stares at you " +
						"menacingly. Bones, muscle and tendons are visible through many gashes and tears in it's rotting skin.";
					break;
				case Monster.MonsterType.Spider:
					monster._Name = monster._Level switch
					{
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
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name + " about the size of a large bear skitters down the corridor towards you. " +
						"Coarse hair sticks out from every direction on it's thorax and legs. It's many eyes stare at " +
						"you, legs ending in sharp claws carrying it closer as it hisses hungrily.";
					break;
				case Monster.MonsterType.Demon:
					monster._Name = monster._Level switch
					{
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
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name + " stands before you with two horns sticking out of it's head. It's eyes glint " +
						"yellow and a look of pure hatred adorns its face. Leathery wings spread out on either side of its " +
						"back as it rises up to its full height and growls at you.";
					break;
				case Monster.MonsterType.Elemental:
					var elementalRandomNumber = GameHandler.GetRandomNumber(1, 3);
					var elementalType = elementalRandomNumber switch
					{
						1 => monster._ElementalCategory == Monster.ElementalType.Fire,
						2 => monster._ElementalCategory == Monster.ElementalType.Water,
						3 => monster._ElementalCategory == Monster.ElementalType.Air,
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Name = monster._Level switch
					{
						1 => "lesser " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						2 => monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						3 => "large " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						4 => "greater " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						5 => "hulking " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						6 => "immense " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						7 => "massive " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						8 => "towering " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						9 => "titanic " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						10 => "colossal " + monster._ElementalCategory.ToString().ToLowerInvariant() + " elemental",
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name + " stands before you, shimmering out of the essence that it is created from. It " +
						"has no expression on its face, but the aggressiveness is clear when it appears to start casting a " +
						"ball of " + monster._ElementalCategory.ToString().ToLowerInvariant() + " to throw at you.";
					break;
				case Monster.MonsterType.Vampire:
					monster._Name = monster._Level switch
					{
						1 => "young vampire neophyte",
						2 => "young vampire acolyte",
						3 => "young vampire warrior",
						4 => "vampire neophyte",
						5 => "vampire acolyte",
						6 => "vampire warrior",
						7 => "old vampire",
						8 => "old vampire warrior",
						9 => "ancient vampire",
						10 => "ancient vampire warrior",
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name + " stands in front of you. It has a pale, ghoulish cast to it's skin, and  " +
						"you can see those trademark canines as it smiles cruelly at you. It does not look friendly but it " +
						"does look like it's ready to kill you and drink your blood.";
					break;
				case Monster.MonsterType.Troll:
					monster._Name = monster._Level switch
					{
						1 => "troll",
						2 => "troll fighter",
						3 => "troll warrior",
						4 => "troll guardian",
						5 => "troll defender",
						6 => "troll conqueror",
						7 => "troll zealot",
						8 => "troll gladiator",
						9 => "troll knight",
						10 => "troll champion",
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc =
						"A " + monster._Name + " stands in front of you. It has a bracelet of bones around both of its wrists " +
						"and tusks sprout from each corner of its mouth. As it eyes you apprehensively, it clutches its weapon " +
						"a little tighter, thinking of how to dissect you with it.";
					break;
				case Monster.MonsterType.Dragon:
					monster._Name = monster._Level switch
					{
						1 => "lesser dragon",
						2 => "dragon",
						3 => "winged dragon",
						4 => "greater dragon",
						5 => "hulking dragon",
						6 => "immense dragon",
						7 => "massive dragon",
						8 => "towering dragon",
						9 => "titanic dragon",
						10 => "colossal dragon",
						_ => throw new ArgumentOutOfRangeException()
					};
					monster._Desc = monster._Level < 3 ?
						"A " + monster._Name + " stands before you, scaled claws clicking and clacking against the ground as it " +
						"paces back and forth. It is small enough not to have any wings, but as it breathes in deeply and you " +
						"can see fire forming in the back of its mouth, that doesn't seem to matter." :
						"A " + monster._Name + " stands before you, scaled claws clicking and clacking against the ground shortly " +
						"before it spreads its wings and roars at you. As it's leathery wings extend to their full wingspan, it " +
						"breathes in deeply, and you can see fire forming in the back of its mouth.";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}