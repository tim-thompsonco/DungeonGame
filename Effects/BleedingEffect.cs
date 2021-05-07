using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BleedingEffect : IEffect {
		public int BleedDamageOverTime { get; }
		public int CurrentRound { get; set; } = 1;
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; set; }
		public int TickDuration { get; } = 1;

		public BleedingEffect(string name, int maxRound, int bleedDamageOverTime) {
			Name = name;
			MaxRound = maxRound;
			BleedDamageOverTime = bleedDamageOverTime;
		}

		public void ProcessBleedingRound(Monster monster) {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBleeding(monster);

			string bleedMessage = GetBleedMessage(monster);
			DisplayBleedMessage(bleedMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessBleedingRound(Player player) {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBleeding(player);

			string bleedMessage = GetBleedMessage();
			DisplayBleedMessage(bleedMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private Monster DecreaseHealthFromBleeding(Monster monster) {
			monster.HitPoints -= BleedDamageOverTime;

			return monster;
		}

		private Player DecreaseHealthFromBleeding(Player player) {
			player._HitPoints -= BleedDamageOverTime;

			return player;
		}

		private string GetBleedMessage(Monster monster) {
			return $"The {monster.Name} bleeds for {BleedDamageOverTime} physical damage.";
		}

		private string GetBleedMessage() {
			return $"You bleed for {BleedDamageOverTime} physical damage.";
		}

		private void DisplayBleedMessage(string bleedMessage) {
			OutputController.StoreOnFireMessage(bleedMessage);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
