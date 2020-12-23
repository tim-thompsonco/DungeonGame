using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class HealingEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		private readonly int _HealOverTimeAmount;

		public HealingEffect(string name, int maxRound, int healOverTimeAmount) {
			_TickDuration = 1;
			_IsHarmful = false;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_HealOverTimeAmount = healOverTimeAmount;
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
