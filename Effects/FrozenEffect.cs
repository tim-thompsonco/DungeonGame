using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class FrozenEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public double _EffectMultiplier { get; }

		public FrozenEffect(string name, int maxRound) {
			TickDuration = 1;
			IsHarmful = true;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_EffectMultiplier = 1.5;
		}

		public int GetIncreasedDamageFromFrozen(int damage) {
			if (IsEffectExpired) {
				return damage;

			}

			return (int)(damage * _EffectMultiplier);
		}

		public void ProcessFrozenRound(Monster monster) {
			if (IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage(monster);
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessFrozenRound() {
			if (IsEffectExpired) {
				return;
			}

			string frozenMessage = GetFrozenMessage();
			DisplayFrozenMessage(frozenMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private string GetFrozenMessage(Monster monster) {
			int percentIncrease = (int)((_EffectMultiplier - 1) * 100);

			return $"The {monster.Name} is frozen. Physical, frost and arcane damage to it will be increased by {percentIncrease}%!";
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
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
