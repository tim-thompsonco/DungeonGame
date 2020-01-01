using System.Collections.Generic;

namespace DungeonGame {
	public interface IMonster : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		int MaxHitPoints { get; set; }
		int HitPoints { get; set; }
		int ExperienceProvided { get; set; }
		int Level { get; set; }
		int Gold { get; set; }
		bool IsStunned { get; set; }
		int StunnedCurRound { get; set; }
		int StunnedMaxRound { get; set; }
		bool OnFire { get; set; }
		int OnFireDamage { get; set; }
		int OnFireCurRound { get; set; }
		int OnFireMaxRound { get; set; }
		bool IsBleeding { get; set; }
		int BleedDamage { get; set; }
		int BleedCurRound { get; set; }
		int BleedMaxRound { get; set; }
		bool WasLooted { get; set; }
		List<IEquipment> MonsterItems { get; set; }
		Loot Item { get; set; }
		Consumable Consumable { get; set; }
		Weapon MonsterWeapon { get; set; }
		Armor MonsterChestArmor { get; set; }
		Armor MonsterHeadArmor { get; set; }
		Armor MonsterLegArmor { get; set; }

		void TakeDamage(int weaponDamage);
		void DisplayStats(UserOutput output);
		int Attack();
		int CheckArmorRating();
		void MonsterDeath(Player player, UserOutput output);
		bool IsMonsterDead(Player player, UserOutput output);
		int ArmorRating(Player player);
		void SetOnFire(bool onFire, int onFireDamage, int onFireCurRound, int onFireMaxRound);
		void StartBleeding(bool bleeding, int bleedDamage, int bleedCurRound, int bleedMaxRound);
		void StartStunned(bool stunned, int stunCurRound, int stunMaxRound);
		void Stunned(UserOutput output);
	}
}