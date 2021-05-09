using DungeonGame.Controllers;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Interfaces;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class StunnedEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public IEffectHolder EffectHolder { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 1;

		public StunnedEffect(EffectSettings effectSettings) {
			effectSettings.ValidateSettings();

			EffectHolder = effectSettings.EffectHolder;
			Name = effectSettings.Name;
			MaxRound = (int)effectSettings.MaxRound;
		}

		public void ProcessRound() {
			if (IsEffectExpired) {
				return;
			}

			if (EffectHolder is Monster monster) {
				monster.IsStunned = true;

				IncrementCurrentRound();

				DisplayStunnedMessage(monster);

				if (CurrentRound > MaxRound) {
					SetEffectAsExpired();
				}
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
