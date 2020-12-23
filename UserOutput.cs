using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonGame.Controllers;

namespace DungeonGame {
	public class UserOutput {
		public List<List<string>> _Output { get; set; }

		public UserOutput() {
			_Output = new List<List<string>>();
		}

		public void StoreUserOutput(string textColorName, string backColorName, string outputData) {
			_Output.Add(new List<string> { textColorName, backColorName, outputData });
		}
		public void StoreUserOutput(List<string> outputLine) {
			_Output.Add(outputLine);
		}
		public void RetrieveUserOutput() {
			// Var i is iterating through each row of output
			foreach (List<string> line in _Output.ToList()) {
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
			for (int i = 0; i < OutputController.MapDisplay._Output.Count; i++) {
				int lineCount = 0;
				if (i < _Output.Count) {
					for (int c = 2; c < _Output[i].Count; c += 3) {
						lineCount += _Output[i][c].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int b = 0; b < bufferAmount; b++) {
					bufferStringBuilder.Append(" ");
				}
				if (i < _Output.Count) {
					_Output[i].Add(Settings.FormatGeneralInfoText());
					_Output[i].Add(Settings.FormatDefaultBackground());
					_Output[i].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var j is iterating through each column of each row of output
				for (int j = 0; j < OutputController.MapDisplay._Output[i].Count; j += 3) {
					_Output[i].Add(OutputController.MapDisplay?._Output[i][j]);
					_Output[i].Add(OutputController.MapDisplay?._Output[i][j + 1]);
					_Output[i].Add(OutputController.MapDisplay?._Output[i][j + 2]);
				}
			}
			// Var k is iterating through each row of output
			// Build effect display underneath map display
			int lc = 0;
			for (int k = OutputController.MapDisplay._Output.Count;
				k < OutputController.EffectDisplay._Output.Count + OutputController.MapDisplay._Output.Count; k++) {
				int lineCount = 0;
				if (k < _Output.Count) {
					for (int d = 2; d < _Output[k].Count; d += 3) {
						lineCount += _Output[k][d].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int e = 0; e < bufferAmount; e++) {
					bufferStringBuilder.Append(" ");
				}
				if (k < _Output.Count) {
					_Output[k].Add(Settings.FormatGeneralInfoText());
					_Output[k].Add(Settings.FormatDefaultBackground());
					_Output[k].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var l is iterating through each column of each row of output
				for (int l = 0; l < OutputController.EffectDisplay._Output[lc].Count; l += 3) {
					_Output[k].Add(OutputController.EffectDisplay?._Output[lc][l]);
					_Output[k].Add(OutputController.EffectDisplay?._Output[lc][l + 1]);
					_Output[k].Add(OutputController.EffectDisplay?._Output[lc][l + 2]);
					if (lc + 1 < OutputController.EffectDisplay._Output.Count) {
						lc++;
					}
				}
			}
			// Var m is iterating through each row of output
			// Build questlog display underneath effect display
			lc = 0;
			for (int m = OutputController.MapDisplay._Output.Count + OutputController.EffectDisplay._Output.Count;
				m < OutputController.EffectDisplay._Output.Count + OutputController.MapDisplay._Output.Count +
				OutputController.QuestDisplay._Output.Count; m++) {
				int lineCount = 0;
				if (m < _Output.Count) {
					for (int d = 2; d < _Output[m].Count; d += 3) {
						lineCount += _Output[m][d].Length;
					}
				}
				int bufferAmount = Settings.GetGameWidth() - lineCount + Settings.GetBufferGap();
				StringBuilder bufferStringBuilder = new StringBuilder();
				for (int e = 0; e < bufferAmount; e++) {
					bufferStringBuilder.Append(" ");
				}
				if (m < _Output.Count) {
					_Output[m].Add(Settings.FormatGeneralInfoText());
					_Output[m].Add(Settings.FormatDefaultBackground());
					_Output[m].Add(bufferStringBuilder.ToString());
				} else {
					StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var n is iterating through each column of each row of output
				for (int n = 0; n < OutputController.QuestDisplay._Output[lc].Count; n += 3) {
					_Output[m].Add(OutputController.QuestDisplay?._Output[lc][n]);
					_Output[m].Add(OutputController.QuestDisplay?._Output[lc][n + 1]);
					_Output[m].Add(OutputController.QuestDisplay?._Output[lc][n + 2]);
					if (lc + 1 < OutputController.QuestDisplay._Output.Count) {
						lc++;
					}
				}
			}
		}
		public void ClearUserOutput() {
			int removeLines = _Output.Count;
			for (int i = 0; i < removeLines; i++) {
				_Output.RemoveAt(removeLines - i - 1);
			}
		}
	}
}