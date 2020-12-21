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
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public StatType _StatType { get; set; }
		public string _Name { get; set; }
		private readonly int _StatAmount;
		private int _CurrentRound;
		private readonly int _MaxRound;

		public ChangeStatEffect(string name, int statAmount, int maxRound, StatType statType) {
			_Name = name;
			_StatAmount = statAmount;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_StatType = statType;
		}

		public void ProcessChangeStatRound(Player player) {
			if (_IsEffectExpired) {
				return;
			}

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();

				RestorePlayerStatToNormal(player);
			}
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
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
