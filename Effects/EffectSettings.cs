using DungeonGame.Interfaces;

namespace DungeonGame.Effects {
	public class EffectSettings {
		public IEffectHolder EffectHolder { get; set; }
		public int MaxRound { get; set; }
		public string Name { get; set; }
	}
}
