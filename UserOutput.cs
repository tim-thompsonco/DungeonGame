using DungeonGame.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public class UserOutput {
		public List<List<string>> Output { get; set; }

		public UserOutput() {
			Output = new List<List<string>>();
		}

		public void StoreUserOutput(string textColorName, string backColorName, string outputData) {
			Output.Add(new List<string> { textColorName, backColorName, outputData });
		}
		public void StoreUserOutput(List<string> outputLine) {
			Output.Add(outputLine);
		}
		public void RetrieveUserOutput() {
			// Var i is iterating through each row of output
			foreach (List<string> line in Output.ToList()) {
				// var j is iterating through each column of each row of output
				for (int i = 0; i < line.Count; i += 3) {
					string textColor = line[i];
					string backColor = line[i + 1];
					string textOutput = line[i + 2];
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
						_ => ConsoleColor.DarkGreen
					};
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
						_ => ConsoleColor.DarkGreen
					};
					// Print each line of output, if less than game width, add buffer space to output line
					Console.Write(textOutput);
				}
				Console.WriteLine();
			}
		}
		public void BuildUserOutput() {
			// Var i is iterating through each row of output
			for (int i = 0; i < OutputController.MapDisplay.Output.Count; i++) {
				int lineCount = 0;
				if (i < Output.Count) {
					for (int c = 2; c < Output[i].Count; c += 3) {
						lineCount += Output[i][c].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int b = 0; b < bufferAmount; b++) {
					bufferStringBuilder.Append(" ");
				}
				if (i < Output.Count) {
					Output[i].Add(Settings.FormatGeneralInfoText());
					Output[i].Add(Settings.FormatDefaultBackground());
					Output[i].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var j is iterating through each column of each row of output
				for (int j = 0; j < OutputController.MapDisplay.Output[i].Count; j += 3) {
					Output[i].Add(OutputController.MapDisplay?.Output[i][j]);
					Output[i].Add(OutputController.MapDisplay?.Output[i][j + 1]);
					Output[i].Add(OutputController.MapDisplay?.Output[i][j + 2]);
				}
			}
			// Var k is iterating through each row of output
			// Build effect display underneath map display
			int lc = 0;
			for (int k = OutputController.MapDisplay.Output.Count;
				k < OutputController.EffectDisplay.Output.Count + OutputController.MapDisplay.Output.Count; k++) {
				int lineCount = 0;
				if (k < Output.Count) {
					for (int d = 2; d < Output[k].Count; d += 3) {
						lineCount += Output[k][d].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int e = 0; e < bufferAmount; e++) {
					bufferStringBuilder.Append(" ");
				}
				if (k < Output.Count) {
					Output[k].Add(Settings.FormatGeneralInfoText());
					Output[k].Add(Settings.FormatDefaultBackground());
					Output[k].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var l is iterating through each column of each row of output
				for (int l = 0; l < OutputController.EffectDisplay.Output[lc].Count; l += 3) {
					Output[k].Add(OutputController.EffectDisplay?.Output[lc][l]);
					Output[k].Add(OutputController.EffectDisplay?.Output[lc][l + 1]);
					Output[k].Add(OutputController.EffectDisplay?.Output[lc][l + 2]);
					if (lc + 1 < OutputController.EffectDisplay.Output.Count) {
						lc++;
					}
				}
			}
			// Var m is iterating through each row of output
			// Build questlog display underneath effect display
			lc = 0;
			for (int m = OutputController.MapDisplay.Output.Count + OutputController.EffectDisplay.Output.Count;
				m < OutputController.EffectDisplay.Output.Count + OutputController.MapDisplay.Output.Count +
				OutputController.QuestDisplay.Output.Count; m++) {
				int lineCount = 0;
				if (m < Output.Count) {
					for (int d = 2; d < Output[m].Count; d += 3) {
						lineCount += Output[m][d].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int e = 0; e < bufferAmount; e++) {
					bufferStringBuilder.Append(" ");
				}
				if (m < Output.Count) {
					Output[m].Add(Settings.FormatGeneralInfoText());
					Output[m].Add(Settings.FormatDefaultBackground());
					Output[m].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var n is iterating through each column of each row of output
				for (int n = 0; n < OutputController.QuestDisplay.Output[lc].Count; n += 3) {
					Output[m].Add(OutputController.QuestDisplay?.Output[lc][n]);
					Output[m].Add(OutputController.QuestDisplay?.Output[lc][n + 1]);
					Output[m].Add(OutputController.QuestDisplay?.Output[lc][n + 2]);
					if (lc + 1 < OutputController.QuestDisplay.Output.Count) {
						lc++;
					}
				}
			}
		}
		public void ClearUserOutput() {
			int removeLines = Output.Count;
			for (int i = 0; i < removeLines; i++) {
				Output.RemoveAt(removeLines - i - 1);
			}
		}
	}
}