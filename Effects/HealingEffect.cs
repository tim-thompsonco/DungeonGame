using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class HealingEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public string _Name { get; set; }
		public int _TickDuration { get; }
		private readonly int _HealOverTimeAmount;
		private int _CurrentRound;
		private readonly int _MaxRound;

		public HealingEffect(string name, int healOverTimeAmount, int tickDuration, int maxRound) {
			_Name = name;
			_HealOverTimeAmount = healOverTimeAmount;
			_TickDuration = tickDuration;
			_CurrentRound = 1;
			_MaxRound = maxRound;
		}

		public void ProcessHealingRound(Player player) {
			if (_IsEffectExpired) {
				return;
			}

			HealPlayer(player);

			DisplayPlayerHealedMessage();

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
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
			_CurrentRound++;
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
