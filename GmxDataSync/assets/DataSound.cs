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
		public override bool Export(string path) {
			string name = File.Remap(File.SoundMap, Index, Name);
			if (AudioId >= 0 && AudioId < File.AudioFiles.Length
			&& (DataFile.ForceExport || System.IO.File.Exists(path + "/" + name + ".sound.gmx"))) {
				string epath = path + "/audio/";
				if (Name != name) {
					epath += FileName.Replace(Name, name);
				} else epath += FileName;
				File.AudioFiles[AudioId].Export(epath);
				return true;
			} else {
				return false;
			}
		}
	}
}
