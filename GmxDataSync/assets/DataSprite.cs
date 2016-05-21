using System.Drawing;

namespace GmxDataSync {
	class DataSprite:DataAsset {
		public int Width;
		public int Height;
		public uint[] ImagePos;
		public int MaskNum;
		public long MaskPos;
		public override void Proc(DataReader buf) {
			Name = buf.ReadRefCString();
			Width = buf.ReadInt32();
			Height = buf.ReadInt32();
			buf.Position += sizeof(uint) * 11;
			ImagePos = buf.ReadUInt32Array();
			MaskNum = buf.ReadInt32();
			MaskPos = buf.Position;
		}
		public static bool ExportMasksOn = false;
		private void ExportMasks(string path) {
			var buf = File.Reader;
			buf.Position = MaskPos;
			for (int i = 0; i < MaskNum; i++) {
				Bitmap bmp = new Bitmap(Width, Height);
				for (int y = 0; y < Height; y++) {
					byte bits = 0;
					for (int x = 0; x < Width; x++) {
						if ((x & 7) == 0) bits = buf.ReadByte();
						if ((bits & (128 >> (x & 7))) != 0) {
							bmp.SetPixel(x, y, Color.Black);
						}
					}
				}
				bmp.Save(path + "/images/" + Name + "_" + i + ".mask.png");
			}
		}
		public override bool Export(string path) {
			if (!System.IO.File.Exists(path + "/" + Name + ".sprite.gmx")) return false;
			for (int i = 0; i < ImagePos.Length; i++) {
				DataImage img = File.ImageMap[ImagePos[i]];
				string next = path + "/images/" + Name + "_" + i + ".png";
				if (img.Width != Width || img.Height != Height) {
					Bitmap bmp = new Bitmap(Width, Height);
					Graphics gfx = Graphics.FromImage(bmp);
					gfx.DrawImage(img.ToImage(), img.OffsetX, img.OffsetY);
					bmp.Save(next);
				} else img.ToImage().Save(next);
			}
			if (ExportMasksOn) ExportMasks(path);
			return true;
		}
	}
}
