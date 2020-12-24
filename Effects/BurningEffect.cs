using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BurningEffect : IEffect {
		public bool _IsEffectExpired { get; set; }
		public int _TickDuration { get; }
		public bool _IsHarmful { get; }
		public string _Name { get; set; }
		public int _CurrentRound { get; set; }
		public int _MaxRound { get; }
		public int _FireDamageOverTime { get; }

		public BurningEffect(string name, int maxRound, int fireDamageOverTime) {
			_TickDuration = 1;
			_IsHarmful = true;
			_Name = name;
			_CurrentRound = 1;
			_MaxRound = maxRound;
			_FireDamageOverTime = fireDamageOverTime;
		}

		public void ProcessBurningRound(Monster monster) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(monster);

			string burnMessage = GetBurnMessage(monster);
			DisplayBurnMessage(burnMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessBurningRound(Player player) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(player);

			string burnMessage = GetBurnMessage();
			DisplayBurnMessage(burnMessage);

			IncrementCurrentRound();

			if (_CurrentRound > _MaxRound) {
				SetEffectAsExpired();
			}
		}

		private Monster DecreaseHealthFromBurnDamage(Monster monster) {
			monster._HitPoints -= _FireDamageOverTime;

			return monster;
		}

		private Player DecreaseHealthFromBurnDamage(Player player) {
			player._HitPoints -= _FireDamageOverTime;

			return player;
		}

		private string GetBurnMessage(Monster monster) {
			return $"The {monster._Name} burns for {_FireDamageOverTime} fire damage.";
		}

		private string GetBurnMessage() {
			return $"You burn for {_FireDamageOverTime} fire damage.";
		}

		private void DisplayBurnMessage(string burnMessage) {
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				burnMessage);
		}

		private void IncrementCurrentRound() {
			_CurrentRound++;
		}

		public void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
