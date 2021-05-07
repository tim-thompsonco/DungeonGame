using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class HealingEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _HealOverTimeAmount { get; }

		public HealingEffect(string name, int maxRound, int healOverTimeAmount) {
			TickDuration = 10;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_HealOverTimeAmount = healOverTimeAmount;
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
			if (player._HitPoints + _HealOverTimeAmount > player._MaxHitPoints) {
				player._HitPoints = player._MaxHitPoints;
			} else {
				player._HitPoints += _HealOverTimeAmount;
			}

			return player;
		}

		private void DisplayPlayerHealedMessage() {
			string healAmtString = $"You have been healed for {_HealOverTimeAmount} health.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healAmtString);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
