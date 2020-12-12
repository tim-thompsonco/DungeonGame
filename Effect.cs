using DungeonGame.Controllers;
using System;

namespace DungeonGame
{
	public class Effect
	{
		public enum EffectType
		{
			Healing,
			ChangePlayerDamage,
			ChangeOpponentDamage,
			ChangeArmor,
			BlockDamage,
			OnFire,
			Bleeding,
			Stunned,
			ReflectDamage,
			Frozen,
			ChangeStat,
		}
		public string _Name { get; set; }
		public EffectType _EffectGroup { get; set; }
		public ChangeStat.StatType? _StatGroup { get; set; }
		public int _EffectAmount { get; set; }
		public int _EffectAmountOverTime { get; set; }
		public int _EffectCurRound { get; set; }
		public int _EffectMaxRound { get; set; }
		public double _EffectMultiplier { get; set; }
		public bool _IsEffectExpired { get; set; }
		public bool _IsHarmful { get; set; }
		public int _TickDuration { get; set; }

		// Default constructor for JSON serialization
		public Effect() { }
		public Effect(string name, EffectType effectGroup, int effectAmount, int tickDuration)
		{
			_Name = name;
			_EffectGroup = effectGroup;
			_EffectAmount = effectAmount;
			_TickDuration = tickDuration;
		}
		public Effect(string name, EffectType effectGroup, int effectCurRound, int effectMaxRound,
			double effectMultiplier, int tickDuration, bool harmful)
		{
			_Name = name;
			_EffectGroup = effectGroup;
			_EffectCurRound = effectCurRound;
			_EffectMaxRound = effectMaxRound;
			_TickDuration = tickDuration;
			_EffectMultiplier = effectMultiplier;
			_IsHarmful = harmful;
		}
		public Effect(string name, EffectType effectGroup, int effectAmountOverTime, int effectCurRound,
			int effectMaxRound, double effectMultiplier, int tickDuration, bool harmful)
			: this(name, effectGroup, effectCurRound, effectMaxRound, effectMultiplier, tickDuration, harmful)
		{
			_EffectAmountOverTime = effectAmountOverTime;
		}
		public Effect(string name, EffectType effectGroup, int effectAmountOverTime, int effectCurRound,
			int effectMaxRound, double effectMultiplier, int tickDuration, bool harmful, ChangeStat.StatType statType)
			: this(name, effectGroup, effectAmountOverTime, effectCurRound, effectMaxRound, effectMultiplier, tickDuration,
				harmful)
		{
			_StatGroup = statType;
		}

		public void HealingRound(Player player)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			player._HitPoints += _EffectAmountOverTime;
			if (player._HitPoints > player._MaxHitPoints)
			{
				player._HitPoints = player._MaxHitPoints;
			}

			string healAmtString = $"You have been healed for {_EffectAmountOverTime} health.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				healAmtString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void ChangeStatRound()
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void BlockDamageRound()
		{
			if (_IsEffectExpired)
			{
				return;
			}

			const string blockString = "Your block effect is slowly fading away.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);
			if (_TickDuration > 0)
			{
				return;
			}

			const string blockEndString = "You are no longer blocking damage!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockEndString);
			_IsEffectExpired = true;
		}
		public void BlockDamageRound(int blockAmount)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			string blockString = $"Your defensive move blocked {blockAmount} damage!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockString);
			_EffectAmount -= blockAmount;
			if (_EffectAmount > 0)
			{
				return;
			}

			const string blockEndString = "You are no longer blocking damage!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				blockEndString);
			_IsEffectExpired = true;
		}
		public void ReflectDamageRound()
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			const string reflectString = "Your spell reflect is slowly fading away.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void ReflectDamageRound(int reflectAmount)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			string reflectString = $"You reflected {reflectAmount} damage back at your opponent!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void ChangeOpponentDamageRound(Player player)
		{
			if (_IsEffectExpired || player._InCombat == false)
			{
				return;
			}

			_EffectCurRound += 1;
			string changeDmgString = _EffectAmountOverTime > 0 ? $"Incoming damage is increased by {_EffectAmountOverTime}." :
				$"Incoming damage is decreased by {-1 * _EffectAmountOverTime}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeDmgString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void ChangePlayerDamageRound(Player player)
		{
			if (_IsEffectExpired || player._InCombat == false)
			{
				return;
			}

			_EffectCurRound += 1;
			int changeAmount = Math.Abs(_EffectAmountOverTime);
			string changeDmgString = _EffectAmountOverTime > 0 ? $"Your damage is increased by {changeAmount}." : 
				$"Your damage is decreased by {changeAmount}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeDmgString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void ChangeArmorRound()
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			int changeAmount = Math.Abs(_EffectAmountOverTime);
			string changeArmorString = _EffectAmountOverTime > 0 ? $"Your armor is increased by {changeAmount}." : 
				$"Your armor is decreased by {changeAmount}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeArmorString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void OnFireRound(Monster opponent)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			opponent._HitPoints -= _EffectAmountOverTime;
			string burnString = $"The {opponent._Name} burns for {_EffectAmountOverTime} fire damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				burnString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void OnFireRound(Player player)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			player._HitPoints -= _EffectAmountOverTime;
			string burnString = $"You burn for {_EffectAmountOverTime} fire damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatOnFireText(),
				Settings.FormatDefaultBackground(),
				burnString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void BleedingRound(Monster opponent)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			opponent._HitPoints -= _EffectAmountOverTime;
			string bleedString = $"The {opponent._Name} bleeds for {_EffectAmountOverTime} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				bleedString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void BleedingRound(Player player)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			player._HitPoints -= _EffectAmountOverTime;
			string bleedString = $"You bleed for {_EffectAmountOverTime} physical damage.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				bleedString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void StunnedRound(Monster opponent)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			opponent._IsStunned = true;
			_EffectCurRound += 1;
			string stunnedString = $"The {opponent._Name} is stunned and cannot attack.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				stunnedString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void FrozenRound(Monster opponent)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			string frozenString = $"The {opponent._Name} is frozen. Physical, frost and arcane damage to it will be double!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
		public void FrozenRound(Player player)
		{
			if (_IsEffectExpired)
			{
				return;
			}

			_EffectCurRound += 1;
			const string frozenString = "You are frozen. Physical, frost and arcane damage to you will be double!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatAttackSuccessText(),
				Settings.FormatDefaultBackground(),
				frozenString);
			if (_EffectCurRound <= _EffectMaxRound)
			{
				return;
			}

			_IsEffectExpired = true;
		}
	}
}