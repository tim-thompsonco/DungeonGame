using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DungeonGame {
	public static class EffectOutput {
		public static UserOutput ShowEffects(Player player) {
			var output = new UserOutput();
			// Draw title to show for player effects
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				"Player Effects:");
			var activeEffects = 0;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var effect in player.Effects) {
				var effectOutput = "(" + (effect.EffectMaxRound + 1 - effect.EffectCurRound) + ") " + 
				                   textInfo.ToTitleCase(effect.Name);
				activeEffects++;
				output.StoreUserOutput(
					Helper.FormatGeneralInfoText(), 
					Helper.FormatDefaultBackground(), 
					effectOutput);
			}
			if (activeEffects == 0) {
				output.StoreUserOutput(
					Helper.FormatInfoText(),
					Helper.FormatDefaultBackground(),
					"None.");
			}
			// Create bottom border for effects area
			var effectsBorder = new StringBuilder();
			// Effects border needs to extend same width as minimap itself in either direction
			for (var b = 0; b < Helper.GetMiniMapBorderWidth(); b++) {
				effectsBorder.Append("=");
			}
			output.StoreUserOutput(
				Helper.FormatGeneralInfoText(), 
				Helper.FormatDefaultBackground(), 
				effectsBorder.ToString());
			return output;
		}
	}
}