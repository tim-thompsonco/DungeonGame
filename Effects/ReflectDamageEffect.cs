using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class ReflectDamageEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _ReflectDamageAmount { get; }

		public ReflectDamageEffect(string name, int maxRound, int reflectDamageAmount) {
			TickDuration = 10;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_ReflectDamageAmount = reflectDamageAmount;
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

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
		}

		private void DisplayReflectDamageMessage(int reflectedAmount) {
			string reflectString = $"You reflected {reflectedAmount} damage back at your opponent!";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public int GetReflectedDamageAmount(int incomingDamage) {
			if (_ReflectDamageAmount < incomingDamage) {
				return _ReflectDamageAmount;
			} else {
				return incomingDamage;
			}
		}
	}
}
