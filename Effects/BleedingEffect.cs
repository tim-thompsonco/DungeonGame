using DungeonGame.Controllers;
using DungeonGame.Effects.SettingsObjects;
using DungeonGame.Interfaces;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BleedingEffect : IEffect {
		public int BleedDamageOverTime { get; }
		public int CurrentRound { get; set; } = 1;
		public IEffectHolder EffectHolder { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; set; }
		public int TickDuration { get; } = 1;

		public BleedingEffect(EffectOverTimeSettings effectSettings) {
			effectSettings.ValidateSettings();

			BleedDamageOverTime = (int)effectSettings.AmountOverTime;
			EffectHolder = effectSettings.EffectHolder;
			MaxRound = (int)effectSettings.MaxRound;
			Name = effectSettings.Name;
		}

		public void ProcessRound() {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBleeding();

			string bleedMessage = GetBleedMessage();
			DisplayBleedMessage(bleedMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void DecreaseHealthFromBleeding() {
			if (EffectHolder is Monster monster) {
				monster.HitPoints -= BleedDamageOverTime;
			} else {
				Player player = EffectHolder as Player;
				player.HitPoints -= BleedDamageOverTime;
			}
		}

		private string GetBleedMessage() {
			if (EffectHolder is Monster monster) {
				return $"The {monster.Name} bleeds for {BleedDamageOverTime} physical damage.";
			}

			return $"You bleed for {BleedDamageOverTime} physical damage.";
		}

		private void DisplayBleedMessage(string bleedMessage) {
			OutputController.StoreOnFireMessage(bleedMessage);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
