using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.IO;

namespace Off_EE
{
	public class Img
	{
		private Bitmap img = null;
		private Texture2D t2d = null;
		private int
			CheckWidth = 0,
			CheckHeight = 0;

		/// <summary>
		/// Throw an exception and log it in the console
		/// </summary>
		/// <param name="Error">The error</param>
		private void Throw(string Error)
		{
			Console.WriteLine(Error);
			throw new Exception(Error);
		}

		/// <summary>
		/// Create an img class
		/// </summary>
		/// <param name="FileLocation">The location of the png to read</param>
		/// <param name="Width">The width of the image</param>
		/// <param name="Height">The height of the image</param>
		public Img(string FileLocation, int Width, int Height)
		{
			if(File.Exists(FileLocation))
			{
				img = new Bitmap(Bitmap.FromFile(FileLocation));
				if(img.Width != Width)
				{
					Throw("The width the image is supposed to have is not what is expected.");
				}
			}
			else
			{
				Throw(string.Concat("File doesn't exist ", FileLocation));
			}
		}

		public Img(Bitmap pic)
		{
			img = pic;
		}

		/// <summary>
		/// Get the bitmap image
		/// </summary>
		public Bitmap BitPic
		{
			get
			{
				return img;
			} set {
				if(value.Width != CheckWidth)
				{
					throw new Exception("The width of the image is not equal to the width for checking.");
				}

				if(value.Height != CheckHeight)
				{
					throw new Exception("The height of the image is not equal to the height for checking.");
				}
			}
		}

		/// <summary>
		/// Get the texture2d version
		/// </summary>
		public Texture2D MonoPic
		{
			get
			{
				if (t2d == null)
				{
					t2d = BitmapToTexture2d.GetTexture(img);
				}

				return t2d;
			} set {
				throw new Exception("Can't set the image to a Texture2D. Please use a bitmap instead.");
			}
		}
	}
}
