using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Interfaces;

namespace DungeonGame.Effects {
	public class ReflectDamageEffect : IEffect, IChangeDamageEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int ReflectDamageAmount { get; }
		public int TickDuration { get; } = 10;

		public ReflectDamageEffect(EffectAmountSettings effectAmountSettings) {
			effectAmountSettings.ValidateSettings();

			EffectHolder = effectAmountSettings.EffectHolder;
			Name = effectAmountSettings.Name;
			MaxRound = effectAmountSettings.MaxRound;
			ReflectDamageAmount = (int)effectAmountSettings.Amount;
		}

		public void ProcessRound() {
			DisplayReflectEffectFadingMessage();

			IncrementEffectRoundAndSetAsExpiredIfNecessary();
		}

		private void DisplayReflectEffectFadingMessage() {
			const string reflectString = "Your spell reflect is slowly fading away.";

			OutputHelper.StoreSuccessMessage(reflectString);
		}

		private void IncrementEffectRoundAndSetAsExpiredIfNecessary() {
			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public void ProcessChangeDamageRound(int incomingDamageAmount) {
			DisplayReflectDamageMessage(incomingDamageAmount);

			IncrementEffectRoundAndSetAsExpiredIfNecessary();
		}

		private void DisplayReflectDamageMessage(int reflectedAmount) {
			string reflectString = $"You reflected {reflectedAmount} damage back at your opponent!";

			OutputHelper.StoreSuccessMessage(reflectString);
		}

		public int GetChangedDamageFromEffect(int incomingDamage) {
			if (ReflectDamageAmount < incomingDamage) {
				return ReflectDamageAmount;
			}

			return incomingDamage;
		}
	}
}
