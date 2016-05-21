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
		public override void Export(string path) {
			if (System.IO.File.Exists(path + "/" + Name + ".font.gmx")) {
				File.ImageMap[ImagePos].Export(path + "/" + Name + ".png");
			}
		}
	}
}
