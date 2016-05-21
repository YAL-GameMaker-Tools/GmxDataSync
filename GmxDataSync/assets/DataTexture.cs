using System.Drawing;
using System.IO;

namespace GmxDataSync {
	class DataTexture:DataAsset {
		public Bitmap Image;
		public override void Proc(DataReader buf) {
			buf.ReadUInt32();
			long pngPos = buf.ReadUInt32();
			buf.Position = pngPos + 8;
			while (buf.Position < buf.DataEnd) {
				int chSize = 0;
				chSize |= (buf.ReadByte() << 24);
				chSize |= (buf.ReadByte() << 16);
				chSize |= (buf.ReadByte() << 8);
				chSize |= buf.ReadByte();
				int chType = buf.ReadInt32();
				if (chType == 0x444E4549) break;
				buf.Position += chSize + 4;
			}
			long pngSize = buf.Position + 4 - pngPos;
			buf.Position = pngPos;
			byte[] pngBytes = buf.ReadBytes((int)pngSize);
			Image = new Bitmap(new MemoryStream(pngBytes));
		}
		public override bool Export(string path) {
			Image.Save(path);
			return true;
		}
	}

}
