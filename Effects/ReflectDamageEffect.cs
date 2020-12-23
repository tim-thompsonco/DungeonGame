using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class ReflectDamageEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		private readonly int _ReflectDamageAmount;

		public ReflectDamageEffect(string name, int maxRound, int reflectDamageAmount) {
			_TickDuration = 1;
			_IsHarmful = false;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_ReflectDamageAmount = reflectDamageAmount;
		}

		public void ProcessReflectDamageRound() {
			if (_IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayReflectEffectFadingMessage();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessReflectDamageRound(int reflectedAmount) {
			if (_IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayReflectDamageMessage(reflectedAmount);

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
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
			_IsEffectExpired = true;
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
