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
				Console.WriteLine("Supported options:");
				Console.WriteLine("-masks: Output collision mask visualizations together with sprites");
				Console.WriteLine("-force: Output files even if there aren't matching GMX files");
				Console.WriteLine("-noreplace: Don't replace existing files in output directory");
				readKey = true;
			} else if (args.Length == 1) {
				Console.WriteLine("No `path` argument - trying to export into directory where datafile is.");
				args = new string[] { args[0], Path.GetDirectoryName(Path.GetFullPath(args[0])), "-force" };
				readKey = true;
			}
			if (args.Length >= 2) {
				int i = 2;
				while (i < args.Length) {
					string arg = args[i++];
					switch (arg) {
						case "-masks": DataSprite.ExportMasksOn = true; break;
						case "-force": DataFile.ForceExport = true; break;
						case "-noreplace": DataFile.NoReplace = true; break;
						default: Console.WriteLine("`" + arg + "` is not a known argument."); break;
					}
				}
#if !DEBUG
				try {
#endif
					Console.WriteLine("Reading file...");
					var file = new DataFile(args[0]);
					//
					string map = args[1] + "/gdsync.txt";
					if (File.Exists(map)) {
						file.LoadMap(map);
					}
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
#if !DEBUG
				} catch (Exception e) {
					Console.WriteLine("Runtime error: " + e.ToString());
					Console.WriteLine("The datafile may be broken or corrupt.");
					readKey = true;
				}
#endif
			}
			//
			if (readKey) {
				Console.Write("Press any key to exit...");
				Console.ReadKey();
			}

		}
	}
}
