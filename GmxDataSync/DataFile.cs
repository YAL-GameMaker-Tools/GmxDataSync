using System;
using System.Collections.Generic;
using System.IO;

namespace GmxDataSync {
	class DataFile {
		// options:
		public static bool ForceExport = false;
		public static bool NoReplace = false;
		//
		public DataReader Reader;
		// raw data:
		public Dictionary<string, long> Chunks = new Dictionary<string, long>();
		public DataTexture[] Textures;
		public DataImage[] Images;
		public DataAudio[] AudioFiles;
		public Dictionary<uint, DataImage> ImageMap = new Dictionary<uint, DataImage>();
		// assets:
		public DataSprite[] Sprites;
		public DataBackground[] Backgrounds;
		public DataFont[] Fonts;
		public DataSound[] Sounds;
		// remap:
		public Dictionary<uint, string> SpriteMap = new Dictionary<uint, string>();
		public Dictionary<uint, string> BackgroundMap = new Dictionary<uint, string>();
		public Dictionary<uint, string> FontMap = new Dictionary<uint, string>();
		public Dictionary<uint, string> SoundMap = new Dictionary<uint, string>();
		public string Remap(Dictionary<uint, string> map, uint index, string name) {
			if (map.ContainsKey(index)) {
				return map[index];
			} else return name;
		}
		//
		public DataFile(string path) {
			Reader = new DataReader(path);
			while (Reader.Position < Reader.DataEnd) {
				string chType = new string(Reader.ReadChars(4));
				uint chSize = Reader.ReadUInt32();
				Chunks[chType] = Reader.Position;
				Reader.Position += chSize;
			}
			Textures = LoadAssets<DataTexture>("TXTR", "texture");
			Images = LoadAssets<DataImage>("TPAG");
			foreach (var q in Images) ImageMap[(uint)(q.Position - Reader.DataStart)] = q;
			Sprites = LoadAssets<DataSprite>("SPRT");
			Fonts = LoadAssets<DataFont>("FONT");
			Backgrounds = LoadAssets<DataBackground>("BGND");
			AudioFiles = LoadAssets<DataAudio>("AUDO");
			Sounds = LoadAssets<DataSound>("SOND");
		}
		public bool LoadMap(string path) {
			try {
				var reader = File.OpenText(path); string line;
				Dictionary<uint, string> map = null;
				string type = "none";
				while ((line = reader.ReadLine()) != null) {
					int last = line.Length - 1;
					int pos = line.IndexOf(':');
					if (pos == last) {
						type = line.Substring(0, last);
						switch (type) {
							case "sprites": map = SpriteMap; break;
							case "backgrounds": map = BackgroundMap; break;
							case "sounds": map = SoundMap; break;
							case "fonts": map = FontMap; break;
							default: throw new Exception("Unknown section type `" + type + "`.");
						}
					} else if (pos >= 0) {
						uint index;
						if (uint.TryParse(line.Substring(0, pos).Trim(), out index)) {
							map[index] = line.Substring(pos + 1).Trim();
						}
					}
				}
				return true;
			} catch (Exception e) {
				Console.WriteLine("Error parsing remap file: " + e);
				return false;
			}
		}
		public T[] LoadAssets<T>(string chunkName, string pg = null) where T : DataAsset, new() {
			Reader.Position = Chunks[chunkName];
			uint count = Reader.ReadUInt32();
			T[] arr = new T[count];
			if (pg != null) Console.Write(string.Format("Reading {0} {1}{2}: ", count, pg, count != 1 ? "s" : ""));
			for (uint i = 0; i < count; i++) {
				T item = new T();
				item.File = this;
				item.Index = i;
				item.Position = Reader.DataStart + Reader.ReadUInt32();
				arr[i] = item;
			}
			foreach (T item in arr) {
				Reader.Position = item.Position;
				if (pg != null) {
					string si = item.Index.ToString();
					Console.Write(si);
					item.Proc(Reader);
					Console.Write("".PadRight(si.Length, '\x08'));
				} else item.Proc(Reader);
			}
			if (pg != null) Console.WriteLine("done.");
			return arr;
		}
		private int ExportAssets<T>(T[] arr, string path, string word) where T : DataAsset {
			if (!ForceExport && !Directory.Exists(path)) return 0;
			int n = arr.Length;
			if (n <= 0) return 0;
			int total = 0;
			Console.Write("Exporting " + n + " " + word + (n != 1 ? "s" : "") + ": ");
			for (int i = 0; i < n; i++) {
				string si = "" + i;
				Console.Write(si);
				if (arr[i].Export(path)) total += 1;
				Console.Write("".PadRight(si.Length, '\x08'));
			}
			Console.WriteLine("done (" + total + " match" + (total != 1 ? "es" : "") + ").");
			return total;
		}
		private static void EnsureDirectory(string path) {
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		}
		public int Export(string path) {
			int total = 0, sections = 0;
			if (ForceExport || Directory.Exists(path + "/sprites")) {
				EnsureDirectory(path + "/sprites/images");
				total += ExportAssets(Sprites, path + "/sprites", "sprite");
				sections += 1;
			}
			if (ForceExport || Directory.Exists(path + "/background")) {
				EnsureDirectory(path + "/background/images");
				total += ExportAssets(Backgrounds, path + "/background", "background");
				sections += 1;
			}
			if (ForceExport || Directory.Exists(path + "/fonts")) {
				if (ForceExport) EnsureDirectory(path + "/fonts");
				total += ExportAssets(Fonts, path + "/fonts", "font");
				sections += 1;
			}
			if (ForceExport || Directory.Exists(path + "/sound")) {
				EnsureDirectory(path + "/sound/audio");
				total += ExportAssets(Sounds, path + "/sound", "sound");
				sections += 1;
			}
			if (sections == 0) total = -1;
			return total;
		}
	}

}
