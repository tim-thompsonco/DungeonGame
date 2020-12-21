using DungeonGame.Controllers;
using System;

namespace DungeonGame.Effects {
	public class ChangeArmorEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public string _Name { get; set; }
		public int _ChangeArmorAmount { get; }
		private int _CurrentRound;
		private readonly int _MaxRound;

		public ChangeArmorEffect(int tickDuration, string name, int changeArmorAmount, int maxRound) {
			_TickDuration = tickDuration;
			_Name = name;
			_ChangeArmorAmount = changeArmorAmount;
			_CurrentRound = 1;
			_MaxRound = maxRound;
		}

		public void ProcessChangeArmorRound() {
			if (_IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			DisplayChangeArmorMessage();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
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
			_IsEffectExpired = true;
		}
	}
}
