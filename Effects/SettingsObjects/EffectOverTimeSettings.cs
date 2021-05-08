using DungeonGame.Interfaces;
using System;

namespace DungeonGame.Effects.SettingsObjects {
	public class EffectOverTimeSettings : IEffectSettings {
		public int? AmountOverTime { get; set; }
		public IEffectHolder EffectHolder { get; set; }
		public int? MaxRound { get; set; }
		public string Name { get; set; }

		public void ValidateSettings() {
			if (AmountOverTime is null) {
				throw new Exception("EffectOverTimeSettings: AmountOverTime has not been set.");
			}

			if (EffectHolder is null) {
				throw new Exception("EffectOverTimeSettings: EffectHolder has not been set.");
			}

			if (MaxRound is null) {
				throw new Exception("EffectOverTimeSettings: MaxRound has not been set.");
			}

			if (Name is null) {
				throw new Exception("EffectOverTimeSettings: Name has not been set.");
			}
		}
	}
}
