﻿using System.Collections.Generic;
using System.Threading;

namespace DungeonGame {
	public interface IMonster : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		int Level { get; set; }
		int MaxHitPoints { get; set; }
		int HitPoints { get; set; }
		int ExperienceProvided { get; set; }
		int Gold { get; set; }
		bool WasLooted { get; set; }
		bool InCombat { get; set; }
		int StatReplenishInterval { get; set; }
		Loot Item { get; set; }
		Consumable Consumable { get; set; }
		Weapon MonsterWeapon { get; set; }
		Armor MonsterHeadArmor { get; set; }
		Armor MonsterBackArmor { get; set; }
		Armor MonsterChestArmor { get; set; }
		Armor MonsterWristArmor { get; set; }
		Armor MonsterHandsArmor { get; set; }
		Armor MonsterWaistArmor { get; set; }
		Armor MonsterLegArmor { get; set; }
		List<IEquipment> MonsterItems { get; set; }
		List<Effect> Effects { get; set; }

		void TakeDamage(int weaponDamage);
		void DisplayStats();
		int Attack(Player player);
		int CheckArmorRating();
		void MonsterDeath(Player player);
		bool IsMonsterDead(Player player);
		int ArmorRating(Player player);
	}
}