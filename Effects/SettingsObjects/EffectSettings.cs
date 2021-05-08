using DungeonGame.Interfaces;
using System;

namespace DungeonGame.Effects.SettingsObjects {
	public class EffectSettings : IEffectSettings {
		public IEffectHolder EffectHolder { get; set; }
		public int? MaxRound { get; set; }
		public string Name { get; set; }

		public void ValidateSettings() {
			if (EffectHolder is null) {
				throw new Exception("EffectSettings: EffectHolder has not been set.");
			}

			if (MaxRound is null) {
				throw new Exception("EffectSettings: MaxRound has not been set.");
			}

			if (Name is null) {
				throw new Exception("EffectSettings: Name has not been set.");
			}
		}
	}
}
