using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class HealingEffect : IEffect {
		public int CurrentRound { get; set; } = 1;
		public int HealOverTimeAmount { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; set; }
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
			if (player._HitPoints + HealOverTimeAmount > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			} else {
				player._HitPoints += HealOverTimeAmount;
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
	}
}
