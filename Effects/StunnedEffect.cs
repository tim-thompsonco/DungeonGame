using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class StunnedEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }

		public StunnedEffect(string name, int maxRound) {
			TickDuration = 1;
			IsHarmful = true;
			Name = name;
			CurrentRound = 1;
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

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunnedString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
