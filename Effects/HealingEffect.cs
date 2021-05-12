using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Helpers;
using DungeonGame.Interfaces;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class HealingEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public int HealOverTimeAmount { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 10;

		public HealingEffect(EffectOverTimeSettings effectOverTimeSettings) {
			effectOverTimeSettings.ValidateSettings();

			EffectHolder = effectOverTimeSettings.EffectHolder;
			Name = effectOverTimeSettings.Name;
			MaxRound = (int)effectOverTimeSettings.MaxRound;
			HealOverTimeAmount = (int)effectOverTimeSettings.AmountOverTime;
		}

		public void ProcessRound() {
			if (IsEffectExpired) {
				return;
			}

			HealPlayer();

			DisplayPlayerHealedMessage();

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void HealPlayer() {
			Player player = EffectHolder as Player;

			if (player.HitPoints + HealOverTimeAmount > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			} else {
				player.HitPoints += HealOverTimeAmount;
			}
		}

		private void DisplayPlayerHealedMessage() {
			string healAmtString = $"You have been healed for {HealOverTimeAmount} health.";

			OutputHelper.StoreSuccessMessage(healAmtString);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
