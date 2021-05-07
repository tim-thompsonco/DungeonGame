using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class BlockDamageEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _BlockAmount { get; set; }

		public BlockDamageEffect(string name, int blockAmount) {
			TickDuration = 1;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = 3;
			_BlockAmount = blockAmount;
		}

		public void ProcessBlockDamageRound() {
			if (IsEffectExpired) {
				return;
			}

			DisplayBlockEffectFadingMessage();

			IncrementCurrentRound();

			if (_BlockAmount <= 0) {
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

			if (_BlockAmount <= 0) {
				SetEffectAsExpired();

				DisplayBlockEffectExpiredMessage();
			}
		}

		private void DisplayBlockEffectFadingMessage() {
			const string blockFadeString = "Your block effect is fading away.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockFadeString);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		private int GetBlockReductionAmount(int incomingDamageAmount) {
			if (_BlockAmount < incomingDamageAmount) {
				return _BlockAmount;
			} else {
				return incomingDamageAmount;
			}
		}

		private void DisplayBlockedDamageMessage(int blockedDamageAmount) {
			string blockString = $"Your defensive move blocked {blockedDamageAmount} damage!";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);
		}

		private void DecreaseBlockAmountByIncomingDamage(int incomingDamageAmount) {
			if (_BlockAmount > incomingDamageAmount) {
				_BlockAmount -= incomingDamageAmount;
			} else {
				_BlockAmount = 0;
			}
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		private void DisplayBlockEffectExpiredMessage() {
			const string blockEndString = "You are no longer blocking damage!";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockEndString);
		}

		public int GetDecreasedDamageFromBlock(int incomingDamage) {
			return incomingDamage - GetBlockReductionAmount(incomingDamage);
		}
	}
}