﻿using DungeonGame.Controllers;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Interfaces;

namespace DungeonGame.Effects {
	public class BlockDamageEffect : IEffect, IChangeDamageEffect {
		public int BlockAmount { get; set; }
		public int CurrentRound { get; set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; } = 3;
		public string Name { get; set; }
		public int TickDuration { get; } = 1;

		public BlockDamageEffect(EffectAmountSettings effectAmountSettings) {
			EffectHolder = effectAmountSettings.EffectHolder;
			Name = effectAmountSettings.Name;
			BlockAmount = (int)effectAmountSettings.Amount;
		}

		public void ProcessRound() {
			DisplayBlockEffectFadingMessage();

			IncrementEffectRoundAndSetAsExpiredIfBlockAmountUsedUp();
		}

		private void DisplayBlockEffectFadingMessage() {
			const string blockFadeString = "Your block effect is fading away.";

			OutputController.StoreSuccessMessage(blockFadeString);
		}

		private void IncrementEffectRoundAndSetAsExpiredIfBlockAmountUsedUp() {
			IncrementCurrentRound();

			if (BlockAmount <= 0) {
				SetEffectAsExpired();

				DisplayBlockEffectExpiredMessage();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		private void DisplayBlockEffectExpiredMessage() {
			const string blockEndString = "You are no longer blocking damage!";

			OutputController.StoreSuccessMessage(blockEndString);
		}

		public void ProcessChangeDamageRound(int incomingDamageAmount) {
			int blockedDamageAmount = GetBlockReductionAmount(incomingDamageAmount);
			DisplayBlockedDamageMessage(blockedDamageAmount);

			DecreaseBlockAmountByIncomingDamage(incomingDamageAmount);

			IncrementEffectRoundAndSetAsExpiredIfBlockAmountUsedUp();
		}

		private int GetBlockReductionAmount(int incomingDamageAmount) {
			if (BlockAmount < incomingDamageAmount) {
				return BlockAmount;
			} else {
				return incomingDamageAmount;
			}
		}

		private void DisplayBlockedDamageMessage(int blockedDamageAmount) {
			string blockString = $"Your defensive move blocked {blockedDamageAmount} damage!";

			OutputController.StoreSuccessMessage(blockString);
		}

		private void DecreaseBlockAmountByIncomingDamage(int incomingDamageAmount) {
			if (BlockAmount > incomingDamageAmount) {
				BlockAmount -= incomingDamageAmount;
			} else {
				BlockAmount = 0;
			}
		}

		public int GetChangedDamageFromEffect(int incomingDamage) {
			return incomingDamage - GetBlockReductionAmount(incomingDamage);
		}
	}
}