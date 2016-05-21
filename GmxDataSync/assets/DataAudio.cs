using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GmxDataSync {
	class DataAudio:DataAsset {
		public long FilePos;
		public int FileSize;
		public override void Proc(DataReader buf) {
			FileSize = buf.ReadInt32();
			FilePos = buf.Position;
		}
		public override void Export(string path) {
			var r = File.Reader;
			var w = new BinaryWriter(new FileStream(path, FileMode.Create));
			r.Position = FilePos;
			int i = FileSize;
			while (--i >= 0) w.Write(r.ReadByte());
			w.Close();
		}
	}
}
