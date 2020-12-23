using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class StunnedEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }

		public StunnedEffect(string name, int maxRound) {
			_TickDuration = 1;
			_IsHarmful = true;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
		}

		public void ProcessStunnedRound(Monster monster) {
			if (_IsEffectExpired) {
				return;
			}

			monster._IsStunned = true;

			IncrementCurrentRound();

			DisplayStunnedMessage(monster);

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		private void DisplayStunnedMessage(Monster monster) {
			string stunnedString = $"The {monster._Name} is stunned and cannot attack.";

			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunnedString);
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
