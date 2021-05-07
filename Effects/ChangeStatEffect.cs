using DungeonGame.Controllers;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class ChangeStatEffect : IEffect {
		public enum StatType {
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public StatType _StatType { get; set; }
		private readonly int _StatAmount;

		public ChangeStatEffect(string name, int maxRound, StatType statType, int statAmount) {
			TickDuration = 1;
			IsHarmful = false;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_StatType = statType;
			_StatAmount = statAmount;
		}

		public void ProcessChangeStatRound(Player player) {
			if (IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();

				RestorePlayerStatToNormal(player);
			}
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		private void RestorePlayerStatToNormal(Player player) {
			if (_StatType is StatType.Intelligence) {
				player._Intelligence -= _StatAmount;
			} else if (_StatType is StatType.Strength) {
				player._Strength -= _StatAmount;
			} else if (_StatType is StatType.Dexterity) {
				player._Dexterity -= _StatAmount;
			} else if (_StatType is StatType.Constitution) {
				player._Constitution -= _StatAmount;
			}

			PlayerController.CalculatePlayerStats(player);
		}
	}
}
