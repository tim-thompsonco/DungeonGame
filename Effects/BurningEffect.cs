using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BurningEffect : IEffect {
		public bool IsEffectExpired { get; set; }
		public int TickDuration { get; }
		public bool IsHarmful { get; }
		public string Name { get; set; }
		public int CurrentRound { get; set; }
		public int MaxRound { get; }
		public int _FireDamageOverTime { get; }

		public BurningEffect(string name, int maxRound, int fireDamageOverTime) {
			TickDuration = 10;
			IsHarmful = true;
			Name = name;
			CurrentRound = 1;
			MaxRound = maxRound;
			_FireDamageOverTime = fireDamageOverTime;
		}

		public void ProcessBurningRound(Monster monster) {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(monster);

			string burnMessage = GetBurnMessage(monster);
			DisplayBurnMessage(burnMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
				SetEffectAsExpired();
			}
		}

		public void ProcessBurningRound(Player player) {
			if (IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(player);

			string burnMessage = GetBurnMessage();
			DisplayBurnMessage(burnMessage);

			IncrementCurrentRound();

			if (CurrentRound > MaxRound) {
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
			return $"The {monster.Name} burns for {_FireDamageOverTime} fire damage.";
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
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}
	}
}
