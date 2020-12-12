using DungeonGame.Controllers;
using System;

namespace DungeonGame
{
	public class ChangeStat
	{
		public enum StatType
		{
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public int _ChangeAmount { get; set; }
		public int _ChangeCurRound { get; set; }
		public int _ChangeMaxRound { get; set; }
		public StatType _StatCategory { get; set; }

		public ChangeStat(int amount, StatType statType)
		{
			_ChangeAmount = amount;
			_StatCategory = statType;
			// Change stat potions will default to 10 minutes for duration of effect
			_ChangeCurRound = 1;
			_ChangeMaxRound = 600;
		}

		public void ChangeStatPlayer(Player player)
		{
			switch (_StatCategory)
			{
				case StatType.Intelligence:
					player._Intelligence += _ChangeAmount;
					break;
				case StatType.Strength:
					player._Strength += _ChangeAmount;
					break;
				case StatType.Dexterity:
					player._Dexterity += _ChangeAmount;
					break;
				case StatType.Constitution:
					player._Constitution += _ChangeAmount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			PlayerController.CalculatePlayerStats(player);
			string effectName = $"{_StatCategory} (+{_ChangeAmount})";
			player._Effects.Add(new Effect(effectName, Effect.EffectType.ChangeStat, _ChangeAmount,
				_ChangeCurRound, _ChangeMaxRound, 1, 1, false, _StatCategory));
		}
	}
}