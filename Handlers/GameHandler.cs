using System;
using System.Linq;

namespace DungeonGame {
	public static class GameHandler {
		private static readonly Random RndGenerate = new Random();
		public static bool IsGameOver { get; set; }
		public static int GameTicks { get; set; }
		
		public static void CheckStatus(Player player) {
			GameTicks++;
			RemovedExpiredEffects(player);
			if (GameTicks % player.StatReplenishInterval == 0) ReplenishStatsOverTime(player);
			if (player.Effects.Any()) {
				foreach (var effect in player.Effects.Where(effect => GameTicks % effect.TickDuration == 0)) {
					switch (effect.EffectGroup) {
						case Effect.EffectType.Healing:
							effect.HealingRound(player);
							break;
						case Effect.EffectType.ChangeDamage:
							if (player.InCombat == false && effect.Name.Contains("berserk")) {
								effect.IsEffectExpired = true;
							}
							break;
						case Effect.EffectType.ChangeArmor:
							if (player.InCombat == false && effect.Name.Contains("berserk")) {
								effect.IsEffectExpired = true;
							}
							if (player.InCombat == false) effect.ChangeArmorRound();
							break;
						case Effect.EffectType.AbsorbDamage:
							break;
						case Effect.EffectType.OnFire:
							break;
						case Effect.EffectType.Bleeding:
							break;
						case Effect.EffectType.Stunned:
							continue;
						case Effect.EffectType.Frozen:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			foreach (var room in RoomHandler.Rooms) {
				foreach (var roomObject in room.RoomObjects.Where(
					roomObject => roomObject.GetType() == typeof(Monster))) {
					var monster = (Monster) roomObject;
					RemovedExpiredEffects(monster);
					if (GameTicks % monster.StatReplenishInterval == 0 && monster.HitPoints > 0) ReplenishStatsOverTime(monster);
					if (!monster.Effects.Any()) continue;
						foreach (var effect in monster.Effects.Where(effect => GameTicks % effect.TickDuration == 0)) {
							switch (effect.EffectGroup) {
								case Effect.EffectType.Healing:
									break;
								case Effect.EffectType.ChangeDamage:
									break;
								case Effect.EffectType.ChangeArmor:
									break;
								case Effect.EffectType.AbsorbDamage:
									break;
								case Effect.EffectType.OnFire:
									effect.OnFireRound(monster);
									break;
								case Effect.EffectType.Bleeding:
									effect.BleedingRound(monster);
									break;
								case Effect.EffectType.Stunned:
									effect.StunnedRound(monster);
									break;
								case Effect.EffectType.Frozen:
									effect.FrozenRound(monster);
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}
				}
			}
		}
		public static int GetRandomNumber(int lowNum, int highNum) {
			return RndGenerate.Next(lowNum, highNum + 1);
		}
		public static int RoundNumber(int number) {
			var lastDigit = number % 10;
			number /= 10;
			var newLastDigit = lastDigit >= 5 ? 1 : 0;
			number += newLastDigit;
			number *= 10;
			return number;
		}
		public static void RemovedExpiredEffects(Player player) {
			for (var i = 0; i < player.Effects.Count; i++) {
				if (player.Effects[i].IsEffectExpired) player.Effects.RemoveAt(i);				
			}
		}
		public static void RemovedExpiredEffects(Monster monster) {
			for (var i = 0; i < monster.Effects.Count; i++) {
				if (monster.Effects[i].IsEffectExpired) monster.Effects.RemoveAt(i);				
			}
		}
		public static void ReplenishStatsOverTime(Player player) {
			if (player.InCombat) return;
			if (player.HitPoints == player.MaxHitPoints) return;
			player.HitPoints += 1;
			switch (player.PlayerClass) {
				case Player.PlayerClassType.Mage:
					if (player.ManaPoints == player.MaxManaPoints) return;
					player.ManaPoints += 1;
					break;
				case Player.PlayerClassType.Warrior:
					if (player.RagePoints == player.MaxRagePoints) return;
					player.RagePoints += 1;
					break;
				case Player.PlayerClassType.Archer:
					if (player.ComboPoints == player.MaxComboPoints) return;
					player.ComboPoints += 1;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void ReplenishStatsOverTime(Monster monster) {
			if (monster.InCombat) return;
			if (monster.HitPoints == monster.MaxHitPoints) return;
			monster.HitPoints += 1;
		}
		public static bool IsWholeNumber(string value) {
			return value.All(char.IsNumber);
		}
	}
}