using DungeonGame.Effects;
using System.Collections.Generic;

namespace DungeonGame.Interfaces {
	public interface IEffectHolder {
		List<IEffect> Effects { get; set; }
	}
}
