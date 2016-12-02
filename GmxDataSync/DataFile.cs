﻿using System;
using System.Collections.Generic;
using System.IO;

namespace GmxDataSync {
	class DataFile {
		public DataReader Reader;
		public Dictionary<string, long> Chunks = new Dictionary<string, long>();
		public DataTexture[] Textures;
		public DataImage[] Images;
		public Dictionary<uint, DataImage> ImageMap = new Dictionary<uint, DataImage>();
		public DataSprite[] Sprites;
		public DataFont[] Fonts;
		public DataBackground[] Backgrounds;
		public DataAudio[] AudioFiles;
		public DataSound[] Sounds;
		public DataFile(string path) {
			Reader = new DataReader(path);
			while (Reader.Position < Reader.DataEnd) {
				string chType = new string(Reader.ReadChars(4));
				uint chSize = Reader.ReadUInt32();
				Chunks[chType] = Reader.Position;
				Reader.Position += chSize;
			}
			Textures = LoadAssets<DataTexture>("TXTR");
			Images = LoadAssets<DataImage>("TPAG");
			foreach (var q in Images) ImageMap[(uint)q.Position] = q;
			Sprites = LoadAssets<DataSprite>("SPRT");
			Fonts = LoadAssets<DataFont>("FONT");
			Backgrounds = LoadAssets<DataBackground>("BGND");
			AudioFiles = LoadAssets<DataAudio>("AUDO");
			Sounds = LoadAssets<DataSound>("SOND");
		}
		public T[] LoadAssets<T>(string chunkName) where T : DataAsset, new() {
			Reader.Position = Chunks[chunkName];
			uint count = Reader.ReadUInt32();
			T[] arr = new T[count];
			for (uint i = 0; i < count; i++) {
				T item = new T();
				item.File = this;
				item.Index = i;
				item.Position = Reader.ReadUInt32();
				arr[i] = item;
			}
			foreach (T item in arr) {
				Reader.Position = item.Position;
				item.Proc(Reader);
			}
			return arr;
		}
		private int ExportAssets<T>(T[] arr, string path, string word) where T : DataAsset {
			if (!Directory.Exists(path)) return 0;
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
			Console.WriteLine("done.");
			return total;
		}
		private static void EnsureDirectory(string path) {
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		}
		public int Export(string path) {
			int total = 0, sections = 0;
			if (Directory.Exists(path + "/sprites")) {
				EnsureDirectory(path + "/sprites/images");
				total += ExportAssets(Sprites, path + "/sprites", "sprite");
				sections += 1;
			}
			if (Directory.Exists(path + "/background")) {
				EnsureDirectory(path + "/background/images");
				total += ExportAssets(Backgrounds, path + "/background", "background");
				sections += 1;
			}
			int fonts = ExportAssets(Fonts, path + "/fonts", "font");
			if (fonts > 0) {
				total += fonts;
				sections += 1;
			}
			if (Directory.Exists(path + "/sound")) {
				EnsureDirectory(path + "/sound/audio");
				total += ExportAssets(Sounds, path + "/sound", "sound");
				sections += 1;
			}
			if (sections == 0) total = -1;
			return total;
		}
	}

}
