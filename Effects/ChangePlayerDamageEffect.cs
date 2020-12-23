using DungeonGame.Controllers;
using DungeonGame.Players;
using System;

namespace DungeonGame.Effects {
	public class ChangePlayerDamageEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		private readonly int _ChangeAmount;

		public ChangePlayerDamageEffect(string name, int maxRound, int changeAmount) {
			_TickDuration = 1;
			_IsHarmful = false;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_ChangeAmount = changeAmount;
		}

		public void ProcessChangePlayerDamageRound(Player player) {
			if (_IsEffectExpired || player._InCombat == false) {
				return;
			}

			IncrementCurrentRound();

			DisplayChangeDamageMessage();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		private void DisplayChangeDamageMessage() {
			int changeAmount = Math.Abs(_ChangeAmount);

			string changeDmgString = _ChangeAmount > 0 ? $"Your damage is increased by {changeAmount}." :
				$"Your damage is decreased by {changeAmount}.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeDmgString);
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}

		public int GetUpdatedDamageFromChange(int attackAmount) {
			return attackAmount + _ChangeAmount;
		}
	}
}
