using DungeonGame.Interfaces;
using System;

namespace DungeonGame.Effects.SettingsObjects {
	public class EffectAmountSettings : IEffectSettings {
		public int? Amount { get; set; }
		public IEffectHolder EffectHolder { get; set; }
		public int MaxRound { get; set; } = 3;
		public string Name { get; set; }

		public void ValidateSettings() {
			if (Amount is null) {
				throw new Exception("EffectAmountSettings: Amount has not been set.");
			}

			if (EffectHolder is null) {
				throw new Exception("EffectAmountSettings: EffectHolder has not been set.");
			}

			if (Name is null) {
				throw new Exception("EffectAmountSettings: Name has not been set.");
			}
		}
	}
}
