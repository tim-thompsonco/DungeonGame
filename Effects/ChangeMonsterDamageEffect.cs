using DungeonGame.Controllers;
using DungeonGame.Interfaces;
using DungeonGame.Players;
using System;

namespace DungeonGame.Effects {
	public class ChangeMonsterDamageEffect : IEffect {
		public int ChangeAmount { get; }
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 1;

		public ChangeMonsterDamageEffect(string name, int maxRound, int changeAmount) {
			Name = name;
			MaxRound = maxRound;
			ChangeAmount = changeAmount;
		}

		public void ProcessChangeMonsterDamageRound(Player player) {
			if (IsEffectExpired || player.InCombat == false) {
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
			int changeAmount = Math.Abs(ChangeAmount);

			string changeDmgString = ChangeAmount > 0 ? $"Incoming damage is increased by {changeAmount}." :
				$"Incoming damage is decreased by {changeAmount}.";

			OutputController.StoreSuccessMessage(changeDmgString);
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public int GetUpdatedDamageFromChange(int attackAmount) {
			return attackAmount + ChangeAmount;
		}

		public void ProcessRound() {
			throw new NotImplementedException();
		}
	}
}
