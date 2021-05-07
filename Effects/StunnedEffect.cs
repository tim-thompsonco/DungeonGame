using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class StunnedEffect : IEffect {
		public int CurrentRound { get; set; } = 1;
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; set; }
		public int TickDuration { get; } = 1;

		public StunnedEffect(string name, int maxRound) {
			Name = name;
			MaxRound = maxRound;
		}

		public void ProcessStunnedRound(Monster monster) {
			if (IsEffectExpired) {
				return;
			}

			monster._IsStunned = true;

			IncrementCurrentRound();

			DisplayStunnedMessage(monster);

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		private void DisplayStunnedMessage(Monster monster) {
			string stunnedString = $"The {monster.Name} is stunned and cannot attack.";

			OutputController.StoreAttackSuccessMessage(stunnedString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
