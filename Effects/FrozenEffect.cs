using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Interfaces;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class FrozenEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public double EffectMultiplier { get; } = 1.5;
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 1;

		public FrozenEffect(EffectSettings effectSettings) {
			effectSettings.ValidateSettings();

			EffectHolder = effectSettings.EffectHolder;
			MaxRound = (int)effectSettings.MaxRound;
			Name = effectSettings.Name;
		}

		public void ProcessRound() {
			string frozenMessage = GetFrozenMessage();
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private string GetFrozenMessage() {
			int percentIncrease = (int)((EffectMultiplier - 1) * 100);

			if (EffectHolder is Monster monster) {
				return $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by" +
					$" {percentIncrease}%!";
			} else {
				return $"You are frozen. Physical, frost and arcane damage to you will be increased by {percentIncrease}%!";
			}
		}

		private void DisplayFrozenMessage(string frozenMessage) {
			OutputHelper.StoreOnFireMessage(frozenMessage);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public int GetIncreasedDamageFromFrozen(int damage) {
			return (int)(damage * EffectMultiplier);
		}
	}
}
