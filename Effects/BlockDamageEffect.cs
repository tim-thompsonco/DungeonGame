using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class BlockDamageEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		private int _BlockAmount;

		public BlockDamageEffect(string name, int blockAmount) {
			_TickDuration = 1;
			_IsHarmful = false;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = 3;
			_BlockAmount = blockAmount;
		}

		public void ProcessBlockDamageRound() {
			if (_IsEffectExpired) {
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
			if (_IsEffectExpired) {
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
			_CurrentRound++;
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
			_IsEffectExpired = true;
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