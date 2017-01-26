using Microsoft.Xna.Framework.Graphics;

namespace Off_EE
{
	public static class BitmapToTexture2d
	{
		public static GraphicsDevice graphicsDevice;

		public static Texture2D GetTexture(System.Drawing.Bitmap b)
		{
			Texture2D tx = null;
			using (System.IO.MemoryStream s = new System.IO.MemoryStream())
			{
				b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
				s.Seek(0, System.IO.SeekOrigin.Begin);
				tx = Texture2D.FromStream(graphicsDevice, s);
			}
			return tx;
		}
	}
}
