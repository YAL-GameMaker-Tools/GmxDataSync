using System;
using System.Collections.Generic;
using System.Text;

namespace GmxDataSync {
	class DataSound:DataAsset {
		public string FileName;
		public int AudioId;
		public override void Proc(DataReader buf) {
			Name = buf.ReadRefCString();
			buf.Position += sizeof(int) * 2;
			FileName = buf.ReadRefCString();
			buf.Position += sizeof(int) * 4;
			AudioId = buf.ReadInt32();
		}
		public override void Export(string path) {
			if (AudioId >= 0 && AudioId < File.AudioFiles.Length
			&& System.IO.File.Exists(path + "/" + Name + ".sound.gmx")) {
				File.AudioFiles[AudioId].Export(path + "/audio/" + FileName);
			}
		}
	}
}
