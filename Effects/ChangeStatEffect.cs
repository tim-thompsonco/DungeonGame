using DungeonGame.Helpers;
using DungeonGame.Interfaces;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public partial class ChangeStatEffect : IEffect {
		public int CurrentRound { get; private set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public StatType EffectStatType { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; }
		public int MaxRound { get; }
		public string Name { get; }
		public int TickDuration { get; } = 1;
		private readonly int _statAmount;

		public ChangeStatEffect(string name, int maxRound, StatType statType, int statAmount) {
			Name = name;
			MaxRound = maxRound;
			EffectStatType = statType;
			_statAmount = statAmount;
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
			if (EffectStatType is StatType.Intelligence) {
				player.Intelligence -= _statAmount;
			} else if (EffectStatType is StatType.Strength) {
				player.Strength -= _statAmount;
			} else if (EffectStatType is StatType.Dexterity) {
				player.Dexterity -= _statAmount;
			} else if (EffectStatType is StatType.Constitution) {
				player.Constitution -= _statAmount;
			}

			PlayerHelper.CalculatePlayerStats(player);
		}

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}
