using System.Drawing;

namespace GmxDataSync {
	class DataImage:DataAsset {
		// Offset of source region inside texture page
		public ushort SrcLeft, SrcTop;
		// Size of source region to draw
		public ushort SrcWidth, SrcHeight;
		// Offset of source image inside destination image
		public ushort DstLeft, DstTop;
		// Same as SrcWidth/SrcHeight for all observed cases
		public ushort DstWidth, DstHeight;
		// Actual output dimensions
		public ushort OutWidth, OutHeight;
		// Texture page ID to clip from
		public ushort TextureId;
		// Cached image
		public Bitmap Image;
		public override void Proc(DataReader buf) {
			SrcLeft = buf.ReadUInt16();
			SrcTop = buf.ReadUInt16();
			SrcWidth = buf.ReadUInt16();
			SrcHeight = buf.ReadUInt16();
			DstLeft = buf.ReadUInt16();
			DstTop = buf.ReadUInt16();
			DstWidth = buf.ReadUInt16();
			DstHeight = buf.ReadUInt16();
			OutWidth = buf.ReadUInt16();
			OutHeight = buf.ReadUInt16();
			TextureId = buf.ReadUInt16();
		}
		public Bitmap ToImage() {
			if (Image == null) {
				DataTexture tex = File.Textures[TextureId];
				Bitmap bmp = new Bitmap(OutWidth, OutHeight);
				Graphics gfx = Graphics.FromImage(bmp);
				Rectangle srcRect = new Rectangle(SrcLeft, SrcTop, SrcWidth, SrcHeight);
				Rectangle dstRect = new Rectangle(DstLeft, DstTop, DstWidth, DstHeight);
				gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				if (DstWidth > SrcWidth || DstHeight > SrcHeight) {
					Bitmap srcBmp = new Bitmap(SrcWidth, SrcHeight);
					Graphics.FromImage(srcBmp).DrawImage(tex.Image, 0, 0, srcRect, GraphicsUnit.Pixel);
					gfx.DrawImage(srcBmp, dstRect);
				} else gfx.DrawImage(tex.Image, dstRect, srcRect, GraphicsUnit.Pixel);
				Image = bmp;
			}
			return Image;
		}
		public override bool Export(string path) {
			if (DataFile.NoReplace && System.IO.File.Exists(path)) return true;
			ToImage().Save(path);
			return true;
		}
	}

}
