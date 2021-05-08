using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class BlockDamageEffect : IEffect {
		public int BlockAmount { get; set; }
		public int CurrentRound { get; set; } = 1;
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; } = 3;
		public string Name { get; set; }
		public int TickDuration { get; } = 1;

		public BlockDamageEffect(string name, int blockAmount) {
			Name = name;
			BlockAmount = blockAmount;
		}

		public void ProcessBlockDamageRound() {
			if (IsEffectExpired) {
				return;
			}

			DisplayBlockEffectFadingMessage();

			IncrementCurrentRound();

			if (BlockAmount <= 0) {
				SetEffectAsExpired();

				DisplayBlockEffectExpiredMessage();
			}
		}

		public void ProcessBlockDamageRound(int incomingDamageAmount) {
			if (IsEffectExpired) {
				return;
			}

			int blockedDamageAmount = GetBlockReductionAmount(incomingDamageAmount);
			DisplayBlockedDamageMessage(blockedDamageAmount);

			DecreaseBlockAmountByIncomingDamage(incomingDamageAmount);

			IncrementCurrentRound();

			if (BlockAmount <= 0) {
				SetEffectAsExpired();

				DisplayBlockEffectExpiredMessage();
			}
		}

		private void DisplayBlockEffectFadingMessage() {
			const string blockFadeString = "Your block effect is fading away.";

			OutputController.StoreSuccessMessage(blockFadeString);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
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

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		private void DisplayBlockEffectExpiredMessage() {
			const string blockEndString = "You are no longer blocking damage!";

			OutputController.StoreSuccessMessage(blockEndString);
		}

		public int GetDecreasedDamageFromBlock(int incomingDamage) {
			return incomingDamage - GetBlockReductionAmount(incomingDamage);
		}

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}