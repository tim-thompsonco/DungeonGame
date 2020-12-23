namespace DungeonGame.Effects {
	public interface IEffect : IName {
		bool _IsEffectExpired { get; set; }
		bool _IsHarmful { get; }
		int _TickDuration { get; }
		int _CurrentRound { get; set; }
		int _MaxRound { get; }

		void SetEffectAsExpired();
	}
}
