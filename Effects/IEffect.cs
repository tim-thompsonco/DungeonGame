namespace DungeonGame.Effects {
	public interface IEffect : IName {
		bool IsEffectExpired { get; set; }
		bool IsHarmful { get; }
		int TickDuration { get; }
		int CurrentRound { get; set; }
		int MaxRound { get; }

		void SetEffectAsExpired();
	}
}
