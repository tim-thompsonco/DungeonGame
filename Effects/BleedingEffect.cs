using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BleedingEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public string _Name { get; set; }
		public bool _IsHarmful { get; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		private readonly int _BleedDamageOverTime;

		public BleedingEffect(string name, int maxRound, int bleedDamageOverTime) {
			_TickDuration = 1;
			_IsHarmful = true;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_BleedDamageOverTime = bleedDamageOverTime;
		}

		public void ProcessBleedingRound(Monster monster) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBleeding(monster);

			string bleedMessage = GetBleedMessage(monster);
			DisplayBleedMessage(bleedMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessBleedingRound(Player player) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBleeding(player);

			string bleedMessage = GetBleedMessage();
			DisplayBleedMessage(bleedMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private Monster DecreaseHealthFromBleeding(Monster monster) {
			monster._HitPoints -= _BleedDamageOverTime;

			return monster;
		}

		private Player DecreaseHealthFromBleeding(Player player) {
			player._HitPoints -= _BleedDamageOverTime;

			return player;
		}

		private string GetBleedMessage(Monster monster) {
			return $"The {monster._Name} bleeds for {_BleedDamageOverTime} physical damage.";
		}

		private string GetBleedMessage() {
			return $"You bleed for {_BleedDamageOverTime} physical damage.";
		}

		private void DisplayBleedMessage(string bleedMessage) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				bleedMessage);
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
