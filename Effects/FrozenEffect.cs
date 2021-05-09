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

		public FrozenEffect(string name, int maxRound) {
			Name = name;
			MaxRound = maxRound;
		}

		public int GetIncreasedDamageFromFrozen(int damage) {
			if (IsEffectExpired) {
				return damage;

			}

			return (int)(damage * EffectMultiplier);
		}

		public void ProcessFrozenRound(Monster monster) {
			if (IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage(monster);
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessFrozenRound() {
			if (IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage();
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private string GetFrozenMessage(Monster monster) {
			int percentIncrease = (int)((EffectMultiplier - 1) * 100);

			return $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by {percentIncrease}%!";
		}

		private string GetFrozenMessage() {
			int percentIncrease = (int)((EffectMultiplier - 1) * 100);

			return $"You are frozen. Physical, frost and arcane damage to you will be increased by {percentIncrease}%!";
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

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}
