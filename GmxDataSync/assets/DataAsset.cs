using System;
using System.Collections.Generic;
using System.Text;

namespace GmxDataSync {
	class DataAsset {
		public DataFile File;
		public string Name;
		public uint Index;
		public long Position;
		public DataAsset() { }
		public virtual void Proc(DataReader buf) { }
		public virtual void Export(string path) { }
	}
}
