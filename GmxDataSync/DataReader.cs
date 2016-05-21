using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GmxDataSync {
	class DataReader:BinaryReader {
		public DataReader(string path) : base(new FileStream(path, FileMode.Open)) {
			if (ReadInt32() != 0x4d524f46) {
				throw new IOException("`" + path + "` doesn't look like a valid datafile.");
			}
			DataEnd = ReadUInt32();
			DataEnd += Position;
		}
		public long DataEnd;
		public long Position {
			get {
				return BaseStream.Position;
			}
			set {
				BaseStream.Position = value;
			}
		}
		public string ReadCString() {
			long pos = Position;
			while (Position < DataEnd) {
				if (ReadByte() == 0) break;
			}
			long len = Position - pos - 1;
			Position = pos;
			string str = new string(ReadChars((int)len));
			ReadByte();
			return str;
		}
		public string ReadRefCString() {
			long pos = ReadUInt32();
			long old = Position;
			Position = pos;
			string r = ReadCString();
			Position = old;
			return r;
		}
		public uint[] ReadUInt32Array() {
			uint count = ReadUInt32();
			uint[] arr = new uint[count];
			for (int i = 0; i < count; i++) {
				arr[i] = ReadUInt32();
			}
			return arr;
		}
	}
}
