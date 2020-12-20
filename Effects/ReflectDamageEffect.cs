using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class ReflectDamageEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public string _Name { get; set; }
		private readonly int _ReflectDamageAmount;
		private int _CurrentRound;
		private readonly int _MaxRound;

		public ReflectDamageEffect(int tickDuration, string name, int reflectDamageAmount, int maxRound) {
			_TickDuration = tickDuration;
			_Name = name;
			_ReflectDamageAmount = reflectDamageAmount;
			_CurrentRound = 1;
			_MaxRound = maxRound;
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

		public void ProcessReflectDamageRound(int incomingDamage) {
			if (_IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayReflectDamageMessage(incomingDamage);

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

		private void DisplayReflectDamageMessage(int incomingDamage) {
			int reflectAmount;
			if (incomingDamage < _ReflectDamageAmount) {
				reflectAmount = incomingDamage;
			} else {
				reflectAmount = _ReflectDamageAmount;
			}

			string reflectString = $"You reflected {reflectAmount} damage back at your opponent!";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
