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
				try {
					Console.WriteLine("Reading file...");
					var file = new DataFile(args[0]);
					Console.WriteLine("Extracting...");
					int total = file.Export(args[1]);
					file.Reader.Close();
					if (total > 0) {
						Console.WriteLine("Exported " + total + " asset" + (total != 1 ? "s" : "") + ".");
					} else Console.WriteLine("Nothing was exported - no matching assets?");
				} catch (Exception e) {
					Console.WriteLine("Error: " + e.ToString());
				}
			} else Console.WriteLine("Usage: GmxDataSync [data.win] [project directory path]");
			#if DEBUG
			Console.ReadKey();
			#endif
		}
	}
}
