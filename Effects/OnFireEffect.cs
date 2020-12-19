using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;

namespace DungeonGame.Effects {
	public class OnFireEffect : IEffect {
		public string _Name { get; set; }
		public bool _IsEffectExpired { get ; set; }
		private readonly int _FireDamageOverTime;
		private int _CurrentTick;
		private readonly int _MaxTick;

		public OnFireEffect(string name, int fireDamageOverTime, int maxTick) {
			_Name = name;
			_FireDamageOverTime = fireDamageOverTime;
			_CurrentTick = 1;
			_MaxTick = maxTick;
		}

		public void OnFireTick(Monster monster) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(monster);

			string burnMessage = GetBurnMessage(monster);
			DisplayBurnMessage(burnMessage);

			IncrementCurrentTick();

			if (_CurrentTick > _MaxTick) {
				SetEffectAsExpired();
			}
		}

		public void OnFireTick(Player player) {
			if (_IsEffectExpired) {
				return;
			}

			DecreaseHealthFromBurnDamage(player);

			string burnMessage = GetBurnMessage();
			DisplayBurnMessage(burnMessage);

			IncrementCurrentTick();

			if (_CurrentTick > _MaxTick) {
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

		private void IncrementCurrentTick() {
			_CurrentTick++;
		}

		private void SetEffectAsExpired() {
			_IsEffectExpired = true;
		}
	}
}
