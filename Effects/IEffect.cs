using DungeonGame.Interfaces;

namespace DungeonGame.Effects {
	public interface IEffect {
		IEffectHolder EffectHolder { get; }
		bool IsEffectExpired { get; set; }
		bool IsHarmful { get; }
		int TickDuration { get; }
		int CurrentRound { get; }
		int MaxRound { get; }
		string Name { get; }

		void SetEffectAsExpired();
		void ProcessRound();
	}
}
