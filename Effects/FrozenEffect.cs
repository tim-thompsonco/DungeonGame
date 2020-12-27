using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class FrozenEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		public double _EffectMultiplier { get; }

		public FrozenEffect(string name, int maxRound) {
			_TickDuration = 1;
			_IsHarmful = true;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_EffectMultiplier = 1.5;
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
			if (_IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage(monster);
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessFrozenRound() {
			if (_IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage();
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private string GetFrozenMessage(Monster monster) {
			int percentIncrease = (int)((_EffectMultiplier - 1) * 100);

			return $"The {monster._Name} is frozen. Physical, frost and arcane damage to it will be increased by {percentIncrease}%!";
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
