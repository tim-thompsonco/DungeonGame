using System;
using System.Collections.Generic;
using System.Text;

namespace DungeonGame {
	public class UserOutput {
		public List<List<string>> Output { get; set; }

		public UserOutput() {
			this.Output = new List<List<string>>();
		}

		public void StoreUserOutput(string textColorName, string backColorName, string outputData) {
			this.Output.Add(new List<string> { textColorName, backColorName, outputData });
		}
		public void StoreUserOutput(List<string> outputLine) {
			this.Output.Add(outputLine);
		}
		public void RetrieveUserOutput() {
			// Var i is iterating through each row of output
			foreach (var line in this.Output) {
				// var j is iterating through each column of each row of output
				for (var i = 0; i < line.Count; i += 3) {
					var textColor = line[i];
					var backColor = line[i + 1];
					var textOutput = line[i + 2];
					Console.ForegroundColor = textColor switch {
						"black" => ConsoleColor.Black,
						"cyan" => ConsoleColor.Cyan,
						"gray" => ConsoleColor.Gray,
						"darkgray" => ConsoleColor.DarkGray,
						"darkcyan" => ConsoleColor.DarkCyan,
						"green" => ConsoleColor.Green,
						"yellow" => ConsoleColor.Yellow,
						"white" => ConsoleColor.White,
						"darkred" => ConsoleColor.DarkRed,
						"darkblue" => ConsoleColor.DarkBlue,
						"darkyellow" => ConsoleColor.DarkYellow,
						"red" => ConsoleColor.Red,
						"blue" => ConsoleColor.Blue,
						"magenta" => ConsoleColor.Magenta,
						_ => ConsoleColor.DarkGreen};
					Console.BackgroundColor = backColor switch {
						"black" => ConsoleColor.Black,
						"cyan" => ConsoleColor.Cyan,
						"gray" => ConsoleColor.Gray,
						"darkgray" => ConsoleColor.DarkGray,
						"darkcyan" => ConsoleColor.DarkCyan,
						"green" => ConsoleColor.Green,
						"yellow" => ConsoleColor.Yellow,
						"white" => ConsoleColor.White,
						"darkred" => ConsoleColor.DarkRed,
						"darkblue" => ConsoleColor.DarkBlue,
						"darkyellow" => ConsoleColor.DarkYellow,
						"red" => ConsoleColor.Red,
						"blue" => ConsoleColor.Blue,
						"magenta" => ConsoleColor.Magenta,
						_ => ConsoleColor.DarkGreen};
					// Print each line of output, if less than game width, add buffer space to output line
					Console.Write(textOutput);
				}
				Console.WriteLine();
			}
		}
		public void BuildUserOutput() {
			// Var i is iterating through each row of output
			for (var i = 0; i < OutputHandler.MapDisplay.Output.Count; i++) {
				var lineCount = 0;
				if (i < this.Output.Count) {
					for (var c = 2; c < this.Output[i].Count; c += 3) {
						lineCount += this.Output[i][c].Length;
					}
				}
				var bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				var bufferStringBuilder = new StringBuilder();
				for (var b = 0; b < bufferAmount; b++) {
					bufferStringBuilder.Append(" ");
				}
				if (i < this.Output.Count) {
					this.Output[i].Add(Settings.FormatGeneralInfoText());
					this.Output[i].Add(Settings.FormatDefaultBackground());
					this.Output[i].Add(bufferStringBuilder.ToString());
				}
				else {
					this.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var j is iterating through each column of each row of output
				for (var j = 0; j < OutputHandler.MapDisplay.Output[i].Count; j += 3) {
					this.Output[i].Add(OutputHandler.MapDisplay?.Output[i][j]);
					this.Output[i].Add(OutputHandler.MapDisplay?.Output[i][j + 1]);
					this.Output[i].Add(OutputHandler.MapDisplay?.Output[i][j + 2]);
				}
			}
			var lc = 0;
			// Var k is iterating through each row of output
			for (var k = OutputHandler.MapDisplay.Output.Count; k < OutputHandler.EffectDisplay.Output.Count + OutputHandler.MapDisplay.Output.Count; k++) {
				var lineCount = 0;
				if (k < this.Output.Count) {
					for (var d = 2; d < this.Output[k].Count; d += 3) {
						lineCount += this.Output[k][d].Length;
					}
				}
				var bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				var bufferStringBuilder = new StringBuilder();
				for (var e = 0; e < bufferAmount; e++) {
					bufferStringBuilder.Append(" ");
				}
				if (k < this.Output.Count) {
					this.Output[k].Add(Settings.FormatGeneralInfoText());
					this.Output[k].Add(Settings.FormatDefaultBackground());
					this.Output[k].Add(bufferStringBuilder.ToString());
				}
				else {
					this.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var j is iterating through each column of each row of output
				for (var l = 0; l < OutputHandler.EffectDisplay.Output[lc].Count; l += 3) {
					this.Output[k].Add(OutputHandler.EffectDisplay?.Output[lc][l]);
					this.Output[k].Add(OutputHandler.EffectDisplay?.Output[lc][l + 1]);
					this.Output[k].Add(OutputHandler.EffectDisplay?.Output[lc][l + 2]);
					if (lc + 1 < OutputHandler.EffectDisplay.Output.Count) lc++;
				}
			}
		}
		public void ClearUserOutput() {
			var removeLines = this.Output.Count;
			for (var i = 0; i < removeLines; i++) {
				this.Output.RemoveAt(removeLines - i - 1);
			}
		}
	}
}