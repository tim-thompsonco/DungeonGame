using System;

namespace DungeonGame {
	public class ChangeStat {
		public enum StatType {
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public int ChangeAmount { get; set; }
		private int ChangeCurRound { get; set; }
		private int ChangeMaxRound { get; set; }
		public StatType StatCategory { get; set; }

		public ChangeStat(int amount, StatType statType) {
			this.ChangeAmount = amount;
			this.StatCategory = statType;
			// Change stat potions will default to 10 minutes
			this.ChangeCurRound = 1;
			this.ChangeMaxRound = 600;
		}
		
		public void ChangeStatPlayer(Player player) {
			switch (this.StatCategory) {
				case StatType.Intelligence:
					player.Intelligence += this.ChangeAmount;
					break;
				case StatType.Strength:
					player.Strength += this.ChangeAmount;
					break;
				case StatType.Dexterity:
					player.Dexterity += this.ChangeAmount;
					break;
				case StatType.Constitution:
					player.Constitution += this.ChangeAmount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			PlayerHandler.CalculatePlayerStats(player);
			var effectName = this.StatCategory + " (" + this.ChangeAmount + ")";
			player.Effects.Add(new Effect(effectName, Effect.EffectType.ChangeStat, this.ChangeAmount, 
				this.ChangeCurRound, this.ChangeMaxRound, 1, 1, false, this.StatCategory));
		}
	}
}