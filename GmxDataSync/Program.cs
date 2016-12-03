using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace GmxDataSync {
	class Program {
		static void Main(string[] args) {
			bool readKey = false;
			#if DEBUG
			readKey = true;
			#endif
			//
			if (args.Length == 0) {
				Console.WriteLine("Usage: GmxDataSync [data.win] [project directory path]");
				readKey = true;
			} else if (args.Length == 1) {
				Console.WriteLine("No `path` argument - trying to export into directory where datafile is.");
				args = new string[] { args[0], Path.GetDirectoryName(Path.GetFullPath(args[0])) };
				readKey = true;
			}
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
					//
					string map = Path.GetDirectoryName(args[0]) + "/gdsync.txt";
					if (File.Exists(map)) file.LoadMap(map);
					//
					Console.WriteLine("Extracting...");
					int total = file.Export(args[1]);
					file.Reader.Close();
					if (total > 0) {
						Console.WriteLine("Exported " + total + " asset" + (total != 1 ? "s" : "") + ".");
					} else if (total == 0) {
						Console.WriteLine("Nothing was exported - no shared assets between datafile and project.");
						Console.WriteLine("Is it the right project as such?");
					} else {
						Console.WriteLine("Directory `" + args[1] + "` does not to seem to contain any project files.");
						readKey = true;
					}
				} catch (Exception e) {
					Console.WriteLine("Runtime error: " + e.ToString());
					Console.WriteLine("The datafile may be broken or corrupt.");
					readKey = true;
				}
			}
			//
			if (readKey) {
				Console.Write("Press any key to exit...");
				Console.ReadKey();
			}

		}
	}
}
