namespace DungeonGame.Effects {
	public interface IEffect : IName {
		bool _IsEffectExpired { get; set; }
		int _TickDuration { get; }

		void SetEffectAsExpired();
	}
}
