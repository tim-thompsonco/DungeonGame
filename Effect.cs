﻿using DungeonGame.Controllers;
using DungeonGame.Monsters;
using DungeonGame.Players;
using System;

namespace DungeonGame {
	public class Effect {
		public enum EffectType {
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
		public enum StatType {
			Intelligence,
			Strength,
			Dexterity,
			Constitution
		}
		public string _Name { get; set; }
		public EffectType _EffectGroup { get; set; }
		public StatType? _StatGroup { get; set; }
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
		public Effect(string name, EffectType effectGroup, int effectAmount, int tickDuration) {
			_Name = name;
			_EffectGroup = effectGroup;
			_EffectAmount = effectAmount;
			_TickDuration = tickDuration;
		}
		public Effect(string name, EffectType effectGroup, int effectCurRound, int effectMaxRound,
			double effectMultiplier, int tickDuration, bool harmful) {
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
			: this(name, effectGroup, effectCurRound, effectMaxRound, effectMultiplier, tickDuration, harmful) {
			_EffectAmountOverTime = effectAmountOverTime;
		}
		public Effect(string name, EffectType effectGroup, int effectAmountOverTime, int effectCurRound,
			int effectMaxRound, double effectMultiplier, int tickDuration, bool harmful, StatType statType)
			: this(name, effectGroup, effectAmountOverTime, effectCurRound, effectMaxRound, effectMultiplier, tickDuration,
				harmful) {
			_StatGroup = statType;
		}

		public void ReflectDamageRound() {
			if (_IsEffectExpired) {
				return;
			}

			_EffectCurRound += 1;
			const string reflectString = "Your spell reflect is slowly fading away.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
			if (_EffectCurRound <= _EffectMaxRound) {
				return;
			}

			_IsEffectExpired = true;
		}
		public void ReflectDamageRound(int reflectAmount) {
			if (_IsEffectExpired) {
				return;
			}

			_EffectCurRound += 1;
			string reflectString = $"You reflected {reflectAmount} damage back at your opponent!";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				reflectString);
			if (_EffectCurRound <= _EffectMaxRound) {
				return;
			}

			_IsEffectExpired = true;
		}
		public void ChangeOpponentDamageRound(Player player) {
			if (_IsEffectExpired || player._InCombat == false) {
				return;
			}

			_EffectCurRound += 1;
			string changeDmgString = _EffectAmountOverTime > 0 ? $"Incoming damage is increased by {_EffectAmountOverTime}." :
				$"Incoming damage is decreased by {-1 * _EffectAmountOverTime}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				changeDmgString);
			if (_EffectCurRound <= _EffectMaxRound) {
				return;
			}

			_IsEffectExpired = true;
		}
	}
}