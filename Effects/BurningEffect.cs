using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Interfaces;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class BurningEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public int FireDamageOverTime { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 10;

		public BurningEffect(EffectOverTimeSettings effectOverTimeSettings) {
			effectOverTimeSettings.ValidateSettings();

			EffectHolder = effectOverTimeSettings.EffectHolder;
			FireDamageOverTime = (int)effectOverTimeSettings.AmountOverTime;
			MaxRound = (int)effectOverTimeSettings.MaxRound;
			Name = effectOverTimeSettings.Name;
		}

		public void ProcessRound() {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage();

			string burnMessage = GetBurnMessage();
			DisplayBurnMessage(burnMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void DecreaseHealthFromBurnDamage() {
			EffectHolder.HitPoints -= FireDamageOverTime;
		}

		private string GetBurnMessage() {
			if (EffectHolder is Monster monster) {
				return $"The {monster.Name} burns for {FireDamageOverTime} fire damage.";
			}

			return $"You burn for {FireDamageOverTime} fire damage.";
		}

		private void DisplayBurnMessage(string burnMessage) {
			OutputHelper.StoreOnFireMessage(burnMessage);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
