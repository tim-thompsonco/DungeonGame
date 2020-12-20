using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class FrozenEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public string _Name { get; set; }
		private readonly double _EffectMultiplier;
		private int _CurrentRound;
		private readonly int _MaxRound;

		public FrozenEffect(string name, int maxRound) {
			_Name = name;
			_EffectMultiplier = 1.5;
			_CurrentRound = 1;
			_MaxRound = maxRound;
		}

		public int GetIncreasedDamageFromFrozen(Monster monster, int damage) {
			if (_IsEffectExpired) {
				return damage;
			}

			return (int)(damage * _EffectMultiplier);
		}

		public int GetIncreasedDamageFromFrozen(int damage) {
			if (_IsEffectExpired) {
				return damage;

			}

			return (int)(damage * _EffectMultiplier);
		}

		public void ProcessFrozenRound(Monster monster) {
			string frozenMessage = GetFrozenMessage(monster);
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessFrozenRound() {
			string frozenMessage = GetFrozenMessage();
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private string GetFrozenMessage(Monster monster) {
			int percentIncrease = (int)((_EffectMultiplier - 1) * 100);

			return $"The {monster._Name} is frozen. Any damage from an attack is increased by {percentIncrease}%!";
		}

		private string GetFrozenMessage() {
			int percentIncrease = (int)((_EffectMultiplier - 1) * 100);

			return $"You are frozen. Physical, frost and arcane damage to you will be increased by {percentIncrease}%!";
		}

		private void DisplayFrozenMessage(string frozenMessage) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				frozenMessage);
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
