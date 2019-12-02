using System.Collections.Generic;

namespace DungeonGame {
  public interface IMonster {
    string Name { get; set; }
		string Desc { get; set; }
    int MaxHitPoints { get; set; }
    int HitPoints { get; set; }
    int ExperienceProvided { get; set; }
		int Level { get; set; }
    int Gold { get; set; }
    bool OnFire { get; set; }
    bool WasLooted { get; set; }
		List<IRoomInteraction> MonsterItems { get; set; }

		void TakeDamage(int weaponDamage);
		void DisplayStats();
    int Attack();
  }
}