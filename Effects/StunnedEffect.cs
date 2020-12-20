using DungeonGame.Controllers;
using DungeonGame.Monsters;

namespace DungeonGame.Effects {
	public class StunnedEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public string _Name { get; set; }
		public int _TickDuration { get; }
		private int _CurrentRound;
		private readonly int _MaxRound;

		public StunnedEffect(string name, int tickDuration, int maxRound) {
			_Name = name;
			_TickDuration = tickDuration;
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
