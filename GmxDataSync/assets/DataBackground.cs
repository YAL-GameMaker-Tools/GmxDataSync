using System;
using System.Collections.Generic;
using System.Text;

namespace GmxDataSync {
	class DataBackground:DataAsset {
		public uint ImagePos;
		public override void Proc(DataReader buf) {
			Name = buf.ReadRefCString();
			buf.Position += sizeof(int) * 3;
			ImagePos = buf.ReadUInt32();
		}
		public override bool Export(string path) {
			string name = File.Remap(File.BackgroundMap, Index, Name);
			if (DataFile.ForceExport || System.IO.File.Exists(path + "/" + name + ".background.gmx")) {
				File.ImageMap[ImagePos].Export(path + "/images/" + name + ".png");
				return true;
			} else {
				return false;
			}
		}
	}
}
