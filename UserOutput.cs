using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

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
		public void RetrieveUserOutput(UserOutput mapOutput) {
			// Var i is iterating through each row of output
			for (var i = 0; i < mapOutput.Output.Count; i++) {
				var lineCount = 0;
				if (i < this.Output.Count) {
					for (var c = 2; c < this.Output[i].Count; c += 3) {
						lineCount += this.Output[i][c].Length;
					}
				}
				var bufferAmount = Helper.GetGameWidth() - lineCount + Helper.GetBufferGap();
				var bufferStringBuilder = new StringBuilder();
				for (var b = 0; b < bufferAmount; b++) {
					bufferStringBuilder.Append(" ");
				}
				if (i < this.Output.Count) {
					this.Output[i].Add(Helper.FormatGeneralInfoText());
					this.Output[i].Add(Helper.FormatDefaultBackground());
					this.Output[i].Add(bufferStringBuilder.ToString());
				}
				else {
					this.StoreUserOutput(
						Helper.FormatGeneralInfoText(),
						Helper.FormatDefaultBackground(),
						bufferStringBuilder.ToString());
				}
				// var j is iterating through each column of each row of output
				for (var j = 0; j < mapOutput.Output[i].Count; j += 3) {
					this.Output[i].Add(mapOutput?.Output[i][j]);
					this.Output[i].Add(mapOutput?.Output[i][j + 1]);
					this.Output[i].Add(mapOutput?.Output[i][j + 2]);
				}
			}
			this.RetrieveUserOutput();
		}
	}
}