using DungeonGame.Controllers;

namespace DungeonGame.Effects {
	public class BlockDamageEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public string _Name { get; set; }
		public int _TickDuration { get; }
		private int _BlockAmount;

		public BlockDamageEffect(string name, int tickDuration, int blockAmount) {
			_Name = name;
			_TickDuration = tickDuration;
			_BlockAmount = blockAmount;
		}

		public void ProcessBlockDamageRound(int incomingDamageAmount) {
			if (_IsEffectExpired) {
				return;
			}

			int blockedDamageAmount = GetBlockReductionAmount(incomingDamageAmount);
			DisplayBlockedDamageMessage(blockedDamageAmount);

			DecreaseBlockAmountByIncomingDamage(incomingDamageAmount);

			if (_BlockAmount <= 0) {
				SetEffectAsExpired();

				DisplayBlockEffectExpiredMessage();
			}
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

		public int GetDamageAmountAfterBlockReduction(int incomingDamageAmount) {
			if (incomingDamageAmount > _BlockAmount) {
				return incomingDamageAmount - _BlockAmount;
			} else {
				return 0;
			}
		}
	}
}