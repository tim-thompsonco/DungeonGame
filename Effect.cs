using System;
using System.Threading;
using Newtonsoft.Json;

namespace DungeonGame {
	public class Effect {
		public enum EffectType {
			Healing,
			ChangeDamage,
			ChangeArmor,
			AbsorbDamage,
			OnFire,
			Bleeding,
			Stunned,
			Frozen
		}
		public string Name { get; set; }
		public EffectType EffectGroup { get; set; }
		public int EffectAmountOverTime { get; set; }
		public int EffectCurRound { get; set; }
		public int EffectMaxRound { get; set; }
		public int EffectMultiplier { get; set; }
		public bool IsEffectExpired { get; set; }
		public Timer EffectTimer { get; set; }
		public int TimerDuration { get; set; }

		// Default constructor for JSON serialization
		public Effect() {}
		public Effect(string name, EffectType effectGroup, int effectCurRound, int effectMaxRound, int effectMultiplier) {
			this.Name = name;
			this.EffectGroup = effectGroup;
			this.EffectCurRound = effectCurRound;
			this.EffectMaxRound = effectMaxRound;
			this.EffectMultiplier = effectMultiplier;
		}
		public Effect(string name, EffectType effectGroup, int effectAmountOverTime, int effectCurRound,
			int effectMaxRound, int effectMultiplier)
			: this(name, effectGroup, effectCurRound, effectMaxRound, effectMultiplier) {
			this.EffectAmountOverTime = effectAmountOverTime;
		}
		public Effect(string name, EffectType effectGroup, int effectAmountOverTime, int effectCurRound,
			int effectMaxRound, Player player, UserOutput output, int timerDuration)
			: this(name, effectGroup, effectAmountOverTime, effectCurRound, effectMaxRound) {
			this.TimerDuration = timerDuration;
			switch (this.EffectGroup) {
				case EffectType.Healing:
					this.EffectTimer = new Timer(
						e => this.HealingRound(player, output),
						null, TimeSpan.FromSeconds(this.TimerDuration), TimeSpan.FromSeconds(this.TimerDuration));
					break;
				case EffectType.ChangeDamage:
					break;
				case EffectType.ChangeArmor:
					break;
				case EffectType.AbsorbDamage:
					break;
				case EffectType.OnFire:
					break;
				case EffectType.Bleeding:
					break;
				case EffectType.Stunned:
					break;
				case EffectType.Frozen:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(effectGroup), effectGroup, null);
			}
		}

		public void EnterCombat(Player player, UserOutput output) {
			switch (this.EffectGroup) {
				case EffectType.Healing:
					this.EffectTimer = new Timer(
						e => this.HealingRound(player, output),
						null, TimeSpan.FromSeconds(Timeout.Infinite), TimeSpan.FromSeconds(Timeout.Infinite));
					break;
				case EffectType.ChangeDamage:
					this.EffectTimer = new Timer(
						e => this.ChangeArmorRound(output),
						null, TimeSpan.FromSeconds(Timeout.Infinite), TimeSpan.FromSeconds(Timeout.Infinite));
					break;
				case EffectType.ChangeArmor:
					this.EffectTimer = new Timer(
						e => this.ChangeDamageRound(output),
						null, TimeSpan.FromSeconds(Timeout.Infinite), TimeSpan.FromSeconds(Timeout.Infinite));
					break;
				case EffectType.AbsorbDamage:
					break;
				case EffectType.OnFire:
					break;
				case EffectType.Bleeding:
					break;
				case EffectType.Stunned:
					break;
				case EffectType.Frozen:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void ExitCombat(Player player, UserOutput output) {
			switch (this.EffectGroup) {
				case EffectType.Healing:
					this.EffectTimer = new Timer(
						e => this.HealingRound(player, output),
						null, TimeSpan.FromSeconds(this.TimerDuration), TimeSpan.FromSeconds(this.TimerDuration));
					break;
				case EffectType.ChangeDamage:
					this.EffectTimer = new Timer(
						e => this.ChangeDamageRound(output),
						null, TimeSpan.FromSeconds(this.TimerDuration), TimeSpan.FromSeconds(this.TimerDuration));
					break;
				case EffectType.ChangeArmor:
					this.EffectTimer = new Timer(
						e => this.ChangeArmorRound(output),
						null, TimeSpan.FromSeconds(this.TimerDuration), TimeSpan.FromSeconds(this.TimerDuration));
					break;
				case EffectType.AbsorbDamage:
					break;
				case EffectType.OnFire:
					break;
				case EffectType.Bleeding:
					break;
				case EffectType.Stunned:
					break;
				case EffectType.Frozen:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			foreach (var effect in player.Effects) {
				if (effect.Name == Ability.WarriorAbility.Berserk.ToString().ToLower()) effect.IsEffectExpired = true;
			}
		}
		public void HealingRound(Player player, UserOutput output) {
			if (this.IsEffectExpired) return;
			this.EffectCurRound += 1;
			player.HitPoints += this.EffectAmountOverTime;
			if (player.HitPoints > player.MaxHitPoints) player.HitPoints = player.MaxHitPoints;
			var healAmtString = "You have been healed for " + this.EffectAmountOverTime + " health."; 
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				healAmtString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void ChangeDamageRound(UserOutput output) {
			this.EffectCurRound += 1;
			var changeDmgString = this.EffectAmountOverTime > 0 ?
				"Your damage is increased by " + this.EffectAmountOverTime + "."
				: "Your damage is decreased by " + this.EffectAmountOverTime + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				changeDmgString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void ChangeArmorRound(UserOutput output) {
			this.EffectCurRound += 1;
			var augmentString = "Your armor is augmented by " + this.EffectAmountOverTime + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				augmentString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void OnFireRound(IMonster opponent, UserOutput output) {
			if (this.IsEffectExpired) return;
			this.EffectCurRound += 1;
			opponent.HitPoints -= this.EffectAmountOverTime;
			var burnString = "The " + opponent.Name + " burns for " + this.EffectAmountOverTime + " fire damage.";
			output.StoreUserOutput(
				Helper.FormatOnFireText(),
				Helper.FormatDefaultBackground(),
				burnString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void BleedingRound(IMonster opponent, UserOutput output) {
			if (this.IsEffectExpired) return;
			this.EffectCurRound += 1;
			opponent.HitPoints -= this.EffectAmountOverTime;
			var bleedString = "The " + opponent.Name + " bleeds for " + this.EffectAmountOverTime + " physical damage.";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				bleedString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void StunnedRound(IMonster opponent, UserOutput output) {
			if (this.IsEffectExpired) return;
			this.EffectCurRound += 1;
			var stunnedString = "The " + opponent.Name + " is stunned and cannot attack.";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				stunnedString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
		public void FrozenRound(IMonster opponent, UserOutput output) {
			if (this.IsEffectExpired) return;
			this.EffectCurRound += 1;
			var frozenString = "The " + opponent.Name + " is frozen. Any damage to it will be double!";
			output.StoreUserOutput(
				Helper.FormatAttackSuccessText(),
				Helper.FormatDefaultBackground(),
				frozenString);
			if (this.EffectCurRound <= this.EffectMaxRound) return;
			this.IsEffectExpired = true;
		}
	}
}