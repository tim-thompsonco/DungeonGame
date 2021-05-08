using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class BurningEffect : IEffect {
		public int CurrentRound { get; set; } = 1;
		public int FireDamageOverTime { get; }
		public bool IsEffectExpired { get; set; }
		public bool IsHarmful { get; } = true;
		public int MaxRound { get; }
		public string Name { get; set; }
		public int TickDuration { get; } = 10;

		public BurningEffect(string name, int maxRound, int fireDamageOverTime) {
			Name = name;
			MaxRound = maxRound;
			FireDamageOverTime = fireDamageOverTime;
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
			monster.HitPoints -= FireDamageOverTime;

			return monster;
		}

		private Player DecreaseHealthFromBurnDamage(Player player) {
			player.HitPoints -= FireDamageOverTime;

			return player;
		}

		private string GetBurnMessage(Monster monster) {
			return $"The {monster.Name} burns for {FireDamageOverTime} fire damage.";
		}

		private string GetBurnMessage() {
			return $"You burn for {FireDamageOverTime} fire damage.";
		}

		private void DisplayBurnMessage(string burnMessage) {
			OutputController.StoreOnFireMessage(burnMessage);
		}

		private void IncrementCurrentRound() {
			CurrentRound++;
		}

		public void SetEffectAsExpired() {
			IsEffectExpired = true;
		}

		public void ProcessRound() {
			throw new System.NotImplementedException();
		}
	}
}
