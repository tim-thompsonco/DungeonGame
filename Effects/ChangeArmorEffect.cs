using DungeonGame.Controllers;
using System;

namespace DungeonGame.Effects {
	public class ChangeArmorEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _ChangeArmorAmount { get; }

		public ChangeArmorEffect(string name, int maxRound, int changeArmorAmount) {
			TickDuration = 10;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_ChangeArmorAmount = changeArmorAmount;
		}

		public void ProcessChangeArmorRound() {
			if (IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayChangeArmorMessage();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		private void DisplayChangeArmorMessage() {
			int changeAmount = Math.Abs(_ChangeArmorAmount);

			string changeArmorString = _ChangeArmorAmount > 0 ? $"Your armor is increased by {changeAmount}." :
				$"Your armor is decreased by {changeAmount}.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeArmorString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
