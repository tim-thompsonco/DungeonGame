namespace DungeonGame.Effects {
	public interface IEffect : IName {
		bool _IsEffectExpired { get; set; }

		void SetEffectAsExpired();
	}
}
