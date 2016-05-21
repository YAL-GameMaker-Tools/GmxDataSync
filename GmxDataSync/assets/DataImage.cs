using System.Drawing;

namespace GmxDataSync {
	class DataImage:DataAsset {
		public ushort Left;
		public ushort Top;
		public ushort Width;
		public ushort Height;
		public ushort OffsetX;
		public ushort OffsetY;
		public ushort TextureId;
		public Bitmap Image;
		public override void Proc(DataReader buf) {
			Left = buf.ReadUInt16();
			Top = buf.ReadUInt16();
			Width = buf.ReadUInt16();
			Height = buf.ReadUInt16();
			OffsetX = buf.ReadUInt16();
			OffsetY = buf.ReadUInt16();
			buf.Position += sizeof(ushort) * 4;
			TextureId = buf.ReadUInt16();
		}
		public Bitmap ToImage() {
			if (Image == null) {
				DataTexture tex = File.Textures[TextureId];
				Bitmap bmp = new Bitmap(Width, Height);
				Graphics gfx = Graphics.FromImage(bmp);
				gfx.DrawImage(tex.Image, -Left, -Top);
				Image = bmp;
			}
			return Image;
		}
		public override void Export(string path) {
			ToImage().Save(path);
		}
	}

}
