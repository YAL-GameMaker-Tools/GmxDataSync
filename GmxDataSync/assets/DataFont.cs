using System;
using System.Collections.Generic;
using System.Text;

namespace GmxDataSync {
	class DataFont:DataAsset {
		public uint ImagePos;
		public override void Proc(DataReader buf) {
			Name = buf.ReadRefCString();
			buf.Position += sizeof(int) * 6;
			ImagePos = buf.ReadUInt32();
		}
		public override bool Export(string path) {
			string name = File.Remap(File.FontMap, Index, Name);
			if (DataFile.ForceExport || System.IO.File.Exists(path + "/" + name + ".font.gmx")) {
				File.ImageMap[ImagePos].Export(path + "/" + name + ".png");
				return true;
			} else {
				return false;
			}
		}
	}
}
