using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace GmxDataSync {
	class Program {
		static void Main(string[] args) {
			if (args.Length >= 2) {
				int i = 2;
				while (i < args.Length) {
					string arg = args[i++];
					switch (arg) {
					case "-masks": DataSprite.ExportMasksOn = true; break;
					default: Console.WriteLine("`" + arg + "` is not a known argument."); break;
					}
				}
				var file = new DataFile(args[0]);
				file.Export(args[1]);
				file.Reader.Close();
				Console.WriteLine("Done.");
			} else Console.WriteLine("Usage: GmxDataSync [data.win] [project directory path]");
			Console.ReadLine();
		}
	}
}
