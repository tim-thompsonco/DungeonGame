using DungeonGame.Controllers;
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

		public HealingEffect(string name, int maxRound, int healOverTimeAmount) {
			Name = name;
			MaxRound = maxRound;
			HealOverTimeAmount = healOverTimeAmount;
		}

		public void ProcessHealingRound(Player player) {
			if (IsEffectExpired) {
				return;
			}

			HealPlayer(player);

			DisplayPlayerHealedMessage();

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private Player HealPlayer(Player player) {
			if (player.HitPoints + HealOverTimeAmount > player.MaxHitPoints) {
				player.HitPoints = player.MaxHitPoints;
			} else {
				player.HitPoints += HealOverTimeAmount;
			}

			return player;
		}

		private void DisplayPlayerHealedMessage() {
			string healAmtString = $"You have been healed for {HealOverTimeAmount} health.";

			OutputController.StoreSuccessMessage(healAmtString);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}
