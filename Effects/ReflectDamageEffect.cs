using DungeonGame.Controllers;
using DungeonGame.Interfaces;

namespace DungeonGame.Effects {
	public class ReflectDamageEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int ReflectDamageAmount { get; }
		public int TickDuration { get; } = 10;

		public ReflectDamageEffect(string name, int maxRound, int reflectDamageAmount) {
			Name = name;
			MaxRound = maxRound;
			ReflectDamageAmount = reflectDamageAmount;
		}

		public void ProcessReflectDamageRound() {
			if (IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayReflectEffectFadingMessage();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessReflectDamageRound(int reflectedAmount) {
			if (IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayReflectDamageMessage(reflectedAmount);

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		private void DisplayReflectEffectFadingMessage() {
			const string reflectString = "Your spell reflect is slowly fading away.";

			OutputController.StoreSuccessMessage(reflectString);
		}

		private void DisplayReflectDamageMessage(int reflectedAmount) {
			string reflectString = $"You reflected {reflectedAmount} damage back at your opponent!";

			OutputController.StoreSuccessMessage(reflectString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public int GetReflectedDamageAmount(int incomingDamage) {
			if (ReflectDamageAmount < incomingDamage) {
				return ReflectDamageAmount;
			} else {
				return incomingDamage;
			}
		}

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}
