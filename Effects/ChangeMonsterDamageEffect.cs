using DungeonGame.Controllers;
using DungeonGame.Players;
using System;

namespace DungeonGame.Effects {
	public class ChangeMonsterDamageEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _ChangeAmount { get; }

		public ChangeMonsterDamageEffect(string name, int maxRound, int changeAmount) {
			TickDuration = 1;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_ChangeAmount = changeAmount;
		}

		public void ProcessChangeMonsterDamageRound(Player player) {
			if (IsEffectExpired || player._InCombat == false) {
				return;
			}

			IncrementCurrentRound();

			DisplayChangeDamageMessage();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		private void DisplayChangeDamageMessage() {
			int changeAmount = Math.Abs(_ChangeAmount);

			string changeDmgString = _ChangeAmount > 0 ? $"Incoming damage is increased by {changeAmount}." :
				$"Incoming damage is decreased by {changeAmount}.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeDmgString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public int GetUpdatedDamageFromChange(int attackAmount) {
			return attackAmount + _ChangeAmount;
		}
	}
}
